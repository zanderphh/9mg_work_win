using _9M.Work.DbObject;
using _9M.Work.Model;
using _9M.Work.WPF_Common;
using _9M.Work.WPF_Main.Infrastrcture;
using _9M.Work.WPF_Main.Views.DataCenter;
using _9M.Work.WPF_Main.Views.NavigationView;
using MahApps.Metro;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
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

namespace _9M.Work.WPF_Main.Views
{
    /// <summary>
    /// TopView.xaml 的交互逻辑
    /// </summary>
    public partial class TopView : UserControl
    {
        BaseDAL dal = new BaseDAL();
        List<AccentColorMenuData> AccentColors { get; set; }
        public bool IsMax = true;
        public TopView()
        {
            InitializeComponent();
            this.DataContext = this;
            CommonTopView = this;
            List<PermissionModel> modellist = dal.GetAll<PermissionModel>();
            BindMenu(modellist);
            //换肤
            AccentColors = ThemeManager.Accents
                                            .Select(a => new AccentColorMenuData() { Name = a.Name, ColorBrush = a.Resources["AccentColorBrush"] as Brush })
                                            .ToList();
            //换肤菜单
            MenuItem it = new MenuItem();

            it.Height = 18;
            it.Width = 18;
            it.Style = Resources["MeStyle"] as Style;//默认样式

            it.Margin = new Thickness(6, 3, 0, 0);
            AccentColors.ForEach(x =>
            {
                MenuItem child = new MenuItem();
                WrapPanel p = new WrapPanel() { Orientation = Orientation.Horizontal };
                Ellipse r = new Ellipse() { Fill = x.ColorBrush, Width = 20, Height = 20 };
                Label l = new Label { Content = x.Name, FontSize = 15 };
                p.Children.Add(r);
                p.Children.Add(l);
                child.Click += child_Click;
                child.Header = p;
                it.Items.Add(child);

            });
            ColorItem.Items.Add(it);
        }

        //切换皮肤
        private void child_Click(object sender, RoutedEventArgs e)
        {
            string Color = (((sender as MenuItem).Header as WrapPanel).Children[1] as Label).Content.ToString();
            AccentColorMenuData model = AccentColors.Where(x => x.Name.Equals(Color)).Single();
            model.DoChangeTheme();
            this.TopGrid.Background = model.ColorBrush;
            BottomView.CommonBottonView.BottonGrid.Background = model.ColorBrush;
            ColorItem.Background = model.ColorBrush;
            MainShell window = Window.GetWindow(this) as MainShell;
            RightContentView rightview = window.MainContent.Content as RightContentView;
            TabItem it = rightview.sampleSelector.Items[0] as TabItem;
            Navigation na = ((it.Content as Border).Child as Navigation);
            na.Background = model.ColorBrush;
            na.NativeUser.Background = model.ColorBrush;
        }



        public static TopView CommonTopView { get; set; }

        //绑定导航
        public void BindMenu(List<PermissionModel> modellist)
        {
            SessionUserModel User = CommonLogin.CommonUser;
            List<PermissionModel> UserList = User.PermissionList;
            List<PermissionModel> list = modellist.Where(x => x.ParentId == 0).OrderBy(x => x.OrderId).ToList();
            list.ForEach(x =>
            {
                MenuItem it = new MenuItem();
                it.Header = x.Pname;
                List<PermissionModel> sonlist = modellist.Where(y => y.ParentId == x.Id).ToList();
                sonlist.ForEach(z =>
                {
                    MenuItem sonit = new MenuItem();

                    //是否拥有权限判断admin
                    var item = UserList.Find(f => { return f.Id == z.Id; });
                    if (!User.UserName.ToLower().Equals("admin"))
                    {
                        sonit.IsEnabled = item != null;
                        //锁定权限编辑（只有管理员和部门管理员可以编辑）
                        if (z.Pname.Equals("权限分配") && User.IsDeptAdmin)
                        {
                            sonit.IsEnabled = true;
                        }
                    }
                    sonit.Header = z.Pname;
                    sonit.Tag = z.Url;
                    sonit.Click += sonit_Click;
                    it.Items.Add(sonit);

                });
                NavigationMenu.Items.Add(it);
            });
        }

        //打开窗体
        private void sonit_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            //获取当前正在运行的  程序集(Assembly)对象
            Assembly ass = this.GetType().Assembly;//获取当前运行对象所属的类所在的程序集对象
            //获取 程序集中的 Dog类的 类型(Type)对象
            Type typForm = ass.GetType(item.Tag.ToString());
            // 创建对象使用类型的"InvokeMember"方法
            Object obj = typForm.InvokeMember(
                null,
                BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.CreateInstance,
                null,
                null,
                null);
            // 显示子窗口
            if (obj != null)
            {
                UserControl targetForm = obj as UserControl;
                new FormInit().OpenView(item.Header.ToString(), targetForm, true);
            }
        }

        //窗体右上角
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string Tag = (sender as Button).Tag.ToString();
            Window w = WPFControlsSearchHelper.GetParentObject<Window>(this, "");
            switch (Tag)
            {
                case "0":
                    if (CCMessageBox.Show("确定退出系统？", "系统提示", CCMessageBoxButton.OKCancel) == CCMessageBoxResult.OK)
                    {
                        Application.Current.Shutdown();
                    }
                    break;
                case "1": //最大化
                    //if (w.WindowState == WindowState.Normal)
                    //{
                    //    w.WindowState = System.Windows.WindowState.Maximized;
                    //}
                    //else if (w.WindowState == WindowState.Maximized)
                    //{
                    //    w.WindowState = System.Windows.WindowState.Normal;
                    //}

                    MainShell window = Window.GetWindow(this) as MainShell;
                    window.Left = 0.0;
                    window.Top = 0.0;
                    if (IsMax == true)
                    {
                        window.Width = System.Windows.SystemParameters.PrimaryScreenWidth / 2;
                        window.Height = System.Windows.SystemParameters.PrimaryScreenHeight / 2 - 40;
                        IsMax = false;
                    }
                    else
                    {
                        window.Width = System.Windows.SystemParameters.PrimaryScreenWidth;
                        window.Height = System.Windows.SystemParameters.PrimaryScreenHeight - 40;
                        IsMax = true;
                    }
                   
                    break;
                case "2": //最小化
                    if (w.WindowState == WindowState.Normal || w.WindowState == WindowState.Maximized)
                    {
                        w.WindowState = WindowState.Minimized;
                    }
                    break;
                case "3": //修改密码
                    FormInit.OpenDialog(this, new UpdateUser(), false);
                    break;
                case "4"://注销用户
                    if (CCMessageBox.Show("确定要注销吗？", "系统提示", CCMessageBoxButton.OKCancel) == CCMessageBoxResult.OK)
                    {
                        w.Hide();
                        CommonLogin.CommonUser = null;
                        new Login().Show();
                    }
                    break;
            }
        }


        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (CCMessageBox.Show("确定退出系统？", "系统提示", CCMessageBoxButton.OKCancel) == CCMessageBoxResult.OK)
            {
                Application.Current.Shutdown();
            }
        }


    }
    public class AccentColorMenuData
    {
        public string Name { get; set; }
        public Brush BorderColorBrush { get; set; }
        public Brush ColorBrush { get; set; }
        public virtual void DoChangeTheme()
        {
            var theme = ThemeManager.DetectAppStyle(Application.Current);
            var accent = ThemeManager.GetAccent(this.Name);
            ThemeManager.ChangeAppStyle(Application.Current, accent, theme.Item1);
        }
    }


}
