using _9M.Work.Model;
using _9M.Work.Model.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _9M.Work.AOP.Goods
{
    public interface PrintInterface
    {
        PrintType PrintT { get; set; }
        [GoodsHandler]
        void PrintLabe(PrintLabeModel model, int count, bool DoublePrint, IntercepGoodsLogModel LogModel);
        [GoodsHandler]
        void PrintLabe(List<PrintLabeModel> modellist, bool DoublePrint, IntercepGoodsLogModel LogModel);
    }
}
