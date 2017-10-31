using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _9M.Work.Model
{
    public class FenXiaoTradeModel
    {
        /// <summary>
        /// 分销流水单号
        /// </summary>
        public long FenxiaoId { get; set; }
        /// <summary>
        /// 主单单号
        /// </summary>
        public long TcOrderId { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public string Created { get; set; }
        /// <summary>
        /// 支付宝账号
        /// </summary>
        public String AlipayNo { get; set; }
        /// <summary>
        /// 采购总额
        /// </summary>
        public string TotalFee { get; set; }
        /// <summary>
        /// 邮费
        /// </summary>
        public string PostFee { get; set; }
        /// <summary>
        /// 分销商实付金额
        /// </summary>
        public string DistributorPayment { get; set; }
        /// <summary>
        /// 付款时间
        /// </summary>
        public String PayTime { get; set; }
        /// <summary>
        /// 供应商备注
        /// </summary>
        public String SupplierMemo { get; set; }
        /// <summary>
        /// 交易方式
        /// </summary>
        public String PayType { get; set; }
        /// <summary>
        /// 订单状态
        /// </summary>
        public String Status { get; set; }
        /// <summary>
        /// 供应商备注旗帜 (vlaue在1-5之间。非1-5之间，都采用1作为默认。 1:红色 2:黄色 3:绿色 4:蓝色 5:粉红色)
        /// </summary>
        public long SupplierFlag { get; set; }
        /// <summary>
        /// 买家支付给分销商的总金额
        /// </summary>
        public string BuyerPayment { get; set; }
        public List<FenxiaoOrder> OrderList { get; set; }

    }

    public class FenxiaoOrder
    {
        /// <summary>
        /// 商品编码
        /// </summary>
        public string ItemOuterId { get; set; }
        /// <summary>
        /// SKU编码
        /// </summary>
        public string SkuOuterId { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public string Created { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 退款金额
        /// </summary>
        public string RefundFee { get; set; }
        /// <summary>
        /// 分销平台的子采购单主键
        /// </summary>
        public long FenxiaoId { get; set; }
        /// <summary>
        /// 同FenxiaoProduct 的pid
        /// </summary>
        public long ItemId { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public long Num { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 分销商应付金额=num(采购数量)*price(采购价)
        /// </summary>
        public string TotalFee { get; set; }
        /// <summary>
        /// 分销商实付金额=total_fee（分销商应付金额）+改价-优惠
        /// </summary>
        public string DistributorPayment { get; set; }
      
    }
}
