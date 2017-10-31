using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace SysUpdate
{
    public class OperIni
    {
        #region "声明变量"

        /// <summary>
        /// 写入INI文件
        /// </summary>
        /// <param name="section">节点名称[如[TypeName]]</param>
        /// <param name="key">键</param>
        /// <param name="val">值</param>
        /// <param name="filepath">文件路径</param>
        /// <returns></returns>
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filepath);
        /// <summary>
        /// 读取INI文件
        /// </summary>
        /// <param name="section">节点名称</param>
        /// <param name="key">键</param>
        /// <param name="def">值</param>
        /// <param name="retval">stringbulider对象</param>
        /// <param name="size">字节大小</param>
        /// <param name="filePath">文件路径</param>
        /// <returns></returns>
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retval, int size, string filePath);

        //private string strFilePath = Application.StartupPath + "\\FileConfig.ini";//获取INI文件路径
        //private string strSec = ""; //INI文件名

        #endregion

        /// <summary>
        /// 读取Ini
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public static int ReadIni(string section, string key, string deVal, string filepath)
        {
            StringBuilder sb = new StringBuilder();
            return GetPrivateProfileString(section, key, deVal, sb, 256, filepath);
        }

        /// <summary>
        /// 写入Ini
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public static long WriteIni(string section, string key, string val, string filepath)
        {
            return WritePrivateProfileString(section, key,val,filepath);
        }

        /// <summary>
        /// 删除节点
        /// </summary>
        /// <param name="section"></param>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public static long DeleteSection(string section, string filepath)
        {
            return WritePrivateProfileString(section, null, null, filepath);
        }

        /// <summary>
        /// 删除节点的键值
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public static long DeleteKey(string section,string key, string filepath)
        {
            return WritePrivateProfileString(section, key, null, filepath);
        }

        /// <summary>
        /// 自定义读取INI文件中的内容方法
        /// </summary>
        /// <param name="Path"></param>
        /// <param name="Section"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string ContentValue(string Path, string Section, string key)
        {

            StringBuilder temp = new StringBuilder(1024);
            GetPrivateProfileString(Section, key, "", temp, 1024, Path);
            return temp.ToString();
        }
    }
}
