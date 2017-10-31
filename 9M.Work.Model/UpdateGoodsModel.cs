using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _9M.Work.Model
{
    class UpdateGoodsModel
    {
    }

    public class UpdateGoodsSub
    {
        public UpdateGoodsSub()
        {
            Pid = 0;
            Price = "0";
            SyncPrice = false;
            SyncStock = false;
            SyncRetailPriceOnly = false;
            ActivityTitle = false;
            SetMaterialsOther = false;
            SyncSellPoint = false;
        }
        //商品ID
        public long Pid { get; set; }

        //价格
        public string Price { get; set; }
        //品牌
        public string Brand { get; set; }

        public string BrandVal { get; set; }
        //货号
        public string ProductNum { get; set; }
        public bool SyncPrice { get; set; }
        public bool SyncStock { get; set; }
        public bool SyncSku { get; set; }
        /// <summary>
        /// 0为不处理,>0为模版ID
        /// </summary>
        public int PostStatus { get; set; }
        /// <summary>
        /// 0为不处理,1为上架,2为下架,3为删除
        /// </summary>
        public int GoodsStatus { get; set; }

        /// <summary>
        /// 上架时间
        /// </summary>
        public DateTime? UpTime { get; set; }
        /// <summary>
        /// 0不处理 ,1前追加 ,2替换,3后追加,4直接修改
        /// </summary>
        public int TitleStatus { get; set; }
        public string AppendTitle { get; set; }
        public string ReplaceTitleValue { get; set; }
        public string ReplaceTitleResult { get; set; }
        public string AllTitle { get; set; }

        /// <summary>
        /// 如果RealTitle的值不为空。那么直接改成RealTitle
        /// </summary>
        public string RealTitle { get; set; }

        /// <summary>
        /// 0不处理 ,1前追加,2替换,3全面更新描述
        /// </summary>
        public int DescStatus { get; set; }
        public string AppendDesc { get; set; }
        public string ReplaceDescValue { get; set; }
        public string ReplaceDescResult { get; set; }

        /// <summary>
        /// 全面更新描述
        /// </summary>
        public string AllDesc { get; set; }

        /// <summary>
        /// 0不处理 ,1打折 ,2不打折
        /// </summary>
        public int Dis { get; set; }

        /// <summary>
        /// 分类添加
        /// </summary>
        public string SellerCids { get; set; }
        /// <summary>
        /// 是否只同步基准价
        /// </summary>

        public bool SyncRetailPriceOnly { get; set; }

        /// <summary>
        /// 活动标题
        /// </summary>
        public bool ActivityTitle { get; set; }
        
        /// <summary>
        /// 修改面料为其它
        /// </summary>
        public bool SetMaterialsOther { get; set; }
        /// <summary>
        /// 是否修改卖点
        /// </summary>
        public bool SyncSellPoint { get; set; }
        /// <summary>
        /// 卖点
        /// </summary>
        public string SellPointStr { get; set; }

    }

    public class GoodsUpdateBind
    {
        public string GoodsNo { get; set; }
        public string Price { get; set; }
        public bool IsUpdateBrand { get; set; }
        public bool IsUpdateProductNum { get; set; }
        public bool IsUpdateSellerCid { get; set; }
        public bool IsUpdateDis { get; set; }
        public bool IsUpdatePrice { get; set; }
        public bool IsUpdateStock { get; set; }
        public bool IsUpdatePost { get; set; }
        public bool IsUpdateStatus { get; set; }
        public bool IsUpdateTitle { get; set; }
        public bool IsUpdateDesc { get; set; }
        public bool IsUpdateSku { get; set; }
        public bool IsUpdateMaterials { get; set; }
        public bool IsUpdateSellPoint { get; set; }
    }
}
