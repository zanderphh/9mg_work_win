using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _9M.Work.Model
{
    public class AndroidPDModel
    {
        public int id { get; set; }
        public string skuinfo { get; set; }
        public string hw { get; set; }
        public int stockcount { get; set; }
        public int sndcount { get; set; }
        public int ordercount { get; set; }
        public int pdcount { get; set; }
        public string pdoperator { get; set; }
        public DateTime pdtime { get; set; }
        public Boolean? ishandle { get; set; }
        public string handleoperator { get; set; }

        
    }
}
