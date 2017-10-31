using _9M.Work.DbObject;
using _9M.Work.Model;
using _9M.Work.Model.WdgjWebService;
using _9M.Work.TopApi;
using _9M.Work.Utility;
using _9M.Work.YouZan;
using _9Mg.Work.TopApi;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Top.Api;
using Top.Api.Domain;
using Top.Api.Request;
using Top.Api.Response;

namespace DebugProject
{
    class Program
    {

        public static UserInfoModel GetInt()
        {
            return new UserInfoModel() { UserName = "Admin" };
        }

        private static void CallBack(UserInfoModel obj)
        {
            var it = obj;
        }
        static BaseDAL dal = new BaseDAL("server=192.168.1.2,3433;database=InSideWorkServer;uid=sa;pwd=www.9mg.cn");
        public static void GetCid()
        {
            ShopModel shop = dal.GetAll<ShopModel>().Where(x => x.shopId == 1000).FirstOrDefault();
            ITopClient client = new DefaultTopClient("http://gw.api.taobao.com/router/rest", shop.appKey, shop.appSecret);
            ItemcatsGetRequest req = new ItemcatsGetRequest();
            //req.Cids = "16";
            req.Fields = "cid,parent_cid,name,is_parent";
            req.ParentCid = 16;
            ItemcatsGetResponse rsp = client.Execute(req, shop.sessionKey);
            Console.WriteLine(rsp.Body);
        }

        public static void FenLei()
        {
            List<string> outlist = new List<string>();
            StreamReader sr = new StreamReader(@"C:\Users\Administrator\Desktop\abc.txt", Encoding.Default);
            String line;
            while ((line = sr.ReadLine()) != null)
            {
                outlist.Add(line.ToString());
            }
            TopSource top = new TopSource();
            List<Item> itemlist = top.GetItemList(outlist, "seller_cids");
            itemlist.ForEach(x =>
            {
                //1237123532
                string Sellercid = "," + x.SellerCids.Replace("1263821046", "").TrimStart(',').TrimEnd(',') + ",";
                bool bs = top.UpdateCid(x.NumIid, Sellercid);
            });
        }

        public class TradePayModel
        {
            public string ConfirmOperator { get; set; }
            public DateTime ConfirmTime { get; set; }
        }

