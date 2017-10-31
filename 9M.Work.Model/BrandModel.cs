using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _9M.Work.Model
{
    public class BrandModel
    {
        public int Id { get; set; }
        public string BrandEN { get; set; }
        public string BrandCN { get; set; }
        public int OrderId { get; set; }
        public int Level { get; set; }
        public string SellPoint { get; set; }
        public string Sizes { get; set; }
        /// <summary>
        /// 文本眶的位置（Spec编码的数值）
        /// </summary>
        public string SizeCodes { get; set; }
    }
}
