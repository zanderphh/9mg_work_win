using _9M.Work.DbObject;
using _9M.Work.Model;
using _9M.Work.Model.Report;
using _9M.Work.TopApi;
using _9M.Work.Utility;
using _9M.Work.WPF_Common;
using _9M.Work.WPF_Common.ValueObjects;
using _9M.Work.WPF_Main.FrameWork;
using _9M.Work.WPF_Main.Infrastrcture;
using _9Mg.Work.TopApi;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Remoting.Messaging;
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
using Top.Api.Domain;

namespace _9M.Work.WPF_Main.Views.Refund
{
    /// <summary>
    /// Interaction logic for RegisterRefund.xaml
    /// </summary>
    public partial class RegisterRefund : UserControl, BaseDialog
    {
        #region 属性

        private PrintHelper ph = new PrintHelper();

        private ObservableCollection<RefundDetailModel> _rdCollection = new ObservableCollection<RefundDetailModel>();

        public ObservableCollection<RefundDetailModel> rdCollection
        {
            get { return _rdCollection; }
            set
            {
                if (_rdCollection != value)
                {
                    _rdCollection = value;
                    this.OnPropertyChanged("rdCollection");
                }
            }
        }

        public string _tShopName;
        public string tShopName
        {
            get { return _tShopName; }
            set
            {
                if (_tShopName != value)
                {
                    _tShopName = value;
                    this.OnPropertyChanged("tShopName");
                }
            }
        }
        public string _tTradeNo;
        public string tTradeNo
        {
            get { return _tTradeNo; }
            set
            {
                if (_tTradeNo != value)
                {
                    _tTradeNo = value;
                    this.OnPropertyChanged("tTradeNo");
                }
            }
        }
        public string _tExpressNo;
        public string tExpressNo
        {
            get { return _tExpressNo; }
            set
            {
                if (_tExpressNo != value)
                {
                    _tExpressNo = value;
                    this.OnPropertyChanged("tExpressNo");
                }
            }
        }
        public string _tNick;
        public string tNick
        {
            get { return _tNick; }
            set
            {
                if (_tNick != value)
                {
                    _tNick = value;
                    this.OnPropertyChanged("tNick");
                }
            }
        }
        public string _tRealName;

        public string tRealName
        {
            get { return _tRealName; }
            set
            {
                if (_tRealName != value)
                {
                    _tRealName = value;
                    this.OnPropertyChanged("tRealName");
                }
            }
        }
        public string _tMobile;
        public string tMobile
        {
            get { return _tMobile; }
            set
            {
                if (_tMobile != value)
                {
                    _tMobile = value;
                    this.OnPropertyChanged("tMobile");
                }
            }
        }
        public string _tRemark { get; set; }
        public string tRemark
        {
            get { return _tRemark; }
            set
            {
                if (_tRemark != value)
                {
                    _tRemark = value;
                    this.OnPropertyChanged("tRemark");
                }
            }
        }
        public Int64? _shopIndexSelected;
        public Int64? shopIndexSelected
        {
            get { return _shopIndexSelected; }
            set
            {
                if (_shopIndexSelected != value)
                {
                    _shopIndexSelected = value;
                    this.OnPropertyChanged("shopIndexSelected");
                }
            }
        }


        public DateTime SndTime
        {
            get;
            set;
        }
        #endregion

        #region 全局变量

        public List<ShopModel> shopList = null;

        /// <summary>
        /// 首次登记无API接口的平台(数据库未保存数据，登记退货单时手动添加的商品)
        /// </summary>
        public ObservableCollection<RefundDetailModel> Golbal_MTRegisterDataSource = new ObservableCollection<RefundDetailModel>();
        /// <summary>
        /// 退货单登记编号
        /// </summary>
        string Golbal_refundNo = string.Empty;
        /// <summary>
        /// 当前操作(添加/编辑)
        /// </summary>
        OperationStatus Golbal_OperatorStatus = OperationStatus.ADD;
        //数据访问
        BaseDAL dal = new BaseDAL();

        ErpApi.GoodsManager manager = new ErpApi.GoodsManager();

        Print.PrintLabel print = new Print.PrintLabel();

        bool isunpacking = CommonLogin.CommonUser.DeptId.Equals(2) ? true : false;//拆包部门登记的都直接转为异常单

        ShopModel sm = null;

        #endregion

        #region 异步委托

        delegate void DataBindHandler(string refundNo, OperationStatus status);

        #endregion

        #region 初始化
        public RegisterRefund(string refundNo, OperationStatus status)
        {
            InitializeComponent();
            this.DataContext = this;
            ShopComboxBind();
            Golbal_OperatorStatus = status;
            DataBindHandler handler = new DataBindHandler(BindDataSource);
            handler.BeginInvoke(refundNo, status, null, null);
            cmbExpressCompanybatch.SelectedValue = (int)ExpressCompanyEnum.DEFAULT;
            if (status == OperationStatus.ADD)
            {
                ckSelected.Visibility = System.Windows.Visibility.Collapsed;
                txtPostcode.Visibility = System.Windows.Visibility.Collapsed;
            }
            else if (status == OperationStatus.Edit)
            {
                ckSelected.Visibility = System.Windows.Visibility.Visible;
                txtPostcode.Visibility = System.Windows.Visibility.Visible;
            }
        }

