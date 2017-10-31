using _9M.Work.DbObject;
using _9M.Work.ErpApi;
using _9M.Work.Model;
using _9M.Work.Utility;
using _9M.Work.WPF_Main.FrameWork;
using System;
using System.Collections.Generic;
using System.Data;
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

namespace _9M.Work.WPF_Main.Views.Live
{
    /// <summary>
    /// Interaction logic for LiveSale.xaml
    /// </summary>
    public partial class LiveSale : UserControl
    {

        BaseDAL dal = new BaseDAL();
        delegate void AsyncSearchHandle(string d);

        public LiveSale()
        {
            InitializeComponent();

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {


            string d = start.Text.Trim();

            if (d == "")
            {
                MessageBox.Show("未选择时间", "提示");
                return;
            }

            bar.LoadBar(true);
            bar.Loading(true);


            AsyncSearchHandle handle = new AsyncSearchHandle(GetData);
            handle.BeginInvoke(d, null, null);
        }


        public void GetData(string d)
        {

            List<ZBMSModel> list = WdgjSource.GetZBMSSpecName(d);
            bar.Loading(false);
            bar.LoadBar(false);
            System.Windows.Application.Current.Dispatcher.InvokeAsync(new Action(() =>
            {

                DetailListDG.ItemsSource = list;

            }));


        }

        private void ButtonExport_Click(object sender, RoutedEventArgs e)
        {

            DataTable dt = new DataTable();
            dt.Columns.Add("规格名称", typeof(string));
            dt.Columns.Add("编号");

            List<ZBMSModel> list = DetailListDG.ItemsSource as List<ZBMSModel>;

            if (list == null)
            {
                MessageBox.Show("数据为空，无法导出");
                return;
            }
            else if (list.Count < 1)
            {
                MessageBox.Show("数据为空，无法导出");
                return;
            }

            foreach (ZBMSModel m in list)
            {
                DataRow dr = dt.NewRow();
                dr["规格名称"] = m.SpecName;
                dr["编号"] = "ZBMS" + m.SpecName.Replace("号", "").Trim();
                dt.Rows.Add(dr);
            }

            string tag = ((Button)sender).Tag.ToString();

            if (tag == "export")
            {
                string res = string.Empty;
                try
                {
                    ExcelNpoi.TableToExcelForXLS(dt, Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + @"/直播秒杀" + System.Guid.NewGuid() + ".xls");
                }
                catch (Exception ex)
                {
                    res = ex.Message;
                }
                CCMessageBox.Show(string.IsNullOrEmpty(res) ? "表格生成在桌面" : "生成错误\r\n" + res);
            }
            else if(tag=="print")
            {
                PrintHelper print = new PrintHelper();
                print.PrintZBMSTag(dt);
            }
        }

        private void btn_SinglePrint(object sender, RoutedEventArgs e)
        {
            ZBMSModel item = DetailListDG.CurrentItem as ZBMSModel;
              if (item != null)
              {
                  DataTable dt = new DataTable();
                  dt.Columns.Add("规格名称", typeof(string));
                  dt.Columns.Add("编号");

                  DataRow dr = dt.NewRow();
                  dr["规格名称"] = item.SpecName;
                  dr["编号"] = "ZBMS" + item.SpecName.Replace("号", "").Trim();
                  dt.Rows.Add(dr);
                  PrintHelper print = new PrintHelper();
                  print.PrintZBMSTag(dt);
              }
        }

    }


}
