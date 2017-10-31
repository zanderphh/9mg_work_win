using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _9M.Work.Model
{
    public class LiveGoodsModel
    {
        public int id { get; set; }

        public int serialNum { get; set; }
        public string no { get; set; }

        public string goodsno { get; set; }

        public string sku { get; set; }

        public string specName { get; set; }

        public string tbLink { get; set; }

        public string remark { get; set; }

        public bool isStop { get; set; }
    }
}
