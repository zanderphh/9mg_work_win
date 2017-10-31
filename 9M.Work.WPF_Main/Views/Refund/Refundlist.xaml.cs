using _9M.Work.DbObject;
using _9M.Work.ErpApi;
using _9M.Work.Model;
using _9M.Work.TopApi;
using _9M.Work.Utility;
using _9M.Work.WPF_Common;
using _9M.Work.WPF_Common.Controls;
using _9M.Work.WPF_Common.ValueObjects;
using _9Mg.Work.TopApi;
using Microsoft.Practices.Prism.Commands;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
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
    /// Interaction logic for Refundlist.xaml
    /// </summary>
    public partial class Refundlist : UserControl
    {

        BaseDAL dal = new BaseDAL();
        public List<ShopModel> ShopList = null;
        /// <summary>
        /// 退货单枚举RefundSatausEnum(默认为全部退货单-1)
        /// </summary>
        public int Query_P_RefundStatus = -1;

        delegate void AsyncInitLoadingHandle();

        public Refundlist()
        {
            InitializeComponent();
            this.DataContext = this;
            ShopComboxBind();
            //new Thread(() =>
            //{
            //    this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Send, new Action(delegate
            //    {
            //        syncExpressInfo();
            //    }));
              
            //}).Start();

            new Thread(() => { InitLoading(); }).Start();
        }

        #region 属性

        private int _updateNum = 0;
        public int UpdateNum
        {
            get { return _updateNum; }
            set
            {
                if (_updateNum != value)
                {
                    _updateNum = value;
                    this.OnPropertyChanged("UpdateNum");
                }
            }
        }

        private int _totalNum = 0;
        public int TotalNum
        {
            get { return _totalNum; }
            set
            {
                if (_totalNum != value)
                {
                    _totalNum = value;
                    this.OnPropertyChanged("TotalNum");
                }
            }
        }


        private string _updateTip;
        public string UpdateTip
        {
            get { return _updateTip; }
            set
            {
                if (_updateTip != value)
                {
                    _updateTip = value;
                    this.OnPropertyChanged("UpdateTip");
                }
            }
        }


        private ObservableCollection<EnumElementEntity> _tabCollection;
        public ObservableCollection<EnumElementEntity> TabCollection
        {
            get { return _tabCollection; }
            set
            {
                if (_tabCollection != value)
                {
                    _tabCollection = value;
                    this.OnPropertyChanged("TabCollection");
                }
            }
        }


        #endregion

        #region 同步快递信息

        void syncExpressInfo()
        {
            try
            {
                var lastUpdateTime = dal.QueryDataTable<LastSyncExpressTimeModel>("select id,syncTime from T_LastSyncExpressTime");
                DateTime dt1 = lastUpdateTime[0].syncTime;
                DateTime dt2 = DateTime.Now;
                double HoursInterval1 = dt2.Subtract(dt1).TotalHours;
                if (HoursInterval1 > 1)
                {
                    loading.Visibility = loadingTip1.Visibility = System.Windows.Visibility.Visible;
                    AsyncGetExpressInfoHandle handle = new AsyncGetExpressInfoHandle(GetExpressInfo);
                    handle.BeginInvoke(null, AsyncGetExpressInfoHandleCallback, null);
                }
            }
            catch (Exception Ecp) { string err = Ecp.ToString(); }
        }

        #endregion

        #region 初始化绑定

        void InitLoading()
        {
            System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    TabCollection = new ObservableCollection<EnumElementEntity>();
                    TabButtonInitBind();
                    CommboxColorFlagBind();

                    labMsg.Content = LabStatisticsTip();
                    NextPageSearchCommand = new DelegateCommand(NextPageSearchCommandFunc);
                    LoadRefundData(Convert.ToInt32(CurrentPage), PageSize);
                }
                catch
                { }
            }));
        }

        #endregion

        #region 店铺绑定
        public void ShopComboxBind()
        {
            ShopList = dal.GetAll<ShopModel>();
            ShopModel m = new ShopModel() { id = -1, shopId = -1, shopName = "所有店铺" };
            ShopList.Insert(0, m);
            shopCombox.ItemsSource = ShopList;
            shopCombox.SelectedItem = m;
        }

        #endregion

        #region 未退单&异常单统计

        string LabStatisticsTip()
        {
            try
            {
                var s = dal.QueryList<StatisticsTip>(string.Format(@" Select refundStatus,COUNT(*) as [refundAmount] from dbo.T_Refund where (Datediff(HH,regTime,GETDATE())/24)>0 and (refundStatus={0} or  refundStatus={1})
                                                                    group by refundStatus", (int)RefundSatausEnum.noUnpacking, (int)RefundSatausEnum.refundException));


                StatisticsTip noRefund = s.Find(a => a.refundStatus.Equals((int)RefundSatausEnum.noUnpacking));
                int? noRefundCount = (noRefund == null) ? 0 : noRefund.refundAmount;

                StatisticsTip exception = s.Find(a => a.refundStatus.Equals((int)RefundSatausEnum.refundException));
                int? exceptionCount = (exception == null) ? 0 : exception.refundAmount;

                return string.Format("当前超过{0}天未退单,未退款{1}单,退货异常{2}单", 1, noRefundCount, exceptionCount);
            }
            catch (Exception err)
            {
                return err.ToString();
            }
        }

        #endregion

        #region 退货状态切换按钮绑定
        public void TabButtonInitBind()
        {
            List<EnumElementEntity> list = EnumHelper.GetEnumEnumerable(typeof(RefundSatausEnum));
            list.Remove(list.Find(a => a.Value.Equals((int)RefundSatausEnum.financePartOperator)));

            list.ForEach(delegate(EnumElementEntity ee)
            {
                ee.Background = (ee.Value.Equals(Query_P_RefundStatus)) ? "#c5e5f7" : "#ffffff";
                TabCollection.Add(ee);
            });

        }

        #endregion

        #region 颜色标记选项
        public void CommboxColorFlagBind()
        {
            ObservableCollection<EnumElementEntity> cmbDataSource = new ObservableCollection<EnumElementEntity>();

            List<EnumElementEntity> list = EnumHelper.GetEnumEnumerable(typeof(FlagColorEnume));
            //list.Remove(list.Find(a => a.Value.Equals((int)FlagColorEnume.noFlag)));
            cmbDataSource.Add(new EnumElementEntity() { Text = "全部颜色", Value = -1 });
            list.ForEach(delegate(EnumElementEntity ee)
            {
                cmbDataSource.Add(ee);
            });

            cmbColorFlag.ItemsSource = cmbDataSource;
            cmbColorFlag.SelectedValue = -1;
            cmbColorFlag.SelectionChanged += cmbColorFlag_SelectionChanged;
        }


        #endregion

        #region 查找子控件

        private void FindChildByType(DependencyObject relate, Type type, ref FrameworkElement resElement)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(relate); i++)
            {
                var el = VisualTreeHelper.GetChild(relate, i) as FrameworkElement;
                if (el.GetType() == type)
                {
                    resElement = el;
                    return;
                }
                else
                {
                    FindChildByType(el, type, ref resElement);
                }
            }
        }

        #endregion

        #region 状态列表切换
        private void TabButton_Click(object sender, RoutedEventArgs e)
        {
            Query_P_RefundStatus = Convert.ToInt32(((Button)sender).Tag);

            foreach (var item in MgTabButton.Items)
            {
                var el = MgTabButton.ItemContainerGenerator.ContainerFromItem(item) as FrameworkElement;

                if (el != null)
                {
                    var cp = el as ContentPresenter;
                    FrameworkElement efind = default(FrameworkElement);
                    FindChildByType(cp, typeof(Button), ref efind);
                    if (efind is Button)
                    {
                        Color color = efind.Tag.Equals(Query_P_RefundStatus) ? (Color)ColorConverter.ConvertFromString("#c5e5f7") : (Color)ColorConverter.ConvertFromString("#ffffff");
                        Button btn = efind as Button;
                        btn.Background = new SolidColorBrush(color);
                    }
                }
            }

            CurrentPage = "1";
            new Thread(() =>
            {
                System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    LoadRefundData(Convert.ToInt32(CurrentPage), PageSize);
                }));
            }).Start();

        }
        #endregion

        #region 采购单数据列表绑定

        void bind(List<RefundModel> resultlist, ObservableCollection<RefundModel> targetCollection)
        {
            new Thread(() =>
                   {
                       foreach (RefundModel m in resultlist)
                       {
                           m.handleDays = DateDiff(m.regTime, DateTime.Now);
                           if (m.refundStatus.Equals((int)RefundSatausEnum.noUnpacking))
                           {
                               m.dColumn1 = EnumHelper.GetEnumTextVal((int)RefundHandleStatusEnum.UnpackingCheck, typeof(RefundHandleStatusEnum));
                               m.dColumn2 = ((int)RefundHandleStatusEnum.UnpackingCheck).ToString();
                           }
                           else if (m.refundStatus.Equals((int)RefundSatausEnum.refundException))
                           {
                               m.dColumn1 = EnumHelper.GetEnumTextVal((int)RefundHandleStatusEnum.ExceptionHandler, typeof(RefundHandleStatusEnum));
                               m.dColumn2 = ((int)RefundHandleStatusEnum.ExceptionHandler).ToString();
                           }
                           else if (m.refundStatus.Equals((int)RefundSatausEnum.financeOperator))
                           {
                               m.dColumn1 = EnumHelper.GetEnumTextVal((int)RefundHandleStatusEnum.FinancialRefund, typeof(RefundHandleStatusEnum));
                               m.dColumn2 = ((int)RefundHandleStatusEnum.FinancialRefund).ToString();
                           }
                           else if (m.refundStatus.Equals((int)RefundSatausEnum.financePartOperator))
                           {
                               m.dColumn1 = EnumHelper.GetEnumTextVal((int)RefundHandleStatusEnum.FinancialPartRefund, typeof(RefundHandleStatusEnum));
                               m.dColumn2 = ((int)RefundHandleStatusEnum.FinancialPartRefund).ToString();
                           }
                           else if (m.refundStatus.Equals((int)RefundSatausEnum.Postage))
                           {
                               m.dColumn1 = EnumHelper.GetEnumTextVal((int)RefundHandleStatusEnum.RefundPostCode, typeof(RefundHandleStatusEnum));
                               m.dColumn2 = ((int)RefundHandleStatusEnum.RefundPostCode).ToString();
                           }
                           else if (m.refundStatus.Equals((int)RefundSatausEnum.End))
                           {
                               m.dColumn1 = EnumHelper.GetEnumTextVal((int)RefundHandleStatusEnum.look, typeof(RefundHandleStatusEnum));
                               m.dColumn2 = ((int)RefundHandleStatusEnum.look).ToString();
                           }


                           this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Send, new Action(delegate
                           {
                               targetCollection.Add(m);
                           }));

                       }

                   }).Start();
        }

        /// <summary>
        /// 加载采购列表
        /// </summary>
        public void LoadRefundData(int pageIndex, int pageSize)
        {
            try
            {
                #region 查询条件

                //List<ExpressionModelField> listWhere = new List<ExpressionModelField>();
                //List<ExpressionModelField> fields_or = new List<ExpressionModelField>();

                //OrderModelField orderField = new OrderModelField() { PropertyName = "regTime", IsDesc = true };

                //if (Convert.ToInt32(shopCombox.SelectedValue) > -1)
                //{
                //    listWhere.Add(new ExpressionModelField() { Name = "shopId", Value = Convert.ToInt32(shopCombox.SelectedValue) });
                //}

                //if (Query_P_RefundStatus > -1)
                //{
                //    //if (Query_P_RefundStatus.Equals((int)RefundSatausEnum.noUnpacking))
                //    //{
                //    //    fields_or.Add(new ExpressionModelField() { Name = "refundStatus", Value = (Int32)RefundSatausEnum.noUnpacking });
                //    //    fields_or.Add(new ExpressionModelField() { Name = "refundStatus", Value = (Int32)RefundSatausEnum.financePartOperator });
                //    //}
                //    //else if (Query_P_RefundStatus.Equals((int)RefundSatausEnum.financeOperator))
                //    //{
                //    //    fields_or.Add(new ExpressionModelField() { Name = "refundStatus", Value = (Int32)RefundSatausEnum.financeOperator });
                //    //    fields_or.Add(new ExpressionModelField() { Name = "refundStatus", Value = (Int32)RefundSatausEnum.financePartOperator });
                //    //    fields_or.Add(new ExpressionModelField() { Name = "refundStatus", Value = (Int32)RefundSatausEnum.Postage });
                //    //}
                //    //else
                //    //{
                //    //    listWhere.Add(new ExpressionModelField() { Name = "refundStatus", Value = Query_P_RefundStatus });
                //    //}

                //    listWhere.Add(new ExpressionModelField() { Name = "refundStatus", Value = Query_P_RefundStatus });
                //}

                //if (txtPNC.Text.Trim().Length > 0)
                //{
                //    fields_or.Add(new ExpressionModelField() { Name = "tradeNo", Value = txtPNC.Text.Trim(), Relation = EnumRelation.Contains });

                //    fields_or.Add(new ExpressionModelField() { Name = "mobile", Value = txtPNC.Text.Trim(), Relation = EnumRelation.Contains });

                //    fields_or.Add(new ExpressionModelField() { Name = "tbnick", Value = txtPNC.Text.Trim(), Relation = EnumRelation.Contains });

                //    fields_or.Add(new ExpressionModelField() { Name = "RealName", Value = txtPNC.Text.Trim(), Relation = EnumRelation.Contains });


                //    string sql = string.Format("select distinct refundNo from T_RefundDetail where expressCode like '%{0}%' or sku like '%{0}%'", txtPNC.Text.Trim());
                //    var RefundNo = dal.QueryDataTable<string>(sql, new object[] { });

                //    RefundNo.ForEach(delegate(string s)
                //    {
                //        fields_or.Add(new ExpressionModelField() { Name = "refundNo", Value = s });
                //    });

                //}

                #endregion

                #region 原始Sql

                ObservableCollection<RefundModel> ObserCollection = new ObservableCollection<RefundModel>();
                RefundGridlist.ItemsSource = ObserCollection;

                StringBuilder sbSql = new StringBuilder();
                StringBuilder sbSqlWhere = new StringBuilder();

                if (Convert.ToInt32(shopCombox.SelectedValue) > -1)
                {
                    sbSqlWhere.Append(string.Format(" and shopId={0}", Convert.ToInt32(shopCombox.SelectedValue)));
                }

                if ((int)cmbColorFlag.SelectedValue > -1)
                {
                    sbSqlWhere.Append(string.Format(" and flagColor={0}", (int)cmbColorFlag.SelectedValue));
                }

                if (Query_P_RefundStatus > -1)
                {
                    if (Query_P_RefundStatus.Equals((int)RefundSatausEnum.noUnpacking))
                    {
                        sbSqlWhere.Append(string.Format(" and refundStatus in({0},{1})", (Int32)RefundSatausEnum.noUnpacking, (Int32)RefundSatausEnum.financePartOperator));
                    }
                    else if (Query_P_RefundStatus.Equals((int)RefundSatausEnum.financeOperator))
                    {
                        sbSqlWhere.Append(string.Format(" and refundStatus in({0},{1})", (Int32)RefundSatausEnum.financeOperator, (Int32)RefundSatausEnum.financePartOperator));
                    }
                    else
                    {
                        sbSqlWhere.Append(string.Format(" and refundStatus={0}", Query_P_RefundStatus));
                    }
                }

                if (txtPNC.Text.Trim().Length > 0)
                {

                    string sql = string.Format("select distinct refundNo from T_RefundDetail where expressCode like '%{0}%' or sku like '%{0}%'", txtPNC.Text.Trim());

                    var RefundNo = dal.QueryDataTable<string>(sql, new object[] { });
                    var r = string.Empty;
                    RefundNo.ForEach(delegate(string s)
                    {
                        r += string.Format(" or refundNo='{0}'", s);
                    });

                    sbSqlWhere.Append(string.Format(" and ( tradeNo like '%{0}%' or  mobile like '%{0}%' or tbnick like '%{0}%' or RealName like '%{0}%' or refundNo like '%{0}%' or sendPostcode like '%{0}%' {1} )", txtPNC.Text.Trim(), r.ToString()));

                }

                System.Data.DataTable Count = dal.QueryDataTable(string.Format("select count(1) as [count] from T_Refund where 1=1 {0}", sbSqlWhere.ToString()), new object[] { });

                if (Count.Rows.Count > 0)
                {

                    int totalCount = Int32.Parse(Count.Rows[0][0].ToString());
                    TotalPage = (totalCount % PageSize == 0) ? (totalCount / PageSize).ToString() : ((totalCount / PageSize) + 1).ToString();
                    if (totalCount > 0)
                    {
                        sbSql.Append(string.Format(@"                                           
                                            select * from (
                                                        
                                               select ROW_NUMBER() Over(order by {3} desc) as rownumber,* from T_Refund where 1=1 {0}
                                                        
                                                  ) a where rownumber between {1} and {2} order by rownumber asc", sbSqlWhere.ToString(), (pageIndex - 1) * pageSize + 1, pageIndex * pageSize, ckShowTime.IsChecked.Equals(true) ? "unpackingTime" : "id"));

                        var rmCollection = dal.QueryList<RefundModel>(sbSql.ToString());
                        bind(rmCollection, ObserCollection);

                    }
                }

                #endregion
            }
            catch (Exception err)
            {
                string msg = err.Message.ToString();
            }

        }
        #endregion

        #region 分页相关属性

        delegate ObservableCollection<RefundModel> NextPageHandle();

        void NextPage()
        {
            LoadRefundData(Convert.ToInt32(CurrentPage), PageSize);
        }

        /// <summary>
        /// 分页查询命令
        /// </summary>
        private void NextPageSearchCommandFunc()
        {
            NextPage();
        }



        /// <summary>
        /// 总页数
        /// </summary>
        private string _totalPage = string.Empty;
        public string TotalPage
        {
            get { return _totalPage; }
            set
            {
                _totalPage = value;
                this.OnPropertyChanged("TotalPage");

            }
        }


        /// <summary>
        /// 当前页
        /// </summary>
        private string _currentPage = "1";
        public string CurrentPage
        {
            get { return _currentPage; }
            set
            {
                _currentPage = value;
                this.OnPropertyChanged("CurrentPage");
            }
        }


        /// <summary>
        /// 每页显示的记录数
        /// </summary>
        private int _pageSize = 100;
        public int PageSize
        {
            get { return _pageSize; }
            set
            {
                _pageSize = value;
                this.OnPropertyChanged("PageSize");
            }
        }

        /// <summary>
        /// 当前页索引
        /// </summary>
        private int _pageIndex;
        public int PageIndex
        {
            get { return _pageIndex; }
            set
            {
                _pageIndex = value;
                this.OnPropertyChanged("PageIndex");
            }
        }

        /// <summary>
        /// 记录总数
        /// </summary>
        private int _totalCount;
        public int TotalCount
        {
            get { return _totalCount; }
            set
            {
                _totalCount = value;

                this.OnPropertyChanged("_totalCount");
            }
        }
        /// <summary>
        /// 分页管理
        /// </summary>
        public ICommand NextPageSearchCommand { get; set; }

        #endregion

        #region 新建/编辑/删除/部分退款

        private void btnHandler_Click(object sender, RoutedEventArgs e)
        {

            string oper = sender.GetType().Equals(typeof(Button)) ? ((Button)sender).Tag.ToString() : ((MenuItem)sender).Tag.ToString();

            if (oper.Equals("0"))//新建
            {
                _9M.Work.WPF_Main.Infrastrcture.FormInit.OpenDialog(this, new RegisterRefund(null, OperationStatus.ADD), false);
            }
            else if (oper.Equals("1"))//编辑
            {
                RefundModel selectedrow = RefundGridlist.SelectedItem as RefundModel;

                if (selectedrow != null)
                {
                    if (selectedrow.refundStatus.Equals((int)RefundSatausEnum.noUnpacking))
                    {
                        _9M.Work.WPF_Main.Infrastrcture.FormInit.OpenDialog(this, new RegisterRefund(selectedrow.refundNo, OperationStatus.Edit), false);
                    }
                    else
                    {
                        MessageBox.Show("订单修改只能是未拆包状态", "提示");
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("选中要编辑的行", "提示");
                }
            }
            else if (oper.Equals("2"))//删除
            {

                PwdDialog pd = new PwdDialog();

                if (pd.ShowDialog().Equals(true))
                {
                    RefundModel selectedrow = RefundGridlist.SelectedItem as RefundModel;
                    if (selectedrow == null)
                    {
                        MessageBox.Show("选中要删除的行", "提示");
                    }
                    else
                    {
                        if (MessageBox.Show("删除后数据将无法恢复，是否继续要删除当前行?", "提示", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        {
                            try
                            {

                                List<string> sql = new List<string>();
                                sql.Add(string.Format("delete T_Refund where refundNo='{0}'", selectedrow.refundNo));
                                sql.Add(string.Format("delete T_RefundDetail where refundNo='{0}'", selectedrow.refundNo));

                                if (dal.ExecuteTransaction(sql, null))
                                {
                                    MessageBox.Show("删除成功", "提示");
                                    LoadRefundData(Convert.ToInt32(this.CurrentPage), this.PageSize);
                                }
                                else
                                {
                                    MessageBox.Show("删除失败", "提示");
                                }
                            }
                            catch (Exception err)
                            {
                                MessageBox.Show(string.Format("失败原因:{0}", err.Message.ToString()), "提示");
                            }

                        }
                    }
                }

            }
            else if (oper.Equals("4"))//手动同步快递单号
            {
                if (MessageBox.Show("确定同步快递单号?同步时间较长，需耐心等待", "提示", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    loading.Visibility = loadingTip1.Visibility = System.Windows.Visibility.Visible;

                    AsyncGetExpressInfoHandle handle = new AsyncGetExpressInfoHandle(GetExpressInfo);
                    handle.BeginInvoke(null, AsyncGetExpressInfoHandleCallback, null);
                }
            }
            else if (oper.Equals("6") || oper.Equals("7") || oper.Equals("8"))//颜色标记&取消
            {
                RefundModel selectedrow = RefundGridlist.SelectedItem as RefundModel;
                List<string> sql = new List<string>();

                if (MessageBox.Show("确认继续操作？", "提示", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    int flag = 0;
                    switch (oper)
                    {
                        case "6":
                            flag = (int)FlagColorEnume.noFlag;
                            sql.Add(SQLTxt.GetInserLogSql(CommonLogin.CommonUser.UserName, EnumHelper.GetEnumTextVal((int)RefundOperatorEnum.CancelFlag, typeof(RefundOperatorEnum)), selectedrow.refundNo, ""));
                            break;
                        case "7":
                            flag = (int)FlagColorEnume.AColor;
                            sql.Add(SQLTxt.GetInserLogSql(CommonLogin.CommonUser.UserName, EnumHelper.GetEnumTextVal((int)RefundOperatorEnum.WattingTack, typeof(RefundOperatorEnum)), selectedrow.refundNo, ""));
                            break;
                        case "8":
                            flag = (int)FlagColorEnume.BColor;
                            sql.Add(SQLTxt.GetInserLogSql(CommonLogin.CommonUser.UserName, EnumHelper.GetEnumTextVal((int)RefundOperatorEnum.AuditEnd, typeof(RefundOperatorEnum)), selectedrow.refundNo, ""));
                            break;

                    }

                    sql.Add(string.Format("update T_Refund set flagColor={1} where id={0}", selectedrow.id, flag));
                    if (dal.ExecuteTransaction(sql, null))
                    {
                        LoadRefundData(Convert.ToInt32(this.CurrentPage), this.PageSize);
                    }
                }
            }
            else if (oper.Equals("10"))
            {
                RefundModel selectedrow = RefundGridlist.SelectedItem as RefundModel;
                _9M.Work.WPF_Main.Infrastrcture.FormInit.OpenDialog(this, new LogList(selectedrow.refundNo), false);
            }
            else if (oper.Equals("11") || oper.Equals("12") || oper.Equals("13") || oper.Equals("3"))
            {

                if (CommonLogin.CommonUser.IsDeptAdmin || CommonLogin.CommonUser.IsAdmin)
                {
                    RefundModel selectedrow = RefundGridlist.SelectedItem as RefundModel;
                    List<string> sql = new List<string>();

                    if (oper.Equals("3"))
                    {
                        if (selectedrow.refundStatus.Equals((int)RefundSatausEnum.refundException))
                        {
                            MessageBox.Show("异常状态的订单无法转到部分退款", "提示");
                            return;
                        }
                        sql.Add(string.Format("update T_Refund set refundStatus={0} where id={1}", (int)RefundSatausEnum.financePartOperator, selectedrow.id));
                        sql.Add(SQLTxt.GetInserLogSql(CommonLogin.CommonUser.UserName, EnumHelper.GetEnumTextVal((int)RefundOperatorEnum.goPartRefund, typeof(RefundOperatorEnum)), selectedrow.refundNo, ""));
                        if (dal.ExecuteTransaction(sql, null))
                        {
                            MessageBox.Show("已转财务状态", "提示");
                        }
                    }
                    else if (oper.Equals("11"))
                    {
                        sql.Add(string.Format("update T_Refund set refundStatus={0},endTime=getdate() where id={1}", (int)RefundSatausEnum.End, selectedrow.id));
                        sql.Add(SQLTxt.GetInserLogSql(CommonLogin.CommonUser.UserName, EnumHelper.GetEnumTextVal((int)RefundOperatorEnum.goEnd, typeof(RefundOperatorEnum)), selectedrow.refundNo, ""));
                        if (dal.ExecuteTransaction(sql, null))
                        {
                            MessageBox.Show("已转完成状态", "提示");
                        }
                    }
                    else if (oper.Equals("12"))
                    {
                        sql.Add(string.Format("update T_Refund set refundStatus={0} where id={1}", (int)RefundSatausEnum.refundException, selectedrow.id));
                        sql.Add(SQLTxt.GetInserLogSql(CommonLogin.CommonUser.UserName, EnumHelper.GetEnumTextVal((int)RefundOperatorEnum.GoException, typeof(RefundOperatorEnum)), selectedrow.refundNo, ""));
                        if (dal.ExecuteTransaction(sql, null))
                        {
                            MessageBox.Show("已转异常状态", "提示");
                        }
                    }
                    else if (oper.Equals("13"))
                    {
                        sql.Add(string.Format("update T_Refund set refundStatus={0} where id={1}", (int)RefundSatausEnum.noUnpacking, selectedrow.id));
                        sql.Add(SQLTxt.GetInserLogSql(CommonLogin.CommonUser.UserName, EnumHelper.GetEnumTextVal((int)RefundOperatorEnum.GoUnpacking, typeof(RefundOperatorEnum)), selectedrow.refundNo, ""));
                        if (dal.ExecuteTransaction(sql, null))
                        {
                            MessageBox.Show("已转到拆包状态", "提示");
                        }
                    }

                    LoadRefundData(Convert.ToInt32(this.CurrentPage), this.PageSize);
                }
                else
                {
                    MessageBox.Show("当前你没有操作权限", "提示");
                }
            }
            else if (oper.Equals("14"))
            {
                if (MessageBox.Show("继续操作合并订单", "提示", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    if (RefundGridlist.SelectedItems.Count > 1)
                    {

                        int? refundCount = 0;
                        int? receiptCount = 0;

                        //保留的单号
                        string saveRefundNo = string.Empty;
                        //备注(合并)
                        string remark = string.Empty;
                        //订单状态(用于比较)
                        int refundStatus = 0;

                        List<string> sql = new List<string>();

                        for (int i = 0; i < RefundGridlist.SelectedItems.Count; i++)
                        {
                            var rm = (RefundGridlist.SelectedItems[i] as RefundModel);
                            refundCount += rm.refundAmount;
                            receiptCount += rm.confirmAmount;
                            remark += rm.remark;

                            if (i.Equals(0))
                            {
                                saveRefundNo = (RefundGridlist.SelectedItems[i] as RefundModel).refundNo;
                                refundStatus = (RefundGridlist.SelectedItems[i] as RefundModel).refundStatus;
                            }
                            else
                            {
                                if (refundStatus == (RefundGridlist.SelectedItems[i] as RefundModel).refundStatus)
                                {
                                    var deleteRefundNo = (RefundGridlist.SelectedItems[i] as RefundModel).refundNo;
                                    sql.Add(string.Format("delete t_refund where refundNo='{0}'", deleteRefundNo));
                                    sql.Add(string.Format("update T_RefundDetail set refundNo='{0}' where refundNo='{1}'", saveRefundNo, deleteRefundNo));
                                }
                                else
                                {
                                    MessageBox.Show("合并的订单状态必须一致才能合并", "提示");
                                    return;
                                }

                            }
                        }

                        sql.Add(string.Format("update t_refund set remark='{0}',refundAmount={2},confirmAmount={3} where refundNo='{1}'", remark, saveRefundNo, refundCount, receiptCount));

                        sql.Add(SQLTxt.GetInserLogSql(CommonLogin.CommonUser.UserName, EnumHelper.GetEnumTextVal((int)RefundOperatorEnum.merge, typeof(RefundOperatorEnum)), saveRefundNo, ""));

                        if (sql.Count > 0)
                        {
                            if (dal.ExecuteTransaction(sql, null))
                            {
                                MessageBox.Show("订单合并成功", "提示");
                                LoadRefundData(Convert.ToInt32(this.CurrentPage), this.PageSize);
                            }
                            else
                            {
                                MessageBox.Show("订单合并失败", "提示");
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("合并订单必需2个或以上订单才能合并", "提示");
                    }
                }
            }
            else if (oper.Equals("15"))
            {
                RefundModel selectedrow = RefundGridlist.SelectedItem as RefundModel;

                if (selectedrow != null)
                {
                    _9M.Work.WPF_Main.Infrastrcture.FormInit.OpenDialog(this, new CsRemark(selectedrow.refundNo), false);
                }
                else
                {
                    MessageBox.Show("选中要编辑的行", "提示");
                }
            }
        }

        #endregion

        #region 获取退货的快递信息(加载时同步)

        private delegate KeyValuePair<int, string> AsyncGetExpressInfoHandle(string RefundNo);

        #region 方法

        public KeyValuePair<int, string> GetExpressInfo(string RefundNo)
        {
            string sql = string.Empty;

            if (string.IsNullOrEmpty(RefundNo))
            {
                sql = @"select tbRefundId,shopId from T_RefundDetail join t_Refund on T_RefundDetail.RefundNo=t_Refund.RefundNo 
                              where tbRefundId<>'' and tbRefundId  is not null 
                              and  ( expressCompany='' or expressCompany is null) and (expressCode='' or expressCode is null)";

            }
            else
            {
                sql = string.Format(@"select tbRefundId,shopId from T_RefundDetail join t_Refund on T_RefundDetail.RefundNo=t_Refund.RefundNo 
                                              where tbRefundId<>'' and tbRefundId  is not null and refundNo='{0}'
                                                 and  ( expressCompany='' or expressCompany is null) and (expressCode='' or expressCode is null)", RefundNo);


            }

            List<RefundInfoModel> list = dal.QueryList<RefundInfoModel>(sql);

            UpdateTip = "淘宝数据获取中...";
            TotalNum = list.Count;

            KeyValuePair<int, string> kvp = new KeyValuePair<int, string>(0, "");

            SessionUserModel user = CommonLogin.CommonUser;

            List<RefundDetailModel> rdm_list = new List<RefundDetailModel>();
            List<string> sql_list = new List<string>();

            #region 本地直接访问淘宝网关

            TopSource com = new TopSource();
            TopSource com_tmall = new TopSource(ShopList.Where(x => x.shopId == 1022).FirstOrDefault());
            Top.Api.Domain.Refund tbRefund_RtnVal = null;

            list.ForEach(delegate(RefundInfoModel rfm)
            {
                if (rfm.shopId.Equals(1000) || rfm.shopId.Equals(1022))
                {
                    tbRefund_RtnVal = null;
                    tbRefund_RtnVal = rfm.shopId.Equals(1000) ? com.GetRefundExpressCode(rfm.tbRefundId) : com_tmall.GetRefundExpressCode(rfm.tbRefundId);

                    if (tbRefund_RtnVal != null)
                    {
                        if (!string.IsNullOrEmpty(tbRefund_RtnVal.Sid))
                        {
                            rdm_list.Add(new RefundDetailModel()
                            {
                                tbRefundId = tbRefund_RtnVal.RefundId,
                                expressCompany = tbRefund_RtnVal.CompanyName,
                                expressCode = tbRefund_RtnVal.Sid.ToString()
                            });


                        }
                    }

                    UpdateNum++;
                }
            });

            #endregion

            #region 从聚石塔访问淘宝网关(中转)

            //List<KeyValuePair<string, long>> kCollection = new List<KeyValuePair<string, long>>();

            //list.ForEach(delegate(RefundInfoModel rfm)
            //{
            //    kCollection.Add(new KeyValuePair<string, long>(((List<ShopModel>)shopCombox.ItemsSource).Find(a => a.shopId.Equals(rfm.shopId)).shopName, rfm.tbRefundId));
            //});

            //if (kCollection.Count > 0)
            //{
            //    var rtnVal = RefundManager.GetRefundExpressInfo(kCollection);

            //    rtnVal.ForEach(delegate(Top.Api.Domain.Refund r)
            //    {
            //        if (!string.IsNullOrEmpty(r.Sid))
            //        {
            //            rdm_list.Add(new RefundDetailModel() { tbRefundId = r.RefundId, expressCompany = r.CompanyName, expressCode = r.Sid.ToString() });
            //        }

            //    });
            //}

            #endregion

            if (rdm_list.Count > 0)
            {
                try
                {
                    UpdateTip = "数据处理中...";
                    TotalNum = rdm_list.Count;
                    UpdateNum = 0;

                    rdm_list.ForEach(delegate(RefundDetailModel rdm)
                    {
                        UpdateNum++;
                        sql_list.Add(string.Format(@"update T_RefundDetail set expressCompany='{0}',expressCode='{1}' where tbRefundId='{2}'", rdm.expressCompany, rdm.expressCode, rdm.tbRefundId));
                    });

                    if (dal.ExecuteTransaction(sql_list, null))
                    {
                        kvp = new KeyValuePair<int, string>(sql_list.Count, string.Format("当前获取快递更新信息{0}条", sql_list.Count.ToString()));
                    }
                    else
                    {
                        kvp = new KeyValuePair<int, string>(0, "当前获取快递更新信息0条");
                    }
                }
                catch (Exception err)
                {
                    kvp = new KeyValuePair<int, string>(0, string.Format("错误提示", err.Message.ToString()));
                }
            }
            else
            {
                kvp = new KeyValuePair<int, string>(0, "当前没有更新的快递单号");
            }

            UpdateTip = "";
            TotalNum = 0;
            UpdateNum = 0;

            return kvp;
        }

        #endregion

        #region 获取快递信息回调(手动点击时)
        void AsyncGetExpressInfoHandleCallback(IAsyncResult ar)
        {
            try
            {
                AsyncGetExpressInfoHandle handler = (AsyncGetExpressInfoHandle)((AsyncResult)ar).AsyncDelegate;
                KeyValuePair<int, string> rtnVal = handler.EndInvoke(ar);

                System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    loading.Visibility = loadingTip1.Visibility = System.Windows.Visibility.Collapsed;
                    MessageBox.Show(rtnVal.Value, "提示");
                    if (rtnVal.Key > 0)
                    {
                        try
                        {
                            dal.ExecuteSql("update T_LastSyncExpressTime set syncTime =getdate()");
                        }
                        catch
                        {

                        }
                    }
                }));
            }
            catch
            { }

        }

        #endregion

        #region 获取快递信息回调(手动点击时)
        void AsyncGetExpressInfCallbackOpenDialog(IAsyncResult ar)
        {
            AsyncGetExpressInfoHandle handler = (AsyncGetExpressInfoHandle)((AsyncResult)ar).AsyncDelegate;
            KeyValuePair<int, string> rtnVal = handler.EndInvoke(ar);

            System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                loading.Visibility = System.Windows.Visibility.Collapsed;
                RefundModel selectedrow = RefundGridlist.SelectedItem as RefundModel;
                _9M.Work.WPF_Main.Infrastrcture.FormInit.OpenDialog(this, new UnpackingCheck(selectedrow, RefundHandleStatusEnum.UnpackingCheck), false);
            }));
        }
        #endregion

        #endregion

        #region 双击Grid

        private void grid_Click(object sender, MouseButtonEventArgs e)
        {
            RefundModel selectedrow = RefundGridlist.SelectedItem as RefundModel;
            _9M.Work.WPF_Main.Infrastrcture.FormInit.OpenDialog(this, new RegisterRefund(selectedrow.refundNo, OperationStatus.Edit), false);
        }

        #endregion

        #region 拆包核验/异常处理/财务退款/退邮费/查看

        #region 更新C店备注
        bool updateCshopRemark(RefundModel selectedrow)
        {
            bool isUpdateSucceed = false;

            try
            {
                // OnlineStoreAuthorization AuthorizationModel = _9M.Work.WPF_Common.CommonLogin.CommonUser.CShop;
                TopSource com = new TopSource();
                //调用淘宝API后返回的订单
                List<Erp_Model_GJRefund> Erp_TradeInfo = new List<Erp_Model_GJRefund>();
                Erp_TradeInfo.Add(new Erp_Model_GJRefund() { tbTradeNo = selectedrow.tradeNo });
                List<Trade> tradeCollection = com.GetTradeInfoByCShopOrTmall(Erp_TradeInfo);
                string RefundRemark = string.Empty;

                tradeCollection.ForEach(delegate(Trade t)
                   {
                       if (!string.IsNullOrEmpty(t.SellerMemo))
                       {
                           RefundRemark += t.SellerMemo + ";";
                       }
                   });

                if (dal.ExecuteSql(string.Format("update t_Refund set remark='{0}' where id={1}", RefundRemark, selectedrow.id)) > 0)
                {
                    isUpdateSucceed = true;
                }
            }
            catch { }
            return isUpdateSucceed;

        }

        #endregion

        #region 更新分销备注
        bool updateFxRemark(RefundModel selectedrow)
        {

            bool isUpdateSucceed = false;

            try
            {
                //调用淘宝API后返回的订单
                TopSource com = new TopSource();
                List<Erp_Model_GJRefund> Erp_TradeInfo = new List<Erp_Model_GJRefund>();
                Erp_TradeInfo.Add(new Erp_Model_GJRefund() { tbTradeNo = selectedrow.tradeNo });
                List<PurchaseOrder> tradeCollectionByFxApi = com.GetTradeInfoByFX(Erp_TradeInfo);
                string RefundRemark = string.Empty;

                tradeCollectionByFxApi.ForEach(delegate(PurchaseOrder po)//遍历采购单
                       {
                           if (!string.IsNullOrEmpty(po.SupplierMemo))
                           {
                               RefundRemark += po.SupplierMemo + ";";
                           }
                       });
                if (dal.ExecuteSql(string.Format("update t_Refund set remark='{0}' where id={1}", RefundRemark, selectedrow.id)) > 0)
                {
                    isUpdateSucceed = true;
                }
            }
            catch { }

            return isUpdateSucceed;
        }

        #endregion

        #region 更新备注(异步)

        delegate bool AsyncUpdateRemarkHandle(RefundModel selectedrow);

        #endregion

        private void Grid_Handler(object sender, RoutedEventArgs e)
        {
            var s = ((Button)sender).Tag;
            RefundModel selectedrow = RefundGridlist.SelectedItem as RefundModel;

            switch (Int32.Parse(s.ToString()))
            {
                case (Int32)RefundHandleStatusEnum.UnpackingCheck:

                    //loading.Visibility = System.Windows.Visibility.Visible;

                    #region 获取快递单号
                    //AsyncGetExpressInfoHandle handle = new AsyncGetExpressInfoHandle(GetExpressInfo);
                    //handle.BeginInvoke(null, AsyncGetExpressInfCallbackOpenDialog, null);
                    #endregion

                    #region 更新备注

                    if (selectedrow.shopId.Equals(1) || selectedrow.shopId.Equals(3))
                    {
                        AsyncUpdateRemarkHandle handle = new AsyncUpdateRemarkHandle(updateCshopRemark);
                        handle.BeginInvoke(selectedrow, null, null);
                    }
                    else if (selectedrow.shopId.Equals(2))
                    {
                        AsyncUpdateRemarkHandle handle = new AsyncUpdateRemarkHandle(updateFxRemark);
                        handle.BeginInvoke(selectedrow, null, null);
                    }

                    _9M.Work.WPF_Main.Infrastrcture.FormInit.OpenDialog(this, new UnpackingCheck(selectedrow, RefundHandleStatusEnum.UnpackingCheck), false);

                    #endregion

                    break;
                case (Int32)RefundHandleStatusEnum.ExceptionHandler:
                    _9M.Work.WPF_Main.Infrastrcture.FormInit.OpenDialog(this, new UnpackingCheck(selectedrow, RefundHandleStatusEnum.ExceptionHandler), false);
                    break;
                case (Int32)RefundHandleStatusEnum.FinancialRefund:
                    _9M.Work.WPF_Main.Infrastrcture.FormInit.OpenDialog(this, new UnpackingCheck(selectedrow, RefundHandleStatusEnum.FinancialRefund), false);
                    break;
                case (Int32)RefundHandleStatusEnum.FinancialPartRefund:
                    _9M.Work.WPF_Main.Infrastrcture.FormInit.OpenDialog(this, new UnpackingCheck(selectedrow, RefundHandleStatusEnum.FinancialPartRefund), false);
                    break;
                case (Int32)RefundHandleStatusEnum.RefundPostCode:
                    _9M.Work.WPF_Main.Infrastrcture.FormInit.OpenDialog(this, new UnpackingCheck(selectedrow, RefundHandleStatusEnum.RefundPostCode), false);
                    break;
                case (Int32)RefundHandleStatusEnum.look:
                    _9M.Work.WPF_Main.Infrastrcture.FormInit.OpenDialog(this, new UnpackingCheck(selectedrow, RefundHandleStatusEnum.look), false);
                    break;
            }
        }

        #endregion

        #region 搜索

        private void btn_Search(object sender, RoutedEventArgs e)
        {

            new Thread(() =>
            {
                System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    LoadRefundData(Int32.Parse(this.CurrentPage), this.PageSize);
                }));

            }).Start();

        }

        #endregion

        #region 日期间隔天数
        private int DateDiff(DateTime dateStart, DateTime dateEnd)
        {
            DateTime start = Convert.ToDateTime(dateStart.ToShortDateString());
            DateTime end = Convert.ToDateTime(dateEnd.ToShortDateString());

            TimeSpan sp = end.Subtract(start);

            return sp.Days;
        }
        #endregion

        #region 回车查询
        private void txtPNC_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (txtPNC.Text.Trim() == "")
                {
                    MessageBox.Show("搜索的关键字不能为空");
                }
                else
                {
                    LoadRefundData(Convert.ToInt32(CurrentPage), PageSize);
                }
            }
        }
        #endregion

        #region 背景色
        private void RefundGridlist_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            DataGridRow dataGridRow = e.Row;
            RefundModel rm = e.Row.Item as RefundModel;
            if (rm.flagColor.Equals((int)FlagColorEnume.AColor))
            {
                e.Row.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00cc33"));
            }
            else if (rm.flagColor.Equals((int)FlagColorEnume.BColor))
            {
                e.Row.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ff99cc"));
            }
        }
        #endregion

        #region 复制

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Clipboard.SetText(((TextBlock)sender).Text);
        }

        #endregion

        #region 颜色筛选
        private void cmbColorFlag_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            new Thread(() =>
            {
                System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    LoadRefundData(Int32.Parse(this.CurrentPage), this.PageSize);
                }));

            }).Start();
        }

        #endregion

    }


    public class RefundInfoModel
    {
        public long tbRefundId { get; set; }
        public int shopId { get; set; }
    }

    public class StatisticsTip
    {
        public int refundStatus { get; set; }
        public int refundAmount { get; set; }
    }

}
