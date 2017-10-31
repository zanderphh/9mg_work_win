using _9M.Work.Model;
using _9Mg.Work.JosApi;
using JdSdk;
using JdSdk.Domain.Ware;
using JdSdk.Request.Ware;
using JdSdk.Response.Ware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _9M.Work.JosApi
{

    public class WareManager
    {

        #region 变量和构造方法
        private string url;
        private string appkey;
        private string appsecret;
        private string sessionKey;
        private IJdClient client;
        public WareManager()
        {
            appkey = Config.Appkey;
            appsecret = Config.Secret;
            sessionKey = Config.SessionKey;
            url = Config.Url;
        }
        public WareManager(string appkey, string appsecret, string sessionKey)
        {
            this.appkey = appkey;
            this.appsecret = appsecret;
            url = Config.Url;
            this.sessionKey = sessionKey;
        }
        #endregion

        #region 查询方法
        /// <summary>
        /// 查询上下架的商品
        /// </summary>
        /// <param name="status"></param>
        /// <param name="Cid"></param>
        /// <param name="TitleKewrod"></param>
        /// <param name="Field"></param>
        /// <returns></returns>
        public List<Ware> GetWares(GoodsStatus status, string Cid, string TitleKewrod, string Field)
        {
            client = new DefaultJdClient(url, appkey, appsecret, sessionKey);
            WareListingGetRequest Upreq = new WareListingGetRequest();
            WareDelistingGetRequest Downreq = new WareDelistingGetRequest();
            List<Ware> itemlist = new List<Ware>();
            if (!string.IsNullOrEmpty(Cid))
            {
                Upreq.Cid = Cid;
                Downreq.Cid = Cid;
            }
            if (!string.IsNullOrEmpty(Field))
            {
                Upreq.Fields = Field;
                Downreq.Fields = Field;
            }
            int type = (int)status;
            int page = 0;    //初始页码值
            int totalpage = 0; //总页码数
            switch (type)
            {
                case 1:  //查询出售中
                    while (true)
                    {
                        page++;
                        Upreq.PageSize = "100";
                        Upreq.Page = page.ToString();
                        WareListingGetResponse response = client.Execute(Upreq);

                        if (page == 1) //如果是第一页。那么开始计算总页码数
                        {
                            double total = Convert.ToDouble(response.Total);
                            if (total == 0)
                            {
                                break;
                            }
                            totalpage = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(total / 100)));
                        }
                        itemlist.AddRange(response.WareInfos);
                        if (page == totalpage)
                        {
                            break;
                        }
                    }
                    break;
                case 2:  //仓库中
                    //指定仓库中商品的状态集合.集合为所有状态就是所有
                    while (true)
                    {
                        page++;
                        Downreq.PageSize = "100";
                        Downreq.Page = page.ToString();
                        WareDelistingGetResponse response = client.Execute(Downreq);

                        if (page == 1) //如果是第一页。那么开始计算总页码数
                        {
                            double total = Convert.ToDouble(response.Total);
                            if (total == 0)
                            {
                                break;
                            }
                            totalpage = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(total / 100)));
                        }
                        itemlist.AddRange(response.WareInfos);
                        if (page == totalpage)
                        {
                            break;
                        }
                    }
                    break;
                case 3:  //查询所有
                    List<Ware> onsalelist = GetWares(GoodsStatus.Onsell, Cid, TitleKewrod, Field);
                    List<Ware> inventlist = GetWares(GoodsStatus.InStock, Cid, TitleKewrod, Field);
                    itemlist.AddRange(onsalelist);
                    itemlist.AddRange(inventlist);
                    break;
            }
            //字符串筛选标题
            if (!string.IsNullOrEmpty(TitleKewrod))
            {
                for (int i = itemlist.Count - 1; i >= 0; i--)
                {
                    if (!itemlist[i].Title.Contains(TitleKewrod))
                    {
                        itemlist.RemoveAt(i);
                    }
                }
            }
            return itemlist;
        }

        
        //WareStatus   1:在售;2:待售 
        /// <summary>
        /// 查询所有商品。错误就直接返回空
        /// </summary>
        /// <param name="ItemNum"></param>
        /// <param name="Title"></param>
        /// <param name="Cid"></param>
        /// <param name="StartPrice"></param>
        /// <param name="EndPrice"></param>
        /// <param name="WareStatus"></param>
        /// <param name="Fields"></param>
        /// <returns></returns>
        public List<Ware> SearchWares(string ItemNum, string Title, string Cid, string StartPrice, string EndPrice, string WareStatus, string Fields)
        {
            List<Ware> itemlist = new List<Ware>();
            client = new DefaultJdClient(url, appkey, appsecret, sessionKey);
            WareInfoByInfoRequest req = new WareInfoByInfoRequest();
            req.Cid = !string.IsNullOrEmpty(Cid) ? Cid : null;
            req.StartPrice = !string.IsNullOrEmpty(StartPrice) ? StartPrice : null;
            req.EndPrice = !string.IsNullOrEmpty(EndPrice) ? EndPrice : null;
            req.ItemNum = !string.IsNullOrEmpty(ItemNum) ? ItemNum : null;
            req.WareStatus = !string.IsNullOrEmpty(WareStatus) ? WareStatus : null;
            req.Fields = !string.IsNullOrEmpty(Fields) ? Fields : null;
            req.Title = !string.IsNullOrEmpty(Title) ? Title : null;
            int page = 0;    //初始页码值
            int totalpage = 0; //总页码数
            while (true)
            {
                page++;
                req.PageSize = "100";
                req.Page = page.ToString();
                WareInfoByInfoSearchResponse response = client.Execute(req);
                if (response.IsError)
                {
                    return null;
                }
                if (page == 1) //如果是第一页。那么开始计算总页码数
                {
                    double total = Convert.ToDouble(response.Total);
                    if (total == 0)
                    {
                        break;
                    }
                    totalpage = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(total / 100)));
                }
                itemlist.AddRange(response.WareInfos);
                if (page == totalpage)
                {
                    break;
                }
            }
            return itemlist;
        }
        /// <summary>
        /// 根据ID查询单个商品
        /// </summary>
        /// <param name="WareId"></param>
        /// <param name="Fields"></param>
        /// <returns></returns>
        public Ware GetWareByID(string WareId, string Fields)
        {
            client = new DefaultJdClient(url, appkey, appsecret, sessionKey);
            WareGetRequest req = new WareGetRequest();
            req.WareId = WareId;
            if (!string.IsNullOrEmpty(Fields))
            {
                req.Fields = Fields;
            }
            WareGetResponse response = client.Execute(req);
            return response.Ware;
        }

        /// <summary>
        /// ID集合查询SKU
        /// </summary>
        /// <param name="wareidlist"></param>
        /// <param name="Fileds"></param>
        /// <param name="errorreturn"></param>
        /// <returns></returns>
        public List<Sku> GetSkuListByID(List<string> wareidlist, string Fileds, bool errorreturn)
        {
            List<string> errorlist = new List<string>();
            List<Sku> itemlist = new List<Sku>();
            client = new DefaultJdClient(url, appkey, appsecret, sessionKey);
            WareSkusGetRequest req = new WareSkusGetRequest();
            if (!string.IsNullOrEmpty(Fileds)) //不为空就追加返回的字段
            {
                req.Fields = Fileds;
            }
            int b = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(Convert.ToDouble(wareidlist.Count) / 10)));
            for (int k = 0; k < b; k++)
            {
                string outer_id = "";
                int flag = (k + 1) * 10;
                if (flag > wareidlist.Count)
                {
                    flag = wareidlist.Count;
                }
                for (int j = k * 10; j < flag; j++)
                {
                    outer_id += wareidlist[j] + ",";
                }
                outer_id = outer_id.TrimEnd(',');
                req.WareIds = outer_id;
                WareSkusGetResponse response = client.Execute(req);
                if (!response.IsError)
                {
                    itemlist.AddRange(response.Skus);
                }
                else
                {
                    errorlist.Add(outer_id);
                }
            }
            int trycount = 0;
            while (errorlist.Count > 0)
            {
                if (trycount == 10)
                {
                    if (errorreturn == false)
                    {
                        itemlist = null;
                    }
                    break;
                }
                for (int i = errorlist.Count - 1; i >= 0; i--)
                {
                    req.WareIds = errorlist[i];
                    if (!string.IsNullOrEmpty(Fileds)) //不为空就追加返回的字段
                    {
                        req.Fields = Fileds;
                    }
                    WareSkusGetResponse response = client.Execute(req);
                    if (!response.IsError)
                    {
                        itemlist.AddRange(response.Skus);
                        errorlist.RemoveAt(i);
                    }
                    else
                    {
                        trycount++;
                    }
                }
            }
            return itemlist;
        }

        /// <summary>
        /// 根据商品返回ID集合
        /// </summary>
        /// <param name="GoodsNoList"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetGoodsNoSkuID(List<string> GoodsNoList)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            //查询所有商品
            List<Ware> idlist = SearchWares(null, null, null, null, null, null, null);
            //得到所有款号查询SKU
            List<string> wareidlist = idlist.Where(x => GoodsNoList.Contains(x.ItemNum)).Select(y => y.WareId.ToString()).ToList();
            List<Sku> skulist = GetSkuListByID(wareidlist, null, false);
            //组装对象
            foreach (string s in GoodsNoList)
            {
                //寻找款
                Ware ware = idlist.Find(x =>
                {
                    return x.ItemNum.Equals(s, StringComparison.CurrentCultureIgnoreCase);
                });
                if (ware != null)
                {
                    //寻找SKU组成出售链接
                    Sku sk = skulist.Find(y =>
                    {
                        return y.WareId == ware.WareId;
                    });
                    if (sk != null)
                    {
                        dic.Add(s, sk.SkuId.ToString());
                    }
                }
            }
            return dic;
        }
        #endregion


        #region 修改方法
        /// <summary>
        /// 修改一个SKU
        /// </summary>
        /// <param name="SkuID"></param>
        /// <param name="OuterId"></param>
        /// <param name="Quantity"></param>
        /// <returns></returns>
        public string UpdateStock(string SkuID, string OuterId, string Quantity)
        {
            string Res = "success";
            client = new DefaultJdClient(url, appkey, appsecret, sessionKey);
            WareSkuStockUpdateRequest request = new WareSkuStockUpdateRequest();
            request.SkuId = SkuID;
            request.OuterId = OuterId;
            request.Quantity = Quantity;
            try
            {
                WareSkuStockUpdateResponse response = client.Execute(request);
                if (response.IsError)
                {
                    Res = response.ZhErrMsg;
                }
            }
            catch (Exception ex)
            {
                Res = ex.Message;
            }
            return Res;
        }

        public string UpdatePrice(string WareId, string JdPrice, string MarketPrice,string SkuPrices, string SkuPropers, string SkuStocks)
        {
            string Res = "success";
            client = new DefaultJdClient(url, appkey, appsecret, sessionKey);
            WareUpdateRequest req = new WareUpdateRequest();
            req.WareId = WareId;
            req.JdPrice = JdPrice;
            req.MarketPrice = MarketPrice;
           // req.OuterId = SkuOuterIds;
            req.SkuProperties = SkuPropers;
            req.SkuStocks = SkuStocks;
            req.SkuPrices = SkuPrices;
            try
            {
                WareUpdateResponse response = client.Execute(req);
                if (response.IsError)
                {
                    Res = response.ZhErrMsg;
                }
            }
            catch (Exception ex)
            {
                Res = ex.Message;
            }
            return Res;
        }

        public string UpdateSkuPrice(string SkuId, string Price)
        {
            string Res = "success";
            client = new DefaultJdClient(url, appkey, appsecret, sessionKey);

            WareSkuPriceUpdateRequest req = new WareSkuPriceUpdateRequest();
            req.SkuId = SkuId;
            req.Price = Price;
            req.MarketPrice = Price;
            try
            {
                WareSkuPriceUpdateResponse response = client.Execute(req);
                if (response.IsError)
                {
                    Res = response.ZhErrMsg;
                }
            }
            catch (Exception ex)
            {
                Res = ex.Message;
            }

            return Res;
        }
        #endregion
    }
}
