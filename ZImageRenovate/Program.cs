using _9M.Work.DbObject;
using _9M.Work.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace ZImageRenovate
{
    class Program
    {
        [DllImport("user32.dll", EntryPoint = "ShowWindow", SetLastError = true)]
        static extern bool ShowWindow(IntPtr hWnd, uint nCmdShow);
        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        static void Main(string[] args)
        {
            Console.Title = "ImageUpdate";
            IntPtr intptr = FindWindow("ConsoleWindowClass", "ImageUpdate");
            if (intptr != IntPtr.Zero)
            {
                ShowWindow(intptr, 2);//隐藏本dos窗体, 0: 后台执行；1:正常启动；2:最小化到任务栏；3:最大化
            }

            BaseDAL db = new BaseDAL();
            //连接网盘

            string LocalName = "Z:";
            string RemotePath = @"\\192.168.1.5\拾玖映画";
            string UserName = @"admin";
            string PassWord = "199711";
            bool b = true;
            try
            {
                //清空共享连接
                DriveReflection.RemoveShareNetConnect("192.168.1.5", "");
                //连接网盘
                int lengh = 20;
                int Conn = DriveReflection.WNetGetConnection(LocalName, new StringBuilder(RemotePath), ref lengh);
                if (Conn != 0)
                {
                    b = DriveReflection.WNetReflectDrive(LocalName, RemotePath, UserName, PassWord);
                }
            }
            catch (Exception)
            {
                b = false;
            }
            if (!b)
            {
                Console.WriteLine("网盘连接失败");
                return;
            }
            Console.WriteLine("网盘连接成功");
            while (true)
            {
                Console.WriteLine("开始执行--" + DateTime.Now.ToString());
                string FilePath = "Z:\\";
                Process compiler = new Process();
                compiler.StartInfo.FileName = "cmd.exe";
                compiler.StartInfo.Arguments = "/c dir " + FilePath + "*.jpg /s /l /b > c:\\ImageFiles.txt";
                compiler.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                compiler.EnableRaisingEvents = true;
                compiler.Exited += (obj, args2) =>
                {
                    DataTable dt = new DataTable();
                    dt.Columns.Add("GoodsNo", typeof(string));
                    dt.Columns.Add("Url", typeof(string));
                    string[] fileNames = System.IO.File.ReadAllLines("c:\\ImageFiles.txt", Encoding.Default);
                    Regex re = new Regex(@"^[\d]{2}[\w]{2,3}[\d]{4}.+(\.jpg)$", RegexOptions.IgnoreCase);
                    Regex GoodsRe = new Regex(@"^[a-zA-Z]+$");
                    Console.WriteLine("得到图片----" + fileNames.Length + "张" + DateTime.Now.ToString());
                    foreach (string file in fileNames)
                    {
                        string filename = Path.GetFileName(file).ToUpper();
                        if (re.IsMatch(filename))
                        {
                            int index = GoodsRe.IsMatch(filename.Substring(6, 1)) ? 9 : 8;
                            string GoodsNo = filename.Substring(0, index);
                            DataRow dr = dt.NewRow();
                            dr["GoodsNo"] = GoodsNo;
                            dr["Url"] = file;
                            dt.Rows.Add(dr);

                        }
                    }
                    Console.WriteLine("处理数据中----" + DateTime.Now.ToString());
                    db.ExecuteSql("truncate table Picture");
                    string Res = db.BulkCopy(dt, "Picture");
                    Console.WriteLine("处理结果: " + Res + "---------" + DateTime.Now.ToString());
                    File.Delete("c:\\ImageFiles.txt");
                    compiler.Close();
                };
                compiler.Start();
                Thread.Sleep(1000 * 60 * 60 * 6);
            }



        }




    }
}
