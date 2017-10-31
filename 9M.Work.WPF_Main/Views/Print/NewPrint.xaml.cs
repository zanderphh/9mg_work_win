using _9M.Work.DbObject;
using _9M.Work.ErpApi;
using _9M.Work.Model;
using _9M.Work.Utility;
using _9M.Work.WPF_Common;
using DevExpress.XtraReports.UI;
using DevExpress.Xpf.Reports.UserDesigner;
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

namespace _9M.Work.WPF_Main.Views.Print
{
    /// <summary>
    /// NewPrint.xaml 的交互逻辑
    /// </summary>
    public partial class NewPrint : UserControl
    {
        private BaseDAL dal = new BaseDAL();
        private XtraReport rpt;
        public NewPrint()
        {
            InitializeComponent();
            Init();
        }
        public void Init()
        {
            //var list = dal.GetAll<ReportInfoModel>();
            //com_printtype.ItemsSource = list;
            //if (list.Count > 0)
            //{
            //    com_printtype.SelectedIndex = 0;
            //    //AppDomain.CurrentDomain.BaseDirectory
            //}
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {

            //string ReportUrl = AppDomain.CurrentDomain.BaseDirectory + @"\LabelReport\Goods.repx";
            //string ReportUrl = AppDomain.CurrentDomain.BaseDirectory + @"\LabelReport\ZBMSLabel.repx"; //标签

            string ReportUrl = AppDomain.CurrentDomain.BaseDirectory + "LabelReport/WaterTag.repx";
            rpt = XtraReport.FromFile(ReportUrl, true);

            rpt.DataSource = waterTag();
            ReportDesigner rd = new ReportDesigner();
            rd.DocumentSource = rpt;
            rd.ShowWindow(Application.Current.MainWindow);
            rpt.SaveLayout(ReportUrl);
        }


        public DataTable GoodsTag()
        {
             DataTable sourcedt = new DataTable();
                       sourcedt.Columns.Add("款号");
               DataRow dr = sourcedt.NewRow();
               dr["款号"] = "12AB3456";
               sourcedt.Rows.Add(dr);
               return sourcedt;
        }


        public DataTable ZBMSTag()
        {
            DataTable sourcedt = new DataTable();
            sourcedt.Columns.Add("规格名称");
            sourcedt.Columns.Add("编号");

            DataRow dr = sourcedt.NewRow();
            dr["规格名称"] = "999号";
            dr["编号"] = "ZBMS93";

            sourcedt.Rows.Add(dr);
            return sourcedt;
        }


        //洗水标
        public DataTable waterTag()
        {
            DataTable sourcedt = new DataTable();
            sourcedt.Columns.Add("款号");
            sourcedt.Columns.Add("面料");
            sourcedt.Columns.Add("图1");
            sourcedt.Columns.Add("图2");
            sourcedt.Columns.Add("图3");
            sourcedt.Columns.Add("图4");

            DataRow dr = sourcedt.NewRow();
            dr["款号"] = "71GX0810";
            string ml = "面料：46.5%羊毛 43.4%聚酯纤维 10.1%锦纶 连接线除外";
            dr["面料"] = ml.Replace(" ",Environment.NewLine);
            dr["图1"] = "http://work.9mg.cn/d/wt/w_02.png";
            dr["图2"] = "http://work.9mg.cn/d/wt/w_03.png";
            dr["图3"] = "http://work.9mg.cn/d/wt/w_04.png";
            dr["图4"] = "http://work.9mg.cn/d/wt/w_06.png";

            sourcedt.Rows.Add(dr);
            return sourcedt;
        }

        //标签
        public DataTable testLabel()
        {
            DataTable sourcedt = new DataTable();
            sourcedt.Columns.Add("款号");
            sourcedt.Columns.Add("规格码");
            sourcedt.Columns.Add("操作员");
            sourcedt.Columns.Add("颜色");
            sourcedt.Columns.Add("尺码");
            sourcedt.Columns.Add("货位");
            sourcedt.Columns.Add("仓库货位");
            sourcedt.Columns.Add("条码");

            DataRow dr = sourcedt.NewRow();
            dr["款号"] = "71GX0810";
            dr["规格码"] = "112";
            dr["操作员"] = "03";
            dr["颜色"] = "黑色";
            dr["尺码"] = "S";
            dr["货位"] = "199";
            dr["条码"] = "71GX0810";
            dr["仓库货位"] = "1ZA08-2-3";
            sourcedt.Rows.Add(dr);

            return sourcedt;

        }

        //DM暗号测试
        public DataTable testDT()
        {
            DataTable sourcedt = new DataTable();
            sourcedt.Columns.Add("暗码");
            sourcedt.Columns.Add("金额");

            DataRow dr = sourcedt.NewRow();
            dr["暗码"] = "ABCDE";
            dr["金额"] = "3";
            sourcedt.Rows.Add(dr);

            DataRow dr1 = sourcedt.NewRow();
            dr1["暗码"] = "AAAAE";
            dr1["金额"] = "3";
            sourcedt.Rows.Add(dr1);

            return sourcedt;
        }


        public DataTable testEshopLable()
        {
            DataTable sourcedt = new DataTable();
            sourcedt.Columns.Add("款号");
            sourcedt.Columns.Add("价格");
            DataRow dr = sourcedt.NewRow();
            dr["款号"] = "71GX0810";
            dr["价格"] = "112";
            sourcedt.Rows.Add(dr);
            return sourcedt;
        }


        public DataTable testPhotoNOLab()
        {
            DataTable sourcedt = new DataTable();
            sourcedt.Columns.Add("拍照单号");
            DataRow dr = sourcedt.NewRow();
            dr["拍照单号"] = "170721001";
            return sourcedt;
        }
    }
}
