using _9M.Work.DbObject;
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

namespace _9M.Work.WPF_Main.Views.Refund
{
    /// <summary>
    /// Interaction logic for refundStatistics.xaml
    /// </summary>
    public partial class refundStatistics : UserControl
    {
        public refundStatistics()
        {
            InitializeComponent(); start.Text = DateTime.Now.ToString("yyyy-MM-01");
            end.Text = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-01")).AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd");
        }


        public string StatisticsSQL(string startDate, string endDate)
        {
            return string.Format(@" 
               
                with t1 as
                (
	                select shopId,a.refundReason,CONVERT(decimal(18,2),COUNT(*)) total   from T_RefundDetail as a join
	                (
	                  select shopId,refundNo from T_Refund where regTime>='{0} 00:00:00' and regTime<='{1} 23:59:59'
	  
	                ) as b on a.refundNo=b.refundNo group by shopId,a.refundReason 
                ),
                t3 as
                (
                select shopId as '店铺',
                max(case refundReason when 1 then total else 0 end) as '卖家原因',
                max(case refundReason when 0 then total else 0 end) as '买家原因'
                from t1 group by shopId
                ),
                t4 as
                (
                select *,
                Convert(varchar(10),cast(round(t3.买家原因/(t3.买家原因+t3.卖家原因),2) as numeric(30,2)) * 100)+ '%' as '买家原因占比' ,
                Convert(varchar(10),cast(round(t3.卖家原因/(t3.买家原因+t3.卖家原因),2) as numeric(30,2)) * 100)+ '%' as '卖家原因占比' 
                from t3
                ) 
                select shopName as[店铺], convert(int,卖家原因) as '卖家原因' ,convert(int,买家原因) as '买家原因',买家原因占比,卖家原因占比 from t4 join T_ShopConfig on t4.店铺=T_ShopConfig.shopId
                union all
                select '合计',convert(int,SUM(卖家原因)),convert(int,SUM(买家原因)),
                Convert(varchar(10),cast(round(sum(买家原因)/(sum(买家原因)+sum(卖家原因)),2) as numeric(30,2)) * 100)+ '%' as '买家原因占比' ,
                Convert(varchar(10),cast(round(sum(卖家原因)/(sum(买家原因)+sum(卖家原因)),2) as numeric(30,2)) * 100)+ '%' as '卖家原因占比' 

                from t4", startDate, endDate);
        }


        public string ErpSaleSQL(string month, List<string> brand)
        {

            string startTime = DateTime.Now.Year + "-" + month + "-" + "01";
            string endTime = Convert.ToDateTime(startTime).AddMonths(1).ToString("yyyy-MM-dd");

            string sqlwhere = "";
            if (brand != null)
            {
                for (int i = 0; i < brand.Count; i++)
                {
                    if (i == brand.Count - 1)
                    {
                        sqlwhere += string.Format(" b.goodsno like '%{0}%'", brand[i]);
                    }
                    else
                    {
                        sqlwhere += string.Format(" b.goodsno like '%{0}%' or", brand[i]);
                    }
                }

                if (sqlwhere != "")
                {
                    sqlwhere = "and (" + sqlwhere + ")";
                }



            }
            string sql = string.Format(@"
                        select c.ShopName,a.ShopId,SndTime,isnull(sum(SellCount),0) SellCount  from (
                        select TradeID, ShopId,Month(SndTime) as SndTime from g_trade_tradelist 
                        where  sndTime>'{0} 00:00:00' and sndTime<'{1} 00:00:00' and TradeStatus=11
                        ) as a left join(select  TradeID,sum(SellCount) SellCount from g_trade_goodslist as a join g_goods_goodslist as b on a.GoodsID=b.GoodsID where 1=1 {2} group by TradeID) as b on a.TradeID=b.TradeID join G_Cfg_ShopList as c on a.ShopID=c.ShopID
                        group by ShopName,SndTime,a.ShopId
            ", startTime, endTime, sqlwhere);
            return sql;
        }

        public string RefundSql(string month, List<string> brand)
        {

            string startTime = DateTime.Now.Year + "-" + month + "-" + "01";
            string endTime = Convert.ToDateTime(startTime).AddMonths(1).ToString("yyyy-MM-dd");

            string sqlwhere = "";
            if (brand != null)
            {
                for (int i = 0; i < brand.Count; i++)
                {
                    if (i == brand.Count - 1)
                    {
                        sqlwhere += string.Format(" goodsno like '%{0}%'", brand[i]);
                    }
                    else
                    {
                        sqlwhere += string.Format(" goodsno like '%{0}%' or", brand[i]);
                    }
                }

                if (sqlwhere != "")
                {
                    sqlwhere = "and (" + sqlwhere + ")";
                }
            }

            string sql = string.Format(@"
                                        select b.shopId,COUNT(1) AS sellcount from T_RefundDetail as a join ( 
                                                   select refundNo,shopId from T_Refund where sndTime>='{0} 00:00:00' and sndTime<'{1} 00:00:00') as b 
                                                        on a.refundNo=b.refundNo where 1=1 {2} group by shopId", startTime, endTime, sqlwhere);
            return sql;
        }

        private void btn_Statistics(object sender, RoutedEventArgs e)
        {
            BaseDAL dal = new BaseDAL();
            var dt = dal.QueryDataTable(StatisticsSQL(start.Text, end.Text));
            dglist.ItemsSource = dt.AsDataView();
        }






        private void btn_RefundStatistics(object sender, RoutedEventArgs e)
        {

            List<Erp_sale_Info> datasource = new List<Erp_sale_Info>();
            BaseDAL dal = new BaseDAL();

            if (cbx_month.SelectedIndex == 0)
            {
                MessageBox.Show("请选择统计的月份", "提示");
                return;
            }

            List<string> brand = new List<string>();

            brand.Add("VV");
            brand.Add("YE");
            brand.Add("YH");
            brand.Add("ZM");
            brand.Add("TT");
            brand.Add("XK");

            brand.Add("HU");
            brand.Add("MU");
            brand.Add("BU");
            brand.Add("AL");
            brand.Add("GX");
            brand.Add("MA");
            brand.Add("JL");
            brand.Add("IV");
            brand.Add("TN");
            brand.Add("LD");
            brand.Add("XL");
            brand.Add("UE");
            brand.Add("TM");
            brand.Add("YJ");
            brand.Add("BO");
            brand.Add("MK");
            brand.Add("FD");
            brand.Add("AS");

            //所有品牌数据
            var m = _9M.Work.ErpApi.CallWdgjServer.CallData<Erp_sale_Info>(ErpSaleSQL(cbx_month.SelectionBoxItem.ToString(), null));
            DataTable dt_erp_sale = dal.QueryDataTable(RefundSql(cbx_month.SelectionBoxItem.ToString(), null));

            foreach (DataRow dr in dt_erp_sale.Rows)
            {
                var model = m.Find(a => a.ShopId.Equals(Convert.ToInt64(dr["shopId"])));
                if (model != null)
                {
                    model.RefundCount = Convert.ToInt32(dr["sellcount"]);
                    model.TotalPerent = (Convert.ToDouble(model.RefundCount) / Convert.ToDouble(model.SellCount) * 100).ToString("f2") + "%";
                    datasource.Add(model);
                }
            }

            //高端品牌数据
            var m_high = _9M.Work.ErpApi.CallWdgjServer.CallData<Erp_sale_Info>(ErpSaleSQL(cbx_month.SelectionBoxItem.ToString(), brand));
            DataTable dt_erp_sale_high = dal.QueryDataTable(RefundSql(cbx_month.SelectionBoxItem.ToString(), brand));

            foreach (DataRow dr in dt_erp_sale_high.Rows)
            {
                var model = datasource.Find(a => a.ShopId.Equals(Convert.ToInt64(dr["shopId"])));

                if (model != null)
                {
                    model.HighRefundCount = Convert.ToInt32(dr["sellcount"]);

                    var mm = m_high.Find(a => a.ShopId.Equals(model.ShopId));

                    if (mm != null)
                    {
                        model.HighSellCount = mm.SellCount;
                    }

                    model.HighPerent = (Convert.ToDouble(model.HighRefundCount) / Convert.ToDouble(model.HighSellCount) * 100).ToString("f2") + "%";
                }
            }


            Erp_sale_Info total = new Erp_sale_Info();
            total.ShopName = "合计";
            total.SellCount = datasource.Sum(a => a.SellCount);
            total.RefundCount = datasource.Sum(a => a.RefundCount);
            total.HighRefundCount = datasource.Sum(a => a.HighRefundCount);
            total.HighSellCount = datasource.Sum(a => a.HighSellCount);
            total.TotalPerent = (Convert.ToDouble(total.RefundCount) / Convert.ToDouble(total.SellCount) * 100).ToString("f2") + "%";
            total.HighPerent = (Convert.ToDouble(total.HighRefundCount) / Convert.ToDouble(total.HighSellCount) * 100).ToString("f2") + "%";
            datasource.Add(total);
            dgRefund.ItemsSource = datasource;
        }
    }


    public class Erp_sale_Info
    {
        public Int64 ShopId { get; set; }
        public string ShopName { get; set; }
        public Int64 SndTime { get; set; }
        public Double SellCount { get; set; }
        public Int32 RefundCount { get; set; }
        public Double HighSellCount { get; set; }
        public Int32 HighRefundCount { get; set; }
        public string TotalPerent { get; set; }
        public string HighPerent { get; set; }
    }

}
