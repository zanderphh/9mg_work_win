using _9M.Work.DbObject;
using _9M.Work.ErpApi;
using _9M.Work.Model;
using _9M.Work.Utility;
using _9M.Work.WPF_Common.WpfBind;
using _9M.Work.WPF_Main.Infrastrcture;
using _9M.Work.WPF_Main.Views.Dialog;
using Microsoft.Win32;
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
using Visifire.Charts;

namespace _9M.Work.WPF_Main.Views.Statistics
{
    /// <summary>
    /// SalesStatistics.xaml 的交互逻辑
    /// </summary>
    public partial class SalesStatistics : UserControl
    {
        private BaseDAL dal = new BaseDAL();
        private List<SaleModel> list = null;
        public SalesStatistics()
        {
            InitializeComponent();
            Init();
        }

        //异步委托
        public delegate void AsyncEventHandler(SalePostModel model, string ShopText);

        public List<string> YearSeasonList { get; set; }
        public List<string> GoodsNoList { get; set; }
        public void Init()
        {
            //品牌下拉眶
            List<BrandModel> brandlist = dal.GetAll<BrandModel>();
            brandlist.Insert(0, new BrandModel() { BrandCN = "请选择", BrandEN = "0" });
            ComboBoxBind.BindComboBox(com_brand, brandlist, "BrandCN", "BrandEN");
            com_brand.SelectedIndex = 0;
            //类目下拉眶
            List<CategoryModel> categorylist = dal.GetAll<CategoryModel>();
            categorylist.Insert(0, new CategoryModel() { CategoryName = "请选择" });
            ComboBoxBind.BindComboBox(com_category, categorylist, "CategoryName", "");
            com_category.SelectedIndex = 0;
            //默认时间
            starttime.Text = DateTime.Now.AddDays(-7).ToShortDateString();
            endtime.Text = DateTime.Now.ToShortDateString();
        }
        private void Btn_CommandClick(object sender, RoutedEventArgs e)
        {
            int Tag = Convert.ToInt32((sender as Button).Tag);
            switch (Tag)
            {
                case 0: //搜索
                    //得到查询对象
                    SalePostModel model = new SalePostModel();
                    model.Shop = com_shop.Text.Equals("全部") ? "0" : com_shop.Text;
                    model.Brand = (com_brand.SelectedItem as BrandModel).BrandEN;
                    model.Category = com_category.Text.Equals("请选择") ? "0" : com_category.Text;
                    model.YearSeasonList = YearSeasonList;
                    model.WeekMin = tb_weekmin.Text;
                    model.WeekMax = tb_weekmax.Text;
                    model.PriceMin = tb_pricemin.Text;
                    model.PriceMax = tb_pricemax.Text;
                    model.StockMin = tb_stockmin.Text;
                    model.StockMax = tb_stockmax.Text;
                    model.StartTime = starttime.Text;
                    model.EndTime = endtime.Text;
                    model.KeyWord = tb_keword.Text;
                    model.GoodsList = GoodsNoList;
                    if (string.IsNullOrEmpty(model.StartTime) || string.IsNullOrEmpty(endtime.Text))
                    {
                        CCMessageBox.Show("请填写时间");
                        return;
                    }
                    //清空组件值
                    TuBiao.Child = null;
                    lab_money.Content = "";
                    lab_count.Content = "";
                    //实例委托
                    AsyncEventHandler asy = new AsyncEventHandler(this.InitData);
                    //异步调用开始，没有回调函数和AsyncState,都为null
                    IAsyncResult ia = asy.BeginInvoke(model, com_shop.Text, null, null);
                    break;
                case 1: //导出
                    if (list == null)
                    {
                        CCMessageBox.Show("请查询数据在导出");
                        return;
                    }
                    //转换
                    DataTable dt = ListToDataTable.ConvertToDataTable<SaleModel>(list);
                    dt.Columns[0].ColumnName = "款号";
                    dt.Columns[1].ColumnName = "库存";
                    dt.Columns[2].ColumnName = "原价";
                    dt.Columns[3].ColumnName = "现价";
                    dt.Columns[4].ColumnName = "出库量";
                    dt.Columns[5].ColumnName = "销售额";
                    dt.Columns[6].ColumnName = "采购量";
                    dt.Columns[7].ColumnName = "退货量";
                    dt.Columns[8].ColumnName = "上新日期";
                    dt.Columns[9].ColumnName = "周期";
                    SaveFileDialog sf = new SaveFileDialog();
                    sf.Filter = "XLS|*.xls|XLSX|*.xlsx";
                    sf.FileName = com_shop.Text + "【" + starttime.Text + "--" + endtime.Text + "】";
                    if (sf.ShowDialog() == true)
                    {
                        ExcelNpoi.TableToExcel(dt, sf.FileName);
                        //  ExcelNpoi.DataTableToExcel(dt, "sheet1", true, sf.FileName);
                    }
                    break;
            }
        }

