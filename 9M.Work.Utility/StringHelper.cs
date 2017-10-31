using System;
using System.Collections.Generic;

using System.Web;
using System.Text;
using System.Text.RegularExpressions;

namespace _9M.Work.Utility
{
    /// <summary>
    /// 字符串操作类
    /// </summary>
    public static class StringHelper
    {
        /// <summary>
        /// 将字符串添加\'，已使用带入SQL
        /// </summary>
        /// <param name="str">以，分割的字符串</param>
        /// <returns></returns>
        public static string ConvertString(string str)
        {
            string ReturnStr = "";
            String[] strs = str.Split(',');
            for (int i = 0; i < strs.Length; i++)
            {
                ReturnStr += "\'" + strs[i] + "\',";
            }
            ReturnStr = RemoveLast(ReturnStr);
            return ReturnStr;
        }
        /// <summary>
        /// 将多个相同字符串拼接为一个字符串
        /// </summary>
        /// <param name="strLong">拼接次数</param>
        /// <param name="str">拼接内容</param>
        public static string AppendString(int strLong, string str)
        {
            string ReturnStr = "";
            for (int i = 0; i < strLong; i++)
            {
                ReturnStr += str;
            }
            return ReturnStr;
        }

        /// <summary>
        /// 将字符串超出长度的部分使用省略号代替
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="length">指定显示的长度</param>
        public static string OmitString(string str, int length)
        {
            if (str.Length > length + 1)
            {
                str = str.Substring(0, length) + "…";
            }
            return str;
        }


        /// <summary>
        /// 将字符串超出长度的部分去掉
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="length">指定显示的长度</param>
        /// <returns></returns>
        public static string SubString(string str, int length)
        {
            if (str.Length > length + 1)
            {
                str = str.Substring(0, length);
            }
            return str;
        }

        /// <summary>
        /// 删除最后一个字符
        /// 如"1,2,3,4,"执行后"1,2,3,4"
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string RemoveLast(string str)
        {
            return (str == "") ? "" : str.Substring(0, str.Length - 1);
        }
        /// <summary>
        /// 删除后面字符
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="length">删除长度</param>
        /// <returns></returns>
        public static string RemoveLast(string str, int length)
        {
            return (str == "") ? "" : str.Substring(0, str.Length - length);
        }

        /// <summary>
        /// 删除最后一个字符
        /// 如"1,2,3,4,"执行后"1,2,3,4"
        /// </summary>
        /// <param name="sb"></param>
        /// <returns></returns>
        public static string RemoveLast(StringBuilder sb)
        {
            return (sb.ToString() == "") ? "" : sb.ToString().Substring(0, sb.ToString().Length - 1);
        }

        /// <summary>
        /// 分割字符串
        /// </summary>
        /// <param name="strContent">需要分割的字符串</param>
        /// <param name="strSplit">用以分割的字符</param>
        /// <returns></returns>
        public static string[] SplitString(string strContent, string strSplit)
        {
            if (!string.IsNullOrEmpty(strContent))
            {
                if (strContent.IndexOf(strSplit) < 0)
                    return new string[] { strContent };
                else
                    return Regex.Split(strContent, Regex.Escape(strSplit), RegexOptions.IgnoreCase);
            }
            else
                return new string[0] { };
        }

        /// <summary>
        /// 分割字符串,后对得到的string[]做截取操作
        /// </summary>
        /// <param name="strContent">需要分割的字符串</param>
        /// <param name="strSplit">用以分割的字符</param>
        /// <param name="count">截取的长度</param>
        /// <returns></returns>
        public static string[] SplitString(string strContent, string strSplit, int count)
        {
            string[] result = new string[count];
            string[] splited = SplitString(strContent, strSplit);

            for (int i = 0; i < count; i++)
            {
                if (i < splited.Length)
                    result[i] = splited[i];
                else
                    result[i] = string.Empty;
            }

            return result;
        }



        #region 取汉字的首字母
        /// <summary> 
        /// 在指定的字符串列表CnStr中检索符合拼音索引字符串 
        /// </summary> 
        /// <param name="CnStr">汉字字符串</param> 
        /// <returns>相对应的汉语拼音首字母串</returns> 
        public static string GetSpellCode(string CnStr)
        {
            string strTemp = "";
            int iLen = CnStr.Length;
            int i = 0;

            for (i = 0; i <= iLen - 1; i++)
            {
                strTemp += GetCharSpellCode(CnStr.Substring(i, 1));
            }

            return strTemp;
        }


