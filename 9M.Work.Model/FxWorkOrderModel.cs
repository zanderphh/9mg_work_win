using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _9M.Work.Model
{
    public class FxWorkOrderModel
    {
        public int id { get; set; }
        public string questionId { get; set; }
        public string status { get; set; }
        public string questionType { get; set; }
        public string tradeId { get; set; }
        public string questionDesc { get; set; }

        public string manualInput { get; set; }
        public string operatorEmp { get; set; }
        public string aliWang { get; set; }

        public DateTime? submitTime { get; set; }
        public DateTime? operatorTime { get; set; }
        public DateTime? endTime { get; set; }

        public string isEnd { get; set; }

    }
}