        public void InitData(SalePostModel model, string ShopText)
        {
            //打开加载
            bar.LoadBar(true);
            //打开加载中的字符
            bar.Loading(true);
            //得到Post数据
            list = new StatisticsManager().Statistics(model).OrderByDescending(x => x.SellCount).ToList();
            //得到Top 20 绑定图表
            List<SaleModel> SaleList = list.Skip(0).Take(20).ToList();
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                lab_count.Content = list.Sum(x => x.SellCount);
                lab_money.Content = list.Sum(x => x.SellTotal);
                CreateChartColumn(ShopText + " Top 20", SaleList.Select(x => x.GoodsNo).ToList(), SaleList.Select(x => x.SellCount.ToString()).ToList());
            }));
            //开始读取进度条
            bar.Loading(false);
            bar.LoadBar(false);
        }

        private void Btn_QueryClick(object sender, RoutedEventArgs e)
        {
            int Tag = Convert.ToInt32((sender as Button).Tag);
            switch (Tag)
            {
                case 0:
                    FormInit.OpenDialog(this, new YearDialog(), false);
                    break;
                case 1:
                    FormInit.OpenDialog(this, new GoodsNoDialog(this, 1), false);
                    break;
            }
        }

        public void CreateChartColumn(string name, List<string> valuex, List<string> valuey)
        {
            //创建一个图标
            Chart chart = new Chart();

            //设置图标的宽度和高度
            chart.Width = this.Width;
            chart.Height = 680;
            //  chart.Margin = new Thickness(100, 5, 10, 5);
            //是否启用打印和保持图片
            chart.ToolBarEnabled = false;

            //设置图标的属性
            chart.ScrollingEnabled = false;//是否启用或禁用滚动
            chart.View3D = true;//3D效果显示

            //创建一个标题的对象
            Title title = new Title();
            title.FontWeight = FontWeights.Bold;
            title.FontSize = 40;
            //设置标题的名称
            title.Text = name;
            title.Padding = new Thickness(0, 10, 5, 0);

            //向图标添加标题
            chart.Titles.Add(title);

            Axis yAxis = new Axis();
            //设置图标中Y轴的最小值永远为0           
            yAxis.AxisMinimum = 0;
            //设置图表中Y轴的后缀          
            yAxis.Suffix = "件";
            chart.AxesY.Add(yAxis);

            // 创建一个新的数据线。               
            DataSeries dataSeries = new DataSeries();

            // 设置数据线的格式
            dataSeries.RenderAs = RenderAs.StackedColumn;//柱状Stacked


            // 设置数据点              
            DataPoint dataPoint;
            for (int i = 0; i < valuex.Count; i++)
            {
                // 创建一个数据点的实例。                   
                dataPoint = new DataPoint();
                // 设置X轴点                    
                dataPoint.AxisXLabel = valuex[i];
                //设置Y轴点                   
                dataPoint.YValue = double.Parse(valuey[i]);
                //添加一个点击事件        
                //  dataPoint.MouseLeftButtonDown += new MouseButtonEventHandler(dataPoint_MouseLeftButtonDown);
                //添加数据点                   
                dataSeries.DataPoints.Add(dataPoint);
            }

            // 添加数据线到数据序列。                
            chart.Series.Add(dataSeries);

            //将生产的图表增加到Grid，然后通过Grid添加到上层Grid.           
            Grid gr = new Grid();
            gr.Children.Add(chart);
            TuBiao.Child = gr;
        }
    }
}
