using _9M.Work.DbObject;
using _9M.Work.ErpApi;
using _9M.Work.JosApi;
using _9M.Work.Model;
using _9M.Work.TopApi;
using _9M.Work.Utility;
using _9M.Work.WPF_Common;
using _9M.Work.WPF_Common.WpfBind;
using _9M.Work.WPF_Main.FrameWork;
using _9M.Work.WPF_Main.Infrastrcture;
using _9M.Work.WPF_Main.Views.Dialog;
using _9Mg.Work.TopApi;
using MahApps.Metro.Controls;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Top.Api.Domain;

namespace _9M.Work.WPF_Main.Views.DataCenter
{
    /// <summary>
    /// GoodsSync.xaml 的交互逻辑
    /// </summary>
    public partial class GoodsSync : UserControl, INotifyPropertyChanged
    {
        private DataTable mgupdatepricetable = new DataTable();
        private BaseDAL dal = new BaseDAL();
        private List<ShopModel> shoplist = null;
        private GoodsManager ware = new GoodsManager();
        public GoodsSync()
        {
            InitializeComponent();
            BindShop();
            this.DataContext = this;  //进度条必须加载
        }

        public void BindShop()
        {
            shoplist = dal.GetList<ShopModel>();
            shoplist.Insert(0, new ShopModel() { shopId = 0, shopName = "请选择" });
            ComboBoxBind.BindComboBox(Com_Shop, shoplist, "shopName", "shopId");
            Com_Shop.SelectedIndex = 0;
        }

        #region Event

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            int tag = Convert.ToInt32(btn.Tag);
            switch (tag)
            {
                case 1:  //确定 
                    if (mgupdatepricetable.Rows.Count <= 0)
                    {
                        CCMessageBox.Show("你没有导入正确的数据");
                        return;
                    }

                    if (!CommonLogin.CommonUser.IsDeptAdmin && !CommonLogin.CommonUser.IsAdmin)
                    {
                        //CCMessageBox.Show("您没有足够的权限完成修改");
                        //return;
                    }
                    if (Com_Shop.Text.Equals("分销"))
                    {
                        CCMessageBox.Show("店铺不支持该功能");
                        return;
                    }

                    if (CCMessageBox.Show("您确定要同步【" + Com_Shop.Text + "】的价格吗?", "提示", CCMessageBoxButton.YesNo) == CCMessageBoxResult.Yes)
                    {
                        ShopModel shop = shoplist.Where(x => x.shopId == Convert.ToInt32(Com_Shop.SelectedValue)).FirstOrDefault();
                        //OnlineStoreAuthorization model = ComboBoxBind.GetAuthorization(Com_Shop);
                        //实例委托
                        AsyncEventHandler asy = new AsyncEventHandler(this.UpdatePrice);
                        //异步调用开始，没有回调函数和AsyncState,都为null
                        IAsyncResult ia = asy.BeginInvoke(mgupdatepricetable, shop, null, null);
                    }
                    break;
                case 2:  //导入调价表 
                    OpenFileDialog op = new OpenFileDialog();
                    op.Filter = "Excel 文件(*.xls)|*.xls|Excel 文件(*.xlsx)|*.xlsx|所有文件(*.*)|*.*";
                    bool b = false;
                    if (op.ShowDialog() == true)
                    {
                        mgupdatepricetable = ExcelNpoi.ExcelToDataTable("sheet1", true, op.FileName);
                        if (mgupdatepricetable.Columns.Count >= 2)
                        {
                            if (mgupdatepricetable.Columns[0].ColumnName.Equals("款号") && mgupdatepricetable.Columns[1].ColumnName.Equals("价格"))
                            {
                                b = true;
                            }
                        }
                    }
                    else
                    {
                        return;
                    }
                    if (b == false)
                    {
                        CCMessageBox.Show("您导入的数据格式不正确");
                    }
                    else
                    {
                        CCMessageBox.Show("数据导入成功");
                    }
                    break;

                case 3: //输入款号
                    FormInit.OpenDialog(this, new GoodsNoDialog(this, 1), false);
                    break;
                case 4: //管家实价
                    DataTable dt = GlobalUtil.ReadExcel(new OpenFileDialog());
                    List<PostUpdateCommodityModel> list = new List<PostUpdateCommodityModel>();
                    foreach (DataRow dr in dt.Rows)
                    {
                        list.Add(new PostUpdateCommodityModel()
                        {
                            UpdateType = 4,
                            GoodsNo = dr["款号"].ToString(),
                            Reserved1 = dr["价格"].ToString()
                        });
                    }
                    bool bs = ware.UpdateGoods(list);
                    CCMessageBox.Show(bs ? "成功" : "失败");
                    break;
                case 5: //上新日期等
                    DataTable dts = GlobalUtil.ReadExcel(new OpenFileDialog());
                    List<PostUpdateCommodityModel> lists = new List<PostUpdateCommodityModel>();
                    foreach (DataRow dr in dts.Rows)
                    {
                        lists.Add(new PostUpdateCommodityModel()
                        {
                            UpdateType = 3,
                            GoodsNo = dr["款号"].ToString(),
                            ReMark = dr["删除备注"].ToString(),
                            Price_Detail = Convert.ToDecimal(dr["零售价"]),
                            Price2 = Convert.ToDecimal(dr["分销价"]),
                            SellDate = dr["上新时间"].ToString()
                        });
                    }
                    bool bss = ware.UpdateGoods(lists);
                    CCMessageBox.Show(bss ? "成功" : "失败");
                    break;
                case 6: //导入款号 
                    try
                    {
                        FormInit.OpenDialog(this, new GoodsNoDialog(this, 2), false);
                    }
                    catch (Exception ex)
                    {
                        CCMessageBox.Show(ex.Message);
                    }
                    break;

            }
        }
        #endregion


