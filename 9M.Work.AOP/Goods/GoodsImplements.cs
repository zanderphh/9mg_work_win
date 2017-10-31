using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _9M.Work.Model;
using _9M.Work.Model.Log;
using _9M.Work.Utility;
using Newtonsoft.Json;

namespace _9M.Work.AOP.Goods
{
    public class GoodsImplements : GoodsInterface
    {
        public static string JuShiTaRequestKey
        {
            get
            {
                return "0X00E045443B12BC";
            }
        }

        public static string JsShiTaUrl
        {
            get
            {
                return "http://my.9mg.cc/new/api";
                //return "http://localhost:11744/api";
            }
        }
        public void DoWork(IntercepGoodsLogModel LogModel)
        {
            throw new NotImplementedException();
        }

        public bool SubmitTrade(List<PostSkuModel> list, IntercepGoodsLogModel LogModel)
        {
            HttpHelper http = new HttpHelper(HttpHelper.WinRestUrL("", ""));
            string data = JsonConvert.SerializeObject(list);
            string res = http.Post(data, string.Format("{1}/StockApi/SavePositionTran?RequestKey={0}", JuShiTaRequestKey, JsShiTaUrl));
            return res.ToUpper().Equals("TRUE");
        }
    }
}
