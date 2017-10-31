using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using SysUpdate;

namespace JunZhe.O2O.SysUpdate
{
    public partial class frmMain : Form
    {
        #region 窗体边框阴影效果变量申明

        private const int CS_DropSHADOW = 0x20000;
        private const int GCL_STYLE = (-26);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SetClassLong(IntPtr hwnd, int nIndex, int dwNewLong);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetClassLong(IntPtr hwnd, int nIndex);

        #endregion


        FtpClient ftpc = null;
        /// <summary>
        /// ftp服务器上更新目录的所有文件
        /// </summary>
        String[] ServerFiles;

        /// <summary>
        /// 需要更新的文件
        /// </summary>
        List<FtpFileInfo> NeedUpdateFiles = null;


        private delegate void AsyncDownload(string arg);

        private void DownloadCallback(IAsyncResult ir)
        {
            //lab_msgText.Text = "正在启动系统...";
            //openSys();
        }


        public frmMain()
        {
            InitializeComponent();
            //窗体样式
            panel_top.BackColor = ColorTranslator.FromHtml("#009ded");
            panel_bottom.BackColor = ColorTranslator.FromHtml("#f0f5f6");
            listView1.HeadColor = ColorTranslator.FromHtml("#009ded");
            SetClassLong(this.Handle, GCL_STYLE, GetClassLong(this.Handle, GCL_STYLE) | CS_DropSHADOW); //API函数加载，实现窗体边框阴影效果
            listViewInit();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            if (FtpServerInfo.FtpIP.Length == 0)
            {
                MessageBox.Show("初次始用请配置更新地址", "提示");
                openSys();
            }
            else
            {
                if (!Ping.CmdPing(FtpServerInfo.FtpIP))
                {
                    MessageBox.Show("更新地址无法连接，请检查网络", "提示");
                    openSys();
                }
                else
                {
                    CheckForIllegalCrossThreadCalls = false;
                    //CheckAppRunStatus(ClientInfo.killAppName);
                    AsyncDownload startDownload = new AsyncDownload(DownLoad);
                    startDownload.BeginInvoke(Application.StartupPath, new AsyncCallback(DownloadCallback), null);
                }
            }

        }


        private void listViewInit()
        {
            listView1.GridLines = true;
            this.listView1.View = View.Details;
            this.listView1.Scrollable = true;

            this.listView1.Columns.Add("序号", 40);
            this.listView1.Columns.Add("文件名", 160);
            this.listView1.Columns.Add("文件(KB)", 80);
            this.listView1.Columns.Add("下载(kB)", 80);
        }

        /// <summary>
        /// 获取本地文件信息
        /// </summary>
        /// <returns></returns>
        private Hashtable GetLocalhostFile(string ExePath)
        {
            Hashtable localFileHtbl = new Hashtable();
            DirectoryInfo d = new DirectoryInfo(@"" + ExePath + "");
            List<DirectoryInfo> list = d.GetDirectories().ToList<DirectoryInfo>();
            list.Insert(0, d);
            foreach (DirectoryInfo dir in list)
            {
                FileInfo[] fi = dir.GetFiles();
                for (int i = 0; i < fi.Length; i++)
                {
                    string fileName = fi[i].Name;
                    string old_suffix = fi[i].Extension;
                    string suffixToLower = fi[i].Extension.ToLower();
                    //将修改时间转换为长整型
                    if (!localFileHtbl.Contains(fileName.Replace(old_suffix, suffixToLower)))
                    {
                        localFileHtbl.Add(fileName.Replace(old_suffix, suffixToLower), Convert.ToInt64(fi[i].LastWriteTime.ToString("yyyyMMddHHmmss")));
                    }

                }

            }
            return localFileHtbl;
        }

