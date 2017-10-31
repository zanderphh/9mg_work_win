using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _9M.Work.Model
{
    public class WareModel
    {
        public int Id { get; set; }
        /// <summary>
        /// 批次号
        /// </summary>
        public int BatchId { get; set; }
        /// <summary>
        /// 款号
        /// </summary>
        public string WareNo { get; set; }
        /// <summary>
        /// 品名
        /// </summary>
        public string WareName { get; set; }
        /// <summary>
        /// 内部号
        /// </summary>
        public int InSideGroupId { get; set; }
        /// <summary>
        /// 原吊牌号
        /// </summary>
        public string OriginalFyiCode { get; set; }
        /// <summary>
        /// 年份
        /// </summary>
        public string Years { get; set; }
        /// <summary>
        /// 季节
        /// </summary>
        public int Season { get; set; }
        /// <summary>
        ///分类ID
        /// </summary>
        public int CategoryId { get; set; }
       
        public string XiuXing { get; set; }
        public string MianLiao { get; set; }
        public string YanSe { get; set; }
        public string MenJin { get; set; }
        public string LingXing { get; set; }
        public string QiTa { get; set; }

        public string ImageKey { get; set; }

        /// <summary>
        /// 是否己测量
        /// </summary>
        public bool IsMeasure { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 厚度
        /// </summary>
        public string HouDu { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 价格
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 重量
        /// </summary>
        public string Weight { get; set; }
    }
}