        #endregion

        #region 异步绑定方法
        void BindDataSource(string refundNo, OperationStatus status)
        {
            System.Windows.Application.Current.Dispatcher.InvokeAsync(new Action(() =>
            {
                try
                {
                    if (status == OperationStatus.Edit) { dataBind(refundNo); GetRefundInfo(refundNo); }

                    if (refundNo != null)
                    {
                        Golbal_refundNo = refundNo;
                    }
                }
                catch { }
            }));
        }
        #endregion

        #region 退货商品明细数据绑定
        private void dataBind(string refundNo)
        {
            List<ExpressionModelField> listWhere = new List<ExpressionModelField>();

            if (!string.IsNullOrEmpty(refundNo)) { listWhere.Add(new ExpressionModelField() { Name = "refundNo", Value = refundNo }); }
            List<RefundDetailModel> list = dal.GetList<RefundDetailModel>(listWhere.ToArray(), new OrderModelField[] { new OrderModelField() { PropertyName = "id", IsDesc = true } });

            int sn = 1;

            #region 获取图片

            //获取商品图片
            //List<string> glist = new List<string>();
            //list.ForEach(a => glist.Add(a.goodsno));
            //_9Mg.Work.TopApi.Commodity InvokeAPI = new _9Mg.Work.TopApi.Commodity();
            //List<Top.Api.Domain.Item> items = InvokeAPI.GetItemList(glist, "outer_id,pic_url", true);


            //items.ForEach(delegate(Top.Api.Domain.Item i)
            //{
            //    RefundDetailModel rd = list.Find(a => a.goodsno.Equals(i.OuterId));
            //    //rd.imgUrl = i.PicUrl + "_120x120.jpg";
            //    rd.dColumn1 = sn.ToString();

            //    sn++;
            //    rdCollection.Add(rd);
            //});

            #endregion

            list.ForEach(delegate(RefundDetailModel rdm)
            {
                if (rdm.confirmReceipt.Equals((int)ReceiptStatus.yes) || (rdm.confirmReceipt.Equals((int)ReceiptStatus.exceptionEnd) && rdm.IsNotRegister.Equals(true)))
                {
                    rdm.confirmReceipt = 1;
                }
                else
                {
                    rdm.confirmReceipt = 0;
                }
                rdm.dColumn1 = sn.ToString();
                sn++;
                rdCollection.Add(rdm);
            });

        }

        #endregion

        #region 退货单主表信息

        void GetRefundInfo(string refundNo)
        {
            RefundModel rm = dal.GetSingle<RefundModel>(a => a.refundNo.Equals(refundNo));

            tExpressNo = rm.sendPostcode;
            tTradeNo = rm.tradeNo;
            tMobile = rm.mobile;
            tRealName = rm.RealName;
            tNick = rm.tbnick;
            shopIndexSelected = rm.shopId;
            tShopName = (shopCombox.ItemsSource as List<ShopModel>).Find(a => a.shopId.Equals(rm.shopId)).shopName;
        }

        #endregion

        #region 店铺绑定
        public void ShopComboxBind()
        {
            shopList = dal.GetAll<ShopModel>();
            ShopModel m = new ShopModel() { id = -1, shopId = -1, shopName = "所有店铺" };
            shopList.Insert(0, m);
            shopCombox.ItemsSource = shopList;
            shopCombox.SelectedItem = m;
        }

        #endregion

        #region 添加商品
        private void btn_AddGoods(object sender, RoutedEventArgs e)
        {

            _9M.Work.WPF_Main.Infrastrcture.FormInit.OpenDialog(this, new AddGoods(labTradeNo.Text.ToString(), Golbal_refundNo, sm), true, 2);
        }
        #endregion

        #region 编辑商品
        private void btn_Edit(object sender, RoutedEventArgs e)
        {
            RefundDetailModel rdm = Registerlist.SelectedItem as RefundDetailModel;
            _9M.Work.WPF_Main.Infrastrcture.FormInit.OpenDialog(this, new EditInfo(rdm, typeof(RegisterRefund)), true, 3);
        }

        #endregion

        #region 删除商品
        private void btn_Del(object sender, RoutedEventArgs e)
        {
            RefundDetailModel rdm = Registerlist.SelectedItem as RefundDetailModel;

            if (rdm.id.Equals(0))//未保存过的
            {
                ObservableCollection<RefundDetailModel> c = Registerlist.ItemsSource as ObservableCollection<RefundDetailModel>;
                var r = c.Where<RefundDetailModel>(a => a.sku.Equals(rdm.sku)).GetEnumerator();

                while (r.MoveNext())
                {
                    c.Remove(r.Current);
                    break;
                }

                Registerlist.ItemsSource = c;
            }
            else//直接删除
            {
                try
                {
                    List<string> sql = new List<string>();
                    sql.Add(string.Format("delete T_RefundDetail where id={0}", rdm.id));
                    sql.Add(SQLTxt.GetInserLogSql(CommonLogin.CommonUser.UserName, EnumHelper.GetEnumTextVal((int)RefundOperatorEnum.Delete, typeof(RefundOperatorEnum)), Golbal_refundNo, rdm.sku));
                    dal.ExecuteTransaction(sql, null);
                }
                catch
                {

                }

                dataBind(rdm.refundNo);
            }
        }
        #endregion

