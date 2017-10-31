using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace _9M.Work.Utility
{
    public class GoodsHelper
    {
        #region 返回实体店或是线上的款号
        /// <summary>
        /// 判断是否标准款号，并返回标准款号和实体店款号
        /// </summary>
        /// <param name="kuan">要判断的款号</param>
        /// <param name="standardKuan">返回标准款号</param>
        /// <param name="entityShopKuan">返回实体店款号</param>
        /// <returns>返回标准款号(StandardKuan)和实体店款号(EntityShopKuan)</returns>
        public static bool CheckStandardKuan(string kuan, out string standardKuan, out string entityShopKuan)
        {
            bool isStandard = false;
            standardKuan = string.Empty;
            entityShopKuan = string.Empty;
            if (!string.IsNullOrEmpty(kuan))
            {
                try
                {
                    //标准
                    string regStandard = @"^(\d{2})([a-zA-Z]{2,3})(\d+)";
                    Regex regS = new Regex(regStandard);

                    //实体店
                    string regEntityShop = @"^([a-zA-Z]{2,3})(\d+)";
                    Regex regE = new Regex(regEntityShop);

                    if (regS.IsMatch(kuan))//标准
                    {
                        isStandard = true;
                        MatchCollection mc = regS.Matches(kuan);
                        foreach (Match match in mc)
                        {
                            if (match.Groups.Count == 4)
                            {
                                entityShopKuan = string.Format("{0}{1}{2}", match.Groups[2].Value,
                                    Convert.ToInt32(match.Groups[1].Value) + 4, match.Groups[3].Value);
                            }
                        }
                        standardKuan = kuan;
                    }
                    else if (regE.IsMatch(kuan))  //实体
                    {
                        isStandard = false;
                        foreach (Match match in regE.Matches(kuan))
                        {
                            if (match.Groups.Count == 3)
                            {
                                string headNumber = string.Empty;
                                string footerNumber = string.Empty;
                                string strNumbers = match.Groups[2].Value;
                                if (strNumbers.Length >= 6)
                                {
                                    headNumber = strNumbers.Substring(0, 2);
                                    footerNumber = strNumbers.Substring(2);
                                }
                                standardKuan = string.Format("{0}{1}{2}", Convert.ToInt32(headNumber) - 4, match.Groups[1].Value, footerNumber);
                            }
                        }
                        entityShopKuan = kuan;
                    }
                    else
                    {
                        entityShopKuan = kuan;
                        standardKuan = kuan;
                    }
                }
                catch (Exception e)
                {
                    entityShopKuan = kuan;
                    standardKuan = kuan;
                    Console.WriteLine(e.Message);
                }
            }
            return isStandard;
        }
        #endregion

        public static string TrueGoodsNo(string GoodsNo,bool IsEntity)
        {
            string standardKuan, entityShopKuan;
            CheckStandardKuan(GoodsNo,out standardKuan,out entityShopKuan);
            string res = IsEntity ? entityShopKuan : standardKuan;
            return res;
        }

        #region 判断一个输入是款号还是带附加码的全编码TRUE是款号FALSE为全编码
        /// <summary>
        /// 判断一个输入是款号还是带附加码的全编码TRUE是款号FALSE为全编码
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool IsGoodId(string text)
        {
            bool b = false;
            if (text.Length >= 8)
            {
                string Brand = Regex.Replace(text, "[0-9]", "", RegexOptions.IgnoreCase);
                int length = Brand.Length == 3 ? 9 : 8;
                b = text.Length <= length;
            }
            return b;
        }
        #endregion

        #region 将一个完整的款号拆分成款号+规格码
        public static void SplitGoodsDetail(string GoodsNoDetail, out string GoodsNo, out string SpecCode)
        {
            GoodsNo = string.Empty;
            SpecCode = string.Empty;
            if (GoodsNoDetail.Length > 8)
            {
                string Brand = Regex.Replace(GoodsNoDetail, "[0-9]", "", RegexOptions.IgnoreCase);
                GoodsNo = Brand.Length == 2 ? GoodsNoDetail.Substring(0, 8) : GoodsNoDetail.Substring(0, 9);
                SpecCode = Brand.Length == 2 ? GoodsNoDetail.Substring(8, GoodsNoDetail.Length - 8) : GoodsNoDetail.Substring(9, GoodsNoDetail.Length - 9);
            }
            else
            {
                GoodsNo = GoodsNoDetail;
            }
        }
        #endregion

        #region 提取款号和附加码
        public static string GoodsNoOrSpecCode(string Txt ,bool IsGetGoodsNo)
        {
            string GoodsNo = string.Empty;
            string SpecCode = string.Empty;
            SplitGoodsDetail(Txt,out GoodsNo,out SpecCode);
            if (IsGetGoodsNo)
            {
                return GoodsNo;
            }
            else
            {
                return SpecCode;
            }
        }
        #endregion

        #region 返回一个款号的英文品牌
        public static string BrandEn(string OuterId)
        {
            return Regex.Replace(OuterId, "[0-9]", "", RegexOptions.IgnoreCase).ToUpper();
        }
        #endregion

        #region 将一个货位备注的字符串以◆分开为主货位和暂存位
        public static void SplitPostion(string Postion, out string P_Postion, out string F_Postion)
        {
            P_Postion = string.Empty;
            F_Postion = string.Empty;
            string[] arry = Postion.Split('◆');
            if (arry.Length > 1)
            {
                P_Postion = arry[0];
                F_Postion = arry[1];
            }
            else
            {
                P_Postion = Postion;
            }
        }
        #endregion

        #region 将主货位和暂存位合并成字符
        public static string AssemblePostion(string P_Postion, string F_Postion)
        {
            string newpostion = string.Empty;
            if (string.IsNullOrEmpty(P_Postion) && string.IsNullOrEmpty(F_Postion))
            {
                newpostion = "";
            }
            else if (!string.IsNullOrEmpty(P_Postion) && string.IsNullOrEmpty(F_Postion))
            {
                newpostion = P_Postion;
            }
            else if (!string.IsNullOrEmpty(P_Postion) && !string.IsNullOrEmpty(F_Postion))
            {
                newpostion = P_Postion + "◆" + F_Postion;
            }
            else
            {
                newpostion = "◆" + F_Postion;
            }
            return newpostion;
        }
        #endregion

        #region 当只是改变主货位的时候返回整个货位备注
        public static string PrimaryPostionOnly(string Postion, string P_Postion)
        {
            string[] Arry = Postion.Split('◆');
            string NewPostion = Arry.Length > 1 ? P_Postion + "◆" + Arry[1] : P_Postion;
            return NewPostion;
        }
        #endregion

        #region 判断款号是否为一个杂款
        public static bool CheckZhaKuan(string GoodsNo)
        {
            bool b = true;
            string Brand = Regex.Replace(GoodsNo, "[0-9]", "", RegexOptions.IgnoreCase);
            string flagtext = Brand.Length == 3 ? GoodsNo.Substring(7, 2) : GoodsNo.Substring(6, 2);
            if (!flagtext.Equals("00"))
            {
                b = false;
            }
            return b;
        }
        #endregion

        #region 得到一个款号的季节
        public static string GetSeaSon(string GoodsNo)
        {
            string SeaSon = string.Empty;
            if (GoodsNo.Length>2)
            {
               string Flag=GoodsNo.Substring(1,1);
               if (Flag.Equals("1") || Flag.Equals("5"))
               {
                   SeaSon = "春";
               }
               if (Flag.Equals("2") || Flag.Equals("6"))
               {
                   SeaSon = "夏";
               }
               if (Flag.Equals("3") || Flag.Equals("7"))
               {
                   SeaSon = "秋";
               }
               if (Flag.Equals("4") || Flag.Equals("8"))
               {
                   SeaSon = "冬";
               }
            }
            return SeaSon;
        }
        #endregion

        #region 根据带-的字符串得到一口价
        public static double GetMaxPrice(string Text)
        {
            double price = 0.00;
            string Res = string.Empty;
            string[] arry = Text.Split('-');
            if (arry.Length > 1)
            {
                double first = 0.00;
                double.TryParse(arry[0],out first);
                double second = 0.00;
                double.TryParse(arry[1], out second);
                Res = first > second ? first.ToString() : second.ToString();
            }
            else
            {
                Res = arry[0];
            }
            double.TryParse(Res,out price);
            return price;
        }
        #endregion

        /// <summary>
        /// 以换行的方式得到一组款号的集合
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static List<string> GoodsListByLine(string Text)
        {
            List<string> GoodsList = Text.Split('\n').Where(x => !string.IsNullOrEmpty(x)).ToList();
            GoodsList = GoodsList.Select(x => x.Replace("\r", "").Trim()).Distinct().ToList();
            return GoodsList;
        }

        /// <summary>
        /// 得到一组字符的字节数
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int GetLength(string str)
        {
            if (str == null || str.Length == 0) { return 0; }

            int l = str.Length;
            int realLen = l;

            #region 计算长度
            int clen = 0;//当前长度
            while (clen < l)
            {
                //每遇到一个中文，则将实际长度加一。
                if ((int)str[clen] > 128) { realLen++; }
                clen++;
            }
            #endregion

            return realLen;
        }

        /// <summary>
        /// 自动识别标题没有（【】[]）就添加，有就替换
        /// </summary>
        /// <param name="OldTitle"></param>
        /// <param name="AppendTitle"></param>
        /// <returns></returns>
        public static string RetrunActityTitle(string OldTitle, string AppendTitle)
        {
            string NewTitle = string.Empty;
            if (OldTitle.Contains("["))
            {
                int start = OldTitle.IndexOf("[");
                int end = OldTitle.IndexOf("]");
                NewTitle = AppendTitle + OldTitle.Remove(start, end - start + 1);
            }
            else if (OldTitle.Contains("【"))
            {
                int start = OldTitle.IndexOf("【");
                int end = OldTitle.IndexOf("】");
                NewTitle = AppendTitle + OldTitle.Remove(start, end - start + 1);
            }

            else
            {
                NewTitle = AppendTitle + OldTitle;
            }
            int len = GoodsHelper.GetLength(NewTitle);
            if (len > 60)
            {
                string real = string.Empty;
                int flag = 0;
                for (int i = 0; i < NewTitle.Length; i++)
                {
                    if (flag == 60)
                    {
                        break;
                    }
                    real += NewTitle[i];
                    if ((int)NewTitle[i] > 128)
                    {
                        flag += 2;
                    }
                    else
                    {
                        flag++;
                    }
                }
                NewTitle = real;
            }
            return NewTitle;
        }
    }
}
