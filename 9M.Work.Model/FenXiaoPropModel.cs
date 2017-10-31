using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _9M.Work.Model
{
    public class FenXiaoPropModel
    {
        public int Id { get; set; }
        public string PropName { get; set; }
        public string PropValue { get; set; }
        /// <summary>
        /// 1为女装普通,2为裤子,3为童装,4女装颜色,5裤子颜色,6童装颜色
        /// </summary>
        public int PropType { get; set; }
    }
}
