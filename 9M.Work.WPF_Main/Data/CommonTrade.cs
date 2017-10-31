using _9M.Work.DbObject;
using _9M.Work.Model;
using _9M.Work.TopApi;
using _9M.Work.WPF_Common.ValueObjects;
using _9Mg.Work.TopApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Top.Api.Domain;

namespace _9M.Work.WPF_Main.Data
{
   public  class CommonTrade
    {
        /// <summary>
        /// 得到C分销9魅季所有未付款的订单
        /// </summary>
        /// <returns></returns>
        public List<NoPayModel> GetNoPayList()
        {
           
            List<ShopModel> shoplist = new BaseDAL().GetAll<ShopModel>();
            //得到淘宝授权对象
            TopSource cos = new TopSource( shoplist.Where(x=>x.shopId==1000).FirstOrDefault());
            TopSource mos = new TopSource(shoplist.Where(x => x.shopId == 1022).FirstOrDefault());
        
            //得到淘宝的当前时间
            DateTime now = new TopSource().GetTaoBaoNow();
            //得到C店和9魅季的未付订单
            List<Order> corderlist = cos.GetOrders(TradeStatus.WAIT_BUYER_PAY, now.AddDays(-3), now, null);
            List<Order> mgorderlist = mos.GetOrders(TradeStatus.WAIT_BUYER_PAY, now.AddDays(-3), now, null);
            List<SubPurchaseOrder> fxtradelist = cos.GetFenXiaoOrders(TradeStatus.WAIT_BUYER_PAY, now.AddDays(-3), now);
            //组装对向
            List<Order> realtrade = new List<Order>();
            realtrade.AddRange(corderlist);
            realtrade.AddRange(mgorderlist);
            fxtradelist.ForEach(
              suborder =>
              {
                  if (suborder.ItemOuterId != null && suborder.SkuOuterId != null)
                  {
                      Order o = new Order();
                      o.OuterIid = suborder.ItemOuterId;
                      o.OuterSkuId = suborder.SkuOuterId;
                      o.Num = suborder.Num;
                      realtrade.Add(o);
                  }
              }
            );
            //求和
            var query = from p in realtrade
                        group p by new { p.OuterSkuId, p.OuterIid } into g
                        select new NoPayModel
                        {
                            OuterIid = g.Key.OuterIid,
                            OuterSkuId = g.Key.OuterSkuId,
                            Num = g.Sum(a => a.Num)
                        };
            List<NoPayModel> list = query.ToList();
            return list;
        }
    }
}
