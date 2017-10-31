using _9M.Work.Utility;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
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

namespace _9M.Work.ProjectRelease
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        string ShareDir = StaticNetWork.RemoteDir;
        public bool status = true;
        public MainWindow()
        {
            InitializeComponent();
            //清除共享连接
            bool cls = FileShareNet.clearState(StaticNetWork.RemoteIp);
            //连接磁盘
            status = FileShareNet.connectState(ShareDir, StaticNetWork.RemoteUser, StaticNetWork.RemotePassWork);
            if (!status)
            {
                MessageBox.Show("网盘权限无法连接");
            }
            tb_path.Text = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).AppSettings.Settings["loadpath"].Value;

        }
        System.Windows.Forms.FolderBrowserDialog fd = null;
        System.Windows.Forms.OpenFileDialog op = null;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            fd = new System.Windows.Forms.FolderBrowserDialog();
            if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                tb_path.Text = fd.SelectedPath;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            op = new System.Windows.Forms.OpenFileDialog();
            op.InitialDirectory = tb_path.Text;
            op.Multiselect = true;
            if (op.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {

               List<string> FileList =   op.FileNames.ToList();
               bool b =  UpLoadFiles(FileList);
               MessageBox.Show(b==true?"发布成功":"发布失败");
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            string Dir = tb_path.Text;
            List<string> FileList = new List<string>()
            {
                System.IO.Path.Combine(Dir,"9M.Work.AOP.dll"),
                System.IO.Path.Combine(Dir,"9M.Work.DbObject.dll"),
                System.IO.Path.Combine(Dir,"9M.Work.ErpApi.dll"),
                System.IO.Path.Combine(Dir,"9M.Work.JosApi.dll"),
                System.IO.Path.Combine(Dir,"9M.Work.Model.dll"),
                System.IO.Path.Combine(Dir,"9M.Work.TopApi.dll"),
                System.IO.Path.Combine(Dir,"9M.Work.WPF_Common.exe"),
                System.IO.Path.Combine(Dir,"9M.Work.WPF_Main.exe"),
                System.IO.Path.Combine(Dir,"9M.Work.WPF_Main.exe.config"),
                System.IO.Path.Combine(Dir,"9M.Work.Utility.dll"),
                System.IO.Path.Combine(Dir,"9M.Work.YouZan.dll")
              
            };
            bool b =  UpLoadFiles(FileList);
            MessageBox.Show(b == true ? "发布成功" : "发布失败");
        }

        public bool UpLoadFiles(List<string> FileList)
        {
            bool b = true;
            string UpPath = ShareDir + "//InSideWork//";
            for (int i = 0; i < FileList.Count; i++)
            {
                try
                {
                    File.Copy(FileList[i], UpPath + System.IO.Path.GetFileName(FileList[i]), true);
                }
                catch
                {
                    b = false;
                    break;
                }
            }
            return b;
        }
    }
}
