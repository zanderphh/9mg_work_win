using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _9M.Work.Model
{
    public class WebApiPostModel
    {
    }

    public class SalePostModel
    {
        public string Shop { get; set; }
        public string Brand { get; set; }
        public string Category { get; set; }
        public List<string> YearSeasonList { get; set; }
        public string WeekMin { get; set; }
        public string WeekMax { get; set; }
        public string PriceMin { get; set; }
        public string PriceMax { get; set; }
        public string StockMin { get; set; }
        public string StockMax { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string KeyWord { get; set; }
        public List<string> GoodsList { get; set; }

    }

    /// <summary>
    /// 遇状态均为 0不处理
    /// </summary>
   
}
