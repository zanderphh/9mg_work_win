using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _9M.Work.Model.Statistics
{
    public class StatisticsEveryDayForBrandModel
    {
        public DateTime TradeTime { get; set; }
        public int ShopId { get; set; }
        public string Year { get; set; }
        public string Season { get; set; }
        public string Brand { get; set; }
        public decimal TotalPrice { get; set; }
        public int TotalCount { get; set; }
    }
}