        public class StaModel
        {
            public string Brand { get; set; }
            public int Count { get; set; }
            public decimal Total { get; set; }
        }
        static void Main(string[] args)
        {

            DataTable dtsa = ExcelNpoi.ExcelToDataTable("Sheet1", true, @"C:\Users\Administrator\Desktop\删除订单.xls");
            string fa = string.Empty;
            foreach (DataRow dr in dtsa.Rows)
            {

                fa += string.Format(@"delete T_TradeGoodsList set ischeck=1 where goodsno='{0}'", dr[0].ToString());
                fa += "\r\n";
              
            }
            return;
            string d = "server=59.173.238.102,3433;database=Db_JunZhe_O2O_V2;uid=sa;pwd=www.9mg.cn";
            var ti =     new Desc().Encode(d);
            var ts = new Desc().Decode(ti);
            return;
            TopSource top = new TopSource();
            BaseDAL dals = new BaseDAL("server=192.168.1.2,3433;database=InSideWorkServer;uid=sa;pwd=www.9mg.cn");
            YzApi yz = new YzApi();


            List<string> goodslist = new List<string>();
            DirectoryInfo folder = new DirectoryInfo(@"C:\Users\Administrator\Desktop\水印图");
            var files = folder.GetFiles();
            foreach (var item in files)
            {
                goodslist.Add(Path.GetFileNameWithoutExtension(item.FullName));
            }

            DataTable dt = new DataTable();
            dt.Columns.Add("款号");
            goodslist.ForEach(x=> {
                DataRow dr = dt.NewRow();
                dr["款号"] = x;
                dt.Rows.Add(dr);
            });
            ExcelNpoi.TableToExcel(dt,@"c:/fd.xls");
            // top.GetItemList(goodslist,"item_img");

            //List<GoodsDetail> list = new List<GoodsDetail>();
            //list.AddRange(yz.OnLineGoodsList(true, ""));
            //list.AddRange(yz.OnLineGoodsList(false, ""));
            // list = list.Where(x => goodslist.Contains(x.outer_id)).ToList();
            string Sql = string.Format(@"select * from Picture where Url like 'z:\后期工作区\成品（上新用）\2016%' and Url like '%-1.jpg'
and GoodsNo in('{0}')", string.Join(",", goodslist).Replace(",", "','"));
            var imagelist = dals.QueryList<ImageEntity>(Sql, new object[] { });
            var Group = imagelist.GroupBy(x => x.GoodsNo);

            foreach (string Url in goodslist)
            {
                var ls = Group.Where(x => x.Key.Equals(Url, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                if (ls != null)
                {
                    var lst = ls.ToList();
                    foreach (var it in lst)
                    {

                        Bitmap pic = new Bitmap(it.Url);
                        int width = pic.Size.Width; // 图片的宽度
                        int height = pic.Size.Height; // 图片的高度
                        if (width == 800 && height == 800)
                        {
                            File.Copy(it.Url, @"C:\Users\Administrator\Desktop\替换图片\" + ls.Key + ".jpg");
                            break;
                        }

                    }
                }
            }



            return;
            DataTable dtr = new DataTable();
            dtr.Columns.Add("款号");
            dtr.Columns.Add("卖点");
            TopSource com = new TopSource();
            List<Item> itemlist = new List<Item>();
            itemlist.AddRange(com.OnSaleList("", null, null, "num,postage_id"));
            itemlist.AddRange(com.InventoryList("", string.Empty, null, null, "num,postage_id"));
            List<string> GoodsList = itemlist.Select(x => x.OuterId).ToList();
            int ListCount = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(GoodsList.Count) / 40));
            for (int i = 0; i < ListCount; i++)
            {
                var ls = com.GetItemList(GoodsList.Skip(i * 40).Take(40).ToList(), "sell_point");
                ls.ForEach(x =>
                {
                    DataRow dr = dtr.NewRow();
                    dr["款号"] = x.OuterId;
                    dr["卖点"] = x.SellPoint;
                    dtr.Rows.Add(dr);
                });
            }
            ExcelNpoi.TableToExcel(dtr, @"c:/Sell_point.xls");
            return;

            #region 获取重量
            //string Ress = string.Empty;
            //DataTable dt = new DataTable();
            //dt.Columns.Add("款号");
            //dt.Columns.Add("重量");
            //DataTable dts = ExcelNpoi.ExcelToDataTable("Sheet1", true, @"C:\Users\Administrator\Desktop\没重量款号.xlsx");
            //BaseDAL dal = new BaseDAL("server=192.168.1.2,3433;database=WorkPlatform;uid=sa;pwd=www.9mg.cn");
            //var list =  dal.QueryList<DataColModel>("select GoodsNo as Name ,ZhongLiang as Ctype from  dbo.tGoodsBaseInfo ",new object[] { });
            //foreach (DataRow dr in dts.Rows)
            //{
            //    string GoodsNo = dr[0].ToString();
            //    string ZhongXiang = string.Empty;
            //    var flag =   list.Find(x=> {
            //        return x.Name.Equals(GoodsNo,StringComparison.CurrentCultureIgnoreCase);
            //    });
            //    if (flag!=null)
            //    {
            //        if (!string.IsNullOrEmpty(flag.Ctype))
            //        {
            //            DataRow drs = dt.NewRow();
            //            drs["款号"] = GoodsNo;
            //            drs["重量"] = flag.Ctype.Replace("g","");
            //            dt.Rows.Add(drs);
            //        }

            //    }
            //}

            //ExcelNpoi.TableToExcel(dt,@"C:\ZhongLiang.xls");

            #endregion
            //  update G_Goods_GoodsList set Weight = { } where GoodsNo =
            BaseDAL dal = new BaseDAL("jconn8ih9giq9.sqlserver.rds.aliyuncs.com,3433;database=yttl_wdgj31;uid=jusrvhsgf5h6;pwd=yttl_20140121");
            string Ress = string.Empty;
            int ErrorCount = 0;
            int Count = 0;
            DataTable dts = ExcelNpoi.ExcelToDataTable("Sheet1", true, System.AppDomain.CurrentDomain.BaseDirectory + "ZhongLiang.xls");
            bool b = true;
            foreach (DataRow dr in dts.Rows)
            {
                try
                {
                    Ress = string.Format(@"update G_Goods_GoodsList set Weight='{0}' where GoodsNo='{1}'", dr[1], dr[0]);
                    int kss = dal.ExecuteSql(Ress);
                    b = kss > 0;
                }
                catch (Exception ex)
                {
                    b = false;
                }
                if (b)
                {
                    Count++;
                }
                else
                {
                    ErrorCount++;
                }
            }
            MessageBox.Show(string.Format("成功{0}   失败{1}", Count, ErrorCount));
        }

        public class DataColModel
        {
            public string Name { get; set; }
            public string Ctype { get; set; }
        }

        public class ImageEntity
        {
            public string GoodsNo { get; set; }
            public string Url { get; set; }
        }

        public class Desc
        {
            const string KEY_64 = "www9mgcn";//注意了，是8个字符，64位

            const string IV_64 = "www9mgcn";
            public string Encode(string data)
            {
                byte[] byKey = System.Text.ASCIIEncoding.ASCII.GetBytes(KEY_64);
                byte[] byIV = System.Text.ASCIIEncoding.ASCII.GetBytes(IV_64);

                DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
                int i = cryptoProvider.KeySize;
                MemoryStream ms = new MemoryStream();
                CryptoStream cst = new CryptoStream(ms, cryptoProvider.CreateEncryptor(byKey, byIV), CryptoStreamMode.Write);

                StreamWriter sw = new StreamWriter(cst);
                sw.Write(data);
                sw.Flush();
                cst.FlushFinalBlock();
                sw.Flush();
                return Convert.ToBase64String(ms.GetBuffer(), 0, (int)ms.Length);

            }

            public string Decode(string data)
            {
                byte[] byKey = System.Text.ASCIIEncoding.ASCII.GetBytes(KEY_64);
                byte[] byIV = System.Text.ASCIIEncoding.ASCII.GetBytes(IV_64);

                byte[] byEnc;
                try
                {
                    byEnc = Convert.FromBase64String(data);
                }
                catch
                {
                    return null;
                }

                DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
                MemoryStream ms = new MemoryStream(byEnc);
                CryptoStream cst = new CryptoStream(ms, cryptoProvider.CreateDecryptor(byKey, byIV), CryptoStreamMode.Read);
                StreamReader sr = new StreamReader(cst);
                return sr.ReadToEnd();
            }
        }
    }
}