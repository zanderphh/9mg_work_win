using _9M.Work.DbObject;
using _9M.Work.Model;
using _9M.Work.TopApi;
using _9M.Work.Utility;
using _9M.Work.WPF_Common;
using _9M.Work.WPF_Common.ValueObjects;
using _9M.Work.WPF_Main.Infrastrcture;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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

namespace _9M.Work.WPF_Main.Views.FinanceRefund
{
    /// <summary>
    /// Interaction logic for FinanceRegister.xaml
    /// </summary>
    public partial class FinanceRegister : UserControl
    {

        private BaseDAL dal = new BaseDAL();
        //店铺appKey\sessionkey\appSecret 对应的信息
        List<ShopModel> shopList = null;
        TopSource com = null;

        private FinanceRefundModel frModel { get; set; }

        #region 构造

        public FinanceRegister(FinanceRefundModel editModel, OperationStatus status)
        {
            InitializeComponent();
            this.DataContext = this;
            CauseBind();
            ShopComboxBind();
            if (editModel != null)
            {
                frModel = editModel;
                ExpressionModelField[] field = { new ExpressionModelField() { Name = "id", Value = Convert.ToInt32(editModel.id) } };
                List<FinanceRefundModel> list = dal.GetList<FinanceRefundModel>(field.ToArray());
                if (list != null)
                {
                    if (list.Count.Equals(1))
                    {
                        tbNo = frModel.tbNo;
                        tbNick = frModel.tbNick;
                        cmbFinanceCause.Text = frModel.cause;
                        alipay = frModel.alipay;
                        remark = frModel.remark;
                        tbRemark = frModel.tbRemark;
                        cash = frModel.cash;
                        coupon = frModel.coupon;
                        ckIsBackRefund.IsChecked = frModel.isBackOperator;
                        backMoney = frModel.backMoney;
                        shopIndexSelected = frModel.shopid;
                        com = new TopSource(shopList.Find(a=>a.shopId.Equals(shopIndexSelected)));
                    }
                }
            }
        }

        #endregion

        #region 属性

        public int _shopIndexSelected = 1000;
        public int shopIndexSelected
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

        private string _tbNo = "";
        public string tbNo
        {
            get { return _tbNo; }
            set
            {
                if (_tbNo != value)
                {
                    _tbNo = value;
                    this.OnPropertyChanged("tbNo");
                }
            }
        }

        private string _tbNick = "";
        public string tbNick
        {
            get { return _tbNick; }
            set
            {
                if (_tbNick != value)
                {
                    _tbNick = value;
                    this.OnPropertyChanged("tbNick");
                }
            }
        }


        private string _alipay = "";
        public string alipay
        {
            get { return _alipay; }
            set
            {
                if (_alipay != value)
                {
                    _alipay = value;
                    this.OnPropertyChanged("alipay");
                }
            }
        }

        private decimal _coupon = 0;
        public decimal coupon
        {
            get { return _coupon; }
            set
            {
                if (_coupon != value)
                {
                    _coupon = value;
                    this.OnPropertyChanged("coupon");
                }
            }
        }

        private decimal _cash = 0;
        public decimal cash
        {
            get { return _cash; }
            set
            {
                if (_cash != value)
                {
                    _cash = value;
                    this.OnPropertyChanged("cash");
                }
            }
        }

        private string _tbRemark = "";

        public string tbRemark
        {
            get { return _tbRemark; }
            set
            {
                if (_tbRemark != value)
                {
                    _tbRemark = value;
                    this.OnPropertyChanged("tbRemark");
                }
            }
        }

        private string _remark = "";
        public string remark
        {
            get { return _remark; }
            set
            {
                if (_remark != value)
                {
                    _remark = value;
                    this.OnPropertyChanged("remark");
                }
            }
        }

        private decimal? _backMoney = 0;
        public decimal? backMoney
        {
            get { return _backMoney; }
            set
            {
                if (_backMoney != value)
                {
                    _backMoney = value;
                    this.OnPropertyChanged("backMoney");
                }
            }
        }

        #endregion