        #region  淘宝平台信息匹配(C店/天猫)
        public void InvoketbApi(ShopModel shop, List<Erp_Model_GJRefund> Erp_TradeInfo, int shopId)
        {
            System.Windows.Application.Current.Dispatcher.InvokeAsync(new Action(() =>
            {
                try
                {

                    TopSource com = new TopSource(shop);

                    //调用API获取退货理由/物流信息
                    List<Top.Api.Domain.Refund> InvokeRefundList = new List<Top.Api.Domain.Refund>();

                    List<Trade> tradeCollection = new List<Trade>();
                    tradeCollection = com.GetTradeInfoByCShopOrTmall(Erp_TradeInfo);



                    if (tradeCollection.Count > 0 && tradeCollection[0] != null)
                    {
                        //从API中获取信息wl0713
                        tMobile = tradeCollection[0].ReceiverMobile;
                        tRealName = tradeCollection[0].ReceiverName;
                        tNick = tradeCollection[0].BuyerNick;

                        //调用淘宝API后返回的退货信息
                        InvokeRefundList = com.GetRefundInfoByRefundId(Erp_TradeInfo[0].NickName);
                    }
                    else
                    {
                        MessageBox.Show("淘宝未返回数据", "提示");
                        return;
                    }


                    int sn = 1;//显示序号

                    //遍历返回的订单
                    tradeCollection.ForEach(delegate(Trade t)
                    {
                        if (!string.IsNullOrEmpty(t.SellerMemo))
                        {
                            txtRemark.Text += t.SellerMemo + ";";
                        }

                        //遍历订单的商品明细
                        t.Orders.ForEach(delegate(Order o)
                        {
                            //如果商品明细中退货状态为 WAIT_BUYER_RETURN_GOODS或者WAIT_SELLER_CONFIRM_GOODS状态，则记录保存
                            //WAIT_BUYER_RETURN_GOODS(卖家已经同意退款，等待买家退货) WAIT_SELLER_CONFIRM_GOODS(买家已经退货，等待卖家确认收货)
                            if (o.RefundStatus.Equals("WAIT_BUYER_RETURN_GOODS") || o.RefundStatus.Equals("WAIT_SELLER_CONFIRM_GOODS"))
                            {

                                //明细单个SKU的数量有多个时拆分多条记录保存
                                for (int i = 0; i < o.Num; i++)
                                {
                                    var r = InvokeRefundList.Find(a => a.Sku.Split('|')[0].Equals(o.SkuId));

                                    if (r != null)
                                    {
                                        //保存至本地数据源以便显示在列表中
                                        rdCollection.Add(new RefundDetailModel
                                        {
                                            imgUrl = o.PicPath + "_120x120.jpg",
                                            goodsno = o.OuterIid,
                                            sku = o.OuterSkuId,
                                            categoryName = o.Cid.ToString(),
                                            specName = o.SkuPropertiesName,
                                            tbTradeNo = t.Tid.ToString(),
                                            refundReason = (r != null) ? (shopIndexSelected.Equals(1000) || shopIndexSelected.Equals(1003)) ? RefundReasonByCshop(r.Reason) : RefundReasonByTmall(r.Reason) : 0,
                                            refundDesc = r != null ? r.Reason : "",
                                            confirmReceipt = isunpacking.Equals(true) ? 1 : 0,
                                            isFinanceEnd = false,
                                            IsNotRegister = false,
                                            remark = r.Desc,
                                            tbRefundId = r.RefundId,
                                            dColumn1 = sn.ToString()
                                        });

                                        sn++;
                                    }
                                }

                            }
                        });
                    });
                    Golbal_MTRegisterDataSource = rdCollection;

                    if (isunpacking)
                    {
                        //订单抓取时标记的蓝旗或有商品因卖家原因引起的
                        if (tradeCollection.Where(a => a.SellerFlag.Equals(4)).Count() > 0 || rdCollection.Where(a => a.refundReason.Equals(1)).Count() > 0)
                        {
                            MessageBox.Show("质量问题退货,请核实!", "提示");
                        }
                    }
                }
                catch (Exception err)
                { string msg = err.Message.ToString(); }

            }));

        }


        #endregion

        #region 根据文字返回退货买家/卖家原因
        int RefundReasonByCshop(string reason)
        {
            if (reason.Equals("褪色/掉色/缩水") || reason.Equals("颜色/款式/图案描述不符") || reason.Equals("假冒品牌") || reason.Equals("大小尺寸与商品描述不符")
               || reason.Equals("做工瑕疵") || reason.Equals("收到商品少件/破损或污渍") || reason.Equals("材质/面料与商品不符")
               || reason.Equals("生产日期/保质期描述不符") || reason.Equals("质量问题") || reason.Equals("主商品破损")
               || reason.Equals("配件破损") || reason.Equals("假冒品牌") || reason.Equals("尺寸问题") || reason.Equals("材质不符") || reason.Equals("卖家发错货")
                )
            {
                return (int)RefundReason.seller;
            }
            else
            {
                return (int)RefundReason.buyer;
            }
        }