        /// <summary> 
        /// 得到一个汉字的拼音第一个字母，如果是一个英文字母则直接返回大写字母 
        /// </summary> 
        /// <param name="CnChar">单个汉字</param> 
        /// <returns>单个大写字母</returns> 
        private static string GetCharSpellCode(string CnChar)
        {
            long iCnChar;

            byte[] ZW = System.Text.Encoding.Default.GetBytes(CnChar);

            //如果是字母，则直接返回 
            if (ZW.Length == 1)
            {
                return CnChar.ToUpper();
            }
            else
            {
                // get the array of byte from the single char 
                int i1 = (short)(ZW[0]);
                int i2 = (short)(ZW[1]);
                iCnChar = i1 * 256 + i2;
            }

            //expresstion 
            //table of the constant list 
            // 'A'; //45217..45252 
            // 'B'; //45253..45760 
            // 'C'; //45761..46317 
            // 'D'; //46318..46825 
            // 'E'; //46826..47009 
            // 'F'; //47010..47296 
            // 'G'; //47297..47613 

            // 'H'; //47614..48118 
            // 'J'; //48119..49061 
            // 'K'; //49062..49323 
            // 'L'; //49324..49895 
            // 'M'; //49896..50370 
            // 'N'; //50371..50613 
            // 'O'; //50614..50621 
            // 'P'; //50622..50905 
            // 'Q'; //50906..51386 

            // 'R'; //51387..51445 
            // 'S'; //51446..52217 
            // 'T'; //52218..52697 
            //没有U,V 
            // 'W'; //52698..52979 
            // 'X'; //52980..53640 
            // 'Y'; //53689..54480 
            // 'Z'; //54481..55289 

            // iCnChar match the constant 
            if ((iCnChar >= 45217) && (iCnChar <= 45252))
            {
                return "A";
            }
            else if ((iCnChar >= 45253) && (iCnChar <= 45760))
            {
                return "B";
            }
            else if ((iCnChar >= 45761) && (iCnChar <= 46317))
            {
                return "C";
            }
            else if ((iCnChar >= 46318) && (iCnChar <= 46825))
            {
                return "D";
            }
            else if ((iCnChar >= 46826) && (iCnChar <= 47009))
            {
                return "E";
            }
            else if ((iCnChar >= 47010) && (iCnChar <= 47296))
            {
                return "F";
            }
            else if ((iCnChar >= 47297) && (iCnChar <= 47613))
            {
                return "G";
            }
            else if ((iCnChar >= 47614) && (iCnChar <= 48118))
            {
                return "H";
            }
            else if ((iCnChar >= 48119) && (iCnChar <= 49061))
            {
                return "J";
            }
            else if ((iCnChar >= 49062) && (iCnChar <= 49323))
            {
                return "K";
            }
            else if ((iCnChar >= 49324) && (iCnChar <= 49895))
            {
                return "L";
            }
            else if ((iCnChar >= 49896) && (iCnChar <= 50370))
            {
                return "M";
            }

            else if ((iCnChar >= 50371) && (iCnChar <= 50613))
            {
                return "N";
            }
            else if ((iCnChar >= 50614) && (iCnChar <= 50621))
            {
                return "O";
            }
            else if ((iCnChar >= 50622) && (iCnChar <= 50905))
            {
                return "P";
            }
            else if ((iCnChar >= 50906) && (iCnChar <= 51386))
            {
                return "Q";
            }
            else if ((iCnChar >= 51387) && (iCnChar <= 51445))
            {
                return "R";
            }
            else if ((iCnChar >= 51446) && (iCnChar <= 52217))
            {
                return "S";
            }
            else if ((iCnChar >= 52218) && (iCnChar <= 52697))
            {
                return "T";
            }
            else if ((iCnChar >= 52698) && (iCnChar <= 52979))
            {
                return "W";
            }
            else if ((iCnChar >= 52980) && (iCnChar <= 53640))
            {
                return "X";
            }
            else if ((iCnChar >= 53689) && (iCnChar <= 54480))
            {
                return "Y";
            }
            else if ((iCnChar >= 54481) && (iCnChar <= 55289))
            {
                return "Z";
            }
            else return ("?");
        }

        #endregion



        #region 根据表达式来截取字段

