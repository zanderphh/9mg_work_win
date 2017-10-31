using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.Interactivity;
using Microsoft.Practices.Unity.InterceptionExtension;
using _9M.Work.Model.Log;
using _9M.Work.DbObject;
using _9M.Work.Utility;
using System.Data;

namespace _9M.Work.AOP.Goods
{
    public class GoodsICallHandler : ICallHandler
    {
        public int Order
        {
            get; set;
        }

        public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
        {
            //0.解析参数
            var Parlist = input.MethodBase.GetParameters();
            var arrInputs = input.Inputs;
            List<GoodsLogModel> LogList = null;
            List<string> Sqlist = null;
            for (int i = 0; i < input.Arguments.Count; i++)
            {

                Type t = input.Arguments[i].GetType();
                if (t == typeof(IntercepGoodsLogModel) && Parlist[i].Name.Equals("LogModel"))
                {
                    IntercepGoodsLogModel lm = input.Arguments[i] as IntercepGoodsLogModel;
                    LogList = lm.LogList;
                    Sqlist = lm.SqlList;
                }
            }


            #region 执行方法之前的拦截

            #endregion

            //2.执行方法
            var messagereturn = getNext()(input, getNext);


            #region 执行方法之后的拦截
          
            bool DoAfter = true;
            if (messagereturn.ReturnValue != null)
            {
                string Message = messagereturn.ReturnValue.ToString();
                if (Message.Equals("True", StringComparison.CurrentCultureIgnoreCase) || Message.Equals("false", StringComparison.CurrentCultureIgnoreCase))
                {
                    bool.TryParse(messagereturn.ReturnValue.ToString(), out DoAfter);
                }
            }
            
            if (DoAfter == true)
            {
                BaseDAL Dal = new BaseDAL();
                if (LogList != null)
                {
                    if (LogList.Count > 0)
                    {
                        DataTable dt = ConvertType.ToDataTable(LogList);
                        string res = Dal.BulkCopy(dt, "L_GoodsLog");
                    }

                }
                if (Sqlist != null)
                {
                    if (Sqlist.Count > 0)
                    {
                        bool bs = Dal.ExecuteTransaction(Sqlist, null);
                    }
                }
            }

            #endregion

            return messagereturn;
        }
    }
}
