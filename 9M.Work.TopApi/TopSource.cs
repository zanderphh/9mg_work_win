using _9M.Work.Model;
using _9M.Work.Utility;
using _9M.Work.WSVariable;
using _9Mg.Work.TopApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Top.Api;
using Top.Api.Domain;
using Top.Api.Request;
using Top.Api.Response;
using Top.Schema.Factory;
using Top.Schema.Fields;
using Top.Schema.Values;

namespace _9M.Work.TopApi
{
    public class CallTopServer
    {
        public static string url = "http://112.124.16.76/TopService.asmx";
        public static T CallData<T>(string Authorize, BaseTopRequest<T> request) where T : TopResponse
        {
            object[] arg = new object[3];
            arg[0] = Authorize;
            arg[1] = request.GetApiName().Replace(".", "");
            arg[2] = JsonConvert.SerializeObject(request);

            //调用非验证的WS  object result = HttpHelper.InvokeWebService(url, "CallTopServer", arg);


            // update wl by 16-08-24 调用需要验证的WS
            WSSoapHeader header = new WSSoapHeader("WSSoapHeader");
            header.AddProperty("UserName", "9mg");
            header.AddProperty("Password", "www.9mg.cn");
            object result = HttpHelper.InvokeWebService(url, "CallTopServer", arg, header);
            T response = new Top.Api.Parser.TopJsonParser().Parse<T>(result.ToString());
            return response;
        }
    }

    public class TopSource
    {
        public string JuShiTaRequestKey = "0X00E045443B12BC";
        private string DefaultFields = "buyer_nick,title,created,tid,status,payment,pay_time,orders.title,orders.num,orders.status,orders.oid,orders.payment,orders.outer_iid,orders.outer_sku_id,";
        private string DefaultGoodsField = "num_iid,outer_id,title,price"; //默认的查询字段 
        public string Authorize { get; set; }

        //默认C店
        public TopSource()
        {

        }
        private ShopModel shop;
        //店铺授权
        public TopSource(ShopModel shop)
        {
            Authorize = shop.appKey + "," + shop.appSecret + "," + shop.sessionKey;
            this.shop = shop;
        }

