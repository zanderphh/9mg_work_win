using _9M.Work.DbObject;
using _9M.Work.ErpApi;
using _9M.Work.Model;
using _9M.Work.Model.WdgjWebService;
using _9M.Work.Utility;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Media;
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
using System.Windows.Threading;

namespace _9M.Work.WPF_Main.Views.EveryDayUpdate
{
    /// <summary>
    /// GoodsQuery.xaml 的交互逻辑
    /// </summary>
    public partial class GoodsQuery : UserControl
    {
        private GoodsManager Manager = new GoodsManager();
        private BaseDAL dal = new BaseDAL();
        private SoundPlayer sp = new SoundPlayer();
        private WdgjSource wdgj = new WdgjSource();
        public GoodsQuery()
        {
            InitializeComponent();
            //让文本框得到焦点
            this.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                 (Action)(() => { Keyboard.Focus(tb_GoodsNo); }));
            Keyboard.Focus(tb_GoodsNo);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
           
            string GoodsNo = string.Empty;
            string SpecCode = string.Empty;
            string GoodsNoDetail = tb_GoodsNo.Text.Trim();
            GoodsHelper.SplitGoodsDetail(GoodsNoDetail, out GoodsNo, out SpecCode);
          
            List<StatisticsModel> mo = dal.QueryList<StatisticsModel>(string.Format(@"select WareNo,SUM(stock) as [Count]  from  dbo.T_WareSpecList 
            where wareno = '{0}' group by WareNo", GoodsNo), new object[] { });
            if (mo.Count > 0)
            {
                lb_goodsstock1.Content = mo[0].Count;
            }
            List<V_GoodsSpecModel> list = new List<V_GoodsSpecModel>();
            if (!string.IsNullOrEmpty(GoodsNo))
            {
                //管家列表
                list = wdgj.SpecList(new SpecListRequest() { GoodsNo = GoodsNo});
                if (list==null)
                {
                    return;
                }
                if (list.Count > 0)
                {
                   
                    List<GridModel> mlist = new List<GridModel>();
                    list.ForEach(x =>
                    {
                        mlist.Add(new GridModel() { StockAll =Convert.ToInt32(x.StockALL), SpecCode = x.SpecCode, SpecName = x.SpecName, SpecName1 = x.SpecName1, SpecName2 = x.SpecName2, ALLCode = GoodsNo + x.SpecCode, OrderCount =(int) x.OrderCount, SndCount = (int)x.SndCount,Postion = x.PositionRemark });
                    });
                    lb_goodsno.Content = list[0].GoodsNO;
                    lb_goodsname.Content = list[0].goodsname;
                    lb_goodsstock.Content = list.Sum(x => x.Stock).ToString();
                    lb_InPrice.Content = list[0].Reserved3 != null ? list[0].Reserved3 : "";
                    lab_gjcount.Content = mlist.GroupBy(x => x.SpecName1).Count();
                    GoodsGrid.ItemsSource = mlist;
                }

                //整理列表
                //List<WareSpecModel> splist = dal.GetList<WareSpecModel>(x => x.WareNo.Equals(GoodsNo, StringComparison.CurrentCultureIgnoreCase) && (!x.Size.Equals("XS") && !x.Size.Equals("XXXL")));
                List<WareSpecModel> splist = dal.QueryList<WareSpecModel>(string.Format(@"select * from T_WareSpecList where wareno = '{0}'", GoodsNo), new object[] { });
                List<WareModel> warelist = dal.QueryList<WareModel>(string.Format(@"select * from T_WareList where wareno = '{0}' ", GoodsNo), new object[] { });

                if (splist.Count > 0)
                {
                    List<GridModel> mlist = new List<GridModel>();
                    splist.ForEach(x =>
                    {
                        mlist.Add(new GridModel() { Stock = x.Stock, SpecCode = x.SpecCode.ToString(), SpecName = x.Color + x.Size, SpecName1 = x.Color, SpecName2 = x.Size, ALLCode = GoodsNo + x.SpecCode,Postion = warelist[0].InSideGroupId.ToString() });
                    });
                    lab_incount.Content = mlist.GroupBy(x => x.SpecName1).Count();
                    InGoodsGrid.ItemsSource = mlist;
                }
            }
            if (mo.Count == 0 && list.Count == 0)
            {
                sp.SoundLocation = AppDomain.CurrentDomain.BaseDirectory + @"\Sounds\Error.WAV";
                CCMessageBox.Show("没有商品信息");
                return;
            }
            if (chk_sound.IsChecked == true)
            {
                int stock = Convert.ToInt32(lb_goodsstock.Content);
                int stock1 = Convert.ToInt32(lb_goodsstock1.Content);
                int realstock = stock > 0 ? stock : stock1;
                if (realstock >= Convert.ToInt32(tb_warningcount.Text))
                {
                    sp.SoundLocation = AppDomain.CurrentDomain.BaseDirectory + @"\Sounds\Success.WAV";
                }
                else
                {
                    sp.SoundLocation = AppDomain.CurrentDomain.BaseDirectory + @"\Sounds\Error.WAV";
                }
                sp.Play();
            }
            tb_GoodsNo.Clear();
            tb_GoodsNo.Focus();
        }

        private void CopyClick(object sender, RoutedEventArgs e)
        {
            GridModel model = (GridModel)GoodsGrid.SelectedItem;
            if (model == null)
            {
                CCMessageBox.Show("请选中在复制");
                return;
            }
            Clipboard.SetDataObject(model.ALLCode);
        }

        private void tb_GoodsNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.Button_Click(new object(), new RoutedEventArgs());
            }
        }

