using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _9M.Work.Model
{
    public class WareLogModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string WareNo { get; set; }
        public string Content { get; set; }
        public DateTime OperationDate { get; set; }
    }
}
