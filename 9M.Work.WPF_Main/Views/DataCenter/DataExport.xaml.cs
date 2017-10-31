using _9M.Work.DbObject;
using _9M.Work.ErpApi;
using _9M.Work.JosApi;
using _9M.Work.Model;
using _9M.Work.TopApi;
using _9M.Work.Utility;
using _9M.Work.WPF_Common;
using _9M.Work.WPF_Common.WpfBind;
using _9M.Work.WPF_Main.FrameWork;
using _9M.Work.YouZan;
using _9Mg.Work.TopApi;
using JdSdk.Domain.Ware;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Text.RegularExpressions;
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
using Top.Api.Domain;

namespace _9M.Work.WPF_Main.Views.DataCenter
{
    /// <summary>
    /// DataExport.xaml 的交互逻辑
    /// </summary>
    public partial class DataExport : UserControl, INotifyPropertyChanged
    {
        private bool CopyUrl = true;
        private BaseDAL dal = new BaseDAL();
        private List<ShopModel> shoplist = null;
        private int ShopCurrent = 0;
        GoodsManager goodsmanager = new GoodsManager();
        //京东ID生成款号
        private List<string> GoodsNoList = new List<string>();
        //异步委托
        public delegate Dictionary<string, string> AsyncEventHandler(List<string> list);
        public delegate void JieXiEventHandler(List<string> list);
        public delegate DataTable ExportAsyncEventHandler(ShopModel model, GoodsStatus status, string title);
        public delegate void ExportClassPrice(List<string> list);
        public delegate void ExportWanlanUrl(List<string> list);
        public delegate Dictionary<string, object> Handler(object[] parm);
        public DataExport()
        {
            InitializeComponent();
            this.DataContext = this;
            BindShop();
        }


        public void BindShop()
        {
            shoplist = dal.GetList<ShopModel>(x => x.isHaveApi == true);
            shoplist.Insert(0, new ShopModel() { shopId = 0, shopName = "请选择" });
            ComboBoxBind.BindComboBox(Com_Shop, shoplist, "shopName", "shopId");
            shoplist.Add(new ShopModel() { shopId = 1028, shopName = "有赞微店", appKey = "", invokeUrl = "", isHaveApi = true, sessionKey = "", appSecret = "" });
            Com_Shop.SelectedIndex = 0;

            ComboBoxBind.BindComboBox(Com_MaiDianShop, shoplist.Where(x => x.invokeUrl == "C" || x.invokeUrl == "TMALL" || x.shopId == 0), "shopName", "shopId");
            Com_MaiDianShop.SelectedIndex = 0;
        }
        #region 事件处理

