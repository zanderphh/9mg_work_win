using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _9M.Work.Model
{
    public class WareMeasureModel
    {
        public bool IsChecked { get; set; }
        public string BatchName { get; set; }
        public string WareNo { get; set; }
        public string WareName { get; set; }
        public int InSideGroupId { get; set; }
        public string Size { get; set; }

        public int Weight { get; set; }
    }
}
