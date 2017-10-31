using _9M.Work.DbObject;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for FinanceWorkStatistics.xaml
    /// </summary>
    public partial class FinanceWorkStatistics : UserControl
    {
        public FinanceWorkStatistics()
        {
            InitializeComponent();
            this.DataContext = this;
            StartTime.Text = DateTime.Now.ToString("yyyy-MM-01");
            EndTime.Text = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-01")).AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd");
        }

        private void btn_click(object sender, RoutedEventArgs e)
        {
 

            SqlParameter[] param = { 
                                       new SqlParameter(){ ParameterName="@StartTime",Value=Convert.ToDateTime(StartTime.Text+" 00:00:00")}, 
                                       new SqlParameter(){ ParameterName="@EndTime",Value=Convert.ToDateTime(EndTime.Text+" 23:59:59")}
                                   };


            string sql = string.Format(@"select regEmployee as employee,COUNT(*) as registerCount from T_FinanceRefund where regTime>@StartTime and regTime<=@EndTime group by regEmployee ");
            List<registerModel> registerModels = new BaseDAL().QueryList<registerModel>(sql, param);
            registerCountlist.ItemsSource = registerModels;


            
            SqlParameter[] param2 = { 
                                       new SqlParameter(){ ParameterName="@StartTime",Value=Convert.ToDateTime(StartTime.Text+" 00:00:00")}, 
                                       new SqlParameter(){ ParameterName="@EndTime",Value=Convert.ToDateTime(EndTime.Text+" 23:59:59")}
                                   };
            sql = string.Format(@"select couponOperator as employee,COUNT(*) as handlerCount from T_FinanceRefund where  regTime>=@StartTime and regTime<=@EndTime and couponOperator is not null group by couponOperator");
            List<handlerModel> handlerModels = new BaseDAL().QueryList<handlerModel>(sql, param2);
            handlerCountlist.ItemsSource = handlerModels;

        }
    }



    public class registerModel
    {
        public string employee { get; set; }
        public int registerCount { get; set; }
    }
    public class handlerModel
    {
        public string employee { get; set; }
        public int handlerCount { get; set; }
    }

}
