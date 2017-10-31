using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Top.Api;

namespace _9Mg.Work.TopApi
{
    public static class Config
    {
        /// <summary>
        /// 修改的respones返回值
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public static string ReslutValue(TopResponse response)
        {
            if (response.IsError == false)
            {
                return "success";
            }
            else
            {
                if (response.SubErrMsg != null)
                {
                    return response.SubErrMsg;
                }
                else
                {
                    return response.ErrMsg;
                }
            }
        }
    }
}
