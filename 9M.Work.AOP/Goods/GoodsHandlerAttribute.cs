using Microsoft.Practices.Unity.InterceptionExtension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using _9M.Work.Model.Log;

namespace _9M.Work.AOP.Goods
{
    public class GoodsHandlerAttribute : HandlerAttribute
    {
        public int Order { get; set; }
        public override ICallHandler CreateHandler(IUnityContainer container)
        {
            return new GoodsICallHandler() { Order = this.Order };
        }
    }
}
