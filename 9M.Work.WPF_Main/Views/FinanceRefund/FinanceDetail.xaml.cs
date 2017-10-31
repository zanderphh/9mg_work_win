using _9M.Work.DbObject;
using _9M.Work.Model;
using _9M.Work.WPF_Main.Infrastrcture;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
    /// Interaction logic for FinanceDetail.xaml
    /// </summary>
    public partial class FinanceDetail : UserControl
    {
        public FinanceDetail(string nickName)
        {
            InitializeComponent();
            this.DataContext = this;
            string sql = string.Format("select * from T_FinanceRefund where tbNick=@tbNick");
            SqlParameter[] param = { new SqlParameter() { ParameterName = "@tbNick", Value = nickName } };
            List<FinanceRefundModel> list = new BaseDAL().QueryList<FinanceRefundModel>(sql, param);
            if (list.Count > 0)
            {
                int count = list.Count;
                decimal coupon = list.Sum(a => a.coupon);
                decimal cash = list.Sum(a => a.cash); 
                labMsg.Content = string.Format("该用户合计处理{0}次,其中优惠券金额:{1}元,现金金额:{2}元", count, coupon, cash);
                detailInfoList.ItemsSource = list;
            }
            else
            {
                labMsg.Content = string.Format("无该客户历史处理记录");
            }

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
            get { return "用户快速退款历史记录"; }
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
    }
}
