using _9M.Model;
using _9M.Work.DbObject;
using _9M.Work.ErpApi;
using _9M.Work.Model;
using _9M.Work.Model.Report;
using _9M.Work.Utility;
using _9M.Work.WPF_Common;
using _9M.Work.WPF_Main.Views.Print;
using DevExpress.XtraReports.UI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _9M.Work.WPF_Main.FrameWork
{
    public class PrintHelper
    {
        private GoodsManager manager = new GoodsManager();
        private XtraReport rpt;
        private BaseDAL wpdal = new BaseDAL(BaseDAL.WorkPlatConntiong);
        private BaseDAL o2odal = new BaseDAL(BaseDAL.O2OConntiong);
        private string LabelReprotUrl = AppDomain.CurrentDomain.BaseDirectory + "LabelReport/Label.repx";
        private string TagsReprotUrl = AppDomain.CurrentDomain.BaseDirectory + "LabelReport/Tags.repx";
        private string EntityGoodsReprotUrl = AppDomain.CurrentDomain.BaseDirectory + "LabelReport/EntityGoods.repx";
        private string GoodsReprotUrl = AppDomain.CurrentDomain.BaseDirectory + "LabelReport/Goods.repx";
        private string SampleGoodsReportUrl = AppDomain.CurrentDomain.BaseDirectory + "LabelReport/SampleGoods.repx";
        private string EntityFyiReportUrl = AppDomain.CurrentDomain.BaseDirectory + "LabelReport/EntityFyi.repx";
        private string DMReportUrl = AppDomain.CurrentDomain.BaseDirectory + "LabelReport/Test.repx";
        private string PhotoNoReportUrl = AppDomain.CurrentDomain.BaseDirectory + "LabelReport/PhotoNoTag.repx";
        private string ShoesReprotUrl = AppDomain.CurrentDomain.BaseDirectory + "LabelReport/ShoesTag.repx";
        private string WaterReportUrl = AppDomain.CurrentDomain.BaseDirectory + "LabelReport/WaterTag.repx";
        private string ZBMSReportUrl = AppDomain.CurrentDomain.BaseDirectory + "LabelReport/ZBMSLabel.repx";

        public PrintHelper()
        {

        }

        public void PrintWareLabel(List<PrintLabeModel> list)
        {
            rpt = XtraReport.FromFile(LabelReprotUrl, true);
            DataTable sourcedt = rpt.DataSource as DataTable;
            if (sourcedt.Columns.Count == 0)
            {
                sourcedt.Columns.Add("款号");
                sourcedt.Columns.Add("规格码");
                sourcedt.Columns.Add("操作员");
                sourcedt.Columns.Add("颜色");
                sourcedt.Columns.Add("尺码");
                sourcedt.Columns.Add("货位");
                sourcedt.Columns.Add("条码");
                sourcedt.Columns.Add("仓库货位");
            }
            sourcedt.Clear();
            list.ForEach(x =>
            {
                DataRow dr = sourcedt.NewRow();
                dr["款号"] = x.WareNo;
                dr["规格码"] = x.SpecCode.Replace(x.WareNo, "");
                dr["操作员"] = CommonLogin.CommonUser.Alias;
                dr["颜色"] = x.Color;
                dr["尺码"] = x.Size.Equals("XXXL", StringComparison.CurrentCultureIgnoreCase) ? "3XL" : x.Size;
                dr["货位"] = x.FlagCode;
                dr["条码"] = x.SpecCode.Contains(x.WareNo) ? x.SpecCode : x.WareNo + x.SpecCode;
                dr["仓库货位"] = x.WarehouseFlagCode;
                sourcedt.Rows.Add(dr);
            });
            rpt.CreateDocument();
            rpt.PrintingSystem.ShowMarginsWarning = false;//取消超出边界提示框
            rpt.Print();
        }


        public void PrintLabel(DataTable dt)
        {
            rpt = XtraReport.FromFile(LabelReprotUrl, true);
            rpt.DataSource = dt;
            rpt.CreateDocument();
            rpt.PrintingSystem.ShowMarginsWarning = false;//取消超出边界提示框
            //rpt.ShowPreviewDialog();
            rpt.Print();
        }


        public void PrintDM(DataTable dt)
        {
            rpt = XtraReport.FromFile(DMReportUrl, true);
            rpt.DataSource = dt;
            rpt.CreateDocument();
            rpt.PrintingSystem.ShowMarginsWarning = false;//取消超出边界提示框
            rpt.Print();
        }

        public void PrintPhotoNo(DataTable dt)
        {
            rpt = XtraReport.FromFile(PhotoNoReportUrl, true);
            rpt.DataSource = dt;
            rpt.CreateDocument();
            rpt.PrintingSystem.ShowMarginsWarning = false;//取消超出边界提示框
            rpt.Print();
        }

        public void PrintWaterTag(DataTable dt)
        {
            rpt = XtraReport.FromFile(WaterReportUrl, true);
            rpt.DataSource = dt;
            rpt.CreateDocument();
            rpt.PrintingSystem.ShowMarginsWarning = false;//取消超出边界提示框
            rpt.Print();

        }

        public void PrintZBMSTag(DataTable dt)
        {
            rpt = XtraReport.FromFile(ZBMSReportUrl, true);
            rpt.DataSource = dt;
            rpt.CreateDocument();
            rpt.PrintingSystem.ShowMarginsWarning = false;//取消超出边界提示框
            rpt.Print();
        }


        /// <summary>
        /// 打印标签
        /// </summary>
        /// <param name="list">款号集合</param>
        public void PrintLabel(List<string> list)
        {
            rpt = XtraReport.FromFile(LabelReprotUrl, true);
            DataTable sourcedt = rpt.DataSource as DataTable;
            if (sourcedt.Columns.Count == 0)
            {
                sourcedt.Columns.Add("款号");
                sourcedt.Columns.Add("规格码");
                sourcedt.Columns.Add("操作员");
                sourcedt.Columns.Add("颜色");
                sourcedt.Columns.Add("尺码");
                sourcedt.Columns.Add("货位");
                sourcedt.Columns.Add("条码");
                sourcedt.Columns.Add("仓库货位");
            }
            sourcedt.Clear();

            foreach (string s in list)
            {
                var model = manager.GetGoodsAll(s);
                DataRow dr = sourcedt.NewRow();
                dr["款号"] = model.GoodsNo;
                dr["规格码"] = model.SpecCode;
                dr["操作员"] = CommonLogin.CommonUser.Alias;
                dr["颜色"] = model.SpecName1;
                dr["尺码"] = model.SpecName2.Equals("XXXL", StringComparison.CurrentCultureIgnoreCase) ? "3XL" : model.SpecName2;
                dr["仓库货位"] = model.P_postion;
                dr["条码"] = model.GoodsNo + model.SpecCode;
                dr["货位"] = "";

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

            rpt.CreateDocument();
            rpt.PrintingSystem.ShowMarginsWarning = false;//取消超出边界提示框
            rpt.Print();
            //rpt.ShowPreviewDialog();
        }

        /// <summary>
        /// 打印吊牌
        /// </summary>
        /// <param name="list"></param>
        public void PrintTags(List<string> list, string Brand, string CustomPrice, bool EntityType)
        {
            rpt = XtraReport.FromFile(TagsReprotUrl, true);
            DataTable dts = rpt.DataSource as DataTable;
            if (dts.Columns.Count == 0)
            {
                dts.Columns.Add("品牌");
                dts.Columns.Add("款号");
                dts.Columns.Add("品名");
                dts.Columns.Add("色号");
                dts.Columns.Add("号型");
                dts.Columns.Add("成份类型1");
                dts.Columns.Add("成份类型2");
                dts.Columns.Add("成份类型3");
                dts.Columns.Add("成份类型4");
                dts.Columns.Add("成份类型5");
                dts.Columns.Add("成份1");
                dts.Columns.Add("成份2");
                dts.Columns.Add("成份3");
                dts.Columns.Add("成份4");
                dts.Columns.Add("成份5");
                dts.Columns.Add("执行标准");
                dts.Columns.Add("质量等级");
                dts.Columns.Add("条码");
                dts.Columns.Add("价格");
            }
            dts.Clear();
            list.ForEach(x =>
            {
                string standkuan = string.Empty;
                string entitykuan = string.Empty;
                string standkuan_spec = string.Empty;
                string entitykuan_spec = string.Empty;
                var model = manager.GetGoodsAll(x);
                if (EntityType)
                {
                    GoodsHelper.CheckStandardKuan(model.GoodsNo, out standkuan, out entitykuan);
                    GoodsHelper.CheckStandardKuan(x, out standkuan_spec, out entitykuan_spec);
                }

                DataRow dr = dts.NewRow();
                //是否杂款
                bool IsZhsKuan = GoodsHelper.CheckZhaKuan(model.GoodsNo);
                dr["品牌"] = Brand;
                dr["款号"] = EntityType ? entitykuan : model.GoodsNo;
                dr["品名"] = model.ClassName;
                dr["色号"] = model.SpecName1;
                dr["号型"] = model.SpecName2;
                //面料
                List<GoodsMaterial> mlist = wpdal.QueryList<GoodsMaterial>(string.Format("SELECT goodsno,[Material] from dbo.tGoodsBaseInfo where goodsno='{0}'", model.GoodsNo), new object[] { });
                string Material = mlist.Count > 0 ? mlist[0].Material.Replace("：", ":") : "";
                List<string> malist = Material.Split(' ').Where(z => !string.IsNullOrEmpty(z)).ToList();
                string[] real = new string[10];
                int k = 0;
                int j = 5;
                for (int i = 0; i < malist.Count; i++)
                {
                    string[] arry = malist[i].Split(':');
                    if (arry.Length == 2)
                    {
                        real[k] = arry[0];
                        real[j] = arry[1];
                    }
                    else
                    {
                        real[j] = arry[0];
                    }
                    j++;
                    k++;
                    if (j > 9 || k > 4)
                    {
                        break;
                    }
                }
                dr["成份类型1"] = string.IsNullOrEmpty(real[0]) ? "" : real[0] + "：";
                dr["成份1"] = real[5];
                dr["成份类型2"] = string.IsNullOrEmpty(real[1]) ? "" : real[1] + "：";
                dr["成份2"] = real[6];
                dr["成份类型3"] = string.IsNullOrEmpty(real[2]) ? "" : real[2] + "：";
                dr["成份3"] = real[7];
                dr["成份类型4"] = string.IsNullOrEmpty(real[3]) ? "" : real[3] + "：";
                dr["成份4"] = real[8];
                dr["成份类型5"] = string.IsNullOrEmpty(real[4]) ? "" : real[4] + "：";
                dr["成份5"] = real[9];


                dr["执行标准"] = string.IsNullOrEmpty(model.HaoXing) ? "GB/T 22849-2009" : model.ZhiXing;
                //智能选取分类
                List<string> classlist = new List<string>() { "外套", "大衣", "棉服", "羽绒服", "皮衣", "风衣", "马夹", "毛呢外套" };
                int count = classlist.Where(z => z.Equals(model.ClassName)).Count();
                dr["质量等级"] = count == 0 ? "B类   GB18401-2010" : "C类   GB18401-2010";
                dr["条码"] = EntityType ? entitykuan_spec : x;
                dr["价格"] = !string.IsNullOrEmpty(CustomPrice) ? CustomPrice : model.Price1.ToString("f2");
                dts.Rows.Add(dr);
            });
            rpt.CreateDocument();
            rpt.PrintingSystem.ShowMarginsWarning = false;//取消超出边界提示框
            rpt.Print();
        }

        /// <summary>
        /// 实体店标签
        /// </summary>
        /// <param name="list"></param>
        public void PrintEntityGoods(List<string> list)
        {
            rpt = XtraReport.FromFile(EntityGoodsReprotUrl, true);
            DataTable sourcedt = rpt.DataSource as DataTable;
            if (sourcedt.Columns.Count == 0)
            {
                sourcedt.Columns.Add("款号");
                sourcedt.Columns.Add("价格");
            }
            sourcedt.Clear();
            list.ForEach(x =>
            {
                DataRow dr = sourcedt.NewRow();
                dr["款号"] = x;
                dr["价格"] = o2odal.QueryDataTable(string.Format("select RetailPrice from Db_JunZhe_O2O_V2.dbo.g_goods_list where GoodsNo='{0}'", GoodsHelper.GoodsNoOrSpecCode(x, true))).Rows[0][0];
                sourcedt.Rows.Add(dr);
            });

            rpt.CreateDocument();
            rpt.PrintingSystem.ShowMarginsWarning = false;//取消超出边界提示框
            rpt.Print();
        }

        /// <summary>
        /// 款号标签
        /// </summary>
        /// <param name="list"></param>
        public void PrintGoods(List<string> list)
        {
            rpt = XtraReport.FromFile(GoodsReprotUrl, true);
            DataTable sourcedt = rpt.DataSource as DataTable;
            if (sourcedt.Columns.Count == 0)
            {
                sourcedt.Columns.Add("款号");

            }
            sourcedt.Clear();
            list.ForEach(x =>
            {
                DataRow dr = sourcedt.NewRow();
                dr["款号"] = x;
                sourcedt.Rows.Add(dr);
            });
            rpt.CreateDocument();
            rpt.PrintingSystem.ShowMarginsWarning = false;//取消超出边界提示框
            rpt.Print();
        }

        /// <summary>
        /// 样品标签
        /// </summary>
        /// <param name="list"></param>
        public void PrintSample(string GoodsNO, string Color)
        {
            rpt = XtraReport.FromFile(SampleGoodsReportUrl, true);
            DataTable sourcedt = rpt.DataSource as DataTable;
            if (sourcedt.Columns.Count == 0)
            {
                sourcedt.Columns.Add("款号");
                sourcedt.Columns.Add("颜色");
            }
            sourcedt.Clear();

            DataRow dr = sourcedt.NewRow();
            dr["款号"] = GoodsNO;
            dr["颜色"] = Color;
            sourcedt.Rows.Add(dr);
            rpt.CreateDocument();
            rpt.PrintingSystem.ShowMarginsWarning = false;//取消超出边界提示框
            rpt.Print();
        }

        public void PrintEntityFyi(string GoodsNoAll, bool PrintCategory)
        {
            string Entity = string.Empty;
            string Real = string.Empty;
            GoodsHelper.CheckStandardKuan(GoodsNoAll, out Real, out Entity);
            string sql = string.Format(@"select a.Sku,b.GoodsNo,c.CategoryName,b.RetailPrice from  dbo.g_goods_spec a join dbo.g_goods_list b on a.GId =b.Id
join  dbo.g_goods_category c on  b.CategoryId = c.CategoryId
where Sku ='{0}'", Entity);
            var list = o2odal.QueryList<O2OGoodsModel>(sql, new object[] { });
            if (list.Count > 0)
            {
                var ms = list[0];
                rpt = XtraReport.FromFile(EntityFyiReportUrl, true);
                DataTable sourcedt = rpt.DataSource as DataTable;
                if (sourcedt.Columns.Count == 0)
                {
                    sourcedt.Columns.Add("款号");
                    sourcedt.Columns.Add("价格");
                    sourcedt.Columns.Add("品名");
                    sourcedt.Columns.Add("二维码");
                    sourcedt.Columns.Add("条码");
                }
                sourcedt.Clear();

                DataRow dr = sourcedt.NewRow();
                dr["款号"] = ms.GoodsNo;
                dr["价格"] = ms.RetailPrice;
                dr["品名"] = PrintCategory ? ms.CategoryName : "";
                dr["条码"] = ms.Sku;
                dr["二维码"] = "WWW.9MYP.COM/QRGOODSDETAIL/INDEX/" + ms.Sku;
                sourcedt.Rows.Add(dr);
                rpt.CreateDocument();
                rpt.PrintingSystem.ShowMarginsWarning = false;//取消超出边界提示框
                rpt.Print();
            }
        }


        /// <summary>
        /// 打印标签
        /// </summary>
        /// <param name="list">款号集合</param>
        public void PrintShoesLabel(List<string> list)
        {
            rpt = XtraReport.FromFile(ShoesReprotUrl, true);
            DataTable sourcedt = rpt.DataSource as DataTable;
            if (sourcedt.Columns.Count == 0)
            {
                sourcedt.Columns.Add("款号");
                sourcedt.Columns.Add("规格码");
                sourcedt.Columns.Add("操作员");
                sourcedt.Columns.Add("颜色");
                sourcedt.Columns.Add("尺码");
                sourcedt.Columns.Add("货位");
                sourcedt.Columns.Add("条码");
                sourcedt.Columns.Add("仓库货位");
            }
            sourcedt.Clear();

            foreach (string s in list)
            {
                var model = manager.GetGoodsAll(s);
                DataRow dr = sourcedt.NewRow();
                dr["款号"] = model.GoodsNo;
                dr["规格码"] = model.SpecCode;
                dr["操作员"] = CommonLogin.CommonUser.Alias;
                dr["颜色"] = model.SpecName1;
                dr["尺码"] = model.SpecName2.Equals("XXXL", StringComparison.CurrentCultureIgnoreCase) ? "3XL" : model.SpecName2;
                dr["仓库货位"] = model.P_postion;
                dr["条码"] = model.GoodsNo + model.SpecCode;
                dr["货位"] = "";

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

            rpt.CreateDocument();
            rpt.PrintingSystem.ShowMarginsWarning = false;//取消超出边界提示框
            rpt.Print();
            //rpt.ShowPreviewDialog();
        }
    }

    public class GoodsMaterial
    {
        public string GoodsNo { get; set; }
        public string Material { get; set; }
    }

    public class O2OGoodsModel
    {
        public string Sku { get; set; }
        public string GoodsNo { get; set; }
        public decimal RetailPrice { get; set; }
        public string CategoryName { get; set; }
    }

}
