using _9M.Work.Model;
using _9M.Work.Utility;
using _9Mg.Work.TopApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Xsl;
using Top.Api.Domain;

namespace _9M.Work.WPF_Main.Views.EveryDayUpdate
{
    public class ShopBll
    {

        #region 单例方式来获取实例

        private static ShopBll _bll;

        private ShopBll() { }

        public static ShopBll InstanceEverydayUpdateBll()
        {
            return _bll ?? (new ShopBll());
        }

        #endregion


        /*  --------------------------------     上新处理方法     --------------------------------  */
        #region 上新处理方法


        /// <summary>
        /// 根据文件(Excel)转换为DataTable
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public DataTable GetTableByFilePath(string filePath)
        {
            DataTable table = ExcelNpoi.ExcelToDataTable("sheet1", true, filePath);
            return table;
        }


        /// <summary>
        /// 根据上传款号来获取商品
        /// </summary>
        /// <param name="filePath">上传文件绝对路径</param>
        /// <param name="type">0：非法款，1：合格款</param>
        /// <returns></returns>
        public List<string> GetGoodsnoList(string filePath, Goods type)
        {
            List<string> goodsList = new List<string>();
            List<string> badGoodsList = new List<string>();
            //匹配款号是否合法
            string pattern = @"^\d{2}\w{2,4}\d{0,5}$";
            if (Path.GetExtension(filePath).ToUpper().Equals(".TXT"))
            {
                StreamReader sr = new StreamReader(filePath, Encoding.Default);
                string s;
                while ((s = sr.ReadLine()) != null)
                {
                    Match m = Regex.Match(s, pattern);
                    if (!m.Success)
                    {
                        badGoodsList.Add(s);
                    }
                    else
                    {
                        goodsList.Add(s);
                    }
                }
                if (type == Goods.Bad)
                {
                    return badGoodsList;
                }
                else
                {
                    return goodsList;
                }
            }
            else
            {
                DataTable table = this.GetTableByFilePath(filePath);
                if (table != null && table.Rows.Count > 0)
                {

                    foreach (DataRow dr in table.Rows)
                    {
                        string outerId = dr["款号"].ToString().Trim();


                        Match m = Regex.Match(outerId, pattern);
                        if (!m.Success)
                        {
                            badGoodsList.Add(outerId);
                        }
                        else
                        {
                            goodsList.Add(outerId);
                        }
                    }

                    if (type == Goods.Bad)
                    {
                        return badGoodsList;
                    }
                    else
                    {
                        return goodsList;
                    }
                }
            }
            return null;
        }


