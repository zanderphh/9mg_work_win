using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _9M.Work.Model
{
    public class AndroidScanReceiptModel
    {
        public int id { get; set; }
        public string scanNo { get; set; }
        public string scanOpt { get; set; }
        public DateTime scanTime { get; set; }
        public string remark { get; set; }
        public bool? isEmployee { get; set; }
    }
}