        int RefundReasonByTmall(string reason)
        {
            if (reason.Equals("退运费") || reason.Equals("做工问题") || reason.Equals("缩水/褪色") || reason.Equals("气味问题")
                         || reason.Equals("大小/尺寸与商品描述不符") || reason.Equals("材质面料与商品不符") || reason.Equals("功能/效果与商品描述不符")
                         || reason.Equals("少件/漏发") || reason.Equals("卖家发错货") || reason.Equals("包装/商品破损/污渍") || reason.Equals("假冒品牌")
                         || reason.Equals("未按约定时间发货") || reason.Equals("发票问题")
                )
            {
                return (int)RefundReason.seller;
            }
            else
            {
                return (int)RefundReason.buyer;
            }
        }

        #endregion

        #region 分销平台信息匹配
        public void InvokeFXApi(ShopModel shop, List<Erp_Model_GJRefund> Erp_TradeInfo, int shopId)
        {

            System.Windows.Application.Current.Dispatcher.InvokeAsync(new Action(() =>
        {

            try
            {
                //调用淘宝API后返回的订单
                TopSource com = new TopSource(sm);
                List<PurchaseOrder> tradeCollectionByFxApi = com.GetTradeInfoByFX(Erp_TradeInfo);

                //调用API获取退货理由/物流信息
                List<Top.Api.Domain.RefundDetail> InvokeRefundList = new List<Top.Api.Domain.RefundDetail>();

                if (tradeCollectionByFxApi.Count > 0)
                {
                    if (tradeCollectionByFxApi[0] != null)
                    {
                        tNick = tradeCollectionByFxApi[0].DistributorUsername;//买家写分销商的呢称
                        //wl0713
                        tMobile = tradeCollectionByFxApi[0].Receiver.MobilePhone;//买家的手机电话
                        tRealName = tradeCollectionByFxApi[0].Receiver.Name;

                        //订单备注(如果多个订单则合并订单备注)
                        List<long> fxSubId = new List<long>();
                        tradeCollectionByFxApi.ForEach(delegate(PurchaseOrder po)//遍历采购单
                        {
                            if (!string.IsNullOrEmpty(po.SupplierMemo))
                            {
                                tRemark += po.SupplierMemo + ";";
                            }
                            po.SubPurchaseOrders.ForEach(delegate(SubPurchaseOrder spo)//遍历采购单的子单
                            {
                                if (spo.Status.Equals("TRADE_REFUNDING"))//退货中的订单
                                {
                                    fxSubId.Add(spo.FenxiaoId);
                                }
                            });
                        });

                        //调用淘宝API后返回的退货信息(退货理由/金额)
                        InvokeRefundList = com.GetRefundInfoByFX(fxSubId);

                        int sn = 1;//列表显示序号

                        //遍历已申请退款商品的信息
                        InvokeRefundList.ForEach(delegate(RefundDetail rd)
                        {
                            tradeCollectionByFxApi.ForEach(delegate(PurchaseOrder po)//遍历采购单
                            {
                                var subOrder = po.SubPurchaseOrders.Find(a => a.FenxiaoId.Equals(rd.SubOrderId));

                                if (subOrder != null)
                                {
                                    for (int i = 0; i < subOrder.Num; i++)
                                    {
                                        //获取图片(调用商品API)
                                        List<string> glist = new List<string>();
                                        glist.Add(subOrder.ItemOuterId);
                                        TopSource InvokeAPI = new TopSource(sm);
                                        List<Top.Api.Domain.Item> items = InvokeAPI.GetItemList(glist, "outer_id,pic_url");
                                        string pic_url = string.Empty;
                                        if (items.Count > 0)
                                        {
                                            pic_url = items[0].PicUrl + "_120x120.jpg";
                                        }

                                        //保存至本地数据源以便显示在列表中
                                        rdCollection.Add(new RefundDetailModel
                                        {
                                            imgUrl = pic_url,
                                            goodsno = subOrder.ItemOuterId,
                                            sku = subOrder.SkuOuterId,
                                            //categoryName = subOrder.
                                            specName = subOrder.SkuProperties,
                                            tbTradeNo = po.FenxiaoId.ToString(),
                                            refundReason = RefundReasonByCshop(rd.RefundReason),
                                            refundDesc = rd.RefundReason,
                                            confirmReceipt = isunpacking.Equals(true) ? 1 : 0,
                                            isFinanceEnd = false,
                                            IsNotRegister = false,
                                            remark = rd.RefundDesc,
                                            refundMoney = String.IsNullOrEmpty(subOrder.DistributorPayment) ? 0 : Convert.ToDecimal(subOrder.DistributorPayment),
                                            dColumn1 = sn.ToString(),
                                        });

                                        sn++;
                                    }


                                }
                            });
                        });

                        Golbal_MTRegisterDataSource = rdCollection;

                        if (isunpacking)
                        {
                            if (tradeCollectionByFxApi.FindAll(a => a.SupplierFlag.Equals(4)).Count() > 0 || rdCollection.Where(a => a.refundReason.Equals(1)).Count() > 0)
                            {
                                MessageBox.Show("质量问题退货,请核实!", "提示");
                            }
                        }
                    }
                }
            }
            catch (Exception err)
            {

                string msg = err.Message.ToString();

            }
        }));
        }




