using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _9M.Work.WPF_Common.Controls
{
   public  class BrowserDialog
    {
       /// <summary>
       /// 得到一个选择的文件夹
       /// </summary>
       /// <returns></returns>
       public string GetUrl()
       {
           string Url = string.Empty;
           FolderBrowserDialog op = new FolderBrowserDialog();
           if (op.ShowDialog()==DialogResult.OK)
           {
               Url = op.SelectedPath;
           }
           return Url;
       }
    }
}