        /// <summary>
        /// 截取指定字符之外的字符串
        /// </summary>
        /// <param name="pageStr">源字符串</param>
        /// <param name="strStart">要截字符串开头</param>
        /// <param name="strEnd">要截字符串结尾下一句</param>
        /// <returns></returns>
        public static string GetInnerBody(string pageStr, int strStart, int strEnd)
        {
            if (strStart != -1 && strEnd != -1)
            {
                pageStr = pageStr.Substring(0, strStart) + pageStr.Substring(strEnd);
            }


            return pageStr;
        }

        /// <summary>
        /// 根据表达式来截取字段(默认不包含截取规则字段)
        /// </summary>
        /// <param name="pageStr">原字符串</param>
        /// <param name="strStart">截取字符开始</param>
        /// <param name="strEnd">截取字符结束</param>
        /// <returns></returns>
        public static string GetBody(string pageStr, string strStart, string strEnd)
        {
            return StringHelper.GetBody(pageStr, strStart, strEnd, false, false);
        }



        /// <summary>
        /// 根据表达式来截取字段
        /// </summary>
        /// <param name="pageStr">原字符串</param>
        /// <param name="strStart">截取字符开始</param>
        /// <param name="strEnd">截取字符结束</param>
        /// <param name="inStart">是否包含strStart,false是不包含</param>
        /// <param name="inEnd">是否包含strEnd,false是不包含</param>
        /// <returns></returns>
        public static string GetBody(string pageStr, string strStart, string strEnd, bool inStart, bool inEnd)
        {
            pageStr = pageStr.Trim();
            int start = pageStr.IndexOf(strStart);
            if (strStart.Length == 0 || start < 0)
                return "$FalseStart";
            pageStr = pageStr.Substring(start + strStart.Length, pageStr.Length - start - strStart.Length);
            int end = pageStr.IndexOf(strEnd);
            if (strEnd.Length == 0 || end < 0)
                return "$FalseEnd";
            string strResult = pageStr.Substring(0, end);
            if (inStart)
                strResult = strStart + strResult;
            if (inEnd)
                strResult += strEnd;
            return strResult.Trim();
        }

        /// <summary>
        /// 根据表达式来截取字段
        /// </summary>
        /// <param name="pageStr">原字符串</param>
        /// <param name="strStart">截取字符开始</param>
        /// <param name="strEnd">截取字符结束</param>
        /// <param name="inStart">是否包含strStart,false是不包含</param>
        /// <param name="inEnd">是否包含strEnd,false是不包含</param>
        /// <returns></returns>
        public static string GetBody(string pageStr, string strStart, bool inStart)
        {
            pageStr = pageStr.Trim();
            int start = pageStr.IndexOf(strStart);
            if (strStart.Length == 0 || start < 0)
                return "$FalseStart";
            pageStr = pageStr.Substring(start + strStart.Length, pageStr.Length - start - strStart.Length);
            if (inStart)
                pageStr = strStart + pageStr;
            return pageStr.Trim();
        }
        #endregion



        #region "按字符截取字符串成数组"
        /// <summary>
        /// 按分隔符截取字符串
        /// </summary>
        /// <param name="s">待分隔的字符串</param>
        /// <param name="_Split">分隔符</param>
        /// <returns></returns>
        public static List<string> Split(string s, char _Split)
        {
            string[] ss = s.Split(_Split);

            List<string> lst = new List<string>();

            foreach (string str in ss)
            {
                if (str != string.Empty && str != "")
                    lst.Add(str);
            }

            return lst;
        }

        #endregion

        /// <summary>
        /// 判断是否为数字
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNumber(string value)
        {
            return Regex.IsMatch(value, @"^[0-9]*[1-9][0-9]*$");
        }

        /// <summary>
        /// 判断是否为正整数
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsPositiveInteger(string value)
        {
            return Regex.IsMatch(value, @"^[1-9][0-9]*$");
        }

        /// <summary>
        /// 判断是否为数字
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNum(string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;
            return Regex.IsMatch(value, @"^[0-9]*$");
        }

        /// <summary>
        /// 判断数字和字母
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNumAndZimo(string value)
        {
            return Regex.IsMatch(value, @"^[A-Za-z0-9]+$");
        }

        /// <summary>
        /// 判断是否为数字
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsInteger(string value)
        {
            return Regex.IsMatch(value, @"^-?[0-9]*$");
        }
        /// <summary>
        /// 判断是否为金额
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsPrice(string value)
        {
            return Regex.IsMatch(value, @"^(([1-9]\d*)|0)(\.\d{1,2})?$");
        }

