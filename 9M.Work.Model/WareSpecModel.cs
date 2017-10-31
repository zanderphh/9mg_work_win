using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _9M.Work.Model
{
    public class WareSpecModel
    {
        public int Id { get; set; }
        public string WareNo { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public int SpecCode { get; set; }
        public string ImageKey { get; set; }
        public bool Activity { get; set; }
        public int Stock { get; set; }

    }
}