        #endregion

        #region 匹配信息

        #region 获取管家数据方法

        delegate List<Erp_Model_GJRefund> AsyncInvoke_ERP_Handle(string query_txt);

        delegate void AsyncInvoke_TOP_Handle(ShopModel shop, List<Erp_Model_GJRefund> Erp_TradeInfo, int shopId);

        void AsyncInvoke_TOP_Callback(IAsyncResult ar)
        {
            System.Windows.Application.Current.Dispatcher.InvokeAsync(new Action(() =>
                          {
                              loading.Visibility = System.Windows.Visibility.Collapsed;
                          }
                      ));
        }

        void AsyncInvoke_ERP_CallBack(IAsyncResult ar)
        {
            AsyncInvoke_ERP_Handle handler = (AsyncInvoke_ERP_Handle)((AsyncResult)ar).AsyncDelegate;
            List<Erp_Model_GJRefund> Erp_TradeInfo = handler.EndInvoke(ar);

            if (Erp_TradeInfo != null)
            {
                if (Erp_TradeInfo.Count > 0)
                {
                    try
                    {

                        System.Windows.Application.Current.Dispatcher.InvokeAsync(new Action(() =>
                        {
                            loadingText.Text = "管家数据已获取,正在处理淘宝数据";
                        }));

                        //调用淘宝API获取淘宝订单状态
                        OnlineStoreAuthorization AuthorizationModel = new OnlineStoreAuthorization();
                        SessionUserModel user = CommonLogin.CommonUser;

                        //根据店铺获取相应的授权信息
                        sm = shopList.Where(x => Convert.ToInt64(x.shopId).Equals(shopIndexSelected)).FirstOrDefault();

                        if (sm.shopId.Equals(1003))//分销
                        {
                            AsyncInvoke_TOP_Handle fx_handle = new AsyncInvoke_TOP_Handle(InvokeFXApi);
                            fx_handle.BeginInvoke(null, Erp_TradeInfo, 0, AsyncInvoke_TOP_Callback, null);
                        }
                        else if (sm.shopId.Equals(1030) || sm.shopId.Equals(1000) || sm.shopId.Equals(1022)||sm.shopId.Equals(1031))
                        {
                            //调用C店\副店\天猫\童装店
                            AsyncInvoke_TOP_Handle handle = new AsyncInvoke_TOP_Handle(InvoketbApi);
                            handle.BeginInvoke(sm, Erp_TradeInfo, 1, AsyncInvoke_TOP_Callback, null);
                        }
                        else
                        {
                            //爱侬俩芊 20170731 wl
                            AsyncInvoke_TOP_Handle handle = new AsyncInvoke_TOP_Handle(InvoketbApi);
                            handle.BeginInvoke(sm, Erp_TradeInfo, 1, AsyncInvoke_TOP_Callback, null);

                            ////爱侬俩芊
                            //System.Windows.Application.Current.Dispatcher.InvokeAsync(new Action(() =>
                            //{
                            //    loading.Visibility = System.Windows.Visibility.Collapsed;
                            //}));
                        }
                    }
                    catch (Exception err)
                    {
                        System.Windows.Application.Current.Dispatcher.InvokeAsync(new Action(() =>
                        {
                            MessageBox.Show(string.Format("错误提示:{0}", err.Message.ToString()));
                            loading.Visibility = System.Windows.Visibility.Collapsed;
                        }));

                    }
                }
                else
                {
                    System.Windows.Application.Current.Dispatcher.InvokeAsync(new Action(() =>
                    {
                        loading.Visibility = System.Windows.Visibility.Collapsed;
                    }));
                }
            }
            else
            {
                loading.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        List<Erp_Model_GJRefund> AsyncInvoke_ERP(string query_txt)
        {
            //调用管家数据获取原始单号
            _9M.Work.ErpApi.GoodsManager gm = new ErpApi.GoodsManager();
            List<Erp_Model_GJRefund> Erp_TradeInfo = gm.GetTBSourceTradeNo(query_txt);

            if (Erp_TradeInfo != null)
            {
                if (Erp_TradeInfo.Count > 0)
                {
                    tShopName = Erp_TradeInfo[0].ShopName;
                    tExpressNo = Erp_TradeInfo[0].SendExpressNo;
                    tTradeNo = Erp_TradeInfo[0].tbTradeNo;
                    shopIndexSelected = Erp_TradeInfo[0].ShopID;
                    SndTime = Erp_TradeInfo[0].SndTime;

                    //管家数据加密修改为从API获取信息wl0713
                    //tMobile = Erp_TradeInfo[0].Mobile;
                    //tRealName = Erp_TradeInfo[0].CName;
                    //tNick = Erp_TradeInfo[0].NickName;
                }
            }

            return Erp_TradeInfo;
        }



        #endregion

        private void btn_InvokeApiMatch(object sender, RoutedEventArgs e)
        {

            if (txtTE.Text.Trim().Equals("")) { MessageBox.Show("请输入管家单号/发货快递单号"); return; }

            //是否已经登记

            string sql = string.Format("select id from dbo.T_Refund where tradeNo like '%{0}%' or sendPostcode='{0}'", txtTE.Text.Trim());
            System.Data.DataTable dt = dal.QueryDataTable(sql, new object[] { });
            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    MessageBox.Show("此单已录入!");
                    Refundlist rl = ((Refundlist)FormInit.FindFather(this));
                    rl.txtPNC.Text = txtTE.Text.Trim();
                    rl.LoadRefundData(Convert.ToInt32(rl.CurrentPage), rl.PageSize);
                    CloseDialog();
                    return;
                }
            }

            if (rdCollection != null)
            {
                if (rdCollection.Count > 0)
                {
                    rdCollection.Clear();
                }
            }
            if (Golbal_MTRegisterDataSource != null)
            {
                if (Golbal_MTRegisterDataSource.Count > 0)
                {
                    Golbal_MTRegisterDataSource.Clear();
                }
            }
            loading.Visibility = System.Windows.Visibility.Visible;
            AsyncInvoke_ERP_Handle handle = new AsyncInvoke_ERP_Handle(AsyncInvoke_ERP);
            handle.BeginInvoke(txtTE.Text.Trim(), AsyncInvoke_ERP_CallBack, null);

        }

