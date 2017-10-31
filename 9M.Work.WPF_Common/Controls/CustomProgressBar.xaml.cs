using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace _9M.Work.WPF_Common.Controls
{
    /// <summary>
    /// CustomProgressBar.xaml 的交互逻辑
    /// </summary>
    public partial class CustomProgressBar : UserControl, INotifyPropertyChanged
    {
        private double currentRate = 0;
        public DispatcherTimer timer;
        private int ii = 0;

        public CustomProgressBar()
        {
            InitializeComponent();
            this.DataContext = this;  //进度条必须加载
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 300);
            timer.Tick += timer_Tick;
        }


        private void timer_Tick(object sender, EventArgs e)
        {
            ii++;
            InitLoad.Text = GetInitContent(ii % 4);
        }

        #region PublicMethod

        /// <summary>
        /// 加载Loading中的字符
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public string GetInitContent(int time)
        {
            string Content = "数据准备中";
            if (time == 1)
            {
                Content += ".";
            }
            else if (time == 2)
            {
                Content += "..";
            }
            else if (time == 3)
            {
                Content += "...";
            }
            else
            {
                Content += "....";
            }
            return Content;
        }

        /// <summary>
        /// 加载中字样开启或关闭
        /// </summary>
        /// <param name="IsStart"></param>
        public void Loading(bool IsStart)
        {
            if (IsStart)
            {
                timer.Start();
            }
            else
            {
                timer.Stop();
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    InitLoad.Text = "";
                }));

            }
        }

        /// <summary>
        /// 设置进度条的标题内容
        /// </summary>
        /// <param name="Title"></param>
        public void SetNavigation(string Title)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    Navigation.Text = Title;
                }));
        }

        /// <summary>
        /// 打开或关闭进度条
        /// </summary>
        /// <param name="IsStart"></param>
        public void LoadBar(bool IsStart)
        {
            System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (IsStart)
                {
                    TuChen.Visibility = System.Windows.Visibility.Visible;
                    BarPanel.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    TuChen.Visibility = System.Windows.Visibility.Collapsed;
                    BarPanel.Visibility = System.Windows.Visibility.Collapsed;
                }
            }));
        }

        /// <summary>
        /// 加载滚动条的值
        /// </summary>
        /// <param name="Total"></param>
        /// <param name="current"></param>
        public void UpdateBarValue(int Total, int current)
        {
            currentRate = Convert.ToDouble(current) / Total * 100;
            SuccessRate = currentRate * 100 / 100;
        }


        #endregion

        #region Properties

        private double successRate = 100;
        public double SuccessRate
        {
            get
            {
                return successRate;
            }
            set
            {
                if (value != successRate)
                {
                    successRate = value;
                    OnPropertyChanged("SuccessRate");
                }
            }
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
