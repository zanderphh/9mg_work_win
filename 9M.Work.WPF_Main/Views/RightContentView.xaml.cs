using _9M.Work.WPF_Main.Infrastrcture;
using _9M.Work.WPF_Main.Views.NavigationView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace _9M.Work.WPF_Main.Views
{
    /// <summary>
    /// RightContentView.xaml 的交互逻辑
    /// </summary>
    public partial class RightContentView : UserControl
    {
        public RightContentView()
        {
            InitializeComponent();
            FormInit.RightView = this;
            new FormInit().OpenView("业务导航", new Navigation(), false);
        }

        private void sampleSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TabItem item = ((sender as TabControl).SelectedItem) as TabItem;
            if (item == null)
            {
                item = (sender as TabControl).Items[0] as TabItem;
            }
            TextBlock tb = (item.Header as StackPanel).Children[0] as TextBlock;
            TopView.CommonTopView.NativeText.Text = "[ " + tb.Text + " ]";
        }
    }
}
