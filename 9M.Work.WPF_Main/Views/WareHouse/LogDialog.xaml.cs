using _9M.Work.WPF_Main.Infrastrcture;
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
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;
using _9M.Work.Model.Log;
using _9M.Work.DbObject;

namespace _9M.Work.WPF_Main.Views.WareHouse
{
    /// <summary>
    /// LogDialog.xaml 的交互逻辑
    /// </summary>
    public partial class LogDialog : UserControl, BaseDialog
    {
        public LogDialog()
        {
            InitializeComponent();
            this.DataContext = this;
            this.Height = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height * 0.7;
        }

        private BaseDAL dal = new BaseDAL();

        public DelegateCommand CancelCommand
        {
            get
            {
                return new DelegateCommand(CloseDialog);
            }
        }

        public string Title
        {
            get
            {
                return "日志查看";
            }
        }

        public void CloseDialog()
        {
            FormInit.CloseDialog(this);
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            throw new NotImplementedException();
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            throw new NotImplementedException();
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="IsGoodsType">0款号1规格2订单</param>
        /// <param name="QueryText">内容</param>
        public void BindDataGrid(int QueryType, string QueryText)
        {
            List<GoodsLogModel> LogList;
            OrderModelField[] orders = new OrderModelField[] { new OrderModelField() { IsDesc = false, PropertyName = "LogTime" } };
            if (QueryType == 0)
            {
                LogList = dal.GetList<GoodsLogModel>(x => x.GoodsNo.Equals(QueryText, StringComparison.CurrentCultureIgnoreCase) && (x.LogType == 1 || x.LogType == 2), orders);
            }
            else if (QueryType == 1)
            {
                LogList = dal.GetList<GoodsLogModel>(x => x.GoodsDetail.Equals(QueryText, StringComparison.CurrentCultureIgnoreCase) && (x.LogType == 1 || x.LogType == 2), orders);
            }
            else
            {
                LogList = dal.GetList<GoodsLogModel>(x => x.TradeId.Equals(QueryText, StringComparison.CurrentCultureIgnoreCase) && (x.LogType == 1 || x.LogType == 2), orders);
            }
            LogGridlist.ItemsSource = LogList;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int QueryType = Com_Type.SelectedIndex; ;
            string QueryText = tb_QueryText.Text.Trim();
            BindDataGrid(QueryType, QueryText);
        }
    }
}