        #endregion

        #region 保存

        private void btn_save(object sender, RoutedEventArgs e)
        {
            //勾选已收到的包果(未知包果)
            if (ckSelected.IsChecked.Equals(true) && Golbal_OperatorStatus == OperationStatus.Edit)
            {
                if (txtPostcode.Text.Trim().Equals(""))
                {
                    MessageBox.Show("快递单号未填写", "提示");
                    return;
                }
                else
                {
                    List<UnknownGoodsModel> list = dal.GetList<UnknownGoodsModel>(new ExpressionModelField[] { new ExpressionModelField() { Name = "ExpressCode", Value = txtPostcode.Text.Trim() } });

                    if (list.Count > 0)
                    {
                        try
                        {
                            List<string> sql = new List<string>();
                            //退货明细引用快递单号并将收货状态改为1
                            sql.Add(string.Format(@"update T_RefundDetail set expressCode='{0}',expressCompany='{1}',confirmReceipt=1,unpackingEmployee='{3}',unpackingTime=getdate() where refundNo='{2}'",
                                                    list[0].ExpressCode,
                                                    list[0].ExpressCompany,
                                                    Golbal_refundNo,
                                                    CommonLogin.CommonUser.UserName));
                            //删除未知包裹记录的数据
                            sql.Add(string.Format("delete T_UnknownGoods where ExpressCode='{0}'", txtPostcode.Text.Trim()));
                            //更新退货表的状态
                            sql.Add(string.Format("update T_Refund set refundStatus={0},confirmAmount={2} where refundNo='{1}'",
                                    (int)RefundSatausEnum.financeOperator,
                                    Golbal_refundNo,
                                    (Registerlist.ItemsSource as List<RefundDetailModel>).Count
                                ));

                            sql.Add(SQLTxt.GetInserLogSql(CommonLogin.CommonUser.UserName, EnumHelper.GetEnumTextVal((int)RefundOperatorEnum.Edit_Cites, typeof(RefundOperatorEnum)), Golbal_refundNo, ""));

                            if (dal.ExecuteTransaction(sql, null))
                            {
                                MessageBox.Show("快递单号引用成功", "提示");
                                Refundlist rl = ((Refundlist)FormInit.FindFather(this));
                                rl.LoadRefundData(Convert.ToInt32(rl.CurrentPage), rl.PageSize);
                                CloseDialog();

                            }
                            else
                            {
                                MessageBox.Show("快递单号引用失败", "提示");
                            }
                        }
                        catch (Exception err)
                        {
                            MessageBox.Show("错误提示" + err.Message.ToString(), "提示");
                        }
                    }
                    else
                    {
                        MessageBox.Show("未找到此快递号的未知包裹", "提示");
                    }

                }
            }
            else
            {

                List<string> sql = new List<string>();

                string ExpressCompany = string.Empty;//获取未知包裹的快递公司
                string ExpressCode = string.Empty;//获取未知包裹的快递单号

                if (ckSelected.IsChecked.Equals(true))//是否勾选引用未知包裹
                {
                    if (txtPostcode.Text.Trim().Equals(""))
                    {
                        MessageBox.Show("快递单号未填写", "提示");
                        return;
                    }

                    List<UnknownGoodsModel> list = dal.GetList<UnknownGoodsModel>(new ExpressionModelField[] { new ExpressionModelField() { Name = "ExpressCode", Value = txtPostcode.Text.Trim() } });
                    if (list.Count > 0)
                    {
                        ExpressCompany = list[0].ExpressCompany;
                        ExpressCode = list[0].ExpressCode;
                        //删除未知包裹记录的数据
                        sql.Add(string.Format("delete T_UnknownGoods where ExpressCode='{0}'", txtPostcode.Text.Trim()));
                    }
                    else
                    {
                        MessageBox.Show("未找到未知包裹中的此快递单号", "提示");
                        return;
                    }
                }

                //数据库是否存在相同的记录
                var dbSource = dal.QueryDataTable<RefundModel>(string.Format("select * from t_Refund where shopId={0} and tradeNo='{1}' and sendPostcode='{2}'",
                   (int)shopIndexSelected, labTradeNo.Text.ToString(), labExpressNo.Text.ToString()));

                if (dbSource.Count > 0)
                {
                    MessageBox.Show("该登记已存在,勿须重复登记，如有需要可已登记中手动添加商品", "提示");
                    return;
                }

                //当前要记录退单的明细数据
                List<RefundDetailModel> rd_list = new List<RefundDetailModel>();

                System.Collections.IEnumerator ie = Registerlist.ItemsSource.GetEnumerator();

                while (ie.MoveNext())
                {
                    RefundDetailModel rdm = ie.Current as RefundDetailModel;
                    //rdm.expressCompany = ckSelected.IsChecked.Equals(true) ? ExpressCompany : "";
                    //rdm.expressCode = ckSelected.IsChecked.Equals(true) ? ExpressCode : "";
                    rd_list.Add(rdm);
                }

                if (rd_list.Count > 0)
                {
                    string RefundId = string.Empty;

                    if (dbSource.Count.Equals(0))
                    {
                        RefundId = SerialNoHelp.rtnRefundNo();

                        //退货单主表记录
                        sql.Add(string.Format(@"insert into T_Refund(refundNo,shopId,tradeNo,refundStatus,financeRefundStatus,tbnick,mobile,remark,RealName,regTime,regEmployee,refundAmount,sendPostcode,confirmAmount,sndTime) 
                                                                     values('{0}',{1},'{2}',{10},0,'{3}','{4}','{5}','{6}',getdate(),'{7}',{8},'{9}',{11},'{12}')",
                                                                     RefundId,
                                                                     shopIndexSelected,
                                                                     labTradeNo.Text.ToString(),
                                                                     txtNick.Text.Trim(),
                                                                     txtMobile.Text.Trim(),
                                                                     txtRemark.Text.Trim().Replace("'", ""),
                                                                     txtRealName.Text.Trim(),
                                                                     CommonLogin.CommonUser.UserName,
                                                                     rd_list.Count,
                                                                     labExpressNo.Text.ToString(),
                                                                     isunpacking.Equals(true) ? (int)RefundSatausEnum.refundException : (int)RefundSatausEnum.noUnpacking,
                                                                     isunpacking.Equals(true) ? rd_list.Count : 0,
                                                                     SndTime
                                                                     ));
                    }
                    else
                    {
                        RefundId = dbSource[0].refundNo;
                    }


                    string LogRemark = string.Empty;

                    //退货明细记录
                    rd_list.ForEach(delegate(RefundDetailModel r)
                    {

                        sql.Add(string.Format(@"  
                                            
                                                insert into T_RefundDetail(refundNo,goodsno,sku,specName,categoryName,tbTradeNo,gjTradeNo,refundReason,confirmReceipt,isFinanceEnd,imgUrl,IsNotRegister,
                                                                               remark,tbRefundId,refundMoney,refundDesc,expressCompany,expressCode,unpackingEmployee,unpackingTime) 
                                                                   values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}',{8},{9},'{10}',{11},'{12}',{13},{14},'{15}','{16}','{17}','{18}','{19}')  
                                                ",
                                                RefundId,
                                                r.goodsno,
                                                r.sku,
                                                r.specName,
                                                r.categoryName,
                                                r.tbTradeNo,
                                                r.gjTradeNo,
                                                r.refundReason,
                                                r.confirmReceipt.Equals(0) ? (isunpacking.Equals(true) ? (int)ReceiptStatus.no : (int)ReceiptStatus.watting) : (int)ReceiptStatus.yes,
                                                0,
                                                r.imgUrl,
                                                isunpacking.Equals(true) ? 1 : 0,
                                                r.remark != null ? r.remark.Replace("'", "") : "",
                                                r.tbRefundId == null ? 0 : r.tbRefundId,
                                                r.refundMoney == null ? 0 : r.refundMoney,
                                                r.refundDesc,
                                                r.expressCompany,
                                                r.expressCode,
                                                isunpacking.Equals(true) ? CommonLogin.CommonUser.UserName : "",
                                                isunpacking.Equals(true) ? DateTime.Now.ToString() : null
                                                ));



                        LogRemark += r.sku + ",";
                    });

                    if (Golbal_OperatorStatus == OperationStatus.ADD)
                    {
                        try
                        {
                            sql.Add(SQLTxt.GetInserLogSql(CommonLogin.CommonUser.UserName, EnumHelper.GetEnumTextVal((int)RefundOperatorEnum.Register, typeof(RefundOperatorEnum)), Golbal_refundNo, LogRemark));


                            if (dal.ExecuteTransaction(sql, null))
                            {
                                MessageBox.Show("登记保存成功！", "提示");
                                Refundlist rl = ((Refundlist)FormInit.FindFather(this));
                                rl.LoadRefundData(Convert.ToInt32(rl.CurrentPage), rl.PageSize);
                                CloseDialog();
                            }
                            else
                            {
                                MessageBox.Show("保存失败！", "提示");
                            }
                        }
                        catch (Exception err)
                        {
                            MessageBox.Show("保存失败,失败原因:" + err.Message.ToString(), "提示");
                        }

                    }
                    else if (Golbal_OperatorStatus == OperationStatus.Edit)
                    {

                    }
                }
            }
        }

