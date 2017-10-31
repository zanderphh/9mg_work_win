using _9M.Work.DbObject;
using _9M.Work.Model;
using _9M.Work.Utility;
using _9M.Work.WPF_Common;
using _9M.Work.WPF_Common.ValueObjects;
using _9M.Work.WPF_Main.Infrastrcture;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for EditInfo.xaml
    /// </summary>
    public partial class EditInfo : UserControl, BaseDialog
    {

        #region 全局变量&属性

        private EnumEntity _expressCompany = new EnumEntity();

        public EnumEntity expressCompany
        {
            get { return _expressCompany; }
            set
            {
                if (_expressCompany != value)
                {
                    _expressCompany = value;
                    this.OnPropertyChanged("expressCompany");
                }
            }
        }

        public int refundReason
        { get; set; }

        public string imgUrl
        { get; set; }


        public RefundDetailModel Golbal_RefundDetailModel
        { get; set; }


        public Type Golbal_Type
        { get; set; }

        public bool Golbal_IsUnpacking
        { get; set; }

        #endregion

        #region 构造

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="m">商品实体</param>
        public EditInfo(RefundDetailModel m, Type t)
        {
            InitializeComponent();
            this.DataContext = this;
            Golbal_Type = t;//调用本窗体的父级对象
            Golbal_RefundDetailModel = m;//商品明细数据
            refundReason = 0;//默认原因为七天无理由
            //当前操作是否为拆包部门
            Golbal_IsUnpacking = CommonLogin.CommonUser.DeptId.Equals(2) ? true : false;

            imgUrl = m.imgUrl != null ? m.imgUrl.Replace("120x120", "360x360") : "";//显示的商品图片
            if (!string.IsNullOrEmpty(m.unpackingEmployee))
            {
                var e = m.unpackingEmployee.Split(Environment.NewLine.ToCharArray());
                m.unpackingEmployee = e[0];
            }
            if (!string.IsNullOrEmpty(m.exceptionEmployee))
            {
                var e = m.exceptionEmployee.Split(Environment.NewLine.ToCharArray());
                m.exceptionEmployee = e[0];
            }

            //区分当前窗体是修改状态还是添加状态
            if (m != null)
            {
                refundReason = m.refundReason;
                txtDESC.Text = m.remark;
                txtExpressNo.Text = m.expressCode;
                txtGjNo.Text = m.tbTradeNo;

                if (string.IsNullOrEmpty(m.expressCompany))
                {
                    cmbExpressBind();
                }
                else
                {
                    expressCompany = new EnumEntity() { Text = m.expressCompany, Value = 99 };
                    List<EnumEntity> datasource = new List<EnumEntity>();
                    datasource.Add(expressCompany);
                    cbxExpressCompany.ItemsSource = datasource;
                }
            }
            else
            {
                cmbExpressBind();
            }

        }

        #endregion

        #region 快递绑定
        private void cmbExpressBind()
        {
            List<EnumEntity> datasource = EnumHelper.GetEnumList(typeof(ExpressCompanyEnum));
            cbxExpressCompany.ItemsSource = datasource;
            expressCompany = datasource.Find(a => a.Value.Equals(-1));
        }

        #endregion

        #region 保存

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_add(object sender, RoutedEventArgs e)
        {
            BaseDAL dal = new BaseDAL();

            //退款原因
            Golbal_RefundDetailModel.refundReason = refundReason;
            //快递公司
            if (expressCompany.Value != -1) { Golbal_RefundDetailModel.expressCompany = expressCompany.Text; }
            //快递单号
            Golbal_RefundDetailModel.expressCode = txtExpressNo.Text;
            //备注
            Golbal_RefundDetailModel.remark = txtDESC.Text;
            //管家单号
            Golbal_RefundDetailModel.tbTradeNo = txtGjNo.Text;

            #region 首次登记无API接口的平台(数据库未保存数据，登记退货单时手动添加的商品)

            if (string.IsNullOrEmpty(Golbal_RefundDetailModel.refundNo))
            {
                //注册页面发起(直接编辑)
                if (Golbal_Type.Equals(typeof(RegisterRefund)))
                {
                    RegisterRefund rr = ((RegisterRefund)FormInit.FindFather(this, typeof(RegisterRefund)));
                    //刷新窗体
                    System.Collections.ObjectModel.ObservableCollection<RefundDetailModel> rdmList = new System.Collections.ObjectModel.ObservableCollection<RefundDetailModel>();

                    System.Collections.IEnumerator ie = rr.Registerlist.ItemsSource.GetEnumerator();

                    while (ie.MoveNext())
                    {
                        RefundDetailModel rm = ie.Current as RefundDetailModel;
                        if (rm.sku.Equals(Golbal_RefundDetailModel.sku))
                        {
                            rm.refundReason = Golbal_RefundDetailModel.refundReason;
                            rm.expressCompany = Golbal_RefundDetailModel.expressCompany;
                            rm.remark = Golbal_RefundDetailModel.remark;
                            rm.tbTradeNo = Golbal_RefundDetailModel.tbTradeNo;
                        }
                        rdmList.Add(rm);
                    }

                    rr.Registerlist.ItemsSource = rdmList;
                    //关闭本窗体
                    this.CloseDialog();

                }
                else if (Golbal_Type.Equals(typeof(AddGoods)))//添加商品页面发起
                {
                    //显示的商品图片
                    Golbal_RefundDetailModel.imgUrl = imgUrl.Replace("360x360", "120x120");
                    //收货状态:拆包部门添加是已收货状态;其它部门添加为待收货状态
                    Golbal_RefundDetailModel.confirmReceipt = Golbal_IsUnpacking.Equals(true) ? (int)ReceiptStatus.yes : (int)ReceiptStatus.watting;

                    //刷新窗体
                    //父级
                    var ag = ((AddGoods)FormInit.FindFather(this, typeof(AddGoods)));
                    //AddGoods 父级
                    var rr = ((RegisterRefund)FormInit.FindFather(ag, typeof(RegisterRefund)));

                    rr.Golbal_MTRegisterDataSource.Add(Golbal_RefundDetailModel);

                    IEnumerator<RefundDetailModel> ie = rr.Golbal_MTRegisterDataSource.GetEnumerator();

                    int no = 1;
                    while (ie.MoveNext())
                    {
                        ie.Current.dColumn1 = no.ToString();
                        no++;
                    }


                    rr.Registerlist.ItemsSource = rr.Golbal_MTRegisterDataSource;

                    //关闭父级
                    ag.CloseDialog();
                    //关闭本窗体
                    this.CloseDialog();
                }
            }

            #endregion

            #region 添加

            ////添加
            else if (Golbal_RefundDetailModel.id.Equals(0))
            {
                try
                {
                    List<string> sql = new List<string>();

                    sql.Add(string.Format(@"insert into T_RefundDetail(
                                                                   refundNo,
                                                                   goodsno,
                                                                   sku,
                                                                   specName,
                                                                   categoryName,
                                                                   tbTradeNo,
                                                                   gjTradeNo,
                                                                   refundReason,
                                                                   expressCompany,
                                                                   expressCode,
                                                                   confirmReceipt,
                                                                   isFinanceEnd,
                                                                   imgUrl,
                                                                   isNotRegister,
                                                                   remark,
                                                                   unpackingEmployee,
                                                                   unpackingTime) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}',{7},'{8}','{9}',{10},{11},'{12}',{13},'{14}','{15}','{16}')",
                                                                   Golbal_RefundDetailModel.refundNo,
                                                                   Golbal_RefundDetailModel.goodsno,
                                                                   Golbal_RefundDetailModel.sku,
                                                                   Golbal_RefundDetailModel.specName,
                                                                   Golbal_RefundDetailModel.categoryName,
                                                                   Golbal_RefundDetailModel.tbTradeNo,
                                                                   Golbal_RefundDetailModel.gjTradeNo,
                                                                   Golbal_RefundDetailModel.refundReason,
                                                                   Golbal_RefundDetailModel.expressCompany,
                                                                   Golbal_RefundDetailModel.expressCode,
                                                                   Golbal_IsUnpacking.Equals(true) ? (int)ReceiptStatus.yes : (int)ReceiptStatus.watting,//收货状态:拆包部门添加是已收货状态;其它部门添加为待收货状态
                                                                   0,
                                                                   imgUrl.Replace("360x360", "120x120"),
                                                                   Golbal_IsUnpacking.Equals(true) ? 1 : 0,//是否注册:拆包部门添加是未注册;其它部门添加为已注册
                                                                   Golbal_RefundDetailModel.remark,
                                                                   Golbal_IsUnpacking.Equals(true) ? CommonLogin.CommonUser.UserName : "",
                                                                   DateTime.Now
                                                                   ));


                    if (Golbal_IsUnpacking)
                    {
                        //拆包时添加将状态改为异常单,需要等待客服处理
                        sql.Add(string.Format(@"update T_Refund set refundStatus={1},refundAmount=isnull((select COUNT(1) from T_RefundDetail where refundNo='{0}'),0) where refundNo='{0}'", Golbal_RefundDetailModel.refundNo, (int)RefundSatausEnum.refundException));
                    }
                    else
                    {
                        //客服添加状态不变，因为如果为异常单，客服需要在异常单处理一次
                        sql.Add(string.Format(@"update T_Refund set refundAmount=isnull((select COUNT(1) from T_RefundDetail where refundNo='{0}'),0) where refundNo='{0}'", Golbal_RefundDetailModel.refundNo));
                        sql.Add(string.Format(@"if exists(select id from T_Refund where refundNo='{0}' and (refundStatus={1} or refundStatus={2} or refundStatus={3})) 
                        begin
                             update T_Refund set refundStatus={4} where refundNo='{0}'
                        end", Golbal_RefundDetailModel.refundNo, (int)RefundSatausEnum.financeOperator, (int)RefundSatausEnum.financePartOperator, (int)RefundSatausEnum.End, (int)RefundSatausEnum.noUnpacking));
                    }

                    sql.Add(SQLTxt.GetInserLogSql(CommonLogin.CommonUser.UserName, EnumHelper.GetEnumTextVal((int)RefundOperatorEnum.AddMuilt, typeof(RefundOperatorEnum)), Golbal_RefundDetailModel.refundNo, Golbal_RefundDetailModel.sku));

                    if (dal.ExecuteTransaction(sql, null))
                    {
                        try
                        {
                            //刷新登记窗体
                            if (Golbal_Type.Equals(typeof(RegisterRefund)))
                            {
                                RegisterRefund rr = ((RegisterRefund)FormInit.FindFather(this, typeof(RegisterRefund)));
                                rr.RefreshBind();
                            }
                            else if (Golbal_Type.Equals(typeof(AddGoods)))//刷新添加商品的窗体
                            {
                                AddGoods rr = ((AddGoods)FormInit.FindFather(this, typeof(AddGoods)));
                                rr.Bind(rr.golbal_GjTradeNo, rr.golbal_RefundNo);
                            }
                            else
                            {
                                CloseDialog();
                            }
                        }
                        catch
                        { }
                        CloseDialog();
                    }
                    else
                    {
                        MessageBox.Show("保存成功", "提示");
                    }
                }
                catch (Exception err)
                {
                    MessageBox.Show(string.Format("捕获到异常{0}", err.Message.ToString()), "提示");
                }
            }
            #endregion

            #region 更新
            else
            {

                //更新操作
                try
                {
                    if (Golbal_RefundDetailModel.tbRefundId.Equals((long)9999))
                    {
                        dal.ExecuteSql(string.Format("update T_Unknownlist set title='{0}' where id={1}", Golbal_RefundDetailModel.remark + "|" + Golbal_RefundDetailModel.expressCompany + ":" + Golbal_RefundDetailModel.expressCode, Golbal_RefundDetailModel.id));
                        //刷新登记窗体
                        if (Golbal_Type.Equals(typeof(RegisterRefund)))
                        {
                            RegisterRefund rr = ((RegisterRefund)FormInit.FindFather(this, typeof(RegisterRefund)));
                            rr.RefreshBind();
                        }
                        else if (Golbal_Type.Equals(typeof(AddGoods)))//刷新添加商品的窗体
                        {
                            AddGoods rr = ((AddGoods)FormInit.FindFather(this, typeof(AddGoods)));
                            rr.Bind(rr.golbal_GjTradeNo, rr.golbal_RefundNo);
                        }
                        else if (Golbal_Type.Equals(typeof(UnpackingCheck)))
                        {
                            UnpackingCheck uc = ((UnpackingCheck)FormInit.FindFather(this, typeof(UnpackingCheck)));
                            uc.dataBind(Golbal_RefundDetailModel.refundNo);
                        }
                    }
                    else
                    {
                        if (dal.Update(Golbal_RefundDetailModel))
                        {
                            try
                            {
                                dal.ExecuteSql(SQLTxt.GetInserLogSql(CommonLogin.CommonUser.UserName, EnumHelper.GetEnumTextVal((int)RefundOperatorEnum.UpdateInfo, typeof(RefundOperatorEnum)), Golbal_RefundDetailModel.refundNo, Golbal_RefundDetailModel.sku));
                                //刷新登记窗体
                                if (Golbal_Type.Equals(typeof(RegisterRefund)))
                                {
                                    RegisterRefund rr = ((RegisterRefund)FormInit.FindFather(this, typeof(RegisterRefund)));
                                    rr.RefreshBind();
                                }
                                else if (Golbal_Type.Equals(typeof(AddGoods)))//刷新添加商品的窗体
                                {
                                    AddGoods rr = ((AddGoods)FormInit.FindFather(this, typeof(AddGoods)));
                                    rr.Bind(rr.golbal_GjTradeNo, rr.golbal_RefundNo);
                                }
                                else if (Golbal_Type.Equals(typeof(UnpackingCheck)))
                                {
                                    UnpackingCheck uc = ((UnpackingCheck)FormInit.FindFather(this, typeof(UnpackingCheck)));
                                    uc.dataBind(Golbal_RefundDetailModel.refundNo);
                                }
                            }
                            catch
                            { }

                            MessageBox.Show("保存成功");
                            CloseDialog();
                        }
                        else
                        {
                            MessageBox.Show("保存失败");
                        }
                    }

                }
                catch (Exception err)
                {
                    MessageBox.Show("保存失败，原因:" + err.Message.ToString());
                }


            }
            #endregion
        }

        #endregion

        #region Dialog
        public Microsoft.Practices.Prism.Commands.DelegateCommand CancelCommand
        {
            get { return new DelegateCommand(CloseDialog); }
        }

        public void CloseDialog()
        {
            FormInit.CloseDialog(this, 3);
        }

        public string Title
        {
            get { return "信息编辑"; }
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
    }
}
