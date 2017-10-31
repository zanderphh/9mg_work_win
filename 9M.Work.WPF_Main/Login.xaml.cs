using _9M.Work.DbObject;
using _9M.Work.Model;
using _9M.Work.Utility;
using _9M.Work.WPF_Common;
using _9M.Work.WPF_Common.WpfBind;
using _9M.Work.WPF_Main.ControlTemplate.PrintTemplate;
using _9Mg.Work.TopApi;
using MahApps.Metro;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Top.Api.Domain;

namespace _9M.Work.WPF_Main
{
    /// <summary>
    /// Login.xaml 的交互逻辑
    /// </summary>
    public partial class Login : Window
    {
        private BaseDAL db;
        private string Path = System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + "\\Config.txt";
        public Login()
        {
            InitializeComponent();
            if (!File.Exists(Path))
            {
                FileStream myFs = new FileStream(Path, FileMode.Create);
                myFs.Close();
            }
            ReacConfig();
            LoadUser();
            InitSqlConfig();
        }

        #region 方法
        /// <summary>
        /// 写入连接配置
        /// </summary>
        public void SaveConfig(string Ip, string Port, string UpIp)
        {
            FileStream stream = File.Open(Path, FileMode.OpenOrCreate, FileAccess.Write);
            stream.Seek(0, SeekOrigin.Begin);
            stream.SetLength(0);
            stream.Close();
            //向txt里面追加信息
            StreamWriter sw = new StreamWriter(Path, true, Encoding.GetEncoding("gb2312"));
            sw.WriteLine(Ip);
            sw.WriteLine(Port);
            sw.WriteLine(UpIp);
            sw.Flush();
            sw.Close();
        }

        public void InitSqlConfig()
        {
            string pwd = ConnIp.Text.Trim().Equals("192.168.2.190") ? "svse" : "www.9mg.cn";
            //CommonLogin.SqlConnString = string.Format(@"Server={0},{1};database=InSideWorkServer;uid=sa;pwd={2};Connection Timeout=4", ConnIp.Text.Trim(), ConnPort.Password.Trim(), pwd);
            BaseDAL.DBConnectionString = string.Format(@"Server={0},{1};database=InSideWorkServer;uid=sa;pwd={2};Connection Timeout=4", ConnIp.Text.Trim(), ConnPort.Password.Trim(), pwd);
            BaseDAL.TemplateConnectionString = string.Format(@"Server={0},{1};database=WorkPlatform;uid=sa;pwd={2};Connection Timeout=4", ConnIp.Text.Trim(), ConnPort.Password.Trim(), pwd);
        }
        /// <summary>
        /// 读取配置链接
        /// </summary>
        public void ReacConfig()
        {
            StreamReader sr = new StreamReader(Path, Encoding.Default);
            String line;
            int k = 0;
            while ((line = sr.ReadLine()) != null)
            {
                k++;
                switch (k)
                {
                    case 1:
                        ConnIp.Text = line;
                        break;
                    case 2:
                        ConnPort.Password = line;
                        break;
                    case 3:
                        UpdateIp.Text = line;
                        break;
                }
            }
        }

        /// <summary>
        /// 记住密码
        /// </summary>
        /// <param name="isRemember"></param>
        /// <param name="userName"></param>
        /// <param name="passWord"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        private void RemeberUser(bool isRemember, string userName, string passWord, string p1, string p2)
        {
            //保存密码
            RegistryKey regKey = Registry.CurrentUser.CreateSubKey("InSideUser");
            if (isRemember)
            {
                regKey.SetValue("uname", userName);
                regKey.SetValue("pwd", passWord);
                regKey.SetValue("shoptypeindex", p1);
                regKey.SetValue("shopname", p2);
                regKey.SetValue("ischecked", "1");
                regKey.Close();
            }
            else
            {
                regKey.SetValue("ischecked", "0");
                regKey.SetValue("pwd", "");
            }
        }

        /// <summary>
        /// 加载用户信息
        /// </summary>
        private void LoadUser()
        {
            //读取记住的密码
            RegistryKey regKey = Registry.CurrentUser.OpenSubKey("InSideUser");
            if (regKey != null)
            {
                if (regKey.GetValue("ischecked") != null)
                {
                    bool res = regKey.GetValue("ischecked").Equals("1");
                    if (res)
                    {
                        UserName.Text = regKey.GetValue("uname") != null ? regKey.GetValue("uname").ToString() : "用户名";
                        PassWrod.Password = regKey.GetValue("pwd") != null ? regKey.GetValue("pwd").ToString() : "密码";
                        int p1 = regKey.GetValue("shoptypeindex") != null ? Convert.ToInt32(regKey.GetValue("shoptypeindex")) : 0;
                        string p2 = regKey.GetValue("shopname") != null ? regKey.GetValue("shopname").ToString() : string.Empty;
                        regKey.Close();
                    }
                    Chk_Remind.IsChecked = res ? true : false;
                }
            }
        }
        #endregion