        /// <summary>
        /// 本地文件和服务器文件对比返回需要更新的文件
        /// </summary>
        /// <param name="hashLocalFile"></param>
        private List<FtpFileInfo> GetNeedUpdateFile(Hashtable hashLocalFile)
        {
            List<FtpFileInfo> validArraylist = new List<FtpFileInfo>();

            //获取FTP上的文件列表
            ftpc = new FtpClient(FtpServerInfo.FtpIP, FtpServerInfo.FtpUser, FtpServerInfo.FtpPwd);
            lab_msgText.Text = "正在登录远程服务器......";
            ftpc.Login();
            lab_msgText.Text = "远程服务器登陆成功！";

            //应用程序的获取所有文件夹
            lab_msgText.Text = "正在获取服务器根文件夹";
            List<string> ftpServerConfigDir = FtpHelper.GetDirctory("");

            for (int i = 0; i < ftpServerConfigDir.Count; i++)
            {
                lab_msgText.Text = string.Format("已获取{0}文件夹", (i + 1).ToString());
                ftpServerConfigDir[i] = FtpServerInfo.SysDirName + ftpServerConfigDir[i];
            }

            ftpServerConfigDir.Insert(0, FtpServerInfo.SysDirName);

            //获取所有文件夹的文件
            Dictionary<string, List<string>> dict = new Dictionary<string, List<string>>();

            foreach (String dir in ftpServerConfigDir)
            {
                List<string> files = ftpc.GetFileList(dir).ToList<string>();
                lab_msgText.Text = string.Format("已加载{0}文件夹下{1}个文件", dir, files.Count.ToString());
                dict.Add(dir, files);
            }

            //KeyValuePai key:服务器文件目录 value:该目录的文件
            foreach (KeyValuePair<string, List<String>> kvp in dict)
            {
                ServerFiles = kvp.Value.ToArray();

                //遍历FTP服务器上文件
                foreach (string ftpServerFile in ServerFiles)
                {
                    try
                    {
                        if (ftpServerFile.Length > 0 && ftpServerFile.Contains("."))
                        {
                            FtpFileInfo fi = new FtpFileInfo();

                            //系统更新获取根目录文件，否则获取配置文件更新目录
                            string ftpServerFilePath = kvp.Key.ToString() + "\\" + ftpServerFile;

                            long ftpServerFileLastModifyTime = ftpc.GetModifyTime(ftpServerFilePath);

                            fi.dir = kvp.Key.ToString();
                            fi.FileSize = ftpc.GetFileSize(ftpServerFilePath);
                            fi.fileName = ftpServerFile;

                            string hour = Convert.ToString((Convert.ToInt32(ftpServerFileLastModifyTime.ToString().Substring(8, 2)) + 8));

                            if (hour.Length == 1) { hour = "0" + hour; }

                            ftpServerFileLastModifyTime = Convert.ToInt64(ftpServerFileLastModifyTime.ToString().Substring(0, 8) + hour + ftpServerFileLastModifyTime.ToString().Substring(10));

                            if (hashLocalFile.Contains(ftpServerFile))
                            {
                                if (ftpServerFileLastModifyTime - Convert.ToInt64(hashLocalFile[ftpServerFile]) > 0)
                                {
                                    validArraylist.Add(fi);
                                }
                            }
                            else
                            {
                                validArraylist.Add(fi);
                            }
                        }
                    }
                    catch
                    {
                        int ks = 1;
                    }
                }
            }

            return validArraylist;
        }

        /// <summary>
        /// 下载回调
        /// </summary>
        /// <param name="ar"></param>
        private void downCallBack(IAsyncResult ar)
        {
            if (ar.IsCompleted)
            {
                curUpdateFile++;
                lab_msgText.Text = "正在下载第" + curUpdateFile + "个文件,一共需下载" + NeedUpdateFiles.Count + "个文件";
                progressBar1.PerformStep();

                if (curUpdateFile == NeedUpdateFiles.Count)
                {

                    lab_msgText.Text = "下载完成,正在启动更新...";
                    UpdateSys(Application.StartupPath);
                    openSys();
                }
                else
                {
                    AsyncCallback acl_down = new AsyncCallback(downCallBack);

                    //Ftp子文件夹
                    string ftpServerDir = NeedUpdateFiles[curUpdateFile].dir;
                    //本地临时文件夹(本地临时文件的目录+Ftp目录子文件夹)
                    string localTempDir = ftpServerDir == "\\" ? ClientInfo.TempSaveFolder : ClientInfo.TempSaveFolder + ftpServerDir;
                    //文件名
                    string fileName = NeedUpdateFiles[curUpdateFile].fileName;

                    ftpc.BeginDownload(FtpServerInfo.FtpSysFileFullPath(ftpServerDir, fileName), ClientInfo.TempSaveFullPath(localTempDir, fileName), acl_down);

                }
            }
        }