        /// <summary>
        /// 判断是否为金额
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsPriceF(string value)
        {
            return Regex.IsMatch(value, @"^(([1-9]\d*)|0)(\.\d{1,4})?$");
        }

        /// <summary>
        /// 判断是否为字母和数字
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsText(string value)
        {
            return Regex.IsMatch(value, @"^[A-Za-z0-9]+$");
        }

        /// <summary>
        /// 将文本中含用链接的部分，转换成HTML链接格式
        /// </summary>
        /// <param name="HtmlCode"></param>
        /// <returns></returns>
        public static String RegValidateHref(String TempHtmlCode)
        {
            String htmlCode = TempHtmlCode;

            string strRegex = @"((http|ftp|https):\/\/)?[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?";

            Regex r = new Regex(strRegex, RegexOptions.IgnoreCase);
            MatchCollection mc = r.Matches(htmlCode);

            foreach (Match m in mc)
            {
                if (m.Value.IndexOf("http") > -1 || m.Value.IndexOf("https") > -1 || m.Value.IndexOf("ftp") > -1)
                {
                    String str = "<a href=\"{0}\" target=\"_blank\">{0}</a>";
                    htmlCode = htmlCode.Replace(m.Value, String.Format(str, m.Value));
                }
            }

            return htmlCode;


        }

        #region 截取字符串(区分中英文)
        /// <summary>
        /// 截取字符串
        /// </summary>
        /// <param name="str_value"></param>
        /// <param name="str_len"></param>
        /// <returns></returns>
        public static string leftx(string str_value, int str_len)
        {
            int p_num = 0;
            int i;
            string New_Str_value = "";

            if (str_value == "")
            {
                New_Str_value = "";
            }
            else
            {
                int Len_Num = str_value.Length;
                for (i = 0; i <= Len_Num - 1; i++)
                {
                    if (i > Len_Num) break;
                    char c = Convert.ToChar(str_value.Substring(i, 1));
                    if (((int)c > 255) || ((int)c < 0))
                        p_num = p_num + 2;
                    else
                        p_num = p_num + 1;



                    if (p_num >= str_len)
                    {

                        New_Str_value = str_value.Substring(0, i + 1);
                        break;
                    }
                    else
                    {
                        New_Str_value = str_value;
                    }

                }

            }
            return New_Str_value;
        }

        /// <summary>
        /// 截取字符串
        /// </summary>
        /// <param name="str_value"></param>
        /// <param name="str_len"></param>
        /// <param name="Suffix"></param>
        /// <returns></returns>
        public static string leftx(string str_value, int str_len, string Suffix)
        {
            string tx = leftx(str_value, str_len);
            //当字符串进行截取后,可匹配增加后缀的缩略符号
            if (tx.Length < str_value.Length)
            {
                tx = tx + Suffix;
            }

            return tx;
        }

        #endregion


        #region 删除多项时字符串处理
        /// <summary>
        /// 删除多项时字符串处理(1001,1002改成'1001','1002')
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ConvertListString(string str)
        {
            return (str == "") ? "" : string.Format("'{0}'", str.Replace(",", "','"));
        }

        /// <summary>
        /// 多项时字符串处理(List改成'1001','1002')
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static String ConvertListString(List<String> list)
        {
            String tempStr = String.Empty;
            foreach (String str in list)
            {
                tempStr += String.Format("'{0}',", str);
            }
            if (tempStr.Length > 0)
            {
                tempStr = tempStr.TrimEnd(',');
            }
            return tempStr;
        }

        #endregion


        #region "获取字符串内的数字"
        /// <summary>
        /// 获取字符串内的数字
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int GetNums(string str)
        {
            var reg = new Regex(@"[0-9]+");
            var ms = reg.Matches(str);
            string[] nums = new string[ms.Count];
            for (int i = 0, len = nums.Length; i < len; i++)
            {
                nums[i] = ms[i].Value;
            }
            return Convert.ToInt32(string.Join("\n", nums));
        }

        #endregion

        #region "按字符串位数补0"
        /// <summary>
        /// 按字符串位数补0
        /// </summary>
        /// <param name="CharTxt">字符串</param>
        /// <param name="CharLen">字符长度</param>
        /// <returns></returns>
        public static string FillZero(string CharTxt, int CharLen)
        {
            if (CharTxt.Length < CharLen)
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < CharLen - CharTxt.Length; i++)
                {
                    sb.Append("0");
                }
                sb.Append(CharTxt);
                return sb.ToString();
            }
            else
            {
                return CharTxt;
            }
        }

        #endregion
    }
}
