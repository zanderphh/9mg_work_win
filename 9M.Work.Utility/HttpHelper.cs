using _9M.Work.WSVariable;
using Microsoft.CSharp;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web.Services.Description;
using System.Web.Services.Protocols;
using System.Xml;
using System.Xml.Serialization;

namespace _9M.Work.Utility
{
    public class HttpHelper
    {
        private static readonly string DefaultUserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
        private static WebClient MyWebClient = new WebClient();

        private static string BaseUri;
        public HttpHelper(string baseUri)
        {
            BaseUri = baseUri;
        }
        public HttpHelper()
        {

        }
        /// <summary>
        /// 返回一个HTTP的状态
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetHead(string url)
        {
            string re = string.Empty;
            WebRequest Http = WebRequest.Create(url);
            Http.Method = "HEAD";//设置Method为HEAD
            try
            {
                HttpWebResponse response = (HttpWebResponse)Http.GetResponse();
                re = Convert.ToInt32(response.StatusCode).ToString();//Statuscode 为枚举类型，200为正常，其他输出异常，需要转为int型才会输出状态码
                response.Close();
            }
            catch (WebException ex)
            {
                re = Convert.ToInt32(((HttpWebResponse)ex.Response).StatusCode) + "  " + ((HttpWebResponse)ex.Response).StatusCode.ToString();
            }
            return re;
        }

        /// <summary>
        /// 返回一个页面的HTML代码
        /// </summary>
        /// <param name="Url">URL</param>
        /// <returns></returns>
        public static string GetElementByHttpUrl(string Url)
        {
            string pageHtml = "";
            try
            {
                MyWebClient.Credentials = CredentialCache.DefaultCredentials;
                Byte[] pageData = MyWebClient.DownloadData(Url);
                // string pageHtml = Encoding.Default.GetString(pageData);
                pageHtml = Encoding.GetEncoding("utf-8").GetString(pageData);
            }
            catch
            {
                pageHtml = "";
            }
            return pageHtml;
        }

        #region 创建一个HTTP请求返回JSON数据

        /// <summary>
        /// 创建一个HTTP请求返回JSON数据
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string GetResponseString(string url, string data)
        {
            string ResponseResult = string.Empty;

            HttpWebRequest oReq = WebRequest.Create(string.Format("{0}?{1}", url, data)) as HttpWebRequest;
            try
            {
                oReq.Method = "GET";
                HttpWebResponse myHttpWebResponse = (HttpWebResponse)oReq.GetResponse();
                string Msg = string.Empty;
                if (myHttpWebResponse.StatusCode == HttpStatusCode.OK)
                {
                    StreamReader sr = new StreamReader(myHttpWebResponse.GetResponseStream(), Encoding.UTF8);
                    ResponseResult = sr.ReadToEnd();
                }
                myHttpWebResponse.Close();
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                oReq.Abort();
            }
            return ResponseResult;
        }

        #endregion