        //显示服务器链接配置
        private void SystemConfig_Click(object sender, RoutedEventArgs e)
        {

            if (this.Height == 345)
            {
                this.Height = 530;
            }
            else
            {
                this.Height = 345;
            }
        }

        private void Btn_CloseForm_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Btn_SaveConfig_Click(object sender, RoutedEventArgs e)
        {
            string Ip = ConnIp.Text.Trim();
            string Port = ConnPort.Password.Trim();
            string UpIp = UpdateIp.Text.Trim();
            if (!string.IsNullOrEmpty(Ip) && !string.IsNullOrEmpty(Port) && !string.IsNullOrEmpty(UpIp))
            {
                SaveConfig(Ip, Port, UpIp);
                this.Height = 345;
                InitSqlConfig();
            }
            else
            {
                CCMessageBox.Show("请完整的填写内容");
            }
        }


        #region 登陆和进度条

        private void Btn_Login_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(ConnIp.Text) || string.IsNullOrEmpty(ConnPort.Password))
            {
                CCMessageBox.Show("请配置连接");
                return;
            }
            db = new BaseDAL();
            BarPannel.Visibility = System.Windows.Visibility.Visible;
            string Name = UserName.Text.Trim();
            string Pass = PassWrod.Password.Trim();
            //实例委托
            AsyncEventHandler asy = new AsyncEventHandler(this.LoginUser);
            //异步调用开始，没有回调函数和AsyncState,都为null
            IAsyncResult ia = asy.BeginInvoke(Name, Pass, null, null);
        }
        public delegate void AsyncEventHandler(string Name, string Pass);

        public static string TextConnIp { get; set; }

        public void LoginUser(string Name, string Pass)
        {
            bool IsLogin = true;
            if (!string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Pass))
            {
                //  UserModel UserModel = db.UserEntities.Where(x => x.PassWord.Equals(Pass) && x.UserName.Equals(Name)).FirstOrDefault();
                UserInfoModel UserModel = new BaseDAL().GetSingle<UserInfoModel>(x => x.PassWord.Equals(Pass) && x.UserName.Equals(Name));
                if (UserModel != null)
                {
                    System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        SessionUserModel model = new SessionUserModel() { Id = UserModel.Id, UserName = UserModel.UserName, PassWord = UserModel.PassWord, DeptId = UserModel.DeptId, IsDeptAdmin = UserModel.IsDeptAdmin, IsAdmin = UserModel.UserName.Equals("admin", StringComparison.CurrentCultureIgnoreCase), Alias = UserModel.Alias };
                        //得到用户的权限
                        List<PermissionModel> perlist = new BaseDAL().QueryList<PermissionModel>(@"select *  from T_permission a join T_userPermission b on a.id = b.PermissionId
                        join T_userinfo c on b.UserId = c.id where c.id = " + model.Id, new object[] { });
                        model.PermissionList = perlist;
                        //为用户添加API授权
                        CommonLogin.AssembleAuthorization(model);
                        //全局用户
                        CommonLogin.CommonUser = model;
                        TextConnIp = ConnIp.Text;
                        RemeberUser(Convert.ToBoolean(Chk_Remind.IsChecked), Name, Pass, "0", string.Empty);
                        //如果是质检部连一下网盘
                        if (model.DeptId == 2)
                        {
                            //清除共享连接
                            bool cls = NetWorkUntity.clearState(CommonLogin.RemoteIp);
                            //连接磁盘
                            NetWorkUntity.connectState(CommonLogin.RemoteDir, CommonLogin.RemoteUser, CommonLogin.RemotePassWork);
                        }
                        this.Hide();
                        Bootstrapper b = new Bootstrapper();
                        b.Run();
                    }));
                }
                else
                {
                    CCMessageBox.Show("账号或密码错误");
                    IsLogin = false;
                }
            }
            else
            {
                CCMessageBox.Show("完整填写登陆信息");
                IsLogin = false;
            }
            if (!IsLogin)
            {
                System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    BarPannel.Visibility = System.Windows.Visibility.Collapsed;
                }));
            }
        }


        #endregion

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Label_MouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.9mg.cn");
        }
    }
}
