using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _9M.Work.Model
{
    public class FuDaiBatchModel
    {
        public int ID { get; set; }
        public string BatchName { get; set; }
        public string Brand { get; set; }
        public decimal PriceMin { get; set; }
        public decimal PriceMax { get; set; }
        public int Num { get; set; }
        public int SellCount { get; set; }
        public DateTime CreateTime { get; set; }
        public string Remark { get; set; }
        public bool ImportErp { get; set; }
    }
}
