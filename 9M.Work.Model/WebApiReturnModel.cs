using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _9M.Work.Model
{
    public class WebApiReturnModel
    {
    }

    public class SaleModel
    {
        public string GoodsNo { get; set; }
        public int Stock { get; set; }
        public string Price { get; set; }
        public Decimal Price_Detail { get; set; }
        public int SellCount { get; set; }
        public decimal SellTotal { get; set; }
        public int PurCount { get; set; }
        public int BackCount { get; set; }
        public DateTime? SellDate { get; set; }
        public int Week { get; set; }
    } 
}
