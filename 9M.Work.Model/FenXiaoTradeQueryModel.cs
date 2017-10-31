using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _9M.Work.Model
{
    public class FenXiaoTradeQueryModel
    {
        /// <summary>
        /// 状态
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime Start_created { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime End_created { get; set; }
        /// <summary>
        /// 每页数量
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// 请求的页码
        /// </summary>
        public List<int> PageNos { get; set; }
        /// <summary>
        /// 单号
        /// </summary>
        public long TradeId { get; set; }
        /// <summary>
        /// 指定字段（多个字段用","分隔。 fields 如果为空：返回所有采购单对象(purchase_orders)字段。 如果不为空：返回指定采购单对象(purchase_orders)字段。 例1： sub_purchase_orders.tc_order_id 表示只返回tc_order_id 例2： sub_purchase_orders表示只返回子采购单列表）
        /// </summary>
        public string Fields { get; set; }
    }

    public class FenXiaoTradeReslutModel
    {
        /// <summary>
        /// 错误的页码数
        /// </summary>
        public List<int> ErrorPage { get; set; }
        /// <summary>
        /// 结果
        /// </summary>
        public List<FenXiaoTradeModel> list { get; set; }
    }
}
