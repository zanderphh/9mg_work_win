using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _9Mg.Work.JosApi
{
    public static class Config
    {
        private static string _appkey = "test";//系统分配 test
        /// <summary>
        /// *系统定义 App Key
        /// </summary>
        public static string Appkey
        {
            get
            {
                if (System.Configuration.ConfigurationManager.AppSettings["JosAppKey"] != null)
                {
                    _appkey = System.Configuration.ConfigurationManager.AppSettings["JosAppKey"];
                    return _appkey;
                }
                else
                {
                    return _appkey;
                }
            }
        }

        private static string _secret = "test";//系统分配 test
        /// <summary>
        /// * 系统定义  App Secret
        /// </summary>
        public static string Secret
        {
            set { _secret = value; }
            get
            {
                if (System.Configuration.ConfigurationManager.AppSettings["JosAppSecret"] != null)
                {
                    _secret = System.Configuration.ConfigurationManager.AppSettings["JosAppSecret"];
                    return _secret;
                }
                else
                {
                    return _secret;
                }
            }
        }

        private static string _sessoinkey = "test";//系统分配 test
        /// <summary>
        /// * 系统定义 SessionKey
        /// </summary>
        public static string SessionKey
        {
            set { _sessoinkey = value; }
            get
            {
                if (System.Configuration.ConfigurationManager.AppSettings["JosSessionKey"] != null)
                {
                    _sessoinkey = System.Configuration.ConfigurationManager.AppSettings["JosSessionKey"];
                    return _sessoinkey;
                }
                else
                {
                    return _sessoinkey;
                }
            }
        }

        /// <summary>
        /// 数据调用入口 
        /// </summary>
        public static string Url
        {
            get
            {
               // return " http://jos.jd.com/";
                return "https://api.jd.com/routerjson";
            }
        }
    }
}
