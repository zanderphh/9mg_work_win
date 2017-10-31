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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace _9M.Work.WPF_Main.ControlTemplate.Activity
{
    /// <summary>
    /// ActivityRed.xaml 的交互逻辑
    /// </summary>
    public partial class ActivityRed : UserControl
    {
        public ActivityRed()
        {
            InitializeComponent();
        }

        public ActivityRed(string imageurl,string activityname,string price)
        {
            InitializeComponent();
            this.lab_acname.Content = activityname;
            this.lab_price.Content = price;
            this.img_picurl.Source = new BitmapImage(new Uri(imageurl+"_260x260.jpg",UriKind.RelativeOrAbsolute));
        }
    }
}