        #endregion

        #region 刷新列表
        public void RefreshBind()
        {
            DataBindHandler handler = new DataBindHandler(BindDataSource);
            handler.BeginInvoke(Golbal_refundNo, Golbal_OperatorStatus, null, null);
        }

        #endregion

        #region 复制

        private void btn_copy(object sender, RoutedEventArgs e)
        {
            string tag = ((Button)sender).Tag.ToString();
            if (tag.Equals("ExpressNo"))
            {
                Clipboard.SetText(labExpressNo.Text);
            }
            else if (tag.Equals("tradeNo"))
            {
                Clipboard.SetText(labTradeNo.Text);
            }
            else if (tag.Equals("mobile"))
            {
                Clipboard.SetText(txtMobile.Text);
            }
            else if (tag.Equals("nick"))
            {
                Clipboard.SetText(txtNick.Text);
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
            FormInit.CloseDialog(this);
        }

        public string Title
        {
            get { return "退货录入"; }
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

        #region 打印标签
        private void btn_print(object sender, RoutedEventArgs e)
        {
            RefundDetailModel rdm = Registerlist.SelectedItem as RefundDetailModel;
            if (rdm != null)
            {
                try
                {
                    List<string> list = new List<string>() { rdm.sku };
                    ph.PrintLabel(list);
                }
                catch (Exception err)
                {
                    MessageBox.Show(string.Format("错误提示:{0}", err.Message.ToString()));
                }
            }
        }
        #endregion

        #region 复制
        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Clipboard.SetText(((TextBlock)sender).Text);
        }

        #endregion

        #region 搜索

        void searchRow()
        {
            if (txtSearch.Text.Trim() != "")
            {
                var gEnumerator = Registerlist.ItemsSource.GetEnumerator();
                int i = 0;
                while (gEnumerator.MoveNext())
                {
                    var isContains = (gEnumerator.Current as RefundDetailModel).sku.Contains(txtSearch.Text.Trim());

                    if (isContains)
                    {
                        var datasource = Registerlist.ItemsSource as ObservableCollection<RefundDetailModel>;
                        datasource.Move(i, 0);
                        Registerlist.SelectedIndex = 0;
                        break;
                    }
                    i++;
                }
            }
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                searchRow();
            }
        }

