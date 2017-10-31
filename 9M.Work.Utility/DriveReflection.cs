using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;

namespace _9M.Work.Utility
{
    /// <summary>
    /// 映射网盘
    /// </summary>
   public  class DriveReflection
    {
        [StructLayout(LayoutKind.Sequential)]
        public class NetResource
        {
            public int dwScope;
            public int dwType;
            public int dwDisplayType;
            public int dwUsage;
            public string LocalName;
            public string RemoteName;
            public string Comment;
            public string provider;
        }
        [DllImport("mpr.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int WNetGetConnection(
            [MarshalAs(UnmanagedType.LPTStr)] string localName,
            [MarshalAs(UnmanagedType.LPTStr)] StringBuilder remoteName,
            ref int length);
        /// <summary>
        /// 给定一个路径，返回的网络路径或原始路径。
        /// 例如：给定路径 P:\2008年2月29日(P:为映射的网络驱动器名)，可能会返回：“//networkserver/照片/2008年2月9日”
        /// </summary> 
        /// <param name="originalPath">指定的路径</param>
        /// <returns>如果是本地路径，返回值与传入参数值一样；如果是本地映射的网络驱动器</returns> 
        public static string GetUNCPath(string originalPath)
        {
            StringBuilder sb = new StringBuilder(512);
            int size = sb.Capacity;
            if (originalPath.Length > 2 && originalPath[1] == ':')
            {
                char c = originalPath[0];
                if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z'))
                {
                    int error = WNetGetConnection(originalPath.Substring(0, 2),
                        sb, ref size);
                    if (error == 0)
                    {
                        DirectoryInfo dir = new DirectoryInfo(originalPath);
                        string path = System.IO.Path.GetFullPath(originalPath)
                            .Substring(System.IO.Path.GetPathRoot(originalPath).Length);
                        return System.IO.Path.Combine(sb.ToString().TrimEnd(), path);
                    }
                }
            }
            return originalPath;

        }
        [DllImport("mpr.dll", CharSet = CharSet.Ansi)]
        private static extern int WNetAddConnection2(NetResource netResource, string password, string username, int flag);
        [DllImport("mpr.dll", CharSet = CharSet.Ansi)]
        private static extern int WNetCancelConnection2(string lpname, int flag, bool force);
        /// <summary>
        /// 映射网络驱动器
        /// </summary>
        /// <param name="localName">本地盘符 如U:</param>
        /// <param name="remotePath">远程路经 如\\\\172.18.118.106\\f</param>
        /// <param name="userName">远程服务器用户名</param>
        /// <param name="password">远程服务器密码</param>
        /// <returns>true映射成功，false映射失败</returns>
        public static bool WNetReflectDrive(string localName, string remotePath, string userName, string password)
        {
            NetResource netResource = new NetResource();
            netResource.dwScope = 2;
            netResource.dwType = 0x1;
            netResource.dwDisplayType = 3;
            netResource.dwUsage = 1;
            netResource.LocalName = localName;
            netResource.RemoteName = remotePath;
            netResource.provider = null;
            int ret = WNetAddConnection2(netResource, password, userName, 0);
            if (ret == 0)
                return true;
            return false;
        }

        /// <summary>
        /// 断开网路驱动器
        /// </summary>
        /// <param name="lpName">映射的盘符</param>
        /// <param name="flag">true时如果打开映射盘文件夹，也会断开,返回成功 false时打开映射盘文件夹，返回失败</param>
        /// <returns></returns>
        public static bool WNetDisconnectDrive(string lpName, bool flag)
        {
            int ret = WNetCancelConnection2(lpName, 0, flag);
            if (ret == 0)
                return true;
            return false;
        }

              /// <summary>
        /// 用net use delete命令移除网络共享连接
        /// </summary>
        /// <param name="Server">目标ip</param>
        /// <param name="ShareName">远程共享名</param>
        /// <param name="Username">远程登录用户</param>
        /// <param name="Password">远程登录密码</param>
        public static void RemoveShareNetConnect(string Server, string ShareName)
        {
            //System.Diagnostics.Process.Start("net.exe", @"use \\" + Server + @"\" + ShareName + " \"" + Password + "\" /user:\"" + Username + "\" ");
            Process process = new Process();
            process.StartInfo.FileName = "net.exe";
            process.StartInfo.Arguments = @"use \\" + Server + @"\" + ShareName + " /delete";
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.Start();
            process.WaitForExit();
            process.Close();
            process.Dispose();

        }
    
    }
}
