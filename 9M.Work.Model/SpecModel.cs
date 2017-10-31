using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _9M.Work.Model
{
    public class SpecModel
    {
        public string GoodsNo { get; set; }
        public string GoodsName { get; set; }
        public string SpecCode { get; set; } //规格码
        public string SpecName { get; set; }//规格名
        public int Stock { get; set; }//库存
        public string SpecName1 { get; set; } //颜色
        public string SpecName2 { get; set; } //尺码
        public int StockAll { get; set; } //当前库存
        public int SndCount { get; set; } // 待发货
        public int OrderCount { get; set; } //订购量
        public string Postion { get; set; }//货位
    }
}
