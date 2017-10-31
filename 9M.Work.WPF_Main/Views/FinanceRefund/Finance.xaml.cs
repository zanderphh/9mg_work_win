using _9M.Work.DbObject;
using _9M.Work.Model;
using _9M.Work.WPF_Common;
using _9M.Work.WPF_Common.ValueObjects;
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

namespace _9M.Work.WPF_Main.Views.FinanceRefund
{
    /// <summary>
    /// Interaction logic for Finance.xaml
    /// </summary>
    public partial class Finance : UserControl
    {

        BaseDAL dal = new BaseDAL();
        public List<ShopModel> ShopList = null;

        #region 属性
        public int _shopIndexSelected = -1;
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
        #endregion

        #region 页面构造
        public Finance()
        {
            InitializeComponent();
            this.DataContext = this;
            endStartTime.Text = regStartTime.Text = DateTime.Now.ToString("yyyy-MM-01");
            endEndTime.Text = regEndTime.Text = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-01")).AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd");
            endStartTime.IsEnabled = regStartTime.IsEnabled = endEndTime.IsEnabled = regEndTime.IsEnabled = false;
            ShopComboxBind();

            System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    NextPageSearchCommand = new DelegateCommand(NextPageSearchCommandFunc);
                    LoadRefundData(Convert.ToInt32(CurrentPage), PageSize);
                }
                catch
                { }
            }));
        }
        #endregion

        #region 分页相关属性

        delegate ObservableCollection<FinanceRefundModel> NextPageHandle();

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

        #region 加载列表
        /// <summary>
        /// 加载采购列表
        /// </summary>
        public void LoadRefundData(int pageIndex, int pageSize)
        {
            try
            {

                ObservableCollection<FinanceRefundModel> ObserCollection = new ObservableCollection<FinanceRefundModel>();
                RefundGridlist.ItemsSource = ObserCollection;

                StringBuilder SqlCount = new StringBuilder("select count(*) as [totalCount] from T_FinanceRefund");
                StringBuilder SqlWhere = new StringBuilder(string.Format(" where [status]={0}", (int)FinanceRefundEnum.Finance));

                if (Convert.ToInt32(shopCombox.SelectedValue) != -1)
                {
                    SqlWhere.Append(string.Format(" and shopid={0}", Convert.ToInt32(shopCombox.SelectedValue)));
                }

                if (ckRegisterTime.IsChecked.Equals(true))
                {
                    SqlWhere.Append(string.Format(" and regTime >='{0} 00:00:00' and regTime<='{1} 23:59:59'", regStartTime.Text, regEndTime.Text));
                }

                if (ckEndTime.IsChecked.Equals(true))
                {
                    SqlWhere.Append(string.Format(" and endTime >='{0} 00:00:00' and endTime<='{1} 23:59:59'", endStartTime.Text, endEndTime.Text));
                }

                if (txtTBNo.Text.Trim() != "")
                {
                    SqlWhere.Append(string.Format(" and tbNo like '%{0}%'", txtTBNo.Text.ToString()));
                }

                if (txtTBNick.Text.Trim() != "")
                {
                    SqlWhere.Append(string.Format(" and tbNick like '%{0}%'", txtTBNick.Text.ToString()));
                }

                if (txtRegEmp.Text.Trim() != "")
                {
                    SqlWhere.Append(string.Format(" and (regEmployee like '%{0}%' or couponOperator like '%{0}%' or financeEmployee like '%{0}%')", txtRegEmp.Text.ToString()));
                }

                System.Data.DataTable Count = dal.QueryDataTable(SqlCount.Append(SqlWhere.ToString()).ToString(), new object[] { });

                if (Count.Rows.Count > 0)
                {

                    int totalCount = Int32.Parse(Count.Rows[0][0].ToString());
                    TotalPage = (totalCount % PageSize == 0) ? (totalCount / PageSize).ToString() : ((totalCount / PageSize) + 1).ToString();
                    if (totalCount > 0)
                    {
                        StringBuilder sbSql = new StringBuilder();

                        sbSql.Append(string.Format(@"                                           
                                            select * from (
                                                        
                                               select ROW_NUMBER() Over(order by regTime desc) as rownumber,* from T_FinanceRefund {0}
                                                        
                                                  ) a where rownumber between {1} and {2} order by rownumber asc", SqlWhere.ToString(), (pageIndex - 1) * pageSize + 1, pageIndex * pageSize));

                        var rmCollection = dal.QueryList<FinanceRefundModel>(sbSql.ToString());

                        foreach (FinanceRefundModel m in rmCollection)
                        {
                            if (m.financeFlag == "1")
                            {
                                m.financeFlag = "/9M.Work.WPF_Main;component/Images/flag.ico";
                            }
                            else
                            {
                                m.financeFlag = "/9M.Work.WPF_Main;component/Images/defaultFlag.ico";
                            }
                            ObserCollection.Add(m);
                        }



                    }
                }

            }
            catch (Exception err)
            {
                string msg = err.Message.ToString();
            }

        }
        #endregion

        #region 点击复制

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Clipboard.SetText(((TextBlock)sender).Text);
        }

        private void alipay_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Clipboard.SetText(((TextBlock)sender).Text.Split(' ')[0]);
        }
        private void tbNo_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Clipboard.SetText(((TextBlock)sender).Text);
        }

        #endregion

        private void btn_search(object sender, RoutedEventArgs e)
        {
            CurrentPage = "1";
            LoadRefundData(Convert.ToInt32(CurrentPage), PageSize);
        }

        private void btn_end(object sender, RoutedEventArgs e)
        {
            FinanceRefundModel selectedrow = RefundGridlist.SelectedItem as FinanceRefundModel;
            if (selectedrow != null)
            {
                string sql = string.Empty;

                int status = (int)FinanceRefundEnum.end;

                string remark = string.Format("{0}({1}{2})\r\n", "已打款", CommonLogin.CommonUser.UserName, DateTime.Now.ToString());

                sql = string.Format("update T_FinanceRefund set status={0},financeEmployee='{1}',remark=remark+'{2}',endTime=getdate()  where id={3}", status, CommonLogin.CommonUser.UserName, remark, selectedrow.id);

                if (dal.ExecuteSql(sql) > 0)
                {
                    MessageBox.Show("已完成", "提示");
                    CurrentPage = "1";
                    LoadRefundData(Convert.ToInt32(CurrentPage), PageSize);
                }
                else
                {
                    MessageBox.Show("未处理成功", "提示");
                    CurrentPage = "1";
                    LoadRefundData(Convert.ToInt32(CurrentPage), PageSize);
                }
            }
            else
            {
                MessageBox.Show("选择要处理的行后再右键点击完成", "提示");
            }
        }

        #region
        private void ckRegisterTime_Click(object sender, RoutedEventArgs e)
        {
            if (ckRegisterTime.IsChecked.Equals(true))
            {
                regStartTime.IsEnabled = regEndTime.IsEnabled = true;
            }
            else if (ckRegisterTime.IsChecked.Equals(false))
            {
                regStartTime.IsEnabled = regEndTime.IsEnabled = false;
            }
        }

        private void ckEndTime_Click(object sender, RoutedEventArgs e)
        {
            if (ckEndTime.IsChecked.Equals(true))
            {
                endStartTime.IsEnabled = endEndTime.IsEnabled = true;
            }
            else if (ckEndTime.IsChecked.Equals(false))
            {
                endStartTime.IsEnabled = endEndTime.IsEnabled = false;
            }
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

        private void btn_flag(object sender, RoutedEventArgs e)
        {
            FinanceRefundModel selectedrow = RefundGridlist.SelectedItem as FinanceRefundModel;
            if (selectedrow != null)
            {
                string sql = string.Format("update T_FinanceRefund set financeFlag=1 where id={0}", selectedrow.id);
                if (dal.ExecuteSql(sql) > 0)
                {
                    MessageBox.Show("已标记", "提示");
                    CurrentPage = "1";
                    LoadRefundData(Convert.ToInt32(CurrentPage), PageSize);
                }
            }
        }




    }
}
