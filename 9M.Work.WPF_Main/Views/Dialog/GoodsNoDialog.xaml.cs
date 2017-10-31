using _9M.Work.WPF_Main.Infrastrcture;
using _9M.Work.WPF_Main.Views.DataCenter;
using _9M.Work.WPF_Main.Views.Statistics;
using Microsoft.Practices.Prism.Commands;
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

namespace _9M.Work.WPF_Main.Views.Dialog
{
    /// <summary>
    /// GoodsNoDialog.xaml 的交互逻辑
    /// </summary>
    public partial class GoodsNoDialog : UserControl, BaseDialog
    {
        private object baseForm;
        private int flag = 0;
        public GoodsNoDialog(object obj, int flag)
        {
            this.DataContext = this;
            InitializeComponent();
            baseForm = obj;
            this.flag = flag;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string Tag = (sender as Button).Tag.ToString();
            List<string> GoodsList = Tb_Goods.Text.Split('\n').Where(x => !string.IsNullOrEmpty(x)).ToList();
            GoodsList = GoodsList.Select(x => x.Replace("\r", "").Trim()).Distinct().ToList();
            switch (Tag)
            {
                case "1":
                    string Name = baseForm.GetType().Name;
                    switch (Name)
                    {
                        case "SalesStatistics" :
                            SalesStatistics s = (SalesStatistics)baseForm;
                            s.GoodsNoList = GoodsList;
                            s.lab_importcount.Content = "[" + GoodsList.Count + "]";
                            FormInit.CloseDialog(this);
                            break;
                    }
                    break;
                case "2":
                    FormInit.CloseDialog(this);
                    break;
                case "3":
                    FormInit.CloseDialog(this);
                    break;
            }
        }

        public Microsoft.Practices.Prism.Commands.DelegateCommand CancelCommand
        {
            get { return new DelegateCommand(CloseDialog); }

        }

        public void CloseDialog()
        {
            FormInit.CloseDialog(this);
        }

        public bool IsNavigationTarget(Microsoft.Practices.Prism.Regions.NavigationContext navigationContext)
        {
            throw new NotImplementedException();
        }

        public void OnNavigatedFrom(Microsoft.Practices.Prism.Regions.NavigationContext navigationContext)
        {
            throw new NotImplementedException();
        }

        public void OnNavigatedTo(Microsoft.Practices.Prism.Regions.NavigationContext navigationContext)
        {
            throw new NotImplementedException();
        }


        public string Title
        {
            get { return "款号选择"; }
        }
    }
}
