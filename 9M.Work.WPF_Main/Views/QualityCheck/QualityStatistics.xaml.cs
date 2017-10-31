using _9M.Work.DbObject;
using _9M.Work.ErpApi;
using _9M.Work.Model;
using _9M.Work.Utility;
using _9M.Work.WPF_Common.WpfBind;
using _9M.Work.WPF_Main.FrameWork;
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

namespace _9M.Work.WPF_Main.Views.QualityCheck
{
    /// <summary>
    /// QualityStatistics.xaml 的交互逻辑
    /// </summary>
    public partial class QualityStatistics : UserControl
    {
        private BaseDAL dal = new BaseDAL();
        public QualityStatistics()
        {
            InitializeComponent();

            List<OrderModelField> orderField = new List<OrderModelField>();
            orderField.Add(new OrderModelField() { PropertyName = "Id", IsDesc = true });
            List<QualityBatchModel> ilist = dal.GetAll<QualityBatchModel>(orderField.ToArray());
            ComboBoxBind.BindComboBox(com_batch, ilist, "BatchName", "Id");
            if (ilist.Count > 0)
            {
                com_batch.SelectedIndex = 0;
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            DateTime now = DateTime.Now;
            DateTime d1 = new DateTime(now.Year, now.Month, 1);
            DateTime d2 = d1.AddMonths(-1);
            start.Text = d2.ToShortDateString();
            end.Text = d1.ToShortDateString();
        }


        delegate void CompareStockHandle(int id);

        DataTable exportTable = null;
        private void Btn_CommandClick(object sender, RoutedEventArgs e)
        {
            int Tag = Convert.ToInt32((sender as Button).Tag);
            string timestart = start.Text;
            string timeend = end.Text;
            switch (Tag)
            {
                case 0: //员工绩效

                    stockslist.Visibility = System.Windows.Visibility.Collapsed;
                    string sql = string.Format(@" select b.username as Item,SUM(finishcount) as [count] from T_Performance a 
                    join T_userinfo b on a.userId = b.id 
                    where dates>='{0}' and dates< '{1}' and a.Performance = {2}
                    group by b.username order by SUM(finishcount) desc", timestart, timeend, com_jixiaotype.SelectedIndex + 1);
                    List<StatisticsModel> modelist = dal.QueryList<StatisticsModel>(sql, new object[] { });
                    CraeteChartTable(modelist, "员工绩效", false);
                    break;
                case 1:
                    Export();
                    break;
                case 2:

                    stockslist.Visibility = System.Windows.Visibility.Visible;
                    bar.LoadBar(true);
                    bar.Loading(true);
                    CompareStockHandle handle = new CompareStockHandle(QueryCompareStock);
                    int id = (com_batch.SelectedItem as QualityBatchModel).Id;
                    handle.BeginInvoke(id, null, null);

                    break;
                case 3:
                    Export();
                    break;
            }
        }


        public void QueryCompareStock(int id)
        {

            string waresql = string.Empty;
            waresql = string.Format(@"select a.WareNo as Item,SUM(stock) as [count],b.InSideGroupId,b.Price,b.OriginalFyiCode,b.HouDu from T_WareSpecList a join dbo.T_WareList b on a.WareNo = b.WareNo
                                            where b.Batchid = {0} group by a.WareNo,b.InSideGroupId,b.Price,b.OriginalFyiCode,b.HouDu order by SUM(stock) desc", id);
            List<StatisticsModel> warelist = dal.QueryList<StatisticsModel>(waresql, new object[] { });


            List<string> skuValueIds = warelist.Select(a => a.Item).ToList();
            WdgjSource wdgj = new WdgjSource();
            SpecListRequest request = new SpecListRequest();
            request.GoodsNoList = skuValueIds;
            List<Model.WdgjWebService.V_GoodsSpecModel> list = wdgj.QueryPurchase(request);

            IEnumerable<IGrouping<string, Model.WdgjWebService.V_GoodsSpecModel>> query = list.GroupBy(x => x.GoodsNO);
            foreach (IGrouping<string, Model.WdgjWebService.V_GoodsSpecModel> info in query)
            {
                List<Model.WdgjWebService.V_GoodsSpecModel> models = info.ToList<Model.WdgjWebService.V_GoodsSpecModel>();
                StatisticsModel model = warelist.Find(a => a.Item == info.Key.ToString());

                model.GJCount = models.Sum(a => a.Stock);
                model.DiffVal = model.Count - model.GJCount;
            }

            exportTable = ConvertType.ToDataTable<StatisticsModel>(warelist);

            bar.Loading(false);
            bar.LoadBar(false);

            System.Windows.Application.Current.Dispatcher.InvokeAsync(new Action(() =>
            {
                stockslist.ItemsSource = warelist;
            }));



        }


        public void Export()
        {
            if (exportTable != null)
            {
                SaveFileDialog sf = new SaveFileDialog();
                sf.Filter = "XLS|*.xls|XLSX|*.xlsx";
                sf.FileName = DateTime.Now.ToShortDateString() + "统计";
                if (sf.ShowDialog() == true)
                {
                    ExcelNpoi.TableToExcel(exportTable, sf.FileName);
                }
                else
                {
                    CCMessageBox.Show("请先查询");
                }
            }
        }
        public void CraeteChartTable(List<StatisticsModel> list, string Title, bool ShowTop20)
        {
            TuBiao.Child = null;
            //保存TABLE
            exportTable = new DataTable();
            exportTable.Columns.Add("Item", typeof(string));
            exportTable.Columns.Add("Count", typeof(int));
            List<WareModel> wlist = null;
            if (com_waretype.SelectedIndex == 1)
            {
                wlist = dal.QueryList<WareModel>(@"select * from T_WareList where batchid=" + (com_batch.SelectedItem as QualityBatchModel).Id, new object[] { });
                exportTable.Columns.Add("货位", typeof(int));
                exportTable.Columns.Add("价格", typeof(decimal));
                exportTable.Columns.Add("吊牌号", typeof(string));
                exportTable.Columns.Add("厚薄", typeof(string));
            }

            list.ForEach(x =>
            {

                DataRow dr = exportTable.NewRow();
                dr["Item"] = x.Item;
                dr["Count"] = x.Count;
                if (com_waretype.SelectedIndex == 1)
                {
                    WareModel wm = wlist.Where(y => y.WareNo.Equals(x.Item, StringComparison.CurrentCultureIgnoreCase)).Single();
                    if (wm != null)
                    {
                        dr["货位"] = wm.InSideGroupId;
                        dr["价格"] = wm.Price;
                        dr["吊牌号"] = wm.OriginalFyiCode;
                        dr["厚薄"] = wm.HouDu;
                    }
                }
                exportTable.Rows.Add(dr);
            });
            if (ShowTop20 == true)
            {
                list = list.Skip(0).Take(20).ToList();
            }
            List<string> xlist = list.Select(x => x.Item).ToList();
            List<string> ylist = list.Select(x => x.Count.ToString()).ToList();
            //创建图像
            ChartHelper.CreateChartColumn(TuBiao, Title, this.Width, 700, "件", xlist, ylist);
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton rad = sender as RadioButton;
            if (rad.Content != null)
            {
                bool IsTrue = rad.Content.Equals("员工绩校");
                if (IsTrue)
                {
                    group1.Visibility = System.Windows.Visibility.Visible;
                    group2.Visibility = System.Windows.Visibility.Collapsed;
                    stockslist.Visibility = System.Windows.Visibility.Collapsed;
                }
                else
                {
                    group1.Visibility = System.Windows.Visibility.Collapsed;
                    group2.Visibility = System.Windows.Visibility.Visible;
                }
            }

        }
    }
}