        /// <summary>
        /// 获取款和价格对应数据
        /// </summary>
        /// <param name="filePath">路径</param>
        /// <param name="list">合格款</param>
        /// <returns></returns>
        public List<UploadExcelModel> GetGoodsnoAndPriceList(string filePath, List<string> list, CheckBox cb)
        {
            List<UploadExcelModel> list1 = new List<UploadExcelModel>();
            DataTable table = this.GetTableByFilePath(filePath);
            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow dr in table.Rows)
                {
                    UploadExcelModel uem = new UploadExcelModel();
                    uem.Goodsno = dr["款号"].ToString().Trim();
                    string item = list.Find(delegate(string str)
                    {
                        return str.Equals(uem.Goodsno);
                    });

                    if (item != null && cb.IsChecked == true)
                    {
                        uem.Price = dr["价格"].ToString().Trim();
                        if (!string.IsNullOrEmpty(uem.Price))
                        {
                            list1.Add(uem);
                        }
                    }
                }
            }
            return list1;
        }


        /// <summary>
        /// 获取商品链接地址
        /// </summary>
        /// <param name="shopType"></param>
        /// <param name="numIid"></param>
        /// <returns></returns>
        public string GetDetailUrl(ShopType shopType, string numIid)
        {
            string detailUrl = string.Empty;
            if (shopType == ShopType.CShop_9M)
            {
                detailUrl = string.Format("http://item.taobao.com/item.htm?id={0}", numIid);
            }
            else if (shopType == ShopType.BShop_9MG || shopType == ShopType.BShop_Mezingr)
            {
                detailUrl = string.Format("http://detail.tmall.com/item.htm?id={0}", numIid);
            }
            return detailUrl;
        }


        /// <summary>
        /// 转换为自定义实体类
        /// </summary>
        /// <param name="itemList"></param>
        /// <param name="list"></param>
        /// <param name="shopType"></param>
        /// <param name="priceType"></param>
        /// <returns></returns>
        public List<CustomItem> ConvertToCustomItem(List<Item> itemList, List<UploadExcelModel> list, ShopType shopType, PriceType priceType)
        {

            List<CustomItem> customList = new List<CustomItem>();
            foreach (Item item in itemList)
            {
                try
                {
                    CustomItem customItem = new CustomItem();
                    customItem.DetailUrl = GetDetailUrl(shopType, item.NumIid.ToString());
                    customItem.OldPrice = item.Price;
                    customItem.Title = item.Title;
                    customItem.OuterId = item.OuterId;
                    customItem.PicUrl = item.PicUrl;
                    customItem.FivePicUrl = item.ItemImgs.Count >= 5 ? item.ItemImgs[4].Url : string.Empty;
                    if (priceType == PriceType.Custom)
                    {
                        UploadExcelModel model = list.Find(delegate(UploadExcelModel m)
                        {
                            return m.Goodsno == item.OuterId;
                        });
                        if (model != null)
                        {
                            customItem.NewPrice = model.Price;
                        }
                    }
                    else
                    {
                        customItem.NewPrice = item.Price;
                    }
                    customList.Add(customItem);
                }
                catch (Exception ex)
                {

                }
            }
            return customList;
        }

        /// <summary>
        /// 转换为自定义实体类
        /// </summary>
        /// <param name="itemList"></param>
        /// <param name="list"></param>
        /// <param name="shopType"></param>
        /// <param name="priceType"></param>
        /// <returns></returns>
        public List<CustomItem> ConvertToCustomItem(List<Item> itemList, ShopType shopType)
        {
            List<CustomItem> customList = new List<CustomItem>();
            foreach (Item item in itemList)
            {
                CustomItem customItem = new CustomItem();
                customItem.DetailUrl = GetDetailUrl(shopType, item.NumIid.ToString());
                customItem.OldPrice = item.Price;
                customItem.Title = item.Title;
                customItem.OuterId = item.OuterId;
                customItem.PicUrl = item.PicUrl;
                customList.Add(customItem);
            }
            return customList;
        }





        /// <summary>
        /// 获取输入的款
        /// </summary>
        /// <param name="goodsno">款</param>
        /// <param name="shopType">店铺类型</param>
        /// <returns></returns>
        public List<CustomItem> GetItems(string goodsno, ShopType shopType)
        {
            //模拟http请求向服务器发出请求
            string url = String.Format("http://my.9mg.cc/ajaxhandle/windatahandler.ashx", goodsno);
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("method", (object)"everydayUpdate");
            dic.Add("goodsno", (object)goodsno);
            dic.Add("type", (int)shopType);
            dic.Add("requestkey", (object)"E045443B12BC");
            HttpWebResponse response = HttpHelper.CreatePostHttpResponse(url, dic, null, null, Encoding.Default, null);
            StreamReader srdPreview = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            String temp = srdPreview.ReadToEnd();
            List<CustomItem> items = (List<CustomItem>)JsonConvert.DeserializeObject(temp, typeof(List<CustomItem>));
            return items;
        }





        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="res"></param>
        /// <returns></returns>
        public bool SaveFile(string fileName, string res, string DirName)
        {
            try
            {
                //获取当前用户桌面的路径
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                //保存路径
                string saveCodePath = desktopPath + string.Format("\\{0}\\", DirName);
                //如果没有就创建文件
                if (!Directory.Exists(saveCodePath))
                {
                    Directory.CreateDirectory(saveCodePath);
                }
                //拼接完成之后写入到文件中
                System.IO.File.WriteAllText(string.Format(saveCodePath + "{0}.txt", fileName), res);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        #endregion


        /*  --------------------------------    专题款处理方法    --------------------------------  */
        #region 专题款处理方法

        /// <summary>
        /// 获取到上传文件里面的款号
        /// </summary>
        /// <param name="FilePath"></param>
        /// <returns></returns>
        public List<string> GetListByFilePath(string FilePath)
        {
            List<string> list = new List<string>();

            string[] lines = File.ReadAllLines(FilePath);
            if (lines != null && lines.Length > 0)
            {
                foreach (string s in lines)
                {
                    list.Add(s);
                }
            }
            return list;
        }


        /// <summary>
        /// 根据上传文件里面的款号来进行排序
        /// </summary>
        /// <param name="itemList">店铺商品集合</param>
        /// <param name="goodsnoList">上传文件合法款号</param>
        /// <returns></returns>
        public List<Item> SortItemList(List<Item> itemList, List<string> goodsnoList)
        {
            List<Item> sortList = new List<Item>();
            foreach (string s in goodsnoList)
            {
                Item item = itemList.Find(delegate(Item i)
                {
                    return i.OuterId.Equals(s);
                });

                if (item != null)
                {
                    sortList.Add(item);
                }
            }
            return sortList;
        }
        #endregion


        /// <summary>
        /// 获取价格标志
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        internal string GetPriceFlag(int index)
        {
            switch (index)
            {
                case 0: return "p_5";
                case 1: return "p_10";
                case 2: return "p_20";
                case 3: return "p_30";
                default:
                    return "p_5";
            }
        }



        public static string UserName { get; set; }

        public static int UserId { get; set; }



        /// <summary>
        /// 获取每日上新模版路径
        /// </summary>
        public static string GetTemplateDir_EverydayUpdateForCShop
        {
            get
            {
                return AppDomain.CurrentDomain.BaseDirectory + @"templates\EverydayUpdateForCShop.xslt";
            }
        }
        /// <summary>
        /// 获取每日上新(专题1100)模版路径(BY WL)
        /// </summary>
        public static string GetTemplateDir_EverydayUpdateForCShopByZT
        {
            get
            {
                return AppDomain.CurrentDomain.BaseDirectory + @"templates\zt1100.xslt";
            }
        }

        /// <summary>
        /// 获取每日上新(专题(分销950))模版路径(BY WL)
        /// </summary>
        public static string GetTemplateDir_EverydayUpdateForFXShopByZT
        {
            get
            {
                return AppDomain.CurrentDomain.BaseDirectory + @"templates\zttable950.xslt";
            }
        }

        public static string GetTemplateDir_EverydayUpdateForBShop
        {
            get
            {
                return AppDomain.CurrentDomain.BaseDirectory + @"templates\EverydayUpdateForBShop.xslt";
            }
        }

        public static string GetTemplateDir_PcActivity
        {
            get 
            {
                return AppDomain.CurrentDomain.BaseDirectory + @"templates\pcactivity.xslt";
            }
        }

        /// <summary>
        /// 获取促销模板(BY WL)
        /// </summary>
        public static string GetTemplateDir_EverydayUpdateForCShopByCX
        {
            get
            {
                return AppDomain.CurrentDomain.BaseDirectory + @"templates\cx1100.xslt";
            }
        }




        /// <summary>
        /// 获取每周模版路径
        /// </summary>
        public static string GetTemplateDir_EverydayWeek
        {
            get
            {
                return AppDomain.CurrentDomain.BaseDirectory + @"templates\EveryweekUpdate.xslt";
            }
        }


        #region "将xml通过xsl转换"
        /// <summary>
        /// 转换xml和xsl为html文档
        /// </summary>
        /// <param name="xsl_name">xsl路径</param>
        /// <param name="xml_Data">xml内容</param>
        /// <returns>转换后字符</returns>
        public static string Xslt(ShopType st, string xml_Data, string xslTemplatePath)
        {
            StringWriter writer = new StringWriter();
            //装入xml对象 
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.LoadXml(xml_Data);
            string xsl_name = xslTemplatePath;
            /*if (ShopType.BShop_9MG == st)
            {
                xsl_name = GetTemplateDir_EverydayUpdateForBShop;
            }*/
            //装入xsl对象 
            XslCompiledTransform xsldoc = new XslCompiledTransform();
            xsldoc.Load(xsl_name);
            xsldoc.Transform(xmldoc, null, writer);
            string str = writer.ToString().Replace("<b class=\"bg\" />", "<b class=\"bg\"></b>");
            writer.Dispose();
            writer.Close();
            return str;
        }
        #endregion

        #region "序列化类为xml文件"
        /// <summary>
        /// 序列化xml
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string Serialize<T>(T t)
        {
            using (StringWriter sw = new StringWriter())
            {
                XmlSerializer xz = new XmlSerializer(t.GetType());
                xz.Serialize(sw, t);
                return sw.ToString();
            }
        }
        #endregion


        #region 创建一个HTTP请求返回JSON数据

        /// <summary>
        /// 创建一个HTTP请求返回JSON数据
        /// </summary>
        /// <param name="Url"></param>
        /// <param name="Data"></param>
        /// <returns></returns>
        public static string GetResponseString(string Url, string Data)
        {
            string postString = Data;
            ; //这里即为传递的参数，可以用工具抓包分析，也可以自己分析，主要是form里面每一个name都要加进来  
            byte[] postData = Encoding.UTF8.GetBytes(postString); //编码，尤其是汉字，事先要看下抓取网页的编码方式  
            string url = Url; //地址  
            WebClient webClient = new WebClient();
            webClient.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            //采取POST方式必须加的header，如果改为GET方式的话就去掉这句话即可  
            byte[] responseData = webClient.UploadData(url, "POST", postData); //得到返回字符流  
            string srcString = Encoding.UTF8.GetString(responseData); //解码 
            return srcString;
        }

        #endregion




    }
}
