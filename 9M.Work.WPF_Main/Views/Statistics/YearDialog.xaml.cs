using _9M.Work.WPF_Common;
using _9M.Work.WPF_Main.Infrastrcture;
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

namespace _9M.Work.WPF_Main.Views.Statistics
{
    /// <summary>
    /// YearDialog.xaml 的交互逻辑
    /// </summary>
    public partial class YearDialog : UserControl, BaseDialog
    {
        public YearDialog()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int Tag = Convert.ToInt32((sender as Button).Tag);
            switch (Tag)
            {
                case 0:
                    string Flag1 = string.Empty;
                    string Flag2 = string.Empty;
                    List<string> FirstList = GetKeyWord(tb1, stack1, out Flag1);
                    List<string> SecondList = GetKeyWord(tb2, stack2, out Flag2);
                    FirstList.AddRange(SecondList);
                    SalesStatistics ss = (SalesStatistics)FormInit.FindFather(this);
                    ss.tb_yearseason.Text = Flag1 + " " + Flag2;
                    ss.YearSeasonList = FirstList;
                    CloseDialog();
                    break;
                case 1:
                    CloseDialog();
                    break;
            }

        }

        public List<string> GetKeyWord(TextBox tb, StackPanel stack, out string Flag)
        {
            string text = string.Empty;
            List<string> list = new List<string>();
            string Year = tb.Text;
            List<CheckBox> checklist = WPFControlsSearchHelper.GetChildObjects<CheckBox>(stack, string.Empty).Where(x => x.IsChecked == true).ToList();
            if (!string.IsNullOrEmpty(Year) && checklist.Count > 0)
            {
                text += Year;
                string First = Year[Year.Length - 1].ToString();
                string Second = string.Empty;
                checklist.ForEach(x =>
                {
                    string Season = x.Content.ToString();
                    switch (Season)
                    {
                        case "春":
                            text += "春";
                            list.Add(First + "1");
                            list.Add(First + "5");
                            break;
                        case "夏":
                            text += "夏";
                            list.Add(First + "2");
                            list.Add(First + "6");
                            break;
                        case "秋":
                            text += "秋";
                            list.Add(First + "3");
                            list.Add(First + "7");
                            break;
                        case "冬":
                            text += "冬";
                            list.Add(First + "4");
                            list.Add(First + "8");
                            break;
                    }
                });
            }
            Flag = text;
            return list;
        }

        public Microsoft.Practices.Prism.Commands.DelegateCommand CancelCommand
        {
            get { return new DelegateCommand(CloseDialog); }
        }

        public void CloseDialog()
        {
            FormInit.CloseDialog(this);
        }

        public string Title
        {
            get { return "年份季节"; }
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
    }
}
