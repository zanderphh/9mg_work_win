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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Top.Api.Domain;

namespace _9M.Work.WPF_Main.Views.Refund
{
    /// <summary>
    /// Interaction logic for UnpackingCheck.xaml
    /// </summary>
    public partial class UnpackingCheck : UserControl, BaseDialog
    {

        #region 全局变量
        PrintHelper ph = new PrintHelper();
        /// <summary>
        /// 当前处理状态
        /// </summary>
        RefundHandleStatusEnum currentHandleStatus = RefundHandleStatusEnum.look;

        RefundModel Golbal_RefundModel = new RefundModel();

        BaseDAL dal = new BaseDAL();

        List<ShopModel> Golbal_shopModel = new List<ShopModel>();

        ErpApi.GoodsManager manager = new ErpApi.GoodsManager();

        Print.PrintLabel print = new Print.PrintLabel();

        List<string> Golbal_Sql = new List<string>();


        #endregion

        #region 窗体初始化

        public UnpackingCheck(RefundModel model, RefundHandleStatusEnum rhse)
        {
            InitializeComponent();
            this.DataContext = this;
            Golbal_shopModel = dal.GetAll<ShopModel>();
            Golbal_RefundModel = model;
            currentHandleStatus = rhse;
            btn_batchFinanceEnd.Visibility = currentHandleStatus.Equals(RefundHandleStatusEnum.FinancialRefund) ? Visibility.Visible : Visibility.Collapsed;

            cmbExpressCompanybatch.SelectedValue = (int)ExpressCompanyEnum.DEFAULT;
            viewInitBind(model);

            if (rhse == RefundHandleStatusEnum.UnpackingCheck || true)
            {
                List<Erp_Model_GJRefund> Erp_TradeInfo = new List<Erp_Model_GJRefund>();
                var tradeNoCollection = model.tradeNo.Split(',');

                foreach (string tradeNo in tradeNoCollection)
                {
                    Erp_TradeInfo.Add(new Erp_Model_GJRefund() { tbTradeNo = tradeNo });
                }

                if (model.shopId.Equals(1003))
                {
                    try
                    {
                        TopSource com = new TopSource();
                        List<PurchaseOrder> tradeCollectionByFxApi = com.GetTradeInfoByFX(Erp_TradeInfo);

                        if (tradeCollectionByFxApi.Count > 0 && tradeCollectionByFxApi[0] != null)
                        {
                            if (tradeCollectionByFxApi.FindAll(a => a.SupplierFlag.Equals(4)).Count() > 0)
                            {
                                MessageBox.Show("质量问题退货,请核实!", "提示");
                            }
                        }
                    }
                    catch
                    { }
                }
                else if (model.shopId.Equals(1000) || model.shopId.Equals(1022))
                {

                    // OnlineStoreAuthorization AuthorizationModel = new OnlineStoreAuthorization();
                    ShopModel shop = null;
                    SessionUserModel user = CommonLogin.CommonUser;
                    if (model.shopId.Equals(1000))
                    {
                        shop = Golbal_shopModel.Where(x => x.shopId == 1000).FirstOrDefault();
                    }
                    else if (model.shopId.Equals(1022))
                    {
                        shop = Golbal_shopModel.Where(x => x.shopId == 1022).FirstOrDefault();
                    }
                    TopSource com = new TopSource(shop);
                    List<Trade> tradeCollection = com.GetTradeInfoByCShopOrTmall(Erp_TradeInfo);

                    bool isHaving = false;
                    foreach (Trade t in tradeCollection)
                    {
                        if (t == null)
                        {
                            isHaving = true;
                        }
                    }

                    if (!isHaving)
                    {
                        if (tradeCollection.Where(a => a.SellerFlag.Equals(4)).Count() > 0)
                        {
                            MessageBox.Show("质量问题退货,请核实!", "提示");
                        }
                    }
                }
            }
        }

        public void viewInitBind(RefundModel model)
        {
            System.Windows.Application.Current.Dispatcher.InvokeAsync(new Action(() =>
            {
                try
                {
                    RefundBaseInfoBind(model);
                    dataBind(model.refundNo);
                }
                catch (Exception err)
                {
                    string msg = err.Message.ToString();
                }
            }));

        }

        #endregion

        #region 退货基本信息绑定
        /// <summary>
        /// 退货基本信息绑定
        /// </summary>
        /// <param name="model"></param>
        void RefundBaseInfoBind(RefundModel model)
        {
            txtShopName.Text = Golbal_shopModel.Find(a => a.shopId.Equals(model.shopId)).shopName;
            txtNickName.Text = model.tbnick;
            txtRealName.Text = model.RealName;
            txtRemark.ToolTip = txtRemark.Text = model.remark;
            txtTbTradeNo.Text = model.tradeNo;
            txtMobile.Text = model.mobile;
            txtExpressNo.Text = model.sendPostcode;
        }
        #endregion

        #region 退货商品明细数据绑定
        /// <summary>
        /// 退货商品明细数据绑定
        /// </summary>
        /// <param name="refundNo"></param>
        public void dataBind(string refundNo)
        {
            List<ExpressionModelField> listWhere = new List<ExpressionModelField>();

            if (!string.IsNullOrEmpty(refundNo)) { listWhere.Add(new ExpressionModelField() { Name = "refundNo", Value = refundNo }); }
            List<RefundDetailModel> datasource = dal.GetList<RefundDetailModel>(listWhere.ToArray(), new OrderModelField[] { new OrderModelField() { PropertyName = "id", IsDesc = true } });

            //是否还有未知款商品
            if (!string.IsNullOrEmpty(refundNo))
            {
                listWhere.Add(new ExpressionModelField() { Name = "refundNo", Value = refundNo });
                listWhere.Add(new ExpressionModelField() { Name = "isHandle", Value = true, Relation = EnumRelation.NotEqual });
                List<UnknownlistModel> list = dal.GetList<UnknownlistModel>(listWhere.ToArray(), new OrderModelField[] { });
                list.ForEach(m =>
                {
                    datasource.Add(new RefundDetailModel() { remark = m.title, id = m.id.HasValue == true ? m.id.Value : 0, confirmReceipt = (int)ReceiptStatus.no, isFinanceEnd = false, IsNotRegister = true, realSku = m.id.ToString(), refundNo = m.refundNo, tbRefundId = 9999 });
                });
            }

            DataSourceRefundStatusBind(datasource);
        }

        #endregion

        #region 添加多退款商品后刷新列表

        /// <summary>
        /// 添加多退款商品后刷新列表
        /// </summary>
        /// <param name="refundNo"></param>
        public void dataBindByRefresh(string refundNo)
        {
            #region 获取数据库里的数据

            List<ExpressionModelField> listWhere = new List<ExpressionModelField>();

            if (!string.IsNullOrEmpty(refundNo)) { listWhere.Add(new ExpressionModelField() { Name = "refundNo", Value = refundNo }); }
            List<RefundDetailModel> datasource = dal.GetList<RefundDetailModel>(listWhere.ToArray(), new OrderModelField[] { new OrderModelField() { PropertyName = "id", IsDesc = true } });

            //是否还有未知款商品
            if (!string.IsNullOrEmpty(refundNo))
            {
                listWhere.Add(new ExpressionModelField() { Name = "refundNo", Value = refundNo });
                listWhere.Add(new ExpressionModelField() { Name = "isHandle", Value = true, Relation = EnumRelation.NotEqual });
                List<UnknownlistModel> list = dal.GetList<UnknownlistModel>(listWhere.ToArray(), new OrderModelField[] { });
                list.ForEach(m =>
                {
                    datasource.Add(new RefundDetailModel() { remark = m.title, id = 0, confirmReceipt = (int)ReceiptStatus.no, isFinanceEnd = false, IsNotRegister = true, realSku = m.id.ToString(), refundNo = m.refundNo });
                });
            }

            #endregion

            #region 当前有操作过的

            List<RefundDetailModel> TempDatasource = new List<RefundDetailModel>();

            //添加商品后刷新列表(拆包添加的商品)
            if (currentHandleStatus == RefundHandleStatusEnum.UnpackingCheck)
            {
                if (Golbal_Sql.Count > 0)
                {
                    var ietor = Handlerslist.ItemsSource.GetEnumerator();
                    while (ietor.MoveNext())
                    {
                        TempDatasource.Add(((GridItemDataSource)ietor.Current).rdm);
                    }
                }
            }

            datasource.ForEach(m =>
            {
                if (TempDatasource.FindAll(a => a.id.Equals(m.id)).Count().Equals(0))
                {
                    TempDatasource.Add(m);
                }
            });

            #endregion

            DataSourceRefundStatusBind(TempDatasource);
        }

