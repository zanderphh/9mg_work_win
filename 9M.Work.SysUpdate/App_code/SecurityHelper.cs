using System;
using System.Collections.Generic;

using System.Web;
using System.Text.RegularExpressions;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace SysUpdate
{
    public class SecurityHelper
    { /// <summary>
        /// 封装数据安全方面的操作
        /// </summary>

        /// <summary>
        /// MD5 加密
        /// </summary>
        /// <param name="passWord">需要加密的字符串</param>
        /// <returns>加密后的字符串</returns>
        public static string Md5(string passWord)
        {
            byte[] b = Encoding.UTF8.GetBytes(passWord);
            b = new MD5CryptoServiceProvider().ComputeHash(b);
            string ret = "";
            for (int i = 0; i < b.Length; i++)
                ret += b[i].ToString("x").PadLeft(2, '0');
            return ret;
            //Byte[] clearBytes = new UnicodeEncoding().GetBytes(passWord);
            //Byte[] hashedBytes = ((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(clearBytes);
            //return BitConverter.ToString(hashedBytes);
        }

        /// <summary>
        /// 取随机数(数字)
        /// </summary>
        /// <param name="length">随机数长度</param>
        /// <returns></returns>
        public static string BuildRandomStr(int length)
        {
            Random rand = new Random();
            int num = rand.Next();
            string str = num.ToString();
            if (str.Length > length)
            {
                str = str.Substring(0, length);
            }
            else if (str.Length < length)
            {
                int n = length - str.Length;
                while (n > 0)
                {
                    str += "2";
                    n--;
                }
            }
            return str;
        }

        private int rep = 0;

        /// <summary>
        /// 生成随机字母字符串(数字字母混和)
        /// </summary>
        /// <param name="codeCount">待生成的位数</param>
        /// <param name="rep">遍历生成随机传遍历当前数</param>
        /// <returns>生成的字母字符串</returns>
        public static string GenerateCheckCode(int codeCount, int rep)
        {
            string str = string.Empty;
            long num2 = DateTime.Now.Ticks + rep;
            Random random = new Random(((int)(((ulong)num2) & 0xffffffffL)) | ((int)(num2 >> rep)));
            for (int i = 0; i < codeCount; i++)
            {
                char ch;
                int num = random.Next();
                if ((num % 2) == 0)
                {
                    ch = (char)(0x30 + ((ushort)(num % 10)));
                }
                else
                {
                    ch = (char)(0x41 + ((ushort)(num % 0x1a)));
                }
                str = str + ch.ToString();
            }
            return str;
        }

        /// <summary>
        /// SHA1 加密
        /// </summary>
        /// <param name="passWord">需要加密的字符串</param>
        /// <returns>加密后的字符串</returns>
        public static string Sha1(string passWord)
        {
            Byte[] clearBytes = new UnicodeEncoding().GetBytes(passWord);
            Byte[] hashedBytes = ((HashAlgorithm)CryptoConfig.CreateFromName("SHA1")).ComputeHash(clearBytes);
            return BitConverter.ToString(hashedBytes);
        }

        //默认密钥向量
        private static byte[] Keys = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
        private static string DESKey = "abcedied";

        /**/
        /**/
        /**/
        /// <summary>
        /// DES加密字符串
        /// </summary>
        /// <param name="encryptString">待加密的字符串</param>
        /// <returns>加密成功返回加密后的字符串，失败返回源串</returns>
        public static string Encode(string encryptString)
        {
            try
            {
                if (String.IsNullOrEmpty(encryptString))
                {
                    return encryptString;
                }
                byte[] rgbKey = Encoding.UTF8.GetBytes(DESKey.Substring(0, 8));
                byte[] rgbIV = Keys;
                byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);
                DESCryptoServiceProvider dCSP = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, dCSP.CreateEncryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Convert.ToBase64String(mStream.ToArray());
            }
            catch
            {
                return encryptString;
            }
        }

        /**/
        /**/
        /**/
        /// <summary>
        /// DES解密字符串
        /// </summary>
        /// <param name="decryptString">待解密的字符串</param>
        /// <returns>解密成功返回解密后的字符串，失败返源串</returns>
        public static string Decode(string decryptString)
        {
            try
            {
                if (String.IsNullOrEmpty(decryptString))
                {
                    return decryptString;
                }
                byte[] rgbKey = Encoding.UTF8.GetBytes(DESKey);
                byte[] rgbIV = Keys;
                byte[] inputByteArray = Convert.FromBase64String(decryptString);
                DESCryptoServiceProvider DCSP = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, DCSP.CreateDecryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Encoding.UTF8.GetString(mStream.ToArray());
            }
            catch
            {
                return decryptString;
            }
        }

        /// <summary>
        /// 对输入框的特殊字串进行过滤，防止SQL注入
        /// </summary>
        /// <param name="strFromText">要被过滤的字符串</param>
        /// <returns>过滤后的字符串</returns>
        public static string SqlInsertEncode(string strFromText)
        {
            if (!System.String.IsNullOrEmpty(strFromText) && strFromText != "")
            {
                System.Text.RegularExpressions.Regex regex1 =
                    new System.Text.RegularExpressions.Regex(@"<script[\s\S]+</script *>",
                        System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                System.Text.RegularExpressions.Regex regex2 =
                    new System.Text.RegularExpressions.Regex(@" href *= *[\s\S]*script *:",
                        System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                System.Text.RegularExpressions.Regex regex3 =
                    new System.Text.RegularExpressions.Regex(@" no[\s\S]*=",
                        System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                System.Text.RegularExpressions.Regex regex4 =
                    new System.Text.RegularExpressions.Regex(@"<iframe[\s\S]+</iframe *>",
                        System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                System.Text.RegularExpressions.Regex regex5 =
                    new System.Text.RegularExpressions.Regex(@"<frameset[\s\S]+</frameset *>",
                        System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                System.Text.RegularExpressions.Regex regex6 =
                            new System.Text.RegularExpressions.Regex(@"\<img[^\>]+\>",
                                System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                System.Text.RegularExpressions.Regex regex7 =
                    new System.Text.RegularExpressions.Regex(@"</p>",
                        System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                System.Text.RegularExpressions.Regex regex8 =
                    new System.Text.RegularExpressions.Regex(@"<p>",
                        System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                System.Text.RegularExpressions.Regex regex9 =
                    new System.Text.RegularExpressions.Regex(@"<[^>]*>",
                        System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                strFromText = regex1.Replace(strFromText, ""); //过滤<script></script>标记 
                strFromText = regex2.Replace(strFromText, ""); //过滤href=javascript: (<A>) 属性 
                strFromText = regex3.Replace(strFromText, " _disibledevent="); //过滤其它控件的on...事件 
                strFromText = regex4.Replace(strFromText, ""); //过滤iframe
                strFromText = regex5.Replace(strFromText, ""); //过滤frameset 
                strFromText = regex6.Replace(strFromText, ""); //过滤frameset
                strFromText = regex7.Replace(strFromText, ""); //过滤frameset
                strFromText = regex8.Replace(strFromText, ""); //过滤frameset
                strFromText = regex9.Replace(strFromText, "");

                strFromText = strFromText.Replace("&nbsp;", "");
                //  strFromText = strFromText.Replace(" ", "");
                strFromText = strFromText.Replace("</strong>", "");
                strFromText = strFromText.Replace("<strong>", "");

                strFromText = strFromText.Replace("javascript", "java脚本");
                strFromText = strFromText.Replace("vbscript", "vb脚本");
                strFromText = strFromText.Replace("cookie", "小甜饼");
                strFromText = strFromText.Replace("document", "文档");

                strFromText = strFromText.Replace("!", "&#33;");
                strFromText = strFromText.Replace("@", "&#64;");
                strFromText = strFromText.Replace("$", "&#36;");
                strFromText = strFromText.Replace("*", "&#42;");
                strFromText = strFromText.Replace("(", "&#40;");
                strFromText = strFromText.Replace(")", "&#41;");
                strFromText = strFromText.Replace("-", "&#45;");
                strFromText = strFromText.Replace("+", "&#43;");
                strFromText = strFromText.Replace("=", "&#61;");
                strFromText = strFromText.Replace("|", "&#124;");
                strFromText = strFromText.Replace("\\", "&#92;");
                strFromText = strFromText.Replace("/", "&#47;");
                strFromText = strFromText.Replace(":", "&#58;");
                strFromText = strFromText.Replace("\"", "&#34;");
                strFromText = strFromText.Replace("'", "&#39;");
                strFromText = strFromText.Replace("<", "&#60;");
                //  strFromText = strFromText.Replace(" ", "&#32;");
                strFromText = strFromText.Replace(">", "&#62;");
                //  strFromText = strFromText.Replace(" ", "&#32;");
                strFromText = strFromText.Replace("<br/>", "\r\n");
                strFromText = strFromText.Replace("●", "");
        
            }

            return strFromText;
        }

        /// <summary>
        /// 对输入框的特殊字串进行过滤，防止SQL注入,不包括符号的转意（除’以外）
        /// </summary>
        /// <param name="strFromText">要被过滤的字符串</param>
        /// <returns>过滤后的字符串</returns>
        public static string SqlInsertEncodebyScript(string strFromText)
        {
            if (!System.String.IsNullOrEmpty(strFromText) && strFromText != "")
            {
                System.Text.RegularExpressions.Regex regex1 =
                    new System.Text.RegularExpressions.Regex(@"<script[\s\S]+</script *>",
                        System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                System.Text.RegularExpressions.Regex regex2 =
                    new System.Text.RegularExpressions.Regex(@" href *= *[\s\S]*script *:",
                        System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                System.Text.RegularExpressions.Regex regex3 =
                    new System.Text.RegularExpressions.Regex(@" no[\s\S]*=",
                        System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                System.Text.RegularExpressions.Regex regex4 =
                    new System.Text.RegularExpressions.Regex(@"<iframe[\s\S]+</iframe *>",
                        System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                System.Text.RegularExpressions.Regex regex5 =
                    new System.Text.RegularExpressions.Regex(@"<frameset[\s\S]+</frameset *>",
                        System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                System.Text.RegularExpressions.Regex regex6 =
                            new System.Text.RegularExpressions.Regex(@"\<img[^\>]+\>",
                                System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                System.Text.RegularExpressions.Regex regex7 =
                    new System.Text.RegularExpressions.Regex(@"</p>",
                        System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                System.Text.RegularExpressions.Regex regex8 =
                    new System.Text.RegularExpressions.Regex(@"<p>",
                        System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                System.Text.RegularExpressions.Regex regex9 =
                    new System.Text.RegularExpressions.Regex(@"<[^>]*>",
                        System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                strFromText = regex1.Replace(strFromText, ""); //过滤<script></script>标记 
                strFromText = regex2.Replace(strFromText, ""); //过滤href=javascript: (<A>) 属性 
                strFromText = regex3.Replace(strFromText, " _disibledevent="); //过滤其它控件的on...事件 
                strFromText = regex4.Replace(strFromText, ""); //过滤iframe
                strFromText = regex5.Replace(strFromText, ""); //过滤frameset 
                strFromText = regex6.Replace(strFromText, ""); //过滤frameset
                strFromText = regex7.Replace(strFromText, ""); //过滤frameset
                strFromText = regex8.Replace(strFromText, ""); //过滤frameset
                strFromText = regex9.Replace(strFromText, "");

                strFromText = strFromText.Replace("&nbsp;", "");
                strFromText = strFromText.Replace("</strong>", "");
                strFromText = strFromText.Replace("<strong>", "");

                strFromText = strFromText.Replace("javascript", "java脚本");
                strFromText = strFromText.Replace("vbscript", "vb脚本");
                strFromText = strFromText.Replace("cookie", "小甜饼");
                strFromText = strFromText.Replace("document", "文档");

                strFromText = strFromText.Replace("'", "&#39;");
            }

            return strFromText;
        }

        /// <summary>
        /// 对输入框的特殊字串进行过滤，防止SQL注入
        /// </summary>
        /// <param name="strFromText">要被过滤的字符串</param>
        /// <returns>过滤后的字符串</returns>
        public static string SqlInsertReEncode(string strFromText)
        {
            if (!System.String.IsNullOrEmpty(strFromText) && strFromText != "")
            {

                strFromText = strFromText.Replace("java脚本", "javascript");
                strFromText = strFromText.Replace("vb脚本", "vbscript");
                strFromText = strFromText.Replace("小甜饼", "cookie");
                strFromText = strFromText.Replace("文档", "document");

                strFromText = strFromText.Replace("&#33;", "!");
                strFromText = strFromText.Replace("&#64;", "@");
                strFromText = strFromText.Replace("&#36;", "$");
                strFromText = strFromText.Replace("&#42;", "*");
                strFromText = strFromText.Replace("&#40;", "(");
                strFromText = strFromText.Replace("&#41;", ")");
                strFromText = strFromText.Replace("&#45;", "-");
                strFromText = strFromText.Replace("&#43;", "+");
                strFromText = strFromText.Replace("&#61;", "=");
                strFromText = strFromText.Replace("&#124;", "|");
                strFromText = strFromText.Replace("&#92;", "\\");
                strFromText = strFromText.Replace("&#47;", "/");
                strFromText = strFromText.Replace("&#58;", ":");
                strFromText = strFromText.Replace("&#34;", "\"");
                strFromText = strFromText.Replace("&#39;", "'");
                strFromText = strFromText.Replace("&#60;", "<");
                //  strFromText = strFromText.Replace(" ", "&#32;");
                strFromText = strFromText.Replace("&#62;", ">");
                //  strFromText = strFromText.Replace(" ", "&#32;");
                strFromText = strFromText.Replace("\r\n", "<br/>");
            }

            return strFromText;
        }

        /// <summary>
        /// 对输入框的特殊字串进行过滤，防止SQL注入
        /// </summary>
        /// <param name="Str"></param>
        /// <returns></returns>
        public static string SqlInsertCheck(string strFromText)
        {
            if (!System.String.IsNullOrEmpty(strFromText) && strFromText != "")
            {
                strFromText = strFromText.Replace("'", "‘");

            }
            return strFromText;
        }

        #region 格式化字符串,符合SQL语句


        /// <summary>
        /// 对输入框的特殊字串进行反过滤，用于显示
        /// </summary>
        /// <param name="Str"></param>
        /// <returns></returns>
        public static string SqlInsertReCheck(string strFromText)
        {
            if (!System.String.IsNullOrEmpty(strFromText) && strFromText != "")
            {
                //strFromText = strFromText.Replace("，", ",");
                //strFromText = strFromText.Replace("；", ";");
                strFromText = strFromText.Replace("‘", "'");
            }
            return strFromText;
        }

        /// <summary>
        /// 查询SQL语句,删除一些SQL注入问题
        /// </summary>
        /// <param name="formatStr">需要格式化的字符串</param>
        /// <returns></returns>
        public static string querySQL(string formatStr)
        {
            string rStr = formatStr;
            if (rStr != null && rStr != "")
            {
                rStr = rStr.Replace("'", "");
            }
            return rStr;
        }
        #endregion


        /// <summary>
        /// 检测是否有Sql危险字符
        /// </summary>
        /// <param name="str">要判断字符串</param>
        /// <returns>判断结果</returns>
        public static bool IsSafeSqlString(string str)
        {
            return !Regex.IsMatch(str, @"[-|;|,|\/|\(|\)|\[|\]|\}|\{|%|@|\*|!|\']");
        }

        /// <summary>
        /// 检测是否有危险的可能用于链接的字符串
        /// </summary>
        /// <param name="str">要判断字符串</param>
        /// <returns>判断结果</returns>
        public static bool IsSafeUserInfoString(string str)
        {
            return !Regex.IsMatch(str, @"^\s*$|^c:\\con\\con$|[%,\*" + "\"" + @"\s\t\<\>\&]|游客|^Guest");
        }

        /// <summary>
        /// 是否为ip
        /// </summary>
        /// <param name="ip">ip字符串</param>
        /// <returns>判断结果</returns>
        public static bool IsIP(string ip)
        {
            return Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        }

        /// <summary>
        /// 是否为ip
        /// </summary>
        /// <param name="ip">ip字符串</param>
        /// <returns>判断结果</returns>
        public static bool IsIPSect(string ip)
        {
            return Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){2}((2[0-4]\d|25[0-5]|[01]?\d\d?|\*)\.)(2[0-4]\d|25[0-5]|[01]?\d\d?|\*)$");
        }

        /// <summary>
        /// 返回指定IP是否在指定的IP数组所限定的范围内, IP数组内的IP地址可以使用*表示该IP段任意, 例如192.168.1.*
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="iparray"></param>
        /// <returns></returns>
        public static bool InIPArray(string ip, string[] iparray)
        {
            string[] userip = StringHelper.SplitString(ip, @".");

            for (int ipIndex = 0; ipIndex < iparray.Length; ipIndex++)
            {
                string[] tmpip = StringHelper.SplitString(iparray[ipIndex], @".");
                int r = 0;
                for (int i = 0; i < tmpip.Length; i++)
                {
                    if (tmpip[i] == "*")
                        return true;

                    if (userip.Length > i)
                    {
                        if (tmpip[i] == userip[i])
                            r++;
                        else
                            break;
                    }
                    else
                        break;
                }

                if (r == 4)
                    return true;
            }
            return false;
        }


        /// 过滤HTML中的不安全标签
        /// </summary>
        /// <param name="content">HTML内容</param>
        /// <returns>过滤后的内容</returns>
        public static string RemoveUnsafeHtml(string content)
        {
            content = Regex.Replace(content, @"(\<|\s+)o([a-z]+\s?=)", "$1$2", RegexOptions.IgnoreCase);
            content = Regex.Replace(content, @"(script|frame|form|meta|behavior|style)([\s|:|>])+", "$1.$2", RegexOptions.IgnoreCase);
            return content;
        }

        #region "3des密钥"
        /// <summary>
        /// 3des密钥
        /// </summary>
        public const string EncryptKey = "100F1COM1HGBZMXNCHFYHGBO";
        #endregion

        #region "3des加密字符串"
        /// <summary>
        /// 3des加密函数(ECB加密模式,PaddingMode.PKCS7,无IV)
        /// </summary>
        /// <param name="encryptValue">加密字符</param>
        /// <param name="key">加密key(24字符)</param>
        /// <returns>加密后Base64字符</returns>
        public static string EncryptString(string encryptValue, string key)
        {
            string enstring = "加密出错!";
            ICryptoTransform ct; //需要此接口才能在任何服务提供程序上调用 CreateEncryptor 方法，服务提供程序将返回定义该接口的实际 encryptor 对象。
            MemoryStream ms;
            CryptoStream cs;
            byte[] byt;
            SymmetricAlgorithm des3 = SymmetricAlgorithm.Create("TripleDES");
            des3.Mode = CipherMode.ECB;
            des3.Key = Encoding.UTF8.GetBytes(splitStringLen(key, 24, '0'));
            //des3.KeySize = 192;
            des3.Padding = PaddingMode.PKCS7;

            ct = des3.CreateEncryptor();

            byt = Encoding.UTF8.GetBytes(encryptValue);//将原始字符串转换成字节数组。大多数 .NET 加密算法处理的是字节数组而不是字符串。

            //创建 CryptoStream 对象 cs 后，现在使用 CryptoStream 对象的 Write 方法将数据写入到内存数据流。这就是进行实际加密的方法，加密每个数据块时，数据将被写入 MemoryStream 对象。

            ms = new MemoryStream();
            cs = new CryptoStream(ms, ct, CryptoStreamMode.Write);
            try
            {
                cs.Write(byt, 0, byt.Length);
                cs.FlushFinalBlock();
                enstring = Convert.ToBase64String(ms.ToArray());
            }
            catch (Exception ex)
            {
                enstring = ex.ToString();
            }
            finally
            {
                cs.Close();
                cs.Dispose();
                ms.Close();
                ms.Dispose();
                des3.Clear();
                ct.Dispose();
            }
            enstring = Convert.ToBase64String(ms.ToArray());
            return enstring;
        }
        #endregion

        #region "3des解密字符串"
        /// <summary>
        /// 3des解密函数(ECB加密模式,PaddingMode.PKCS7,无IV)
        /// </summary>
        /// <param name="decryptString">解密字符</param>
        /// <param name="key">解密key(24字符)</param>
        /// <returns>解密后字符</returns>
        public static string DecryptString(string decryptString, string key)
        {
            if (string.IsNullOrEmpty(decryptString))
            {
                return "";
            }
            string destring = "解密字符失败!";
            ICryptoTransform ct;
            MemoryStream ms;
            CryptoStream cs;
            byte[] byt;

            SymmetricAlgorithm des3 = SymmetricAlgorithm.Create("TripleDES");
            des3.Mode = CipherMode.ECB;
            des3.Key = Encoding.UTF8.GetBytes(splitStringLen(key, 24, '0'));
            //des3.KeySize = 192;
            des3.Padding = PaddingMode.PKCS7;

            ct = des3.CreateDecryptor();

            byt = Convert.FromBase64String(decryptString);

            ms = new MemoryStream();
            cs = new CryptoStream(ms, ct, CryptoStreamMode.Write);
            try
            {
                cs.Write(byt, 0, byt.Length);
                cs.FlushFinalBlock();
                destring = Encoding.UTF8.GetString(ms.ToArray());
            }
            catch (Exception ex)
            {
                destring = ex.ToString();
            }
            finally
            {
                ms.Close();
                cs.Close();
                ms.Dispose();
                cs.Dispose();
                ct.Dispose();
                des3.Clear();
            }
            return destring;
        }
        #endregion

        #region 字符串截取补字符函数
        /// <summary>
        /// 字符串截取补字符函数
        /// </summary>
        /// <param name="s">要处理的字符串</param>
        /// <param name="len">长度</param>
        /// <param name="b">补充的字符</param>
        /// <returns>处理后字符</returns>
        public static string splitStringLen(string s, int len, char b)
        {
            if (string.IsNullOrEmpty(s))
                return "";
            if (s.Length >= len)
                return s.Substring(0, len);
            return s.PadRight(len, b);
        }


        #endregion
    }
}

