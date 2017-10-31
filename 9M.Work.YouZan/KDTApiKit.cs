using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Net;
using System.IO;

namespace _9M.Work.YouZan
{
    public class KDTApiKit
    {
        const String Version = "1.0";

        static String apiEntry = "http://open.koudaitong.com/api/entry";

        String format = "json";

        String signMethod = "md5";

        String appId, appSecret;

        private static readonly string DefaultUserAgent = "KdtApiSdk Client v0.1";

        public KDTApiKit(String appId, String appSecret)
        {
            if (appId == "" || appSecret == "")
            {
                throw new Exception("appId和appSecret不能为空");
            }
            this.appId = appId;
            this.appSecret = appSecret;
        }

        public String get(String method, Dictionary<String,String> parames)
        {
            Dictionary<String, String> completeParam = buildCompleteParams(method, parames);
            String paramStr = buildParamStr(completeParam);
            HttpWebRequest request = WebRequest.Create(apiEntry + "?" + paramStr) as HttpWebRequest;
            request.Method = "GET";
            request.UserAgent = DefaultUserAgent;
            HttpWebResponse result = request.GetResponse() as HttpWebResponse;
            StreamReader sr = new StreamReader(result.GetResponseStream(), Encoding.GetEncoding("utf-8"));
            string strResult = sr.ReadToEnd();
            sr.Close();
            //Console.WriteLine(strResult);
            return strResult;
        }

        public String post(String method, Dictionary<String, String> parames,Dictionary<String,object> files,String fileKey)
        {
            Dictionary<String, String> completeParam = buildCompleteParams(method, parames);
            String paramStr = buildParamStr(completeParam);
            HttpWebRequest request;
            if (files == null || files.Count == 0)
            {
                request = WebRequest.Create(apiEntry + "?" + paramStr) as HttpWebRequest;
                request.Method = "POST";
                request.UserAgent = DefaultUserAgent;
            }
            else 
            {
                request = KDTUtil.MultipartFormDataPost(apiEntry + "?" + paramStr, DefaultUserAgent, files, fileKey);
            }

            HttpWebResponse result = request.GetResponse() as HttpWebResponse;
            StreamReader sr = new StreamReader(result.GetResponseStream(), Encoding.UTF8);
            string strResult = sr.ReadToEnd();
            sr.Close();
            Console.WriteLine(strResult);
            return strResult;
        }

       

        String buildParamStr(Dictionary<String, String> param)
        {
            String paramStr = String.Empty;
            foreach (var key in param.Keys.ToList())
            {
                if (param.Keys.ToList().IndexOf(key) == 0)
                {
                    paramStr += (key + "=" + param[key]);
                }
                else
                {
                    paramStr += ("&" + key + "=" + param[key]);
                }
            }

            return paramStr;
        }


        Dictionary<String,String> buildCompleteParams(String method, Dictionary<String,String> parames)
        {
            Dictionary<String, String> commonParams = getCommonParams(method);
            foreach (var key in parames.Keys)
            { 
                if(commonParams.ContainsKey(key))
                    throw new Exception("参数名冲突");

                commonParams.Add(key,parames[key]);
            }
            commonParams.Add(KDTUtil.SIGN_KEY, KDTUtil.sign(appSecret, commonParams, method));
            return commonParams;
        }

        Dictionary<String, String> getCommonParams(String method)
        {
           Dictionary<String, String> parames = new Dictionary<String, String>();
            parames.Add(KDTUtil.APP_ID_KEY, appId);
            parames.Add(KDTUtil.METHOD_KEY, method);
            parames.Add(KDTUtil.TIMESTAMP_KEY, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            parames.Add(KDTUtil.FORMAT_KEY, format);
            parames.Add(KDTUtil.SIGN_METHOD_KEY, signMethod);
            parames.Add(KDTUtil.VERSION_KEY, Version);
            return parames;
        }
    

    }
}
