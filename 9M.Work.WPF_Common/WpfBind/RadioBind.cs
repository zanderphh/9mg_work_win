using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace _9M.Work.WPF_Common.WpfBind
{
    public class RadioBind
    {
        /// <summary>
        /// 得到一个控件的单选框选中的值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ReadSelectedContent(DependencyObject obj)
        {
            List<RadioButton> buttonlist = WPFControlsSearchHelper.GetChildObjects<RadioButton>(obj, string.Empty);
            string Res = string.Empty;
            foreach(var item in buttonlist)
            {
               if(item.IsChecked == true)
               {
                   Res = item.Content.ToString();
                   break;
               }
            }
            return Res;
        }

        public static void ClearSelect(DependencyObject obj)
        {
            List<RadioButton> buttonlist = WPFControlsSearchHelper.GetChildObjects<RadioButton>(obj, string.Empty);
            foreach (var item in buttonlist)
            {
                item.IsChecked = false;
            }
        }
    }
}
