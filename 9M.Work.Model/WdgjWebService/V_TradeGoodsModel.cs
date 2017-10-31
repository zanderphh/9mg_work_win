using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _9M.Work.Model.WdgjWebService
{
    public class V_TradeGoodsModel
    {
       public int TradeID { get; set; }
       public string TradeNO { get; set; }
        public int Sellcount { get; set; }
        public Double  SellPrice { get; set; }
        public Double SellTotal { get; set; }
        public int ShopID { get; set; }
        public DateTime? RegTime { get; set; }
        public DateTime? TradeTime { get; set; }
        public int TradeStatus { get; set; }
        public DateTime? ConfirmTime { get; set; }
        public DateTime? ChkTime { get; set; }
        public DateTime? SndTime { get; set; }
        public int GoodsID { get; set; }
        public int SpecID { get; set; }
        public int ClassID { get; set; }
        public string GoodsNO { get; set; }
        public string Year { get; set; }
        public string Season { get; set; }
        public string GoodsName { get; set; }
        public string ClassName { get; set; }
        public string brand { get; set; }
        public string SpecCode { get; set; }
        public string GoodsNoAll { get; set; }
    }
}
