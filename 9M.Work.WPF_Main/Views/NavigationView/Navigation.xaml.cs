using _9M.Work.DbObject;
using _9M.Work.Model;
using _9M.Work.WPF_Common;
using _9M.Work.WPF_Main.Infrastrcture;
using _9M.Work.WPF_Main.Views.QualityCheck;
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
using System.Windows.Threading;

namespace _9M.Work.WPF_Main.Views.NavigationView
{
    /// <summary>
    /// Navigation.xaml 的交互逻辑
    /// </summary>
    public partial class Navigation : UserControl
    {
        public Navigation()
        {
            InitializeComponent();
        }

        private void WrapPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //过滤权限
            List<PermissionModel> UserList = CommonLogin.CommonUser.PermissionList;

            WrapPanel wrap = sender as WrapPanel;
            FormInit F = new FormInit();
            int Tag = Convert.ToInt32(wrap.Tag);
            //Window win = new Window();
            //win.WindowStyle = WindowStyle.None;
            //win.WindowState = WindowState.Maximized;
            string title = (wrap.Children[1] as Label).Content.ToString();
            if (!CommonLogin.CommonUser.IsAdmin)
            {
                if (UserList.Where(x => x.Pname.Equals(title)).Count() == 0)
                {
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                      (Action)(() =>
                      {
                        CCMessageBox.Show("您没有足够的权限");
                      }));
                    return;
                }
            }
            object Form = null;
            switch (Tag)
            {
                case 1:
                    Form = new BatchManagement();
                    break;
                case 2:
                    Form = new WaresSubsec();
                    break;
                case 3:
                    Form = new WarePacking();
                    break;
                case 4:
                    Form = new WareMeasure();
                    break;
                case 5:
                    Form = new QualityStatistics();
                    break;
            }

            this.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                (Action)(() => { F.OpenView(title, Form, true); }));
        }


    }
}
