using _9M.Work.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _9M.Work.Model
{


    public enum EnumRefundSendStatus
    {
        [Text("9魅季")]
        MG = 1,

        [Text("魅季歌儿")]
        Mezingr = 2,

        [Text("C店")]
        CShop = 3,

        [Text("分销")]
        FX = 4,

        [Text("京东")]
        JD = 5,
        [Text("爱侬")]
        AiNo = 6,
    }

    public enum GoodsStatus
    {
        [Text("出售中")]
        Onsell = 1,

        [Text("仓库中")]
        InStock = 2,

        [Text("全部")]
        All = 3,
    }

    ///// <summary>
    ///// API模式
    ///// </summary>
    //public enum ApiType
    //{
    //    [Text("C店")]
    //    C = 1,

    //    [Text("天猫")]
    //    Tmall = 2,

    //    [Text("分销")]
    //    FX = 3,

    //    [Text("京东")]
    //    JD = 4,
    //}

    public enum Goods
    {
        Bad = 0,
        Good = 1
    }

    public enum PriceType
    {
        Default = 0,
        Custom = 1
    }
    public enum ShopType
    {
        CShop_9M = 0,
        BShop_9MG = 1,
        BShop_Mezingr = 2
    }
    /// <summary>
    /// 仓库中商品的搜索范围
    /// </summary>
    public enum InventStatus
    {
        /// <summary>
        /// 全部
        /// </summary>
        All = 3,
        /// <summary>
        /// 默认,等待上架的
        /// </summary>
        Default = 1,

        /// <summary>
        /// 能重新上架的
        /// </summary>
        CanUp = 2
    }

    public enum OperationStatus
    {

        /// <summary>
        /// 添加
        /// </summary>
        ADD = 1,

        /// <summary>
        /// 修改
        /// </summary>
        Edit = 2,
        /// <summary>
        /// 添加子级
        /// </summary>
        ADDChild = 3,
    }

    public enum CategoryTraitAttr
    {
        /// <summary>
        /// 袖型
        /// </summary>
        [Text("袖型")]
        XiuXing = 1,
        /// <summary>
        /// 面料
        /// </summary>
        [Text("面料")]
        MianLiao = 2,
        /// <summary>
        /// 颜色
        /// </summary>
        [Text("颜色")]
        YanSe = 3,
        /// <summary>
        /// 门襟
        /// </summary>
        [Text("门襟")]
        MenJin = 4,
        /// <summary>
        /// 领型
        /// </summary>
        [Text("领型")]
        LingXing = 5,
        /// <summary>
        /// 其它特点
        /// </summary>
       [Text("其它特点")]
        QiTa = 6,
    }

    public enum YearAttr
    {
        /// <summary>
        /// 春
        /// </summary>
        [Text("春")]
        Spring = 1,
        /// <summary>
        /// 夏
        /// </summary>
        [Text("夏")]
        Summer = 2,
        /// <summary>
        /// 秋
        /// </summary>
        [Text("秋")]
        Autumn = 3,
        /// <summary>
        /// 冬
        /// </summary>
        [Text("冬")]
        Winter = 4,
    }

    public enum Performance
    {
        /// <summary>
        /// 包装
        /// </summary>
        [Text("包装")]
        Packing = 1,
        /// <summary>
        /// 测量
        /// </summary>
        [Text("测量")]
        Measure = 2,
        /// <summary>
        /// 上架
        /// </summary>
        [Text("上架")]
        StockIn = 3,
    }

    public enum PrintType
    {
        /// <summary>
        /// 标签
        /// </summary>
        [Text("标签")]
        Lable = 1,
        /// <summary>
        /// 吊牌
        /// </summary>
        [Text("吊牌")]
        Fyi = 2,

        /// <summary>
        /// 款号
        /// </summary>
        [Text("款号")]
        Ware = 3,

        /// <summary>
        /// 样品
        /// </summary>
        [Text("样品")]
        Sample = 4,
    }

    public enum GoodsLogType
    {
        /// <summary>
        /// 入库上架
        /// </summary>
        [Text("入库上架")]
        StockIn = 1,
        /// <summary>
        /// 修改货位
        /// </summary>
        [Text("修改货位")]
        UpdatePostion = 2,

        /// <summary>
        /// 包装
        /// </summary>
        [Text("包装")]
        Packing = 3,

        /// <summary>
        /// 货品分款
        /// </summary>
        [Text("货品分款")]
        Subsec = 4,
    }

}
