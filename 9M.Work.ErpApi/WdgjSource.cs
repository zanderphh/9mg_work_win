using _9M.Work.Model;
using _9M.Work.Model.Statistics;
using _9M.Work.Model.WdgjWebService;
using _9M.Work.Utility;
using _9M.Work.WSVariable;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _9M.Work.ErpApi
{
    public class CallWdgjServer
    {
        public static string url = "http://112.124.16.76/Wdgj/WdgjService.asmx";

        public static WSSoapHeader getWsHead()
        {
            WSSoapHeader header = new WSSoapHeader("WSSoapHeader");
            header.AddProperty("UserName", "9mg");
            header.AddProperty("Password", "www.9mg.cn");
            return header;
        }

        public static List<T> CallData<T>(string Sql) where T : new()
        {
            object[] arg = new object[2];
            arg[0] = "DataSet";
            arg[1] = Sql;

            object result = HttpHelper.InvokeWebService(url, "CallWdgjServerVariable", arg, getWsHead());
            var list = JsonConvert.DeserializeObject<DataTable>(result.ToString());
            return ConvertType.ConvertToModel<T>(list);
        }

        public static DataTable CallData(string Sql)
        {
            object[] arg = new object[2];
            arg[0] = "DataSet";
            arg[1] = Sql;


            object result = HttpHelper.InvokeWebService(url, "CallWdgjServerVariable", arg, getWsHead());
            var list = JsonConvert.DeserializeObject<DataTable>(result.ToString());
            return list;
        }

    }

    public class SpecListRequest
    {
        /// <summary>
        /// 单款号查询
        /// </summary>
        public string GoodsNo { get; set; }
        /// <summary>
        /// 附加码（GoodsNoALL可代替）
        /// </summary>
        public string SpecCode { get; set; }
        /// <summary>
        /// 单条SKU记录
        /// </summary>
        public string GoodsNoAll { get; set; }
        /// <summary>
        /// 款号的IN查询
        /// </summary>
        public List<string> GoodsNoList { get; set; }
    }

    public class Spec
    {
        public string GoodsNo { get; set; }
        public string SpecName { get; set; }
        public string SpecCode { get; set; }
        public Int64 Stock { get; set; }
    }

    public class WdgjSource
    {
        public List<V_GoodsSpecModel> QueryPurchase(SpecListRequest request)
        {
            List<string> GoodsNoList = new List<string>();

            if (request.GoodsNoList != null)
            {
                if (request.GoodsNoList.Count > 0)
                {
                    GoodsNoList = request.GoodsNoList;
                }
            }

            string Sql = string.Format(@"select GoodsNO,SUM(GoodsCount) as Stock from dbo.G_Stock_StockINDetail AS A (nolock)
                    JOIN G_Stock_StockIN AS B (nolock) ON A.billID=B.StockINID 
                    JOIN dbo.G_Goods_GoodsList AS C (nolock) ON A.GoodsID=C.GoodsID
                    where theCause='采购' AND  GoodsNo in('{0}') GROUP BY GoodsNO", string.Join(",", GoodsNoList.ToArray()).Replace(",", "','"));

            return CallWdgjServer.CallData<V_GoodsSpecModel>(Sql);
        }

        /// <summary>
        /// 查询商品的多规格
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public List<V_GoodsSpecModel> SpecList(SpecListRequest request)
        {
            string Sql = "select * from M_SpecList where 1=1 ";
            string SqlWhere = string.Empty;
            //条件组合
            if (!string.IsNullOrEmpty(request.GoodsNo))
            {
                SqlWhere += string.Format(@"and GoodsNo='{0}'", request.GoodsNo);
            }
            if (!string.IsNullOrEmpty(request.SpecCode))
            {
                SqlWhere += string.Format(@"and SpecCode='{0}'", request.SpecCode);
            }
            if (!string.IsNullOrEmpty(request.GoodsNoAll))
            {
                SqlWhere += string.Format(@"and GoodsNoAll='{0}'", request.GoodsNoAll);
            }
            if (request.GoodsNoList != null)
            {
                if (request.GoodsNoList.Count > 0)
                {
                    List<string> GoodsNoList = request.GoodsNoList;
                    SqlWhere += string.Format(@"and GoodsNo in('{0}')", string.Join(",", GoodsNoList.ToArray()).Replace(",", "','"));
                }
            }
            return CallWdgjServer.CallData<V_GoodsSpecModel>(Sql + SqlWhere);
        }

        public List<Spec> SpecListByGoodsNo(string GoodsNo)
        {
            string Sql = string.Format("select GoodsNo,SpecCode,Stock,SpecName from M_SpecList where GoodsNo = '{0}'", GoodsNo);
            return CallWdgjServer.CallData<Spec>(Sql);
        }

        public static List<SpecModel> SpecList(string goodsno)
        {
            string sql = string.Format(@"select a.SpecCode,Convert(int,c.stock) as StockAll,Convert(int,c.sndcount) as sndcount,Convert(int,c.ordercount) as ordercount, (c.Stock-c.sndcount-c.ordercount) as stock,a.SpecName ,a.SpecName2
                ,a.SpecName1,b.GoodsNO,b.GoodsName from dbo.G_Goods_GoodsSpec a
                left join dbo.G_Goods_GoodsList b on a.GoodsID= b.GoodsID
                left join G_Stock_Spec c on a.Specid = c.Specid
                where b.GoodsNO='{0}' and c.WarehouseID=1000", goodsno);
            List<SpecModel> list = CallWdgjServer.CallData<SpecModel>(sql);
            list = list.OrderBy(a => a.SpecName1).ThenBy(a => a.SpecCode).ToList();
            return list;
        }

        public static GoodsModel GoodsInformation(string GoodsNo)
        {
            string sql = string.Format(@"select b.GoodsNo,b.Brand,b.Price1,b.Price_Member,b.Reserved3,a.classname,b.goodsname from dbo.G_Goods_GoodsList b join G_Goods_GoodsClass a
              on a.classid = b.classid where b.goodsno='{0}'", GoodsNo);

            try
            {
                GoodsModel model = CallWdgjServer.CallData<GoodsModel>(sql)[0];
                return model;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public static List<Erp_Model_GJRefund> GetTBSourceTradeNo(string InputNo)
        {
            List<Erp_Model_GJRefund> tbTradeNo = new List<Erp_Model_GJRefund>();
            string sql = string.Empty;
            if (InputNo.Contains("JY"))
            {
                sql = string.Format(@"select Convert(varchar()TradeNO2 AS [tbTradeNo],SndTo as [CName],a.Adr as [Address],a.PostID as [SendExpressNo],a.TradeNo as [GjTradeNo],a.TradeNick as [NickName],a.Tel as [Mobile],
                                       a.ShopID,b.ShopName,a.SndTime
                                       from dbo.G_Trade_TradeList as a join  G_Cfg_ShopList as b on  a.ShopId= b.ShopId 
                                       where TradeNo='{0}'", InputNo);
            }
            else
            {

                sql = string.Format(@"select TradeNO2 AS [tbTradeNo],SndTo as [CName],a.Adr as [Address],a.PostID as [SendExpressNo],a.TradeNo as [GjTradeNo],a.TradeNick as [NickName],a.Tel as [Mobile],
                                      a.ShopID,b.ShopName,a.SndTime
                                      from dbo.G_Trade_TradeList as a join  G_Cfg_ShopList as b on  a.ShopId= b.ShopId 
                                      where TradeNO2 like('%{0}%') or PostID ='{0}'", InputNo);
            }
            try
            {
                tbTradeNo = CallWdgjServer.CallData<Erp_Model_GJRefund>(sql);
            }
            catch (Exception ex)
            { }
            return tbTradeNo;
        }

        public static List<GoodsModel> GoodsByTrade(string TradeNo)
        {
            string sql = string.Format(@"select  c.GoodsNO,d.SpecCode,c.GoodsName,d.SpecName,Convert(int,a.SellCount) as SellCount ,e.PositionRemark from dbo.G_Trade_GoodsList a 
                  join dbo.G_Trade_TradeList b on a.TradeID = b.TradeID
                  join dbo.G_Goods_GoodsList c on a.GoodsID = c.GoodsID
                  join dbo.G_Goods_GoodsSpec d on a.SpecID = d.SpecID and a.GoodsID=d.GoodsID
                  join G_Stock_Spec e on e.specID = d.specID and a.GoodsID=e.GoodsID
                  where ( b.TradeNO in ('{0}') or b.tradeNo2 like'%{1}%') and e.warehouseid=1000", TradeNo.Replace(",", "','"), TradeNo);

            List<GoodsModel> modellist = CallWdgjServer.CallData<GoodsModel>(sql);
            modellist.ForEach(
                delegate(GoodsModel model)
                {
                    string P = string.Empty;
                    string F = string.Empty;
                    if (!string.IsNullOrEmpty(model.PositionRemark))
                    {
                        GoodsHelper.SplitPostion(model.PositionRemark, out P, out F);
                    }
                    model.P_postion = P;
                    model.F_postion = F;
                }
                );
            return modellist;
        }

        public static List<Erp_Model__GJTradeGoods> GetGjTradeGoods(string TBTtradeNo)
        {
            List<GoodsModel> gm = GoodsByTrade(TBTtradeNo);

            List<Erp_Model__GJTradeGoods> TradeGoods = new List<Erp_Model__GJTradeGoods>();

            try
            {
                gm.ForEach(g =>
                {
                    TradeGoods.Add(new Erp_Model__GJTradeGoods() { goodsno = g.GoodsNo, tradegoodsno = g.GoodsNo + g.SpecCode, tradespec = g.SpecName });
                });
            }
            catch (Exception ex)
            { }
            return TradeGoods;
        }

        public static GoodsModel GoodsDetail(string GoodsNoDetail)
        {
            string GoodsNo = string.Empty;
            string SpecCode = string.Empty;
            GoodsHelper.SplitGoodsDetail(GoodsNoDetail, out GoodsNo, out SpecCode);
            string sql = string.Format(@"select b.GoodsNO,a.SpecCode,convert(int,(c.Stock-c.sndcount-c.ordercount)) as stock,a.SpecName ,
                f.classname,b.goodsname,c.PositionRemark,b.Brand,b.Price1,b.Price_Member,b.Reserved3 ,a.SpecName2,b.goodsname2
                ,a.SpecName1 ,b.Price_Detail,ISNULL(d.HaoXing,'') as HaoXing,ISNULL(d.ZhiXing,'FZ/T?81016-2008') as ZhiXing from dbo.G_Goods_GoodsSpec a
                left   join dbo.G_Goods_GoodsList b on a.GoodsID= b.GoodsID 
                left   join G_Stock_Spec c on a.Specid = c.Specid and c.Goodsid=a.Goodsid
                left   join FyiByClass d on b.ClassID = d.ClassID  and a.SpecName2 = d.SpecType
                left join G_Goods_GoodsClass f on f.classid = b.classid
                where b.GoodsNO='{0}'  and a.speccode='{1}' and c.WarehouseID=1000", GoodsNo, SpecCode);
            GoodsModel model = null;
            try
            {
                model = CallWdgjServer.CallData<GoodsModel>(sql)[0];
                string P = string.Empty;
                string F = string.Empty;
                if (!string.IsNullOrEmpty(model.PositionRemark))
                {
                    GoodsHelper.SplitPostion(model.PositionRemark, out P, out F);
                }
                model.P_postion = P;
                model.F_postion = F;
            }
            catch { }
            return model;
        }

        public static List<ApiSpecModel> GetSpecListByGoodsNos(List<string> IdsList)
        {

            string ids = string.Empty;
            IdsList.ForEach(
                delegate(string str)
                {
                    ids += str.Trim() + ",";
                }
                );
            string sql = string.Format(@"select a.GoodsNo,a.goodsname,c.ClassName,b.SpecCode,b.SpecName,b.SpecName1,b.SpecName2,convert(int, (case when (d.Stock-d.sndcount-d.ordercount)<0 then 0 else (d.Stock-d.sndcount-d.ordercount) end)) as stock from G_Goods_GoodsList a 
                                         join G_Goods_GoodsSpec b on a.GoodsID = b.GoodsID
                                         join G_Goods_GoodsClass c on a.ClassID = c.ClassID 
                                         join G_Stock_Spec d  on b.SpecID = d.SpecID
                                         where a.GoodsNO in ('{0}')", ids.TrimEnd(',').Replace(",", "','"));

            return CallWdgjServer.CallData<ApiSpecModel>(sql);
        }

        public static List<SpecModel> GetSpecListByGoodsNo(string goodsNo)
        {
            try
            {
                string sql = string.Format(@"select a.SpecCode,Convert(int,c.stock) as StockAll,Convert(int,c.sndcount) as sndcount,Convert(int,c.ordercount) as ordercount, convert(int,(c.Stock-c.sndcount-c.ordercount)) as stock,a.SpecName ,a.SpecName2
                         ,a.SpecName1,b.GoodsNO,b.GoodsName from dbo.G_Goods_GoodsSpec a
                         left   join dbo.G_Goods_GoodsList b on a.GoodsID= b.GoodsID
                         left   join G_Stock_Spec c on a.Specid = c.Specid
                         where b.GoodsNO='{0}' and c.WarehouseID=1000", goodsNo);

                return CallWdgjServer.CallData<SpecModel>(sql.ToString());
            }

            catch
            {
                return null;
            }


        }

        public static List<ApiTopicGoodsModel> GetGoodsByHasStock(string GoodsNos)
        {
            try
            {
                string sql = string.Format(@"select GoodsNO,GoodsName,CONVERT(int,Stock3) as stock,CONVERT(int,Stock1/(case when (SellcountWeek+sndcount+OrderCount)<=0 then 1 else  (SellcountWeek+sndcount+OrderCount) end)) as SellcountWeek ,ClassName from V_GoodsList where 
                        bBlockUp=0  and Stock3>0  and Stock1/(case when (SellcountWeek+sndcount+OrderCount)<=0 then 1 else   (SellcountWeek+sndcount+OrderCount) end)>0
                        and GoodsNo in('{0}')", GoodsNos.Replace(",", "','"));
                return CallWdgjServer.CallData<ApiTopicGoodsModel>(sql.Replace("+", "%2B"));
            }
            catch
            {
                return new List<ApiTopicGoodsModel>();
            }
        }

        public static List<WareHouseInModel> ByBillDID(string BillDID)
        {
            string sql = string.Format(@"select a.goodsid,a.Goodsno,c.SpecCode,a.SpecName,Convert(int,a.GoodsCount) as GoodsCount,b.PositionRemark,a.RegOperator,a.CHKTime,a.BillID from 
V_Sta_StockINDetail a 
join G_Stock_Spec b  on a.GoodsID=b.GoodsID and a.specid = b.specid
join  dbo.G_Goods_GoodsSpec c on c.SpecID=a.SpecID and a.goodsid = c.goodsid where a. BillID='{0}' and a.SpecID = b.SpecID and b.WareHouseID=1000", BillDID);
            List<WareHouseInModel> list = CallWdgjServer.CallData<WareHouseInModel>(sql);
            list.ForEach(
                delegate(WareHouseInModel model)
                {
                    string P = string.Empty;
                    string F = string.Empty;
                    if (!string.IsNullOrEmpty(model.PositionRemark))
                    {
                        GoodsHelper.SplitPostion(model.PositionRemark, out P, out F);
                    }
                    model.P_Position = P;
                    model.F_Position = F;
                }
                );
            return list;
        }

        public static bool SavePositionTran(List<PostSkuModel> list)
        {
            bool b = false;

            try
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat(" SET XACT_ABORT ON ").AppendLine();
                sb.AppendFormat(" BEGIN TRANSACTION ").AppendLine();
                // 这里才是事务中的代码
                for (int i = 0; i < list.Count; i++)
                {
                    string GoodsNo = string.Empty;
                    string SpecCode = string.Empty;
                    GoodsHelper.SplitGoodsDetail(list[i].GoodsNo, out GoodsNo, out SpecCode);
                    string P_Position = list[i].P_postion;
                    string F_Position = list[i].F_postion;
                    string PositionRemark = string.Empty;
                    if (list[i].UpdatePrimaryOnly) //如果只修改主货位
                    {
                        string Postion = GoodsDetail(list[i].GoodsNo).PositionRemark; //查出货位
                        PositionRemark = GoodsHelper.PrimaryPostionOnly(Postion, P_Position);
                    }
                    else
                    {
                        PositionRemark = GoodsHelper.AssemblePostion(P_Position, F_Position);
                    }
                    sb.AppendFormat(string.Format(" update G_Stock_Spec set PositionRemark='{0}' where goodsid=(select goodsid from dbo.G_Goods_GoodsList where goodsno= '{1}') and specid =(select specid from dbo.G_Goods_GoodsSpec where speccode='{2}' and goodsid=(select goodsid from  dbo.G_Goods_GoodsList where goodsno='{3}')) and WarehouseID=1000 ", PositionRemark, GoodsNo, SpecCode, GoodsNo)).AppendLine();
                }

                sb.AppendFormat(" select 1 as col ").AppendLine();
                sb.AppendFormat(" COMMIT TRANSACTION").AppendLine();

                List<DataTable> tables = CallWdgjServer.CallData<DataTable>(sb.ToString());

                if (tables != null)
                {
                    if (tables.Count > 0)
                    {
                        b = true;
                    }
                }
            }
            catch
            {

            }

            return b;
        }

        public static bool UpdateGoods(List<PostUpdateCommodityModel> list)
        {
            bool b = false;

            if (list.Count > 0)
            {
                int Type = list[0].UpdateType;
                try
                {

                    StringBuilder sb = new StringBuilder();
                    sb.AppendFormat(" SET XACT_ABORT ON ").AppendLine();
                    sb.AppendFormat(" BEGIN TRANSACTION ").AppendLine();

                    if (Type == 1) //只追加描述
                    {
                        foreach (PostUpdateCommodityModel model in list)
                        {
                            string sql = string.Format("update G_Goods_GoodsList set Remark=Remark+'{0}' where Goodsno='{1}'", model.ReMark, model.GoodsNo);
                            sb.AppendFormat(sql.Replace("+", "%2B")).AppendLine();
                        }
                    }
                    else if (Type == 2) //只修改拍照备注 
                    {
                        foreach (PostUpdateCommodityModel model in list)
                        {
                            string sql = string.Format("update G_Goods_GoodsList set Reserved1='{0}',Reserved2='{1}' where Goodsno='{2}'", model.Reserved1, model.Reserved2, model.GoodsNo);
                            sb.AppendFormat(sql).AppendLine();
                        }
                    }
                    else if (Type == 3)
                    {
                        foreach (PostUpdateCommodityModel model in list)
                        {
                            string sql = string.Format("update G_Goods_GoodsList set Price_Detail={0},Price2={1},selldate='{2}',remark=REPLACE(remark,'{3}','') where Goodsno='{4}'",
                                model.Price_Detail, model.Price2, model.SellDate, model.ReMark, model.GoodsNo);
                            sb.AppendFormat(sql).AppendLine();
                        }
                    }
                    else if (Type == 4)
                    {
                        foreach (PostUpdateCommodityModel model in list)
                        {
                            string sql = string.Format("update G_Goods_GoodsList set Reserved3='{0}' where goodsno='{1}'", model.Reserved1, model.GoodsNo);
                            sb.AppendFormat(sql).AppendLine();
                        }
                    }

                    sb.AppendFormat(" select 1 as col ").AppendLine();
                    sb.AppendFormat(" COMMIT TRANSACTION").AppendLine();

                    List<DataTable> tables = CallWdgjServer.CallData<DataTable>(sb.ToString());

                    if (tables != null)
                    {
                        if (tables[0] != null)
                        {
                            if (tables[0].Rows.Count > 0)
                            {
                                if (tables[0].Rows[0]["col"].ToString().Equals("1"))
                                {
                                    b = true;
                                }
                            }
                        }
                    }
                }
                catch
                {
                    b = false;
                }
            }
            return b;
        }

        public static DataTable getErpStockByGoodsno(string goodsno)
        {
            DataTable dt = new DataTable();
            try
            {
                string sql = string.Format(@"select b.GoodsNO as 款号, CONVERT(int, SUM(a.Stock)) as  '库存'  from G_Stock_Spec a 
                       join G_Goods_GoodsList b on a.GoodsID = b.GoodsID
                        where b.GoodsNO like '{0}%'
                        group by b.GoodsNO", goodsno);

                dt = CallWdgjServer.CallData(sql);

            }
            catch
            {
            }

            return dt;
        }

        public static String CallDataJsonString(string Sql)
        {
            object[] arg = new object[2];
            arg[0] = "DataSet";
            arg[1] = Sql;
            object result = HttpHelper.InvokeWebService(CallWdgjServer.url, "CallWdgjServer", arg);
            return result.ToString();
        }


        public static List<SaleModel> Statist(SalePostModel model)
        {

            try
            {
                string GoodsWhere = string.Empty;
                //品牌
                if (model.Brand != "0")
                {
                    GoodsWhere += string.Format(" and c.GoodsNo like '%{0}%'", model.Brand);
                }
                //类目
                if (model.Category != "0")
                {
                    GoodsWhere += string.Format(" and c.ClassID = (select  ClassID from G_Goods_GoodsClass where ClassName='{0}' and FatherID=0)", model.Category);
                }
                //指定款号
                if (model.GoodsList != null)
                {
                    string res = string.Empty;
                    model.GoodsList.ForEach(x =>
                    {
                        res += x + ",";
                    });
                    GoodsWhere += string.Format(" and c.GoodsNo in ('{0}')", res.TrimEnd(',').Replace(",", "','"));
                }
                //款号模糊查询
                if (!string.IsNullOrEmpty(model.KeyWord))
                {
                    GoodsWhere += string.Format(" and c.GoodsNo like '%{0}%'", model.KeyWord);
                }
                //价格
                if (model.PriceMin != "0")
                {
                    GoodsWhere += string.Format(" and c.Price_Detail >={0}", model.PriceMin);
                }
                if (model.PriceMax != "0")
                {
                    GoodsWhere += string.Format(" and c.Price_Detail <={0}", model.PriceMax);
                }
                //库存
                if (model.StockMin != "0")
                {
                    GoodsWhere += string.Format(" and c.Stock >={0}", model.StockMin);
                }
                if (model.StockMax != "0")
                {
                    GoodsWhere += string.Format(" and c.Stock <={0}", model.StockMax);
                }
                //区间
                if (model.WeekMin != "0")
                {
                    GoodsWhere += string.Format(" and CONVERT(int, round((Stock/Sellcount),0)) >={0}", model.WeekMin);
                }
                if (model.WeekMax != "0")
                {
                    GoodsWhere += string.Format(" and CONVERT(int, round((Stock/Sellcount),0))<={0}", model.WeekMax);
                }
                //年份季节
                if (model.YearSeasonList != null)
                {
                    if (model.YearSeasonList.Count > 0)
                    {
                        string res = string.Empty;
                        for (int i = 0; i < model.YearSeasonList.Count; i++)
                        {
                            if (i == 0)
                            {
                                res += string.Format(@"  c.GoodsNo like '{0}%'", model.YearSeasonList[i]);
                            }
                            else
                            {
                                res += string.Format(@" or c.GoodsNo like '{0}%'", model.YearSeasonList[i]);
                            }
                        }
                        GoodsWhere += string.Format(@" and ({0})", res);
                    }
                }
                string ShopWhere = string.Empty;
                //店铺
                if (model.Shop != "0")
                {
                    if (model.Shop.Equals("C店"))
                    {
                        ShopWhere = string.Format("and ShopId={0}", 1000);
                    }
                    if (model.Shop.Equals("分销"))
                    {
                        ShopWhere = string.Format("and ShopId={0}", 1003);
                    }
                    if (model.Shop.Equals("9魅季"))
                    {
                        ShopWhere = string.Format("and ShopId={0}", 1022);
                    }
                }

                string sql = string.Format(@"select  c.GoodsNO,convert(int, c.Stock) as Stock,IsNull( c.Reserved3 ,'')as Price,isnull( c.Price_Detail ,'') as Price_Detail,CONVERT(int, a.Sellcount) as sellcount, a.SellTotal as selltotal,
                CONVERT(int, ISNULL(f.GoodsCount,0)) as PurCount,CONVERT(int, ISNULL(g.GoodsCount,0)) as BackCount,isnull( c.SellDate,'') as SellDate,CONVERT(int, round((Stock/Sellcount),0)) as Week
                from (
                select  b.GoodsID,SUM(b.SellCount) as Sellcount,SUM(b.SellTotal) as SellTotal 
                from G_Trade_TradeList a join G_Trade_GoodsList b on a.TradeID = b.TradeID
                where a.SndTime> '{0}' and a.SndTime<'{1}' and a.TradeStatus<>0 {2} and a.shopid<>1026
                group by b.GoodsID)  a 
                join (
                select c.GoodsID,c.ClassID, c.GoodsNO ,c.Reserved3,c.Price_Detail,c.SellDate ,SUM(b.Stock-b.SndCount-b.OrderCount) as Stock from G_Goods_GoodsList c join G_Stock_Spec b
                on c.GoodsID = b.GoodsID
                group by c.GoodsNO ,c.Reserved3,c.Price_Detail,c.SellDate,c.GoodsID,c.ClassID) c on a.GoodsID = c.GoodsID
                join G_Goods_GoodsClass d on c.ClassID = d.ClassID

                left join (select b.GoodsID,SUM(b.GoodsCount) as GoodsCount from G_Stock_StockIN a join G_Stock_StockINDetail b on a.StockINID = b.BillID 
                where a.theCause = '采购入库' and a.regDate> '{0}' and a.regDate<'{1}' 
                group by b.GoodsID) f on c.GoodsID = f.GoodsID
                left join (select b.GoodsID,SUM(b.GoodsCount) as GoodsCount from G_Stock_StockIN a join G_Stock_StockINDetail b on a.StockINID = b.BillID 
                where a.theCause = '退货' and a.regDate> '{0}' and a.regDate<'{1}' 
                group by b.GoodsID) g on c.GoodsID = g.GoodsID
                where 0=0 {3}
                Group by c.GoodsNO ,c.Reserved3,c.Price_Detail,c.SellDate,c.Stock,f.GoodsCount,g.GoodsCount,a.Sellcount,a.SellTotal", model.StartTime, model.EndTime, ShopWhere, GoodsWhere);

                return CallWdgjServer.CallData<SaleModel>(sql);
            }
            catch
            {
                return null;
            }


        }

        public static List<StatisticsEveryDayForBrandModel> StatisticsEveryDayForBrand(string Date, string DateMax, bool IsKeep)
        {
            string TradeTable = "G_Trade_TradeList";
            string TradeGoodsTable = "G_Trade_GoodsList";
            if (IsKeep)
            {
                TradeTable = "G_Trade_TradeList_His";
                TradeGoodsTable = "G_Trade_GoodsList_His";
            }

            DateTime dt = DateTime.Parse(Date);
            DateTime dtend = DateTime.Parse(DateMax);
            //            string Sql = string.Format(@"select ShopID, Goods,SUM(tb.price) as totalprice ,CONVERT(int,SUM(tb.SellCount)) as totalcount ,(SUM(tb.price)/SUM(tb.SellCount)) as avgprice from 
            //(select tba.ShopID, tba.Goods,(tba.SellTotal-tbb.post*tba.SellCount) as price ,tba.SellCount from (
            //select b.ShopID, b.TradeNO,b.PostageTotal,c.GoodsNO, SUBSTRING(c.goodsno,1,2)+DBO.GET_STR(c.GoodsNO) as Goods,a.SellCount,a.SellTotal from {2} a join {3} b on a.TradeID = b.TradeID
            //join G_Goods_GoodsList c on a.GoodsID = c.GoodsID
            //where b.TradeTime>='{0}' and b.TradeTime<'{1}' and b.tradestatus<>0 ) tba
            //join 
            //
            //--订单平均邮费
            //(select b.TradeNO,(b.PostageTotal/ sum(SellCount)) as post from {2} a 
            //join {3} b on  a.TradeID = b.TradeID 
            //where  b.TradeTime>='{0}'  and b.TradeTime<'{1}' and b.tradestatus<>0
            //group by b.TradeID,b.TradeNO,b.PostageTotal)
            //tbb on tba.TradeNO = tbb.TradeNO) as tb
            //group by Goods,ShopID", dt, dt.AddDays(1), TradeGoodsTable, TradeTable);

            string Sql = string.Format(@" select Convert(datetime, TradeTime) as TradeTime,shopid,[Year],Season,Brand ,SUM(Price) as TotalPrice,CONVERT(int, SUM(Sellcount)) as Totalcount from 
            (select tba.*,(tba.SellTotal -tba.Sellcount*tbb.AvgPostPrice) as Price from (
            select  CONVERT(varchar(100), TradeTime, 23) as TradeTime, b.ShopID, b.TradeNO,b.PostageTotal,c.GoodsNO, SUBSTRING(c.goodsno,1,2)+DBO.GET_STR(c.GoodsNO) as Goods,a.SellCount,a.SellTotal,
            ('1'+SUBSTRING(goodsno,1,1)) as [Year],
            case SUBSTRING(goodsno,2,1) 
             when '5' then '1'  
             when '6' then '2'  
             when '7' then '3'  
             when '8' then '4'  
             else SUBSTRING(goodsno,2,1) end as Season  , DBO.GET_STR(GoodsNO) as Brand
             from {2} a join {3} b on a.TradeID = b.TradeID
            join G_Goods_GoodsList c on a.GoodsID = c.GoodsID
            where b.TradeTime>='{0}' and b.TradeTime<'{1}' and b.tradestatus<>0  and a.SellTotal>0) tba
            join 

            --订单平均邮费
            (select b.TradeNO,(b.PostageTotal/ sum(SellCount)) as AvgPostPrice from {2} a 
            join {3} b on  a.TradeID = b.TradeID 
            where b.TradeTime>='{0}' and b.TradeTime<'{1}' and b.tradestatus<>0 and a.SellTotal>0
            group by b.TradeNO,b.PostageTotal)
            tbb on tba.TradeNO = tbb.TradeNO) as tb
            group by TradeTime,shopid,Year,Season,Brand
            order by TradeTime,Brand", dt, dtend, TradeGoodsTable, TradeTable);

            return CallWdgjServer.CallData<StatisticsEveryDayForBrandModel>(Sql);

        }



        public static List<ZBMSModel> GetZBMSSpecName(string date)
        {
            string sql = string.Format(@"select SpecName from dbo.G_Goods_GoodsSpec as a join (

	        select GoodsID,SpecID from G_Trade_Goodslist where tradeId in(
	
	        select TradeId from G_Trade_TradeList where (tradeStatus=5 OR tradeStatus=11) and Convert(varchar(10),tradeTime,121)=Convert(varchar(10),'{0}',121)
	
	        ) AND GoodsID=96565
          ) as b on a.GoodsId=b.GoodsID and a.SpecID=b.SpecID order by Convert(int, Replace(SpecName,'号','')) asc", date);

            return CallWdgjServer.CallData<ZBMSModel>(sql);
        }




    }
}
