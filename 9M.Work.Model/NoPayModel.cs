using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _9M.Work.Model
{
    public class NoPayModel
    {
        /// <summary>
        /// 款号
        /// </summary>
        public string OuterIid { get; set; }
        /// <summary>
        /// SKU编码
        /// </summary>
        public string OuterSkuId { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public long Num { get; set; }
    }
}
