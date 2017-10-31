using _9M.Work.DbObject;
using _9M.Work.Model;
using _9M.Work.WPF_Main.Infrastrcture;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
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

namespace _9M.Work.WPF_Main.Views.Live
{
    /// <summary>
    /// Interaction logic for LiveList.xaml
    /// </summary>
    public partial class LiveList : UserControl
    {

        BaseDAL dal = new BaseDAL();

        public LiveList()
        {
            InitializeComponent();
            this.DataContext = this;
            LoadData(1, this.PageSize);
        }


        #region 属性

        private ObservableCollection<LiveModel> _liveCollection = new ObservableCollection<LiveModel>();

        public ObservableCollection<LiveModel> liveCollection
        {
            get { return _liveCollection; }
            set
            {
                if (_liveCollection != value)
                {
                    _liveCollection = value;
                    this.OnPropertyChanged("liveCollection");
                }
            }
        }




        #endregion


        #region 加载列表

        void bind(List<LiveModel> resultlist, ObservableCollection<LiveModel> targetCollection)
        {
            new Thread(() =>
            {
                foreach (LiveModel m in resultlist)
                {
                    this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Send, new Action(delegate
                    {
                        targetCollection.Add(m);
                    }));

                }

            }).Start();
        }

        public void LoadData(int pageIndex, int pageSize)
        {
            try
            {


                #region 原始Sql

                ObservableCollection<LiveModel> ObserCollection = new ObservableCollection<LiveModel>();
                MainListDG.ItemsSource = ObserCollection;

                StringBuilder sbSql = new StringBuilder();
                StringBuilder sbSqlWhere = new StringBuilder();

                //if (txtCondition.Text.Trim().Length > 0)
                //{
                //    sbSqlWhere.Append(string.Format(" and (photoid like '%{0}%' or photographer like '%{0}%')", txtCondition.Text.Trim()));
                //}

                //var dateStart = date_start.Text;
                //var dateEnd = date_end.Text;

                //if (dateStart.Trim().Length > 0 && dateEnd.Trim().Length > 0)
                //{
                //    sbSqlWhere.Append(string.Format(" and  createTime>='{0} 00:00:00' and createTime<='{1} 23:59:59", dateStart, dateEnd));
                //}


                System.Data.DataTable Count = dal.QueryDataTable(string.Format("select count(1) as [count] from T_Photography where 1=1 {0}", sbSqlWhere.ToString()), new object[] { });

                if (Count.Rows.Count > 0)
                {

                    int totalCount = Int32.Parse(Count.Rows[0][0].ToString());
                    TotalPage = (totalCount % PageSize == 0) ? (totalCount / PageSize).ToString() : ((totalCount / PageSize) + 1).ToString();
                    if (totalCount > 0)
                    {
                        sbSql.Append(string.Format(@"                                           
                                            select * from (
                                                        
                                               select ROW_NUMBER() Over(order by id desc) as rownumber,* from T_Live where 1=1 {0}
                                                        
                                                  ) a where rownumber between {1} and {2} order by rownumber asc", sbSqlWhere.ToString(), (pageIndex - 1) * pageSize + 1, pageIndex * pageSize));

                        var rmCollection = dal.QueryList<LiveModel>(sbSql.ToString());
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

        delegate ObservableCollection<PhotographyModel> NextPageHandle();

        void NextPage()
        {
            LoadData(Convert.ToInt32(CurrentPage), PageSize);
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

        private void MainListDG_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LiveModel item = MainListDG.CurrentItem as LiveModel;
            if (item != null)
            {
                List<ExpressionModelField> whereField = new List<ExpressionModelField>();

                whereField.Add(new ExpressionModelField() { Name = "no", Value = item.liveId });
                List<LiveGoodsModel> list = dal.GetList<LiveGoodsModel>(whereField.ToArray(), new OrderModelField[] { new OrderModelField() { PropertyName = "serialNum", IsDesc = false } });
                DetailListDG.ItemsSource = list;
            }
        }

        private void btnNewCreate_Click(object sender, RoutedEventArgs e)
        {
            NewCreate NC = new NewCreate();
            new FormInit().OpenView("新建直播", NC, true);
        }

        private void btn_editClick(object sender, RoutedEventArgs e)
        {
            LiveModel item = MainListDG.CurrentItem as LiveModel;

            if (item != null)
            {
                NewCreate NC = new NewCreate(item);
                new FormInit().OpenView("新建直播", NC, true);
            }
        }

        private void btn_delClick(object sender, RoutedEventArgs e)
        {
            LiveModel item = MainListDG.CurrentItem as LiveModel;

            if (item != null)
            {
                if (MessageBox.Show("是否继续删除?", "提示", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {

                    List<string> sql = new List<string>();

                    sql.Add(string.Format("delete T_Live where liveId='{0}'", item.liveId));

                    sql.Add(string.Format("delete T_LiveGoods where no='{0}'", item.liveId));
                    if (dal.ExecuteTransaction(sql, null))
                    {
                        LoadData(1, this.PageSize);
                    }
                }
            }
        }
    }
}
