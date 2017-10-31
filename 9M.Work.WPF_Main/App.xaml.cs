using _9M.Work.AOP;
using _9M.Work.AOP.Goods;
using _9M.Work.Model;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;

namespace _9M.Work.WPF_Main
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
           
            Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
            //AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            #region Ioc注入
            //商品绩效和日志
            UnityContainerConfigure unc = new UnityContainerConfigure("unity", "GoodsLog");
            UnityAopFactory.GoodsServices = unc.GetServer<GoodsInterface>("GoodsAop");

            //标签打印（注入构照函数）
            Dictionary<string, object> parameterList = new Dictionary<string, object>();
            parameterList.Add("PrintT", PrintType.Lable);
            UnityAopFactory.PrintLabServices = unc.GetServer<PrintInterface>("PrintAop", parameterList);

           // Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("zh-Hans");
            #endregion
        }

        private void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {

            //CCMessageBox.Show(string.Format(@"发生错误:{0}\r\n请重试，如问题继续存在，请联系管理员", ((Exception)e.Exception).Message, "意外的操作"));
            //MessageBox.Show(string.Format(@"发生错误:{0}\r\n请重试，如问题继续存在，请联系管理员",
            //                                                 ((Exception)e.Exception).Message, "意外的操作"));
            e.Handled = true; //使用这一行代码告诉运行时，该异常被处理了，不再作为UnhandledException抛出了。

        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
//            CCMessageBox.Show(string.Format(@"发生错误:{0}\r\n,
//                                            错误详情:{1}\r\n
//                                            请重试，如问题继续存在，请联系管理员",
//                                                             ((Exception)e.ExceptionObject).Message,
//                                                             ((Exception)e.ExceptionObject).StackTrace), "意外的操作");
        }

        private void OnAppStartup_UpdateThemeName(object sender, StartupEventArgs e)
        {

            
        }
    }
}
