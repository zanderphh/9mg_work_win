using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace _9M.Work.Utility
{
    public class FileHelper
    {
        private static WebClient client = null;
        /// <summary>
        /// HTTP获取文件夹中的所有文件
        /// </summary>
        /// <param name="url">网络地址</param>
        /// <returns></returns>
        public static List<string> GetImageFile(string url)
        {
             
            List<string> imgList = new List<string>();
            try
            {
                Uri siteUri = new Uri(url);
                HttpWebRequest req = (HttpWebRequest) WebRequest.Create(siteUri);
                req.AllowAutoRedirect = true;
                HttpWebResponse res = (HttpWebResponse) req.GetResponse();
                if (res.StatusCode == HttpStatusCode.OK)
                {
                    string strBuff = "";
                    char[] cbuffer = new char[256];
                    int byteRead = 0;

                    Stream respStream = res.GetResponseStream();
                    StreamReader respStreamReader = new StreamReader(respStream, System.Text.Encoding.UTF8);

                    byteRead = respStreamReader.Read(cbuffer, 0, 256);

                    while (byteRead != 0)
                    {
                        string strResp = new string(cbuffer, 0, byteRead);
                        strBuff = strBuff + strResp;
                        byteRead = respStreamReader.Read(cbuffer, 0, 256);
                    }
                    respStream.Close();
                    string lowerHTML = strBuff.ToLower();


                    Regex regImg = new Regex("<a href=\"([^\"]+)\"[^>]*>\\s*([^<]+)</a>");
                    MatchCollection matches = regImg.Matches(lowerHTML);
                    foreach (Match match in matches)
                    {
                        if (match.Groups[2].Value.Contains(".jpg"))
                        {
                            imgList.Add(match.Groups[2].Value);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return imgList;
        }


        /// <summary>
        /// 创建目录
        /// </summary>
        /// <param name="FileOrPath">文件或目录</param>
        public static void CreateDirectory(string FileOrPath)
        {
            if (FileOrPath != null)
            {
                string path;
                if (FileOrPath.Contains("."))
                    path = Path.GetDirectoryName(FileOrPath);
                else
                    path = FileOrPath;

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }
        }


        /// <summary>
        /// 上传图片
        /// </summary>
        /// <param name="hpf">上传文件类型</param>
        /// <param name="errDic">返回错误信息</param>
        /// <param name="fullFileSavaName">返回图片新名字(完整目录)</param>
        /// <param name="fileSavaName">返回图片新名字(带一层目录)</param>
        /// <returns></returns>
        public static bool UploadImage(HttpPostedFileBase hpf, ref Dictionary<string, object> errDic, ref string fullFileSavaName, ref string fileSavaName)
        {
            bool res = false;

            //判断是否有文件
            if (hpf != null && !string.IsNullOrEmpty(hpf.FileName))
            {
                //判断是否为图片
                if (hpf.ContentType.ToLower().Contains("image"))
                {
                    string folder = DateTime.Now.ToString("yyyyMMdd");
                    string uploadSavaPath = string.Format("~/Uploads/images/{0}/", folder);
                    FileHelper.CreateDirectory(HttpContext.Current.Server.MapPath(uploadSavaPath));
                    //构建新的图片名
                    string fileName = Guid.NewGuid() + Path.GetExtension(hpf.FileName);
                    fileSavaName = string.Format("{0}/{1}", folder, fileName);  //数据库记录
                    //构建图片存储完整名
                    fullFileSavaName = Path.Combine(HttpContext.Current.Server.MapPath(uploadSavaPath), fileName);
                    hpf.SaveAs(fullFileSavaName);
                    if (System.IO.File.Exists(fullFileSavaName))
                    {
                        res = true;
                    }
                    else
                    {
                        //图片上传失败
                        errDic.Add("code", "9");
                        errDic.Add("errMsg", "图片上传异常，请重新上传");
                    }
                }
                else
                {
                    //图片类型不正确
                    errDic.Add("code", "9");
                    errDic.Add("errMsg", "文件类型不正确，请选择图片类型");
                }
            }
            else
            {
                //文件不能为空
                errDic.Add("code", "9");
                errDic.Add("errMsg", "文件不能为空，请选择图片");
            }
            return res;
        }


        /// <summary>
        /// 通过FileStream 来打开文件，这样就可以实现不锁定Image文件，到时可以让多用户同时访问Image文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Bitmap ReadImageFile(string path)
        {
            FileStream fs = File.OpenRead(path); //OpenRead
            int filelength = 0;
            filelength = (int)fs.Length; //获得文件长度 
            Byte[] image = new Byte[filelength]; //建立一个字节数组 
            fs.Read(image, 0, filelength); //按字节流读取 
            System.Drawing.Image result = System.Drawing.Image.FromStream(fs);
            fs.Close();
            Bitmap bit = new Bitmap(result);
            return bit;
        }

        public static byte[] Bitmap2Byte(Bitmap bitmap)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Jpeg);
                byte[] data = new byte[stream.Length];
                stream.Seek(0, SeekOrigin.Begin);
                stream.Read(data, 0, Convert.ToInt32(stream.Length));
                return data;
            }
        }


        public static string ReadFile(string path)
        {
            string strLine;
            using (System.IO.StreamReader sr = new System.IO.StreamReader(path, Encoding.Default))
            {
                strLine = sr.ReadToEnd();
            }
            return strLine;
        }

        /// <summary>
        /// 以换行的方式读取一个文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static List<string> RealFileLine(string path)
        { 
            List<string> list = new List<string>();
            StreamReader sr = new StreamReader(path,Encoding.Default);
            String line;
            while ((line = sr.ReadLine()) != null) 
            {
                list.Add(line.TrimEnd('\r').TrimEnd('\n').TrimEnd());
            }
            sr.Close();
            return list;
        }

        /// <summary>
        /// 将一个网络图片保存为Image
        /// </summary>
        /// <param name="Url"></param>
        /// <returns></returns>
        public static Image ReadImageFormUrl(string Url)
        {
            client = new WebClient();
            var buffer = client.DownloadData(Url);
            MemoryStream ms = new MemoryStream(buffer);
            Image image = System.Drawing.Image.FromStream(ms);
            client.Dispose();
            return image;
        }

        public static bool DownloadFile(string URL, string filename)
        {
            try
            {
                System.Net.HttpWebRequest Myrq = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(URL);
                System.Net.HttpWebResponse myrp = (System.Net.HttpWebResponse)Myrq.GetResponse();
                System.IO.Stream st = myrp.GetResponseStream();
                System.IO.Stream so = new System.IO.FileStream(filename, System.IO.FileMode.Create);
                byte[] by = new byte[1024];
                int osize = st.Read(by, 0, (int)by.Length);
                while (osize > 0)
                {
                    so.Write(by, 0, osize);
                    osize = st.Read(by, 0, (int)by.Length);
                }
                so.Close();
                st.Close();
                myrp.Close();
                Myrq.Abort();
                return true;
            }
            catch (System.Exception e)
            {
                return false;
            }
        }
    }
}
