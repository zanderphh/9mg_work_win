using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _9M.Work.Model
{
    public partial class RefundDetailModel
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
        /// 商品编号
        /// </summary>
        public string goodsno { get; set; }
        /// <summary>
        /// 货品编号
        /// </summary>
        public string sku { get; set; }
        /// <summary>
        /// 规格名称
        /// </summary>
        public string specName { get; set; }
        /// <summary>
        /// 类目名称
        /// </summary>
        public string categoryName { get; set; }
        /// <summary>
        /// 淘宝单号
        /// </summary>
        public string tbTradeNo { get; set; }
        /// <summary>
        /// 管家单号
        /// </summary>
        public string gjTradeNo { get; set; }
        /// <summary>
        /// 退货原因(买家原因\卖家原因)
        /// </summary>
        public int refundReason { get; set; }
        /// <summary>
        /// 退货描述
        /// </summary>
        public string refundDesc { get; set; }
        /// <summary>
        /// 实际退货编码/规格(退货原因为发错货的情况下面需要填写)
        /// </summary>
        public string realGoodsno { get; set; }
        /// <summary>
        /// 实际退货编码/规格(退货原因为发错货的情况下面需要填写)
        /// </summary>
        public string realSku { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string remark { get; set; }
        /// <summary>
        ///快递公司(枚举ExpressCompanyEnum)
        /// </summary>
        public string expressCompany { get; set; }
        /// <summary>
        /// 快递单号
        /// </summary>
        public string expressCode { get; set; }
        /// <summary>
        /// 拆包员工
        /// </summary>
        public string unpackingEmployee { get; set; }
        /// <summary>
        /// 拆包时间
        /// </summary>
        public DateTime? unpackingTime { get; set; }
        /// <summary>
        /// 是否退款(0否1是)
        /// </summary>
        public Boolean? isFinanceEnd { get; set; }
        /// <summary>
        /// 确认已收货(待收货&已收货&未收货 ReceiptStatus)
        /// </summary>
        public int confirmReceipt { get; set; }
        /// <summary>
        /// 商品图片地址
        /// </summary>
        public string imgUrl { get; set; }
        /// <summary>
        /// 是否为多退款(0否,1是)
        /// </summary>
        public Boolean? IsNotRegister { get; set; }
        /// <summary>
        /// 预留列
        /// </summary>
        public string dColumn1 { get; set; }

        /// <summary>
        /// 淘宝退款的ID
        /// </summary>
        public long? tbRefundId { get; set; }

        /// <summary>
        /// 退款金额(分销用->及时到帐)
        /// </summary>
        public decimal? refundMoney { get; set; }

        /// <summary>
        /// 是否及时到帐
        /// </summary>
        public Boolean? isJSDZ { get; set; }

        /// <summary>
        /// 异常处理员工
        /// </summary>
        public string exceptionEmployee { get; set; }

        /// <summary>
        /// 异常处理时间
        /// </summary>
        public DateTime? exceptionTime { get; set; }
    }
}
