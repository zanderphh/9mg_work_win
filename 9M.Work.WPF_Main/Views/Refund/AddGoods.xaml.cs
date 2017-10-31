using _9M.Work.DbObject;
using _9M.Work.ErpApi;
using _9M.Work.Model;
using _9M.Work.TopApi;
using _9M.Work.Utility;
using _9M.Work.WPF_Common;
using _9M.Work.WPF_Common.ValueObjects;
using _9M.Work.WPF_Main.Infrastrcture;
using _9Mg.Work.TopApi;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace _9M.Work.WPF_Main.Views.Refund
{
    /// <summary>
    /// Interaction logic for AddGoods.xaml
    /// </summary>
    public partial class AddGoods : UserControl, BaseDialog
    {

        BaseDAL dal = new BaseDAL();
        delegate void AsyncLoadHandler(string GjTradeNo, string RefundNo);
        public string golbal_GjTradeNo = null;
        public string golbal_RefundNo = null;
        public ObservableCollection<RefundDetailModel> Golbal_Grid_Source = new ObservableCollection<RefundDetailModel>();
        ShopModel golbal_ShopModel = null;

        #region 构造1
        /// <summary>
        /// 添加商品
        /// </summary>
        /// <param name="GjTradeNo">淘宝原订单号</param>
        /// <param name="RefundNo">退货单号</param>
        /// <param name="ismulti">是否多退款(多退款标记订单为异常单)</param>
        public AddGoods(string GjTradeNo, string RefundNo,ShopModel sm)
        {
            InitializeComponent();
            this.DataContext = this;
            golbal_RefundNo = RefundNo;
            golbal_ShopModel = sm;

            if (!string.IsNullOrEmpty(GjTradeNo))
            {
                golbal_GjTradeNo = GjTradeNo;
                loading.Visibility = System.Windows.Visibility.Visible;
                AsyncLoadHandler handler = new AsyncLoadHandler(Bind);
                handler.BeginInvoke(GjTradeNo, RefundNo, AsyncLoadHandlerCallback, null);
            }
            else
            {
                loading.Visibility = System.Windows.Visibility.Collapsed;
            }
        }




        void AsyncLoadHandlerCallback(IAsyncResult ir)
        {

            System.Windows.Application.Current.Dispatcher.InvokeAsync(new Action(() =>
            {
                try
                {
                    loading.Visibility = System.Windows.Visibility.Collapsed;
                }

                catch { }

            }));


        }

        #endregion

        #region 绑定商品列表
        public void Bind(string GjTradeNo, string RefundNo)
        {
            //管家的商品
            Golbal_Grid_Source = GetGJTradeByGjTradeNo(GjTradeNo, RefundNo);

            if (Golbal_Grid_Source.Count.Equals(0))
            {
                System.Windows.Application.Current.Dispatcher.InvokeAsync(new Action(() =>
                {
                    loadingText.Text = "未能获取到管家数据，稍后请重试...";
                    System.Threading.Thread.Sleep(2000);
                    loading.Visibility = System.Windows.Visibility.Collapsed;
                }));
            }

            else
            {

                //退货记录的商品
                var RefundGoods = !String.IsNullOrEmpty(RefundNo) ? GetRefundByRefundNo(RefundNo) : new List<RefundDetailModel>();

                //同一订单SKU分组键值对应
                var groupInfo = Golbal_Grid_Source.GroupBy(m => m.sku).ToList();

                foreach (var GJItem in groupInfo)
                {
                    //管家订单中同一SKU的数量(通常一个订单一个SKU为1件，但不排除有多件)
                    string GjSku = GJItem.Key;

                    //退货登记的SKU数量
                    var RegisterSku = RefundGoods.FindAll(a => a.sku.Equals(GjSku));

                    //同一SKU商品登记的数量和销售订单的数量不一致
                    IEnumerator<RefundDetailModel> ie = GJItem.GetEnumerator();

                    while (ie.MoveNext())
                    {
                        RefundDetailModel m = ie.Current;

                        if (RegisterSku.Count.Equals(0))
                        {
                            m.dColumn1 = "未添加";
                        }
                        else
                        {
                            //已登记生成"已添加"按钮，未登记的生成"添加"按钮
                            m.dColumn1 = (Golbal_Grid_Source.Where(a => a.sku.Equals(GjSku) && a.dColumn1.Equals("已添加")).Count() < RegisterSku.Count) ? "已添加" : "未添加";
                        }
                    }
                }

                var AlreadyAdd = Golbal_Grid_Source.Where(a => a.dColumn1.Equals("已添加"));


                var enumerator = AlreadyAdd.GetEnumerator();
                //赋值已添加的商品
                while (enumerator.MoveNext())
                {
                    RefundDetailModel model = RefundGoods.Where(a => a.id != enumerator.Current.id && a.sku.Equals(enumerator.Current.sku)).ToList()[0];
                    enumerator.Current.id = model.id;
                    enumerator.Current.isFinanceEnd = model.isFinanceEnd;
                    enumerator.Current.IsNotRegister = model.IsNotRegister;
                    enumerator.Current.realGoodsno = model.realGoodsno;
                    enumerator.Current.realSku = model.realSku;
                    enumerator.Current.refundNo = model.refundNo;
                    enumerator.Current.refundReason = model.refundReason;
                    enumerator.Current.remark = model.remark;
                    enumerator.Current.sku = model.sku;
                    enumerator.Current.specName = model.specName;
                    enumerator.Current.tbTradeNo = model.tbTradeNo;
                    enumerator.Current.unpackingEmployee = model.unpackingEmployee;
                    enumerator.Current.unpackingTime = model.unpackingTime;
                    enumerator.Current.categoryName = model.categoryName;
                    enumerator.Current.confirmReceipt = model.confirmReceipt;
                    enumerator.Current.expressCode = model.expressCode;
                    enumerator.Current.expressCompany = model.expressCompany;
                    enumerator.Current.gjTradeNo = model.gjTradeNo;
                    enumerator.Current.goodsno = model.goodsno;
                }


                RefundGoods.ForEach(delegate(RefundDetailModel rdm)
                {
                    var m = Golbal_Grid_Source.Where(a => a.sku.Equals(rdm.sku));

                    if (m == null)
                    {
                        rdm.dColumn1 = "已添加";
                    }
                });

                System.Windows.Application.Current.Dispatcher.InvokeAsync(new Action(() =>
                {
                    try
                    {
                        LVGoods.ItemsSource = Golbal_Grid_Source;
                    }

                    catch { }

                }));
            }

        }

        #endregion

        #region 获取管家订单商品
        /// <summary>
        /// 获取管家订单商品
        /// </summary>
        /// <param name="No"></param>
        ObservableCollection<RefundDetailModel> GetGJTradeByGjTradeNo(string No, string RefundNo)
        {
            ObservableCollection<RefundDetailModel> datasource = new ObservableCollection<RefundDetailModel>();

            //调用管家数据获取订单商品
            _9M.Work.ErpApi.GoodsManager gm = new ErpApi.GoodsManager();
            List<Erp_Model__GJTradeGoods> Erp_TradeGoods = gm.GetGJTradeGoods(No);

            //获取商品图片
            List<string> glist = new List<string>();

            if (Erp_TradeGoods != null)
            {
                Erp_TradeGoods.ForEach(delegate(Erp_Model__GJTradeGoods m)
                {
                    datasource.Add(new RefundDetailModel() { goodsno = m.goodsno, sku = m.tradegoodsno, specName = m.tradespec, dColumn1 = "", refundNo = RefundNo });
                    glist.Add(m.goodsno);
                });

                TopSource InvokeAPI = golbal_ShopModel!=null? new TopSource(golbal_ShopModel):new TopSource();
                List<Top.Api.Domain.Item> items = InvokeAPI.GetItemList(glist, "outer_id,pic_url");

                items.ForEach(delegate(Top.Api.Domain.Item i)
                {
                    IEnumerator<RefundDetailModel> ie = datasource.Where(a => a.goodsno.Equals(i.OuterId)).GetEnumerator();

                    while (ie.MoveNext())
                    {
                        ie.Current.imgUrl = i.PicUrl + "_120x120.jpg";
                    }

                });
            }

            return datasource;
        }

        #endregion

        #region 获取退货单商品

        /// <summary>
        /// 获取退货单商品
        /// </summary>
        /// <param name="No"></param>
        List<RefundDetailModel> GetRefundByRefundNo(string No)
        {
            List<ExpressionModelField> listWhere = new List<ExpressionModelField>();
            if (!string.IsNullOrEmpty(No)) { listWhere.Add(new ExpressionModelField() { Name = "refundNo", Value = No }); }
            List<RefundDetailModel> datasource = dal.GetList<RefundDetailModel>(listWhere.ToArray(), new OrderModelField[] { new OrderModelField() { PropertyName = "id", IsDesc = true } });
            return datasource;
        }
        #endregion

        #region 编辑/添加
        /// <summary>
        ///  编辑/添加
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_handler(object sender, RoutedEventArgs e)
        {
            RefundDetailModel m = LVGoods.SelectedItem as RefundDetailModel;
            _9M.Work.WPF_Main.Infrastrcture.FormInit.OpenDialog(this, new EditInfo(m, typeof(AddGoods)), true, 3);
        }

        #endregion

        #region 添加其它款

        private void btn_addOther(object sender, RoutedEventArgs e)
        {
            if (txtOtherGoods.Text.Equals(""))
            {
                MessageBox.Show("请输入商品编号SKU", "提示");
            }
            else
            {
                SessionUserModel user = CommonLogin.CommonUser;
                // OnlineStoreAuthorization AuthorizationModel = user.CShop;
                TopSource com = new TopSource();
                string Goodsno = GoodsHelper.GoodsNoOrSpecCode(txtOtherGoods.Text, true);

                List<string> list = new List<string>();
                list.Add(Goodsno.ToUpper());
                var rtnVal = com.GetItemList(list, "pic_url,Skus");

                GoodsManager manager = new GoodsManager();
                GoodsModel model = manager.GetGoodsAll(txtOtherGoods.Text.Trim());

                if (model != null)
                {
                    RefundDetailModel rdm = new RefundDetailModel()
                    {
                        sku = txtOtherGoods.Text.ToUpper().Trim(),
                        imgUrl = rtnVal.Count > 0 ? rtnVal[0].PicUrl + "_120x120.jpg" : "",
                        refundNo = golbal_RefundNo,
                        confirmReceipt = (int)ReceiptStatus.yes,
                        isFinanceEnd = false,
                        goodsno = Goodsno,
                        specName = model.SpecName,
                        dColumn1 = "未添加"
                    };

                    Golbal_Grid_Source.Add(rdm);
                    LVGoods.ItemsSource = Golbal_Grid_Source;
                }
                else
                {
                    MessageBox.Show("本次连接未获取商品信息，稍后再试", "提示");
                }


            }
        }

        #endregion

        #region Dialog
        public Microsoft.Practices.Prism.Commands.DelegateCommand CancelCommand
        {
            get { return new DelegateCommand(CloseDialog); }
        }

        public void CloseDialog()
        {
            #region 刷新父窗体

            List<Type> UserWindow = new List<Type>() { typeof(RegisterRefund), typeof(UnpackingCheck) };

            foreach (Type t in UserWindow)
            {
                if (t.Equals(typeof(RegisterRefund)))
                {
                    RegisterRefund rr = ((RegisterRefund)FormInit.FindFather(this, typeof(RegisterRefund)));
                    if (rr != null)
                    {
                        rr.RefreshBind();
                        break;
                    }

                }
                else if (t.Equals(typeof(UnpackingCheck)))
                {
                    UnpackingCheck uc = ((UnpackingCheck)FormInit.FindFather(this, typeof(UnpackingCheck)));
                    if (uc != null)
                    {
                        uc.dataBindByRefresh(golbal_RefundNo);
                        break;
                    }
                }
            }

            #endregion

            FormInit.CloseDialog(this, 2);
        }

        public string Title
        {
            get { return "添加商品"; }
        }

        public bool IsNavigationTarget(Microsoft.Practices.Prism.Regions.NavigationContext navigationContext)
        {
            throw new NotImplementedException();
        }

        public void OnNavigatedFrom(Microsoft.Practices.Prism.Regions.NavigationContext navigationContext)
        {
            throw new NotImplementedException();
        }

        public void OnNavigatedTo(Microsoft.Practices.Prism.Regions.NavigationContext navigationContext)
        {
            throw new NotImplementedException();
        }

        #endregion

        private void txtScan_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (txtScan.Text.Trim().Equals(""))
                {
                    MessageBox.Show("扫描的商品编号不能为空", "提示");
                    return;
                }
                else
                {
                    ObservableCollection<RefundDetailModel> query_collection = new ObservableCollection<RefundDetailModel>();
                    IEnumerator<RefundDetailModel> ie = Golbal_Grid_Source.Where(a => a.sku.ToUpper().Contains(txtScan.Text.Trim().ToUpper())).GetEnumerator();
                    while (ie.MoveNext())
                    {
                        query_collection.Add(ie.Current);
                    }
                    if (query_collection.Count > 0)
                    {
                        LVGoods.ItemsSource = query_collection;
                    }
                    else
                    {
                        MessageBox.Show("未搜索到此商品", "提示");
                    }
                }
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (ckShow.IsChecked.Equals(true))
            {
                LVGoods.ItemsSource = Golbal_Grid_Source;
            }
        }
    }
}
