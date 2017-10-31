using _9M.Work.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;


namespace _9M.Work.YouZan
{
    public class YzApi
    {
        private KDTApiKit kit = new KDTApiKit("01f4049f3350864e4f", "c1a0c70777b962d431813cda5e7c54e9");
        private Dictionary<String, String> param;
        private string Filed = string.Empty;
        public YzApi()
        {
            Filed = GetPropertyInfoArray<GoodsDetail>();
        }
        /// <summary>
        /// 将一个实体类的属性反射成字符串a,b,c
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private string GetPropertyInfoArray<T>()
        {
            Type type = typeof(T);
            object obj = Activator.CreateInstance(type);
            PropertyInfo[] props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            return string.Join(",", props.Select(x => x.Name));
        }

        #region
        /// <summary>
        /// 获取出售中或仓库中商品的总数量
        /// </summary>
        /// <param name="IsOnsell"></param>
        /// <returns></returns>
        public int TotalGoods(bool IsOnsell)
        {
            string ApiName = "kdt.items.onsale.get";
            if (!IsOnsell)
            {
                ApiName = "kdt.items.inventory.get";
            }
            param = new Dictionary<string, string>();
            param.Add("page_size", "150");
            param.Add("page_no", "1");
            param.Add("fields", Filed);
            var response = kit.get(ApiName, param);
            var obj = JsonConvert.DeserializeObject<JObject>(response);
            return JsonConvert.DeserializeObject<int>(obj["response"]["total_results"].ToString());
        }

        /// <summary>
        /// 商品类目
        /// </summary>
        /// <returns></returns>
        public List<GoodsCategory> Categories()
        {
            param = new Dictionary<string, string>();
            var response = kit.get("kdt.itemcategories.get", param);
            var obj = JsonConvert.DeserializeObject<JObject>(response);
            return JsonConvert.DeserializeObject<List<GoodsCategory>>(obj["response"]["categories"].ToString());
        }


        /// <summary>
        /// 上架或下架商品
        /// </summary>
        /// <returns></returns>
        public bool UpOrDownGoods(List<string> num_iids, bool IsUp)
        {
            param = new Dictionary<string, string>();
            string ApiName = "kdt.items.update.listing";
            if (!IsUp)
                ApiName = "kdt.items.update.delisting";
            param.Add("num_iids", string.Join(",", num_iids));
            var response = kit.get(ApiName, param);
            var obj = JsonConvert.DeserializeObject<JObject>(response);
            return JsonConvert.DeserializeObject<bool>(obj["response"]["is_success"].ToString());
        }

        /// <summary>
        /// 商品组
        /// </summary>
        /// <returns></returns>
        public List<GoodsTag> CategoriesTags()
        {
            param = new Dictionary<string, string>();
            param.Add("is_sort", "true");
            var response = kit.get("kdt.itemcategories.tags.get", param);
            var obj = JsonConvert.DeserializeObject<JObject>(response);
            return JsonConvert.DeserializeObject<List<GoodsTag>>(obj["response"]["tags"].ToString());
        }

        /// <summary>
        /// 查询仓库或是出售中的商品
        /// </summary>
        /// <param name="IsOnSell">true为出售中,false为仓库中</param>
        /// <param name="Field">自定义查询如果为空就是所有模型类的属性</param>
        /// <returns></returns>
        public List<GoodsDetail> OnLineGoodsList(bool IsOnSell, string CustomField)
        {
            string ApiName = "kdt.items.onsale.get";
            if (!IsOnSell)
            {
                ApiName = "kdt.items.inventory.get";
            }
            if (!string.IsNullOrEmpty(CustomField))
            {
                Filed = CustomField;
            }
            List<GoodsDetail> list = new List<GoodsDetail>();
            //得到商品总数算出页码总数
            int TotalReslut = TotalGoods(IsOnSell);
            int PageNo = (int)Math.Ceiling(Convert.ToDouble(TotalReslut) / 150);
            for (int i = 1; i <= PageNo; i++)
            {
                param = new Dictionary<string, string>();
                param.Add("page_size", "150");
                param.Add("page_no", i.ToString());
                param.Add("fields", Filed);
                //请求
                var response = kit.get(ApiName, param);
                //解析
                var obj = JsonConvert.DeserializeObject<JObject>(response);
                var glist = JsonConvert.DeserializeObject<List<GoodsDetail>>(obj["response"]["items"].ToString());
                list.AddRange(glist);
            }
            return list;
        }



