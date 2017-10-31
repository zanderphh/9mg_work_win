using _9M.Work.DbObject;
using _9M.Work.Model;
using _9M.Work.WPF_Common;
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

namespace _9M.Work.WPF_Main.Views.Refund
{
    /// <summary>
    /// Interaction logic for RefundScanReceipt.xaml
    /// </summary>
    public partial class RefundScanReceipt : UserControl
    {
        public RefundScanReceipt()
        {
            InitializeComponent();
            this.DataContext = this;
            start.Text = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd");
            end.Text = DateTime.Now.ToString("yyyy-MM-dd");
            tbScanExpressNo.Focus();
            ExpressCollection = new List<AndroidScanReceiptModel>();
            NextPageSearchCommand = new DelegateCommand(NextPageSearchCommandFunc);
        }

        public void DataBind(int pPageSize, int pPageIndex)
        {

            BaseDAL dal = new BaseDAL();

            List<ExpressionModelField> listWhere = new List<ExpressionModelField>();
            listWhere.Add(new ExpressionModelField() { Name = "scanTime", Value = Convert.ToDateTime(start.Text + " 00:00:00"), Relation = EnumRelation.GreaterThanOrEqual });
            listWhere.Add(new ExpressionModelField() { Name = "scanTime", Value = Convert.ToDateTime(end.Text + " 23:59:59"), Relation = EnumRelation.LessThanOrEqual });


            if (tbExpressNo.Text.Trim().Length > 0)
            {
                listWhere.Add(new ExpressionModelField() { Name = "scanNo", Value = tbExpressNo.Text.Trim(), Relation = EnumRelation.Contains });
            }


            if (tbOperator.Text.Trim().Length > 0)
            {
                listWhere.Add(new ExpressionModelField() { Name = "scanOpt", Value = tbOperator.Text.Trim(), Relation = EnumRelation.Contains });
            }


            OrderModelField orderField = new OrderModelField() { PropertyName = "id", IsDesc = true };
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic = dal.GetListPaged<AndroidScanReceiptModel>(pPageIndex, pPageSize, listWhere.ToArray(), new[] { orderField });

            if (dic != null)
            {
                int totalCount = (int)dic["total"];
                labTotalCount.Content = String.Format("列表合计：{0} 条", totalCount.ToString());
                if (totalCount % PageSize == 0) TotalPage = (totalCount / PageSize).ToString();
                else TotalPage = ((totalCount / PageSize) + 1).ToString();
                List<AndroidScanReceiptModel> list = dic["rows"] as List<AndroidScanReceiptModel>;
                LogGridlist.ItemsSource = list;
            }

        }

        #region 分页相关属性

        delegate void NextPageHandle();

        void NextPage()
        {
            DataBind(PageSize,Convert.ToInt32(CurrentPage));
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
            ExpressCollection.Clear();
            DataBind(this.PageSize, 1);
        }


        private List<AndroidScanReceiptModel> _expressCollection;
        public List<AndroidScanReceiptModel> ExpressCollection
        {
            get { return _expressCollection; }
            set
            {
                if (_expressCollection != value)
                {
                    _expressCollection = value;
                    this.OnPropertyChanged("ExpressCollection");
                }
            }
        }

        private void tbScanExpressNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (tbScanExpressNo.Text.Trim().Equals(""))
                {
                    MessageBox.Show("扫描的商品编号不能为空", "提示");
                    return;
                }
                else
                {
                    List<ExpressionModelField> listWhere = new List<ExpressionModelField>();
                    listWhere.Add(new ExpressionModelField() { Name = "scanNo", Value = tbScanExpressNo.Text.Trim() });
                    //数据访问
                    BaseDAL dal = new BaseDAL();
                    bool isSave = true;
                    bool? isEmployee = ckIsEmployee.IsChecked;

                    var list = dal.GetList<AndroidScanReceiptModel>(listWhere.ToArray());
                    if (list.Count > 0)
                    {
                        isSave = false;
                        MessageBox.Show("邮单号已存在", "提示");
                        tbExpressNo.Text = tbScanExpressNo.Text.Trim();
                        DataBind(this.PageSize, 1);
                    }

                    if (isSave)
                    {
                        if (dal.ExecuteSql(String.Format(@" insert into T_AndroidScanReceipt(scanNo,scanOpt,scanTime,isEmployee) values('{0}','{1}',getdate(),{2})", tbScanExpressNo.Text.Trim(), CommonLogin.CommonUser.UserName, isEmployee == true ? 1 : 0)) > 0)
                        {
                            ExpressCollection.Add(new AndroidScanReceiptModel() { id = 0, scanNo = tbScanExpressNo.Text, scanOpt = CommonLogin.CommonUser.UserName, scanTime = DateTime.Now });
                            LogGridlist.ItemsSource = ExpressCollection.OrderByDescending(a => a.scanTime);
                        }
                    }

                    tbScanExpressNo.Text = "";
                    tbScanExpressNo.Focus();
                }
            }
        }

        private void btn_editRemark(object sender, RoutedEventArgs e)
        {
            AndroidScanReceiptModel selectedrow = LogGridlist.SelectedItem as AndroidScanReceiptModel;

            if (selectedrow != null)
            {
                _9M.Work.WPF_Main.Infrastrcture.FormInit.OpenDialog(this, new RefundScanReceiptRemark(selectedrow.scanNo), false);
            }
            else
            {
                MessageBox.Show("选中要编辑的行", "提示");
            }


            //string sql=string.Format("update T_AndroidScanReceipt set remark='{0}'")
        }

        private void BtnStatistics_Click(object sender, RoutedEventArgs e)
        {
            _9M.Work.WPF_Main.Infrastrcture.FormInit.OpenDialog(this, new RefundScanReceiptStatistics(), false);
        }

    }
}
