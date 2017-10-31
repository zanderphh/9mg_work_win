using _9M.Work.Model;
using _9M.Work.Model.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _9M.Work.AOP.Goods
{
    public interface GoodsInterface
    {
        /// <summary>
        /// 记录日志与绩效
        /// </summary>
        /// <param name="LogList">日志对象</param>
        /// <param name="SqlLlist">绩效绩效</param>
        [GoodsHandler]
         void DoWork(IntercepGoodsLogModel LogModel);
        /// <summary>
        /// 提交货位
        /// </summary>
        /// <param name="list">货位POST对向</param>
        /// <param name="LogList">日志对象</param>
        /// <param name="SqlLlist">绩效绩效</param>
        /// <returns></returns>
        [GoodsHandler]
        bool SubmitTrade(List<PostSkuModel> list, IntercepGoodsLogModel LogModel);
    }
}