        #region 公共方法
        /// <summary>
        /// 修改价格
        /// </summary>
        /// <param name="dt"></param>
        public void UpdatePrice(DataTable dt, ShopModel model)
        {
            //创建一个TXT文件记录错误
            string filename = (DateTime.Now.ToShortDateString() + DateTime.Now.ToShortTimeString() + DateTime.Now.Second.ToString()).Replace("-", "").Replace(":", "");
            StreamWriter sw = new StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\价格同步日志-" + filename + ".txt");
            //打开加载
            bar.LoadBar(true);
            //打开加载中的字符
            bar.Loading(true);

            if (model.id == 1023) //如果是京东的调价(大于100的价格为整数。小于100可以小数)
            {
                //获取商品数据
                WareManager ware = new WareManager(model.appKey, model.appSecret, model.sessionKey);
                List<JdSdk.Domain.Ware.Ware> warelist = ware.SearchWares(null, null, null, null, null, null, null);
                if (warelist == null)
                {
                    CCMessageBox.Show("API错误,请重试\r\n");
                    bar.Loading(false);
                    bar.LoadBar(false);
                    return;
                }
                //需要修改价格的款号
                List<JdSdk.Domain.Ware.Ware> SyncList = new List<JdSdk.Domain.Ware.Ware>();
                //筛选出店铺中存在的款号
                foreach (DataRow dr in dt.Rows)
                {
                    string GoodsNo = dr["款号"].ToString();
                    JdSdk.Domain.Ware.Ware wa = warelist.Find(x =>
                    {
                        return x.ItemNum.Equals(GoodsNo, StringComparison.CurrentCultureIgnoreCase);
                    });
                    if (wa != null)
                    {
                        double price = Convert.ToDouble(dr["价格"]);
                        if (price >= 100)
                        {
                            wa.CostPrice = Math.Round(price).ToString();
                        }
                        else
                        {
                            wa.CostPrice = Convert.ToDouble(dr["价格"]).ToString("f2");
                        }
                        SyncList.Add(wa);
                    }
                }
                //查询SKU
                List<string> wareidlist = SyncList.Select(x => x.WareId.ToString()).ToList();
                List<JdSdk.Domain.Ware.Sku> skulist = ware.GetSkuListByID(wareidlist, null, false);
                if (skulist == null)
                {
                    CCMessageBox.Show("API错误,请重试\r\n");
                    bar.Loading(false);
                    bar.LoadBar(false);
                    return;
                }
                //开始修改
                bar.Loading(false);
                for (int i = 0; i < SyncList.Count; i++)
                {
                    string Res = string.Empty;
                    try
                    {
                        if (Convert.ToDouble(SyncList[i].CostPrice) < 9.9)
                        {
                            sw.WriteLine(SyncList[i].ItemNum + "      " + "不能低于最低价格9.9");
                            continue;
                        }
                        //计算差价
                        double change = Convert.ToDouble(SyncList[i].JdPrice) - Convert.ToDouble(SyncList[i].CostPrice);
                        var sklist = skulist.FindAll(x =>
                        {
                            return x.WareId == SyncList[i].WareId;
                        });
                        string SkuPrices = string.Empty;
                        string SkuPropers = string.Empty;
                        string SkuStock = string.Empty;
                        if (sklist != null)
                        {
                            sklist.ForEach(z =>
                            {
                                SkuPropers += z.Attributes + "|";
                                double skupr = Convert.ToDouble(z.JdPrice) - change;
                                // double skupr = Convert.ToDouble(SyncList[i].CostPrice);
                                if (skupr >= 100)
                                {
                                    SkuPrices += Math.Round(skupr).ToString() + "|";
                                }
                                else
                                {
                                    SkuPrices += (Convert.ToDouble(z.JdPrice) - change).ToString("f2") + "|";
                                    //   SkuPrices += (Convert.ToDouble(SyncList[i].CostPrice)).ToString("f2") + "|";
                                }
                                SkuStock += z.StockNum + "|";
                            });
                        }
                        Res = ware.UpdatePrice(SyncList[i].WareId.ToString(), SyncList[i].CostPrice, SyncList[i].CostPrice, SkuPrices.TrimEnd('|'), SkuPropers.TrimEnd('|'), SkuStock.TrimEnd('|'));

                    }
                    catch (Exception ex)
                    {
                        Res = ex.Message;
                    }
                    //写入错误记录
                    if (!Res.Equals("success"))
                    {
                        sw.WriteLine(SyncList[i].ItemNum + "      " + Res);
                    }

                    //更新进度条
                    int current = i == SyncList.Count - 1 ? SyncList.Count : i + 1;
                    bar.UpdateBarValue(SyncList.Count, current);
                }
            }
            else //淘宝
            {
                TopSource com = new TopSource(model);

                //查询所有的商品
                List<string> slist = new List<string>();
                foreach (DataRow dr in dt.Rows)
                {
                    slist.Add(dr["款号"].ToString());
                }
                List<Item> itemlist = com.GetItemList(slist, string.Empty);
                //List<Item> itemlist = com.GetGoods(GoodsStatus.All, null, null, null, null, InventStatus.Default);
                itemlist = itemlist.Where(a => !string.IsNullOrEmpty(a.OuterId)).ToList();
                //存在于商店的商品
                List<Item> reallist = new List<Item>();
                List<string> numiidlist = new List<string>();
                //筛选出店铺中正在出售的款号
                foreach (DataRow dr in dt.Rows)
                {
                    string goodsno = dr["款号"].ToString().Trim();
                    Item item = itemlist.Find(
                        delegate (Item i)
                        {
                            return goodsno.ToUpper().Equals(i.OuterId.ToUpper());
                        }
                        );
                    if (item != null)
                    {
                        numiidlist.Add(item.NumIid.ToString());
                        item.PostFee = dr["价格"].ToString(); //POSTFEE字段设置为需要更新的价格
                        reallist.Add(item);
                    }
                }
                //为要修改的对象注入SKU属性
                List<Sku> skslist = com.LoadSkuByNumIID(numiidlist);

                for (int i = reallist.Count - 1; i >= 0; i--)
                {
                    List<Sku> sklist = skslist.FindAll(
                        delegate (Sku sk)
                        {
                            return sk.NumIid == reallist[i].NumIid;
                        }
                        );
                    if (sklist.Count > 0)
                    {
                        reallist[i].Skus = sklist;
                    }
                    else
                    {
                        reallist.RemoveAt(i);
                    }
                }
                bar.Loading(false);


                for (int i = 0; i < reallist.Count; i++)
                {
                    string res = string.Empty;
                    try
                    {
                        if (Convert.ToDouble(reallist[i].PostFee) < 9)
                        {
                            sw.WriteLine(reallist[i].OuterId + "      " + "不能低于最低价格9");
                            continue;
                        }
                        decimal Price_Chage = Convert.ToDecimal(reallist[i].Price) - Convert.ToDecimal(reallist[i].PostFee); //需要修改的差价值

                        string price = reallist[i].PostFee;

                        //C店修改需要的参数
                        string sku_outerid = "";  // //目标的SKU_outid;
                        string sku_propertis = ""; //目标的SKU参数
                        string sku_price = "";  //目标的SKU价格窜
                        string sku_quantities = "";//目标的SKU库存
                        List<Sku> skulist = reallist[i].Skus;
                        if (skulist.Count > 0)
                        {
                            for (int j = 0; j < skulist.Count; j++)
                            {
                                decimal NewSkuPirce = (Convert.ToDecimal(skulist[j].Price) - Price_Chage) > Convert.ToDecimal(price) ? Convert.ToDecimal(price) : Convert.ToDecimal(skulist[j].Price) - Price_Chage;
                                if (j == skulist.Count - 1)
                                {
                                    sku_propertis += skulist[j].Properties;
                                    sku_price += NewSkuPirce.ToString();
                                    sku_outerid += skulist[j].OuterId;
                                    sku_quantities += skulist[j].Quantity;
                                }
                                else
                                {
                                    sku_propertis += skulist[j].Properties + ",";
                                    sku_price += NewSkuPirce.ToString() + ",";
                                    sku_outerid += skulist[j].OuterId + ",";
                                    sku_quantities += skulist[j].Quantity + ",";
                                }
                            }
                            List<UpdateSku> sklist = new List<UpdateSku>();
                            for (int j = 0; j < skulist.Count; j++)
                            {
                                decimal NewSkuPirce = (Convert.ToDecimal(skulist[j].Price) - Price_Chage) > Convert.ToDecimal(price) ? Convert.ToDecimal(price) : Convert.ToDecimal(skulist[j].Price) - Price_Chage;

                                sklist.Add(new UpdateSku() { price = NewSkuPirce.ToString(), properties = skulist[j].Properties });
                            }
                            string[] arry = price.Split('.');
                            if (arry.Length > 1)
                            {
                                for (int j = 0; j < arry.Length; j++)
                                {
                                    if (arry[1].Length > 2)
                                    {
                                        arry[1] = arry[1].Substring(0, 2);
                                    }
                                }
                                price = arry[0] + "." + arry[1];
                            }
                            // int range = Convert.ToInt32(double.Parse(item.Price) - double.Parse(item.PostFee));
                            if (model.shopId!=1022)
                            {
                                //res= com.UpdateItemByModel(reallist[i], new UpdateGoodsSub() { Price = price,SyncPrice=true},new List<NoPayModel>());
                                res = com.Update_ItemPrice(reallist[i].NumIid, price, sku_propertis, sku_price, sku_quantities, sku_outerid);
                            }
                            else  
                            {
                                res = com.TmallPriceUpdate(reallist[i].NumIid, price, JsonConvert.SerializeObject(sklist));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        res = ex.Message;
                    }
                    //写入错误记录
                    if (!res.Equals("success"))
                    {
                        sw.WriteLine(reallist[i].OuterId + "      " + res);
                    }

                    //更新进度条
                    int current = i == reallist.Count - 1 ? reallist.Count : i + 1;
                    bar.UpdateBarValue(reallist.Count, current);
                }
            }
            sw.Close();
            bar.Loading(false);
            bar.LoadBar(false);
            mgupdatepricetable.Clear();
        }


        //异步委托
        public delegate void AsyncEventHandler(DataTable dt, ShopModel model);

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion




    }
    public class UpdateSku
    {
        public string price { get; set; }
        public string properties { get; set; }
    }


}