        #endregion

        #region 遍历数据源加载对应状态

        public void DataSourceRefundStatusBind(List<RefundDetailModel> datasource)
        {
            ObservableCollection<GridItemDataSource> gids_collection = new ObservableCollection<GridItemDataSource>();

            #region  数据源遍历绑定

            int sn = 1;
            int unpackingNum = 0;
            int financeEndNum = 0;
            int exceptionNum = 0;

            datasource.ForEach(delegate (RefundDetailModel m)
            {
                if (m.IsNotRegister.Equals(true) && !m.confirmReceipt.Equals((int)ReceiptStatus.exceptionEnd)) { exceptionNum += 1; }
                if (m.isFinanceEnd.Equals(true)) { financeEndNum += 1; }
                if (m.IsNotRegister.Equals(true) && m.confirmReceipt.Equals((int)ReceiptStatus.exceptionEnd)) { unpackingNum += 1; }
                else if (m.confirmReceipt.Equals((int)ReceiptStatus.yes)) { unpackingNum += 1; }

                RefundDetailGridModel rdgm = new RefundDetailGridModel();
                //默认隐藏分销店铺转即时到帐按钮
                rdgm.FXHandleButton = System.Windows.Visibility.Collapsed;

                List<EnumEntity> expressItems = new List<EnumEntity>();

                if (!string.IsNullOrEmpty(m.expressCompany))
                {
                    List<EnumEntity> items = new List<EnumEntity>();
                    items.Add(new EnumEntity() { Text = m.expressCompany, Value = 99 });
                    rdgm.ExpressCompanyItem = items;
                }
                else
                {
                    rdgm.ExpressCompanyItem = EnumHelper.GetEnumList(typeof(ExpressCompanyEnum));
                    m.expressCompany = "-请选择-";
                }

                #region 拆包
                if (currentHandleStatus == RefundHandleStatusEnum.UnpackingCheck)
                {
                    if (m.id.Equals(0) && m.IsNotRegister.Equals(true))//未知款
                    {
                        rdgm.GridButtonVisible = System.Windows.Visibility.Collapsed;
                        rdgm.GridTextBlockVisible = System.Windows.Visibility.Visible;

                        if ((m.confirmReceipt.Equals((int)ReceiptStatus.exceptionEnd)))
                        {
                            rdgm.GridButtonVisible = System.Windows.Visibility.Visible;
                            rdgm.GridTextBlockVisible = System.Windows.Visibility.Collapsed;
                            //确认到货
                            rdgm.GridButtonName = EnumHelper.GetEnumTextVal((int)RefundOperatorEnum.Receive_Yes, typeof(RefundOperatorEnum));
                        }
                        else
                        {
                            rdgm.GridButtonVisible = System.Windows.Visibility.Collapsed;
                            rdgm.GridTextBlockVisible = System.Windows.Visibility.Visible;
                            //未知款处理
                            rdgm.GridTextBlock = EnumHelper.GetEnumTextVal((int)RefundOperatorEnum.Nothing, typeof(RefundOperatorEnum));
                        }
                    }
                    else if (m.id > 0 && m.IsNotRegister.Equals(true))//多退款
                    {
                        if (m.isFinanceEnd.Equals(true))
                        {
                            rdgm.GridButtonVisible = System.Windows.Visibility.Collapsed;
                            rdgm.GridTextBlockVisible = System.Windows.Visibility.Visible;
                            rdgm.FXHandleButton = System.Windows.Visibility.Collapsed;
                            rdgm.GridTextBlock = (m.isJSDZ == true) ? EnumHelper.GetEnumTextVal((int)RefundOperatorEnum.JSDZ_YES, typeof(RefundOperatorEnum)) : EnumHelper.GetEnumTextVal((int)RefundOperatorEnum.End, typeof(RefundOperatorEnum));
                        }
                        else if (m.confirmReceipt.Equals((int)ReceiptStatus.exceptionEnd))
                        {
                            rdgm.GridTextBlockVisible = System.Windows.Visibility.Collapsed;
                            rdgm.GridButtonVisible = System.Windows.Visibility.Visible;
                            //取消收货
                            rdgm.GridButtonName = EnumHelper.GetEnumTextVal((int)RefundOperatorEnum.Receive_Cancel, typeof(RefundOperatorEnum));
                        }
                        else if (m.confirmReceipt.Equals((int)ReceiptStatus.watting))
                        {
                            rdgm.GridButtonVisible = System.Windows.Visibility.Visible;
                            rdgm.GridTextBlockVisible = System.Windows.Visibility.Collapsed;
                            //确认到货
                            rdgm.GridButtonName = EnumHelper.GetEnumTextVal((int)RefundOperatorEnum.Receive_Yes, typeof(RefundOperatorEnum));
                            rdgm.FXHandleButton = System.Windows.Visibility.Visible;
                            //未收到货
                            rdgm.FxHandleButtonName = EnumHelper.GetEnumTextVal((int)RefundOperatorEnum.Receive_No, typeof(RefundOperatorEnum));
                        }
                        else
                        {
                            rdgm.GridButtonVisible = System.Windows.Visibility.Collapsed;
                            rdgm.GridTextBlockVisible = System.Windows.Visibility.Visible;
                            //多退款待处理
                            rdgm.GridTextBlock = EnumHelper.GetEnumTextVal((int)RefundOperatorEnum.Mulit_Wait_Handle, typeof(RefundOperatorEnum));
                        }
                    }
                    else if (m.isFinanceEnd.Equals(true))
                    {
                        rdgm.GridButtonVisible = System.Windows.Visibility.Collapsed;
                        rdgm.GridTextBlockVisible = System.Windows.Visibility.Visible;
                        rdgm.FXHandleButton = System.Windows.Visibility.Collapsed;
                        rdgm.GridTextBlock = (m.isJSDZ == true) ? EnumHelper.GetEnumTextVal((int)RefundOperatorEnum.JSDZ_YES, typeof(RefundOperatorEnum)) : EnumHelper.GetEnumTextVal((int)RefundOperatorEnum.End, typeof(RefundOperatorEnum));
                    }
                    else
                    {
                        rdgm.GridTextBlockVisible = System.Windows.Visibility.Collapsed;
                        rdgm.GridButtonVisible = System.Windows.Visibility.Visible;

                        if (m.confirmReceipt.Equals((int)ReceiptStatus.yes))
                        {
                            //取消收货
                            rdgm.GridButtonName = EnumHelper.GetEnumTextVal((int)RefundOperatorEnum.Receive_Cancel, typeof(RefundOperatorEnum));
                        }
                        else if (m.confirmReceipt.Equals((int)ReceiptStatus.watting))
                        {
                            rdgm.FXHandleButton = System.Windows.Visibility.Visible;
                            //未收到货
                            rdgm.FxHandleButtonName = EnumHelper.GetEnumTextVal((int)RefundOperatorEnum.Receive_No, typeof(RefundOperatorEnum));
                            //确认到货
                            rdgm.GridButtonName = EnumHelper.GetEnumTextVal((int)RefundOperatorEnum.Receive_Yes, typeof(RefundOperatorEnum));
                        }
                        else if (m.confirmReceipt.Equals((int)ReceiptStatus.no))
                        {
                            rdgm.FXHandleButton = System.Windows.Visibility.Collapsed;
                            rdgm.GridButtonVisible = System.Windows.Visibility.Collapsed;
                            rdgm.GridTextBlockVisible = System.Windows.Visibility.Visible;
                            //未收到货
                            rdgm.GridTextBlock = EnumHelper.GetEnumTextVal((int)RefundOperatorEnum.Receive_No, typeof(RefundOperatorEnum));
                        }
                        else if (m.confirmReceipt.Equals((int)ReceiptStatus.exceptionEnd) && m.IsNotRegister.Equals(true))
                        {
                            rdgm.FXHandleButton = System.Windows.Visibility.Collapsed;
                            rdgm.GridButtonVisible = System.Windows.Visibility.Collapsed;
                            rdgm.GridTextBlockVisible = System.Windows.Visibility.Visible;
                            //异常已处理
                            EnumHelper.GetEnumTextVal((int)RefundOperatorEnum.Exception_YES, typeof(RefundOperatorEnum));
                        }
                    }
                }
                #endregion

                #region 财务退款&财务部分退款

                else if (currentHandleStatus == RefundHandleStatusEnum.FinancialRefund || currentHandleStatus == RefundHandleStatusEnum.FinancialPartRefund)
                {
                    if (m.id.Equals(0) && m.IsNotRegister.Equals(true))//未知款
                    {
                        if (m.confirmReceipt.Equals((int)ReceiptStatus.exceptionEnd))
                        {
                            rdgm.GridTextBlockVisible = System.Windows.Visibility.Collapsed;
                            rdgm.GridButtonName = EnumHelper.GetEnumTextVal((int)RefundOperatorEnum.Confirm_Refund, typeof(RefundOperatorEnum));
                            if (txtShopName.Text.ToString().Contains("分销"))
                            {
                                rdgm.FXHandleButton = System.Windows.Visibility.Visible;
                                rdgm.FxHandleButtonName = EnumHelper.GetEnumTextVal((int)RefundOperatorEnum.Confirm_JSDZ, typeof(RefundOperatorEnum));
                            }
                        }
                        else
                        {
                            rdgm.GridButtonVisible = System.Windows.Visibility.Collapsed;
                            rdgm.GridTextBlock = EnumHelper.GetEnumTextVal((int)RefundOperatorEnum.Nothing_Wait, typeof(RefundOperatorEnum));
                        }
                    }
                    else if (m.id > 0 && m.IsNotRegister.Equals(true))//多退款
                    {

                        if (m.confirmReceipt.Equals((int)ReceiptStatus.exceptionEnd) && !m.isFinanceEnd.Equals(true))//异常为完成状态并且财务未退款
                        {
                            rdgm.GridTextBlockVisible = System.Windows.Visibility.Collapsed;
                            rdgm.GridButtonVisible = System.Windows.Visibility.Visible;
                            rdgm.GridButtonName = EnumHelper.GetEnumTextVal((int)RefundOperatorEnum.Confirm_Refund, typeof(RefundOperatorEnum));
                            if (txtShopName.Text.ToString().Contains("分销"))
                            {
                                rdgm.FXHandleButton = System.Windows.Visibility.Visible;
                                rdgm.FxHandleButtonName = EnumHelper.GetEnumTextVal((int)RefundOperatorEnum.Confirm_JSDZ, typeof(RefundOperatorEnum));
                            }
                        }
                        else if (m.confirmReceipt.Equals((int)ReceiptStatus.yes) && !m.isFinanceEnd.Equals(true))
                        {
                            rdgm.GridTextBlockVisible = System.Windows.Visibility.Visible;
                            rdgm.GridButtonVisible = System.Windows.Visibility.Collapsed;
                            rdgm.GridTextBlock = EnumHelper.GetEnumTextVal((int)RefundOperatorEnum.Mulit_Wait_Handle, typeof(RefundOperatorEnum));
                        }
                        else
                        {
                            if (m.isFinanceEnd.Equals(true))
                            {
                                //财务已完成的
                                rdgm.GridTextBlock = EnumHelper.GetEnumTextVal((int)RefundOperatorEnum.End, typeof(RefundOperatorEnum));
                            }
                            else
                            {
                                //多退款待处理
                                rdgm.GridTextBlock = EnumHelper.GetEnumTextVal((int)RefundOperatorEnum.Mulit_Wait_Handle, typeof(RefundOperatorEnum));
                            }

                            rdgm.GridTextBlockVisible = System.Windows.Visibility.Visible;
                            rdgm.GridButtonVisible = System.Windows.Visibility.Collapsed;
                        }

                    }
                    else if (m.isFinanceEnd.Equals(true))
                    {
                        rdgm.GridButtonVisible = System.Windows.Visibility.Collapsed;
                        rdgm.GridTextBlockVisible = System.Windows.Visibility.Visible;
                        rdgm.FXHandleButton = System.Windows.Visibility.Collapsed;
                        //转即使到帐&已完成
                        rdgm.GridTextBlock = (m.isJSDZ == true) ? EnumHelper.GetEnumTextVal((int)RefundOperatorEnum.Confirm_JSDZ, typeof(RefundOperatorEnum)) : EnumHelper.GetEnumTextVal((int)RefundOperatorEnum.End, typeof(RefundOperatorEnum));
                    }
                    else
                    {
                        if ((m.confirmReceipt.Equals((int)ReceiptStatus.yes) && m.IsNotRegister.Equals(false)) || (m.IsNotRegister.Equals(true) && m.confirmReceipt.Equals((int)ReceiptStatus.exceptionEnd)))
                        {
                            rdgm.GridButtonVisible = System.Windows.Visibility.Visible;
                            rdgm.GridButtonName = EnumHelper.GetEnumTextVal((int)RefundOperatorEnum.Confirm_Refund, typeof(RefundOperatorEnum));
                            rdgm.GridTextBlockVisible = System.Windows.Visibility.Collapsed;
                            if (txtShopName.Text.ToString().Contains("分销"))
                            {
                                rdgm.FXHandleButton = System.Windows.Visibility.Visible;
                                rdgm.FxHandleButtonName = EnumHelper.GetEnumTextVal((int)RefundOperatorEnum.Confirm_JSDZ, typeof(RefundOperatorEnum));
                            }
                        }
                        else if (m.confirmReceipt.Equals((int)ReceiptStatus.watting))
                        {
                            rdgm.GridButtonVisible = System.Windows.Visibility.Collapsed;
                            rdgm.FXHandleButton = System.Windows.Visibility.Collapsed;
                            rdgm.GridTextBlockVisible = System.Windows.Visibility.Visible;
                            rdgm.GridTextBlock = EnumHelper.GetEnumTextVal((int)RefundOperatorEnum.Receive_Yes, typeof(RefundOperatorEnum));
                        }
                        else if (m.confirmReceipt.Equals((int)ReceiptStatus.no))
                        {
                            rdgm.GridButtonVisible = System.Windows.Visibility.Collapsed;
                            rdgm.FXHandleButton = System.Windows.Visibility.Collapsed;
                            rdgm.GridTextBlockVisible = System.Windows.Visibility.Visible;
                            rdgm.GridTextBlock = EnumHelper.GetEnumTextVal((int)RefundOperatorEnum.Receive_No, typeof(RefundOperatorEnum));
                        }
                        else
                        {
                            rdgm.GridButtonVisible = System.Windows.Visibility.Collapsed;
                            rdgm.GridTextBlockVisible = System.Windows.Visibility.Collapsed;
                            rdgm.FXHandleButton = System.Windows.Visibility.Collapsed;
                        }
                    }
                }

                #endregion

                #region 异常状态(添加了未知款&多退款)

                else if (currentHandleStatus == RefundHandleStatusEnum.ExceptionHandler)
                {
                    if (m.id.Equals(0) && m.IsNotRegister.Equals(true) || m.tbRefundId.Equals((long)9999))//未知款
                    {
                        rdgm.GridButtonVisible = System.Windows.Visibility.Visible;
                        rdgm.GridButtonName = EnumHelper.GetEnumTextVal((int)RefundOperatorEnum.Nothing_Wait, typeof(RefundOperatorEnum));
                        rdgm.GridTextBlockVisible = System.Windows.Visibility.Collapsed;
                    }
                    else if (m.id > 0 && m.IsNotRegister.Equals(true))//多退款
                    {
                        if (m.isFinanceEnd.Equals(true))
                        {
                            rdgm.GridButtonVisible = System.Windows.Visibility.Collapsed;
                            rdgm.GridTextBlockVisible = System.Windows.Visibility.Visible;
                            rdgm.GridTextBlock = EnumHelper.GetEnumTextVal((int)RefundOperatorEnum.End, typeof(RefundOperatorEnum));
                        }

                        else if (m.confirmReceipt.Equals((int)ReceiptStatus.exceptionEnd))
                        {
                            rdgm.FXHandleButton = System.Windows.Visibility.Collapsed;
                            rdgm.GridButtonVisible = System.Windows.Visibility.Collapsed;
                            rdgm.GridTextBlockVisible = System.Windows.Visibility.Visible;
                            rdgm.GridTextBlock = EnumHelper.GetEnumTextVal((int)RefundOperatorEnum.Exception_YES, typeof(RefundOperatorEnum));
                        }
                        else if (m.confirmReceipt.Equals((int)ReceiptStatus.no))
                        {
                            rdgm.GridButtonVisible = System.Windows.Visibility.Visible;
                            rdgm.GridButtonName = EnumHelper.GetEnumTextVal((int)RefundOperatorEnum.Receive_No_Handle, typeof(RefundOperatorEnum));
                            rdgm.GridTextBlockVisible = System.Windows.Visibility.Collapsed;
                        }
                        else if (m.confirmReceipt.Equals((int)ReceiptStatus.watting))
                        {
                            rdgm.GridButtonVisible = System.Windows.Visibility.Collapsed;
                            rdgm.GridTextBlockVisible = System.Windows.Visibility.Visible;
                            rdgm.GridTextBlock = EnumHelper.GetEnumTextVal((int)RefundOperatorEnum.Receive_Yes, typeof(RefundOperatorEnum));
                        }
                        else
                        {
                            rdgm.GridButtonVisible = System.Windows.Visibility.Visible;
                            rdgm.GridButtonName = EnumHelper.GetEnumTextVal((int)RefundOperatorEnum.Mulit_Wait_Handle, typeof(RefundOperatorEnum));
                            rdgm.GridTextBlockVisible = System.Windows.Visibility.Collapsed;
                        }

                    }
                    else if (m.isFinanceEnd.Equals(true))//财务完成
                    {
                        rdgm.GridButtonVisible = System.Windows.Visibility.Collapsed;
                        rdgm.GridTextBlockVisible = System.Windows.Visibility.Visible;
                        rdgm.GridTextBlock = m.isJSDZ == true ? EnumHelper.GetEnumTextVal((int)RefundOperatorEnum.Confirm_JSDZ, typeof(RefundOperatorEnum)) : EnumHelper.GetEnumTextVal((int)RefundOperatorEnum.End, typeof(RefundOperatorEnum));
                    }
                    else if (m.confirmReceipt.Equals((int)ReceiptStatus.watting))
                    {
                        rdgm.GridButtonVisible = System.Windows.Visibility.Collapsed;
                        rdgm.GridTextBlockVisible = System.Windows.Visibility.Visible;
                        rdgm.GridTextBlock = EnumHelper.GetEnumTextVal((int)RefundOperatorEnum.Receive_Yes, typeof(RefundOperatorEnum));
                    }
                    else if (m.confirmReceipt.Equals((int)ReceiptStatus.yes))
                    {
                        rdgm.GridButtonVisible = System.Windows.Visibility.Collapsed;
                        rdgm.GridTextBlockVisible = System.Windows.Visibility.Visible;
                        rdgm.GridTextBlock = EnumHelper.GetEnumTextVal((int)RefundOperatorEnum.Confirm_Refund, typeof(RefundOperatorEnum));
                    }
                    else if (m.confirmReceipt.Equals((int)ReceiptStatus.no))
                    {
                        rdgm.GridTextBlockVisible = System.Windows.Visibility.Collapsed;
                        rdgm.GridButtonVisible = System.Windows.Visibility.Visible;
                        rdgm.GridButtonName = EnumHelper.GetEnumTextVal((int)RefundOperatorEnum.Receive_No_Handle, typeof(RefundOperatorEnum));
                        rdgm.FXHandleButton = System.Windows.Visibility.Collapsed;
                    }
                    else if (m.confirmReceipt.Equals((int)ReceiptStatus.exceptionEnd))
                    {
                        rdgm.FXHandleButton = System.Windows.Visibility.Collapsed;
                        rdgm.GridButtonVisible = System.Windows.Visibility.Collapsed;
                        rdgm.GridTextBlockVisible = System.Windows.Visibility.Visible;
                        rdgm.GridTextBlock = EnumHelper.GetEnumTextVal((int)RefundOperatorEnum.End, typeof(RefundOperatorEnum));
                    }

                }
                #endregion

                #region 完成状态

                else if (currentHandleStatus == RefundHandleStatusEnum.look)
                {
                    rdgm.GridButtonVisible = System.Windows.Visibility.Collapsed;

                    if (m.isFinanceEnd.Equals(true))
                    {
                        if (m.isJSDZ.Equals(true))
                            rdgm.GridTextBlock = EnumHelper.GetEnumTextVal((int)RefundOperatorEnum.JSDZ_YES, typeof(RefundOperatorEnum));
                        else
                            rdgm.GridTextBlock = EnumHelper.GetEnumTextVal((int)RefundOperatorEnum.End, typeof(RefundOperatorEnum));
                    }

                }

                #endregion

                #region 退邮费状态

                else if (currentHandleStatus == RefundHandleStatusEnum.RefundPostCode)
                {
                    if (m.isFinanceEnd.Equals(false))
                    {
                        rdgm.GridTextBlockVisible = System.Windows.Visibility.Visible;
                        rdgm.GridTextBlock = "已完成";
                        rdgm.GridButtonVisible = System.Windows.Visibility.Collapsed;
                    }
                    else
                    {
                        rdgm.GridTextBlockVisible = System.Windows.Visibility.Collapsed;
                        rdgm.GridButtonVisible = System.Windows.Visibility.Visible;
                        rdgm.GridButtonName = EnumHelper.GetEnumTextVal((int)RefundOperatorEnum.Refund_Post, typeof(RefundOperatorEnum));
                    }
                }

                #endregion

                #region 处理员工的姓名和时间

                if (!string.IsNullOrEmpty(m.unpackingEmployee))
                {
                    try
                    {
                        if (!m.unpackingEmployee.Contains("\r\n"))
                        {
                            var date = m.unpackingTime.Value.ToShortDateString();
                            var time = m.unpackingTime.Value.ToShortTimeString();
                            m.unpackingEmployee = m.unpackingEmployee + "\r\n" + date + "\r\n" + time;
                        }

                    }
                    catch
                    { }
                }
                if (!string.IsNullOrEmpty(m.exceptionEmployee))
                {
                    try
                    {
                        if (!m.exceptionEmployee.Contains("\r\n"))
                        {
                            var date = m.exceptionTime.Value.ToShortDateString();
                            var time = m.exceptionTime.Value.ToShortTimeString();
                            m.exceptionEmployee = m.exceptionEmployee + "\r\n" + date + "\r\n" + time;
                        }
                    }
                    catch
                    { }
                }

                #endregion

                m.dColumn1 = sn.ToString();
                gids_collection.Add(new GridItemDataSource() { rdm = m, rdgm = rdgm });
                sn++;

            });

            #endregion

            totalNumber.Text = gids_collection.Count.ToString();
            unpackingNumber.Text = unpackingNum.ToString();
            exceptionNumber.Text = exceptionNum.ToString();
            finRefundNumber.Text = financeEndNum.ToString();
            Handlerslist.ItemsSource = gids_collection;
            golbal_datasource = gids_collection;
        }