        /// <summary>
        /// 添加一个商品
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool AddItem(Itemparm item)
        {
            bool b = true;
            try
            {
                WebClient client = new WebClient();
                //byte[]参数
                Dictionary<string, object> postParameters = new Dictionary<string, object>();
                //参数
                param = new Dictionary<string, string>();
                //价格
                param.Add("price", item.price);
                //标题
                param.Add("title", item.title);
                //描述
                string Desc = string.Empty;
                //只选15张图
                List<string> image = RegexHelper.GetImgAll(item.desc).Skip(0).Take(item.DescImageCount).ToList();

                image.ForEach(x =>
                {
                    Desc += string.Format(@"<img src='{0}'/>", x);
                });
                param.Add("desc", Desc);
                //数量
                param.Add("quantity", item.quantity);
                //邮费
                param.Add("post_fee", item.post_fee);
                //sku
                param.Add("skus_with_json", item.skus_with_json);
                //开始出售时间。默认0，设置为0 立即出售
                if (!string.IsNullOrEmpty(item.auto_listing_time))
                    param.Add("auto_listing_time", item.auto_listing_time);
                //邮费模版
                if (!string.IsNullOrEmpty(item.delivery_template_id))
                    param.Add("delivery_template_id", item.delivery_template_id);
                //吊牌价
                if (!string.IsNullOrEmpty(item.origin_price))
                    param.Add("origin_price", item.origin_price);
                //图片
                for (int i = 0; i < item.images.Count; i++)
                {
                    byte[] data = client.DownloadData(item.images[i].Replace("_100x100", "_640x640"));
                    postParameters.Add("file" + i, new KDTUtil.FileParameter(data, i + ".jpg", "image/unknown"));
                }
                //状态默认1 上架商品，设置为0 不上架商品，放入仓库
                if (!string.IsNullOrEmpty(item.is_display))
                    param.Add("is_display", item.is_display);
                //编号
                param.Add("outer_id", item.outer_id);
                //默认分类
                param.Add("cid", "1000000");
                //商品分组
                if (!string.IsNullOrEmpty(item.tag_ids))
                    param.Add("tag_ids", item.tag_ids);

                var response = kit.post("kdt.item.add", param, postParameters, "images[]");
            }
            catch(Exception ex)
            {
                b = false;
            }
            return b;
        }

        /// <summary>
        /// 修改一个商品
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Update(Itemparm item)
        {
            WebClient client = new WebClient();
            //byte[]参数
            Dictionary<string, object> postParameters = new Dictionary<string, object>();
            //参数
            param = new Dictionary<string, string>();
            //ID
            param.Add("num_iid", item.num_iid);
            //价格
            param.Add("price", item.price);
            //标题
            param.Add("title", item.title);
            //描述
            param.Add("desc", item.desc);
            //邮费
            param.Add("post_fee", item.post_fee);
            //sku
            param.Add("skus_with_json", item.skus_with_json);
            //开始出售时间。默认0，设置为0 立即出售
            if (!string.IsNullOrEmpty(item.auto_listing_time))
                param.Add("auto_listing_time", item.auto_listing_time);
            //邮费模版
            if (!string.IsNullOrEmpty(item.delivery_template_id))
                param.Add("delivery_template_id", item.delivery_template_id);
            //图片
            for (int i = 0; i < item.images.Count; i++)
            {
                byte[] data = client.DownloadData(item.images[i]);
                postParameters.Add("file" + i, new KDTUtil.FileParameter(data, i + ".jpg", "image/unknown"));
            }
            //状态默认1 上架商品，设置为0 不上架商品，放入仓库
            if (!string.IsNullOrEmpty(item.is_display))
                param.Add("is_display", item.is_display);
            //编号
            param.Add("outer_id", item.outer_id);
            //商品分组
            if (!string.IsNullOrEmpty(item.tag_ids))
                param.Add("tag_ids", item.tag_ids);
            var response = kit.post("kdt.item.update", param, postParameters, "images[]");
            return true;
        }
        #endregion

    }
}
