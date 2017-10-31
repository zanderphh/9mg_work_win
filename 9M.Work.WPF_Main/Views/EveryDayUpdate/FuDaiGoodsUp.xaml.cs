using _9M.Work.Utility;
using _9M.Work.WPF_Common;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace _9M.Work.WPF_Main.Views.EveryDayUpdate
{
    /// <summary>
    /// FuDaiGoodsUp.xaml 的交互逻辑
    /// </summary>
    public partial class FuDaiGoodsUp : UserControl
    {
        public FuDaiGoodsUp()
        {
            InitializeComponent();
        }
     
        private void Batch_BtnClick(object sender, RoutedEventArgs e)
        {

        }

        private void Tabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int Index = (sender as TabControl).SelectedIndex;
            switch (Index)
            {
                case 0:
                   // temp_batch.LoadBatchData(1,30);
                    break;
                case 1:
                    //PhotoTemp.com_batch.ItemsSource = null;
                    //PhotoTemp.BindBatch();
                    break;
                case 2:
                    //UpGoodsTemp.BindCheckListBox(5);
                    break;
                case 3:
                   // FudaiGoodsTemp.LoadGoodsData(1,30);
                    break;
            }
        }
    }
}
