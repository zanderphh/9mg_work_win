using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _9M.Work.Model
{
    public class ActivityLogModel
    {
        public int Id { get; set; }
        public string ActivityNo { get; set; }
        public bool Finish { get; set; }
        public string GoodsNo { get; set; }
        public string ErrorMsg { get; set; }
        public int LogType { get; set; }
        public string UserName { get; set; }
        public DateTime LogTime { get; set; }

    }
}
