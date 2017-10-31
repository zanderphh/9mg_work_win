using _9M.Work.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Top.Api.Domain;

namespace _9M.Work.ErpApi
{
    public class RefundManager
    {
        //public static string RequestKey = CommonConfig.JuShiTaRequestKey;

        ///// <summary>
        ///// 获取快递信息
        ///// </summary>
        ///// <param name="kvpList"></param>
        ///// <returns></returns>
        //public static List<Refund> GetRefundExpressInfo(List<KeyValuePair<string, long>> kvpList)
        //{

        //    HttpHelper http = new HttpHelper();
        //    string data = JsonConvert.SerializeObject(kvpList);
        //    string res = http.Post(data, string.Format("{1}/TopicApi/GetRefundExpressInfo?RequestKey={0}", RequestKey, CommonConfig.JsShiTaUrl));
        //    List<Refund> list = (List<Refund>)JsonConvert.DeserializeObject(res, typeof(List<Refund>));
        //    return list;
        //}

        ///// <summary>
        ///// 匹配订单
        ///// </summary>
        ///// <param name="tradeCollection"></param>
        ///// <param name="ShopId"></param>
        ///// <returns></returns>
        //public static List<Trade> GetTradeInfoByCShopOrTmall(List<string> tradeCollection, int ShopId)
        //{
        //    HttpHelper http = new HttpHelper();
        //    string data = JsonConvert.SerializeObject(tradeCollection);
        //    string res = http.Post(data, string.Format("{1}/TopicApi/GetTradeInfoByCShopOrTmall?RequestKey={0}&ShopId={2}", RequestKey, CommonConfig.JsShiTaUrl, ShopId));
        //    List<Trade> list = (List<Trade>)JsonConvert.DeserializeObject(res, typeof(List<Trade>));
        //    return list;
        //}

        //public static List<Object> GetRefundDetailByNick(string buyer_nick, int ShopId)
        //{
        //    HttpHelper http = new HttpHelper(CommonConfig.JsShiTaUrl);
        //    string res = http.Get(string.Format("/TopicApi/GetRefundDetailByNick?RequestKey={0}&buyer_nick={1}&ShopId={2}", RequestKey, buyer_nick, ShopId));
        //    List<Object> list = (List<Object>)JsonConvert.DeserializeObject(res, typeof(List<Object>));
        //    return list;
        //}
    }
}
