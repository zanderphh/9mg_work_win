using _9M.Work.DbObject;
using _9M.Work.Model;
using _9M.Work.Utility;
using _9M.Work.WPF_Common.ValueObjects;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace _9M.Work.WPF_Main.Views.FinanceRefund
{
    /// <summary>
    /// Interaction logic for FinanceRegsterList.xaml
    /// </summary>
    public partial class FinanceRegsterList : UserControl
    {
        BaseDAL dal = new BaseDAL();
        public int TabSelectedIndex = 1;

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

        #region 初始化绑定

        void InitLoading()
        {
            System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {

                    TabCollection = new ObservableCollection<EnumElementEntity>();
                    TabButtonInitBind();

                    NextPageSearchCommand = new DelegateCommand(NextPageSearchCommandFunc);
                    LoadRefundData(Convert.ToInt32(CurrentPage), PageSize);

                }
                catch
                { }
            }));
        }

        #endregion

        #region 状态切换按钮绑定
        public void TabButtonInitBind()
        {
            List<EnumElementEntity> list = EnumHelper.GetEnumEnumerable(typeof(FinanceRefundEnum));

            list.ForEach(delegate(EnumElementEntity ee)
            {
                ee.Background = (ee.Value.Equals(TabSelectedIndex)) ? "#c5e5f7" : "#ffffff";
                TabCollection.Add(ee);
            });

        }

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
                StringBuilder SqlWhere = new StringBuilder(string.Format(" where 1=1"));

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

        #region 页面构造
        public FinanceRegsterList()
        {
            InitializeComponent();
            this.DataContext = this;
            InitLoading();

            endStartTime.Text = regStartTime.Text = DateTime.Now.ToString("yyyy-MM-01");
            endEndTime.Text = regEndTime.Text = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-01")).AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd");
            endStartTime.IsEnabled = regStartTime.IsEnabled = endEndTime.IsEnabled = regEndTime.IsEnabled = false;

        }
        #endregion

        #region 点击复制

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Clipboard.SetText(((TextBlock)sender).Text);
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
        //private void TabSelected_Click(object sender, RoutedEventArgs e)
        //{
        //    TabSelectedIndex = Convert.ToInt32(((Button)sender).Tag);

        //    foreach (var item in FianceTabButton.Items)
        //    {
        //        var el = FianceTabButton.ItemContainerGenerator.ContainerFromItem(item) as FrameworkElement;

        //        if (el != null)
        //        {
        //            var cp = el as ContentPresenter;
        //            FrameworkElement efind = default(FrameworkElement);
        //            FindChildByType(cp, typeof(Button), ref efind);
        //            if (efind is Button)
        //            {
        //                Color color = efind.Tag.Equals(TabSelectedIndex) ? (Color)ColorConverter.ConvertFromString("#c5e5f7") : (Color)ColorConverter.ConvertFromString("#ffffff");
        //                Button btn = efind as Button;
        //                btn.Background = new SolidColorBrush(color);
        //            }
        //        }
        //    }

        //    CurrentPage = "1";
        //    new Thread(() =>
        //    {
        //        System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
        //        {
        //            LoadRefundData(Convert.ToInt32(CurrentPage), PageSize);
        //        }));
        //    }).Start();
        //}

        #endregion

        #region 提交保存
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string oper = sender.GetType().Equals(typeof(Button)) ? ((Button)sender).Tag.ToString() : ((MenuItem)sender).Tag.ToString();
            if (oper.Equals("0"))//新建
            {
                _9M.Work.WPF_Main.Infrastrcture.FormInit.OpenDialog(this, new FinanceRegister(null, OperationStatus.ADD), false);
            }
            else if (oper.Equals("1"))
            {
                FinanceRefundModel selectedrow = RefundGridlist.SelectedItem as FinanceRefundModel;
                if (selectedrow != null)
                {
                    if (selectedrow.status == (int)FinanceRefundEnum.Finance || selectedrow.status == (int)FinanceRefundEnum.Groupleader)
                    {
                        _9M.Work.WPF_Main.Infrastrcture.FormInit.OpenDialog(this, new FinanceRegister(selectedrow, OperationStatus.ADD), false);
                    }
                    else
                    {
                        MessageBox.Show("当前单据已是完成状态,无法编辑修改！");
                    }
                }
                else
                {
                    MessageBox.Show("选中要编辑修改的行后再操作！");
                }
            }
            else if (oper.Equals("2"))
            {
                FinanceRefundModel selectedrow = RefundGridlist.SelectedItem as FinanceRefundModel;
                if (MessageBox.Show("继续删除?", "提示", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    try
                    {
                        dal.Delete(selectedrow);
                        MessageBox.Show("删除成功");
                        CurrentPage = "1";
                        LoadRefundData(Convert.ToInt32(CurrentPage), PageSize);
                    }
                    catch (Exception err)
                    {
                        MessageBox.Show("删除失败" + err.Message.ToString());
                    }

                }
            }
            else if (oper.Equals("3"))
            {
                FinanceRefundModel selectedrow = RefundGridlist.SelectedItem as FinanceRefundModel;
                if (selectedrow != null)
                {
                    string sql = string.Format("update T_FinanceRefund set financeFlag=0 where id={0}", selectedrow.id);
                    if (dal.ExecuteSql(sql) > 0)
                    {
                        MessageBox.Show("已取消", "提示");
                        CurrentPage = "1";
                        LoadRefundData(Convert.ToInt32(CurrentPage), PageSize);
                    }
                }
            }

        }

        #endregion

        #region 搜索
        private void btn_search(object sender, RoutedEventArgs e)
        {
            CurrentPage = "1";
            LoadRefundData(Convert.ToInt32(CurrentPage), PageSize);
        }
        #endregion

        #region 表格导出
        private void tableExport(object sender, RoutedEventArgs e)
        {
            try
            {


                StringBuilder SqlCount = new StringBuilder(@"
                select  ShopName as [店铺],tbNo as [淘宝单号],tbNick as[淘宝用户名],regTime as[登记时间],
                regEmployee as[登记员工],cause as[原因],coupon as [优惠券金额],cash as [现金金额],endTime as [完成时间],
                case when isBackOperator=0 then '否' else  '是' end as [是否后台直接打款],couponoperator as [优惠券操作员工],
                financeEmployee as [财务操作员工]
                from T_FinanceRefund a join T_ShopConfig as b on a.shopid=b.shopId");

                StringBuilder SqlWhere = new StringBuilder(string.Format(" where 1=1"));

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

                System.Data.DataTable dt = dal.QueryDataTable(SqlCount.Append(SqlWhere.ToString()).ToString(), new object[] { });



                SaveFileDialog sf = new SaveFileDialog();
                sf.Filter = "XLS|*.xls|XLSX|*.xlsx";
                sf.FileName = "售后退款表格";
                if (sf.ShowDialog() == true)
                {
                    ExcelNpoi.TableToExcel(dt, sf.FileName);
                    //  ExcelNpoi.DataTableToExcel(dt, "sheet1", true, sf.FileName);
                    MessageBox.Show("导出成功");
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message.ToString());
            }
        }

        #endregion

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

        #region 查看历史
        private void btn_hisinfo(object sender, RoutedEventArgs e)
        {
            if (txtTBNick.Text.Trim() == "")
            {
                MessageBox.Show("请输入要查看淘宝用户名");
                return;
            }
            _9M.Work.WPF_Main.Infrastrcture.FormInit.OpenDialog(this, new FinanceDetail(txtTBNick.Text), false);
        }
        #endregion







    }
}
