using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SysUpdate
{
    public class ClientInfo
    {

        /// <summary>
        /// 临时存放的文件夹名称
        /// </summary>
        public static String TempSaveFolder
        {
            get { return "tempo2oDownload"; }
        }

        /// <summary>
        /// 临时存放文件的完成路径
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static String TempSaveFullPath(string dirName,string fileName)
        {
            return System.IO.Path.GetTempPath() + dirName + @"\" + fileName;
        }

        /// <summary>
        /// 启动App名称
        /// </summary>
        public static String startAppName
        { get { return @"\9M.Work.WPF_Main.exe"; } }


        /// <summary>
        /// 关闭应用程序的名称
        /// </summary>
        public static String killAppName
        { get { return @"9M.Work.WPF_Main.WPF_Main"; } }

    }



}
