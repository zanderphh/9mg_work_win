using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _9M.Work.Model
{
    public class CategoryModel
    {
        public int Id { get; set; }
        public string CategoryName { get; set; }
        public int CategoryCodeMin { get; set; }
        public int CategoryCodeMax { get; set; }
        public bool XiuXing { get; set; }
        public bool MianLiao { get; set; }
        public bool YanSe { get; set; }
        public bool MenJin { get; set; }
        public bool LingXing { get; set; }
        public bool QiTa { get; set; }
        public string TbClassName { get; set; }
        public long TbCid { get; set; }
        public string TbSkuSizeProp { get; set; }
    }
}
