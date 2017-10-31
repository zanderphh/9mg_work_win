using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _9M.Work.Model
{
    public class FinanceRefundModel
    {

        public int id { get; set; }

        public string tbNo { get; set; }

        public string tbNick { get; set; }

        public DateTime regTime { get; set; }

        public string regEmployee { get; set; }

        public string cause { get; set; }

        public string tbRemark { get; set; }

        public string remark { get; set; }

        public decimal coupon { get; set; }

        public decimal cash { get; set; }

        public int status { get; set; }

        public DateTime? endTime { get; set; }

        public string financeEmployee { get; set; }

        public string alipay { get; set; }

        public bool? isBackOperator { get; set; }

        public string couponOperator { get; set; }

        public int shopid { get; set; }

        public decimal? backMoney { get; set; }

        public string financeFlag { get; set; }
    }
}
