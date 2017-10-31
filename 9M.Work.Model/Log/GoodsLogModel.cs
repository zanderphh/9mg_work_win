using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _9M.Work.Model.Log
{
    public class GoodsLogModel
    {
        public int Id { get; set; }
        public int LogType { get; set; }
        public string UserName { get; set; }
     
        public string TradeId { get; set; }
        public string GoodsNo { get; set; }
        public string GoodsDetail { get; set; }
        public string SpecName { get; set; }
      
        public int GoodsCount { get; set; }
        public DateTime LogTime { get; set; }
        public string DoEvent { get; set; }
    }

    public class IntercepGoodsLogModel
    {
        /// <summary>
        /// 日志
        /// </summary>
         public List<GoodsLogModel> LogList { get; set; }
         /// <summary>
         /// 绩效
         /// </summary>
         public List<string> SqlList { get; set; }
    }
}
