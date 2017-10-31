using _9M.Work.DbObject;
using _9M.Work.Model;
using _9M.Work.Utility;
using _9M.Work.WPF_Main.Infrastrcture;
using Microsoft.Practices.Prism.Commands;
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

namespace _9M.Work.WPF_Main.Views.EveryDayUpdate
{
    /// <summary>
    /// Interaction logic for logDialog.xaml
    /// </summary>
    public partial class logDialog : UserControl
    {

        BaseDAL dal = new BaseDAL();

        public logDialog(string ALLCode, string specName1)
        {
            InitializeComponent();
            this.DataContext = this;

            DataTable dt = dal.QueryDataTable(string.Format("select Operator as 'UserName',Color+' : '+logType as 'Content',RecordTime as 'OperationDate'  from T_WareLog where Convert(varchar(10),WareNo)+Convert(varchar(10),SpecCode)='{0}' and Color='{1}'", ALLCode, specName1));

            List<WareLogModel> list= ConvertType.ConvertToModel<WareLogModel>(dt);

            loglist.ItemsSource = list;

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
            get { return "颜色选择"; }
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
