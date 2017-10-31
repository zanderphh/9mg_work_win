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

namespace _9M.Work.WPF_Main.Views.QualityCheck
{
    /// <summary>
    /// LockingSet.xaml 的交互逻辑
    /// </summary>
    public partial class LockingSet : UserControl, BaseDialog
    {
        private BaseDAL dal = new BaseDAL();
        public LockingSet()
        {
            InitializeComponent();
            this.DataContext = this;
            Init();
        }

        public void Init()
        {
            int k = dal.QuerySingle<SetValueModel>("select * from T_SetValues", new object[] { }).Value;
            Tb_value.Text = k.ToString();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int Tag = Convert.ToInt32((sender as Button).Tag);
            switch (Tag)
            {
                case 0:
                    int k = Convert.ToInt32(Tb_value.Text);
                    dal.ExecuteSql(string.Format("update T_SetValues set Value = {0}",k));
                    CloseDialog();
                    break;
                case 1:
                    CloseDialog();
                    break;
            }
        }

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
            get { return "间隔设置"; }
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
    }
}
