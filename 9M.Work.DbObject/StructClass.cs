using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _9M.Work.DbObject
{
    /// <summary>
    /// 包含列
    /// </summary>
    [Serializable]
    public struct LeftModelField
    {
        public string PropertyName { get; set; }
    }


    /// <summary>
    /// 排序列
    /// </summary>
    [Serializable]
    public struct OrderModelField
    {
        /// <summary>
        /// 是否倒序
        /// </summary>
        public bool IsDesc { get; set; }
        /// <summary>
        /// 排序字段
        /// </summary>
        public string PropertyName { get; set; }
    }

    /// <summary>
    /// 条件表达式列
    /// </summary>
    [Serializable]
    public struct ExpressionModelField
    {
        /// <summary>
        /// 左边的值
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 右边的值
        /// </summary>
        public object Value { get; set; }
        /// <summary>
        /// 关系(等于、包含、大于、小于)
        /// </summary>
        public EnumRelation Relation { get; set; }


    }

    /// <summary>
    /// 关系枚举
    /// </summary>
    [Serializable]
    public enum EnumRelation : long
    {
        /// <summary>
        /// 等于
        /// </summary>
        Equal = 1L << 1,
        /// <summary>
        /// 不等于
        /// </summary>
        NotEqual = 1L << 2,
        /// <summary>
        /// 大于
        /// </summary>
        GreaterThan = 1L << 3,
        /// <summary>
        /// 大于和等于
        /// </summary>
        GreaterThanOrEqual = 1L << 4,
        /// <summary>
        /// 小于
        /// </summary>
        LessThan = 1L << 5,
        /// <summary>
        /// 小于和等于
        /// </summary>
        LessThanOrEqual = 1L << 6,
        /// <summary>
        /// 包含
        /// </summary>
        Contains = 1L << 7,
        /// <summary>
        /// 前端包含
        /// </summary>
        StartsWith = 1L << 8,
    }
}
