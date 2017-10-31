using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _9M.Work.Model
{
    public class StatisticsModel
    {
        public string Item { get; set; }
        public int Count { get; set; }
        public long GJCount { get; set; }

        public long DiffVal { get; set; }

        public int InSideGroupId { get; set; }

        public decimal Price { get; set; }

        public string OriginalFyiCode { get; set; }

        public string HouDu { get; set; }
    }
}
