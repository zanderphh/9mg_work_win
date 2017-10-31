using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _9M.Work.Model
{
    public class FuDaiGoodsModel
    {
        public int ID { get; set; }
        public int BatchID { get; set; }
        public string GoodsNo { get; set; }
        public string SpecCode { get; set; }
        public string GoodsALL { get; set; }
        public string Brand { get; set; }
        public string CategoryName { get; set; }
        public string Class { get; set; }
        public decimal Price { get; set; }
        public string Size { get; set; }
        public string ImageUrl { get; set; }
        /// <summary>
        /// 销售次数
        /// </summary>
        public int SellMore { get; set; }
        /// <summary>
        /// 是否销售
        /// </summary>
        public bool IsSell { get; set; }

        public DateTime? CreateTime { get; set; }
    }
}
