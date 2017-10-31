using _9M.Work.WPF_Common.WpfBind;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace _9M.Work.WPF_Common.Controls
{
   public class GoodsStatusComboBox : ComboBox
    {
        public GoodsStatusComboBox()
        {
            ComboBoxBind.BindGoodsStatusBox(this);
            this.Width = 70;
            this.DisplayMemberPath = "Text";
            this.SelectedValuePath = "Id";
            this.SelectedIndex = 0;
        }
    }
}
