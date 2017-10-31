using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _9M.Work.Model
{
    public class UnknownGoodsModel
    {
        public int id { get; set; }
        public string ExpressCompany { get; set; }
        public string ExpressCode { get; set; }
        public string UName { get; set; }
        public string Mobile { get; set; }
        public string Area { get; set; }
        public DateTime? regTime { get; set; }
        public string regEmployee { get; set; }
    }
}
