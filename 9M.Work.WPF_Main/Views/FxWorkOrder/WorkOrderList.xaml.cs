using _9M.Work.DbObject;
using _9M.Work.Model;
using _9M.Work.Utility;
using _9M.Work.WPF_Common.ValueObjects;
using _9M.Work.WPF_Main.Infrastrcture;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
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

namespace _9M.Work.WPF_Main.Views.FxWorkOrder
{
    /// <summary>
    /// Interaction logic for WorkOrderList.xaml
    /// </summary>
    public partial class WorkOrderList : UserControl
    {
        #region 属性
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

        #region 状态Tab绑定

        /// <summary>
        /// 处理状态FxWorkOrderEnum(默认为所有-1)
        /// </summary>
        public int FxWorkOrder_Status_Val = -1;


        public void TabButtonInitBind()
        {
            List<EnumElementEntity> list = EnumHelper.GetEnumEnumerable(typeof(FxWorkOrderEnum));

            list.ForEach(delegate(EnumElementEntity ee)
            {
                ee.Background = (ee.Value.Equals(FxWorkOrder_Status_Val)) ? "#c5e5f7" : "#ffffff";
                TabCollection.Add(ee);
            });

        }

        #endregion

        BaseDAL dal = new BaseDAL();
        MainShell w = _9M.Work.WPF_Common.WPFControlsSearchHelper.GetParentObject<MainShell>(FormInit.RightView, "");

        public WorkOrderList()
        {
            InitializeComponent();
            this.DataContext = this;
            TabCollection = new ObservableCollection<EnumElementEntity>();
            TabButtonInitBind();
            NextPageSearchCommand = new DelegateCommand(NextPageSearchCommandFunc);
            LoadRefundData(Convert.ToInt32(CurrentPage), PageSize);
            newMsgNotice.IsChecked = IsPlay();
        }

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

        private void TabButton_Click(object sender, RoutedEventArgs e)
        {
            FxWorkOrder_Status_Val = Convert.ToInt32(((Button)sender).Tag);

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
                        Color color = efind.Tag.Equals(FxWorkOrder_Status_Val) ? (Color)ColorConverter.ConvertFromString("#c5e5f7") : (Color)ColorConverter.ConvertFromString("#ffffff");
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


        #region 分销工单列表绑定

        /// <summary>
        /// 加载采购列表
        /// </summary>
        public void LoadRefundData(int pageIndex, int pageSize)
        {

            BaseDAL dal = new BaseDAL();

            try
            {
                #region 查询条件

                ObservableCollection<FxWorkOrderModel> ObserCollection = new ObservableCollection<FxWorkOrderModel>();
                workorderlist.ItemsSource = ObserCollection;

                StringBuilder sbSql = new StringBuilder();
                StringBuilder sbSqlWhere = new StringBuilder();

                if (ckOnlySelf.IsChecked == true)
                {
                    sbSqlWhere.Append(string.Format(" and operatorEmp='{0}'", WPF_Common.CommonLogin.CommonUser.UserName));
                }

                if (txtSearchField.Text.Trim().Length > 0)
                {
                    sbSqlWhere.Append(string.Format(" and (questionId like '%{0}%' or tradeId like '%{0}%' or questionDesc like '%{0}%' or manualInput like '%{0}%' or aliWang like '%{0}%')", txtSearchField.Text.Trim()));
                }

                if (FxWorkOrder_Status_Val != -1)
                {
                    sbSqlWhere.Append(string.Format(" and status='{0}'", EnumHelper.GetEnumTextVal(FxWorkOrder_Status_Val, typeof(FxWorkOrderEnum))));
                }

                System.Data.DataTable Count = dal.QueryDataTable(string.Format("select count(1) as [count] from DB_9MFenXiao.dbo.WorkOrder  where 1=1 {0}", sbSqlWhere.ToString()), new object[] { });

                if (Count.Rows.Count > 0)
                {

                    int totalCount = Int32.Parse(Count.Rows[0][0].ToString());
                    TotalPage = (totalCount % PageSize == 0) ? (totalCount / PageSize).ToString() : ((totalCount / PageSize) + 1).ToString();
                    if (totalCount > 0)
                    {
                        sbSql.Append(string.Format(@"                                           
                                            select [id]
      ,[questionId]
      ,[status]
      ,[questionType]
      ,[tradeId]
      ,[questionDesc]
      ,[operatorEmp]
      ,[aliwang]
      ,[submitTime]
      ,[operatorTime]
      ,[endTime]
      ,case when endTime is not null then '查看' else '处理' end as [isEnd] from (
                                                        
                                               select ROW_NUMBER() Over(order by id desc) as rownumber,* from DB_9MFenXiao.dbo.WorkOrder where 1=1 {0}
                                                        
                                                  ) a where rownumber between {1} and {2} order by rownumber asc", sbSqlWhere.ToString(), (pageIndex - 1) * pageSize + 1, pageIndex * pageSize));

                        var rmCollection = dal.QueryList<FxWorkOrderModel>(sbSql.ToString());

                        foreach (FxWorkOrderModel m in rmCollection)
                        {
                            ObserCollection.Add(m);
                        }

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

        delegate ObservableCollection<FxWorkOrderModel> NextPageHandle();

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
        private int _pageSize = 50;
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

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            FxWorkOrder_Status_Val = -1;
            CurrentPage = "1";
            LoadRefundData(Convert.ToInt32(CurrentPage), PageSize);
        }

        private void Grid_Handler(object sender, RoutedEventArgs e)
        {
            var s = ((Button)sender).Tag;
            FxWorkOrderModel selectedrow = workorderlist.SelectedItem as FxWorkOrderModel;

            if (selectedrow != null)
            {
                _9M.Work.WPF_Main.Infrastrcture.FormInit.OpenDialog(this, new WorkOrderDialog(selectedrow), false);
            }
        }

        private void btnConfig_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("开发中。。。!", "提示");
        }

        private void btnStatistics_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("开发中。。。!", "提示");
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadRefundData(Convert.ToInt32(CurrentPage), PageSize);
        }

        private void btnTest_Click(object sender, RoutedEventArgs e)
        {
            w.iconPlay();
        }

        private void btnTest_Click1(object sender, RoutedEventArgs e)
        {
            w.iconStop();
        }


        private bool? IsPlay()
        {
            try
            {
                NewMsgNoticeModel model = dal.GetSingle<NewMsgNoticeModel>(x => x.model == "WorkOrder" && x.uname == WPF_Common.CommonLogin.CommonUser.UserName);
                if (model != null)
                {
                    return model.isplay;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }

        }


        private void ckNewMsgNotice_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                NewMsgNoticeModel model = dal.GetSingle<NewMsgNoticeModel>(x => x.model == "WorkOrder" && x.uname == WPF_Common.CommonLogin.CommonUser.UserName);
                if (model != null)
                {
                    model.isplay = newMsgNotice.IsChecked;
                    if (!dal.Update(model))
                    {
                        MessageBox.Show("设置失败", "提示");
                    }
                }
                else
                {
                    model = new NewMsgNoticeModel() { uname=WPF_Common.CommonLogin.CommonUser.UserName, model="WorkOrder", isplay= newMsgNotice.IsChecked};
                    if (!dal.Add(model))
                    {
                        MessageBox.Show("设置失败", "提示");
                    }
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message.ToString(), "提示");
            }


        }



    }
}
