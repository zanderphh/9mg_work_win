using _9M.Work.AOP.Goods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _9M.Work.AOP
{
    public class UnityAopFactory
    {
        public static GoodsInterface GoodsServices { get; set; }  //商品
        public static PrintInterface PrintLabServices { get; set; } //打印标签
    }
}
