using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace _9M.Work.Utility
{
    public class Code128Auto
    {
        #region 自定义变量
        /// <summary>
        /// 条码高度
        /// </summary>
        private float heightf;
        public float Heightf
        {
            get { return heightf; }
            set { heightf = value; }
        }

        /// <summary>
        /// 条码单条线的宽度
        /// </summary>
        private float widthf;
        public float Widthf
        {
            get { return widthf; }
            set { widthf = value; }
        }

        /// <summary>
        /// 条码内容
        /// </summary>
        private string strText;
        public string StrText
        {
            get { return strText; }
            set { strText = value; }
        }

        private float xf;
        public float Xf
        {
            get { return xf; }
            set { xf = value; }
        }

        private float yf;
        public float Yf
        {
            get { return yf; }
            set { yf = value; }
        }
        private List<int> ib = new List<int>();
        string strBarcode;
        DataTable dt;
        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        public Code128Auto()
        {
            dt = new DataTable();
            dt.Columns.Add("ID", typeof(string));
            dt.Columns.Add("CODE128A", typeof(string));
            dt.Columns.Add("CODE128B", typeof(string));
            dt.Columns.Add("CODE128C", typeof(string));
            dt.Columns.Add("BANDCODE", typeof(string));
            dt.Rows.Add("0", "SP", "SP", "00", "212222");
            dt.Rows.Add("1", "!", "!", "01", "222122");
            dt.Rows.Add("2", "\"", "\"", "02", "222221");
            dt.Rows.Add("3", "#", "#", "03", "121223");
            dt.Rows.Add("4", "$", "$", "04", "121322");
            dt.Rows.Add("5", "%", "%", "05", "131222");
            dt.Rows.Add("6", "&", "&", "06", "122213");
            dt.Rows.Add("7", "'", "'", "07", "122312");
            dt.Rows.Add("8", "(", "(", "08", "132212");
            dt.Rows.Add("9", ")", ")", "09", "221213");
            dt.Rows.Add("10", "*", "*", "10", "221312");
            dt.Rows.Add("11", "+", "+", "11", "231212");
            dt.Rows.Add("12", ",", ",", "12", "112232");
            dt.Rows.Add("13", "-", "-", "13", "122132");
            dt.Rows.Add("14", ".", ".", "14", "122231");
            dt.Rows.Add("15", "/", "/", "15", "113222");
            dt.Rows.Add("16", "0", "0", "16", "123122");
            dt.Rows.Add("17", "1", "1", "17", "123221");
            dt.Rows.Add("18", "2", "2", "18", "223211");
            dt.Rows.Add("19", "3", "3", "19", "221132");
            dt.Rows.Add("20", "4", "4", "20", "221231");
            dt.Rows.Add("21", "5", "5", "21", "213212");
            dt.Rows.Add("22", "6", "6", "22", "223112");
            dt.Rows.Add("23", "7", "7", "23", "312131");
            dt.Rows.Add("24", "8", "8", "24", "311222");
            dt.Rows.Add("25", "9", "9", "25", "321122");
            dt.Rows.Add("26", ":", ":", "26", "321221");
            dt.Rows.Add("27", ";", ";", "27", "312212");
            dt.Rows.Add("28", "<", "<", "28", "322112");
            dt.Rows.Add("29", "=", "=", "29", "322211");
            dt.Rows.Add("30", ">", ">", "30", "212123");
            dt.Rows.Add("31", "?", "?", "31", "212321");
            dt.Rows.Add("32", "@", "@", "32", "232121");
            dt.Rows.Add("33", "A", "A", "33", "111323");
            dt.Rows.Add("34", "B", "B", "34", "131123");
            dt.Rows.Add("35", "C", "C", "35", "131321");
            dt.Rows.Add("36", "D", "D", "36", "112313");
            dt.Rows.Add("37", "E", "E", "37", "132113");
            dt.Rows.Add("38", "F", "F", "38", "132311");
            dt.Rows.Add("39", "G", "G", "39", "211313");
            dt.Rows.Add("40", "H", "H", "40", "231113");
            dt.Rows.Add("41", "I", "I", "41", "231311");
            dt.Rows.Add("42", "J", "J", "42", "112133");
            dt.Rows.Add("43", "K", "K", "43", "112331");
            dt.Rows.Add("44", "L", "L", "44", "132131");
            dt.Rows.Add("45", "M", "M", "45", "113123");
            dt.Rows.Add("46", "N", "N", "46", "113321");
            dt.Rows.Add("47", "O", "O", "47", "133121");
            dt.Rows.Add("48", "P", "P", "48", "313121");
            dt.Rows.Add("49", "Q", "Q", "49", "211331");
            dt.Rows.Add("50", "R", "R", "50", "231131");
            dt.Rows.Add("51", "S", "S", "51", "213113");
            dt.Rows.Add("52", "T", "T", "52", "213311");
            dt.Rows.Add("53", "U", "U", "53", "213131");
            dt.Rows.Add("54", "V", "V", "54", "311123");
            dt.Rows.Add("55", "W", "W", "55", "311321");
            dt.Rows.Add("56", "X", "X", "56", "331121");
            dt.Rows.Add("57", "Y", "Y", "57", "312113");
            dt.Rows.Add("58", "Z", "Z", "58", "312311");
            dt.Rows.Add("59", "[", "[", "59", "332111");
            dt.Rows.Add("60", @"\", @"\", "60", "314111");
            dt.Rows.Add("61", "]", "]", "61", "221411");
            dt.Rows.Add("62", "^", "^", "62", "431111");
            dt.Rows.Add("63", "_", "_", "63", "111224");
            dt.Rows.Add("64", "NUL", "`", "64", "111422");
            dt.Rows.Add("65", "SOH", "a", "65", "121124");
            dt.Rows.Add("66", "STX", "b", "66", "121421");
            dt.Rows.Add("67", "ETX", "c", "67", "141122");
            dt.Rows.Add("68", "EOT", "d", "68", "141221");
            dt.Rows.Add("69", "ENQ", "e", "69", "112214");
            dt.Rows.Add("70", "ACK", "f", "70", "112412");
            dt.Rows.Add("71", "BEL", "g", "71", "122114");
            dt.Rows.Add("72", "BS", "h", "72", "122411");
            dt.Rows.Add("73", "HT", "i", "73", "142112");
            dt.Rows.Add("74", "LF", "j", "74", "142211");
            dt.Rows.Add("75", "VT", "k", "75", "241211");
            dt.Rows.Add("76", "FF", "l", "76", "221114");
            dt.Rows.Add("77", "CR", "m", "77", "413111");
            dt.Rows.Add("78", "SO", "n", "78", "241112");
            dt.Rows.Add("79", "SI", "o", "79", "134111");
            dt.Rows.Add("80", "DLE", "p", "80", "111242");
            dt.Rows.Add("81", "DC1", "q", "81", "121142");
            dt.Rows.Add("82", "DC2", "r", "82", "121241");
            dt.Rows.Add("83", "DC3", "s", "83", "114212");
            dt.Rows.Add("84", "DC4", "t", "84", "124112");
            dt.Rows.Add("85", "NAK", "u", "85", "124211");
            dt.Rows.Add("86", "SYN", "v", "86", "411212");
            dt.Rows.Add("87", "ETB", "w", "87", "421112");
            dt.Rows.Add("88", "CAN", "x", "88", "421211");
            dt.Rows.Add("89", "EM", "y", "89", "212141");
            dt.Rows.Add("90", "SUB", "z", "90", "214121");
            dt.Rows.Add("91", "ESC", "{", "91", "412121");
            dt.Rows.Add("92", "FS", "|", "92", "111143");
            dt.Rows.Add("93", "GS", "}", "93", "111341");
            dt.Rows.Add("94", "RS", "~", "94", "131141");
            dt.Rows.Add("95", "US", "DEL", "95", "114113");
            dt.Rows.Add("96", "FNC3", "FNC3", "96", "114311");
            dt.Rows.Add("97", "FNC2", "FNC2", "97", "411113");
            dt.Rows.Add("98", "SHIFT", "SHIFT", "98", "411311");
            dt.Rows.Add("99", "CODEC", "CODEC", "99", "113141");
            dt.Rows.Add("100", "CODEB", "FNC4", "CODEB", "114131");
            dt.Rows.Add("101", "FNC4", "CODEA", "CODEA", "311141");
            dt.Rows.Add("102", "FNC1", "FNC1", "FNC1", "411131");
            dt.Rows.Add("103", "StartA", "StartA", "StartA", "211412");
            dt.Rows.Add("104", "StartB", "StartB", "StartB", "211214");
            dt.Rows.Add("105", "StartC", "StartC", "StartC", "211232");
            dt.Rows.Add("106", "Stop", "Stop", "Stop", "2331112");
        }

        public Bitmap Get128AutoImage(string strText)
        {
            string strcheck = "";
            string strStart = "";
            int intcheck = 0;
            strBarcode = "";
            int i = 0;
            List<string> tem = new List<string>();
            Regex rg = new Regex(@"^[0-9]{2}.*");

            bool blstartnumber = false;
            bool blstartoterh = false;
            #region 对字符串进行拆分，拆分为CODEC和CODEA部分
            while (i < strText.Length - 1 && strText.Substring(i, 1) != "…")
            {
                if ((strText.Length - i & 1) != 0)
                {
                    strText += "…";
                }
                if (rg.IsMatch(strText.Substring(i, 2)))
                {
                    if (blstartnumber == false)
                    {
                        tem.Add("CODEC");
                        blstartnumber = true;
                    }
                    tem.Add(strText.Substring(i, 2));
                    i += 2;
                    blstartnumber = true;
                    blstartoterh = false;
                }
                else
                {
                    blstartnumber = false;
                    if (blstartoterh == false)
                    {
                        tem.Add("CODEA");
                        blstartoterh = true;
                    }
                    while (i < strText.Length && strText.Substring(i, 1) != "…")
                    {
                        if (i == strText.Length - 1)
                        {
                            strText += "…";
                        }
                        if (!rg.IsMatch(strText.Substring(i, 2)))
                        {
                            if (strText.Substring(i, 1) != "…")
                            {
                                tem.Add(strText.Substring(i, 1));

                            }
                            i += 1; blstartoterh = true; blstartnumber = false;
                        }
                        else
                        {
                            blstartoterh = false;
                            break;
                        }
                    }
                }
            }
            #endregion

            #region 对起始位进行处理
            switch (tem[0].ToString())
            {
                case "CODEA": tem[0] = "103"; strStart = "211412"; break;
                case "CODEC": tem[0] = "105"; strStart = "211232"; break;
                default: strStart = ""; break;
            }
            #endregion

            #region 进行校验位的处理以及条码字符串处理
            for (int j = 1; j < tem.Count; j++)
            {
                if (tem[j].ToString().Length > 1)
                {
                    if (tem[j].ToString() == "CODEA")
                    {
                        intcheck += 101 * j;
                        strBarcode += "311141";
                    }
                    else
                    {
                        if (tem[j].ToString() == "CODEC")
                        {
                            intcheck += 99 * j;
                            strBarcode += "113141";
                        }
                        else
                        {
                            foreach (DataRow dr in dt.Rows)
                            {
                                if (dr[3].ToString().Equals(tem[j].ToString()))
                                {
                                    intcheck += Int32.Parse(dr[0].ToString()) * j;
                                    strBarcode += dr[4].ToString();
                                }
                            }
                        }
                    }
                }
                else
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (dr[1].ToString().Equals(tem[j].ToString()))
                        {
                            intcheck += Int32.Parse(dr[0].ToString()) * j;
                            strBarcode += dr[4].ToString();
                        }
                    }
                }
            }
            #endregion 

            #region 计算校验位，并获取其编码
            intcheck += Int32.Parse(tem[0]);
            strcheck = (intcheck % 103).ToString();


            foreach (DataRow DR in dt.Rows)
            {
                if (DR[0].ToString().Equals(strcheck))
                {
                    strcheck = DR[4].ToString();
                    break;
                }
            }
            #endregion

            strBarcode = strStart + strBarcode + strcheck + "2331112";
            strText = strText.Replace("…", "");

            Bitmap _CodeImage = GetImage(strBarcode);
            return _CodeImage;

            //for (int k = 0; k < strBarcode.Length; k++)
            //{
            //    if (k % 2 == 0)
            //    {
            //        for (int j = 0; j < Int32.Parse(strBarcode.Substring(k, 1)); j++)
            //        {
            //            e.Graphics.FillRectangle(Brushes.Black, new RectangleF(xf, yf, widthf, heightf));
            //            xf += widthf;
            //        }
            //    }
            //    else
            //    {
            //        for (int j = 0; j < Int32.Parse(strBarcode.Substring(k, 1)); j++)
            //        {
            //            e.Graphics.FillRectangle(Brushes.White, new RectangleF(xf, yf, widthf, heightf));
            //            xf += widthf;
            //        }
            //    }
            //}

        }

        private uint m_Height = 40;

        /// <summary>
        /// 高度
        /// </summary>
        public uint Height
        {
            get { return m_Height; }
            set { m_Height = value; }
        }
        private byte m_Magnify = 0;

        /// <summary>
        /// 放大倍数
        /// </summary>
        public byte Magnify
        {
            get { return m_Magnify; }
            set { m_Magnify = value; }
        }
        public Bitmap GetImage(string p_Text)
        {
            char[] _Value = p_Text.ToCharArray();
            int _Width = 0;
            for (int i = 0; i != _Value.Length; i++)
            {
                _Width += Int32.Parse(_Value[i].ToString()) * (m_Magnify + 2);
            }

            // Bitmap _CodeImage = new Bitmap(_Width, (int) m_Height);
            Bitmap _CodeImage = new Bitmap(_Width, (int)m_Height);
            Graphics _Garphics = Graphics.FromImage(_CodeImage);
            //Pen _Pen;
            int _LenEx = 0;
            for (int i = 0; i != _Value.Length; i++)
            {
                int _ValueNumb = Int32.Parse(_Value[i].ToString()) * (m_Magnify + 2); //获取宽和放大系数
                if (!((i & 1) == 0))
                {
                    //_Pen = new Pen(Brushes.White, _ValueNumb);
                    //var brush = new SolidBrush(Color.FromArgb(255, (byte)59, (byte)42, (byte)16));
                    _Garphics.FillRectangle(Brushes.White, new Rectangle(_LenEx, 0, _ValueNumb, (int)m_Height));
                }
                else
                {
                    //_Pen = new Pen(Brushes.Black, _ValueNumb);
                    _Garphics.FillRectangle(Brushes.Black, new Rectangle(_LenEx, 0, _ValueNumb, (int)m_Height));
                }
                //_Garphics.(_Pen, new Point(_LenEx, 0), new Point(_LenEx, m_Height));
                _LenEx += _ValueNumb;
            }
            _Garphics.Dispose();
            return _CodeImage;
        }
        #endregion
    }
}
