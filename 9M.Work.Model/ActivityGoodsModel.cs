using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _9M.Work.Model
{
    public class ActivityGoodsModel
    {
        public int Id { get; set; }
        public string ActivityNo { get; set; }
        public string GoodsNo {get;set;}
        public decimal Defaultprice { get; set; }
        public string Defaulttitle { get; set; }
        public decimal Activityprice { get; set; }
        public string Activittitle { get; set; }
    }
}
