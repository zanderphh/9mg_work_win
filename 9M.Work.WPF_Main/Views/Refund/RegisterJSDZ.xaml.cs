using _9M.Work.DbObject;
using _9M.Work.Model;
using _9M.Work.Utility;
using _9M.Work.WPF_Main.Infrastrcture;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
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
    /// Interaction logic for RegisterJSDZ.xaml
    /// </summary>
    public partial class RegisterJSDZ : UserControl
    {

        BaseDAL dal = new BaseDAL();

        public RegisterJSDZ()
        {
            InitializeComponent();
            cmbCheckStatus.SelectedValue = -1;
            DateTime now = DateTime.Now;
            date_start.Text = new DateTime(now.Year, now.Month - 1, 1).ToShortDateString();
            date_end.Text = new DateTime(now.Year, now.Month, 1).AddDays(-1).ToShortDateString();
            this.DataContext = this;
            NextPageSearchCommand = new DelegateCommand(NextPageSearchCommandFunc);
            SourceDataBind(Convert.ToInt32(CurrentPage), PageSize);

        }

        #region 数据绑定

        public void SourceDataBind(int pageIndex, int pageSize)
        {

            OrderModelField orderField = new OrderModelField() { PropertyName = "registerTime", IsDesc = true };

            List<ExpressionModelField> fieldWhere = new List<ExpressionModelField>();
            if (!txtTradeNo.Text.Trim().Equals(""))
            { fieldWhere.Add(new ExpressionModelField() { Name = "tradeNo", Value = txtTradeNo.Text.Trim() }); }

            if (!txtDistributorNick.Text.Trim().Equals(""))
            { fieldWhere.Add(new ExpressionModelField() { Name = "distributorNick", Value = txtDistributorNick.Text.Trim() }); }

            int Val = (int)cmbCheckStatus.SelectedValue;
            if (Val > -1)
            {
                fieldWhere.Add(new ExpressionModelField() { Name = "isCheck", Value = Val == 1 ? true : false });
            }

            Dictionary<string, object> dic = new Dictionary<string, object>();

            dic = dal.GetListPaged<RegisterJSDZModel>(pageIndex, pageSize, fieldWhere.ToArray(), new[] { orderField });

            if (dic != null)
            {
                int totalCount = (int)dic["total"];

                if (totalCount % PageSize == 0) TotalPage = (totalCount / PageSize).ToString();
                else TotalPage = ((totalCount / PageSize) + 1).ToString();

                List<RegisterJSDZModel> list = dic["rows"] as List<RegisterJSDZModel>;

                JSDZ_Grid.ItemsSource = list;
            }
        }

        #endregion

        #region 查询

        private void btn_query(object sender, RoutedEventArgs e)
        {
            SourceDataBind(1, PageSize);
        }

        #endregion

        #region 分页相关属性

        delegate void NextPageHandle();

        void NextPage()
        {
            SourceDataBind(Convert.ToInt32(CurrentPage), PageSize);
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

        #region 新建&编辑&删除

        private void btnHandler_Click(object sender, RoutedEventArgs e)
        {
            string oper = sender.GetType().Equals(typeof(Button)) ? ((Button)sender).Tag.ToString() : ((MenuItem)sender).Tag.ToString();

            if (oper.Equals("0"))//新建
            {
                _9M.Work.WPF_Main.Infrastrcture.FormInit.OpenDialog(this, new RegisterJSDZedit(null), false);
            }
            else if (oper.Equals("1"))//编辑
            {
                RegisterJSDZModel selectedrow = JSDZ_Grid.SelectedItem as RegisterJSDZModel;
                if (selectedrow != null)
                {
                    _9M.Work.WPF_Main.Infrastrcture.FormInit.OpenDialog(this, new RegisterJSDZedit(selectedrow), false);
                }
                else
                {
                    MessageBox.Show("选中要编辑的行", "提示");
                }
            }
            else if (oper.Equals("2"))//删除
            {
                RegisterJSDZModel selectedrow = JSDZ_Grid.SelectedItem as RegisterJSDZModel;

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
                            if (dal.Delete(selectedrow))
                            {
                                MessageBox.Show("删除成功！", "提示");
                                SourceDataBind(Convert.ToInt32(CurrentPage), PageSize);
                            }
                            else
                            {
                                MessageBox.Show("删除失败！", "提示");
                            }
                        }
                        catch (Exception err)
                        {
                            MessageBox.Show(string.Format("错误提示:{0}", err.Message.ToString()), "提示");
                        }
                    }
                }
            }
            else if (oper.Equals("3"))//审核
            {

                RegisterJSDZModel rjsdzm = JSDZ_Grid.SelectedItem as RegisterJSDZModel;

                if (rjsdzm != null)
                {
                    if (rjsdzm.isCheck.Equals(true))
                    {
                        MessageBox.Show("该订单已审核！", "提示");
                        return;
                    }

                    if (MessageBox.Show("确认审核操作", "提示", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        try
                        {
                            dal.ExecuteSql(string.Format("update T_RegisterJSDZ set isCheck=1,checkOperator='{0}' where id={1}", WPF_Common.CommonLogin.CommonUser.UserName, rjsdzm.id));
                            NextPageSearchCommandFunc();
                        }
                        catch
                        { }
                    }
                }
            }
            else if (oper.Equals("4"))//取消审核
            {
                RegisterJSDZModel rjsdzm = JSDZ_Grid.SelectedItem as RegisterJSDZModel;
                if (rjsdzm != null)
                {
                    try
                    {
                        dal.ExecuteSql(string.Format("update T_RegisterJSDZ set isCheck=0,checkOperator='{0}' where id={1}", "", rjsdzm.id));
                        NextPageSearchCommandFunc();
                    }
                    catch
                    { }

                }
            }

        #endregion

        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Clipboard.SetText(((TextBlock)sender).Text);
        }


        #region 背景色
        private void JSDZ_Grid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            DataGridRow dataGridRow = e.Row;
            RegisterJSDZModel rm = e.Row.Item as RegisterJSDZModel;
            if (rm.isCheck.Equals(true))
            {
                e.Row.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00cc33"));
            }
        }
        #endregion

        private void btnExport(object sender, RoutedEventArgs e)
        {
            var dateStart = date_start.Text;
            var dateEnd = date_end.Text;


            DataTable dt = dal.QueryDataTable(string.Format(@"select distributorNick,SUM(refundMoney) refundMoney from T_RegisterJSDZ where registerTime>='{0} 00:00:00' and registerTime<='{1} 23:59:59'
                               group by distributorNick", dateStart, dateEnd));

            SaveFileDialog sf = new SaveFileDialog();
            sf.Filter = "XLS|*.xls|XLSX|*.xlsx|TXT|*.txt";
            if (sf.ShowDialog() == true)
            {
                ExcelNpoi.TableToExcelForXLS(dt, sf.FileName);
            }


        }

    }
}
