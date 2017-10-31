using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _9M.Work.Model.WdgjWebService
{
    public class V_GoodsModel
    {
        public int GoodsID { get; set; }

        public string GoodsNO { get; set; }
        public string Year { get; set; }
        public string Season { get; set; }
        public string brand { get; set; }
        public int ClassID { get; set; }
        public string ClassName { get; set; }
        public string GoodsName { get; set; }
        public string GoodsName2 { get; set; }
        public long StockALL { get; set; }
        public decimal Price_Member { get; set; }
        public decimal Price1 { get; set; }
        public string Reserved3 { get; set; }
        public string Remark { get; set; }
        public DateTime? SellDate { get; set; }
        public bool bBlockUp { get; set; }
    }
}
