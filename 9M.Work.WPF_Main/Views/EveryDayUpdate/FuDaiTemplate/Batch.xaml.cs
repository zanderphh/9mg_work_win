using _9M.Work.DbObject;
using _9M.Work.Model;
using _9M.Work.WPF_Main.Infrastrcture;
using Microsoft.Practices.Prism.Commands;
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

namespace _9M.Work.WPF_Main.Views.EveryDayUpdate.FuDaiTemplate
{
    /// <summary>
    /// Batch.xaml 的交互逻辑
    /// </summary>
    public partial class Batch : UserControl
    {
        private ObservableCollection<FuDaiBatchModel> dataSource;
        BaseDAL dal = new BaseDAL();
        public Batch()
        {
            InitializeComponent();
            dataSource = new ObservableCollection<FuDaiBatchModel>();
            this.DataContext = this;
            NextPageSearchCommand = new DelegateCommand(NextPageSearchCommandFunc);
            LoadBatchData(PageIndex, PageSize);
        }

        private void Batch_BtnClick(object sender, RoutedEventArgs e)
        {
            int Tag = Convert.ToInt32((sender as Button).Tag);
            switch (Tag)
            {
                case 0:
                    LoadBatchData(PageIndex, PageSize);
                    break;
                case 1:
                    FormInit.OpenDialog(this, new FuDaiBatchOperat(Model.OperationStatus.ADD, null), false);
                    break;
                case 2:
                    FuDaiBatchModel Item = (FuDaiBatchModel) BatchGridlist.SelectedItem;
                    if (Item==null)
                    {
                        CCMessageBox.Show("请选中要编辑的行");
                        return;
                    }
                    FormInit.OpenDialog(this, new FuDaiBatchOperat(Model.OperationStatus.Edit, Item), false);
                    break;
                case 3:
                    break;
            }
        }
        public void LoadBatchData(int PageIndex, int PageSize)
        {
            dataSource.Clear();
            BatchGridlist.ItemsSource = null;
            BatchGridlist.ItemsSource = dataSource;
            string QueryText = tb_querybox.Text;
            List<ExpressionModelField> orList = new List<ExpressionModelField>();
            if (!string.IsNullOrEmpty(QueryText))
            {
                orList.Add(new ExpressionModelField() { Relation = EnumRelation.Contains, Name = "BatchName", Value = QueryText });
                orList.Add(new ExpressionModelField() { Relation = EnumRelation.Contains, Name = "Brand", Value = QueryText });
                orList.Add(new ExpressionModelField() { Relation = EnumRelation.Contains, Name = "Remark", Value = QueryText });
            }
            Dictionary<string, object> dic = dal.GetListPaged<FuDaiBatchModel>(PageIndex, PageSize, new ExpressionModelField[] { }, orList.ToArray(), new OrderModelField[] { new OrderModelField() { IsDesc = true, PropertyName = "CreateTime" } });
            int totalCount = Convert.ToInt32(dic["total"]);
            TotalCount = totalCount;

            TotalPage = (totalCount % PageSize == 0) ? (totalCount / PageSize).ToString() : ((totalCount / PageSize) + 1).ToString();
            List<FuDaiBatchModel> list = dic["rows"] as List<FuDaiBatchModel>;
          
            list.ForEach(x =>
            {
                dataSource.Add(x);
            });
        }
        #region 分页相关属性

        void NextPage()
        {
            LoadBatchData(Convert.ToInt32(CurrentPage), PageSize);
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
    }
}
