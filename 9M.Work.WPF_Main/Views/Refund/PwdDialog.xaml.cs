using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace _9M.Work.WPF_Main.Views.Refund
{
    /// <summary>
    /// Interaction logic for PwdDialog.xaml
    /// </summary>
    public partial class PwdDialog : Window
    {
        public PwdDialog()
        {
            InitializeComponent();
            txtPwd.Focus();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (txtPwd.Password.Equals("1591"))
                {
                    this.DialogResult = true;
                }
                else
                {
                    MessageBox.Show("密码错误","提示");
                }
            }
        }
    }
}
