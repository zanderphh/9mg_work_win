using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _9M.Work.Model
{
    public class Erp_Model_GJRefund
    {

            public string tbTradeNo { get; set; }
            public string SendExpressNo { get; set; }
            public string GjTradeNo { get; set; }
            public string NickName { get; set; }
            public string CName { get; set; }
            public string Mobile { get; set; }
            public string Address { get; set; }
            public Int64 ShopID { get; set; }
            public string ShopName { get; set; }
            public DateTime SndTime { get; set; }
        
    }

    public class Erp_Model__GJTradeGoods
    {
        public string goodsno { get; set; }
        public string tradegoodsno { get; set; }
        public string tradespec { get; set; }
    }

    
}
