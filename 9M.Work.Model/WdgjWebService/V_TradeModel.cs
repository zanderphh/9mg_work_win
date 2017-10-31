using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _9M.Work.Model.WdgjWebService
{
    public class V_TradeModel
    {
        public int TradeID { get; set; }
        public string TradeNO { get; set; }
        public int ShopID { get; set; }
        public int GoodsCount { get; set; }
        public DateTime? RegTime { get; set; }
        public DateTime? TradeTime { get; set; }
        public int TradeStatus { get; set; }
        public string SndTo { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string Town { get; set; }
        public string Adr { get; set; }
        public string Tel { get; set; }
        public DateTime? ConfirmTime { get; set; }
        public DateTime? ChkTime { get; set; }
        public DateTime? SndTime { get; set; }
        public string Remark { get; set; }
        public int LogisticID { get; set; }
        public string Name { get; set; }
        public string PostID { get; set; }
        public decimal GoodsTotal { get; set; }
        public decimal PostageTotal { get; set; }
        public decimal AllTotal { get; set; }
        public decimal FavourableTotal { get; set; }
        public string TradeNO2 { get; set; }
    }
}
