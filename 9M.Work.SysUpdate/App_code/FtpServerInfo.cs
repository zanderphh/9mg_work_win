using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SysUpdate
{
    public static class FtpServerInfo
    {

        static string configPath = System.IO.Path.Combine(System.Environment.CurrentDirectory, "Config.ini");

        public static string FtpIP
        {
            get { return "192.168.1.2"; }
          //  get { return SecurityHelper.DecryptString(OperIni.ContentValue(configPath, "Update", "Ip"), SecurityHelper.EncryptKey); }
        }

        public static string FtpPort
        {
           // get { return SecurityHelper.DecryptString(OperIni.ContentValue(configPath, "Update", "Port"), SecurityHelper.EncryptKey); }
            get { return "21"; }
        }

        public static string FtpUser
        {
            get { return "insideuser"; }
            //get { return SecurityHelper.DecryptString(OperIni.ContentValue(configPath, "Update", "User"), SecurityHelper.EncryptKey); }
        }

        public static string FtpPwd
        {
            get { return "www.9myp.com"; }
            //get { return SecurityHelper.DecryptString(OperIni.ContentValue(configPath, "Update", "PassWord"), SecurityHelper.EncryptKey); }
        }

        public static string SysDirName
        {
            get { return "\\"; }
        }

        /// <summary>
        /// ftp系统文件完整路径
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string FtpSysFileFullPath(string dir, string fileName)
        {
            return @"\" + dir + @"\" + fileName;
        }

    }
}
