using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _9M.Work.YouZan
{
    /// <summary>
    /// 商品
    /// </summary>
    public class GoodsDetail
    {
        public long num_iid { get; set; }
        public string alias { get; set; }
        public string title { get; set; }
        public string desc { get; set; }
        public string outer_id { get; set; }
        public string outer_buy_url { get; set; }
        public string pic_url { get; set; }
        public string pic_thumb_url { get; set; }
        public string tag_ids { get; set; }
        public long num { get; set; }
    }


    public class GoodsCategory
    {
        public long cid { get; set; }
        public long parent_cid { get; set; }
        public string name { get; set; }
        public bool is_parent { get; set; }
        public List<GoodsCategory> sub_categories { get; set; }
    }
    /// <summary>
    /// 商品组
    /// </summary>
    public class GoodsTag
    {
        public long id { get; set; }
        public string name { get; set; }
        public int type { get; set; }
        public DateTime created { get; set; }
        public long item_num { get; set; }
        public string tag_url { get; set; }
        public string share_url { get; set; }
        public string desc { get; set; }
    }

    /// <summary>
    /// 商品添加或更新
    /// </summary>
    public class Itemparm
    {
        public int DescImageCount { get; set; }
        public string quantity { get; set; }
        public string num_iid { get; set; }
        public string title { get; set; }
        public string tag_ids { get; set; }
        public string skus_with_json { get; set; }
        public string price { get; set; }
        public string post_fee { get; set; }
        public string outer_id { get; set; }
        /// <summary>
        /// 默认1 上架商品，设置为0 不上架商品，放入仓库
        /// </summary>
        public string is_display { get; set; }
        /// <summary>
        /// 吊牌价
        /// </summary>
        public string origin_price { get; set; }
        public List<string> images { get; set; }
        public string image_ids { get; set; }
        public string desc { get; set; }
        public string delivery_template_id { get; set; }
        public string auto_listing_time { get; set; }
    }

    public class sku_property
    {
        public string color { get; set; }
        public string size { get; set; }
    }

    public class skus
    {
        public sku_property sku_property { get; set; }
        public decimal sku_price { get; set; }
        public int sku_quantity { get; set; }
        public string sku_outer_id { get; set; }

    }
}
