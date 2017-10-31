using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _9M.Work.Model
{
    public class GoodsImageOperationModel
    {
        public int Id { get; set; }
        public string GoodsNo { get; set; }
        public long NumIid { get; set; }
        public string Title { get; set; }
        public string ItemImg { get; set; }
        public string PropImg { get; set; }
        public string DescImg { get; set; }
        public bool NeedOperation { get; set; }
        public bool OperationStatus { get; set; }
        public bool IsOnSell { get; set; }
        public string Desc { get; set; }
        public string PrimaryImage { get; set; }
    }
}
