using _9M.Work.Model;
using _9M.Work.TopApi;
using _9M.Work.Utility;
using _9M.Work.WPF_Common;
using _9M.Work.WPF_Common.Controls.Print;
using _9M.Work.WPF_Main.ControlTemplate.PrintTemplate;
using _9M.Work.WPF_Main.FrameWork;
using _9M.Work.WPF_Main.Views.EveryDayUpdate;
using _9Mg.Work.TopApi;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using Top.Api.Domain;

namespace _9M.Work.WPF_Main
{
    /// <summary>
    /// TestWindwos.xaml 的交互逻辑
    /// </summary>
    public partial class TestWindwos : Window
    {
        BackgroundWorker _backTask = new BackgroundWorker();
        public TestWindwos()
        {
            InitializeComponent();


        }





        TopSource top = new TopSource();
        public delegate void AsyncEventHandler();
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            List<string> goodslist = new List<string>();
            string File = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\goods.txt";
            using (System.IO.StreamReader sr = new System.IO.StreamReader(File))
            {
                string str;
                while ((str = sr.ReadLine()) != null)
                {
                    goodslist.Add(str);
                }
            }
            List<Item> itemlist = new List<Item>();
            int Page = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(goodslist.Count) / 40));
            for (int i = 0; i < Page; i++)
            {
                List<string> list = goodslist.Skip(i * 40).Take(40).ToList();
                List<Item> itlist = top.GetItemList(list, "item_img.url,prop_img.url, pic_url");
                itemlist.AddRange(itlist);
            }
            Panel.Children.Clear();
            itemlist.ForEach(x =>
            {
                WrapPanel p = new WrapPanel() { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 10, 0, 0) };
                //x.ItemImgs.ForEach(y =>
                //{
                //    p.Children.Add(new Image() { Width = 300, Height = 300, Source = new BitmapImage(new Uri(y.Url+"_300x300", UriKind.RelativeOrAbsolute)) });
                //});
                p.Children.Add(new Image() { Width = 300, Height = 300, Source = new BitmapImage(new Uri(x.PicUrl + "_300x300", UriKind.RelativeOrAbsolute)) });
                p.Children.Add(new CheckBox() { Content = "选择", Tag = x.OuterId });
                Panel.Children.Add(p);
            });
            Button btn = new Button() { Content = "提交" };
            btn.Click += Btn_Click1;
            Panel.Children.Add(btn);
        }

        private void Btn_Click1(object sender, RoutedEventArgs e)
        {
            List<string> outerid = WPFControlsSearchHelper.GetChildObjects<CheckBox>(Panel, string.Empty).Where(x => x.IsChecked == true).Select(x => x.Tag.ToString()).ToList();
            DataTable dt = new DataTable();
            dt.Columns.Add("款号");
            outerid.ForEach(x=> {
                DataRow dr = dt.NewRow();
                dr["款号"] = x;
                dt.Rows.Add(dr);
            });
            ExcelNpoi.TableToExcelForXLS(dt, Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\橱窗.xls");
            MessageBox.Show("己经生成在桌面");
        }
    }
}
