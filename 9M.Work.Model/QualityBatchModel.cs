using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _9M.Work.Model
{
    public class QualityBatchModel
    {
        public int Id { get; set; }
        public string BatchName { get; set; }
        public string Brand { get; set; }
        public string BrandEn { get; set; }
        //对应春的年份
        public string Year1 { get; set; }
        //对应夏的年份
        public string Year2 { get; set; }
        //对应秋的年份
        public string Year3 { get; set; }
        //对应冬的年份
        public string Year4 { get; set; }
        public bool Spring { get; set; }
        public bool Summer { get; set; }
        public bool Autumn { get; set; }
        public bool Winter { get; set; }
        /// <summary>
        /// 总件数
        /// </summary>
        public int UnitCount { get; set; }
        /// <summary>
        /// 总款数
        /// </summary>
        public int WareCount { get; set; }
        /// <summary>
        ///残次品数量
        /// </summary>
        public int ImperfectCount { get; set; }
        /// <summary>
        /// 残次率
        /// </summary>
        public decimal Imperfectrate { get; set; }
        /// <summary>
        /// 单款单件数量
        /// </summary>
        public int OneononeCount { get; set; }
        /// <summary>
        /// 分款时间
        /// </summary>
        public DateTime DistinguishDate { get; set; }
        /// <summary>
        /// 入库时间
        /// </summary>
        public DateTime IntstockDate { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        public bool IsLock { get; set; }

        public bool? IsDisplay { get; set; }
    }
}
