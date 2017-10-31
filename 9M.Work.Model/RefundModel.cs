using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _9M.Work.Model
{
    public class RefundModel
    {
        /// <summary>
        /// 序号
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 退货编号
        /// </summary>
        public string refundNo { get; set; }
        /// <summary>
        /// 店铺编号
        /// </summary>
        public int? shopId { get; set; }
        /// <summary>
        /// 原淘宝订单号(1个或多个)
        /// </summary>
        public string tradeNo { get; set; }
        /// <summary>
        /// 发货快递单号
        /// </summary>
        public string sendPostcode { get; set; }
        /// <summary>
        /// 退款状态(RefundSatausEnum)
        /// </summary>
        public int refundStatus { get; set; }
        /// <summary>
        /// 财务退款状态(FinanceRefund)
        /// </summary>
        public int? financeRefundStatus { get; set; }
        /// <summary>
        /// 网名呢称
        /// </summary>
        public string tbnick { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string mobile { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string address { get; set; }
        /// <summary>
        /// 真实姓名
        /// </summary>
        public string RealName { get; set; }
        /// <summary>
        /// 登记时间
        /// </summary>
        public DateTime regTime { get; set; }
        /// <summary>
        /// 登记员工
        /// </summary>
        public string regEmployee { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string remark { get; set; }
        /// <summary>
        /// 完成时间
        /// </summary>
        public DateTime? endTime { get; set; }
        /// <summary>
        /// 合计退货数量
        /// </summary>
        public int? refundAmount { get; set; }
        /// <summary>
        /// 确认收到的退货数量
        /// </summary>
        public int? confirmAmount { get; set; }
        /// <summary>
        /// 处理天数
        /// </summary>
        public int? handleDays { get; set; }
        /// <summary>
        /// 分销颜色标记
        /// </summary>
        public int? flagColor { get; set; }

        /// <summary>
        /// 拆包时间
        /// </summary>
        public DateTime? unpackingTime { get; set; }

        /// <summary>
        /// 预留字段1
        /// </summary>
        public string dColumn1 { get; set; }
        /// <summary>
        /// 预留字段2
        /// </summary>
        public string dColumn2 { get; set; }
    }
}
