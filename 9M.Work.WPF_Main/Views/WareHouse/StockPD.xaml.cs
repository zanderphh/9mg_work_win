using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using _9M.Work.Model;
using _9M.Work.DbObject;
using Microsoft.Practices.Prism.Commands;

namespace _9M.Work.WPF_Main.Views.WareHouse
{
    /// <summary>
    /// Interaction logic for StockPD.xaml
    /// </summary>
    public partial class StockPD : UserControl
    {
        public StockPD()
        {
            InitializeComponent();
            this.DataContext = this;
            NextPageSearchCommand = new DelegateCommand(NextPageSearchCommandFunc);
            DataBind(Convert.ToInt32(CurrentPage), PageSize);
        }


        public void DataBind(int pPageIndex,int pPageSize)
        {

            BaseDAL dal = new BaseDAL();

            List<ExpressionModelField> listWhere = new List<ExpressionModelField>();

            if (tbSku.Text.Trim() != "")
            {
                listWhere.Add(new ExpressionModelField() { Name = "skuinfo", Value = tbSku.Text.Trim(), Relation = EnumRelation.Contains });
            }
            OrderModelField orderField = new OrderModelField() { PropertyName = "id", IsDesc = true };
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic = dal.GetListPaged<AndroidPDModel>(pPageIndex,pPageSize, listWhere.ToArray(), new[] { orderField });

            if (dic != null)
            {
                int totalCount = (int)dic["total"];

                if (totalCount % PageSize == 0) TotalPage = (totalCount / PageSize).ToString();
                else TotalPage = ((totalCount / PageSize) + 1).ToString();
                List<AndroidPDModel> list = dic["rows"] as List<AndroidPDModel>;
                PdGridlist.ItemsSource = list;
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
            DataBind(1,this.PageSize);
        }

        private void tbSku_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (tbSku.Text.Trim() == "")
                {
                    MessageBox.Show("搜索商品编号的关键字不能为空");
                    return;
                }
                else
                {
                    DataBind(1, this.PageSize);
                }
            }
        }
    }




}
