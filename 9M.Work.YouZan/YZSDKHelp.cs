using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YZOpenSDK;

namespace _9M.Work.YouZan
{
    public class YZSDKHelp
    {

        static String appID = "01f4049f3350864e4f";
        static string appSecret = "c1a0c70777b962d431813cda5e7c54e9";
        public static bool UpdateYZGoodsPrice(string GoodsNo, string Price)
        {
            bool isUpdate = false;

            try
            {
                Auth auth = new Sign(appID, appSecret);
                YZClient yzClient = new DefaultYZClient(auth);

                Dictionary<string, object> dict = new System.Collections.Generic.Dictionary<string, object>();
                dict.Add("item_no", GoodsNo);
                var result = yzClient.Invoke("youzan.items.custom.get", "3.0.0", "post", dict, null);

                GoodsResponse g = Newtonsoft.Json.JsonConvert.DeserializeObject<GoodsResponse>(result);

                if (g != null)
                {
                    if (g.response != null)
                    {
                        if (g.response.items != null)
                        {
                            dict.Clear();

                            #region Api获取的SKU为null 2017-06-17 暂修改为调用下列接口

                            //dict.Add("item_no", g.response.items[0].item_no);
                            //dict.Add("item_id", g.response.items[0].item_id);
                            //result = yzClient.Invoke("youzan.skus.custom.get", "3.0.0", "post", dict, null);
                            //SkuResponse s = Newtonsoft.Json.JsonConvert.DeserializeObject<SkuResponse>(result);
                            //if (s != null)
                            //{
                            //    if (s.response != null)
                            //    {
                            //        if (s.response.skus != null)
                            //        {
                            //            if (s.response.skus.Count > 0)
                            //            {
                            //                #region 修改SKU

                            //                foreach (SkuItems gs in s.response.skus)
                            //                {
                            //                    dict.Clear();
                            //                    dict = new System.Collections.Generic.Dictionary<string, object>();

                            //                    dict.Add("sku_id", gs.sku_id);
                            //                    dict.Add("item_id", gs.item_id);
                            //                    dict.Add("price", Price);
                            //                    result = yzClient.Invoke("youzan.item.sku.update", "3.0.0", "post", dict, null);
                            //                    isUpdate = true;
                            //                }
                            //                #endregion
                            //            }
                            //        }
                            //    }
                            //}
                            #endregion


                            dict.Add("item_id", g.response.items[0].item_id);
                            result = yzClient.Invoke("youzan.item.get", "3.0.0", "post", dict, null);
                            SkuStockResponse s = Newtonsoft.Json.JsonConvert.DeserializeObject<SkuStockResponse>(result);

                            if (s != null)
                            {
                                if (s.response != null)
                                {
                                    if (s.response.item != null)
                                    {
                                        if (s.response.item.skus != null)
                                        {
                                            if (s.response.item.skus.Count > 0)
                                            {
                                                #region 修改SKU

                                                foreach (SkuStockItem gs in s.response.item.skus)
                                                {
                                                    if (Convert.ToDecimal(Price) != gs.price/100)
                                                    {
                                                        dict.Clear();
                                                        dict = new System.Collections.Generic.Dictionary<string, object>();

                                                        dict.Add("sku_id", gs.sku_id);
                                                        dict.Add("item_id", gs.item_id);
                                                        dict.Add("price", Price);
                                                        result = yzClient.Invoke("youzan.item.sku.update", "3.0.0", "post", dict, null);
                                                        isUpdate = true;
                                                    }

                                                }
                                                #endregion
                                            }
                                        }
                                    }
                                }
                            }

                        }
                    }
                }
            }
            catch
            {

            }

            return isUpdate;
        }



        public static List<OnSaleItems> GetOnSaleItems()
        {
            int Page_no = 1;
            int page_size = 150;

            List<OnSaleItems> items = new List<OnSaleItems>();
            try
            {
                Auth auth = new Sign(appID, appSecret);
                YZClient yzClient = new DefaultYZClient(auth);
                Dictionary<string, object> dict = new System.Collections.Generic.Dictionary<string, object>();

                while (page_size.Equals(150))
                {
                    dict.Clear();
                    dict.Add("page_size", page_size);
                    dict.Add("page_no", Page_no);
                    var result = yzClient.Invoke("kdt.items.onsale.get", "1.0.0", "post", dict, null);
                    OnSaleResponse oResponse = JsonConvert.DeserializeObject<OnSaleResponse>(result);
                    if (oResponse != null)
                    {
                        if (oResponse.response != null)
                        {
                            if (oResponse.response.items != null)
                            {
                                page_size = oResponse.response.items.Count;
                                items.AddRange(oResponse.response.items);
                                Page_no++;

                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
            catch
            {

            }

            return items;

        }
    }


    /*--------------youzan.items.custom.get----------------------------*/
    public class GoodsItem
    {
        public int item_id { get; set; }
        public string item_no { get; set; }
    }


    public class GoodsResponseItems
    {
        public List<GoodsItem> items { get; set; }
    }


    public class GoodsResponse
    {
        public GoodsResponseItems response { get; set; }
    }
    /*-------------------------------------------*/


    /*---------------youzan.skus.custom.get----------------------------*/
    public class SkuItems
    {
        public int item_id { get; set; }
        public int sku_id { get; set; }
        public string price { get; set; }
        public string item_no { get; set; }
    }

    public class SkuResponseItems
    {
        public List<SkuItems> skus { get; set; }
    }

    public class SkuResponse
    {
        public SkuResponseItems response { get; set; }
    }
    /*-------------------------------------------*/


    /*---------------youzan.item.get----------------------------*/

    public class SkuStockItem
    {
        /// <summary>
        /// 商品ID
        /// </summary>
        public int item_id { get; set; }
        /// <summary>
        /// 规格ID
        /// </summary>
        public int sku_id { get; set; }
        /// <summary>
        /// 商品的这个Sku的价格；精确到2位小数；单位：元
        /// </summary>
        public decimal price { get; set; }
        /// <summary>
        /// 属于这个Sku的商品的数量
        /// </summary>
        public String outer_id { get; set; }
        /// <summary>
        /// 商家编码（商家为Sku设置的外部编号）
        /// </summary>
        public int quantity { get; set; }
    }

    public class SkuStockResponseItem
    {
        public List<SkuStockItem> skus { get; set; }
    }

    public class SkuStockResponseItemObj
    {
        public SkuStockResponseItem item { get; set; }
    }

    public class SkuStockResponse
    {
        public SkuStockResponseItemObj response { get; set; }
    }


    /*-------------------------------------------*/




    public class GoodsSku
    {
        public int num_iid { get; set; }
        public string outer_id { get; set; }
        public decimal price { get; set; }
        public long quantity { get; set; }
        public int sku_id { get; set; }
    }


    public class OnSaleItems
    {
        /// <summary>
        /// 商品价格
        /// </summary>
        public decimal price { get; set; }

        /// <summary>
        /// 商品数字编号
        /// </summary>
        public int num_iid { get; set; }

        /// <summary>
        /// 商品货号（商家为商品设置的外部编号，可与商家外部系统对接）
        /// </summary>
        public string outer_id { get; set; }

        public GoodsSku[] skus { get; set; }

    }

    public class OnSaleResponse
    {
        public OnSaleResponseItem response { get; set; }
    }

    public class OnSaleResponseItem
    {
        public List<OnSaleItems> items { get; set; }
        public string total_results { get; set; }
    }
}
