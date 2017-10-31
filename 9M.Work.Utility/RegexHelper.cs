using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace _9M.Work.Utility
{
    public class RegexHelper
    {
        /// <summary>
        /// 指定的正则分分割字符串
        /// </summary>
        /// <param name="target"></param>
        /// <param name="regex"></param>
        /// <param name="isIncludeMatch"></param>
        /// <returns></returns>
        public static string[] Split(string target, Regex regex, bool isIncludeMatch = true)
        {
            List<string> list = new List<string>();
            MatchCollection mc = regex.Matches(target);
            int curPostion = 0;
            foreach (Match match in mc)
            {
                if (match.Index != curPostion)
                {
                    list.Add(target.Substring(curPostion, match.Index - curPostion));
                }
                curPostion = match.Index + match.Length;
                if (isIncludeMatch)
                {
                    list.Add(match.Value);
                }
            }
            if (target.Length > curPostion)
            {
                list.Add(target.Substring(curPostion));
            }
            return list.ToArray();
        }

        //正则匹配整个IMG标签
        public static List<string> GetImgAll(string sHtmlText)
        {
            List<string> list = new List<string>();
            // 定义正则表达式用来匹配 img 标签 
            Regex regImg = new Regex(@"<img\b[^<>]*?\bsrc[\s\t\r\n]*=[\s\t\r\n]*[""']?[\s\t\r\n]*(?<imgUrl>[^\s\t\r\n""'<>]*)[^<>]*?/?[\s\t\r\n]*>", RegexOptions.IgnoreCase);

            // 搜索匹配的字符串 
            MatchCollection matches = regImg.Matches(sHtmlText);
            // 取得匹配项列表 
            foreach (Match match in matches)
            {
                list.Add(match.Groups["imgUrl"].Value);
            }
            return list;
        }
    }
}
