using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _9M.Work.Model
{
    public class UnknownlistModel
    {
        public int? id { get; set; }
        public string refundNo { get; set; }
        public byte[] img { get; set; }
        public string title { get; set; }

        public bool? isHandle { get; set; }
    }
}