        #region 回车匹配
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

                }
            }
        }
        #endregion

        #region 匹配信息

        delegate Hashtable AsyncInvokeViewInfoBindHandle();

        void AsyncInvokeViewInfoBindHandle_Callback(IAsyncResult ar)
        {

            System.Windows.Application.Current.Dispatcher.InvokeAsync(new Action(() =>
            {
                loading.Visibility = System.Windows.Visibility.Collapsed;
            }));

            AsyncInvokeViewInfoBindHandle handler = (AsyncInvokeViewInfoBindHandle)((AsyncResult)ar).AsyncDelegate;
            Hashtable result = handler.EndInvoke(ar);
            foreach (DictionaryEntry item in result)
            {
                if (item.Key.Equals(1))
                {
                    MessageBox.Show(item.Value.ToString());
                }
            }
        }

        private Hashtable ViewInfoBind()
        {
            Hashtable h = new Hashtable();
            if (!string.IsNullOrEmpty(tbNo))
            {
                string sql = string.Format("select tbNo from dbo.T_FinanceRefund where ([status]=@Groupleader or [status]=@Finance) and tbNo=@tbNo");
                SqlParameter[] param = { 
                                           new SqlParameter() { ParameterName = "@Groupleader", Value = (int)FinanceRefundEnum.Groupleader },
                                           new SqlParameter() { ParameterName = "@Finance", Value = (int)FinanceRefundEnum.Finance },
                                           new SqlParameter() { ParameterName = "@tbNo", Value = tbNo.Trim() },
                                       };
                DataTable dt = dal.QueryDataTable(sql, param);
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        h.Add(1, "此前已登记此单,但尚未处理完成，请完成后再次登记");
                        return h;
                    }
                }


                //调用管家数据获取原始单号
                _9M.Work.ErpApi.GoodsManager gm = new ErpApi.GoodsManager();
                //根据店铺获取相应的授权信息
                com = new TopSource(shopList.Find(a => a.shopId.Equals(shopIndexSelected)));
                Trade t = com.GetTradeInfoByCShopOrTmall(tbNo);
                if (t != null)
                {
                    tbRemark = t.SellerMemo;
                    tbNick = t.BuyerNick;
                }
                else
                {
                    h.Add(1, "请检查Sessionkey是否过期");
                }
            }
            else
            {
                h.Add(1, "淘宝编号不能为空！");
            }

            return h;

        }




        private void btn_InvokeApiMatch(object sender, RoutedEventArgs e)
        {
            loading.Visibility = System.Windows.Visibility.Visible;
            AsyncInvokeViewInfoBindHandle handle = new AsyncInvokeViewInfoBindHandle(ViewInfoBind);
            handle.BeginInvoke(AsyncInvokeViewInfoBindHandle_Callback, null);

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
            get { return "快速退款登记"; }
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

        #region 退获原因绑定(枚举)
        public void CauseBind()
        {
            _9M.Work.WPF_Common.WpfBind.ComboBoxBind.BindEnum(cmbFinanceCause, typeof(FinanceRefundCauseEnum), true);
        }
        #endregion

        #region 店铺绑定
        public void ShopComboxBind()
        {
            shopList = dal.GetAll<ShopModel>();
            shopCombox.ItemsSource = shopList;
        }

        #endregion

        private void btn_save(object sender, RoutedEventArgs e)
        {
            if (txtcoupon.Text.Trim() == "" )
            {
                coupon = 0;
            }
            if (txtcash.Text.Trim() == "")
            {
                cash = 0;
            }
            if (txtBackMoney.Text.Trim() == "")
            {
                backMoney = 0;
            }

            if (tbNo.Trim().Length.Equals(0) || tbNick.Trim().Length.Equals(0) || cmbFinanceCause.SelectedValue.Equals(0))
            {
                MessageBox.Show("数据未填写完整!");
                return;
            }
            if (StringHelper.IsPrice(cash.ToString()) == false || StringHelper.IsPrice(coupon.ToString()) == false)
            {
                MessageBox.Show("优惠券或金额必须为金额类型");
                return;
            }
            if (cash.Equals(0) && coupon.Equals(0) && ckIsBackRefund.IsChecked == false)
            {
                MessageBox.Show("优惠券&现金&后台直接退款必须选择其中一项！");
                return;
            }

            if (frModel == null)
            {
                frModel = new FinanceRefundModel();
            }

            frModel.tbNo = tbNo;
            frModel.tbNick = tbNick;
            frModel.cause = cmbFinanceCause.Text.ToString();
            frModel.alipay = alipay;
            frModel.regTime = DateTime.Now;
            frModel.regEmployee = CommonLogin.CommonUser.UserName;
            frModel.remark = remark;
            frModel.tbRemark = tbRemark;
            frModel.cash = cash;
            frModel.coupon = coupon;
            frModel.isBackOperator = ckIsBackRefund.IsChecked;
            frModel.shopid = shopIndexSelected;
            frModel.backMoney = backMoney;


            if (frModel.coupon > 0 || frModel.isBackOperator == true)
            {
                frModel.status = (int)FinanceRefundEnum.Groupleader;
            }
            else if (frModel.cash > 0)
            {
                frModel.status = (int)FinanceRefundEnum.Finance;
            }

            bool isSucceed = false;

            if (frModel.id != 0)
            {
                frModel.remark = frModel.remark + string.Format("{0}({1}{2})\r\n", "修改编辑", CommonLogin.CommonUser.UserName, DateTime.Now.ToString());

                isSucceed = dal.Update(frModel);
                if (tbRemark != "")
                { com.TradeMemoUpdate(tbRemark, Convert.ToInt64(tbNo)); }
            }
            else
            {
                isSucceed = dal.Add(frModel);
                if (tbRemark != "")
                { com.TradeMemoUpdate(tbRemark, Convert.ToInt64(tbNo)); }
            }

            if (isSucceed)
            {
                MessageBox.Show("提交成功！");
                FinanceRegsterList rl = ((FinanceRegsterList)FormInit.FindFather(this));
                rl.LoadRefundData(Convert.ToInt32(rl.CurrentPage), rl.PageSize);
                CloseDialog();
            }


        }
    }
}