        #endregion

        #region 刷新状态
        private void RefreshDataSource()
        {
            List<RefundDetailModel> datagridDatasource = new List<RefundDetailModel>();
            var ietor = Handlerslist.ItemsSource.GetEnumerator();
            while (ietor.MoveNext())
            {
                datagridDatasource.Add(((GridItemDataSource)ietor.Current).rdm);
            }

            DataSourceRefundStatusBind(datagridDatasource);
        }

        #endregion

        #region Grid按钮操作处理

        private void btn_Operator(object sender, RoutedEventArgs e)
        {
            GridItemDataSource selectedrow = Handlerslist.SelectedItem as GridItemDataSource;

            #region 拆包操作

            if (currentHandleStatus == RefundHandleStatusEnum.UnpackingCheck)
            {
                if (WPF_Common.CommonLogin.CommonUser.DeptId.Equals(2) || WPF_Common.CommonLogin.CommonUser.UserName.ToUpper().Equals("ADMIN"))
                {
                    if (selectedrow.rdm.confirmReceipt.Equals((int)ReceiptStatus.yes) || (selectedrow.rdm.confirmReceipt.Equals((int)ReceiptStatus.exceptionEnd) && selectedrow.rdm.IsNotRegister.Equals(true)))
                    {
                        if (CCMessageBox.Show("是否取消确认收货", "提示", CCMessageBoxButton.YesNo) == CCMessageBoxResult.Yes)
                        {

                            Golbal_Sql.Add(string.Format("update T_RefundDetail set confirmReceipt={1},expressCompany='',expressCode='',unpackingEmployee='',unpackingTime=null where id={0}", selectedrow.rdm.id, (int)ReceiptStatus.watting));
                            Golbal_Sql.Add(SQLTxt.GetInserLogSql(CommonLogin.CommonUser.UserName, EnumHelper.GetEnumTextVal((int)RefundOperatorEnum.Receive_Cancel, typeof(RefundOperatorEnum)), selectedrow.rdm.refundNo, selectedrow.rdm.sku));

                            selectedrow.rdm.confirmReceipt = (int)ReceiptStatus.watting;
                            selectedrow.rdm.unpackingEmployee = "";
                            selectedrow.rdm.unpackingTime = null;
                            selectedrow.rdm.expressCode = "";
                            selectedrow.rdm.expressCompany = "";

                            RefreshDataSource();
                        }
                    }
                    else
                    {
                        Golbal_Sql.Add(string.Format("UPDATE T_RefundDetail set confirmReceipt={4},expressCompany='{1}',expressCode='{2}',unpackingEmployee='{3}',unpackingTime=getdate() where id={0}", selectedrow.rdm.id, selectedrow.rdm.expressCompany, selectedrow.rdm.expressCode, _9M.Work.WPF_Common.CommonLogin.CommonUser.UserName, selectedrow.rdm.IsNotRegister.Equals(true) ? (int)ReceiptStatus.exceptionEnd : (int)ReceiptStatus.yes));
                        Golbal_Sql.Add(SQLTxt.GetInserLogSql(CommonLogin.CommonUser.UserName, EnumHelper.GetEnumTextVal((int)RefundOperatorEnum.Receive_Yes, typeof(RefundOperatorEnum)), selectedrow.rdm.refundNo, selectedrow.rdm.sku));

                        selectedrow.rdm.confirmReceipt = selectedrow.rdm.IsNotRegister.Equals(true) ? (int)ReceiptStatus.exceptionEnd : (int)ReceiptStatus.yes;
                        selectedrow.rdm.unpackingEmployee = _9M.Work.WPF_Common.CommonLogin.CommonUser.UserName;
                        selectedrow.rdm.unpackingTime = DateTime.Now;

                        RefreshDataSource();

                        if (MessageBox.Show("是否打印标签", "提示", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        {
                            try
                            {
                                List<string> list = new List<string>() { selectedrow.rdm.sku };
                                ph.PrintLabel(list);
                            }
                            catch (Exception err)
                            {
                                MessageBox.Show(string.Format("错误提示:{0}", err.Message.ToString()));
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("只有拆包部门才能进行操作", "提示");
                }
            }
            #endregion

            #region 财务退款&部分退款操作

            else if (currentHandleStatus == RefundHandleStatusEnum.FinancialRefund || currentHandleStatus == RefundHandleStatusEnum.FinancialPartRefund)
            {
                Golbal_Sql.Add(string.Format("update T_RefundDetail set isFinanceEnd=1 where id={0}", selectedrow.rdm.id));
                Golbal_Sql.Add(SQLTxt.GetInserLogSql(CommonLogin.CommonUser.UserName, EnumHelper.GetEnumTextVal((int)RefundOperatorEnum.Confirm_Refund, typeof(RefundOperatorEnum)), selectedrow.rdm.refundNo, selectedrow.rdm.sku));

                selectedrow.rdm.isFinanceEnd = true;
                RefreshDataSource();
            }


            #endregion

            #region 财务退款(邮费)

            else if (currentHandleStatus == RefundHandleStatusEnum.RefundPostCode)
            {

                Golbal_Sql.Add(string.Format("update T_Refund set refundStatus={0},financeRefundStatus={1},endTime=GETDATE() where refundNo='{2}'", (int)RefundHandleStatusEnum.look, (int)FinanceRefundStatusEnum.RefundEnd, selectedrow.rdm.refundNo));
                Golbal_Sql.Add(SQLTxt.GetInserLogSql(CommonLogin.CommonUser.UserName, EnumHelper.GetEnumTextVal((int)RefundOperatorEnum.Refund_Post, typeof(RefundOperatorEnum)), selectedrow.rdm.refundNo, selectedrow.rdm.sku));
                selectedrow.rdm.isFinanceEnd = false;

                RefreshDataSource();

            }
            #endregion

            #region 异常状态

            else if (currentHandleStatus == RefundHandleStatusEnum.ExceptionHandler)
            {
                try
                {
                    if (selectedrow.rdm.id.Equals(0) && selectedrow.rdm.IsNotRegister.Equals(true))//未知款处理
                    {
                        Golbal_Sql.Add(string.Format("update T_Unknownlist set isHandle=1 where id={0}", selectedrow.rdm.realSku));
                        Golbal_Sql.Add(SQLTxt.GetInserLogSql(CommonLogin.CommonUser.UserName, EnumHelper.GetEnumTextVal((int)RefundOperatorEnum.Nothing, typeof(RefundOperatorEnum)), selectedrow.rdm.refundNo, selectedrow.rdm.sku));


                    }

                    else if (selectedrow.rdm.confirmReceipt.Equals((int)ReceiptStatus.no))//未收到货处理
                    {
                        Golbal_Sql.Add(string.Format("update T_RefundDetail set confirmReceipt={1},exceptionEmployee='{2}',exceptionTime=getdate() where id={0}", selectedrow.rdm.id, (int)ReceiptStatus.watting, WPF_Common.CommonLogin.CommonUser.UserName));
                        Golbal_Sql.Add(SQLTxt.GetInserLogSql(CommonLogin.CommonUser.UserName, EnumHelper.GetEnumTextVal((int)RefundOperatorEnum.Receive_No_Handle, typeof(RefundOperatorEnum)), selectedrow.rdm.refundNo, selectedrow.rdm.sku));

                        selectedrow.rdm.confirmReceipt = (int)ReceiptStatus.watting;
                        selectedrow.rdm.exceptionEmployee = CommonLogin.CommonUser.UserName;
                        selectedrow.rdm.exceptionTime = DateTime.Now;

                        RefreshDataSource();

                    }
                    else if (selectedrow.rdm.id > 0 && selectedrow.rdm.IsNotRegister.Equals(true))//多退款处理
                    {

                        Golbal_Sql.Add(string.Format("update T_RefundDetail set confirmReceipt={1},exceptionEmployee='{2}',exceptionTime=getdate() where id={0}", selectedrow.rdm.id, (int)ReceiptStatus.exceptionEnd, WPF_Common.CommonLogin.CommonUser.UserName));
                        Golbal_Sql.Add(SQLTxt.GetInserLogSql(CommonLogin.CommonUser.UserName, EnumHelper.GetEnumTextVal((int)RefundOperatorEnum.Mulit_Handle, typeof(RefundOperatorEnum)), selectedrow.rdm.refundNo, selectedrow.rdm.sku));

                        selectedrow.rdm.confirmReceipt = (int)ReceiptStatus.exceptionEnd;
                        selectedrow.rdm.exceptionEmployee = CommonLogin.CommonUser.UserName;
                        selectedrow.rdm.exceptionTime = DateTime.Now;

                        RefreshDataSource();

                    }
                }
                catch (Exception err)
                {
                    MessageBox.Show("失败原因:" + err.Message.ToString(), "提示");
                }

            }


            #endregion
        }

        #region 多退款&未知款操作

        private void btn_ExcHandle(object sender, RoutedEventArgs e)
        {
            string Tag = ((Button)sender).Tag.ToString();
            //多退款
            if (Tag.Equals("multi"))
            {
                var model = Golbal_shopModel.Find(a => a.shopName.Equals(txtShopName.Text.ToString()));
                if (model.isHaveApi.Equals(true))
                {
                    _9M.Work.WPF_Main.Infrastrcture.FormInit.OpenDialog(this, new AddGoods(Golbal_RefundModel.tradeNo, Golbal_RefundModel.refundNo,null), true, 2);
                }
                else
                {
                    MessageBox.Show("此店铺未设置KEY", "提示");
                }
            }
            else if (Tag.Equals("unknown"))//未知款
            {
                _9M.Work.WPF_Main.Infrastrcture.FormInit.OpenDialog(this, new AddUnkonwn(Golbal_RefundModel.refundNo), true, 2);
            }

        }

        #endregion

        #region 财务转即时到帐&未收到货

        private void btn_FXOperator(object sender, RoutedEventArgs e)
        {
            GridItemDataSource selectedrow = Handlerslist.SelectedItem as GridItemDataSource;

            if (((Button)sender).Content.Equals("转即时到帐"))
            {
                _9M.Work.WPF_Main.Infrastrcture.FormInit.OpenDialog(this, new FinanceJSDZ(selectedrow.rdm.id, txtNickName.Text.ToString(), selectedrow.rdm.goodsno, selectedrow.rdm.refundMoney, selectedrow.rdm.refundNo), true, 2);
            }
            else if (((Button)sender).Content.Equals("未收到货"))
            {
                Golbal_Sql.Add(string.Format("update T_RefundDetail set confirmReceipt={1} where id={0}", selectedrow.rdm.id, (int)ReceiptStatus.no));
                Golbal_Sql.Add(SQLTxt.GetInserLogSql(CommonLogin.CommonUser.UserName, EnumHelper.GetEnumTextVal((int)RefundOperatorEnum.Receive_No, typeof(RefundOperatorEnum)), selectedrow.rdm.refundNo, selectedrow.rdm.sku + "(订单转入异常)"));
                selectedrow.rdm.confirmReceipt = (int)ReceiptStatus.no;
                RefreshDataSource();
            }
        }

        #endregion

        #region 删除

        private void btnDel_Click(object sender, RoutedEventArgs e)
        {
            PwdDialog pd = new PwdDialog();

            if (pd.ShowDialog().Equals(true))
            {
                if (MessageBox.Show("确认删除？!是否要继续？", "提示", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    try
                    {
                        GridItemDataSource selectedrow = Handlerslist.SelectedItem as GridItemDataSource;

                        if (selectedrow != null)
                        {
                            if (selectedrow.rdm.id.Equals(0))//多退款
                            {
                                Golbal_Sql.Add(string.Format("delete T_Unknownlist where id={0}", selectedrow.rdm.realSku));
                            }
                            else
                            {
                                Golbal_Sql.Add(string.Format("delete T_RefundDetail where id={0}", selectedrow.rdm.id));
                            }

                            Golbal_Sql.Add(SQLTxt.GetInserLogSql(CommonLogin.CommonUser.UserName, EnumHelper.GetEnumTextVal((int)RefundOperatorEnum.Delete, typeof(RefundOperatorEnum)), selectedrow.rdm.refundNo, selectedrow.rdm.sku));

                            var erator = Handlerslist.ItemsSource.GetEnumerator();

                            var temp = new ObservableCollection<GridItemDataSource>();

                            while (erator.MoveNext())
                            {
                                if (!(erator.Current as GridItemDataSource).Equals(selectedrow))
                                {
                                    temp.Add(erator.Current as GridItemDataSource);
                                }
                            }

                            Handlerslist.ItemsSource = temp;
                            golbal_datasource = temp;
                        }
                    }
                    catch (Exception err)
                    {
                        MessageBox.Show("失败原因:" + err.Message.ToString(), "提示");
                    }
                }
            }
        }

        #endregion

        #region 修改退货原因

        private void btn_updateRefundReason(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("更新退货原因!是否要继续？", "提示", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    GridItemDataSource selectedrow = Handlerslist.SelectedItem as GridItemDataSource;

                    List<string> sql = new List<string>();
                    sql.Add(string.Format("update T_RefundDetail set refundReason={0} where id={1}", selectedrow.rdm.refundReason, selectedrow.rdm.id));
                    sql.Add(SQLTxt.GetInserLogSql(CommonLogin.CommonUser.UserName, EnumHelper.GetEnumTextVal((int)RefundOperatorEnum.updateRefundReaon, typeof(RefundOperatorEnum)), selectedrow.rdm.refundNo, selectedrow.rdm.sku));


                    if (dal.ExecuteTransaction(sql, null))
                    {
                        MessageBox.Show("操作成功", "提示");
                    }
                    else
                    {
                        MessageBox.Show("操作失败", "提示");
                    }
                }
                catch (Exception err)
                {
                    MessageBox.Show("失败原因:" + err.Message.ToString(), "提示");
                }
            }
        }

        #endregion

        #endregion

        #region 复制
        private void btn_copy(object sender, RoutedEventArgs e)
        {
            string tag = ((Button)sender).Tag.ToString();
            if (tag.Equals("realName"))
            {
                Clipboard.SetText(txtRealName.Text);
            }
            else if (tag.Equals("nick"))
            {
                Clipboard.SetText(txtNickName.Text);
            }
            else if (tag.Equals("expressNo"))
            {
                Clipboard.SetText(txtExpressNo.Text);
            }
            else if (tag.Equals("mobile"))
            {
                Clipboard.SetText(txtMobile.Text);
            }
            else if (tag.Equals("tbTradeNo"))
            {
                Clipboard.SetText(txtTbTradeNo.Text);
            }


        }

        #endregion

        #region 右键处理(转入拆包&转入财务&图片跳转链接)
        private void btn_goto(object sender, RoutedEventArgs e)
        {

            try
            {
                string p = ((MenuItem)sender).Tag.ToString();

                if (p.Equals("link"))
                {
                    try
                    {
                        GridItemDataSource selectedrow = Handlerslist.SelectedItem as GridItemDataSource;
                        // OnlineStoreAuthorization AuthorizationModel = _9M.Work.WPF_Common.CommonLogin.CommonUser.CShop;
                        TopSource com = new TopSource();
                        List<string> outeridlist = new List<string>();
                        if (!string.IsNullOrEmpty(selectedrow.rdm.goodsno))
                        {
                            outeridlist.Add(selectedrow.rdm.goodsno);
                            var info = com.GetItemList(outeridlist, null);
                            if (info[0].NumIid > 0)
                            {
                                System.Diagnostics.Process.Start("https://item.taobao.com/item.htm?id=" + info[0].NumIid);
                            }
                        }
                        else
                        {
                            MessageBox.Show("当前商品未记录商品款号,无法打开商品详情", "提示");
                        }
                    }
                    catch (Exception err)
                    {
                        MessageBox.Show(string.Format("错误提示@{0}@", err.Message.ToString()), "提示");
                    }
                }
                else if (p.Equals("print"))
                {
                    GridItemDataSource selectedrow = Handlerslist.SelectedItem as GridItemDataSource;
                    if (selectedrow.rdm.sku != null)
                    {
                        try
                        {
                            List<string> list = new List<string>() { selectedrow.rdm.sku };
                            ph.PrintLabel(list);
                        }
                        catch (Exception err)
                        {
                            MessageBox.Show(string.Format("错误提示:{0}", err.Message.ToString()));
                        }
                    }
                }
                else if (p.Equals("updateremark"))
                {
                    GridItemDataSource selectedrow = Handlerslist.SelectedItem as GridItemDataSource;
                    _9M.Work.WPF_Main.Infrastrcture.FormInit.OpenDialog(this, new EditInfo(selectedrow.rdm, typeof(UnpackingCheck)), true, 3);
                }
                else if (p.Equals("MuiltWattinigHandle"))
                {
                    if (currentHandleStatus == RefundHandleStatusEnum.ExceptionHandler)
                    {
                        if (CommonLogin.CommonUser.IsDeptAdmin || CommonLogin.CommonUser.IsAdmin)
                        {
                            try
                            {
                                GridItemDataSource selectedrow = Handlerslist.SelectedItem as GridItemDataSource;
                                List<string> sql = new List<string>();
                                sql.Add(string.Format("update T_RefundDetail set confirmReceipt={0},IsNotRegister=1 where refundNo='{1}'", (int)ReceiptStatus.yes, selectedrow.rdm.id));
                                sql.Add(SQLTxt.GetInserLogSql(CommonLogin.CommonUser.UserName, EnumHelper.GetEnumTextVal((int)RefundOperatorEnum.goMuiltHandle, typeof(RefundOperatorEnum)), Golbal_RefundModel.refundNo, selectedrow.rdm.sku));
                                if (dal.ExecuteTransaction(sql, null))
                                {
                                    MessageBox.Show("转入成功！", "提示");
                                }
                                else
                                {
                                    MessageBox.Show("转入失败！", "提示");
                                }
                            }
                            catch
                            { }

                        }
                        else
                        {
                            MessageBox.Show("你当前没有权限进行操作", "提示");
                        }
                    }

                }
                else if (p.Equals("Split"))
                {
                    //MessageBox.Show("开发中。。。。", "提示");

                    if (Handlerslist.SelectedItems.Count > 0)
                    {
                        if (MessageBox.Show("确认继续拆单", "提示", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        {

                            List<string> sql = new List<string>();
                            string newRefundNo = SerialNoHelp.rtnRefundNo();
                            sql.Add(string.Format(@"insert into T_Refund(refundNo,shopId,tradeNo,sendPostcode,refundStatus,financeRefundStatus,tbnick,mobile,RealName,regTime,regEmployee,remark,refundAmount,confirmAmount) 
                                                    select '{0}',shopId,tradeNo,sendPostcode,refundStatus,financeRefundStatus,tbnick,mobile,RealName,regTime,regEmployee,remark,{2},confirmAmount from T_Refund where refundNo='{1}'",
                                                    newRefundNo, Golbal_RefundModel.refundNo, Handlerslist.SelectedItems.Count));


                            ObservableCollection<GridItemDataSource> datasource = Handlerslist.ItemsSource as ObservableCollection<GridItemDataSource>;
                            IEnumerator datasourceIEnumerator = datasource.GetEnumerator();

                            ObservableCollection<GridItemDataSource> newRefundList = new ObservableCollection<GridItemDataSource>();
                            IEnumerator selectedIEnumerator = Handlerslist.SelectedItems.GetEnumerator();

                            while (selectedIEnumerator.MoveNext())
                            {
                                sql.Add(string.Format("update T_RefundDetail set refundNo='{0}' where id={1}", newRefundNo, ((GridItemDataSource)selectedIEnumerator.Current).rdm.id));
                                newRefundList.Add(selectedIEnumerator.Current as GridItemDataSource);
                                sql.Add(SQLTxt.GetInserLogSql(CommonLogin.CommonUser.UserName, EnumHelper.GetEnumTextVal((int)RefundOperatorEnum.Split, typeof(RefundOperatorEnum)), newRefundNo, Golbal_RefundModel.refundNo + ":" + ((GridItemDataSource)selectedIEnumerator.Current).rdm.sku));
                                sql.Add(SQLTxt.GetInserLogSql(CommonLogin.CommonUser.UserName, EnumHelper.GetEnumTextVal((int)RefundOperatorEnum.Split, typeof(RefundOperatorEnum)), Golbal_RefundModel.refundNo, newRefundNo + ":" + ((GridItemDataSource)selectedIEnumerator.Current).rdm.sku));
                            }

                            ObservableCollection<GridItemDataSource> oldRefundList = new ObservableCollection<GridItemDataSource>();

                            while (datasourceIEnumerator.MoveNext())
                            {
                                if (newRefundList.Where(a => a.rdm.id.Equals(((GridItemDataSource)datasourceIEnumerator.Current).rdm.id)).Count().Equals(0))
                                {
                                    oldRefundList.Add(datasourceIEnumerator.Current as GridItemDataSource);
                                }
                            }

                            //修改原单收货数量和退货数量
                            sql.Add(SQLTxt.GetReceiveCountSql(Golbal_RefundModel.refundNo));
                            var sqlNewCollection = SQLTxt.GetRefundStatus(newRefundList, newRefundNo, currentHandleStatus);
                            sqlNewCollection.ForEach(a => sql.Add(a));


                            //修改新单的收货数量和退货数量
                            sql.Add(SQLTxt.GetReceiveCountSql(newRefundNo));
                            SQLTxt.GetRefundStatus(oldRefundList, Golbal_RefundModel.refundNo, currentHandleStatus);
                            var sqlOldCollection = SQLTxt.GetRefundStatus(newRefundList, Golbal_RefundModel.refundNo, currentHandleStatus);
                            sqlOldCollection.ForEach(a => sql.Add(a));

                            if (dal.ExecuteTransaction(sql, null))
                            {
                                MessageBox.Show("拆单成功！", "提示");
                                dataBind(Golbal_RefundModel.refundNo);
                            }
                            else
                            {
                                MessageBox.Show("拆单失败", "提示");
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("请选择要拆单的数据", "提示");
                    }
                }
                else
                {
                    List<string> sql = new List<string>();

                    if (p.Equals("Excepion"))
                    {
                        if (MessageBox.Show("继续转入异常？", "提示", MessageBoxButton.YesNo) == MessageBoxResult.No)
                        {
                            return;
                        }
                        sql.Add(string.Format("update t_Refund set refundStatus={0} where refundNo='{1}'", (int)RefundSatausEnum.refundException, Golbal_RefundModel.refundNo));

                        sql.Add(SQLTxt.GetInserLogSql(CommonLogin.CommonUser.UserName, EnumHelper.GetEnumTextVal((int)RefundOperatorEnum.GoException, typeof(RefundOperatorEnum)), Golbal_RefundModel.refundNo, ""));
                    }
                    else if (p.Equals("Unpacking"))
                    {
                        if (MessageBox.Show("确定要转至拆包？", "提示", MessageBoxButton.YesNo) == MessageBoxResult.No)
                        {
                            return;
                        }
                        sql.Add(string.Format("update t_Refund set refundStatus={0} where refundNo='{1}'", (int)RefundSatausEnum.noUnpacking, Golbal_RefundModel.refundNo));
                        sql.Add(SQLTxt.GetInserLogSql(CommonLogin.CommonUser.UserName, EnumHelper.GetEnumTextVal((int)RefundOperatorEnum.GoUnpacking, typeof(RefundOperatorEnum)), Golbal_RefundModel.refundNo, ""));
                    }
                    else if (p.Equals("Finance"))
                    {
                        if (Golbal_RefundModel.refundStatus.Equals((int)RefundSatausEnum.refundException))
                        {
                            MessageBox.Show("还有未处理的异常，无法转至财务", "提示");
                            return;
                        }

                        if (Golbal_RefundModel.refundStatus.Equals((int)RefundSatausEnum.refundException))
                        {
                            MessageBox.Show("已完成的订单，无法转至财务", "提示");
                            return;
                        }

                        if (MessageBox.Show("确定要转至财务？", "提示", MessageBoxButton.YesNo) == MessageBoxResult.No)
                        {
                            return;
                        }
                        sql.Add(SQLTxt.GetInserLogSql(CommonLogin.CommonUser.UserName, EnumHelper.GetEnumTextVal((int)RefundOperatorEnum.GoFinance, typeof(RefundOperatorEnum)), Golbal_RefundModel.refundNo, ""));
                        sql.Add(string.Format("update t_Refund set refundStatus={0} where refundNo='{1}'", (int)RefundSatausEnum.financeOperator, Golbal_RefundModel.refundNo));
                    }
                    if (dal.ExecuteTransaction(sql, null))
                    {
                        MessageBox.Show("转入成功！", "提示");
                    }
                    else
                    {
                        MessageBox.Show("转入失败！", "提示");
                    }
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(string.Format("错误提示:@{0}@", err.Message.ToString()));
            }

        }

        #endregion

        #region 批量保存
        private void btn_batchSave(object sender, RoutedEventArgs e)
        {
            string expressCompany = EnumHelper.GetEnumTextVal((int)cmbExpressCompanybatch.SelectedValue, typeof(ExpressCompanyEnum));
            string expressCode = txtPostCodebatch.Text.Trim();
            if (expressCode.Trim().Length > 0)
            {
                List<string> sql = new List<string>();
                sql.Add(string.Format("update T_RefundDetail set expressCompany='{0}',expressCode='{1}' where refundnO='{2}'", expressCompany, expressCode, Golbal_RefundModel.refundNo));
                sql.Add(SQLTxt.GetInserLogSql(CommonLogin.CommonUser.UserName, EnumHelper.GetEnumTextVal((int)RefundOperatorEnum.BatchUpdatePostCode, typeof(RefundOperatorEnum)), Golbal_RefundModel.refundNo, ""));
                if (dal.ExecuteTransaction(sql, null))
                {
                    MessageBox.Show("批量修改成功！", "提示");
                    dataBind(Golbal_RefundModel.refundNo);
                }
                else
                {
                    MessageBox.Show("信息未填写完整！", "提示");
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

        private void btn_searchRow(object sender, RoutedEventArgs e)
        {
            searchRow();
        }

        #endregion

        #region 回车
        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                searchRow();
            }
        }

        #endregion

        #region 行查找
        void searchRow()
        {
            if (txtSearch.Text.Trim() != "")
            {
                var gEnumerator = Handlerslist.ItemsSource.GetEnumerator();
                int i = 0;
                while (gEnumerator.MoveNext())
                {
                    var isContains = (gEnumerator.Current as GridItemDataSource).rdm.sku.Contains(txtSearch.Text.Trim());

                    if (isContains)
                    {
                        var datasource = Handlerslist.ItemsSource as ObservableCollection<GridItemDataSource>;
                        datasource.Move(i, 0);
                        Handlerslist.SelectedIndex = 0;
                        break;
                    }
                    i++;
                }
            }
        }
        #endregion

        #region 筛选

        ObservableCollection<GridItemDataSource> golbal_datasource = new ObservableCollection<GridItemDataSource>();
        void filter(FilterEnum fe)
        {
            ObservableCollection<GridItemDataSource> temp = new ObservableCollection<GridItemDataSource>();

            var gEnumerator = golbal_datasource.GetEnumerator();
            while (gEnumerator.MoveNext())
            {
                var gids = gEnumerator.Current as GridItemDataSource;

                if (fe == FilterEnum.Many)
                {
                    if (gids.rdm.IsNotRegister.Equals(true))
                    {
                        temp.Add(gids);
                    }
                }
                else if (fe == FilterEnum.Less)
                {
                    if (gids.rdm.confirmReceipt.Equals((int)ReceiptStatus.no))
                    {
                        temp.Add(gids);
                    }
                }
            }

            if (fe == FilterEnum.All)
            {
                Handlerslist.ItemsSource = golbal_datasource;
            }
            else
            {
                Handlerslist.ItemsSource = temp;
            }



        }

        private void filterDatasource_Click(object sender, RoutedEventArgs e)
        {
            int Val = Convert.ToInt32(((RadioButton)sender).Tag);
            FilterEnum fe = (FilterEnum)Enum.ToObject(typeof(FilterEnum), Val);
            filter(fe);
        }

        #endregion

        #region 批量确认完成

        private void btn_batchFinanceEnd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MessageBox.Show("确认批量确认退款操作", "提示") == MessageBoxResult.Yes)
                {
                    List<string> sql = new List<string>();
                    sql.Add(string.Format("update T_RefundDetail set isFinanceEnd=1 where refundNo='{0}'", Golbal_RefundModel.refundNo));
                    sql.Add(string.Format("update T_Refund set refundStatus={0},endTime=getdate() where refundNo='{1}'", (int)RefundSatausEnum.End, Golbal_RefundModel.refundNo));
                    sql.Add(SQLTxt.GetInserLogSql(CommonLogin.CommonUser.UserName, EnumHelper.GetEnumTextVal((int)RefundOperatorEnum.batchConfirmFinanceEnd, typeof(RefundOperatorEnum)), Golbal_RefundModel.refundNo, ""));
                    if (dal.ExecuteTransaction(sql, null))
                    {
                        dataBind(Golbal_RefundModel.refundNo);
                    }
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message.ToString());
            }
        }

        #endregion

        #region 保存提交

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            if (currentHandleStatus == RefundHandleStatusEnum.UnpackingCheck)
            {
                Golbal_Sql.Add(string.Format("update T_Refund set unpackingTime=getdate() where refundNo='{0}'", Golbal_RefundModel.refundNo));
            }

            //确认收货数量
            Golbal_Sql.Add(SQLTxt.GetReceiveCountSql(Golbal_RefundModel.refundNo));

            //修改订单状态
            var sqllist = rtunRefundStatus(Golbal_RefundModel.refundNo);
            foreach (string s in sqllist)
            {
                Golbal_Sql.Add(s);
            }

            try
            {
                if (dal.ExecuteTransaction(Golbal_Sql, null))
                {
                    Golbal_Sql.Clear();
                    MessageBox.Show("数据提交成功!", "提示");
                    CloseDialog();

                }
                else
                {
                    MessageBox.Show("数据提交遇到未知错误!", "提示");
                    return;
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message.ToString(), "提示");
                return;
            }

        }

        #endregion

        #region 根据退货明细修改退货单状态

        private List<string> rtunRefundStatus(string refundNo)
        {
            ObservableCollection<GridItemDataSource> datasource = Handlerslist.ItemsSource as ObservableCollection<GridItemDataSource>;
            return SQLTxt.GetRefundStatus(datasource, refundNo, currentHandleStatus);
        }

        #endregion

        #region Dialog
        public Microsoft.Practices.Prism.Commands.DelegateCommand CancelCommand
        {
            get { return new DelegateCommand(CloseDialog); }
        }

        public void CloseDialog()
        {
            bool isClose = true;
            if (Golbal_Sql.Count > 0)
            {
                if (MessageBox.Show("当前检测到有修改未保存,是否续续关闭？", "提示", MessageBoxButton.YesNo) == MessageBoxResult.No)
                {
                    isClose = false;
                }
            }

            if (isClose)
            {
                Refundlist rl = ((Refundlist)FormInit.FindFather(this));
                rl.LoadRefundData(Convert.ToInt32(rl.CurrentPage), rl.PageSize);
                FormInit.CloseDialog(this);
            }
        }

        public string Title
        {
            get { return "退货处理"; }
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

#region 自定义列表绑定结构

public class RefundDetailGridModel
{
    public string GridButtonName { get; set; }
    public Visibility GridButtonVisible { get; set; }
    public string GridTextBlock { get; set; }
    public Visibility GridTextBlockVisible { get; set; }
    public List<EnumEntity> ExpressCompanyItem { get; set; }
    public Visibility FXHandleButton { get; set; }
    public string FxHandleButtonName { get; set; }
}

public class GridItemDataSource
{
    public RefundDetailGridModel rdgm { get; set; }
    public RefundDetailModel rdm { get; set; }
}

#endregion

public class SQLTxt
{

    #region  修改已收货数量和退货数量
    public static string GetReceiveCountSql(string refundNo)
    {
        return string.Format(@"UPDATE T_Refund SET confirmAmount=(
                                   select COUNT(*) from T_RefundDetail where refundNo='{0}' and (confirmReceipt='{1}' or (IsNotRegister=1 and confirmReceipt='{2}'))
                                   ), refundAmount=(select count(*) from T_RefundDetail where refundNo='{0}') where refundNo='{0}'",
                               refundNo, (int)ReceiptStatus.yes, (int)ReceiptStatus.exceptionEnd);
    }
    #endregion

    #region 插入日志
    public static string GetInserLogSql(string oper, string eventName, string refundNo, string remark)
    {
        return string.Format(@"Insert into T_RefundLog(oper,operTime,eventName,refundNo,remark) values('{0}',getdate(),'{1}','{2}','{3}')", oper, eventName, refundNo, remark);
    }


    #endregion

    #region 返回状态

    public static List<string> GetRefundStatus(ObservableCollection<GridItemDataSource> datasource, string refundNo, RefundHandleStatusEnum rhse)
    {

        List<string> sql = new List<string>();

        if (datasource.Where(a => a.rdm.confirmReceipt.Equals((int)ReceiptStatus.no)).Count() > 0)//未收货
        {
            sql.Add(string.Format("update T_Refund set refundStatus='{1}' where refundNo='{0}'", refundNo, (int)RefundSatausEnum.refundException));
        }
        else if (datasource.Where(a => a.rdm.confirmReceipt.Equals((int)ReceiptStatus.no) && a.rdm.IsNotRegister.Equals(true)).Count() > 0)//多退款未收货(异常未处理的)
        {
            sql.Add(string.Format("update T_Refund set refundStatus='{1}' where refundNo='{0}'", refundNo, (int)RefundSatausEnum.refundException));
        }
        else if (datasource.Where(a => a.rdm.confirmReceipt.Equals((int)ReceiptStatus.yes) && a.rdm.IsNotRegister.Equals(true)).Count() > 0)//多退款已收货(异常未处理的)拆包添加的多退款
        {
            sql.Add(string.Format("update T_Refund set refundStatus='{1}' where refundNo='{0}'", refundNo, (int)RefundSatausEnum.refundException));
        }
        else if (datasource.Where(a => a.rdm.confirmReceipt.Equals((int)ReceiptStatus.watting)).Count() > 0)// 修改为拆包
        {
            sql.Add(string.Format("update T_Refund set refundStatus='{1}' where refundNo='{0}'", refundNo, (int)RefundSatausEnum.noUnpacking));
        }
        else if (datasource.Where(a => a.rdm.isFinanceEnd.Equals(true)).Count() == datasource.Count())//修改为完成并取消颜色
        {
            if (datasource.Where(a => a.rdm.refundReason.Equals((int)RefundReason.seller)).Count() > 0)//有质量问题的修改为退邮费
            {
                sql.Add(string.Format(@"update T_Refund set refundStatus='{1}' where refundNo='{0}'", refundNo, (int)RefundSatausEnum.Postage));
            }
            else
            {
                sql.Add(string.Format(@"update T_Refund set refundStatus='{1}',endTime=getdate() where refundNo='{0}'", refundNo, (int)RefundSatausEnum.End));//非质量问题的修改为完成
            }
            sql.Add(string.Format(@"update T_Refund set flagColor=0 where refundNo='{0}'", refundNo)); //取消标记的颜色
        }
        else if (datasource.Where(a => a.rdm.isFinanceEnd.Equals(false)).Count() > 0 && rhse == RefundHandleStatusEnum.RefundPostCode) //完成退邮费的
        {
            sql.Add(string.Format(@"update T_Refund set refundStatus='{1}',endTime=getdate() where refundNo='{0}'", refundNo, (int)RefundSatausEnum.End));
        }
        else
        {
            if (CommonLogin.CommonUser.DeptId.Equals(2))//拆包
            {
                //拆包处理完后，明细都是已收货但是未注册的异常订单
                if (datasource.Where(a => a.rdm.confirmReceipt.Equals((int)ReceiptStatus.yes) && a.rdm.IsNotRegister.Equals(true)).Count() > 0)
                {
                    sql.Add(string.Format("update T_Refund set refundStatus='{1}' where refundNo='{0}'", refundNo, (int)RefundSatausEnum.refundException));//修改为异常状态
                }
                else
                {
                    //客服处理完后，明细都是已收货( IsNotRegister=true && ReceiptStatus.exceptionEnd || IsNotRegister=false && ReceiptStatus.yes  )
                    sql.Add(string.Format("update T_Refund set refundStatus='{1}' where refundNo='{0}'", refundNo, (int)RefundSatausEnum.financeOperator));//修改为财务退款 
                }
            }
            else
            {
                //客服处理完后，明细都是已收货
                sql.Add(string.Format("update T_Refund set refundStatus='{1}' where refundNo='{0}'", refundNo, (int)RefundSatausEnum.financeOperator));//修改为财务退款 
            }
        }

        return sql;
    }


    #endregion

}