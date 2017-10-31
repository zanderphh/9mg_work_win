
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _9M.Work.Model
{
    public class ApiSpecModel
    {
        public string GoodsNo { get; set; }
        public string ClassName { get; set; }
        public string GoodsName { get; set; }
        public string SpecCode { get; set; } //规格码
        public string SpecName { get; set; }//规格名
        public int Stock { get; set; }//库存
        public string SpecName1 { get; set; } //颜色
        public string SpecName2 { get; set; } //尺码
    }


    public class ApiTopicGoodsModel
    {
        public string GoodsNo { get; set; }
        public string GoodsName { get; set; }
        public int Stock { get; set; }
        public int SellcountWeek { get; set; }
        public string ClassName { get; set; }
    }

    public class WareHouseInModel
    {
        public string GoodsNo { get; set; }
        public string SpecCode { get; set; }
        public string SpecName { get; set; }
        public int GoodsCount { get; set; }
        public string PositionRemark { get; set; }
        public string P_Position { get; set; }
        public string F_Position { get; set; }
        public string RegOperator { get; set; }
        public DateTime? CHKTime { get; set; }
        public string BillID { get; set; }
    }
    /// <summary>
    /// WEBAPI的参数封装
    /// </summary>
    public class PostSkuModel
    {
        public string GoodsNo { get; set; }  //款号+规格码的全编号
        public string P_postion { get; set; } //主货位
        public string F_postion { get; set; } //次货位
        public bool UpdatePrimaryOnly { get; set; }//是否只更改主货位
    }

    public class PostUpdateCommodityModel
    {
        public string GoodsNo { get; set; }
        /// <summary>
        /// 分销价格
        /// </summary>
        public decimal Price2 { get; set; }
        /// <summary>
        /// 零售价
        /// </summary>
        public decimal Price_Detail { get; set; }

        /// <summary>
        /// 上架日期
        /// </summary>
        public string SellDate { get; set; }

        /// <summary>
        /// 拍照状态（状态为4的时候为实价）
        /// </summary>
        public string Reserved1 { get; set; }

        /// <summary>
        /// 拍照备注
        /// </summary>
        public string Reserved2 { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string ReMark { get; set; }


        /// <summary>
        /// 1.只追加描述
        /// 2.只备注拍照状态
        /// 3.上新组修改商品(备注为取消关键字符)
        /// 4.商品原价
        /// </summary>
        public int UpdateType { get; set; }

    }
}
