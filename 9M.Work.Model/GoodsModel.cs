using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _9M.Work.Model
{
    public class GoodsModel
    {
        public int GoodsID { get; set; }//主键
        public string GoodsNo { get; set; } //款号
        public string GoodsName { get; set; }//品名
        public string GoodsName2 { get; set; }//别名
        public string SpecCode { get; set; }//一对一范型
        public int Stock { get; set; }//一对一范型
        public string SpecName { get; set; } //规格名
        public decimal Price { get; set; } //价格
        public decimal Price1 { get; set; } //品牌价（用于打印吊牌）
        public double Price_new { get; set; } //修改的目标价格
        public int SellWeekDate { get; set; } //销售周期
        public bool Isunsalable { get; set; } //是否滞销
        public string Title { get; set; } //商品标题
        public string Url { get; set; } //商品链接
        public int SellcountWeek { get; set; } //周销售量
        public string Pic_Url { get; set; } //图片地址
        public long Num_Iid { get; set; } //淘宝主键ID
        public string P_postion { get; set; } //主货位
        public string F_postion { get; set; } //次货位
        public string PositionRemark { get; set; } //货位备注
        public bool IsDis { get; set; } //是否是折扣款
        public bool IsClearInStock { get; set; } //是否清仓
        public string SellModel { get; set; } //商品模式
        public int Sellcount { get; set; } //销量
        public string Brand { get; set; } //品牌
        public string SpecName2 { get; set; } //号型
        public string SpecName1 { get; set; } //颜色
        public string ClassName { get; set; } //类目名
        public decimal Price_Member { get; set; } //实体店的零售价
        public string HaoXing { get; set; } //号型
        public string ZhiXing { get; set; } //执行标准
        public decimal Price_Detail { get; set; } //管家的零售价格
        public string Reserved3 { get; set; } //商品最初的定价有区间的话如 23-24.5
        public string Remark { get; set; }
        public int Num { get; set; }
        public int Stock1 { get; set; }
        public string PriceRange { get; set; }
    }
}
