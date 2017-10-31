using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace _9M.Work.Utility
{
    public class FontOperate
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern int WriteProfileString(string lpszSection, string lpszKeyName, string lpszString);

        [DllImport("user32.dll")]
        public static extern int SendMessage(int hWnd,
        uint Msg,
        int wParam,
        int lParam
        );
        [DllImport("gdi32")]
        public static extern int AddFontResource(string lpFileName);


        public static bool InstallFont(string sFontFileName, string sFontName)
        {
            string _sTargetFontPath = string.Format(@"{0}\fonts\{1}", System.Environment.GetEnvironmentVariable("WINDIR"), sFontFileName);//系统FONT目录
            string _sResourceFontPath = string.Format(@"{0}\Font\{1}", System.Windows.Forms.Application.StartupPath, sFontFileName);//需要安装的FONT目录
            try
            {
                if (!File.Exists(_sTargetFontPath) && File.Exists(_sResourceFontPath))
                {
                    int _nRet;
                    File.Copy(_sResourceFontPath, _sTargetFontPath);
                    _nRet = AddFontResource(_sTargetFontPath);
                    _nRet = WriteProfileString("fonts", sFontName + "(TrueType)", sFontFileName);
                }
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
