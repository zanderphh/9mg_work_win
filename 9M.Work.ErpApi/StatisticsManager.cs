using _9M.Work.Model;
using _9M.Work.Model.Statistics;
using _9M.Work.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _9M.Work.ErpApi
{
    public class StatisticsManager
    {
        public string RequestKey = CommonConfig.JuShiTaRequestKey;
        /// <summary>
        /// 销售统计
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public List<SaleModel> Statistics(SalePostModel model)
        {
            #region wl 2017-05-26 替换下列方法

            //HttpHelper http = new HttpHelper();
            //string data = JsonConvert.SerializeObject(model);
            //string res = http.Post(data, string.Format("{1}/StatisticsApi/Statist?RequestKey={0}", RequestKey, CommonConfig.JsShiTaUrl));
            //List<SaleModel> modellist = (List<SaleModel>)JsonConvert.DeserializeObject(res, typeof(List<SaleModel>));

            #endregion

            List<SaleModel> modellist = WdgjSource.Statist(model);

            return modellist;
        }

        #region wl 2017-05-26 屏蔽
        /// <summary>
        /// 得到指定条件的订单集合
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        //public FenXiaoTradeReslutModel FenXiaoTrade(FenXiaoTradeQueryModel model)
        //{
        //    HttpHelper http = new HttpHelper();
        //    string data = JsonConvert.SerializeObject(model);
        //    string res = http.Post(data, string.Format("{1}/StatisticsApi/FenXiaoTrade?RequestKey={0}", RequestKey, CommonConfig.JsShiTaUrl));
        //    FenXiaoTradeReslutModel ms = (FenXiaoTradeReslutModel)JsonConvert.DeserializeObject(res, typeof(FenXiaoTradeReslutModel));
        //    return ms;
        //}

        /// <summary>
        /// 得到指定条件的总页数。如果是-1则是遇到了错误
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        //public int GetTradeAllPage(FenXiaoTradeQueryModel model)
        //{

        //    HttpHelper http = new HttpHelper();
        //    string data = JsonConvert.SerializeObject(model);
        //    string res = http.Post(data, string.Format("{1}/StatisticsApi/GetTradeAllPage?RequestKey={0}", RequestKey, CommonConfig.JsShiTaUrl));
        //    int ms = (int)JsonConvert.DeserializeObject(res, typeof(int));
        //    return ms;
        //}
     



        #region 销售统计
        //public List<StatisticsEveryDayForBrandModel> StatisForEveryDayBrand(string Date, string DateMax, bool IsKeep)
        //{
        //    HttpHelper http = new HttpHelper(HttpHelper.WinRestUrL("", ""));
        //    string data = string.Format("StatisticsApi/StatisticsEveryDayForBrand?RequestKey={0}&Date={1}&DateMax={3}&IsKeep={2}", RequestKey, Date, IsKeep, DateMax);
        //    string temp = http.Get(data);
        //    List<StatisticsEveryDayForBrandModel> list = (List<StatisticsEveryDayForBrandModel>)JsonConvert.DeserializeObject(temp, typeof(List<StatisticsEveryDayForBrandModel>));
        //    return list;
        //}

        #endregion  
        
        #endregion
    }
}
