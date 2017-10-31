using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _9M.Work.Model
{
    public class StockInCaCheModel
    {
        public int Id { get; set; }
        public string BillID { get; set; }
        public DateTime BillDate { get; set; }
        public string GoodsNo { get; set; }
        public string SpecCode { get; set; }
        public string SpecName { get; set; }
        public int GoodsCount { get; set; }
        public string P_Position { get; set; }
        public string F_Position { get; set; }
        public bool GoodsStatus { get; set; }
    }
}