        private void btn_searchRow(object sender, RoutedEventArgs e)
        {
            searchRow();
        }

        #endregion

        #region 商品详情
        private void btn_info(object sender, RoutedEventArgs e)
        {
            try
            {
                RefundDetailModel selectedrow = Registerlist.SelectedItem as RefundDetailModel;
                //OnlineStoreAuthorization AuthorizationModel = _9M.Work.WPF_Common.CommonLogin.CommonUser.CShop;
                TopSource com = new TopSource(sm);
                List<string> outeridlist = new List<string>();
                if (!string.IsNullOrEmpty(selectedrow.goodsno))
                {
                    outeridlist.Add(selectedrow.goodsno);
                    var info = com.GetItemList(outeridlist, null);
                    if (info[0].NumIid > 0)
                    {
                        System.Diagnostics.Process.Start("https://item.taobao.com/item.htm?id=" + info[0].NumIid);
                    }
                }
                else
                {
                    MessageBox.Show("当前商品未记录商品款号,无法打开商详情", "提示");
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(string.Format("错误提示@{0}@", err.Message.ToString()), "提示");
            }
        }

        #endregion

        #region 是否收到
        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            var rdm = Registerlist.SelectedItem as RefundDetailModel;
            if (rdm != null)
            {
                if (rdm.confirmReceipt.Equals(0))
                {
                    rdm.confirmReceipt = 1;
                }
                else if (rdm.confirmReceipt.Equals(1))
                {
                    rdm.confirmReceipt = 0;
                }
            }
        }
        #endregion

        #region 批量添加快递

        private void btn_batchSave(object sender, RoutedEventArgs e)
        {

            ObservableCollection<RefundDetailModel> rdm = new ObservableCollection<RefundDetailModel>();

            string expressCompany = EnumHelper.GetEnumTextVal((int)cmbExpressCompanybatch.SelectedValue, typeof(ExpressCompanyEnum));
            string expressCode = txtPostCodebatch.Text.Trim();
            if (expressCode.Trim().Length > 0)
            {
                var ietor = rdCollection.GetEnumerator();

                while (ietor.MoveNext())
                {
                    ietor.Current.expressCompany = expressCompany;
                    ietor.Current.expressCode = expressCode;
                    rdm.Add(ietor.Current);
                }
            }
            rdCollection.Clear();
            rdCollection = rdm;
            Golbal_MTRegisterDataSource = rdCollection;
        }

        #endregion

        private void txtTE_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (txtTE.Text.Trim() == "")
                {
                    MessageBox.Show("匹配的关键字不能为空");
                    return;
                }
                else
                {
                    btn_InvokeApiMatch(sender, null);
                }
            }
        }

    }
}