        Dictionary<string, string> maidiandic = null;
        List<string> HuiZhongExcel = null;
        List<string> maidianflag = null;
        private void Button_Click(object sender, RoutedEventArgs e)
        {

            string Tag = (sender as Button).Tag.ToString();
            switch (Tag)
            {
                case "1": //导出商品数据
                    if (Com_Shop.Text.Equals("分销"))
                    {
                        CCMessageBox.Show("分销不支持该功能");
                        return;
                    }
                    //OnlineStoreAuthorization model = ComboBoxBind.GetAuthorization(Com_Shop);

                    string Title = tb_TitleKeword.Text;
                    //得到商品的状态
                    GoodsStatus status = ComboBoxBind.ReloadGoodsStatus(GoodsStatus_Com);
                    ShopCurrent = Convert.ToInt32(Com_Shop.SelectedValue);
                    ShopModel shop = shoplist.Where(x => x.shopId == ShopCurrent).FirstOrDefault();
                    ExportAsyncEventHandler ass = new ExportAsyncEventHandler(this.ExportData);
                    //异步调用开始，没有回调函数和AsyncState,都为null
                    IAsyncResult ias = ass.BeginInvoke(shop, status, Title, ExportCallBack, null);
                    break;
                case "2":  //京东ID生成导入
                    Import();
                    break;
                case "3":  //京东ID生成确定
                    if (GoodsNoList.Count == 0)
                    {
                        CCMessageBox.Show("请正确导入文件");
                        return;
                    }
                    CopyUrl = false;
                    //实例委托
                    AsyncEventHandler asy = new AsyncEventHandler(this.GetJdSkuId);
                    //异步调用开始，没有回调函数和AsyncState,都为null
                    IAsyncResult ia = asy.BeginInvoke(GoodsNoList, CodeCallBack, null);
                    break;
                case "4":
                    Import();
                    break;
                case "5":
                    if (GoodsNoList.Count == 0)
                    {
                        CCMessageBox.Show("请正确导入文件");
                        return;
                    }
                    //实例委托
                    ExportClassPrice asss = new ExportClassPrice(ExportPrice);
                    //异步调用开始，没有回调函数和AsyncState,都为null
                    IAsyncResult iass = asss.BeginInvoke(GoodsNoList, null, null);
                    break;
                case "6":
                    CopyUrl = true;
                    if (GoodsNoList.Count == 0)
                    {
                        CCMessageBox.Show("请正确导入文件");
                        return;
                    }
                    //实例委托
                    AsyncEventHandler asysss = new AsyncEventHandler(this.GetJdSkuId);
                    //异步调用开始，没有回调函数和AsyncState,都为null
                    IAsyncResult iasss = asysss.BeginInvoke(GoodsNoList, CodeCallBack, null);
                    break;
                case "7":
                    Import();
                    break;
                case "8":
                    //实例委托
                    ExportWanlanUrl ewu = new ExportWanlanUrl(ExportWanLanUrl);
                    //异步调用开始，没有回调函数和AsyncState,都为null
                    IAsyncResult ir = ewu.BeginInvoke(GoodsNoList, null, null);
                    break;
                case "9":
                    Import();
                    break;
                case "10":
                    //实例委托
                    ExportWanlanUrl ewus = new ExportWanlanUrl(ExportCheckGoods);
                    //异步调用开始，没有回调函数和AsyncState,都为null
                    IAsyncResult irs = ewus.BeginInvoke(GoodsNoList, null, null);
                    break;
                case "11":
                    var list = dal.QueryList<CheckSkuStockModel>("select * from T_CheckSku", new object[] { });
                    // DataTable dt = new DataTable();
                    var table = list.ConvertToDataTable();
                    bool IsOk = true;
                    try
                    {
                        ExcelNpoi.TableToExcelForXLSX(table, Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + @"//SKU检查.xlsx");
                    }
                    catch (Exception ex)
                    {
                        IsOk = false;
                    }
                    CCMessageBox.Show(IsOk ? "Excel生成在桌面" : "生成失败");
                    break;
                case "12": //数据汇总导入
                    OpenFileDialog op = new OpenFileDialog();
                    //过滤器
                    op.Filter = "XLS|*.xls|XLSX|*.xlsx|TXT|*.txt";
                    op.Multiselect = true;
                    if (op.ShowDialog() == true)
                    {
                        HuiZhongExcel = new List<string>();
                        HuiZhongExcel.AddRange(op.FileNames);
                        if (HuiZhongExcel.Count > 0)
                        {
                            CCMessageBox.Show(string.Format("导入成功：销售表{0}张,退货表{1}张", HuiZhongExcel.Where(x => x.Contains("销售")).Count(), HuiZhongExcel.Where(x => x.Contains("退货")).Count()));
                        }
                    }

                    break;
                case "13": //数据汇总导出
                    if (HuiZhongExcel != null && HuiZhongExcel.Count > 0)
                    {
                        JieXiEventHandler abs = new JieXiEventHandler(this.JieXi);
                        //异步调用开始，没有回调函数和AsyncState,都为null
                        IAsyncResult iabs = abs.BeginInvoke(HuiZhongExcel, HuiZhongCallBack, null);

                    }
                    break;
                case "14": //文件夹名导出
                    System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
                    if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        string Path = fbd.SelectedPath;
                        DirectoryInfo thisOne = new DirectoryInfo(Path);
                        DirectoryInfo[] infolist = thisOne.GetDirectories();
                        using (StreamWriter sw = File.CreateText(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + @"\\款号.txt"))
                        {
                            foreach (var item in infolist)
                            {
                                if (item.Name.Length > 7 && item.Name.Length < 10)
                                {
                                    sw.WriteLine(item.Name);
                                }
                            }

                            sw.Close();
                        }


                    }
                    break;
                case "15":
                    string Flag = tb_Flag.Text;
                    if (!string.IsNullOrEmpty(Flag))
                    {
                        string Msg = "成功";
                        try
                        {
                            #region wl 2017-05-26 替换下列方法

                            //                            DataTable dt = goodsmanager.TableForSql(string.Format(@"select b.GoodsNO as 款号, CONVERT(int, SUM(a.Stock)) as  '库存'  from G_Stock_Spec a 
                            //                            join G_Goods_GoodsList b on a.GoodsID = b.GoodsID
                            //                            where b.GoodsNO like '{0}%'
                            //                            group by b.GoodsNO", Flag));

                            #endregion


                            DataTable dt = WdgjSource.getErpStockByGoodsno(Flag);

                            ExcelNpoi.TableToExcelForXLS(dt, Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + @"//" + Flag + ".xls");
                        }
                        catch (Exception ex)
                        {
                            Msg = ex.Message;
                        }
                        CCMessageBox.Show(Msg);
                    }
                    break;
                case "16": //卖点检查
                    maidianflag = GlobalUtil.GetImport();
                    break;
                case "17": //卖点检查
                    if (Com_MaiDianShop.SelectedIndex == 0)
                    {
                        CCMessageBox.Show("请选择店铺");
                        return;
                    }
                    if (maidianflag == null || maidianflag.Count == 0)
                    {
                        CCMessageBox.Show("请导入卖点关键词");
                        return;
                    }
                    if (Chk_MaidianUpdate.IsChecked == true)
                    {
                        if (maidiandic == null || maidiandic.Count == 0)
                        {
                            CCMessageBox.Show("请导入要修改的品牌卖点");
                            return;
                        }
                    }
                    ShopModel pointshop = Com_MaiDianShop.SelectedItem as ShopModel;

                    Handler sellpointcheck = new Handler(CheckPoint);
                    sellpointcheck.BeginInvoke(new object[] { pointshop, maidianflag, Convert.ToBoolean(Chk_MaidianUpdate.IsChecked) }, PointCallBack, null);
                    break;
                case "18": //品牌卖点修改
                    OpenFileDialog ops = new OpenFileDialog();
                    //过滤器
                    ops.Filter = "XLS|*.xls|XLSX|*.xlsx|TXT|*.txt";
                    if (ops.ShowDialog() == true)
                    {
                        maidiandic = new Dictionary<string, string>();
                        DataTable dt = ExcelNpoi.ExcelToDataTable("Sheet1", true, ops.FileName);
                        foreach (DataRow dr in dt.Rows)
                        {
                            try
                            {
                                maidiandic.Add(dr[0].ToString(), dr[1].ToString());
                            }
                            catch (Exception ex)
                            {
                                CCMessageBox.Show("品牌:" + dr[0].ToString() + "   重复");
                            }
                        }
                    }
                    break;
            }
        }

