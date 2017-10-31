using _9M.Work.DbObject;
using _9M.Work.Model;
using _9M.Work.WPF_Common.ValueObjects;
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

namespace _9M.Work.WPF_Main.Views.Refund
{
    /// <summary>
    /// Interaction logic for workStatistics.xaml
    /// </summary>
    public partial class workStatistics : UserControl
    {

        BaseDAL dal = new BaseDAL();

        public workStatistics()
        {
            InitializeComponent();
            start.Text = DateTime.Now.ToString("yyyy-MM-01");
            end.Text = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-01")).AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd");
            shopBind();
        }


        #region 更新退货表的发货时间

        public void updateSndTime()
        {
            for (int i = 29901; i < 29901; i++)
            {

                RefundModel rm = dal.GetSingle<RefundModel>(a => a.id.Equals(i));
                if (rm != null)
                {
                    string sql = string.Format("select SndTime from G_trade_tradelist where tradeNo2='{0}'", rm.tradeNo);
                    var m = _9M.Work.ErpApi.CallWdgjServer.CallData<Erp_Model_GJRefund>(sql);
                    if (m != null)
                    {
                        if (m.Count > 0)
                        {
                            dal.ExecuteSql(string.Format("update T_Refund set sndTime='{0}' where id={1}", m[0].SndTime, i));
                        }
                    }
                }

            }


        }

        #endregion

        void shopBind()
        {
            try
            {
                List<ShopModel> list = dal.GetAll<ShopModel>();
                ShopModel m = new ShopModel() { id = -1, shopId = -1, shopName = "所有店铺" };
                list.Insert(0, m);
                cbx_Shop.ItemsSource = list;
                cbx_Shop.SelectedItem = m;
            }
            catch { }
        }

        #region 开始统计

        private void btn_Statistics(object sender, RoutedEventArgs e)
        {
            ShopModel s = cbx_Shop.SelectedItem as ShopModel;

            string sql = string.Empty;
            string sqlWhere = string.Empty;

            if (s.id != -1) { sqlWhere = string.Format(" and shopId={0}", s.id); }

            if (rdException.IsChecked.Equals(true))
            {
                //                sql = string.Format(@"select exceptionEmployee as eName,ISNULL(COUNT(*),0) as [goodsCount]  from T_RefundDetail where refundNo in(                                                                                                                                
                //                                        select refundNo from T_Refund where regTime>='{0}' and regTime<='{1}' {2} and  exceptionEmployee!='' and exceptionEmployee is not null) 
                //                                          and confirmReceipt={3}  group by exceptionEmployee", start.Text + " 00:00:00", end.Text + " 23:59:59", sqlWhere, (int)ReceiptStatus.exceptionEnd);

                sql = string.Format(@"with t1 as(
                                        select exceptionEmployee as eName,ISNULL(COUNT(*),0) as [goodsCount]  from T_RefundDetail where refundNo in(                                                                                                                                
                                               select refundNo from T_Refund where regTime>='{0}' and regTime<='{1}' {2}
                                                        ) and confirmReceipt={3} and  exceptionEmployee!='' and exceptionEmployee is not null  group by exceptionEmployee
                                        ),t2 as
                                        (
                                        select ename,COUNT(*) as orderCount from(
                                          select exceptionEmployee as eName, refundNo  from T_RefundDetail where refundNo in(                                                                                                                                
                                               select refundNo from T_Refund where regTime>='{0}' and regTime<='{1}' 
                                                        ) and confirmReceipt=3 and  exceptionEmployee!='' and exceptionEmployee is not null  group by refundNo,exceptionEmployee)as a group by eName
                                        )
                                        select t2.eName, t2.orderCount,t1.goodsCount from t2 join t1 on t1.eName=t2.eName", start.Text + " 00:00:00", end.Text + " 23:59:59", sqlWhere, (int)ReceiptStatus.exceptionEnd);
            }
            else if (rdRegister.IsChecked.Equals(true))
            {
                sql = string.Format(@"with tb as
                                        (
                                         select regEmployee as eName,ISNULL(COUNT(*),0) as [orderCount]  from T_Refund where regTime>='{0}' 
                                         and regTime<='{1}' {2}  group by regEmployee
                                        ),
                                        tb2 as
                                        (
                                          select regEmployee as eName,ISNULL(COUNT(*),0) as [goodsCount] from T_Refund as a join T_RefundDetail as b on a.refundNo=b.refundNo where regTime>='{0}' 
                                          and regTime<='{1}' {2} group by regEmployee
                                        )
                                        select a.eName,a.orderCount,b.goodsCount from tb as a join tb2 as b on a.eName=b.eName", start.Text + " 00:00:00", end.Text + " 23:59:59", sqlWhere);
            }
            else if (rdUnpacking.IsChecked.Equals(true))
            {
                sql = string.Format(@"with tb as
                                        (
                              
                                            select unpackingEmployee as eName,ISNULL(COUNT(*),0) as [goodsCount]  from T_RefundDetail as a join 
                                            T_Refund as b on a.refundNo=b.refundNo   where a.unpackingTime>='{0}' and a.unpackingTime<='{1}' and unpackingEmployee<>'' {2}
                                            group by unpackingEmployee
                                        ),
                                        tb2 as
                                        (
                                        select unpackingEmployee as eName,ISNULL(COUNT(*),0) as [orderCount] from (
                                                 select distinct a.refundNo,unpackingEmployee  from T_RefundDetail as a join T_Refund as b on a.refundNo=b.refundNo
                                                     where  a.unpackingTime>='{0}' and a.unpackingTime<='{1}' {2}
                                        ) as a group by unpackingEmployee
                                        )
                                        select a.eName,a.goodsCount,b.orderCount from tb as a join tb2 as b on a.eName=b.eName",
                                            start.Text + " 00:00:00", end.Text + " 23:59:59", sqlWhere);
            }
            else if (rdfxRefundHandle.IsChecked.Equals(true))
            {
                sql = string.Format(@"select oper as [eName],COUNT(*) as  [orderCount],null as [goodsCount] from T_RefundLog where eventName='确认完成' and oper in(
                                        select username from dbo.T_userinfo where DeptId=6) and operTime>='{0}' and operTime<='{1}'
                                        group by oper", start.Text + " 00:00:00", end.Text + " 23:59:59");

            }

            dglist.ItemsSource = dal.QueryList<workStatisticsModel>(sql, new object[] { });
        }

        #endregion
    }

    public class workStatisticsModel
    {
        public string eName { get; set; }

        public int? goodsCount { get; set; }

        public int? orderCount { get; set; }

    }





}


