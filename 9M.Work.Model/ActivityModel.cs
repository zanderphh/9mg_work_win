using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _9M.Work.Model
{
    public class ActivityModel
    { 
        public int Id { get; set; }
        public string ActivityNo { get; set; }
        public string ActivityName { get; set; }
        public string ActivityRealName { get; set; }
        public int Shopid { get; set; }
        public DateTime Startdate { get; set; }
        public DateTime Enddate { get; set; }
        public DateTime Createdate { get; set; }
        public bool Isactivity { get; set; }
        public int ActivityStatus { get; set; }
        public bool Supdatepost { get; set; }
        public bool Supdatedis { get; set; }
        public bool Supdateprice { get; set; }
        public bool Supdatetitle { get; set; }
        public bool Eupdatepost { get; set; }
        public bool Eupdatedis { get; set; }
        public bool Eupdateprice { get; set; }
        public bool Eupdatetitle { get; set; }
        public virtual ShopModel ShopModel { get; set; }
    }
}
