using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace _9M.Work.WPF_Common.WpfBind
{
    public class ListBoxBind
    {
        public static void BindListBox(ListBox listbox,IEnumerable Data,string DisplayMemberPath,string SelectedValuePath )
        {
            listbox.ItemsSource = Data;
            listbox.DisplayMemberPath = DisplayMemberPath;
            listbox.SelectedValuePath = SelectedValuePath;
        }
    }
}
