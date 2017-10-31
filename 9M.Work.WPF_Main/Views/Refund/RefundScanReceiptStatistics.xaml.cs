using _9M.Work.DbObject;
using _9M.Work.WPF_Main.Infrastrcture;
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

namespace _9M.Work.WPF_Main.Views.Refund
{
    /// <summary>
    /// Interaction logic for RefundScanReceiptStatistics.xaml
    /// </summary>
    public partial class RefundScanReceiptStatistics : UserControl, BaseDialog
    {
        public RefundScanReceiptStatistics()
        {
            InitializeComponent();
            this.DataContext = this;
            start.Text = DateTime.Now.AddDays(-6).ToString("yyyy-MM-dd");
            end.Text = DateTime.Now.ToString("yyyy-MM-dd");
        }

        #region Dialog
        public Microsoft.Practices.Prism.Commands.DelegateCommand CancelCommand
        {
            get { return new DelegateCommand(CloseDialog); }
        }

        public void CloseDialog()
        {
            FormInit.CloseDialog(this);
        }

        public string Title
        {
            get { return "签收统计"; }
        }

        public bool IsNavigationTarget(Microsoft.Practices.Prism.Regions.NavigationContext navigationContext)
        {
            throw new NotImplementedException();
        }

        public void OnNavigatedFrom(Microsoft.Practices.Prism.Regions.NavigationContext navigationContext)
        {
            throw new NotImplementedException();
        }

        public void OnNavigatedTo(Microsoft.Practices.Prism.Regions.NavigationContext navigationContext)
        {
            throw new NotImplementedException();
        }

        #endregion

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            BaseDAL dal = new BaseDAL();

            string sql = string.Format(@"select CONVERT(varchar(10),scanTime,121) scanTime ,COUNT(*) num from T_AndroidScanReceipt where scanTime>='{0}' and scanTime<='{1}'
            group by CONVERT(varchar(10),scanTime,121)", Convert.ToDateTime(start.Text + " 00:00:00"), Convert.ToDateTime(end.Text + " 23:59:59"));
            RefundScanReceiptStatisticslist.ItemsSource = dal.QueryDataTable(sql, new object[] { }).DefaultView;
            
        }
    }
}
