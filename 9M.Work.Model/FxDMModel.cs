using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _9M.Work.Model
{
    public class FxDMModel
    {
        public int id { get; set; }

        public string code { get; set; }

        public decimal money { get; set; }

        public string distributor { get; set; }

        public bool isUse { get; set; }

        public DateTime? useTime { get; set; }

        public bool isEnd { get; set; }

        public DateTime? endTime { get; set; }

        public bool isGrant { get; set; }

        public DateTime? grantTime { get; set; }


    }
}
