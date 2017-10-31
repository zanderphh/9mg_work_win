using _9M.Work.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _9M.Work.WPF_Common
{
    public class CommonLogin
    {
        //连接字符
        public static string SqlConnString { get; set; }
        //用户名
        public static SessionUserModel CommonUser { get; set; }

        /// <summary>
        /// 为用户登陆加入淘宝JD权限
        /// </summary>
        /// <param name="User"></param>
        /// <returns></returns>
        public static void  AssembleAuthorization(SessionUserModel User)
        {
            string CAppKey = "12253100";
            string CAppSecret = "e7dd7a0cad3eaf2beaa9f7308527f29f";
            string TaobaoUrl = "http://gw.api.taobao.com/router/rest";
            User.CShop = new OnlineStoreAuthorization() { AppKey = CAppKey, AppSecret = CAppSecret, Url = TaobaoUrl, SessionKey = "6100002914160e7428b1cb891b61515dd7a1da962d290f373900259" };
            User.TmallMg = new OnlineStoreAuthorization() { AppKey = CAppKey, AppSecret = CAppSecret, Url = TaobaoUrl, SessionKey = "61006227a981ed835d5ccbe01c17bf1265e7c8cacd0c3c92104595454" };
            User.TmallMezingr = new OnlineStoreAuthorization() { AppKey = CAppKey, AppSecret = CAppSecret, Url = TaobaoUrl, SessionKey = "6101c129a822426fb8b517a96e591ea6b1039f667e8281b1087950190" };
            User.CAino = new OnlineStoreAuthorization() { AppKey = CAppKey, AppSecret = CAppSecret, Url = TaobaoUrl, SessionKey = "6100d0502f7ce13853f052fc15f9e10cae5f296166a0a0325412303" };
            User.JDShop = new OnlineStoreAuthorization() { AppKey = "40E118FA79BB166F8BB52C2EFE08D631",AppSecret = "893856554f6246319f988a96c1862178",SessionKey= "cddbccaf-2fce-40d3-85e5-6fc498bace95" };
        }

        public static string RemoteUser = "administrator";
        public static string RemotePassWork = "admin@www.9mg.cn";
        public static string RemoteDir = @"\\192.168.1.2\wwwroot\InSideWorkImage";
        public static string RemoteIp = @"\\192.168.1.2";
    }

}
