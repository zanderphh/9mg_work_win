using _9M.Work.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _9M.Work.WPF_Common.ValueObjects
{

    /// <summary>
    /// 退款处理状态
    /// </summary>
    public enum RefundSatausEnum
    {
        /// <summary>
        /// 全部退货单
        /// </summary>
        [Text("全部退货单")]
        Default = -1,

        /// <summary>
        /// 未拆包
        /// </summary>
        [Text("未拆包")]
        noUnpacking = 0,

        /// <summary>
        /// 退货异常
        /// </summary>
        [Text("退货异常")]
        refundException = 1,

        /// <summary>
        /// 财务退款
        /// </summary>
        [Text("财务退款")]
        financeOperator = 2,

        /// <summary>
        /// 财务部分退款
        /// </summary>
        [Text("部分退款")]
        financePartOperator = 3,

        /// <summary>
        /// 退邮费
        /// </summary>
        [Text("退邮费")]
        Postage = 4,

        /// <summary>
        /// 退货完成
        /// </summary>
        [Text("退货完成")]
        End = 5,

        /// <summary>
        /// 退货关闭
        /// </summary>
        [Text("退货关闭")]
        Close = 6
    }

    public enum FxWorkOrderEnum
    {
        /// <summary>
        /// 所有工单
        /// </summary>
        [Text("所有工单")]
        Default = -1,
        /// <summary>
        /// 未处理
        /// </summary>
        [Text("未处理")]
        Untreated = 0,
        /// <summary>
        /// 正在处理
        /// </summary>
        [Text("正在处理")]
        BeingProcessed = 1,
        /// <summary>
        /// 等待跟踪
        /// </summary>
        [Text("等待跟踪")]
        WaitingTracking = 2,
        /// <summary>
        /// 已完成
        /// </summary>
        [Text("已完成")]
        end = 3
    }

    /// <summary>
    /// 财务退款状态
    /// </summary>
    public enum FinanceRefundStatusEnum
    {
        /// <summary>
        /// 未退款
        /// </summary>
        [Text("未退款")]
        noRefund = 0,
        /// <summary>
        /// 退款中
        /// </summary>
        [Text("退款中")]
        Refunding = 1,
        /// <summary>
        /// 退款完成
        /// </summary>
        [Text("退款完成")]
        RefundEnd = 2

    }


    /// <summary>
    /// 退款操作处理
    /// </summary>
    public enum RefundHandleStatusEnum
    {
        /// <summary>
        /// 拆包核验
        /// </summary>
        [Text("拆包核验")]
        UnpackingCheck = 0,
        /// <summary>
        /// 异常处理
        /// </summary>
        [Text("异常处理")]
        ExceptionHandler = 1,
        /// <summary>
        /// 财务退款
        /// </summary>
        [Text("财务退款")]
        FinancialRefund = 2,
        /// <summary>
        /// 财务部分退款
        /// </summary>
        [Text("部分退款")]
        FinancialPartRefund = 3,
        /// <summary>
        /// 财务退款(邮费)
        /// </summary>
        [Text("退邮费")]
        RefundPostCode = 4,
        /// <summary>
        /// 完成
        /// </summary>
        [Text("完成")]
        look = 5
    }

    public enum ExpressCompanyEnum
    {
        /// <summary>
        /// 请选择
        /// </summary>
        [Text("-请选择-")]
        DEFAULT = -1,
        /// <summary>
        /// 自提
        /// </summary>
        [Text("自提")]
        SELF = 1,
        /// <summary>
        /// 中通快递
        /// </summary>
        [Text("中通快递")]
        ZT = 0,
        /// <summary>
        /// 申通快递
        /// </summary>
        [Text("申通快递")]
        ST = 2,
        /// <summary>
        /// 顺丰快递
        /// </summary>
        [Text("顺丰快递")]
        SF = 3,
        /// <summary>
        /// 邮政快递
        /// </summary>
        [Text("邮政快递")]
        YZ = 4,
        /// <summary>
        /// EMS
        /// </summary>
        [Text("EMS")]
        EMS = 5,
        /// <summary>
        /// 圆通
        /// </summary>
        [Text("圆通")]
        YT = 6,
        /// <summary>
        /// 韵达
        /// </summary>
        [Text("韵达")]
        YD = 7,
        /// <summary>
        /// 国通
        /// </summary>
        [Text("国通")]
        GT = 8,
        /// <summary>
        /// 快捷
        /// </summary>
        [Text("快捷")]
        KJ = 9,        /// <summary>
        /// 优速
        /// </summary>
        [Text("优速")]
        YS = 10,
        /// 龙邦
        /// </summary>
        [Text("龙邦")]
        NB = 11,
        /// 宅急送
        /// </summary>
        [Text("宅急送")]
        ZJS = 12,
        /// 平邮
        /// </summary>
        [Text("平邮")]
        PY = 13,
        /// 全峰
        /// </summary>
        [Text("全峰")]
        QF = 14,
        /// 全峰
        /// </summary>
        [Text("德邦")]
        DB = 15,
        /// 天天
        /// </summary>
        [Text("天天")]
        TT = 16,
        /// 汇通
        /// </summary>
        [Text("汇通")]
        HT = 17,
        /// <summary>
        /// 其它
        /// </summary>
        [Text("其它")]
        OTHER = 99
    }


    public enum RefundReason
    {
        /// <summary>
        /// 买家原因
        /// </summary>
        [Text("买家原因")]
        buyer = 0,
        /// <summary>
        /// 卖家原因
        /// </summary>
        [Text("卖家原因")]
        seller = 1
    }

    public enum ReceiptStatus
    {
        /// <summary>
        /// 待收货
        /// </summary>
        [Text("待收货")]
        watting = 0,
        /// <summary>
        /// 已收货
        /// </summary>
        [Text("已收货")]
        yes = 1,
        /// <summary>
        /// 未收货
        /// </summary>
        [Text("未收货")]
        no = 2,
        /// <summary>
        /// 异常处理完毕
        /// </summary>
        [Text("异常处理完毕")]
        exceptionEnd = 3

    }




    public enum FlagColorEnume
    {
        /// <summary>
        /// 无标记
        /// </summary>
        [Text("无标记")]
        noFlag = 0,
        /// <summary>
        /// 颜色A
        /// </summary>
        [Text("待跟踪")]
        AColor = 2,
        /// <summary>
        /// 颜色B
        /// </summary>
        [Text("确认完成")]
        BColor = 3

    }


    public enum FilterEnum
    {
        /// <summary>
        /// 全部
        /// </summary>
        [Text("全部")]
        All = 1,
        /// <summary>
        /// 多退款
        /// </summary>
        [Text("多退款")]
        Many = 2,
        /// <summary>
        /// 少退款
        /// </summary>
        [Text("少退款")]
        Less = 3
    }


    public enum RefundOperatorLog
    {
        /// <summary>
        /// 登记
        /// </summary>
        [Text("登记")]
        Reg = 1,
        /// <summary>
        /// 添加多退款
        /// </summary>
        [Text("添加多退款")]
        AddMuilt = 2,
        /// <summary>
        /// 添加未知款
        /// </summary>
        [Text("添加未知款")]
        AddNothing = 3,
        /// <summary>
        /// 异常处理
        /// </summary>
        [Text("异常处理[多退款]")]
        ExceptionMuiltHandle = 4,
        /// <summary>
        /// 财务退款
        /// </summary>
        [Text("财务退款")]
        FinanceHandle = 5,
        /// <summary>
        /// 删除
        /// </删除>
        [Text("删除")]
        Delete = 6,
        /// <summary>
        /// 确认到货
        /// </删除>
        [Text("确认收货")]
        Receive = 7,
        /// <summary>
        /// 确认到货
        /// </删除>
        [Text("确认未到货")]
        NoReceive = 8

    }


    public enum RefundOperatorEnum
    {
        /// <summary>
        /// 确认到货
        /// </summary>
        [Text("确认到货")]
        Receive_Yes = 1,
        /// <summary>
        /// 未知款处理
        /// </summary>
        [Text("未知款处理")]
        Nothing = 2,
        /// <summary>
        /// 未收到货
        /// </summary>
        [Text("未收到货")]
        Receive_No = 3,
        /// <summary>
        /// 多退款待处理
        /// </summary>
        [Text("多退款待处理")]
        Mulit_Wait_Handle = 4,
        /// <summary>
        /// 已转即时到帐
        /// </summary>
        [Text("已转即时到帐")]
        JSDZ_YES = 5,
        /// <summary>
        /// 已完成
        /// </summary>
        [Text("已完成")]
        End = 6,
        /// <summary>
        /// 异常已处理
        /// </summary>
        [Text("异常已处理")]
        Exception_YES = 7,
        /// <summary>
        /// 确认退款
        /// </summary>
        [Text("确认退款")]
        Confirm_Refund = 8,
        /// <summary>
        /// 转即时到帐
        /// </summary>
        [Text("转即时到帐")]
        Confirm_JSDZ = 9,
        /// <summary>
        /// 未知款待处理
        /// </summary>
        [Text("未知款待处理")]
        Nothing_Wait = 10,
        /// <summary>
        /// 取消确认
        /// </summary>
        [Text("取消确认")]
        Receive_Cancel = 11,
        /// <summary>
        /// 未收到处理
        /// </summary>
        [Text("未收到处理")]
        Receive_No_Handle = 12,
        /// <summary>
        /// 退邮费
        /// </summary>
        [Text("退邮费")]
        Refund_Post = 13,
        /// <summary>
        /// 登记
        /// </summary>
        [Text("登记")]
        Register = 14,
        /// <summary>
        /// 编辑订单-引用快递单号
        /// </summary>
        [Text("编辑订单-引用快递")]
        Edit_Cites = 15,
        /// <summary>
        /// 编辑订单-添加多退款商品
        /// </summary>
        [Text("添加多退款商品")]
        AddMuilt = 16,
        /// <summary>
        /// 编辑订单-删除商品
        /// </summary>
        [Text("删除商品")]
        Delete = 17,
        /// <summary>
        /// 修改商品信息
        /// </summary>
        [Text("修改商品信息")]
        UpdateInfo = 18,
        /// <summary>
        /// 多退款处理
        /// </summary>
        [Text("多退款处理")]
        Mulit_Handle = 19,
        /// <summary>
        /// 修改退货原因
        /// </summary>
        [Text("修改退货原因")]
        updateRefundReaon = 20,
        /// <summary>
        /// 转异常
        /// </summary>
        [Text("转异常")]
        GoException = 21,
        /// <summary>
        /// 转拆包
        /// </summary>
        [Text("转拆包")]
        GoUnpacking = 22,
        /// <summary>
        /// 转财务
        /// </summary>
        [Text("转财务")]
        GoFinance = 23,
        /// <summary>
        /// 批量修改邮单
        /// </summary>
        [Text("批量修改邮单")]
        BatchUpdatePostCode = 24,
        /// <summary>
        /// 转部分退款
        /// </summary>
        [Text("转部分退款")]
        goPartRefund = 25,
        /// <summary>
        /// 转完成
        /// </summary>
        [Text("转完成")]
        goEnd = 26,
        /// <summary>
        /// 转多退款处理
        /// </summary>
        [Text("转多退款处理")]
        goMuiltHandle = 27,
        /// <summary>
        /// 批量确认退款完成
        /// </summary>
        [Text("批量确认退款完成")]
        batchConfirmFinanceEnd = 28,
        /// <summary>
        /// 合并订单
        /// </summary>
        [Text("合并订单")]
        merge = 29,
        /// <summary>
        /// 拆单
        /// </summary>
        [Text("拆单")]
        Split = 30,
        /// <summary>
        /// 待跟踪
        /// </summary>
        [Text("待跟踪")]
        WattingTack = 31,
        /// <summary>
        /// 待跟踪
        /// </summary>
        [Text("确认完成")]
        AuditEnd = 32,
        /// <summary>
        /// 取消标记
        /// </summary>
        [Text("取消标记")]
        CancelFlag = 33

    }

    public enum JSDZ_CheckStatusEnum
    {
        /// <summary>
        /// 全部
        /// </summary>
        [Text("全部")]
        All = -1,
        /// <summary>
        /// 已审核
        /// </summary>
        [Text("已审核")]
        YES = 1,
        /// <summary>
        /// 未审核
        /// </summary>
        [Text("未审核")]
        NO = 0
    }


    public enum JSDZ_RegisterTypeEnum
    {

        /// <summary>
        /// 请选择
        /// </summary>
        [Text("请选择")]
        SELECTED = -1,
        /// <summary>
        /// 退货
        /// </summary>
        [Text("退货")]
        TH = 1,
        /// <summary>
        /// 优惠
        /// </summary>
        [Text("优惠")]
        YH = 2,
        /// <summary>
        /// 退差价
        /// </summary>
        [Text("退差价")]
        TCJ = 3,
        /// <summary>
        /// 退邮费
        /// </summary>
        [Text("退邮费")]
        TYF = 4,
        /// <summary>
        /// 其它
        /// </summary>
        [Text("其它")]
        OTHER = 5
    }

    public enum ActivityType
    {
        /// <summary>
        /// 请选择
        /// </summary>
        [Text("请选择")]
        SELECTED = -1,
        /// <summary>
        /// 满减
        /// </summary>
        [Text("满额优惠")]
        FullCut = 1,
        /// <summary>
        /// 数量递减
        /// </summary>
        [Text("满件优惠")]
        CountCut = 2,
        /// <summary>
        /// 指定金额
        /// </summary>
        [Text("金额优惠")]
        Specify = 3,
        /// <summary>
        /// 折扣
        /// </summary>
        [Text("包邮优惠")]
        Post = 4,
        /// <summary>
        /// 其它
        /// </summary>
        [Text("其它")]
        OTHER = 5
    }

    public enum ActivityStatus
    {
        /// <summary>
        /// 请选择
        /// </summary>
        [Text("请选择")]
        SELECTED = -1,
        /// <summary>
        /// 待开始
        /// </summary>
        [Text("待开始")]
        Wait = 1,
        /// <summary>
        /// 进行中
        /// </summary>
        [Text("进行中")]
        Doing = 2,
        /// <summary>
        /// 待处理(商品异常)
        /// </summary>
        [Text("待处理")]
        WaitOperation = 3,
        /// <summary>
        /// 完成
        /// </summary>
        [Text("完成")]
        Finish = 4,
    }

    public enum FinanceRefundEnum
    {
        /// <summary>
        /// 组长待处理
        /// </summary>
        [Text("组长待处理")]
        Groupleader = 1,
        /// <summary>
        /// 财务待处理
        /// </summary>
        [Text("财务待处理")]
        Finance = 2,
        /// <summary>
        /// 已完成
        /// </summary>
        [Text("已完成")]
        end = 3
    }


    public enum FinanceRefundCauseEnum
    {
        请选择 = 0,
        质量问题 = 1,
        瑕疵问题 = 2,
        色差 = 3,
        发错颜色 = 4,
        发错尺码 = 5,
        发错款 = 6,
        页面描述问题 = 7,
        配饰问题 = 8,
        中差评补偿 = 9,
        尺码不符 = 10,
        退差价 = 11,
        发出少件 = 12,
        寄回少件 = 13,
        垫付邮费 = 14,
        严重破损全额补偿 = 15,
        缺货补偿 = 16,
        其他 = 17,
        绿色处理通道 = 18
    }

}
