using _9M.Work.DbObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _9M.Work.WPF_Main.FrameWork
{
    public class SerialNoHelp
    {
        /// <summary>
        /// 返回退货订单编号
        /// </summary>
        /// <returns></returns>
        public static string rtnRefundNo()
        {
            BaseDAL dal = new BaseDAL();
            string sql = string.Format("select count(1)+1 as RefundId from T_RefundDetail where substring(refundNo,1,8)='{0}'", DateTime.Now.ToString("yyyyMMdd"));
            int RefundNo = dal.QueryField<int>(sql, new object[] { });
            return DateTime.Now.ToString("yyyyMMdd") + RefundNo.ToString().PadLeft(4, '0') + _9M.Work.Utility.SecurityHelper.BuildRandomStr(6);
        }
    }
}
