using _9M.Work.DbObject;
using _9M.Work.Model;
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
    /// Interaction logic for LogList.xaml
    /// </summary>
    public partial class LogList : UserControl, BaseDialog
    {
        public LogList(string refundNo)
        {
            InitializeComponent();
            this.DataContext = this;
            BaseDAL dal = new BaseDAL();
            List<ExpressionModelField> listWhere = new List<ExpressionModelField>();
            listWhere.Add(new ExpressionModelField() { Name = "refundNo", Value = refundNo });

            List<OrderModelField> orderField = new List<OrderModelField>();
            orderField.Add(new OrderModelField() { PropertyName = "operTime", IsDesc = false });
            LogGridlist.ItemsSource=dal.GetList<RefundLogModel>(listWhere.ToArray(), orderField.ToArray());

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
            get { return "操作日志"; }
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