        private void Button_Click1(object sender, RoutedEventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            //过滤器
            op.Filter = "XLS|*.xls|XLSX|*.xlsx|TXT|*.txt";
            if (op.ShowDialog() == true) //选择完成之后
            {
                DataTable dt = ExcelNpoi.ExcelToDataTable("sheet1", true, op.FileName);
                List<string> GoodsNoList = new List<string>();
                string Request = string.Empty;
                foreach (DataRow dr in dt.Rows)
                {
                    GoodsNoList.Add(dr["款号"].ToString());
                    Request += dr["款号"].ToString() + ",";
                }
                List<ApiSpecModel> modellist = Manager.GetSpecListByGoodsNos(Request.TrimEnd(','));
                DataTable dts = new DataTable();
                dts.Columns.Add("款号", typeof(string));
                dts.Columns.Add("库存", typeof(int));
                GoodsNoList.ForEach(x =>
                {
                    int Stock = modellist.Where(y => y.GoodsNo.Equals(x, StringComparison.CurrentCultureIgnoreCase)).Sum(z => z.Stock);
                    DataRow dr = dts.NewRow();
                    dr["款号"] = x;
                    dr["库存"] = Stock;
                    dts.Rows.Add(dr);
                });
                //获取当前用户桌面的路径
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                ExcelNpoi.DataTableToExcel(dts, "sheet1", true, desktopPath + "//库存查询" + System.Guid.NewGuid().ToString() + ".xlsx");
                CCMessageBox.Show("名称(库存查询.xlsx)己生成在桌面");
            }
        }

        private void btn_lookLog(object sender, RoutedEventArgs e)
        {
            GridModel selectedrow = InGoodsGrid.SelectedItem as GridModel;
            _9M.Work.WPF_Main.Infrastrcture.FormInit.OpenDialog(this, new logDialog(selectedrow.ALLCode,selectedrow.SpecName1), false);
        }
    }

    public class GridModel
    {
        public string SpecCode { get; set; } //规格码
        public string SpecName { get; set; }//规格名
        public string SpecName1 { get; set; } //颜色
        public string SpecName2 { get; set; } //尺码
        public int Stock { get; set; }
        public string ALLCode { get; set; }
        public int SndCount { get; set; }
        public int OrderCount { get; set; }
        public int StockAll { get; set; }

        public string Postion { get; set; }
    }
}