        private void UpdateSys(string ExePath)
        {
            try
            {
                String TempRootDir = System.IO.Path.GetTempPath() + ClientInfo.TempSaveFolder;

                DirectoryInfo dir2 = new DirectoryInfo(TempRootDir);
                FileInfo[] fi = dir2.GetFiles();
                for (int i = 0; i < fi.Length; i++)
                {
                    string fileName = fi[i].Name;
                    string OrignFile, NewFile;
                    OrignFile = ClientInfo.TempSaveFullPath(ClientInfo.TempSaveFolder, fileName);
                    NewFile = ExePath + @"\" + fileName;
                    if (!fileName.Contains("9Mg.AutoUpdate.exe"))
                    {
                        lab_msgText.Text = string.Format("正在复制第{0}个文件", (i + 1).ToString());
                        File.Copy(OrignFile, NewFile, true);
                        lab_msgText.Text = string.Format("第{0}个文件已复制完成", (i + 1).ToString());
                    }

                }

                String[] dirs = Directory.GetDirectories(TempRootDir);

                foreach (String dir in dirs)
                {
                    DirectoryInfo di = new DirectoryInfo(dir);

                    foreach (FileInfo f in di.GetFiles())
                    {
                        if (!Directory.Exists(ExePath + @"\" + di.Name + @"\"))
                        {
                            Directory.CreateDirectory(ExePath + @"\" + di.Name + @"\");
                        }

                        File.Copy(dir + @"\" + f.Name, ExePath + @"\" + di.Name + @"\" + f.Name, true);

                        FileInfo filesize = new FileInfo(ExePath + @"\" + di.Name + @"\" + f.Name);

                        int fileIndex = this.listView1.FindItemWithText(f.Name).Index;
                        this.listView1.Items[fileIndex].SubItems[3].Text = filesize.Length.ToString();
                        if (filesize.Length == Convert.ToInt64(this.listView1.Items[fileIndex].SubItems[2].Text))
                        {
                            this.listView1.Items[fileIndex].ForeColor = Color.Red;
                        }
                    }
                }

                deleteDir();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message.ToString(), "错误提示!");
                deleteDir();
            }
        }

        /// <summary>
        /// 删除目录文件夹
        /// </summary>
        /// <returns></returns>
        public bool deleteDir()
        {
            bool isSucceed = true;

            try
            {
                if (Directory.Exists(System.IO.Path.GetTempPath() + ClientInfo.TempSaveFolder))
                {
                    foreach (string d in Directory.GetFileSystemEntries(System.IO.Path.GetTempPath() + ClientInfo.TempSaveFolder))
                    {
                        if (Directory.Exists(d))
                        {
                            Directory.Delete(d, true);
                        }
                        else if (File.Exists(d))
                            File.Delete(d);
                    }

                    Directory.Delete(System.IO.Path.GetTempPath() + ClientInfo.TempSaveFolder);
                }
            }
            catch
            {
                isSucceed = false;
            }

            return isSucceed;
        }


        ///<summary>     
        /// 验证是否已经运行  
        /// </summary>  
        public static void CheckAppRunStatus(string appName)
        {
            System.Diagnostics.Process[] app = System.Diagnostics.Process.GetProcessesByName(appName);
            if (app.Length > 0)
            {
                app[0].Kill();
            }
        }


        /// <summary>
        /// APP是否运行
        /// </summary>
        /// <param name="appName"></param>
        /// <returns></returns>
        public static bool AppIsRunStatus(string appName)
        {
            System.Diagnostics.Process[] app = System.Diagnostics.Process.GetProcessesByName(appName);
            if (app.Length > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        private void openSys()
        {
            try
            {
                Thread.Sleep(1000);
                System.Diagnostics.Process ps = new System.Diagnostics.Process();
                ps.StartInfo.FileName = Application.StartupPath + ClientInfo.startAppName;
                ps.Start();
                Application.Exit();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message.ToString(), "提示");
                Application.Exit();
            }
        }


        /// <summary>
        /// 文件下载
        /// </summary>
        private void DownLoad(string ExePath)
        {
            try
            {

                lab_msgText.Text = "正在启动程序...";
                List<string> tempAlldir = new List<string>();

                this.listView1.BeginUpdate();

                NeedUpdateFiles = GetNeedUpdateFile(this.GetLocalhostFile(ExePath));

                this.listView1.EndUpdate();

                Boolean StartApp = false;

                if (NeedUpdateFiles.Count > 0)
                {
                    this.Height = 330;
                    #region 是否能够开启多个应用
                    if (AppIsRunStatus(ClientInfo.killAppName))
                    {
                        MessageBox.Show("发现新版本！无法开启多个窗口,系统将自动退出!");

                        //关闭当前运行的进程实例
                        CheckAppRunStatus(ClientInfo.killAppName);

                        Application.Exit();
                    }
                    #endregion
                    else
                    {

                        lab_msgText.Text = string.Format("{0}个文件有更新", NeedUpdateFiles.Count.ToString());

                        //将文件信息加载到控件
                        int i = 1;

                        foreach (FtpFileInfo f in NeedUpdateFiles)
                        {
                            //绑定到控件
                            ListViewItem lvi = new ListViewItem(new string[] { i.ToString(), f.fileName, f.FileSize.ToString(), "" });
                            i++;
                            this.listView1.Items.Add(lvi);

                            if (!tempAlldir.Contains(f.dir))
                            {
                                tempAlldir.Add(f.dir);
                            }
                        }

                        if (MessageBox.Show("发现新版本!是否更新?", "更新提示", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                        {
                            deleteDir();

                            lab_msgText.Text = "正在下载更新文件,请稍后...";
                            int m = (NeedUpdateFiles.Count != 0) ? (100 / NeedUpdateFiles.Count) : 0;
                            progressBar1.Step = m;

                            DirectoryInfo dir = new DirectoryInfo(System.IO.Path.GetTempPath());
                            foreach (String s in tempAlldir)
                            {
                                if (s.Equals("\\"))
                                {
                                    if (Directory.Exists(System.IO.Path.GetTempPath() + @"\" + ClientInfo.TempSaveFolder))
                                    {
                                        deleteDir();
                                    }

                                    dir.CreateSubdirectory(ClientInfo.TempSaveFolder);
                                }
                                else
                                {
                                    new DirectoryInfo(System.IO.Path.GetTempPath() + ClientInfo.TempSaveFolder).CreateSubdirectory(s.Replace("\\", ""));
                                }


                            }

                            AsyncCallback acl_down = new AsyncCallback(downCallBack);
                            //Ftp子文件夹
                            string ftpServerDir = NeedUpdateFiles[curUpdateFile].dir;
                            //本地临时文件夹(本地临时文件的目录+Ftp目录子文件夹)
                            string localTempDir = ftpServerDir == "\\" ? ClientInfo.TempSaveFolder : ClientInfo.TempSaveFolder + ftpServerDir;
                            //文件名
                            string fileName = NeedUpdateFiles[curUpdateFile].fileName;
                            ftpc.BeginDownload(FtpServerInfo.FtpSysFileFullPath(ftpServerDir, fileName), ClientInfo.TempSaveFullPath(localTempDir, fileName), acl_down);
                        }
                        else
                        {
                            StartApp = true;
                        }
                    }
                }
                else
                {
                    StartApp = true;
                }

                if (StartApp)
                {
                    lab_msgText.Text = "正在启动系统...";
                    Thread.Sleep(1000);
                    openSys();
                }


            }
            catch (Exception err)
            {
                lab_msgText.Text = "系统更新失败,正在恢复最近一次更新,错误提示:" + err.Message.ToString();
                Thread.Sleep(1000);
                openSys();
            }


        }

        /// <summary>
        /// 当前已经更新的文件数量
        /// </summary>
        int curUpdateFile = 0;
    }


    public class FtpFileInfo
    {
        /// <summary>
        /// 文件大小
        /// </summary>
        public long FileSize { get; set; }
        /// <summary>
        /// 文件名称
        /// </summary>
        public String fileName { get; set; }
        /// <summary>
        /// 文件目录
        /// </summary>
        public String dir { get; set; }
    }


    public class fileBaseInfo
    {
        public Int64 lastUpdatetime
        { get; set; }

        public string name
        { get; set; }

    }
}
