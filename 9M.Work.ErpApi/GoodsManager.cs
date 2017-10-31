using _9M.WebReqeust;
using _9M.Work.Model;
using _9M.Work.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _9M.Work.ErpApi
{
    public class GoodsManager
    {
        public string RequestKey = CommonConfig.JuShiTaRequestKey;
        string url = "http://112.124.16.76/Wdgj/WdgjService.asmx";

        /// <summary>
        /// 得到一个款号的多规格
        /// </summary>
        /// <param name="goodsno"></param>
        /// <returns></returns>
        public List<SpecModel> SpecList(string goodsno)
        {
            #region wl-0411
            ////HttpHelper http = new HttpHelper(HttpHelper.WinRestUrL("", ""));
            ////string data = string.Format("CommodityApi/SpecList?RequestKey={0}&GoodsNo={1}", RequestKey, goodsno);
            ////String temp = http.Get(data);
            ////List<SpecModel> modellist = (List<SpecModel>)JsonConvert.DeserializeObject(temp, typeof(List<SpecModel>));
            ////modellist = modellist.OrderBy(a => a.SpecName1).ThenBy(a => a.SpecCode).ToList();
            ////return modellist;
            #endregion

            return WdgjSource.SpecList(goodsno);
        }

        public GoodsModel GoodsInformation(string goodsno)
        {
            #region wl-0411

            //HttpHelper http = new HttpHelper(HttpHelper.WinRestUrL("", ""));
            //string data = string.Format("CommodityApi/GoodsInformation?RequestKey={0}&GoodsNo={1}", RequestKey, goodsno);
            //string temp = http.Get(data);
            //GoodsModel model = (GoodsModel)JsonConvert.DeserializeObject(temp, typeof(GoodsModel));
            //return model;

            #endregion

            return WdgjSource.GoodsInformation(goodsno);
        }

        /// <summary>
        /// 返回淘宝原始订单号
        /// </summary>
        /// <param name="InputNo"></param>
        /// <returns></returns>
        public List<Erp_Model_GJRefund> GetTBSourceTradeNo(string InputNo)
        {
            #region wl-0411
            //HttpHelper http = new HttpHelper(HttpHelper.WinRestUrL("", ""));
            //string data = string.Format("CommodityApi/GetTBTradeNo?RequestKey={0}&InputNo={1}", RequestKey, InputNo);
            //string temp = http.Get(data);
            //List<Erp_Model_GJRefund> list = (List<Erp_Model_GJRefund>)JsonConvert.DeserializeObject(temp, typeof(List<Erp_Model_GJRefund>));
            //return list;
            #endregion

            return WdgjSource.GetTBSourceTradeNo(InputNo);
        }



        /// <summary>
        /// 订单中的商品
        /// </summary>
        /// <param name="InputNo"></param>
        /// <returns></returns>
        public List<Erp_Model__GJTradeGoods> GetGJTradeGoods(string TBTrade)
        {
            #region wl-0411
            //HttpHelper http = new HttpHelper(HttpHelper.WinRestUrL("", ""));
            //string data = string.Format("CommodityApi/GetGjTradeGoods?RequestKey={0}&TBTradeno={1}", RequestKey, TBTrade);
            //string temp = http.Get(data);
            //List<Erp_Model__GJTradeGoods> list = (List<Erp_Model__GJTradeGoods>)JsonConvert.DeserializeObject(temp, typeof(List<Erp_Model__GJTradeGoods>));
            //return list;
            #endregion

            return WdgjSource.GetGjTradeGoods(TBTrade);
        }

        /// <summary>
        /// 得到一个带附加码款号的信息
        /// </summary>
        /// <param name="goodsno">款号</param>
        /// <param name="type"></param>
        /// <returns></returns>
        public GoodsModel GetGoodsAll(string goodsno)
        {
            #region wl-0411
            //HttpHelper http = new HttpHelper(HttpHelper.WinRestUrL("", ""));
            //string data = string.Format("CommodityApi/GoodsDetail?RequestKey={0}&GoodsDetail={1}", RequestKey, goodsno);
            //string temp = http.Get(data);
            //GoodsModel model = (GoodsModel)JsonConvert.DeserializeObject(temp, typeof(GoodsModel));
            //if (model != null)
            //{
            //    model.GoodsNo = model.GoodsNo.ToUpper();
            //}
            //return model;

            #endregion

            return WdgjSource.GoodsDetail(goodsno);
        }

        public List<ApiSpecModel> GetSpecListByGoodsNos(string GoodsNos)
        {
            #region wl-0411
            //HttpHelper http = new HttpHelper(HttpHelper.WinRestUrL("", ""));
            //string data = string.Format("JzGoodsApi/GoodsByGoodsIds?RequestKey={0}&Ids={1}", RequestKey, GoodsNos);
            //string temp = http.Get(data);
            //List<ApiSpecModel> model = (List<ApiSpecModel>)JsonConvert.DeserializeObject(temp, typeof(List<ApiSpecModel>));
            //return model;
            #endregion

            return WdgjSource.GetSpecListByGoodsNos(GoodsNos.Split(',').ToList());
        }

        /// <summary>
        /// 返回有库存的商品基本信息
        /// </summary>
        /// <param name="GoodsNos"></param>
        /// <returns></returns>
        public List<ApiTopicGoodsModel> GetGoodsByHasStock(string GoodsNos)
        {
            #region wl-0411
            //HttpHelper http = new HttpHelper();
            //string data = JsonConvert.SerializeObject(GoodsNos);
            //string res = http.Post(data, string.Format("{1}/TopicApi/GetTopicGoodsOnly?RequestKey={0}", RequestKey, CommonConfig.JsShiTaUrl));
            //List<ApiTopicGoodsModel> modellist = (List<ApiTopicGoodsModel>)JsonConvert.DeserializeObject(res, typeof(List<ApiTopicGoodsModel>));
            //return modellist;
            #endregion

            return WdgjSource.GetGoodsByHasStock(GoodsNos);
        }

        /// <summary>
        /// 根椐入库单单号来查询所有商品信息
        /// </summary>
        /// <param name="BillDID"></param>
        /// <returns></returns>
        public List<WareHouseInModel> WareHouseInList(string BillDID)
        {
            #region wl-0411
            //HttpHelper http = new HttpHelper(HttpHelper.WinRestUrL("", ""));
            //string data = string.Format("StockApi/ByBillDID?RequestKey={0}&BillDID={1}", RequestKey, BillDID);
            //String temp = http.Get(data);
            //List<WareHouseInModel> modellist = (List<WareHouseInModel>)JsonConvert.DeserializeObject(temp, typeof(List<WareHouseInModel>));
            //return modellist;
            #endregion

            return WdgjSource.ByBillDID(BillDID);
        }

        /// <summary>
        /// 保存入库单
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public bool SubmitTrade(List<PostSkuModel> list)
        {

            #region wl-0411
            //HttpHelper http = new HttpHelper(HttpHelper.WinRestUrL("", ""));
            //string data = JsonConvert.SerializeObject(list);
            //string res = http.Post(data, string.Format("{1}/StockApi/SavePositionTran?RequestKey={0}", RequestKey, CommonConfig.JsShiTaUrl));
            //return res.ToUpper().Equals("TRUE");
            #endregion

            return WdgjSource.SavePositionTran(list);
        }


        public bool UpdateGoods(List<PostUpdateCommodityModel> list)
        {
            #region wl-0411
            //HttpHelper http = new HttpHelper(HttpHelper.WinRestUrL("", ""));
            //string data = JsonConvert.SerializeObject(list);
            //string res = http.Post(data, string.Format("{1}/CommodityApi/UpdateGoods?RequestKey={0}", RequestKey, CommonConfig.JsShiTaUrl));
            //return res.ToUpper().Equals("TRUE");
            #endregion

            return WdgjSource.UpdateGoods(list);
        }
        /// <summary>
        /// 获取订单货品
        /// </summary>
        /// <param name="TradeNo"></param>
        /// <returns></returns>
        public List<GoodsModel> GoodsByTrade(string TradeNo)
        {
            #region wl-0411
            //HttpHelper http = new HttpHelper(HttpHelper.WinRestUrL("", ""));
            //string data = string.Format("CommodityApi/GoodsByTrade?RequestKey={0}&TradeNo={1}", RequestKey, TradeNo);
            //string temp = http.Get(data);
            //List<GoodsModel> modellist = (List<GoodsModel>)JsonConvert.DeserializeObject(temp, typeof(List<GoodsModel>));
            //return modellist;
            #endregion

            return WdgjSource.GoodsByTrade(TradeNo);
        }


        public DataTable TableForSql(string Sql)
        {
            //HttpHelper Http = new HttpHelper("http://my.9mg.cc/new/api");
            //string Purl = string.Format("CommodityApi/RetrueDataForSql?RequestKey=0X00E045443B12BC&Sql={0}", Sql);
            //string Presponse = Http.Get(Purl);
            //DataTable gt = JsonConvert.DeserializeObject<DataTable>(Presponse);
            //return gt;

            return new DataTable();
        }

        public string JsonForSql(string Sql)
        {
            //HttpHelper Http = new HttpHelper("http://my.9mg.cc/new/api");
            //string Purl = string.Format("CommodityApi/RetrueDataForSql?RequestKey=0X00E045443B12BC&Sql={0}", Sql);
            //string Presponse = Http.Get(Purl);
            //return Presponse;

            return null;
        }


        #region
        //public List<SpecModel> GetGoodsDetail(string GoodsNo)
        //{
        //    List<SpecModel> speclist = new List<SpecModel>();

        //    object[] arg = new object[2];
        //    GoodsGetRequest req = new GoodsGetRequest();
        //    req.Fields = "ClassModel, SpecList,StockList";
        //    req.GoodsNo = GoodsNo;
        //    arg[0] = req.GetApiName().Replace(".", "");
        //    arg[1] = JsonConvert.SerializeObject(req);
        //    object result = HttpHelper.InvokeWebService(url, "CallWdgjServer", arg);
        //    List<_9M.Model.GoodsModel> ist = JsonConvert.DeserializeObject<List<_9M.Model.GoodsModel>>(result.ToString());
        //    if (ist.Count > 0)
        //    {
        //        _9M.Model.GoodsModel model = ist[0];
        //        var splist = model.SpecList;
        //        var stlist = model.StockList;
        //        splist.ForEach(x =>
        //        {
        //            var s = stlist.Where(y => y.SpecID == x.SpecID).FirstOrDefault();
        //            SpecModel sm = new SpecModel()
        //            {
        //                GoodsName = model.GoodsName,
        //                GoodsNo = model.GoodsNO,
        //                OrderCount = s == null ? 0 : Convert.ToInt32(s.OrderCount),
        //                SndCount = s == null ? 0 : Convert.ToInt32(s.SndCount),
        //                SpecCode = x.SpecCode,
        //                SpecName = x.SpecName,
        //                SpecName1 = x.SpecName1,
        //                SpecName2 = x.SpecName2,
        //                Stock = s == null ? 0 : Convert.ToInt32(s.Stock),
        //                Postion = s == null ? "" : s.PositionRemark,
        //            };
        //            speclist.Add(sm);
        //        });
        //        return speclist;
        //    }
        //    return null;
        //}
        #endregion
    }
}
