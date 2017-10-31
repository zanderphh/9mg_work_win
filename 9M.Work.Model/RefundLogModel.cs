using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _9M.Work.Model
{
    public class RefundLogModel
    {
        public int id { get; set; }

        public string oper { get; set; }

        public DateTime operTime { get; set; }

        public string eventName { get; set; }

        public string refundNo { get; set; }

        public string remark { get; set; }
    }
}
