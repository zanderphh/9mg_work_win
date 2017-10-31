using _9M.Work.DbObject;
using _9M.Work.Model;
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

namespace _9M.Work.WPF_Main.Views.WareHouse
{
    /// <summary>
    /// Interaction logic for AndroidLog.xaml
    /// </summary>
    public partial class AndroidLog : UserControl
    {
        public AndroidLog()
        {
            InitializeComponent();
            this.DataContext = this;
            start.Text = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
            end.Text = DateTime.Now.ToString("yyyy-MM-dd");
            NextPageSearchCommand = new DelegateCommand(NextPageSearchCommandFunc);
            DataBind(PageSize,Convert.ToInt32(CurrentPage));
        }

        public void DataBind(int pPageSize, int pPageIndex)
        {

            BaseDAL dal = new BaseDAL();

            List<ExpressionModelField> listWhere = new List<ExpressionModelField>();
            listWhere.Add(new ExpressionModelField() { Name = "optime", Value =Convert.ToDateTime(start.Text + " 00:00:00"), Relation = EnumRelation.GreaterThanOrEqual });
            listWhere.Add(new ExpressionModelField() { Name = "optime", Value =Convert.ToDateTime(end.Text + " 23:59:59"), Relation = EnumRelation.LessThanOrEqual });


            if (rdSj.IsChecked == true)
            {
                listWhere.Add(new ExpressionModelField() { Name = "logType", Value = 1 });
            }
            else if (rdUpdateHW.IsChecked == true)
            {
                listWhere.Add(new ExpressionModelField() { Name = "logType", Value = 0 });
            }

            if (tbSku.Text.Trim().Length > 0)
            {
                listWhere.Add(new ExpressionModelField() { Name = "skuinfo", Value = tbSku.Text.Trim(), Relation = EnumRelation.Contains });
            }

            if (tbRkNo.Text.Trim().Length > 0)
            {
                listWhere.Add(new ExpressionModelField() { Name = "rkNo", Value = tbRkNo.Text.Trim(), Relation = EnumRelation.Contains });
            }

            if (tbOperator.Text.Trim().Length > 0)
            {
                listWhere.Add(new ExpressionModelField() { Name = "opt", Value = tbOperator.Text.Trim(), Relation = EnumRelation.Contains });
            }




            OrderModelField orderField = new OrderModelField() { PropertyName = "id", IsDesc = false };
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic = dal.GetListPaged<AndroidLogModel>(pPageIndex,pPageSize,listWhere.ToArray(), new[] { orderField });

            if (dic != null)
            {
                int totalCount = (int)dic["total"];

                if (totalCount % PageSize == 0) TotalPage = (totalCount / PageSize).ToString();
                else TotalPage = ((totalCount / PageSize) + 1).ToString();
                List<AndroidLogModel> list = dic["rows"] as List<AndroidLogModel>;
                LogGridlist.ItemsSource = list;
            }

        }

        #region 分页相关属性

        delegate void NextPageHandle();

        void NextPage()
        {
            DataBind(Convert.ToInt32(CurrentPage), PageSize);
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
        private int _pageSize = 30;
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DataBind(this.PageSize, 1);
        }


        private void rd_Click(object sender, RoutedEventArgs e)
        {
            DataBind(this.PageSize, 1);
        }
    }
}