        #region 商品
        /// <summary>
        /// 根据款号得到商品集合
        /// </summary>
        /// <param name="goodsnolist"></param>
        /// <param name="OtherFields"></param>
        /// <returns></returns>
        public List<Item> GetItemList(List<string> goodsnolist, string OtherFields)
        {
            List<Item> itemlist = new List<Item>();
            int b = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(goodsnolist.Count) / 40));
            ItemsCustomGetRequest request = new ItemsCustomGetRequest();
            request.Fields = string.IsNullOrEmpty(OtherFields) ? DefaultGoodsField : DefaultGoodsField + "," + OtherFields;
            for (int i = 0; i < b; i++)
            {
                var outerids = string.Join(",", goodsnolist.Skip(40 * i).Take(40).ToArray());
                request.OuterId = outerids;
                ItemsCustomGetResponse data = CallTopServer.CallData(Authorize, request);
                if (data.Items != null)
                {
                    itemlist.AddRange(data.Items);
                }

            }
            return itemlist;
        }

        /// <summary>
        /// 主键NUMIID获取商品SKU
        /// </summary>
        /// <param name="numiids"></param>
        /// <returns></returns>
        public List<Sku> LoadSkuByNumIID(List<string> numiids)
        {
            List<Sku> skulist = new List<Sku>();
            ItemSkusGetRequest request = new ItemSkusGetRequest();
            request.Fields = "sku_spec_id,outer_id,num_iid,quantity,price,properties,sku_id";
            int b = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(Convert.ToDouble(numiids.Count) / 40)));
            for (int i = 0; i < b; i++)
            {
                var ids = string.Join(",", numiids.Skip(40 * i).Take(40).ToArray());
                request.NumIids = ids;
                ItemSkusGetResponse data = CallTopServer.CallData(Authorize, request);
                skulist.AddRange(data.Skus);
            }
            return skulist;
        }

        /// <summary>
        /// 得到单个商品的详情
        /// </summary>
        /// <param name="numiid"></param>
        /// <param name="OtherFields"></param>
        /// <returns></returns>
        public Item GetItemDetail(long numiid, string OtherFields)
        {
            ItemSellerGetRequest request = new ItemSellerGetRequest();
            request.Fields = string.IsNullOrEmpty(OtherFields) ? DefaultGoodsField : DefaultGoodsField + "," + OtherFields;
            request.NumIid = numiid;
            ItemSellerGetResponse data = CallTopServer.CallData(Authorize, request);
            return data.Item;
        }

        /// <summary>
        /// 得到一组商品的详情
        /// </summary>
        /// <param name="numiidlist"></param>
        /// <param name="OtherFields"></param>
        /// <returns></returns>
        public List<Item> GetItemListDetail(List<string> numiidlist, string OtherFields)
        {
            List<Item> itemlist = new List<Item>();
            int b = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(numiidlist.Count) / 20));
            ItemsSellerListGetRequest request = new ItemsSellerListGetRequest();
            request.Fields = string.IsNullOrEmpty(OtherFields) ? DefaultGoodsField : DefaultGoodsField + "," + OtherFields;
            for (int i = 0; i < b; i++)
            {
                var ids = string.Join(",", numiidlist.Skip(20 * i).Take(20).ToArray());
                request.NumIids = ids;
                ItemsSellerListGetResponse data = CallTopServer.CallData(Authorize, request);
                itemlist.AddRange(data.Items);
            }
            return itemlist;
        }

        public string UpdateSkuQuanityByOuterIds(long numiid, string QuantityStr)
        {
            SkusQuantityUpdateRequest req = new SkusQuantityUpdateRequest();
            req.NumIid = numiid;
            req.Type = 1L;
            req.OuteridQuantities = QuantityStr;
            SkusQuantityUpdateResponse response = CallTopServer.CallData(Authorize, req);
            return Config.ReslutValue(response);
        }
        /// <summary>
        /// 出售中的商品
        /// </summary>
        /// <param name="Q">标题关键字</param>
        /// <param name="sellercids">分类字符</param>
        /// <param name="has_discount">会员折扣</param>
        /// <param name="OtherFields">额外的查询内容</param>
        /// <returns></returns>
        public List<Item> OnSaleList(string Q, string sellercids, bool? has_discount, string OtherFields)
        {
            List<Item> itemlist = new List<Item>();
            ItemsOnsaleGetRequest request = new ItemsOnsaleGetRequest();
            request.Q = Q;
            request.SellerCids = sellercids;
            request.HasDiscount = has_discount;
            request.Fields = string.IsNullOrEmpty(OtherFields) ? DefaultGoodsField : DefaultGoodsField + "," + OtherFields;

            request.PageSize = 200;
            request.PageNo = 1;
            //得到总页数
            ItemsOnsaleGetResponse data = CallTopServer.CallData(Authorize, request);
            long total = data.TotalResults;
            int TotalPage = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(total) / 200));
            //得到数据集合
            for (int i = 1; i <= TotalPage; i++)
            {
                request.PageNo = i;
                ItemsOnsaleGetResponse respones = CallTopServer.CallData(Authorize, request);
                itemlist.AddRange(respones.Items);
            }
            return itemlist;
        }


        public List<Sku> SkusByGoodsNo(string GoodsNo)
        {
            SkusCustomGetRequest req = new SkusCustomGetRequest();
            req.OuterId = GoodsNo;
            req.Fields = "sku_id,properties,quantity,outer_id";
            SkusCustomGetResponse rsp = CallTopServer.CallData(Authorize, req);
            return rsp.Skus;
        }

        public bool UpdateCid(long NumIID, string cids)
        {
            ItemUpdateRequest req = new ItemUpdateRequest();
            req.NumIid = NumIID;
            req.SellerCids = cids;
            ItemUpdateResponse Rsp = CallTopServer.CallData(Authorize, req);
            return Rsp.IsError;
        }
        /// <summary>
        /// 仓库中的商品
        /// </summary>
        /// <param name="Q">标题关键字</param>
        /// <param name="Banner">商品在仓库中的状态</param>
        /// <param name="sellercids">分类字符</param>
        /// <param name="has_discount">会员折扣</param>
        /// <param name="OtherFields">额外的查询内容</param>
        /// <returns></returns>
        public List<Item> InventoryList(string Q, string Banner, string sellercids, bool? has_discount, string OtherFields)
        {
            List<Item> itemlist = new List<Item>();
            ItemsInventoryGetRequest request = new ItemsInventoryGetRequest();
            request.Q = Q;
            request.SellerCids = sellercids;
            request.HasDiscount = has_discount;
            request.Fields = string.IsNullOrEmpty(OtherFields) ? DefaultGoodsField : DefaultGoodsField + "," + OtherFields;
            request.PageSize = 200;
            request.PageNo = 1;
            request.Banner = Banner;
            //得到总页数
            ItemsInventoryGetResponse data = CallTopServer.CallData(Authorize, request);
            long total = data.TotalResults;
            int TotalPage = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(total) / 200));
            //得到数据集合
            for (int i = 1; i <= TotalPage; i++)
            {
                request.PageNo = i;
                ItemsInventoryGetResponse respones = CallTopServer.CallData(Authorize, request);
                itemlist.AddRange(respones.Items);
            }
            return itemlist;
        }

        /// <summary>
        /// 删除一个商品
        /// </summary>
        /// <param name="NumIid"></param>
        /// <returns></returns>
        public string DeleteItem(long NumIid)
        {
            ItemDeleteRequest deleterequest = new ItemDeleteRequest();
            deleterequest.NumIid = NumIid;
            ItemDeleteResponse rsp = CallTopServer.CallData(Authorize, deleterequest);
            return Config.ReslutValue(rsp);
        }



        /// <summary>
        /// 下架一个商品
        /// </summary>
        /// <param name="NumIid"></param>
        /// <returns></returns>
        public string DownGoods(long NumIid)
        {
            ItemUpdateDelistingRequest req = new ItemUpdateDelistingRequest();
            req.NumIid = NumIid;
            ItemUpdateDelistingResponse response = CallTopServer.CallData(Authorize, req);
            return Config.ReslutValue(response);
        }

        /// <summary>
        /// 上架一个商品
        /// </summary>
        /// <param name="NumIid"></param>
        /// <returns></returns>
        public string UpGoods(long NumIid)
        {
            ItemUpdateListingRequest req = new ItemUpdateListingRequest();
            req.NumIid = NumIid;
            req.Num = 5L;
            ItemUpdateListingResponse response = CallTopServer.CallData(Authorize, req);
            return Config.ReslutValue(response);
        }

        /// <summary>
        /// 更新一个商品的价格(改价接口)
        /// </summary>
        /// <param name="NumIid"></param>
        /// <param name="SkuOuterIds"></param>
        /// <param name="SkuProps"></param>
        /// <param name="SkuPrices"></param>
        /// <param name="SkuQuantitys"></param>
        /// <returns></returns>
        public string UpdateItemPrice(long NumIid, string Price, string SkuProps, string SkuPrices, string SkuQuantitys, string SkuOuterIds)
        {
            ItemPriceUpdateRequest req = new ItemPriceUpdateRequest();
            req.NumIid = NumIid;
            req.Price = Price;
            req.SkuQuantities = SkuQuantitys;
            req.SkuOuterIds = SkuOuterIds;
            req.SkuProperties = SkuProps;
            req.SkuPrices = SkuPrices;
            ItemPriceUpdateResponse rsp = CallTopServer.CallData(Authorize, req);
            return Config.ReslutValue(rsp);
        }

        /// <summary>
        /// 修改商品的价格
        /// </summary>
        /// <param name="numiid"></param>
        /// <param name="price"></param>
        /// <param name="sku_properties"></param>
        /// <param name="sku_price"></param>
        /// <param name="sku_quantities"></param>
        /// <param name="sku_outerids"></param>
        /// <returns></returns>
        public string Update_ItemPrice(long numiid, string price, string sku_properties, string sku_price, string sku_quantities, string sku_outerids)
        {

            ItemUpdateRequest req = new ItemUpdateRequest();
            req.NumIid = numiid;
            req.Price = price;
            req.SkuQuantities = sku_quantities;
            req.SkuOuterIds = sku_outerids;
            req.SkuProperties = sku_properties;
            req.SkuPrices = sku_price;
            ItemUpdateResponse respones = CallTopServer.CallData(Authorize, req);
            return Config.ReslutValue(respones);
        }

        public string UpdateSellPoint(long numiid, string sellpoint)
        {

            ItemUpdateRequest req = new ItemUpdateRequest();
            req.NumIid = numiid;
            req.SellPoint = sellpoint;
            ItemUpdateResponse respones = CallTopServer.CallData(Authorize, req);
            return Config.ReslutValue(respones);
        }

        public string UpdateTitle(long numiid, string Title)
        {

            ItemUpdateRequest req = new ItemUpdateRequest();
            req.NumIid = numiid;
            req.Title = Title;
            ItemUpdateResponse respones = CallTopServer.CallData(Authorize, req);
            return Config.ReslutValue(respones);
        }

        /// <summary>
        /// 修改一个商品
        /// </summary>
        /// <param name="item"></param>
        /// <param name="model">条件</param>
        /// <param name="tradelist"></param>
        /// <returns></returns>
        public string UpdateItemByModel(Item item, UpdateGoodsSub model, List<NoPayModel> tradelist)
        {
            string Ainostockmsg = "success";
            ItemUpdateRequest req = new ItemUpdateRequest();
            req.NumIid = item.NumIid;
            #region 库存价格(SKU)
            if (model.SyncStock || model.SyncPrice) //如果勾选了库存刷新
            {
                List<SpecModel> list = new List<SpecModel>();
                //得到管家库存
                if (model.SyncStock)
                {

                    #region wl 2017-05-26 替换下列方法
                    //HttpHelper http = new HttpHelper(HttpHelper.WinRestUrL("", ""));
                    //string data = string.Format("CommodityApi/SpecList?RequestKey={0}&GoodsNo={1}", JuShiTaRequestKey, item.OuterId);
                    //String temp = http.Get(data);
                    //List<SpecModel> modellist = (List<SpecModel>)JsonConvert.DeserializeObject(temp, typeof(List<SpecModel>));
                    #endregion

                    List<SpecModel> modellist = _9M.Work.ErpApi.WdgjSource.GetSpecListByGoodsNo(item.OuterId);


                    list = modellist.OrderBy(a => a.SpecName1).ThenBy(a => a.SpecCode).ToList();
                }
                //得到差价
                decimal Price_Chage = Convert.ToDecimal(item.Price) - Convert.ToDecimal(model.Price);
                string price = model.Price;
                string skuprops = string.Empty;
                string sku_quantitys = string.Empty;
                string sku_outerids = string.Empty;
                string sku_price = string.Empty;
                List<Sku> skulist = item.Skus;
                //对比库存
                foreach (Sku sku in skulist)
                {
                    long quantity = sku.Quantity;
                    SpecModel ms = list.Find(x =>
                    {
                        return (x.GoodsNo + x.SpecCode).Equals(sku.OuterId, StringComparison.CurrentCultureIgnoreCase);
                    });
                    if (ms != null)
                    {
                        //减掉未付款
                        NoPayModel pay = tradelist.Find(z =>
                        {
                            return z.OuterSkuId.Equals(sku.OuterId, StringComparison.CurrentCultureIgnoreCase);
                        });
                        long Num = pay != null ? pay.Num : 0;

                        if (sku.Quantity != ms.Stock - Num)
                        {
                            if (model.SyncPrice == false)
                            {
                                skuprops += sku.Properties + ",";
                                quantity = ms.Stock - Num;
                                quantity = quantity < 0 ? 0 : quantity;
                                sku_quantitys += quantity + ",";
                                sku_outerids += sku.OuterId + ",";
                                sku_price += sku.Price + ",";
                            }
                        }
                    }
                    if (model.SyncPrice == true) //统一SKU数量（如果有改价的条件。那么所有的SKU参数都加上）
                    {
                        skuprops += sku.Properties + ",";
                        sku_quantitys += quantity + ",";
                        sku_outerids += sku.OuterId + ",";
                        decimal lastprice = Convert.ToDecimal(sku.Price) - Price_Chage;
                        sku_price += lastprice + ",";
                    }
                }

                if (model.SyncPrice)
                {
                    req.Price = price;
                }

                req.SkuQuantities = sku_quantitys.TrimEnd(',');
                req.SkuOuterIds = sku_outerids.TrimEnd(',');
                req.SkuProperties = skuprops.TrimEnd(',');
                req.SkuPrices = sku_price.TrimEnd(',');
                if (shop.shopId == 1027) //如果是AINO。库存单独的刷新
                {

                    string QuantityStr = string.Empty;
                    string[] arry = req.SkuOuterIds.Split(',');
                    string[] arry1 = req.SkuQuantities.Split(',');

                    for (int i = 0; i < arry.Length; i++)
                    {
                        QuantityStr += arry[i] + ":" + arry1[i] + ";";
                    }
                    if (!string.IsNullOrEmpty(QuantityStr))
                    {
                        Ainostockmsg = this.UpdateSkuQuanityByOuterIds(item.NumIid, QuantityStr);
                    }
                }
            }
            #endregion

            #region 邮费

            if (model.PostStatus > 0)//邮费
            {
                req.PostageId = model.PostStatus;
            }
            #endregion

            #region 卖点
            if (model.SyncSellPoint == true)
            {
                req.SellPoint = model.SellPointStr;
            }
            #endregion

            #region  商品状态
            string GoodsStatus = string.Empty;
            if (model.GoodsStatus > 0) //商品状态
            {
                switch (model.GoodsStatus)
                {
                    case 1:
                        GoodsStatus = "onsale";
                        break;
                    case 2:
                        GoodsStatus = "instock";
                        break;
                    case 3: //删除商品调用删除的API接口
                        return DeleteItem(item.NumIid);
                }
                req.ApproveStatus = GoodsStatus;
                req.ListTime = model.UpTime;
            }
            #endregion

            #region 标题

            if (!string.IsNullOrEmpty(model.RealTitle))
            {
                req.Title = model.RealTitle;
            }
            else
            {
                string Title = string.Empty;
                if (model.TitleStatus > 0) //标题
                {
                    switch (model.TitleStatus)
                    {
                        case 1:
                            Title = model.AppendTitle + item.Title;
                            break;
                        case 2:
                            Title = item.Title.Replace(model.ReplaceTitleValue, model.ReplaceTitleResult);
                            break;
                        case 3:
                            Title = item.Title + model.AppendTitle;
                            break;
                        case 4:
                            Title = model.AllTitle;
                            break;
                    }

                    req.Title = Title;
                }
            }

            #endregion

            #region 面料
            string Props = string.Empty;
            //面料
            if (model.SetMaterialsOther)
            {
                var prop = item.Props.Split(';');
                for (int i = 0; i < prop.Length; i++)
                {
                    if (prop[i].Contains("1627863:"))
                    {
                        prop[i] = "1627863:20213";
                    }
                }
                Props = string.Join(";", prop);
                item.Props = Props;
            }
            #endregion

            #region 品牌
            if (!string.IsNullOrEmpty(model.Brand))
            {
                string B = item.Props.Split(';').Where(x => x.Contains("20000:")).FirstOrDefault();
                if (!string.IsNullOrEmpty(B))
                {
                    Props = item.Props.Replace(B, model.BrandVal).TrimStart(';');
                    item.Props = Props;
                }
                else
                {
                    Props = item.Props.TrimEnd(';') + ";20000:" + model.BrandVal;
                }
            }

            if (!string.IsNullOrEmpty(Props))
            {
                req.Props = Props.TrimEnd(';');
            }
            #endregion

            #region 会员折扣
            if (model.Dis > 0)
            {
                bool hasdis = model.Dis == 1 ? true : false;
                req.HasDiscount = hasdis;
            }
            #endregion

            #region 分类添加
            if (!string.IsNullOrEmpty(model.SellerCids))
            {
                if (item.SellerCids != null)
                {
                    //处理一个参数如果有就不添加
                    string sellcids = string.Empty;
                    List<string> slist = model.SellerCids.Split(',').ToList();
                    slist.ForEach(x =>
                    {
                        if (!item.SellerCids.Contains(x))
                        {
                            sellcids += x + ",";
                        }
                    });
                    sellcids = sellcids.TrimEnd(',');
                    if (item.SellerCids.Equals("-1") || item.SellerCids.Equals("") || item.SellerCids == null)
                    {
                        req.SellerCids = "," + sellcids + ",";
                    }
                    else
                    {
                        req.SellerCids = "," + item.SellerCids.Trim(',') + "," + sellcids + ",";
                    }
                }
                else
                {
                    req.SellerCids = "," + model.SellerCids + ",";
                }
                req.SellerCids = req.SellerCids.TrimEnd(',') + ",";
            }

            #endregion

            #region 描述
            if (model.DescStatus > 0)
            {
                switch (model.DescStatus)
                {
                    case 1:  //前追加
                        if (!string.IsNullOrEmpty(item.Desc)) //判断一下。如果默认描述为空就不处理
                        {
                            req.Desc = model.AppendDesc + item.Desc;
                        }
                        break;
                    case 2:  //替换
                        if (!string.IsNullOrEmpty(item.Desc)) //判断一下。如果默认描述为空就不处理
                        {
                            req.Desc = item.Desc.Replace(model.ReplaceDescValue, model.ReplaceDescResult);
                        }
                        break;
                    case 3:  //直接更新指定描述
                        req.Desc = model.AllDesc;

                        break;
                }
            }
            #endregion

            ItemUpdateResponse respones = CallTopServer.CallData(Authorize, req);
            if (shop.shopId == 1027 && model.SyncStock) //AINO库存返回结果
            {
                return Ainostockmsg;
            }
            return Config.ReslutValue(respones);
        }

        /// <summary>
        /// 天猫修改价格
        /// </summary>
        /// <param name="NumIid"></param>
        /// <param name="ItemPrice"></param>
        /// <param name="Data"></param>
        /// <returns></returns>
        public string TmallPriceUpdate(long NumIid, string ItemPrice, string Data)
        {
            TmallItemPriceUpdateRequest req = new TmallItemPriceUpdateRequest();
            req.ItemId = NumIid;
            req.ItemPrice = ItemPrice;
            req.SkuPrices = Data;
            TmallItemPriceUpdateResponse response = CallTopServer.CallData(Authorize, req);
            return Config.ReslutValue(response);
        }

        /// <summary>
        /// 天猫曾量更新商品
        /// </summary>
        /// <param name="NumIid"></param>
        /// <param name="XmlData"></param>
        /// <returns></returns>
        public string TmallUpdateGoods_Increment(long NumIid, string XmlData)
        {
            TmallItemSchemaIncrementUpdateRequest req = new TmallItemSchemaIncrementUpdateRequest();
            req.ItemId = NumIid;
            req.XmlData = XmlData;
            TmallItemSchemaIncrementUpdateResponse response = CallTopServer.CallData(Authorize, req);
            return Config.ReslutValue(response);
        }

        /// <summary>
        /// 得到天猫修改分类的XML
        /// </summary>
        /// <param name="UpdateField">seller_cids</param>
        /// <param name="seller_cidlist">分类ID组</param>
        /// <returns></returns>
        public string Seller_CidsXmlData(string UpdateField, List<string> seller_cidlist)
        {
            List<Field> fieldList = new List<Field>();
            MultiInputField Input = new MultiInputField();
            Input.Id = UpdateField;
            Input.SetValues(seller_cidlist);
            fieldList.Add(Input);
            MultiCheckField check = new MultiCheckField();
            check.Id = "update_fields";
            List<Value> valuelist = new List<Value>();
            valuelist.Add(new Value(UpdateField));
            check.SetValues(valuelist);
            fieldList.Add(check);
            return SchemaWriter.WriteParamXmlString(fieldList);
        }

        #endregion

        #region 订单
        /// <summary>
        /// 返回查询的订单(C店/天猫)
        /// </summary>
        /// <param name="tradeNoCollection"></param>
        /// <returns></returns>
        public List<Trade> GetTradeInfoByCShopOrTmall(List<Erp_Model_GJRefund> tradeNoCollection)
        {
            List<Trade> trades = new List<Trade>();
            try
            {
                TradeFullinfoGetRequest req = new TradeFullinfoGetRequest();
                req.Fields = "orders,buyer_nick,tid,seller_memo,seller_flag,receiver_name,receiver_mobile";//红、黄、绿、蓝、紫 分别对应 1、2、3、4、5
                foreach (Erp_Model_GJRefund GjRefund in tradeNoCollection)
                {
                    string[] tradeNos = GjRefund.tbTradeNo.Split(',');
                    foreach (string s in tradeNos)
                    {
                        req.Tid = long.Parse(s);
                        TradeFullinfoGetResponse rsp = CallTopServer.CallData(Authorize, req);
                        trades.Add(rsp.Trade);
                    }
                }
            }
            catch
            { }

            return trades;
        }


        public Trade GetTradeInfoByCShopOrTmall(string tradeNo)
        {

            try
            {
                TradeFullinfoGetRequest req = new TradeFullinfoGetRequest();
                req.Fields = "orders,buyer_nick,tid,seller_memo,seller_flag";//红、黄、绿、蓝、紫 分别对应 1、2、3、4、5
                req.Tid = long.Parse(tradeNo);
                TradeFullinfoGetResponse rsp = CallTopServer.CallData(Authorize, req);
                return rsp.Trade;
            }
            catch
            {
                return null;
            }


        }

        public bool TradeMemoUpdate(string memo, long tbTradeid)
        {
            try
            {
                TradeMemoUpdateRequest req = new TradeMemoUpdateRequest();
                req.Tid = tbTradeid;
                req.Memo = memo;
                TradeMemoUpdateResponse rsp = CallTopServer.CallData(Authorize, req);
                return true;
            }
            catch
            {
                return false;
            }
        }


        /// <summary>
        /// 获取交易帐务
        /// </summary>
        /// <param name="tid"></param>
        /// <returns></returns>
        public List<Top.Api.Domain.TradeAmount> GetTradeAmountByTid(string tid)
        {
            TradeAmountGetRequest req = new TradeAmountGetRequest();
            req.Fields = "order_amounts";
            List<Top.Api.Domain.TradeAmount> tradeAmountCollection = new List<TradeAmount>();

            string[] tidCollection = tid.Split(',');

            foreach (string s in tidCollection)
            {
                req.Tid = Convert.ToInt64(s);

                TradeAmountGetResponse rsp = CallTopServer.CallData(Authorize, req);

                if (rsp.TradeAmount != null)
                {
                    tradeAmountCollection.Add(rsp.TradeAmount);
                }
            }

            return tradeAmountCollection;
        }



        /// <summary>
        /// 获取退款商品的退换理由
        /// </summary>
        /// <param name="Refund_id"></param>
        /// <returns></returns>
        public List<Top.Api.Domain.Refund> GetRefundInfoByRefundId(string buyer_nick)
        {
            List<Top.Api.Domain.Refund> RefundCollection = new List<Top.Api.Domain.Refund>();

            try
            {
                List<string> RefundReason = new List<string>();
                RefundReason.Add("WAIT_BUYER_RETURN_GOODS");//卖家已经同意退款，等待买家退货
                RefundReason.Add("WAIT_SELLER_CONFIRM_GOODS");//买家已经退货，等待卖家确认收货
                RefundsReceiveGetRequest req = new RefundsReceiveGetRequest();

                foreach (string s in RefundReason)
                {
                    bool isNextPage = true;
                    req.Fields = "refund_id, tid, reason,sid,company_name,sku,outer_id,num";
                    req.BuyerNick = buyer_nick;
                    req.Status = s;
                    req.UseHasNext = true;
                    req.PageNo = 1;
                    req.PageSize = 40;

                    while (isNextPage)
                    {
                        try
                        {
                            RefundsReceiveGetResponse rsp = CallTopServer.CallData(Authorize, req);

                            if (rsp.Refunds != null)
                            {
                                isNextPage = rsp.HasNext;
                                req.PageNo++;
                                rsp.Refunds.ForEach(a => RefundCollection.Add(a));
                            }
                            else
                            {
                                isNextPage = false;
                            }
                        }
                        catch
                        {
                            isNextPage = false;
                        }
                    }
                }
            }
            catch
            { }

            return RefundCollection;
        }

        /// <summary>
        /// 根据淘宝退款编号获取退款信息
        /// </summary>
        /// <param name="tb_RefundId"></param>
        /// <returns></returns>
        public Top.Api.Domain.Refund GetRefundExpressCode(long tb_RefundId)
        {
            try
            {
                List<Top.Api.Domain.Refund> RefundCollection = new List<Top.Api.Domain.Refund>();


                RefundGetRequest req = new RefundGetRequest();
                req.Fields = "refund_id, company_name, sid";
                req.RefundId = tb_RefundId;
                RefundGetResponse rsp = CallTopServer.CallData(Authorize, req);
                return rsp.Refund;
            }
            catch
            {
                return new Refund();
            }
        }

        /// <summary>
        /// 返回查询的订单(分销)
        /// </summary>
        /// <param name="tradeNoCollection"></param>
        /// <returns></returns>
        public List<PurchaseOrder> GetTradeInfoByFX(List<Erp_Model_GJRefund> tradeNoCollection)
        {
            List<PurchaseOrder> trades = new List<PurchaseOrder>();
            foreach (Erp_Model_GJRefund GjRefund in tradeNoCollection)
            {
                string[] tradeNos = GjRefund.tbTradeNo.Split(',');
                foreach (string s in tradeNos)
                {
                    trades.Add(GetTradeByRefund(long.Parse(s), "supplier_flag,sub_purchase_orders,distributor_username,supplier_memo,receiver"));
                }
            }
            return trades;
        }

        /// <summary>
        /// 获取退款商品的退换理由(分销)
        /// </summary>
        /// <param name="Refund_id"></param>
        /// <returns></returns>
        public List<Top.Api.Domain.RefundDetail> GetRefundInfoByFX(List<long> fxIds)
        {
            List<RefundDetail> trades = new List<RefundDetail>();
            return GetRefundInfoByFXSubId(fxIds);
        }


        /// <summary>
        /// 查询店铺子单的集合 开始时间与结束时间不能大于六天
        /// </summary>
        /// <param name="Status">状态</param>
        /// <param name="starttime">开始时间</param>
        /// <param name="endtime">结束时间</param>
        /// <returns></returns>
        public List<Order> GetOrders(TradeStatus st, DateTime starttime, DateTime endtime, string Fields)
        {
            string Status = string.Empty;
            int stint = (int)st;
            if (stint == 0)
            {
                Status = "";
            }
            else if (stint == 1)
            {
                Status = "WAIT_BUYER_PAY";
            }
            else if (stint == 2)
            {
                Status = "TRADE_REFUNDING";
            }
            else if (stint == 3)
            {
                Status = "WAIT_SELLER_SEND_GOODS";
            }
            List<Order> list = new List<Order>();
            List<int> errorlist = new List<int>();
            TradesSoldGetRequest req = new TradesSoldGetRequest();
            if (!string.IsNullOrEmpty(Status))
            {
                req.Status = Status;
            }
            if (!string.IsNullOrEmpty(Fields))
            {
                DefaultFields = DefaultFields + "," + Fields;
            }
            req.Fields = DefaultFields;
            req.StartCreated = starttime;
            req.EndCreated = endtime;
            int page = 0;    //初始页码值
            int totalpage = 0; //总页码数
            while (true)
            {
                page++;
                req.PageSize = 100;
                req.PageNo = page;
                TradesSoldGetResponse response = CallTopServer.CallData(Authorize, req);
                if (page == 1)
                {
                    double total = Convert.ToDouble(response.TotalResults);
                    totalpage = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(total / 100)));
                    if (totalpage == 0)
                    {
                        totalpage = 1;
                    }
                }
                if (!response.IsError)
                {
                    List<Trade> tradelist = response.Trades;
                    foreach (Trade t in tradelist)
                    {
                        List<Order> orderlist = t.Orders;
                        list.AddRange(orderlist.Where(a => a.Status.Equals(Status)).ToList());
                    }
                }
                else
                {
                    errorlist.Add(page);
                }
                if (page == totalpage)
                {
                    break;
                }
            }

            //处理错误
            int errorcount = 0; //错误重试次数3
            while (errorlist.Count > 0)
            {
                errorcount++;
                if (errorcount == 3)
                {
                    break;
                }
                for (int i = errorlist.Count - 1; i >= 0; i--)
                {
                    req.PageNo = errorlist[i];
                    TradesSoldGetResponse response = CallTopServer.CallData(Authorize, req);
                    if (!response.IsError)
                    {
                        errorlist.RemoveAt(i);
                        List<Trade> tradelist = response.Trades;
                        foreach (Trade t in tradelist)
                        {
                            List<Order> orderlist = t.Orders;
                            list.AddRange(orderlist.Where(a => a.Status.Equals(Status)).ToList());
                        }
                    }
                }
            }
            return list;
        }


        /// <summary>
        /// 查询分销子单的集合 开始时间与结束时间不能大于六天
        /// </summary>
        /// <param name="Status">状态</param>
        /// <param name="starttime">开始时间</param>
        /// <param name="endtime">结束时间</param>
        /// <returns></returns>
        public List<SubPurchaseOrder> GetFenXiaoOrders(TradeStatus st, DateTime starttime, DateTime endtime)
        {
            string Status = string.Empty;
            int stint = (int)st;
            if (stint == 0)
            {
                Status = "";
            }
            else if (stint == 1)
            {
                Status = "WAIT_BUYER_PAY";
            }
            else if (stint == 2)
            {
                Status = "TRADE_REFUNDING";
            }
            else if (stint == 3)
            {
                Status = "WAIT_SELLER_SEND_GOODS";
            }
            List<SubPurchaseOrder> list = new List<SubPurchaseOrder>();
            List<int> errorlist = new List<int>();
            FenxiaoOrdersGetRequest req = new FenxiaoOrdersGetRequest(); //采购单Request
            if (!string.IsNullOrEmpty(Status))
            {
                req.Status = Status;
            }
            req.StartCreated = starttime;
            req.EndCreated = endtime;
            int page = 0;    //初始页码值
            int totalpage = 0; //总页码数
            while (true)
            {
                page++;
                req.PageSize = 50;
                req.PageNo = page;
                FenxiaoOrdersGetResponse response = CallTopServer.CallData(Authorize, req);
                if (page == 1)
                {
                    double total = Convert.ToDouble(response.TotalResults);
                    totalpage = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(total / 50)));
                    if (totalpage == 0)
                    {
                        totalpage = 1;
                    }
                }
                if (!response.IsError)
                {
                    List<PurchaseOrder> tradelist = response.PurchaseOrders;
                    tradelist.ForEach(
                        delegate(PurchaseOrder t)
                        {
                            list.AddRange(t.SubPurchaseOrders.Where(a => a.Status.Equals(Status)).ToList());
                        }
                        );

                }
                else
                {
                    errorlist.Add(page);
                }
                if (page == totalpage)
                {
                    break;
                }
            }

            //处理错误
            int errorcount = 0; //错误重试次数3
            while (errorlist.Count > 0)
            {
                errorcount++;
                if (errorcount == 3)
                {
                    break;
                }
                for (int i = errorlist.Count - 1; i >= 0; i--)
                {
                    req.PageNo = errorlist[i];
                    FenxiaoOrdersGetResponse response = CallTopServer.CallData(Authorize, req);
                    if (!response.IsError)
                    {
                        errorlist.RemoveAt(i);
                        List<PurchaseOrder> tradelist = response.PurchaseOrders;
                        tradelist.ForEach(
                            delegate(PurchaseOrder t)
                            {
                                list.AddRange(t.SubPurchaseOrders.Where(a => a.Status.Equals(Status)).ToList());
                            }
                            );
                    }
                }
            }
            return list;
        }
        #endregion

        #region 物流
        /// <summary>
        /// 得到所有运费的模板
        /// </summary>
        /// <returns></returns>
        public List<DeliveryTemplate> GetPostTemplate()
        {
            DeliveryTemplatesGetRequest req = new DeliveryTemplatesGetRequest();
            req.Fields = "template_id,template_name";
            DeliveryTemplatesGetResponse response = CallTopServer.CallData(Authorize, req);
            return response.DeliveryTemplates;
        }

        /// <summary>
        /// 返回淘宝的物流公司
        /// </summary>
        /// <returns></returns>
        public List<LogisticsCompany> GetLogisticsCompanys()
        {
            LogisticsCompaniesGetRequest req = new LogisticsCompaniesGetRequest();
            req.Fields = "id,code,name,reg_mail_no";
            LogisticsCompaniesGetResponse rsp = CallTopServer.CallData(Authorize, req);
            return rsp.LogisticsCompanies;
        }


        /// <summary>
        /// 发货
        /// </summary>
        /// <param name="Tid">订单ID</param>
        /// <param name="OutSid">物流编号</param>
        /// <param name="CompanyCode">物流代号</param>
        /// <returns></returns>
        public string SendPost(long Tid, string OutSid, string CompanyCode)
        {
            LogisticsOnlineSendRequest req = new LogisticsOnlineSendRequest();
            req.Tid = Tid;
            req.OutSid = OutSid;
            req.CompanyCode = CompanyCode;
            LogisticsOnlineSendResponse respones = CallTopServer.CallData(Authorize, req);
            return Config.ReslutValue(respones);
        }
        #endregion

        #region 分销
        ///<summary>
        ///查询产品线
        ///</summary>
        ///     
        public List<ProductCat> getProductLine()
        {
            FenxiaoProductcatsGetRequest req = new FenxiaoProductcatsGetRequest();
            req.Fields = "productcats";
            FenxiaoProductcatsGetResponse response = CallTopServer.CallData(Authorize, req);
            return response.Productcats; ;
        }

        /// <summary>
        /// 根椐商家编码查询产品.如果要单独查询SKU。那么FIELD只能为SKUS
        /// </summary>
        /// <param name="outer_id">商家编码</param>
        /// <param name="Field">指定额外的查询字段,可以为空</param>
        /// <returns></returns>
        public FenxiaoProduct GetScItemByCode(string outer_id, string Field)
        {
            FenxiaoProduct pro = null;
            FenxiaoProductsGetRequest req = new FenxiaoProductsGetRequest();
            req.Fields = Field;
            req.OuterId = outer_id;
            FenxiaoProductsGetResponse response = CallTopServer.CallData(Authorize, req);
            if (response.Products != null && response.Products.Count > 0)
            {
                pro = response.Products[0];
            }
            return pro;
        }

        /// <summary>
        /// 修改一个产品
        /// </summary>
        /// <param name="item">分销商品对象</param>
        /// <param name="model">条件对象</param>
        /// <param name="tradelist">未付款的订单（库存用）</param>
        /// <param name="catlist">产品线集合（改价用）</param>
        /// <param name="proplist">分销属性集合（同步SKU用）</param>
        /// <returns></returns>
        public string UpdateProductByModel(FenxiaoProduct item, UpdateGoodsSub model, List<NoPayModel> tradelist, List<ProductCat> catlist, List<FenXiaoPropModel> proplist)
        {
            FenxiaoProductUpdateRequest req = new FenxiaoProductUpdateRequest();
            req.Pid = item.Pid;

            #region 品牌货号
            string InputProperties = string.Empty;
            if (!string.IsNullOrEmpty(model.Brand))
            {
                InputProperties += "20000:" + model.Brand + ";";
            }

            //货号
            if (!string.IsNullOrEmpty(model.ProductNum))
            {
                InputProperties += "13021751:" + model.ProductNum + ";";
            }

            if (!string.IsNullOrEmpty(InputProperties))
            {
                req.InputProperties = InputProperties.TrimEnd(';');
            }

            #endregion

            #region 面料
            string Properties = string.Empty;
            //面料
            if (model.SetMaterialsOther)
            {
                var prop = item.Properties.Split(';');
                for (int i = 0; i < prop.Length; i++)
                {
                    if (prop[i].Contains("20021:"))
                    {
                        prop[i] = "20021:20213";
                    }
                    if (prop[i].Contains("20551:"))
                    {
                        prop[i] = "20551:20213";
                    }
                }
                Properties = string.Join(";", prop);
                item.Properties = Properties;
            }

            if (!string.IsNullOrEmpty(Properties))
            {
                req.Properties = Properties.TrimEnd(';');
            }
            #endregion

            #region 库存价格(SKU)
            string skuids = string.Empty;
            string sku_quantitys = string.Empty;
            string sku_outerids = string.Empty;
            string sku_prop = string.Empty;
            string skucostprice = string.Empty;
            string skustandprice = string.Empty;
            string skudealacostprice = string.Empty;
            if (model.SyncStock || model.SyncPrice) //如果勾选了库存刷新
            {
                List<SpecModel> list = new List<SpecModel>();
                //得到管家库存
                if (model.SyncStock)
                {
                    #region wl 2017-05-26 替换下列方法
                    //HttpHelper http = new HttpHelper(HttpHelper.WinRestUrL("", ""));
                    //string data = string.Format("CommodityApi/SpecList?RequestKey={0}&GoodsNo={1}", JuShiTaRequestKey, item.OuterId);
                    //String temp = http.Get(data);
                    //List<SpecModel> modellist = (List<SpecModel>)JsonConvert.DeserializeObject(temp, typeof(List<SpecModel>));
                    #endregion

                    List<SpecModel> modellist = _9M.Work.ErpApi.WdgjSource.GetSpecListByGoodsNo(item.OuterId);
                    list = modellist.OrderBy(a => a.SpecName1).ThenBy(a => a.SpecCode).ToList();
                }

                double Flag = Convert.ToDouble(model.Price);
                long lineid = item.ProductcatId; //产品线ID

                //得到价格折扣
                ProductCat cat = catlist.Where(x => x.Id == lineid).FirstOrDefault();
                if (cat == null && model.SyncPrice)
                {
                    return "没有产品线参数";
                }
                double dis = double.Parse(cat.CostPercentAgent) / 100;

                string CostPrice = (Flag * dis).ToString();
                string DealerCostPrice = (Flag * dis).ToString();
                string StandPrice = model.Price;
                string ReatilPriceLow = model.Price;
                string ReatilPriceHigh = (Flag * 8).ToString();
                string StandDeaitlPrice = model.Price;
                List<FenxiaoSku> skulist = item.Skus;
                //对比库存
                foreach (FenxiaoSku sku in skulist)
                {
                    long quantity = sku.Quantity;
                    SpecModel ms = list.Find(x =>
                    {
                        return (x.GoodsNo + x.SpecCode).Equals(sku.OuterId, StringComparison.CurrentCultureIgnoreCase);
                    });
                    if (ms != null)
                    {
                        //减掉未付款
                        NoPayModel pay = tradelist.Find(z =>
                        {
                            return z.OuterSkuId.Equals(sku.OuterId, StringComparison.CurrentCultureIgnoreCase);
                        });
                        long Num = pay != null ? pay.Num : 0;

                        if (sku.Quantity != ms.Stock - Num)
                        {
                            if (model.SyncPrice == false)
                            {
                                skuids += sku.Id + ",";
                                quantity = ms.Stock - Num;
                                sku_quantitys += quantity + ",";
                                sku_outerids += sku.OuterId + ",";

                                skudealacostprice += DealerCostPrice + ",";
                                skustandprice += model.Price + ",";
                                skucostprice += DealerCostPrice + ",";
                            }
                        }
                    }
                    if (model.SyncPrice == true) //统一SKU数量（如果有改价的条件。那么所有的SKU参数都加上）
                    {
                        skuids += sku.Id + ",";
                        sku_quantitys += quantity + ",";
                        sku_outerids += sku.OuterId + ",";

                        skudealacostprice += DealerCostPrice + ",";
                        skustandprice += model.Price + ",";
                        skucostprice += DealerCostPrice + ",";
                    }
                }

                if (model.SyncPrice)
                {

                    req.CostPrice = CostPrice;
                    req.DealerCostPrice = DealerCostPrice;
                    req.StandardPrice = StandPrice;
                    if (model.SyncRetailPriceOnly == false)
                    {
                        req.RetailPriceLow = ReatilPriceLow;
                        req.RetailPriceHigh = ReatilPriceHigh;
                        req.StandardRetailPrice = StandDeaitlPrice;
                    }
                    req.SkuCostPrices = skucostprice.TrimEnd(',');
                    req.SkuStandardPrices = skustandprice.TrimEnd(',');
                    req.SkuDealerCostPrices = skudealacostprice.TrimEnd(',');
                    req.SkuIds = skuids.TrimEnd(',');
                    req.SkuOuterIds = sku_outerids.TrimEnd(',');
                    req.SkuQuantitys = sku_quantitys.TrimEnd(',');
                }
                else if (model.SyncStock && !model.SyncPrice)
                {
                    req.SkuIds = skuids.TrimEnd(',');
                    req.SkuQuantitys = sku_quantitys.TrimEnd(',');
                }
            }

            #endregion

            #region 同步SKU
            if (model.SyncSku)
            {
                //20509:28315:M;20509:28314:S;20509:28317:XL;20509:28316:L;20509:28318:XXL

                //得到管家的数据
                #region wl 2017-05-26 替换下列方法
                //HttpHelper http = new HttpHelper(HttpHelper.WinRestUrL("", ""));
                //string data = string.Format("CommodityApi/SpecList?RequestKey={0}&GoodsNo={1}", JuShiTaRequestKey, item.OuterId);
                //String temp = http.Get(data);
                //List<SpecModel> splist = (List<SpecModel>)JsonConvert.DeserializeObject(temp, typeof(List<SpecModel>));
                #endregion

                List<SpecModel> splist = _9M.Work.ErpApi.WdgjSource.GetSpecListByGoodsNo(item.OuterId);

                //这里处理一下配饰（如果有配饰那么,那么把无配饰的加上无）
                IEnumerable<SpecModel> filist = splist.Where(x => x.SpecName1.Contains("有"));
                if (filist.Count() > 0) //如果有配饰
                {
                    //得到配饰的名字
                    string Color = filist.FirstOrDefault().SpecName1;
                    string PeiShi = Color.Substring(Color.IndexOf('有'), Color.Length - Color.IndexOf('有'));
                    for (int i = 0; i < splist.Count; i++)
                    {
                        string Col = splist[i].SpecName1;
                        if (!Col.Contains("有") && !Col.Contains("无"))
                        {
                            splist[i].SpecName1 = Col + PeiShi.Replace("有", "无");
                        }
                    }
                }
                if (splist.Count == 0)
                {
                    return "同步SKU没有获取到同步商品";
                }
                var colorlist = splist.GroupBy(x => x.SpecName1).Select(x => x.Key).ToList();
                var sizelist = splist.GroupBy(x => x.SpecName2).Select(x => x.Key).ToList();
                string propalias = string.Empty;
                #region 得到分类集合
                //到得类目
                if (item.OuterId.Length < 8)
                {
                    return "款号不合格";
                }
                //默认女装
                int SizeNum = 1;
                int ColorNum = 4;
                string GoodsNo = item.OuterId;
                string BrandEn = GoodsHelper.BrandEn(GoodsNo);
                //童装
                if (BrandEn.Length == 3 && BrandEn[BrandEn.Length - 1].ToString().Equals("T", StringComparison.CurrentCultureIgnoreCase))
                {
                    SizeNum = 3;
                    ColorNum = 6;
                }
                else
                {
                    //裤子
                    int Flag = Convert.ToInt32(GoodsNo.Substring(BrandEn.Length + 2, 2));
                    if ((Flag >= 44 && Flag <= 45) || (Flag >= 36 && Flag <= 39) || (Flag >= 40 && Flag <= 43) || (Flag >= 85 && Flag <= 86) || (Flag >= 50 && Flag <= 51))
                    {
                        SizeNum = 2;
                        ColorNum = 5;
                    }
                }
                //颜色(有几个颜色就拿前几条数据)
                List<FenXiaoPropModel> colors = proplist.Where(x => x.PropType == ColorNum).Skip(0).Take(colorlist.Count).ToList();
                //尺码(有几个尺码就拿前几条数据)
                List<FenXiaoPropModel> sizes = proplist.Where(x => x.PropType == SizeNum).Skip(0).Take(sizelist.Count).ToList();

                if ((colorlist.Count != colors.Count) || (sizelist.Count != sizes.Count))
                {
                    return "SKU数量匹配不合格";
                }
                //组合SKU

                for (int i = 0; i < colors.Count; i++)
                {
                    for (int j = 0; j < sizes.Count; j++)
                    {
                        SpecModel ms = splist.Where(x => x.SpecName1.Equals(colorlist[i]) && x.SpecName2.Equals(sizelist[j])).SingleOrDefault();
                        sku_prop += colors[i].PropValue + ";" + sizes[j].PropValue + ",";
                        sku_outerids += ms.GoodsNo + ms.SpecCode + ",";
                        sku_quantitys += ms.Stock + ",";

                        skudealacostprice += item.CostPrice + ",";
                        skustandprice += item.StandardPrice + ",";
                        skucostprice += item.CostPrice + ",";
                        //尺码自定义属性只加一次
                        if (i == 0)
                        {
                            string SizeName = ms.SpecName2;
                            if (SizeNum == 3) //如果是童装。那么转换尺码的名称
                            {
                                Dictionary<string, string> dic = new Dictionary<string, string>();
                                dic.Add("2", "80cm");
                                dic.Add("3", "80cm");
                                dic.Add("4", "90cm");
                                dic.Add("5", "90cm");
                                dic.Add("6", "100cm");
                                dic.Add("7", "100cm");
                                dic.Add("8", "110cm");
                                dic.Add("9", "110cm");
                                dic.Add("10", "120cm");
                                dic.Add("11", "120cm");
                                dic.Add("S", "S");
                                dic.Add("M", "M");
                                dic.Add("L", "L");
                                dic.Add("XL", "XL");
                                dic.Add("XXL", "XXL");
                                var dc = dic.Where(x => x.Key.Equals(ms.SpecName2));
                                if (dc.Count() == 0)
                                {
                                    return "没有匹配到童装的属性";
                                }
                                SizeName = dc.First().Value;
                            }
                            propalias += sizes[j].PropValue + ":" + SizeName + ";";
                        }
                    }
                    propalias += colors[i].PropValue + ":" + colorlist[i] + ";";
                }
                //同步SKU先删后加
                string Prop_Del = string.Empty;
                if (item.Skus != null)
                {
                    item.Skus.ForEach(sk =>
                    {

                        Prop_Del += sk.Properties + ",";
                    });
                    //这里品牌跟货号都不能为空
                    InputProperties += "20000:9魅家;" + "13021751:9m";
                    req.InputProperties = InputProperties;
                    req.SkuPropertiesDel = Prop_Del.TrimEnd(',');
                    FenxiaoProductUpdateResponse delresponse = CallTopServer.CallData(Authorize, req);
                    if (delresponse.IsError)
                    {
                        return "删除SKU时出错";
                    }
                }

                #endregion
                req.SkuProperties = sku_prop.TrimEnd(',');
                req.SkuOuterIds = sku_outerids.TrimEnd(',');
                req.SkuQuantitys = sku_quantitys.TrimEnd(',');
                req.SkuCostPrices = skucostprice.TrimEnd(',');
                req.SkuStandardPrices = skustandprice.TrimEnd(',');
                req.SkuDealerCostPrices = skudealacostprice.TrimEnd(',');
                req.SkuPropertiesDel = string.Empty;
                req.PropertyAlias = propalias.TrimEnd(';');


            }
            #endregion

            #region 邮费
            long PostId = 0;
            string PostType = string.Empty;
            if (model.PostStatus > 0)//邮费
            {
                PostType = model.PostStatus == 1 ? "buyer" : "seller";
                PostId = model.PostStatus == 1 ? 1683490 : 0;

                req.PostageType = PostType;
                if (PostType.Equals("buyer"))
                {
                    req.PostageId = PostId;
                }
            }
            #endregion

            #region 标题
            string Title = string.Empty;
            if (model.TitleStatus > 0) //标题
            {
                if (model.ActivityTitle == true) //如果是活动标题
                {
                    string NewTitle = string.Empty;
                    string OldTitle = item.Name;

                    if (OldTitle.Contains("["))
                    {
                        int start = OldTitle.IndexOf("[");
                        int end = OldTitle.IndexOf("]");
                        NewTitle = model.AppendTitle + OldTitle.Remove(start, end - start + 1);
                    }
                    else if (OldTitle.Contains("【"))
                    {
                        int start = OldTitle.IndexOf("【");
                        int end = OldTitle.IndexOf("】");
                        NewTitle = model.AppendTitle + OldTitle.Remove(start, end - start + 1);
                    }
                    else if (OldTitle.Contains("特价"))
                    {
                        NewTitle = model.AppendTitle + OldTitle.Replace("特价", "");
                    }
                    else
                    {
                        NewTitle = model.AppendTitle + OldTitle;
                    }
                    int len = GoodsHelper.GetLength(NewTitle);
                    if (len > 60)
                    {
                        string real = string.Empty;
                        int flag = 0;
                        for (int i = 0; i < NewTitle.Length; i++)
                        {
                            if (flag == 60)
                            {
                                break;
                            }
                            real += NewTitle[i];
                            if ((int)NewTitle[i] > 128)
                            {
                                flag += 2;
                            }
                            else
                            {
                                flag++;
                            }
                        }
                        NewTitle = real;
                    }
                    req.Name = NewTitle;
                }
                else //如果不是
                {
                    Title = model.TitleStatus == 1 ? model.AppendTitle + item.Name : item.Name.Replace(model.ReplaceTitleValue, model.ReplaceTitleResult);
                    req.Name = Title;
                }
            }
            #endregion

            #region  商品状态
            string GoodsStatus = string.Empty;
            if (model.GoodsStatus > 0) //商品状态
            {
                switch (model.GoodsStatus)
                {
                    case 1:
                        GoodsStatus = "up";
                        break;
                    case 2:
                        GoodsStatus = "down";
                        break;
                    case 3:
                        GoodsStatus = "delete";
                        break;
                }
                req.Status = GoodsStatus;
            }
            #endregion

            #region 描述
            if (model.DescStatus > 0) //描述
            {
                string Desc = string.Empty;
                switch (model.DescStatus)
                {
                    case 1:
                        Desc = model.AppendDesc + item.Description;
                        break;
                    case 2:
                        Desc = item.Description.Replace(model.ReplaceDescValue, model.ReplaceDescResult);
                        break;
                    case 3:
                        Desc = item.Description + model.AppendDesc;
                        break;

                }
                // Desc = model.DescStatus == 1 ? model.AppendDesc + item.Description : item.Description.Replace(model.ReplaceDescValue, model.ReplaceDescResult);
                if (!string.IsNullOrEmpty(Desc))
                {
                    req.Desc = Desc;
                }
            }
            #endregion

            FenxiaoProductUpdateResponse response = CallTopServer.CallData(Authorize, req);
            return Config.ReslutValue(response);
        }

        /// <summary>
        /// 查询采购单的信息
        /// </summary>
        /// <param name="TradeId"></param>
        /// <param name="Fields"></param>
        /// <returns></returns>
        public PurchaseOrder GetTradeByRefund(long TradeId, string Fields)
        {
            PurchaseOrder order = null;
            FenxiaoOrdersGetRequest req = new FenxiaoOrdersGetRequest();
            req.PurchaseOrderId = TradeId;
            //req.TcOrderId = TradeId;
            if (!string.IsNullOrEmpty(Fields))
            {
                req.Fields = Fields;
            }
            FenxiaoOrdersGetResponse rsp = CallTopServer.CallData(Authorize, req);
            if (rsp.PurchaseOrders != null)
            {
                if (rsp.PurchaseOrders.Count > 0)
                {
                    order = rsp.PurchaseOrders[0];
                }
            }
            return order;
        }

        public PurchaseOrder GetTradeByTid(long TradeId, string Fields)
        {
            PurchaseOrder order = null;
            FenxiaoOrdersGetRequest req = new FenxiaoOrdersGetRequest();
            //req.PurchaseOrderId = TradeId;
            req.TcOrderId = TradeId;
            if (!string.IsNullOrEmpty(Fields))
            {
                req.Fields = Fields;
            }
            FenxiaoOrdersGetResponse rsp = CallTopServer.CallData(Authorize, req);
            if (rsp.PurchaseOrders != null)
            {
                if (rsp.PurchaseOrders.Count > 0)
                {
                    order = rsp.PurchaseOrders[0];
                }
            }
            return order;
        }

        /// <summary>
        /// 得到退款的详情
        /// </summary>
        /// <param name="fxSubId"></param>
        /// <returns></returns>
        public List<RefundDetail> GetRefundInfoByFXSubId(List<long> fxSubId)
        {
            List<Top.Api.Domain.RefundDetail> RefundCollection = new List<Top.Api.Domain.RefundDetail>();

            FenxiaoRefundGetRequest req = new FenxiaoRefundGetRequest();

            fxSubId.ForEach(delegate(long s)
            {
                req.SubOrderId = s;
                FenxiaoRefundGetResponse rsp = CallTopServer.CallData(Authorize, req);
                RefundCollection.Add(rsp.RefundDetail);
            });

            return RefundCollection;
        }

        public FenXiaoTradeReslutModel FromApiTrade(FenXiaoTradeQueryModel model)
        {
            FenXiaoTradeReslutModel reslutmodel = new FenXiaoTradeReslutModel();
            //订单结果
            List<FenXiaoTradeModel> Tradelist = new List<FenXiaoTradeModel>();
            //错误页码
            List<int> ErrorPage = new List<int>();
            FenxiaoOrdersGetRequest req = new FenxiaoOrdersGetRequest();
            req.TimeType = "trade_time_type";
            //查询条件
            if (!string.IsNullOrEmpty(model.Status))
            {
                req.Status = model.Status;
            }
            req.StartCreated = model.Start_created;
            req.EndCreated = model.End_created;
            req.PageSize = model.PageSize;
            if (model.TradeId > 0)
            {
                req.PurchaseOrderId = model.TradeId;
            }
            if (!string.IsNullOrEmpty(model.Fields))
            {
                req.Fields = model.Fields;
            }
            List<int> PageNoList = model.PageNos;
            //加载指定的页码
            for (int i = 0; i < PageNoList.Count; i++)
            {
                req.PageNo = PageNoList[i];
                FenxiaoOrdersGetResponse rsp = CallTopServer.CallData(Authorize, req);

                if (rsp.IsError) //如果错误那么添加页码
                {
                    ErrorPage.Add(PageNoList[i]);
                }
                else
                {

                    List<PurchaseOrder> plist = rsp.PurchaseOrders;
                    var query = from pp in plist
                                select new FenXiaoTradeModel()
                                {
                                    FenxiaoId = pp.FenxiaoId,
                                    TcOrderId = pp.TcOrderId,
                                    Created = pp.Created,
                                    AlipayNo = pp.AlipayNo,
                                    TotalFee = pp.TotalFee,
                                    PostFee = pp.PostFee,
                                    DistributorPayment = pp.DistributorPayment,
                                    PayTime = pp.PayTime,
                                    SupplierMemo = pp.SupplierMemo,
                                    PayType = pp.PayType,
                                    Status = pp.Status,
                                    SupplierFlag = pp.SupplierFlag,
                                    BuyerPayment = pp.BuyerPayment,
                                    OrderList = (
                                                 from p in pp.SubPurchaseOrders
                                                 select new FenxiaoOrder()
                                                 {
                                                     ItemOuterId = p.ItemOuterId,
                                                     SkuOuterId = p.SkuOuterId,
                                                     Created = p.Created,
                                                     Status = p.Status,
                                                     RefundFee = p.RefundFee,
                                                     FenxiaoId = p.FenxiaoId,
                                                     ItemId = p.ItemId,
                                                     Num = p.Num,
                                                     Title = p.Title,
                                                     TotalFee = p.TotalFee,
                                                     DistributorPayment = p.DistributorPayment

                                                 }).ToList()

                                };
                    List<FenXiaoTradeModel> list = query.ToList();
                    Tradelist.AddRange(list);
                }
            }

            reslutmodel.ErrorPage = ErrorPage;
            reslutmodel.list = Tradelist;
            return reslutmodel;
        }


        /// <summary>
        /// 得到页总数
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int GetAllPage(FenXiaoTradeQueryModel model)
        {
            int Page = 0;
            FenxiaoOrdersGetRequest req = new FenxiaoOrdersGetRequest();
            req.TimeType = "trade_time_type";
            //查询条件
            if (!string.IsNullOrEmpty(model.Status))
            {
                req.Status = model.Status;
            }
            req.StartCreated = model.Start_created;
            req.EndCreated = model.End_created;
            req.PageSize = model.PageSize;
            if (model.TradeId > 0)
            {
                req.PurchaseOrderId = model.TradeId;
            }
            if (!string.IsNullOrEmpty(model.Fields))
            {
                req.Fields = model.Fields;
            }
            req.PageNo = 1;
            FenxiaoOrdersGetResponse rsp = CallTopServer.CallData(Authorize, req);

            if (rsp.IsError) //如果错误那么添加页码
            {
                Page = -1;
            }
            else
            {
                Page = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(rsp.TotalResults) / model.PageSize));
            }
            return Page;
        }

        #endregion

        #region 类目店铺

        /// <summary>
        /// 通过APPKEY查分类
        /// </summary>
        public List<SellerCat> GetShopCat()
        {
            List<SellerCat> selist = new List<SellerCat>();
            SellercatsListGetRequest req = new SellercatsListGetRequest();
            string nick = GetSellerNick();
            req.Nick = nick;
            SellercatsListGetResponse response = CallTopServer.CallData(Authorize, req);
            return response.SellerCats;
        }


        public List<ItemCat> GetItemCate()
        {
            ITopClient client = new DefaultTopClient("http://gw.api.taobao.com/router/rest", "12253100", "e7dd7a0cad3eaf2beaa9f7308527f29f");
            ItemcatsGetRequest req = new ItemcatsGetRequest();
            req.Fields = "cid,parent_cid,name,is_parent";
            req.ParentCid = 1636;
            ItemcatsGetResponse response = client.Execute(req);
            return response.ItemCats;
        }

        /// <summary>
        ///   通过APP查用户名
        /// </summary>
        public string GetSellerNick()
        {
            UserSellerGetRequest req = new UserSellerGetRequest();
            req.Fields = "nick";
            UserSellerGetResponse response = CallTopServer.CallData(Authorize, req);
            return response.User.Nick;
        }

        /// <summary>
        /// 得到淘宝的当前时间
        /// </summary>
        /// <returns></returns>
        public DateTime GetTaoBaoNow()
        {
            TimeGetRequest req = new TimeGetRequest();
            TimeGetResponse response = CallTopServer.CallData(Authorize, req);
            return DateTime.Parse(response.Time);
        }

        //得到淘宝的属性键值
        public List<PropValue> GetPropDetail(long Cid, string Pvs)
        {
            //20509 1627207
            ItempropvaluesGetRequest req = new ItempropvaluesGetRequest();
            req.Fields = "cid,pid,prop_name,vid,name,name_alias,status,sort_order";
            req.Cid = Cid;
            req.Datetime = DateTime.Parse("2000-01-01 00:00:00");
            req.Pvs = Pvs;
            req.Type = 1L;
            ItempropvaluesGetResponse rsp = CallTopServer.CallData(Authorize, req);
            return rsp.PropValues;
        }
        #endregion
    }

    public enum TradeStatus
    {
        /// <summary>
        /// 等待买家付款
        /// </summary>
        //WAIT_BUYER_PAY = "WAIT_BUYER_PAY",
        WAIT_BUYER_PAY = 1,
        /// <summary>
        /// 退款中
        /// </summary>
        //TRADE_REFUNDING = "TRADE_REFUNDING",
        TRADE_REFUNDING = 2,

        /// <summary>
        /// 买家已付款
        /// </summary>
        WAIT_SELLER_SEND_GOODS = 3,
        /// <summary>
        /// 全部
        /// </summary>
        /// 
        //ALL = ""
        ALL = 0
    }
}
