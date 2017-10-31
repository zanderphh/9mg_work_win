using System;
using System.Collections.Generic;

using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace _9M.Work.Utility
{
    /// <summary>
    /// 序列化公用类
    /// </summary>
    public class SerializationHelper
    {
        private SerializationHelper()
        {
            //构造函数        
        }
        private static Dictionary<int, XmlSerializer> serializer_dict = new Dictionary<int, XmlSerializer>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static XmlSerializer GetSerializer(Type t)
        {
            int type_hash = t.GetHashCode();

            if (!serializer_dict.ContainsKey(type_hash))
                serializer_dict.Add(type_hash, new XmlSerializer(t));

            return serializer_dict[type_hash];
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="type">对象类型</param>
        /// <param name="filename">文件路径</param>
        /// <returns></returns>
        public static object Load(Type type, string filename)
        {
            FileStream fs = null;
            try
            {
                // open the stream...
                fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                XmlSerializer serializer = new XmlSerializer(type);
                return serializer.Deserialize(fs);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (fs != null)
                    fs.Close();
            }
        }


        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="filename">文件路径</param>
        public static bool Save(object obj, string filename)
        {
            bool success = false;

            FileStream fs = null;
            // serialize it...
            try
            {
                fs = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                XmlSerializer serializer = new XmlSerializer(obj.GetType());
                serializer.Serialize(fs, obj);
                success = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (fs != null)
                    fs.Close();
            }
            return success;

        }

        /// <summary>
        /// xml序列化成字符串
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns>xml字符串</returns>
        public static string Serialize(object obj)
        {
            string returnStr = "";
            XmlSerializer serializer = GetSerializer(obj.GetType());
            MemoryStream ms = new MemoryStream();
            XmlTextWriter xtw = null;
            StreamReader sr = null;
            try
            {
                xtw = new System.Xml.XmlTextWriter(ms, Encoding.UTF8);
                xtw.Formatting = System.Xml.Formatting.Indented;
                serializer.Serialize(xtw, obj);
                ms.Seek(0, SeekOrigin.Begin);
                sr = new StreamReader(ms);
                returnStr = sr.ReadToEnd();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (xtw != null)
                    xtw.Close();
                if (sr != null)
                    sr.Close();
                ms.Close();
            }
            return returnStr;
        }
        

        public static object DeSerialize(Type type, string s)
        {
            byte[] b = System.Text.Encoding.UTF8.GetBytes(s);
            try
            {
                XmlSerializer serializer = GetSerializer(type);
                return serializer.Deserialize(new MemoryStream(b));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// 将字节数组转为ASCII字符(是否进行Gzip压缩)
        /// </summary>
        /// <param name="MessageArray">字节数组</param>
        /// <param name="isgzip">是否启用gzip</param>
        /// <returns>返回字符</returns>
        public static string Serializable_Model(byte[] MessageArray, bool isgzip)
        {
            if (isgzip)
            {
                MemoryStream ms = new MemoryStream();
                Stream s = new GZipStream(ms, CompressionMode.Compress);
                s.Write(MessageArray, 0, MessageArray.Length);
                s.Close();
                MessageArray = (byte[])ms.ToArray();
                ms.Dispose();
                ms.Close();
                s.Dispose();
            }
            return Serializable_Model(MessageArray);
        }
        /// <summary>
        /// 序列化Model类
        /// </summary>
        /// <param name="MBx"></param>
        /// <returns>字符数组</returns>
        public static byte[] Serializable_Model<T>(List<T> list)
        {
            IFormatter formatter = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            byte[] b;
            formatter.Serialize(ms, list);
            ms.Position = 0;
            b = new byte[ms.Length];
            ms.Read(b, 0, b.Length);
            ms.Close();
            return b;
        }

        /// <summary>
        /// 将字节数组转为ASCII字符
        /// </summary>
        /// <param name="MessageArray">字节数组</param>
        /// <returns></returns>
        public static string Serializable_Model(byte[] MessageArray)
        {
            return Convert.ToBase64String(MessageArray);
        }

        /// <summary>
        /// 反序列化MessageBox类
        /// </summary>
        /// <param name="BytArray">字节内容</param>
        /// <returns></returns>
        public static List<T> Deserialize_Model<T>(byte[] BytArray)
        {
            IFormatter formatter = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            ms.Write(BytArray, 0, BytArray.Length);
            ms.Position = 0;
            List<T> list = (List<T>)formatter.Deserialize(ms);
            return list;
        }

        /// <summary>
        /// 将字符按ASCII转为字节数组
        /// </summary>
        /// <param name="Messages">字符</param>
        /// <returns>字节流</returns>
        public static byte[] Deserialize_Model(string Messages)
        {
            return Convert.FromBase64String(Messages);
        }

        /// <summary>
        /// 将字符按ASCII转为字节数组(是否进行Gzip压缩)
        /// </summary>
        /// <param name="Messages">字符</param>
        /// <param name="isgzip">字符是否进行过gzip压缩</param>
        /// <returns>字节流</returns>
        public static byte[] Deserialize_Model(string Messages, bool isgzip)
        {
            byte[] BytArray = Deserialize_Model(Messages);
            if (isgzip)
            {
                MemoryStream ms = new MemoryStream();
                GZipStream s = new GZipStream(new MemoryStream(BytArray), CompressionMode.Decompress);
                byte[] bytes = new byte[4096];
                int n;
                while ((n = s.Read(bytes, 0, bytes.Length)) != 0)
                {
                    ms.Write(bytes, 0, n);
                }

                s.Close();
                BytArray = (byte[])ms.ToArray();
                ms.Dispose();
                ms.Close();
                s.Dispose();
            }
            return BytArray;
        }

        #region 可序列化对象到byte数组的相互转换

        /// <summary>
        /// 将可序列化对象转成Byte数组
        /// </summary>
        /// <param name="o">对象</param>
        /// <returns>返回相关数组</returns>
        public static byte[] ObjectToByteArray(object o)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, o);
                return ms.ToArray();
            }
        }
        /// <summary>
        /// 将可序列化对象转成的byte数组还原为对象
        /// </summary>
        /// <param name="b">byte数组</param>
        /// <returns>相关对象</returns>
        public static object ByteArrayToObject(byte[] b)
        {
            using (MemoryStream ms = new MemoryStream(b, 0, b.Length))
            {
                BinaryFormatter bf = new BinaryFormatter();
                return bf.Deserialize(ms);
            }
        }

        /// <summary>
        /// 将可序列化对象转成的str还原为对象
        /// </summary>
        /// <param name="str">可序列化字符串</param>
        /// <returns>相关对象</returns>
        public static object StringToObject(string str)
        {
            byte[] b = System.Text.Encoding.UTF8.GetBytes(str);

            System.Buffer.BlockCopy(str.ToCharArray(), 0, b, 0, b.Length);

            using (MemoryStream ms = new MemoryStream(b, 0, b.Length))
            {
                BinaryFormatter bf = new BinaryFormatter();
                return bf.Deserialize(ms);
            }
        }

        #endregion

    }
}
