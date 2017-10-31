using _9M.Work.Model;
using _9M.Work.Utility;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _9M.Work.WPF_Main.FrameWork
{
    public class GlobalUtil
    {
        /// <summary>
        /// 解析导入的款号
        /// </summary>
        /// <returns></returns>
        public static List<string> GetImport()
        {
            List<string> list = null;
            OpenFileDialog op = new OpenFileDialog();
            //过滤器
            op.Filter = "XLS|*.xls|XLSX|*.xlsx|TXT|*.txt";
            if (op.ShowDialog() == true) //选择完成之后
            {
                if (System.IO.Path.GetExtension(op.FileName.ToLower()).Equals(".txt"))
                {
                    list = FileHelper.RealFileLine(op.FileName);
                }
                else
                {
                    DataTable dt = ExcelNpoi.ExcelToDataTable("sheet1", true, op.FileName);
                    if (!dt.Columns[0].ColumnName.Equals("款号"))
                    {
                        CCMessageBox.Show("列头为款号");
                        return null;
                    }
                    list = new List<string>();
                    foreach (DataRow dr in dt.Rows)
                    {
                        list.Add(dr["款号"].ToString().Trim());
                    }
                }
                list = list.Distinct().Where(x => !string.IsNullOrEmpty(x)).ToList();
                if (list.Count > 0)
                {
                    CCMessageBox.Show("导入商品件数:" + list.Count);
                }
            }
            return list;
        }

        public static DataTable ReadExcel(OpenFileDialog op)
        {
            DataTable dt = null;
            op.Filter = "Excel 文件(*.xls)|*.xls|Excel 文件(*.xlsx)|*.xlsx|所有文件(*.*)|*.*";
            if (op.ShowDialog() == true)
            {
                dt = ExcelNpoi.ExcelToDataTable("sheet1", true, op.FileName);
                dt = dt.Columns.Count > 0 ? dt : null;
            }
            return dt;
        }

        /// <summary>
        /// 得到描述的代码
        /// </summary>
        /// <param name="GoodsNo"></param>
        /// <returns></returns>
        public static string Desc(string GoodsNo)
        {
            //<P align=center><IMG align=absMiddle src=" + Url + "/" + GoodsNo + "-0.jpg" + "></P>
            //描述图地址
            string BaseUrl = "http://192.168.1.2:81/9mgimg/2017/images/c";
            string Url = BaseUrl + @"/Detail/" + GoodsNo;
            string append = string.Empty;
            //吊牌
            //if (HttpHelper.GetHead(Url + @"/" + GoodsNo + "-0.jpg").Equals("200"))
            //{
            //    append = "<P align='center'><IMG align='absMiddle' src='" + Url + "/" + GoodsNo + "-0.jpg'></P>";
            //}
            string Html = string.Format("<P align='center'><IMG src='" + BaseUrl + "/Size/" + GoodsNo + ".png" + "'></P>{0}<P align='center'><IMG src='" + BaseUrl + "/Detail/xj.gif" + "'></P>", append);
            List<string> alink = FileHelper.GetImageFile(Url);
            alink.RemoveAll(x => x.Equals(GoodsNo + "-0.jpg", StringComparison.CurrentCultureIgnoreCase));
            Dictionary<int, string> dic = new Dictionary<int, string>();
            int Order = 0;
            for (int i = 0; i < alink.Count; i++)
            {
                string File = Path.GetFileNameWithoutExtension(alink[i]);
                string[] arry = File.Split('-');
                int flag = 0;
                if (arry.Length > 1)
                {
                    int.TryParse(arry[1], out flag);
                }
                if (!dic.Keys.Contains(flag))
                {
                    dic.Add(flag, alink[i]);
                }
                else
                {
                    Order = Order - 1;
                    dic.Add(Order, alink[i]);
                }
               
            }
            var list = dic.OrderBy(x => x.Key).ToList();
            foreach (var str in list)
            {
                Html = Html + "<P align='center'><IMG align='absMiddle' src='" + Url + "/" + Path.GetFileName(str.Value) + "'></P>";
            }
            Html = Html + "<P align = 'center' ><IMG align = 'absMiddle' src = 'http://192.168.1.2:81/9mgimg/2017/images/C/Detail/LB.jpg' ></P >";
            return Html;
        }

        /// <summary>
        /// 得到描述的所有图片
        /// </summary>
        /// <param name="GoodsNo"></param>
        /// <returns></returns>
        public static List<string> DescImageList(string GoodsNo)
        {
            List<string> imagelist = new List<string>();
            string BaseUrl = "http://192.168.1.2:81/9mgimg/2017/images/c";
            string Url = BaseUrl + @"/Detail/" + GoodsNo;
            imagelist.Add(BaseUrl + "/Size/" + GoodsNo + ".png");
            //if (HttpHelper.GetHead(Url + @"/" + GoodsNo + "-0.jpg").Equals("200"))
            //{
            //    imagelist.Add(Url + @"/" + GoodsNo + "-0.jpg");
            //}
            imagelist.Add(BaseUrl + "/Detail/xj.gif");

            List<string> alink = FileHelper.GetImageFile(Url);
            alink.RemoveAll(x => x.Equals(GoodsNo + "-0.jpg", StringComparison.CurrentCultureIgnoreCase));
            Dictionary<int, string> dic = new Dictionary<int, string>();
            for (int i = 0; i < alink.Count; i++)
            {

                string File = Path.GetFileNameWithoutExtension(alink[i]);
                string[] arry = File.Split('-');
                int flag = 0;
                if (arry.Length > 1)
                {
                    int.TryParse(arry[1], out flag);
                }
                dic.Add(flag, alink[i]);

            }
            var list = dic.OrderBy(x => x.Key).ToList();
            foreach (var str in list)
            {
                imagelist.Add(Url + "/" + Path.GetFileName(str.Value));
            }
            imagelist.Add("http://192.168.1.2:81/9mgimg/2017/images/C/Detail/LB.jpg");
            return imagelist;
        }


        /// <summary>
        /// 福袋描述文字
        /// </summary>
        /// <param name="Goodslist"></param>
        /// <returns></returns>
        public static string FuDaiDesc(List<FuDaiGoodsModel> Goodslist)
        {
            string BaseUrl = "http://192.168.1.2:81/InWorkImg/";
            string Html = string.Empty;
            Goodslist.ForEach(x=> {
                string Src = BaseUrl + x.GoodsNo + "/" + x.ImageUrl + ".jpg";
                Html = Html + string.Format("<P align='center'><IMG align='absMiddle' src='{0}'></P>", Src);
            });
            return Html;
        }


        /// <summary>
        /// 福袋描述图片路径
        /// </summary>
        /// <param name="Goodslist"></param>
        /// <returns></returns>
        public static List<string> FuDaiImageUrl(List<FuDaiGoodsModel> Goodslist)
        {
            List<string> list = new List<string>();
            string BaseUrl = "http://192.168.1.2:81/InWorkImg/";
            string Html = string.Empty;
            Goodslist.ForEach(x => {
                string Src = BaseUrl + x.GoodsNo + "/" + x.ImageUrl + ".jpg";
                list.Add(Src);
            });
            return list;
        }
    }
}
