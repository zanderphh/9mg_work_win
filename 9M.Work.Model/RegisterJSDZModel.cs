using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _9M.Work.Model
{
    public class RegisterJSDZModel
    {
        public int id { get; set; }
        public string tradeNo { get; set; }
        public string distributorNick { get; set; }
        public decimal? refundMoney { get; set; }
        public string remark { get; set; }
        public DateTime? registerTime { get; set; }
        public string registerOperator { get; set; }
        public string sku { get; set; }
        public string checkOperator { get; set; }
        public bool? isCheck { get; set; }
        public int regType { get; set; }
    }
}
