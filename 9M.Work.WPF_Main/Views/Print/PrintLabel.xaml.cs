using _9M.Work.DbObject;
using _9M.Work.ErpApi;
using _9M.Work.Model;
using _9M.Work.Model.Report;
using _9M.Work.Utility;
using _9M.Work.WPF_Common;
using _9M.Work.WPF_Main.FrameWork;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Printing;
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

namespace _9M.Work.WPF_Main.Views.Print
{
    /// <summary>
    /// PrintLabel.xaml 的交互逻辑
    /// </summary>
    public partial class PrintLabel : UserControl
    {
        private PrintHelper ph = new PrintHelper();
        private GoodsManager manager = new GoodsManager();
        private GoodsModel gm = null;
        private List<PrintGoodsModel> plist = new List<PrintGoodsModel>();

        public PrintLabel()
        {
            InitializeComponent();
        }


        #region 公共方法

        public GoodsModel GetData(string GoodsDetail)
        {
            return manager.GetGoodsAll(GoodsDetail);
        }

        /// <summary>
        /// 绑定规格
        /// </summary>
        /// <param name="GoodsNo"></param>
        public void BindSpec(string GoodsNo)
        {
            List<SpecModel> list = manager.SpecList(GoodsNo);
            com_spec.ItemsSource = list.Select(x => x.SpecName + "     " + x.SpecCode).ToList();
        }




        #endregion

        #region 事件处理
        //确定 打印
        private void Button_Print(object sender, RoutedEventArgs e)
        {
            int Index = com_fyitype.SelectedIndex;
            string GoodsNo = GoodsHelper.TrueGoodsNo(tb_GoodsNo.Text.Trim(), false);
            if (GoodsNo.Length < 8)
            {
                CCMessageBox.Show("这不是一个正确的款号");
                return;
            }
            if (GoodsHelper.IsGoodId(GoodsNo)) //如果是款号那么梆定下拉眶
            {
                if (Index == 3) //水洗标就直接打印
                {
                    gm = new GoodsModel() { GoodsNo = tb_GoodsNo.Text.Trim().ToUpper() };
                    PrintAny(Index, GoodsNo);
                }
                else
                {
                    BindSpec(GoodsNo);
                }
            }
            else //直接打印
            {

                PrintAny(Index, GoodsNo);
            }
        }

        public void PrintAny(int Index, string GoodsNo)
        {
            int count = Convert.ToInt32(tb_count.Text);
            List<string> list = new List<string>();
            for (int i = 0; i < count; i++)
            {
                list.Add(GoodsNo);
            }
            if (list.Count == 0)
            {
                return;
            }

            switch (Index)
            {
                case 0:
                    ph.PrintTags(list, com_brand.Text, tb_cusprice.Text, Convert.ToBoolean(chk_entityType.IsChecked));
                    break;
                case 1:
                    string Entity = string.Empty;
                    string Real = string.Empty;
                    GoodsHelper.CheckStandardKuan(list[0], out Real, out Entity);
                    //ph.PrintEntityGoods(list);
                    string G = string.Empty;
                    string S = string.Empty;
                    GoodsHelper.SplitGoodsDetail(Real, out G, out S);
                    list.Clear();
                    for (int i = 0; i < count; i++)
                    {
                        list = new List<string>();
                        list.Add(Entity);
                    }
                    ph.PrintEntityGoods(list);
                    GoodsModel ms = manager.GoodsInformation(G);
                    if (ms != null)
                    {
                        tb_fyiprice.Text = ms.Price1.ToString();
                    }

                    break;
                case 2:
                    ph.PrintLabel(list);
                    break;
                case 3:
                    DataTable dt = SqlDalHelp.getWaterTagInfo(list);

                    if (count == 1)
                    {
                        ph.PrintWaterTag(dt);
                    }
                    else
                    {
                        for (int i = 0; i < count; i++)
                        {
                            ph.PrintWaterTag(dt);
                        }
                    }

                    break;
                case 4:
                    ph.PrintGoods(list);
                    break;
                case 5:
                    string en = string.Empty;
                    string re = string.Empty;
                    GoodsHelper.CheckStandardKuan(list[0], out re, out en);
                    ph.PrintEntityFyi(en, Convert.ToBoolean(chk_printgoodsname.IsChecked));
                    break;
                case 6:
                    ph.PrintShoesLabel(list);
                    break;

            }


            tb_GoodsNo.Clear();
            tb_GoodsNo.Focus();
            com_spec.ItemsSource = null;
        }



