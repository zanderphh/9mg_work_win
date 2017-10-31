using _9M.Work.DbObject;
using _9M.Work.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace _9M.Work.WPF_Main
{
    /// <summary>
    /// MainShell.xaml 的交互逻辑
    /// </summary>
    public partial class MainShell : Window
    {


        delegate void GetNewMessageHandle();

        public MainShell()
        {
            InitializeComponent();
            //this.WindowState = System.Windows.WindowState.Maximized;
            this.Left = 0.0;
            this.Top = 0.0;
            this.Width = System.Windows.SystemParameters.PrimaryScreenWidth;
            this.Height = System.Windows.SystemParameters.PrimaryScreenHeight - 40;

            icon();

            GetNewMessageHandle handle = new GetNewMessageHandle(loadNewMessage);
            handle.BeginInvoke(null, null);

        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }


        public DispatcherTimer icoTimer = new DispatcherTimer();
        NotifyIcon notifyIcon = new NotifyIcon();
        private long i = 0;
        System.Drawing.Icon taskICON = new System.Drawing.Icon(System.Windows.Forms.Application.StartupPath + @"\9mg.ico");
        System.Drawing.Icon taskICON_PNG = new System.Drawing.Icon(System.Windows.Forms.Application.StartupPath + @"\message.ico");



        public DispatcherTimer icoGetNewMessage = new DispatcherTimer();
        private void loadNewMessage()
        {
            icoGetNewMessage.Interval = TimeSpan.FromSeconds(60);
            icoGetNewMessage.Tick += new EventHandler(GetNewMessage);
            icoGetNewMessage.Start();
        }

        private void GetNewMessage(object sender, EventArgs e)
        {

            bool? isStartNewThread = false;

            BaseDAL dal = new BaseDAL();
            NewMsgNoticeModel model = dal.GetSingle<NewMsgNoticeModel>(x => x.model == "WorkOrder" && x.uname == WPF_Common.CommonLogin.CommonUser.UserName);
            if (model != null)
            {
                isStartNewThread = model.isplay;
            }

            if (isStartNewThread == true)
            {
                string sql = string.Format("select * from DB_9MFenXiao.dbo.WorkOrder where operatorEmp is null");
                DataTable dt = dal.QueryDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    iconPlay();
                }

            }
        }

        private void icon()
        {

            this.notifyIcon = new NotifyIcon();
            this.notifyIcon.Text = "魅工作台";//最小化到托盘时，鼠标点击时显示的文本
            this.notifyIcon.Icon = taskICON;//程序图标
            this.notifyIcon.Visible = true;

            this.notifyIcon.MouseClick += new System.Windows.Forms.MouseEventHandler((o, e) =>
                 {
                     if (e.Button == MouseButtons.Left)
                     {
                         if (isPlay)
                         {
                             isPlay = false;
                         }

                         iconStop();

                         if (this.WindowState == System.Windows.WindowState.Minimized)
                         {
                             this.WindowState = System.Windows.WindowState.Normal;
                         }

                         this.Topmost = true;
                         this.Topmost = false;
                         this.notifyIcon.Icon = taskICON;

                         //if (this.WindowState == System.Windows.WindowState.Maximized)
                         //{
                         //    this.WindowState = System.Windows.WindowState.Minimized;
                         //}
                         //else if (this.WindowState == System.Windows.WindowState.Minimized)
                         //{
                         //    this.WindowState = System.Windows.WindowState.Normal;
                         //}
                         //else if (this.WindowState == System.Windows.WindowState.Normal)
                         //{
                         //    this.WindowState = System.Windows.WindowState.Minimized;
                         //}
                     }
                 });

            //this.notifyIcon.BalloonTipText = "9魅工作台"; //汽泡文本
            //this.notifyIcon.ShowBalloonTip(1000);//汽泡显示的时间

            //闪烁图标
            icoTimer.Interval = TimeSpan.FromSeconds(0.3);
            icoTimer.Tick += new EventHandler(IcoTimer_Tick);

        }


        bool isPlay = false;

        public void iconPlay()
        {
            isPlay = true;
            icoTimer.Start();
        }

        public void iconStop()
        {
            icoTimer.Stop();
        }



        protected void IcoTimer_Tick(object sender, EventArgs e)
        {
            i = i + 1;
            if (i % 2 != 0)
            {
                this.notifyIcon.Icon = taskICON;
            }
            else
            {
                this.notifyIcon.Icon = taskICON_PNG;
            }
        }


    }
}