        private void PointCallBack(IAsyncResult ar)
        {
            Handler handler = (Handler)((AsyncResult)ar).AsyncDelegate;
            var dic = handler.EndInvoke(ar);
            string Msg = dic.First().Key;
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (string.IsNullOrEmpty(Msg.ToString()))
                {
                    CCMessageBox.Show("成功");
                }
                else
                {
                    CCMessageBox.Show("失败: " + Msg);
                }
            }));
        }

        private Dictionary<string, object> CheckPoint(object[] parm)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            DataTable dt = new DataTable();
            bool Update = Convert.ToBoolean(parm[2]);
            dt.Columns.Add("款号", typeof(string));
            dt.Columns.Add("字符", typeof(string));
            dt.Columns.Add("卖点", typeof(string));
            List<string> flaglist = parm[1] as List<string>;

            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            StreamWriter sw = System.IO.File.CreateText(desktopPath + @"\\卖点检查.txt");
            try
            {
                ShopModel shop = parm[0] as ShopModel;
                bar.LoadBar(true);
                bar.Loading(true);
                TopSource tops = new TopSource(shop);
                var itemlist = tops.OnSaleList(null, null, null, null);
                var invlist = tops.InventoryList(null, null, null, null, null);
                itemlist.AddRange(invlist);
                var GoodsNoList = itemlist.Select(x => x.OuterId);
                int Total = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(GoodsNoList.Count()) / 40));
                bar.Loading(false);
                for (int i = 1; i <= Total; i++)
                {
                    var ls = GoodsNoList.Skip(i - 1).Take(40).ToList();
                    List<Item> itlist = tops.GetItemList(ls, "sell_point").Where(x => !string.IsNullOrEmpty(x.OuterId) && !string.IsNullOrEmpty(x.SellPoint)).ToList();
                    //itlist =   itlist.Where(x => flaglist.Contains(x.SellPoint)).ToList();
                    List<Item> relist = new List<Item>();
                    itlist.ForEach(x =>
                    {
                        bool b = false;
                        string Keword = string.Empty;
                        foreach (var it in flaglist)
                        {
                            if (x.SellPoint.Contains(it))
                            {
                                Keword += it + ",";
                                b = true;
                            }
                        }
                        if (b == true)
                        {
                            DataRow dr = dt.NewRow();
                            dr["款号"] = x.OuterId;
                            dr["字符"] = Keword.TrimEnd(',');
                            dr["卖点"] = x.SellPoint;
                            dt.Rows.Add(dr);
                            if (Update)//如果需要修改
                            {
                                string Brand = GoodsHelper.BrandEn(x.OuterId);
                                var dics = maidiandic.Where(z => z.Key.Equals(Brand)).FirstOrDefault();
                                if (dics.Value != null)
                                {
                                    string res = tops.UpdateItemByModel(x, new UpdateGoodsSub() { SellPointStr = dics.Value, SyncSellPoint = true }, new List<NoPayModel>());
                                    if (!res.Equals("success"))
                                    {
                                        sw.WriteLine(x.OuterId + ":" + res);
                                    }
                                }
                                else
                                {
                                    sw.WriteLine(Brand + "  需要新建卖点");
                                }
                            }
                        }
                    });

                    bar.UpdateBarValue(Total, i);
                }
                dic.Add("", null);
                ExcelNpoi.TableToExcelForXLS(dt, desktopPath + @"\\卖点.xls");
            }
            catch (Exception ex)
            {
                sw.WriteLine(ex.Message);
                dic.Add(ex.Message, null);
            }
            finally
            {
                sw.Close();
                bar.LoadBar(false);
            }
            return dic;
        }

        private void HuiZhongCallBack(IAsyncResult ar)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (string.IsNullOrEmpty(Msg))
                {
                    CCMessageBox.Show("成功");
                }
                else
                {
                    CCMessageBox.Show("失败: " + Msg);
                }
            }));
        }


        string Msg = string.Empty;
        /// <summary>
        /// 汇总EXCEL
        /// </summary>
        /// <param name="HuiZhongExcel"></param>
        public void JieXi(List<string> HuiZhongExcel)
        {
            try
            {
                //打开加载
                bar.LoadBar(true);
                //打开加载中的字符
                bar.Loading(true);
                //桌面路径
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                string Dir = string.Format("汇总{0}", System.Guid.NewGuid().ToString().Replace("-", ""));
                if (!Directory.Exists(desktopPath + @"\" + Dir))
                {
                    Directory.CreateDirectory(desktopPath + @"\" + Dir);
                }
                var XiaoShouList = HuiZhongExcel.Where(x => x.Contains("销售")).ToList();
                bar.Loading(false);
                int Cut = 0;
                for (int i = 0; i < XiaoShouList.Count; i++)
                {
                    string Url = XiaoShouList[i];
                    DataTable dttt = ExcelNpoi.ExcelToDataTable("Sheet1", true, Url);
                    List<StaModel> stalist = new List<StaModel>();
                    foreach (DataRow dr in dttt.Rows)
                    {
                        try
                        {
                            string Text = dr["品名"].ToString();
                            var Flag = Text.Split(' ').Where(x => !string.IsNullOrEmpty(x)).ToList();
                            string Brand = string.Empty;
                            if (Flag.Count > 1)
                            {
                                if (Flag.Count == 2)
                                {

                                    Brand = Flag[0];
                                }
                                else
                                {
                                    //删除最后一位
                                    Flag.RemoveAt(Flag.Count - 1);
                                    Brand = string.Join(" ", Flag.ToArray());
                                }

                                StaModel st = new StaModel()
                                {
                                    Shop = dr["店铺"].ToString(),
                                    Brand = Brand,
                                    Count = Convert.ToInt32(dr["数量"]),
                                    Total = Convert.ToDecimal(dr["合计"])
                                };
                                stalist.Add(st);
                            }
                        }
                        catch
                        {
                            continue;
                        }
                    }
                    //去掉9魅优品与不需要的数据
                    stalist = stalist.Where(x => !x.Shop.Equals("9魅优品") && x.Count > 0 && x.Total > 0).ToList();
                    //分组求和
                    var ls = stalist.GroupBy(a => a.Brand).Select(g => (new StaModel { Brand = g.Key, Count = g.Sum(item => item.Count), Total = g.Sum(item => item.Total) }));
                    //导出
                    DataTable Exportdt = new DataTable();
                    Exportdt.Columns.Add("品牌", typeof(string));
                    Exportdt.Columns.Add("数量", typeof(int));
                    Exportdt.Columns.Add("金额", typeof(decimal));
                    foreach (var it in ls)
                    {
                        DataRow dr = Exportdt.NewRow();
                        dr["品牌"] = it.Brand;
                        dr["数量"] = it.Count;
                        dr["金额"] = it.Total;
                        Exportdt.Rows.Add(dr);
                    }

                    string ExportUrl = string.Format(desktopPath + @"\\{0}\\{1}.xls", Dir, System.IO.Path.GetFileNameWithoutExtension(Url));
                    ExcelNpoi.TableToExcelForXLS(Exportdt, ExportUrl);
                    Cut++;
                    bar.UpdateBarValue(HuiZhongExcel.Count, Cut);
                }

                var TuiHuoList = HuiZhongExcel.Where(x => x.Contains("退货")).ToList();
                for (int i = 0; i < TuiHuoList.Count; i++)
                {
                    string Url = TuiHuoList[i];
                    DataTable dttt = ExcelNpoi.ExcelToDataTable("Sheet1", true, Url);
                    List<StaModel> stalist = new List<StaModel>();
                    foreach (DataRow dr in dttt.Rows)
                    {
                        try
                        {
                            string Text = dr["货品名称"].ToString();
                            var Flag = Text.Split(' ').Where(x => !string.IsNullOrEmpty(x)).ToList();
                            string Brand = string.Empty;

                            if (Flag.Count > 1)
                            {
                                if (Flag.Count == 2)
                                {

                                    Brand = Flag[0];
                                }
                                else
                                {

                                    //删除最后一位
                                    Flag.RemoveAt(Flag.Count - 1);
                                    Brand = string.Join(" ", Flag.ToArray());
                                }
                                StaModel st = new StaModel()
                                {

                                    Brand = Brand,
                                    Count = Convert.ToInt32(dr["数量"]),

                                };
                                stalist.Add(st);
                            }
                        }
                        catch
                        {
                            continue;
                        }
                    }
                    //去掉9魅优品与不需要的数据
                    stalist = stalist.Where(x => x.Count > 0).ToList();
                    //分组求和
                    var ls = stalist.GroupBy(a => a.Brand).Select(g => (new StaModel { Brand = g.Key, Count = g.Sum(item => item.Count), Total = g.Sum(item => item.Total) }));
                    //导出
                    DataTable Exportdt = new DataTable();
                    Exportdt.Columns.Add("品牌", typeof(string));
                    Exportdt.Columns.Add("数量", typeof(int));

                    foreach (var it in ls)
                    {
                        DataRow dr = Exportdt.NewRow();
                        dr["品牌"] = it.Brand;
                        dr["数量"] = it.Count;

                        Exportdt.Rows.Add(dr);
                    }

                    string ExportUrl = string.Format(desktopPath + @"\\{0}\\{1}.xls", Dir, System.IO.Path.GetFileNameWithoutExtension(Url));
                    ExcelNpoi.TableToExcelForXLS(Exportdt, ExportUrl);
                    Cut++;
                    bar.UpdateBarValue(HuiZhongExcel.Count, Cut);
                }
                bar.LoadBar(false);
                HuiZhongExcel = null;
                Msg = string.Empty;
            }
            catch (Exception ex)
            {
                bar.LoadBar(false);
                Msg = ex.Message;
            }
        }

        private void ExportCheckGoods(List<string> list)
        {
            bar.LoadBar(true);
            bar.Loading(true);
            DataTable dts = new DataTable();
            dts.Columns.Add("款号", typeof(string));
            dts.Columns.Add("区间", typeof(string));
            List<Item> itemlist = new TopSource().GetItemList(list, "sku");
            itemlist.ForEach(x =>
            {
                List<Top.Api.Domain.Sku> skulist = x.Skus;
                int Count = skulist.GroupBy(y => y.Price).Count();
                DataRow dr = dts.NewRow();
                dr["款号"] = x.OuterId;
                dr["区间"] = Count > 1 ? "是" : "否";
                dts.Rows.Add(dr);
            });
            ExcelNpoi.TableToExcel(dts, @"c:\区间.xls");
            CCMessageBox.Show("表格己生成在C盘根目录");
            bar.LoadBar(false);
            bar.Loading(false);
        }





        #endregion

        #region 公共方法
        private void ExportWanLanUrl(List<string> list)
        {
            bar.LoadBar(true);
            bar.Loading(true);
            DataTable WanLanUrltable = new DataTable();
            WanLanUrltable.Columns.Add("款号", typeof(string));
            WanLanUrltable.Columns.Add("链接", typeof(string));
            //淘宝现价
            List<Item> itemlist = new TopSource().GetItemList(list, string.Empty);
            itemlist.ForEach(x =>
            {
                DataRow dr = WanLanUrltable.NewRow();
                dr["款号"] = x.OuterId + "-WJ";
                dr["链接"] = "http://h5.m.taobao.com/awp/core/detail.htm?id=" + x.NumIid;
                WanLanUrltable.Rows.Add(dr);
            });
            //获取当前用户桌面的路径
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            if (WanLanUrltable.Rows.Count > 0)
            {
                ExcelNpoi.TableToExcel(WanLanUrltable, desktopPath + string.Format(@"\\{0}无线链接.xls", DateTime.Now.ToShortDateString()));
                CCMessageBox.Show("表格己生成在桌面");
            }
            else
            {
                CCMessageBox.Show("无数据导出");
            }

            bar.LoadBar(false);
            bar.Loading(false);
        }

        /// <summary>
        /// 导出秒杀价格
        /// </summary>
        /// <param name="list"></param>
        private void ExportPrice(List<string> list)
        {
            bar.LoadBar(true);
            bar.Loading(true);
            DataTable pricetable = new DataTable();
            pricetable.Columns.Add("款号", typeof(string));
            pricetable.Columns.Add("秒杀价", typeof(decimal));
            GoodsManager goods = new GoodsManager();
            string Res = string.Empty;
            list.ForEach(x =>
            {
                Res += x + ",";
            });
            //读取分类
            List<ApiTopicGoodsModel> apilist = goods.GetGoodsByHasStock(Res.TrimEnd(','));
            //分类价格表
            List<ClassPriceModel> classlist = dal.QueryList<ClassPriceModel>(string.Format(@"select * from T_Class_Price"), new object[] { });
            //淘宝现价
            List<Item> itemlist = new TopSource().GetItemList(list, string.Empty);
            //得到品牌列表
            List<BrandModel> brandlist = dal.GetAll<BrandModel>();
            apilist.ForEach(x =>
            {
                //根据品牌得到品牌价格表
                string BrandEn = Regex.Replace(x.GoodsNo, "[0-9]", "", RegexOptions.IgnoreCase); ;
                BrandModel mm = brandlist.Where(z => z.BrandEN.Equals(BrandEn, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                if (mm != null)
                {
                    //得到一二三线的秒杀价格
                    decimal price = 0.00M;
                    int Level = mm.Level;
                    string Sql = string.Format(@"select *  from T_Class_Price where ClassName='{0}'", x.ClassName);
                    ClassPriceModel classmodel = dal.QuerySingle<ClassPriceModel>(Sql, new object[] { });
                    if (classmodel != null)
                    {
                        switch (Level)
                        {
                            case 1:
                                price = classmodel.Price1;
                                break;
                            case 2:
                                price = classmodel.Price2;
                                break;
                            case 3:
                                price = classmodel.Price3;
                                break;
                        }
                        //得到淘宝价格
                        Item it = itemlist.Where(z => z.OuterId.Equals(x.GoodsNo, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                        decimal tbprice = it != null ? decimal.Parse(it.Price) : 0.00M;
                        DataRow dr = pricetable.NewRow();
                        dr["款号"] = x.GoodsNo;

                        decimal realprice = 0.00M;
                        if (price != 0.00M && tbprice == 0.00M)
                        {
                            realprice = price;
                        }
                        else if (price == 0.00M && tbprice != 0.00M)
                        {
                            realprice = tbprice;
                        }
                        else if (price != 0.00M && tbprice != 0.00M)
                        {
                            realprice = price > tbprice ? tbprice : price;
                        }
                        if (realprice > 0.00M)
                        {
                            dr["秒杀价"] = realprice;
                            pricetable.Rows.Add(dr);
                        }
                    }
                }
            });
            //获取当前用户桌面的路径
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            ExcelNpoi.TableToExcel(pricetable, desktopPath + string.Format(@"\\{0}秒杀.xls", DateTime.Now.ToShortDateString()));
            CCMessageBox.Show("表格己生成在桌面");

            bar.LoadBar(false);
            bar.Loading(false);
        }
        public void Import()
        {
            GoodsNoList.Clear();
            OpenFileDialog op = new OpenFileDialog();
            //过滤器
            op.Filter = "XLS|*.xls|XLSX|*.xlsx|TXT|*.txt";
            if (op.ShowDialog() == true) //选择完成之后
            {
                if (System.IO.Path.GetExtension(op.FileName.ToLower()).Equals(".txt"))
                {
                    GoodsNoList = FileHelper.RealFileLine(op.FileName);
                }
                else
                {
                    DataTable dt = ExcelNpoi.ExcelToDataTable("sheet1", true, op.FileName);
                    if (!dt.Columns[0].ColumnName.Equals("款号"))
                    {
                        CCMessageBox.Show("列头为款号");
                        return;
                    }
                    foreach (DataRow dr in dt.Rows)
                    {
                        GoodsNoList.Add(dr["款号"].ToString().Trim());
                    }
                }
                GoodsNoList = GoodsNoList.Distinct().Where(x => !string.IsNullOrEmpty(x)).ToList();
                if (GoodsNoList.Count > 0)
                {
                    CCMessageBox.Show("导入商品件数:" + GoodsNoList.Count);
                }
            }
        }

        private DataTable ExportData(ShopModel model, GoodsStatus status, string title)
        {
            //打开加载
            bar.LoadBar(true);
            //打开加载中的字符
            bar.Loading(true);

            DataTable dt = new DataTable();

            if (!model.shopId.Equals(1028))
            {
                dt.Columns.Add("数字ID", typeof(long));
                dt.Columns.Add("款号", typeof(string));
                dt.Columns.Add("价格", typeof(decimal));
                dt.Columns.Add("标题", typeof(string));
                dt.Columns.Add("数量", typeof(long));
                dt.Columns.Add("运费模版", typeof(long));
                //京东
                if (model.shopId == 1023)
                {
                    WareManager ware = new WareManager(model.appKey, model.appSecret, model.sessionKey);
                    List<Ware> warelist = ware.GetWares(status, null, title, null);
                    warelist.ForEach(x =>
                    {
                        DataRow dr = dt.NewRow();
                        dr["数字ID"] = 0;
                        dr["款号"] = x.ItemNum;
                        dr["价格"] = Convert.ToDecimal(x.JdPrice);
                        dr["标题"] = x.Title;
                        dr["数量"] = x.StockNum;
                        dt.Rows.Add(dr);
                    });
                }
                else
                {
                    List<Item> itemlist = null;
                    TopSource com = new TopSource(model);
                    if (status == GoodsStatus.Onsell)
                    {
                        itemlist = com.OnSaleList(title, null, null, "num,postage_id");
                    }
                    else if (status == GoodsStatus.InStock)
                    {
                        itemlist = com.InventoryList(title, string.Empty, null, null, "num,postage_id");
                    }
                    else
                    {
                        itemlist = new List<Item>();
                        itemlist.AddRange(com.OnSaleList(title, null, null, "num,postage_id"));
                        itemlist.AddRange(com.InventoryList(title, string.Empty, null, null, "num,postage_id"));
                    }

                    itemlist.ForEach(x =>
                    {
                        DataRow dr = dt.NewRow();
                        dr["数字ID"] = x.NumIid;
                        dr["款号"] = x.OuterId;
                        dr["价格"] = Convert.ToDecimal(x.Price);
                        dr["标题"] = x.Title;
                        dr["数量"] = x.Num;
                        dr["运费模版"] = x.PostageId;
                        dt.Rows.Add(dr);
                    });
                }
            }
            else
            {
                List<OnSaleItems> items = YZSDKHelp.GetOnSaleItems();
                dt.Columns.Add("商品编号", typeof(string));
                items.ForEach(a =>
                {

                    DataRow dr = dt.NewRow();
                    dr["商品编号"] = a.outer_id;
                    dt.Rows.Add(dr);
                });
            }

            //关闭加载加载中的字符
            bar.Loading(false);
            //关闭加载
            bar.LoadBar(false);
            return dt;
        }

        /// <summary>
        /// 返回SKUID
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private Dictionary<string, string> GetJdSkuId(List<string> list)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            bar.LoadBar(true);
            bar.Loading(true);
            try
            {
                OnlineStoreAuthorization model = CommonLogin.CommonUser.JDShop;
                WareManager ware = new WareManager(model.AppKey, model.AppSecret, model.SessionKey);
                dic = ware.GetGoodsNoSkuID(list);
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    CCMessageBox.Show("API错误,请重试\r\n" + ex.Message);
                }));

            }
            finally
            {
                bar.LoadBar(false);
                bar.Loading(false);
            }
            return dic;
        }

        /// <summary>
        /// 生成回调
        /// </summary>
        /// <param name="ar"></param>
        /// 
        private void ExportCallBack(IAsyncResult ar)
        {
            ExportAsyncEventHandler handler = (ExportAsyncEventHandler)((AsyncResult)ar).AsyncDelegate;
            DataTable dt = handler.EndInvoke(ar);
            if (dt.Rows.Count == 0)
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    CCMessageBox.Show("没有正确的数据可以导出");
                }));
            }
            else
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    SaveFileDialog sf = new SaveFileDialog();
                    sf.Filter = "XLS|*.xls|XLSX|*.xlsx";
                    sf.FileName = "【" + Com_Shop.Text + "】--" + "【" + GoodsStatus_Com.Text + "】";
                    if (sf.ShowDialog() == true)
                    {
                        ExcelNpoi.TableToExcel(dt, sf.FileName);
                        //  ExcelNpoi.DataTableToExcel(dt, "sheet1", true, sf.FileName);
                    }
                }));
            }

        }
        public void CodeCallBack(IAsyncResult ar)
        {
            DataTable exportdt = new DataTable();
            exportdt.Columns.Add("款号", typeof(string));
            exportdt.Columns.Add("链接", typeof(string));
            AsyncEventHandler handler = (AsyncEventHandler)((AsyncResult)ar).AsyncDelegate;
            Dictionary<string, string> dic = handler.EndInvoke(ar);

            //Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            //{
            //    InitLoad.Text = "";
            //}));
            string Msg = string.Empty;
            string YES = string.Empty;
            string ERROR = string.Empty;
            if (dic.Count > 0)
            {
                GoodsNoList.ForEach(x =>
                {
                    DataRow dr = exportdt.NewRow();
                    dr["款号"] = x;
                    bool b = false;
                    foreach (var item in dic)
                    {
                        if (x.Equals(item.Key, StringComparison.CurrentCultureIgnoreCase))
                        {
                            b = true;
                            dr["链接"] = string.Format(@"http://item.jd.com/{0}.html", item.Value);
                            YES += item.Value + ",";
                            break;
                        }
                    }
                    if (b == false)
                    {
                        dr["链接"] = "";
                        ERROR += x + "\r\n";
                    }
                    exportdt.Rows.Add(dr);
                });

                if (!string.IsNullOrEmpty(ERROR))
                {
                    ERROR += "没有找到的款号,点击确定复制";
                    Msg = ERROR;
                }
                else
                {
                    Msg = "己成功点击确定复制";
                }
            }
            else
            {
                CCMessageBox.Show("没有获取到数据");
                Msg = "没有获取到数据";
                return;
            }
            if (CopyUrl == true)
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    //获取当前用户桌面的路径
                    StreamWriter sw = new StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\京东链接" + System.Guid.NewGuid() + ".txt");
                    //写入EXCEL
                    // ExcelNpoi.TableToExcel(exportdt, desktopPath + "\\京东链接" + System.Guid.NewGuid() + ".xls");
                    //写入TXT
                    foreach (DataRow dr in exportdt.Rows)
                    {
                        string goodsno = dr["款号"].ToString();
                        string link = dr["链接"].ToString();
                        if (!string.IsNullOrEmpty(link))
                        {
                            sw.WriteLine(goodsno);
                            sw.WriteLine(link);
                        }
                    }
                    sw.Close();
                    CCMessageBox.Show("己生成在桌面");
                }));
            }
            else
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (CCMessageBox.Show(Msg, "提示", CCMessageBoxButton.YesNo) == CCMessageBoxResult.Yes)
                    {
                        Clipboard.SetDataObject(YES.TrimEnd(','));
                    }
                    else
                    {
                        //保存文件
                        try
                        {
                            //获取当前用户桌面的路径
                            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                            //保存路径
                            string saveCodePath = desktopPath + string.Format("\\{0}\\", "JdID");
                            //如果没有就创建文件
                            if (!Directory.Exists(saveCodePath))
                            {
                                Directory.CreateDirectory(saveCodePath);
                            }
                            //拼接完成之后写入到文件中
                            System.IO.File.WriteAllText(string.Format(saveCodePath + "{0}.txt", "JdID"), YES.TrimEnd(','));

                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }));
            }
        }
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

        private void Chk_MaidianUpdate_Checked(object sender, RoutedEventArgs e)
        {

            Btn_UpMaidianImp.Visibility = (sender as CheckBox).IsChecked == true ? Visibility.Visible : Visibility.Collapsed;

        }
    }

    public class StaModel
    {
        public string Shop { get; set; }
        public string Brand { get; set; }
        public int Count { get; set; }
        public decimal Total { get; set; }
    }
}