        /// <summary>
        /// 创建GET方式的HTTP请求
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <param name="timeout">请求的超时时间</param>
        /// <param name="userAgent">请求的客户端浏览器信息，可以为空</param>
        /// <param name="cookies">随同HTTP请求发送的Cookie信息，如果不需要身份验证可以为空</param>
        /// <returns></returns>
        public static HttpWebResponse CreateGetHttpResponse(string url, int? timeout, string userAgent, CookieContainer cookies)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "GET";
            request.UserAgent = DefaultUserAgent;
            if (!string.IsNullOrEmpty(userAgent))
            {
                request.UserAgent = userAgent;
            }
            if (timeout.HasValue)
            {
                request.Timeout = timeout.Value;
            }
            if (cookies != null)
            {
                request.CookieContainer = cookies;
            }
            try
            {
                return request.GetResponse() as HttpWebResponse;
            }
            catch (WebException we)
            {
                throw we;
            }
        }
        /// <summary>
        /// 创建POST方式的HTTP请求
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <param name="parameters">随同请求POST的参数名称及参数值字典</param>
        /// <param name="timeout">请求的超时时间</param>
        /// <param name="userAgent">请求的客户端浏览器信息，可以为空</param>
        /// <param name="requestEncoding">发送HTTP请求时所用的编码</param>
        /// <param name="cookies">随同HTTP请求发送的Cookie信息，如果不需要身份验证可以为空</param>
        /// <returns></returns>
        public static HttpWebResponse CreatePostHttpResponse(string url, IDictionary<string, object> parameters, int? timeout, string userAgent, Encoding requestEncoding, CookieContainer cookies)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }
            if (requestEncoding == null)
            {
                throw new ArgumentNullException("requestEncoding");
            }
            HttpWebRequest request = null;
            //如果是发送HTTPS请求
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                request = WebRequest.Create(url) as HttpWebRequest;
                request.ProtocolVersion = HttpVersion.Version10;
            }
            else
            {
                request = WebRequest.Create(url) as HttpWebRequest;
            }
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            if (!string.IsNullOrEmpty(userAgent))
            {
                request.UserAgent = userAgent;
            }
            else
            {
                request.UserAgent = DefaultUserAgent;
            }

            if (timeout.HasValue)
            {
                request.Timeout = timeout.Value;
            }
            if (cookies != null)
            {
                request.CookieContainer = cookies;
            }
            //如果需要POST数据
            if (!(parameters == null || parameters.Count == 0))
            {
                StringBuilder buffer = new StringBuilder();
                int i = 0;
                foreach (string key in parameters.Keys)
                {
                    if (i > 0)
                    {
                        //判断参数是否int类型数组
                        if (parameters[key] is List<int>)
                        {
                            List<int> l = (List<int>)parameters[key];
                            foreach (int temp in l)
                            {
                                buffer.AppendFormat("&{0}={1}", key, temp);
                            }
                        }
                        else
                        {
                            buffer.AppendFormat("&{0}={1}", key, parameters[key]);
                        }
                    }
                    else
                    {
                        if (parameters[key] is List<int>)
                        {
                            List<int> l = (List<int>)parameters[key];
                            for (int temp = 0; temp < l.Count; temp++)
                            {
                                if (temp > 0)
                                {
                                    buffer.AppendFormat("&{0}={1}", key, l[temp]);
                                }
                                else
                                {
                                    buffer.AppendFormat("{0}={1}", key, l[temp]);
                                }
                            }
                        }
                        else
                        {
                            buffer.AppendFormat("{0}={1}", key, parameters[key]);
                        }
                    }
                    i++;
                }
                byte[] data = requestEncoding.GetBytes(buffer.ToString());
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
            }
            try
            {
                return request.GetResponse() as HttpWebResponse;
            }
            catch (WebException we)
            {
                throw we;
            }
        }

        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true; //总是接受
        }


        #region Delete方式
        public string Delete(string data, string uri)
        {
            return CommonHttpRequest(data, uri, "DELETE");
        }

        public static string Delete(string uri)
        {
            //Web访问对象64
            string serviceUrl = string.Format("{0}/{1}", BaseUri, uri);
            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(serviceUrl);
            myRequest.Method = "DELETE";
            // 获得接口返回值68
            HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();
            StreamReader reader = new StreamReader(myResponse.GetResponseStream(), Encoding.UTF8);
            //string ReturnXml = HttpUtility.UrlDecode(reader.ReadToEnd());
            string ReturnXml = reader.ReadToEnd();
            reader.Close();
            myResponse.Close();
            return ReturnXml;
        }
        #endregion

        #region Put方式
        public static string Put(string data, string uri)
        {
            return CommonHttpRequest(data, uri, "PUT");
        }
        #endregion

        #region POST方式实现

        public string Post(string data, string uri)
        {
            return CommonHttpRequest(data, uri, "POST");
        }


        public static string CommonHttpRequest(string data, string uri, string type)
        {
            //Web访问对象，构造请求的url地址
            // string serviceUrl = string.Format("{0}/{1}", this.BaseUri, uri);

            //构造http请求的对象
            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(uri);
            //转成网络流
            byte[] buf = System.Text.Encoding.GetEncoding("UTF-8").GetBytes(data);
            //设置
            myRequest.Method = type;
            myRequest.ContentLength = buf.Length;
            myRequest.ContentType = "application/json";
            myRequest.MaximumAutomaticRedirections = 1;
            myRequest.AllowAutoRedirect = true;
            // 发送请求
            Stream newStream = myRequest.GetRequestStream();
            newStream.Write(buf, 0, buf.Length);
            newStream.Close();
            // 获得接口返回值
            HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();
            StreamReader reader = new StreamReader(myResponse.GetResponseStream(), Encoding.UTF8);
            string ReturnXml = reader.ReadToEnd();
            reader.Close();
            myResponse.Close();
            return ReturnXml;
        }
        #endregion

        #region GET方式实现
        public string Get(string uri)
        {
            //Web访问对象64
            string serviceUrl = string.Format("{0}/{1}", BaseUri, uri);

            //构造一个Web请求的对象
            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(serviceUrl);
            //myRequest.Timeout = 6000;
            // 获得接口返回值68
            //获取web请求的响应的内容
            string ReturnXml = string.Empty;
            try
            {
                HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();
                //通过响应流构造一个StreamReader
                StreamReader reader = new StreamReader(myResponse.GetResponseStream(), Encoding.UTF8);
                //string ReturnXml = HttpUtility.UrlDecode(reader.ReadToEnd());
                ReturnXml = reader.ReadToEnd();
                reader.Close();
                myResponse.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }


            return ReturnXml;
        }
        #endregion


        /// <summary>
        /// 得到WINFORM的客户端请求地址
        /// </summary>
        /// <param name="ControllerName">控制器名</param>
        /// <param name="Method">请求的方法名</param>
        /// <returns></returns>
        public static string WinRestUrL(string ControllerName, string MethodName)
        {
            string Url = "http://my.9mg.cc/new/api";
            // string Url = "http://localhost:11744/api";
            if (!string.IsNullOrEmpty(ControllerName) && !string.IsNullOrEmpty(MethodName))
            {
                Url = Url + "/" + ControllerName + "/" + MethodName;
            }
            return Url;
        }



        /// <summary>
        /// 得到请求的Key
        /// </summary>
        public static string JuShiTaRequestKey
        {
            get
            {
                return "0X00E045443B12BC";
            }
        }

        public static Image LoadImageFromUrl(string Url)
        {
            if (GetHead(Url).Equals("200"))
            {
                WebRequest webRequest = WebRequest.Create(Url);
                HttpWebRequest request = webRequest as HttpWebRequest;
                WebResponse response = request.GetResponse();
                Stream stream = response.GetResponseStream();
                return Image.FromStream(stream);
            }
            else
            {
                return null;
            }
        }

        public static string PostStream(string url, string[] files, System.Collections.Specialized.NameValueCollection nvc)
        {
            MemoryStream memsStream = new MemoryStream();
            string FileHeaderName = "media";
            string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\" Content-Type:application/octet-stream\r\n\r\n";
            for (int i = 0; i < files.Length; i++)
            {

                string header = string.Format(headerTemplate, FileHeaderName, files[i]);
                byte[] headerbytes = System.Text.Encoding.GetEncoding("utf-8").GetBytes(header);
                FileStream fileStream = new FileStream(files[i], FileMode.Open, FileAccess.Read);
                byte[] buffer = new byte[1024];
                int bytesRead = 0;
                while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    memsStream.Write(buffer, 0, bytesRead);
                }
            }
            return null;
        }

        public static object InvokeWebMethod(string _url, string _methodName,
                                params object[] _params)
        {
            WebClient client = new WebClient();
            //String url = "http://localhost:3182/Service1.asmx?WSDL";//这个地址可以写在Config文件里面
            Stream stream = client.OpenRead(_url);
            ServiceDescription description = ServiceDescription.Read(stream);

            ServiceDescriptionImporter importer = new ServiceDescriptionImporter();//创建客户端代理代理类。
            importer.ProtocolName = "Soap"; //指定访问协议。
            importer.Style = ServiceDescriptionImportStyle.Client; //生成客户端代理。
            importer.CodeGenerationOptions = CodeGenerationOptions.GenerateProperties |
                CodeGenerationOptions.GenerateNewAsync;
            importer.AddServiceDescription(description, null, null); //添加WSDL文档。
            CodeNamespace nmspace = new CodeNamespace(); //命名空间
            nmspace.Name = "CITI_WS_InWH1";      //这个命名空间可以自己取
            CodeCompileUnit unit = new CodeCompileUnit();
            unit.Namespaces.Add(nmspace);
            ServiceDescriptionImportWarnings warning = importer.Import(nmspace, unit);
            CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
            CompilerParameters parameter = new CompilerParameters();
            parameter.GenerateExecutable = false;
            parameter.OutputAssembly = "MyTest.dll";//输出程序集的名称
            parameter.ReferencedAssemblies.Add("System.dll");
            parameter.ReferencedAssemblies.Add("System.XML.dll");
            parameter.ReferencedAssemblies.Add("System.Web.Services.dll");
            parameter.ReferencedAssemblies.Add("System.Data.dll");
            CompilerResults result = provider.CompileAssemblyFromDom(parameter, unit);
            if (result.Errors.HasErrors)
            {
                // 显示编译错误信息             
            }
            Assembly asm = Assembly.LoadFrom("MyTest.dll");//加载前面生成的程序集
            Type t = asm.GetType("CITI_WS_InWH1.wsTransData_InWH"); //前面的命名空间.类名,类必须是webservice中定义的
            object o = Activator.CreateInstance(t);
            MethodInfo method = t.GetMethod(_methodName);//GetPersons是服务端的方法名称,你想调用服务端的什么方法都可以在这里改,最好封装一下
            object item = method.Invoke(o, _params); //注：method.Invoke(o, null)返回的是一个Object,如果你服务端返回的是DataSet,这里也是用(DataSet)method.Invoke(o, null)转一下就行了
            //foreach (string str in item)
            //    Console.WriteLine(str);               //上面是根据WebService地址，模似生成一个代理类,如果你想看看生成的代码文件是什么样子，可以用以下代码保存下来，默认是保存在bin目录下面
            //TextWriter writer = File.CreateText("MyTest.cs");
            //provider.GenerateCodeFromCompileUnit(unit, writer, null);
            //writer.Flush();
            //writer.Close();

            return item;
        }

        public static object InvokeWebService(string url, string methodname, object[] args)
        {
            return HttpHelper.InvokeWebService(url, null, methodname, args);
        }
        /// < summary>          
        /// 动态调用web服务 
        /// < /summary>          
        /// < param name="url">WSDL服务地址< /param>
        /// < param name="classname">类名< /param>  
        /// < param name="methodname">方法名< /param>  
        /// < param name="args">参数< /param> 
        /// < returns>< /returns>
        public static object InvokeWebService(string url, string classname, string methodname, object[] args)
        {
            string @namespace = "EnterpriseServerBase.WebService.DynamicWebCalling";
            if ((classname == null) || (classname == ""))
            {
                classname = HttpHelper.GetWsClassName(url);
            }
            try
            {                   //获取WSDL   
                WebClient wc = new WebClient();
                Stream stream = wc.OpenRead(url + "?WSDL");
                ServiceDescription sd = ServiceDescription.Read(stream);
                ServiceDescriptionImporter sdi = new ServiceDescriptionImporter();
                sdi.AddServiceDescription(sd, "", "");
                CodeNamespace cn = new CodeNamespace(@namespace);
                //生成客户端代理类代码          
                CodeCompileUnit ccu = new CodeCompileUnit();
                ccu.Namespaces.Add(cn);
                sdi.Import(cn, ccu);
                CSharpCodeProvider icc = new CSharpCodeProvider();
                //设定编译参数                 
                CompilerParameters cplist = new CompilerParameters();
                cplist.GenerateExecutable = false;
                cplist.GenerateInMemory = true;
                cplist.ReferencedAssemblies.Add("System.dll");
                cplist.ReferencedAssemblies.Add("System.XML.dll");
                cplist.ReferencedAssemblies.Add("System.Web.Services.dll");
                cplist.ReferencedAssemblies.Add("System.Data.dll");
                //编译代理类                 
                CompilerResults cr = icc.CompileAssemblyFromDom(cplist, ccu);
                if (true == cr.Errors.HasErrors)
                {
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    foreach (System.CodeDom.Compiler.CompilerError ce in cr.Errors)
                    {
                        sb.Append(ce.ToString());
                        sb.Append(System.Environment.NewLine);
                    }
                    throw new Exception(sb.ToString());
                }
                //生成代理实例，并调用方法   
                System.Reflection.Assembly assembly = cr.CompiledAssembly;
                Type t = assembly.GetType(@namespace + "." + classname, true, true);
                object obj = Activator.CreateInstance(t);
                System.Reflection.MethodInfo mi = t.GetMethod(methodname);
                return mi.Invoke(obj, args);
                // PropertyInfo propertyInfo = type.GetProperty(propertyname);     
                //return propertyInfo.GetValue(obj, null); 
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException.Message, new Exception(ex.InnerException.StackTrace));
            }
        }



        private static string GetWsClassName(string wsUrl)
        {
            string[] parts = wsUrl.Split('/');
            string[] pps = parts[parts.Length - 1].Split('.');
            return pps[0];
        }

        #region 2016-08-22 by wl

        // ===============================================================================
        // 函数用途：WebService验证
        // 创 建 人：WL
        // 修改时间：2016-08-22
        // ===============================================================================

        #region soapheader InvokeWebService

        public static object InvokeWebService(string url, string methodname, object[] args, WSSoapHeader soapHeader)
        {
            return HttpHelper.InvokeWebService(url, null, methodname, args, soapHeader);
        }

        #endregion

        #region soapheader

        /// <summary>     
        /// 调用WebService  
        /// </summary>     
        /// <param name="wsUrl">WebService地址</param>     
        /// <param name="className">类名</param>     
        /// <param name="methodName">方法名称</param>     
        /// <param name="soapHeader">SOAP头</param>  
        /// <param name="args">参数列表</param>     
        /// <returns>返回调用结果</returns>     
        public static object InvokeWebService(string wsUrl, string className, string methodName, object[] args, WSSoapHeader soapHeader)
        {
            string @namespace = "EnterpriseServerBase.WebService.DynamicWebCalling";
            if ((className == null) || (className == ""))
            {
                className = GetWsClassName(wsUrl);
            }
            try
            {
                //获取WSDL     
                WebClient wc = new WebClient();
                Stream stream = wc.OpenRead(wsUrl + "?wsdl");
                ServiceDescription sd = ServiceDescription.Read(stream);
                ServiceDescriptionImporter sdi = new ServiceDescriptionImporter();
                sdi.AddServiceDescription(sd, "", "");
                CodeNamespace cn = new CodeNamespace(@namespace);

                //生成客户端代理类代码     
                CodeCompileUnit ccu = new CodeCompileUnit();
                ccu.Namespaces.Add(cn);
                sdi.Import(cn, ccu);
                CSharpCodeProvider icc = new CSharpCodeProvider();

                //设定编译参数     
                CompilerParameters cplist = new CompilerParameters();
                cplist.GenerateExecutable = false;
                cplist.GenerateInMemory = true;
                cplist.ReferencedAssemblies.Add("System.dll");
                cplist.ReferencedAssemblies.Add("System.XML.dll");
                cplist.ReferencedAssemblies.Add("System.Web.Services.dll");
                cplist.ReferencedAssemblies.Add("System.Data.dll");

                //编译代理类     
                CompilerResults cr = icc.CompileAssemblyFromDom(cplist, ccu);
                if (true == cr.Errors.HasErrors)
                {
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    foreach (System.CodeDom.Compiler.CompilerError ce in cr.Errors)
                    {
                        sb.Append(ce.ToString());
                        sb.Append(System.Environment.NewLine);
                    }
                    throw new Exception(sb.ToString());
                }


                //生成代理实例，并调用方法     
                System.Reflection.Assembly assembly = cr.CompiledAssembly;
                Type t = assembly.GetType(@namespace + "." + className, true, true);

                FieldInfo[] arry = t.GetFields();

                FieldInfo client = null;
                object clientkey = null;
                if (soapHeader != null)
                {
                    //Soap头开始     
                    client = t.GetField(soapHeader.ClassName + "Value");

                    //获取客户端验证对象     
                    Type typeClient = assembly.GetType(@namespace + "." + soapHeader.ClassName);

                    //为验证对象赋值     
                    clientkey = Activator.CreateInstance(typeClient);

                    foreach (KeyValuePair<string, object> property in soapHeader.Properties)
                    {
                        typeClient.GetField(property.Key).SetValue(clientkey, property.Value);
                    }
                    //Soap头结束     
                }

                //实例类型对象     
                object obj = Activator.CreateInstance(t);

                if (soapHeader != null)
                {
                    //设置Soap头  
                    client.SetValue(obj, clientkey);
                }

                System.Reflection.MethodInfo mi = t.GetMethod(methodName);

                return mi.Invoke(obj, args);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException.Message, new Exception(ex.InnerException.StackTrace));
            }
        }

        #endregion


        #endregion



    }


}