        //款号眶回车事件
        private void tb_GoodsNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.Button_Print(new object(), new RoutedEventArgs());
            }
        }
        //选择规格时
        private void com_spec_DropDownClosed(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(tb_GoodsNo.Text))
            {
                string Txt = com_spec.Text;
                List<string> codelist = Txt.Split(' ').ToList();
                tb_GoodsNo.Text = GoodsHelper.GoodsNoOrSpecCode(tb_GoodsNo.Text, true) + codelist[codelist.Count - 1];
                tb_GoodsNo.Focus();
                tb_GoodsNo.Select(tb_GoodsNo.Text.Length, 0);
            }
        }

        #endregion

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            //过滤器
            op.Filter = "XLS|*.xls|XLSX|*.xlsx|TXT|*.txt";
            if (op.ShowDialog() == true) //选择完成之后
            {
                DataTable dt = ExcelNpoi.ExcelToDataTable("sheet1", true, op.FileName);
                foreach (DataRow dr in dt.Rows)
                {
                    string P, F;
                    GoodsHelper.SplitPostion(dr["货位"].ToString(), out P, out F);
                    PrintGoodsModel model = new PrintGoodsModel()
                    {
                        BarCode = dr["款号"].ToString() + dr["规格码"].ToString(),
                        PrintCount = Convert.ToInt32(dr["数量"]),
                        Color = dr["颜色"].ToString(),
                        Size = dr["尺码"].ToString(),
                        GoodsNo = dr["款号"].ToString(),
                        SpecCode = dr["规格码"].ToString(),
                        Postion = P
                    };
                    plist.Add(model);
                }
                CCMessageBox.Show(plist.Count > 0 ? "导入数量" + plist.Sum(x => x.PrintCount) : "导入失败");
            }
        }

        private void Button_Click1(object sender, RoutedEventArgs e)
        {
            DataTable sourcedt = new DataTable();
            sourcedt.Columns.Add("款号");
            sourcedt.Columns.Add("规格码");
            sourcedt.Columns.Add("操作员");
            sourcedt.Columns.Add("颜色");
            sourcedt.Columns.Add("尺码");
            sourcedt.Columns.Add("货位");
            sourcedt.Columns.Add("条码");
            sourcedt.Columns.Add("仓库货位");

            List<string> slist = new List<string>();
            plist.ForEach(x =>
            {
                for (int i = 0; i < x.PrintCount; i++)
                {
                    DataRow dr = sourcedt.NewRow();
                    dr["款号"] = x.GoodsNo;
                    dr["规格码"] = x.SpecCode;
                    dr["操作员"] = string.Empty;
                    dr["颜色"] = x.Color;
                    dr["尺码"] = x.Size;
                    dr["仓库货位"] = x.Postion;
                    dr["条码"] = x.BarCode;

                    List<PrintLabeModel> plm = SqlDalHelp.getWareFlagCode("", dr["款号"].ToString(), dr["颜色"].ToString(), dr["尺码"].ToString());
                    if (plm != null)
                    {
                        if (plm.Count > 0)
                        {
                            dr["货位"] = plm[0].FlagCode;
                        }
                    }
                    sourcedt.Rows.Add(dr);
                }
            });
            ph.PrintLabel(sourcedt);
        }
    }

    public class PrintGoodsModel
    {
        public string GoodsNo { get; set; }
        public string SpecCode { get; set; }
        public string BarCode { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public int PrintCount { get; set; }
        public string Postion { get; set; }

        public string EShopPrice { get; set; }


    }
}
