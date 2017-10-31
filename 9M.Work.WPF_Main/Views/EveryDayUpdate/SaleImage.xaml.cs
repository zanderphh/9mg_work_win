using _9M.Work.DbObject;
using _9M.Work.JosApi;
using _9M.Work.Model;
using _9M.Work.TopApi;
using _9M.Work.Utility;
using _9M.Work.WPF_Common;
using _9M.Work.WPF_Common.WpfBind;
using _9M.Work.WPF_Main.ControlTemplate.Activity;
using _9M.Work.WPF_Main.FrameWork;
using JdSdk.Domain.Ware;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Top.Api.Domain;

namespace _9M.Work.WPF_Main.Views.EveryDayUpdate
{
    /// <summary>
    /// SaleImage.xaml 的交互逻辑
    /// </summary>
    public partial class SaleImage : UserControl
    {
        private DataTable dt = null;
        private List<Item> itemList = null;
        private List<CreateImageModel> list = null;
        private string Youhui = string.Empty;
        private DispatcherTimer timer;
        private string saveCodePath = string.Empty;
        string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
        private List<ShopModel> Shoplist = null;
        //  private TopSource com = new TopSource(); //默认全局授权C店 用于获取橱窗图等
        private BaseDAL dal = new BaseDAL();
        public SaleImage()
        {
            InitializeComponent();
            timer = new DispatcherTimer();
            timer.Tick += timer_Tick;
            timer.Interval = new TimeSpan(0, 0, 0, 0, 2000);
            // Shoplist = new BaseDAL().GetAll<ShopModel>();
            BindShop();

        }

        public void BindShop()
        {
            Shoplist = dal.GetList<ShopModel>(x => x.isHaveApi == true);
            Shoplist.Insert(0, new ShopModel() { shopId = 0, shopName = "请选择" });
            ComboBoxBind.BindComboBox(Com_Shop, Shoplist, "shopName", "shopId");
            Com_Shop.SelectedIndex = 0;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            //Current++;
            //if (Current == itemList.Count)
            //{
            //    Current = -1;
            //    timer.Stop();
            //    bar.LoadBar(false);
            //    CCMessageBox.Show("图片已生成至桌面！");
            //}
            //else
            //{
            //    Item it = itemList[Current];
            //    CreateImageModel cim = list.Where(x => x.GoodsNo.Equals(it.OuterId, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            //    lab_price.Text = cim.Price;
            //    lab_youhui.Content = tb_youhui.Text;
            //    img_box.Source = new BitmapImage(new Uri(it.ItemImgs[0].Url, UriKind.RelativeOrAbsolute));
            //    //更新进度条
            //    int current = Current == itemList.Count - 1 ? itemList.Count : Current + 1;
            //    bar.UpdateBarValue(itemList.Count, current);
            //    if (Current > 0)
            //    {
            //        ImageHelper.SaveToImage(imageTempate, saveCodePath + itemList[Current - 1].OuterId + ".png");
            //    }
            //}
        }

        public delegate void AsyncEventHandler(string YouHuiMsg, string KaiQiangMsg, DataTable dt, ShopModel osa, bool IsJD);
        public delegate void AsyncEventHandlerCreate(List<WrapPanel> warp);
        public delegate void ExportWanlanUrl(List<Item> list, bool IsWanLan);
        public delegate void AsyncItemImgHandler(List<Item> list);
        public List<string> GoodsList { get; set; }
        private bool Ismg = false;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int Tag = Convert.ToInt32((sender as Button).Tag);
            switch (Tag)
            {
                case 0:
                    OpenFileDialog op = new OpenFileDialog();
                    op.Filter = "Excel 文件(*.xlsx)|*.xlsx|Excel 文件(*.xls)|*.xls|所有文件(*.*)|*.*";
                    bool b = false;
                    if (op.ShowDialog() == true)
                    {
                        dt = ExcelNpoi.ExcelToDataTable("sheet1", true, op.FileName);
                        if (dt.Columns.Count >= 2)
                        {
                            if (dt.Columns[0].ColumnName.Equals("款号") && dt.Columns[1].ColumnName.Equals("价格") && dt.Columns[2].ColumnName.Equals("吊牌价"))
                            {
                                b = true;
                            }
                        }
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

                case 9: //生成橱窗图
                    AsyncItemImgHandler asycreateimg = new AsyncItemImgHandler(CreateImgItem);
                    asycreateimg.BeginInvoke(itemList, null, null);
                    break;
            }
        }

        private void NewPrieView(string YouHuiMsg, string KaiQiangMsg, DataTable dt, ShopModel osa, bool IsJD)
        {
            list = new List<CreateImageModel>();
            foreach (DataRow dr in dt.Rows)
            {
                CreateImageModel model = new CreateImageModel() { GoodsNo = dr["款号"].ToString() };
                list.Add(model);
            }

            List<string> rlist = list.Select(x => x.GoodsNo.Trim()).Distinct().ToList();
            itemList = new TopSource(osa).GetItemList(rlist, "item_img.url,prop_img.url, pic_url");
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                NewGridPannel(itemList, YouHuiMsg);
            }));
        }


        private void HotSaleView(string YouHuiMsg, string KaiQiangMsg, DataTable dt, ShopModel osa, bool IsJD)
        {
            list = new List<CreateImageModel>();
            foreach (DataRow dr in dt.Rows)
            {
                CreateImageModel model = new CreateImageModel() { GoodsNo = dr["款号"].ToString(), Text = dr["文字"].ToString() };
                list.Add(model);
            }
            List<string> rlist = list.Select(x => x.GoodsNo.Trim()).Distinct().ToList();
            itemList = new TopSource(osa).GetItemList(rlist, "item_img.url,prop_img.url, pic_url");
            foreach (Item it in itemList)
            {
                list.Find(a => a.GoodsNo.Equals(it.OuterId, StringComparison.CurrentCultureIgnoreCase)).GoodsUrl = it.PicUrl;
            }
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                HotSaleTwoPage(list);
            }));
        }


        private void NewEvaluate(string YouHuiMsg, string KaiQiangMsg, DataTable dt, ShopModel osa, bool IsJD)
        {
            list = new List<CreateImageModel>();
            foreach (DataRow dr in dt.Rows)
            {
                CreateImageModel model = new CreateImageModel() { GoodsNo = dr["款号"].ToString(), Text = dr["文字"].ToString() };
                list.Add(model);
            }
            List<string> rlist = list.Select(x => x.GoodsNo.Trim()).Distinct().ToList();
            itemList = new TopSource(osa).GetItemList(rlist, "item_img.url,prop_img.url, pic_url");
            foreach (Item it in itemList)
            {
                list.Find(a => a.GoodsNo.Equals(it.OuterId)).GoodsUrl = it.PicUrl;
            }
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                Evaluate(list);
            }));
        }


        private void ActivityTemplatePre(string YouHuiMsg, string KaiQiangMsg, DataTable dt, ShopModel osa, bool IsJD)
        {
            list = new List<CreateImageModel>();
            foreach (DataRow dr in dt.Rows)
            {
                CreateImageModel model = new CreateImageModel() { GoodsNo = dr["款号"].ToString().Trim(), Price = dr["价格"].ToString(), DpPrice = dr["吊牌价"].ToString(), WaterImgUrl = dr["水印地址"].ToString() == "" ? "http://www.9myp.com/d/TemplateImage/activity.png" : dr["水印地址"].ToString(), GoodsUrl = "" };
                list.Add(model);
            }

            List<string> rlist = list.Select(x => x.GoodsNo.Trim()).Distinct().ToList();
            itemList = new TopSource(osa).GetItemList(rlist, "item_img.url,prop_img.url, pic_url,price");


            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                string selectItem = comboxMobileItem.SelectedValue.ToString();

                itemList.ForEach(delegate(Item i)
                {
                    CreateImageModel m = list.Find(a => a.GoodsNo.Equals(i.OuterId));
                    m.GoodsUrl = i.PicUrl;
                    //if (selectItem.Equals("activity"))
                    //{
                    //    m.DpPrice = i.Price.ToString();
                    //}
                });

                if (selectItem.Equals("activity"))
                {
                    MobileActivityGridPannel(list);
                }
                else if (selectItem.Equals("free"))
                {
                    MobileFreeGridPannel(list);
                }
                else if (selectItem.Equals("new"))
                {
                    MobileNewGridPannel(list);
                }
                else if (selectItem.Equals("bigPromotion"))
                {
                    foreach (CreateImageModel cim in list)
                    {
                        cim.WaterImgUrl = txtfilePath.Text.Trim();
                    }

                    MobileBigPromotionGridPannel(list);
                }

            }));

        }


        private void CreateImgItem(List<Item> list)
        {
            if (list == null)
            {
                return;
            }
            bar.LoadBar(true);
            bar.Loading(true);
            string path = desktopPath + "\\橱窗图" + System.Guid.NewGuid().ToString().Replace("-", "") + "\\";
            WebClient client = new WebClient();
            DataTable dt = new DataTable();
            dt.Columns.Add("款号", typeof(string));
            dt.Columns.Add("ID", typeof(long));
            dt.Columns.Add("标题", typeof(string));
            bar.Loading(false);
            bar.SetNavigation("正在下载图片");
            int count = list.Select(x => x.ItemImgs).Sum(x => x.Count);
            int start = 0;
            for (int i = 0; i < list.Count; i++)
            {
                DataRow dr = dt.NewRow();
                dr["款号"] = list[i].OuterId;
                dr["ID"] = list[i].NumIid;
                dr["标题"] = list[i].Title;
                dt.Rows.Add(dr);
                List<ItemImg> imglist = list[i].ItemImgs;
                for (int j = 0; j < imglist.Count; j++)
                {
                    start++;
                    string Dir = path + list[i].OuterId + "\\";
                    if (!Directory.Exists(Dir))
                    {
                        Directory.CreateDirectory(Dir);
                    }
                    string filepath = Dir + list[i].OuterId + "-" + j + ".jpg";
                    client.DownloadFile(imglist[j].Url, filepath);
                    int current = start;
                    bar.UpdateBarValue(count, current);
                }

            }
            ExcelNpoi.TableToExcel(dt, path + "标题.xls");
            bar.SetNavigation("");
            bar.LoadBar(false);

        }

        public void RunExportUrl(bool IsWanlan, ShopModel shop)
        {
            List<CheckBox> checklists = WPFControlsSearchHelper.GetChildObjects<CheckBox>(GoodsGrid, "");
            if (checklists.Count > 0)
            {
                for (int i = 0; i < itemList.Count; i++)
                {
                    if (checklists[i].IsChecked == true)
                    {
                        itemList[i].Is3D = true;
                    }
                    else
                    {
                        itemList[i].Is3D = false;
                    }
                }

                itemList = itemList.OrderByDescending(x => x.Is3D).ToList();
            }

            if (itemList == null)
            {
                if (dt == null)
                {
                    CCMessageBox.Show("没有加载数据");
                    return;
                }
                //将TABLE转换成List
                list = new List<CreateImageModel>();
                foreach (DataRow dr in dt.Rows)
                {
                    CreateImageModel model = new CreateImageModel() { GoodsNo = dr["款号"].ToString(), Price = Convert.ToDecimal(dr["价格"]).ToString("f2"), DpPrice = Convert.ToDecimal(dr["吊牌价"]).ToString("f2"), WaterImgUrl = dr["水印地址"].ToString(), Text = dr["文字"].ToString() };
                    list.Add(model);
                }

                //得到淘宝的数据集合

                List<string> rlist = list.Select(x => x.GoodsNo.Trim()).Distinct().ToList();
                itemList = new TopSource(shop).GetItemList(rlist, "item_img.url,prop_img.url, pic_url");
            }
            //实例委托
            ExportWanlanUrl ewus = new ExportWanlanUrl(ExportWanLanUrl);
            //异步调用开始，没有回调函数和AsyncState,都为null
            IAsyncResult irr = ewus.BeginInvoke(itemList, IsWanlan, null, null);
        }

        private void ExportWanLanUrl(List<Item> list, bool IsWanLan)
        {
            bar.LoadBar(true);
            bar.Loading(true);
            DataTable WanLanUrltable = new DataTable();
            WanLanUrltable.Columns.Add("款号", typeof(string));
            WanLanUrltable.Columns.Add("链接", typeof(string));
            WanLanUrltable.Columns.Add("模特图", typeof(string));
            //淘宝现价
            // List<Item> itemlist = new Commodity().GetItemList(list, string.Empty, false);
            list.ForEach(x =>
            {
                DataRow dr = WanLanUrltable.NewRow();
                dr["款号"] = x.OuterId + "-WJ";
                dr["链接"] = IsWanLan ? "http://h5.m.taobao.com/awp/core/detail.htm?id=" + x.NumIid : "https://item.taobao.com/item.htm?id=" + x.NumIid;
                dr["模特图"] = x.Is3D ? "是" : "否";
                WanLanUrltable.Rows.Add(dr);
            });
            //获取当前用户桌面的路径
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            if (WanLanUrltable.Rows.Count > 0)
            {
                string text = IsWanLan ? "无线链接" : "PC链接";
                ExcelNpoi.TableToExcel(WanLanUrltable, desktopPath + string.Format(@"\\{0}{1}.xls", DateTime.Now.ToShortDateString(), text));
                CCMessageBox.Show("表格己生成在桌面");
            }
            else
            {
                CCMessageBox.Show("无数据导出");
            }

            bar.LoadBar(false);
            bar.Loading(false);
        }

        public void ClearGoodsGrid()
        {
            GoodsGrid.Children.Clear();
            GoodsGrid.RowDefinitions.Clear();
            GoodsGrid.ColumnDefinitions.Clear();
        }


        #region HTML
        private void CreateHTMLFile(string YouHuiMsg, string KaiQiangMsg, DataTable dt, ShopModel osa, bool IsJD)
        {
            StringBuilder sb = new StringBuilder();
            bar.LoadBar(true);
            bar.Loading(true);
            list = new List<CreateImageModel>();
            foreach (DataRow dr in dt.Rows)
            {
                CreateImageModel model = new CreateImageModel() { GoodsNo = dr["款号"].ToString(), Price = Convert.ToDecimal(dr["价格"]).ToString("f2") };
                list.Add(model);
            }
            //获取当前用户桌面的路径
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            //保存路径
            saveCodePath = desktopPath + string.Format(@"\{0}\", DateTime.Now.ToString("yyyyMMdd") + @"\" + System.Guid.NewGuid().ToString());
            //如果没有就创建文件
            if (!Directory.Exists(saveCodePath))
            {
                Directory.CreateDirectory(saveCodePath);
            }

            //如果是京东
            if (IsJD)
            {
                WareManager manager = new WareManager(osa.appKey, osa.appSecret, osa.sessionKey);

                //查询所有商品
                List<Ware> idlist = manager.SearchWares(null, null, null, null, null, null, null);
                //得到所有款号查询SKU
                List<string> itemlist = list.Select(x => x.GoodsNo).ToList();
                List<string> wareidlist = idlist.Where(x => itemlist.Contains(x.ItemNum)).Select(y => y.WareId.ToString()).ToList();
                List<JdSdk.Domain.Ware.Sku> skulist = manager.GetSkuListByID(wareidlist, null, false);
                //组装对象(MarketPrice为优惠后价格)
                sb.Append(@"<div style='width:1100px;overflow:hidden;margin-left:auto;margin-right:auto;'>
                                    <ul style='width:110%;'>");
                foreach (var item in list)
                {
                    //寻找款
                    Ware ware = idlist.Find(x =>
                    {
                        return x.ItemNum.Equals(item.GoodsNo, StringComparison.CurrentCultureIgnoreCase);
                    });
                    if (ware != null)
                    {
                        //寻找SKU组成出售链接
                        JdSdk.Domain.Ware.Sku sk = skulist.Find(y =>
                        {
                            return y.WareId == ware.WareId;
                        });
                        if (sk != null)
                        {
                            Ware t = new Ware();
                            t.AdContent = string.Format("http://item.jd.com/{0}.html", sk.SkuId); //出售链接
                            //t.JdPrice = item.Value : ware.JdPrice;
                            t.MarketPrice = item.Price; //加入价格
                            t.Logo = ware.Logo;
                            t.JdPrice = ware.JdPrice;
                            sb.Append(string.Format(@"<li style='float:left;overflow:hidden;margin:12px 12px 15px 0;width:266px;'>
                                        <a href='{5}' target='_blank'>
                                            <img src='{0}' width='266' height='266'/>
                                            <span style='display:block;text-align:center;font-size:15px;color:#666;margin-top:10px;font-family:微软雅黑;'>
					                            {1}：
					                            <b style='font-weight: normal;font-family:微软雅黑;'>￥</b>
					                            <em style='font-size:16px;color:#ff0000;font-style: inherit;font-family:微软雅黑;'>{2}</em>
				                            </span>
                                            <span style='display:block;text-align:center;font-size:14px;color:#a6a6a6;padding:3px 0;'><s style='font-family:微软雅黑;'>￥{3}</s></span>
                                            <span style='display:block;text-align:center;padding:5px 0;'><strong style='font-family:微软雅黑;padding:2px 5px;border:2px solid #ff0000;color:#ff0000;font-size:15px;'>{4}</strong></span>
                                        </a>
                                    </li>", t.Logo, YouHuiMsg, t.MarketPrice, t.JdPrice, KaiQiangMsg, t.AdContent));
                        }
                    }
                }
                sb.Append("</ul></div>");
            }
            else //得到淘宝的数据集合
            {
                TopSource com = new TopSource(osa);
                itemList = com.GetItemList(list.Select(x => x.GoodsNo).Distinct().ToList(), "item_img.url,prop_img.url, pic_url,detail_url,approve_status,num");

                sb.Append(@"<div class='width-1100' style='width:1100px;overflow: hidden;margin-left:-75px;'>
                                    <ul style='width:110%;'>");
                foreach (Item it in itemList)
                {
                    string style = string.Empty;
                    string saleout = string.Empty;
                    if (Ismg == true)
                    {
                        if (it.ApproveStatus.Equals("instock") || it.Num == 0)
                        {
                            style = @"style='position:relative;'";
                            saleout = @" <span class='po-a' style='position:absolute; left:69px; top:64px; display:block; width:110px; height:110px; z-index:999; background:url(https://img.alicdn.com/imgextra/i1/73900259/TB2.H7kipXXXXaoXpXXXXXXXXXX_!!73900259.png) no-repeat;'>&nbsp;</span>";
                        }
                    }
                    CreateImageModel cim = list.Where(x => x.GoodsNo.Equals(it.OuterId, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                    if (string.IsNullOrEmpty(KaiQiangMsg))
                    {
                        sb.Append(string.Format(@"<li style='float:left;overflow:hidden;margin:12px 12px 15px 0;width:266px;'>
                                        <a href='{5}' target='_blank' {6}>
                                            <img src='{0}' width='266' height='266'/>
                                            <span style='display:block;text-align:center;font-size:15px;color:#666;margin-top:10px;font-family:微软雅黑;'>
					                            {1}：
					                            <b style='font-weight: normal;font-family:微软雅黑;'>￥</b>
					                            <em style='font-size:16px;color:#ff0000;font-style: inherit;font-family:微软雅黑;'>{2}</em>
				                            </span>
                                            <span style='display:block;text-align:center;font-size:14px;color:#a6a6a6;padding:3px 0;'><s style='font-family:微软雅黑;'>￥{3}</s></span>
                                           
                                            {7}
                                        </a>
                                    </li>", it.ItemImgs[0].Url + "_300x300", YouHuiMsg, cim.Price, it.Price, KaiQiangMsg, it.DetailUrl, style, saleout));
                    }
                    else
                    {
                        sb.Append(string.Format(@"<li style='float:left;overflow:hidden;margin:12px 12px 15px 0;width:266px;'>
                                        <a href='{5}' target='_blank' {6}>
                                            <img src='{0}' width='266' height='266'/>
                                            <span style='display:block;text-align:center;font-size:15px;color:#666;margin-top:10px;font-family:微软雅黑;'>
					                            {1}：
					                            <b style='font-weight: normal;font-family:微软雅黑;'>￥</b>
					                            <em style='font-size:16px;color:#ff0000;font-style: inherit;font-family:微软雅黑;'>{2}</em>
				                            </span>
                                            <span style='display:block;text-align:center;font-size:14px;color:#a6a6a6;padding:3px 0;'><s style='font-family:微软雅黑;'>￥{3}</s></span>
                                            <span style='display:block;text-align:center;padding:5px 0;'><strong style='font-family:微软雅黑;padding:2px 5px;border:2px solid #ff0000;color:#ff0000;font-size:15px;'>{4}</strong></span>
                                            {7}
                                        </a>
                                    </li>", it.ItemImgs[0].Url + "_300x300", YouHuiMsg, cim.Price, it.Price, KaiQiangMsg, it.DetailUrl, style, saleout));
                    }
                }

                sb.Append("</ul></div>");
                Ismg = false;
            }
            System.IO.File.WriteAllText(string.Format(saveCodePath + "{0}.txt", DateTime.Now.ToString("yyyyMMddhhmmss")), sb.ToString());
        }
        private void CreateHTMLFileCallBack(IAsyncResult ar)
        {
            try
            {
                bar.LoadBar(false);
                bar.Loading(false);
            }
            catch
            { }
            CCMessageBox.Show("代码已生成至桌面！");
        }

        #endregion

        #region 生成图片
        private void CreateImage(List<WrapPanel> list)
        {
            //获取当前用户桌面的路径
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            //保存路径
            saveCodePath = desktopPath + string.Format(@"\{0}\", DateTime.Now.ToString("yyyyMMdd") + @"\" + System.Guid.NewGuid().ToString());
            //如果没有就创建文件
            if (!Directory.Exists(saveCodePath))
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    Directory.CreateDirectory(saveCodePath);
                }));
            }
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
              {
                  for (int i = 0; i < list.Count; i++)
                  {
                      ImageHelper.SaveToImage(list[i], saveCodePath + list[i].Tag.ToString() + "-WJ.png");
                      //ImageHelper.SaveToImage(list[i], saveCodePath + itemList[i].OuterId + "-WJ.png");
                  }
                  CCMessageBox.Show("全部图片己生成在桌面");
              }));
        }

        #endregion

        #region 图片预览
        public void PreviewImage(string YouHuiMsg, string KaiQiangMsg, DataTable dt, ShopModel osas, bool IsJD)
        {
            if (YouHuiMsg == null)
            {
                #region 新模板 2016-10-22 BY WL

                //打开加载
                bar.LoadBar(true);
                //打开加载中的字符
                bar.Loading(true);
                //将TABLE转换成List
                list = new List<CreateImageModel>();
                foreach (DataRow dr in dt.Rows)
                {
                    CreateImageModel model = new CreateImageModel() { GoodsNo = dr["款号"].ToString(), Price = Convert.ToDecimal(dr["价格"]).ToString("f2"), DpPrice = Convert.ToDecimal(dr["吊牌价"]).ToString("f2"), WaterImgUrl = dr["水印地址"].ToString(), Text = dr["文字"].ToString() };
                    list.Add(model);
                }

                //得到淘宝的数据集合

                List<string> rlist = list.Select(x => x.GoodsNo.Trim()).Distinct().ToList();


                itemList = new TopSource(osas).GetItemList(rlist, "item_img.url,prop_img.url, pic_url");

                foreach (Item it in itemList)
                {
                    list.Find(a => a.GoodsNo.Equals(it.OuterId)).GoodsUrl = it.PicUrl;
                }
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    //GridPannel(itemList, YouHuiMsg);
                    GridPannel(list);
                }));
                bar.Loading(false);
                bar.LoadBar(false);

                #endregion
            }
            else if (YouHuiMsg != "")
            {
                #region 旧模板

                //打开加载
                bar.LoadBar(true);
                //打开加载中的字符
                bar.Loading(true);
                //将TABLE转换成List
                list = new List<CreateImageModel>();
                foreach (DataRow dr in dt.Rows)
                {
                    CreateImageModel model = new CreateImageModel() { GoodsNo = dr["款号"].ToString(), Price = Convert.ToDecimal(dr["价格"]).ToString("f2"), DpPrice = dr["吊牌价"] != DBNull.Value ? Convert.ToDecimal(dr["吊牌价"]).ToString("f2") : "0" };
                    list.Add(model);
                }

                //得到淘宝的数据集合
                // OnlineStoreAuthorization osa = CommonLogin.CommonUser.CShop;

                List<string> rlist = list.Select(x => x.GoodsNo.Trim()).Distinct().ToList();


                List<Item> List = new TopSource(osas).GetItemList(rlist, "item_img.url,prop_img.url, pic_url");

                //获取不存在的款号
                //string Res = string.Empty;
                //list.ForEach(x =>
                //{
                //    int count = itemList.Where(y => y.OuterId.Equals(x.GoodsNo, StringComparison.CurrentCultureIgnoreCase)).Count();
                //    if(count==0)
                //    {
                //        Res += x.GoodsNo + "\r\n";
                //    }
                //});
                //加入优惠价格(PostFee字段)
                //foreach (Item it in itemList)
                //{
                //    it.PostFee = list.Where(x => x.GoodsNo.Trim().Equals(it.OuterId.Trim(), StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Price;
                //}

                //itlist这里的排序要与导入表格的顺序一致。否则GridPannel()的吊牌价会错乱
                itemList = new List<Item>();
                rlist.ForEach(x =>
                {
                    Item it = List.Find(z =>
                    {
                        return z.OuterId.Equals(x);
                    });
                    if (it != null)
                    {
                        it.PostFee = list.Where(f => f.GoodsNo.Trim().Equals(it.OuterId.Trim(), StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Price;
                        itemList.Add(it);
                    }

                });
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    GridPannel(itemList, YouHuiMsg);
                }));
                bar.Loading(false);
                bar.LoadBar(false);

                #endregion
            }
        }
        #endregion

        #region 手机普通模版(旧模板)
        public void GridPannel(List<Item> GoodsNoList, string YouHuiMsg)
        {

            ClearGoodsGrid();
            //每行几列
            int Column = 6;
            //得到行数
            int Rows = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(GoodsNoList.Count) / Column));
            //GRID布局
            for (int i = 0; i < Rows; i++)
            {
                RowDefinition row = new RowDefinition();
                row.Height = new GridLength(400);
                GoodsGrid.RowDefinitions.Add(row);
            }
            for (int j = 0; j < Column; j++)
            {
                ColumnDefinition col = new ColumnDefinition();
                col.Width = new GridLength(305);
                GoodsGrid.ColumnDefinitions.Add(col);
            }

            //添加控件
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Column; j++)
                {
                    int Index = (i * Column) + j;
                    if (Index == GoodsNoList.Count)
                    {
                        break;
                    }

                    //<StackPanel  Background="#FFFFFF" Orientation="Vertical" Name="imageTempate" Height="380" Width="304">
                    //                <Image Width="290" Height="290" Name="img_box" Margin="0 17 0 0"></Image>
                    //                <StackPanel Orientation="Horizontal" Margin="0,5,0,0" Height="68">
                    //                    <Label Width="80"  Foreground="#525661" FontSize="18" VerticalContentAlignment="Center" HorizontalContentAlignment="Right" Name="lab_youhui">特惠价</Label>

                    //                    <TextBlock FontFamily="/9M.Work.WPF_Main;component/Resources/#Vinyl" Name="lab_price" Width="100"  Foreground="Red" FontSize="30" VerticalAlignment="Center">88.00</TextBlock>
                    //                    <!--<Label Width="100"  Foreground="Red" FontSize="30" VerticalContentAlignment="Center" Name="lab_price" Style="{DynamicResource QuartzMSFont}">88.00</Label>-->
                    //                    <Label Width="90"  FontSize="18" Foreground="#525661" VerticalContentAlignment="Center">点击购买</Label>
                    //                    <Image Source="/9M.Work.WPF_Main;component/Images/jiantou.png" Margin="3,0,0,0" Height="12"></Image>
                    //                </StackPanel>
                    //</StackPanel>

                    //-------------------生成模板
                    string ImageUrl = itemList[Index].ItemImgs.Count > 0 ? itemList[Index].ItemImgs[0].Url + "_300x300" : "";
                    StackPanel stt = new StackPanel() { Orientation = Orientation.Vertical, Height = 400, Width = 305 };

                    WrapPanel wrap = new WrapPanel() { Background = new SolidColorBrush(Colors.White), Height = 380, Width = 304, Tag = GoodsNoList[Index].OuterId };
                    Image image = new Image() { Width = 290, Height = 290, Margin = new Thickness(6, 17, 0, 0), Source = new BitmapImage(new Uri(ImageUrl, UriKind.RelativeOrAbsolute)) };


                    StackPanel sta_dp = new StackPanel() { Orientation = Orientation.Horizontal, Margin = new Thickness(20, 5, 0, 0) };
                    TextBlock lab_dp_tb = new TextBlock()
                    {
                        Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#525661")),
                        FontSize = 12,
                        Text = "吊牌价:" + Convert.ToInt32(Convert.ToDecimal(list[Index].DpPrice)),
                        TextDecorations = TextDecorations.Strikethrough,
                    };

                    //如果吊牌价为空就不显示
                    if (list[Index].DpPrice.Equals("0"))
                    {
                        lab_dp_tb.Text = string.Empty;
                    }
                    sta_dp.Children.Add(lab_dp_tb);
                    StackPanel sta = new StackPanel() { Orientation = Orientation.Horizontal };
                    Label lab = new Label()
                    {
                        Width = 80,
                        Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#525661")),
                        FontSize = 18,
                        VerticalContentAlignment = System.Windows.VerticalAlignment.Center,
                        HorizontalContentAlignment = System.Windows.HorizontalAlignment.Right,
                        Content = YouHuiMsg
                    };

                    TextBlock tb = new TextBlock()
                    {
                        Width = 100,
                        Foreground = new SolidColorBrush(Colors.Red),
                        FontSize = 30,
                        Style = this.FindResource("QuartzMSFont") as Style,
                        VerticalAlignment = System.Windows.VerticalAlignment.Center,
                        Text = Convert.ToDecimal(itemList[Index].PostFee).ToString("F1")
                    };
                    Label xlab = new Label()
                    {
                        Width = 90,
                        FontSize = 18,
                        Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#525661")),
                        VerticalContentAlignment = System.Windows.VerticalAlignment.Center,
                        Content = "点击购买"
                    };
                    Image im = new Image()
                    {
                        Height = 12,
                        Margin = new Thickness(3, 0, 0, 0),
                        Source = new BitmapImage(new Uri("/9M.Work.WPF_Main;component/Images/jiantou.png", UriKind.RelativeOrAbsolute))
                    };
                    sta.Children.Add(lab);
                    sta.Children.Add(tb);
                    sta.Children.Add(xlab);
                    sta.Children.Add(im);
                    wrap.Children.Add(image);
                    wrap.Children.Add(sta_dp);
                    wrap.Children.Add(sta);

                    Border bs = new Border() { BorderThickness = new Thickness(1), BorderBrush = new SolidColorBrush(Colors.Gray), HorizontalAlignment = System.Windows.HorizontalAlignment.Center };
                    //bs.Child = wrap;
                    stt.Children.Add(wrap);
                    stt.Children.Add(new CheckBox() { Content = "模特图", FontSize = 15, FontWeight = FontWeights.Bold, Margin = new Thickness(200, -10, 0, 0) });
                    bs.Child = stt;

                    Grid.SetRow(bs, i);
                    Grid.SetColumn(bs, j);
                    GoodsGrid.Children.Add(bs);

                }
            }
        }

        public void NewGridPannel(List<Item> GoodsNoList, string YouHuiMsg)
        {
            ClearGoodsGrid();
            //每行几列
            int Column = 6;
            //得到行数
            int Rows = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(GoodsNoList.Count) / Column));
            //GRID布局
            for (int i = 0; i < Rows; i++)
            {
                RowDefinition row = new RowDefinition();
                row.Height = new GridLength(350);
                GoodsGrid.RowDefinitions.Add(row);
            }
            for (int j = 0; j < Column; j++)
            {
                ColumnDefinition col = new ColumnDefinition();
                col.Width = new GridLength(305);
                GoodsGrid.ColumnDefinitions.Add(col);
            }

            //添加控件
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Column; j++)
                {
                    int Index = (i * Column) + j;
                    if (Index == GoodsNoList.Count)
                    {
                        break;
                    }
                    WrapPanel wrap = new WrapPanel() { Tag = GoodsNoList[Index].OuterId, Height = 330, Width = 260 };
                    Border bs = new Border() { BorderThickness = new Thickness(1), BorderBrush = new SolidColorBrush(Colors.Gray), HorizontalAlignment = System.Windows.HorizontalAlignment.Center };
                    // bs.Child = new ActivityRed(GoodsNoList[Index].PicUrl, YouHuiMsg, GoodsNoList[Index].Price);
                    Grid.SetRow(bs, i);
                    Grid.SetColumn(bs, j);
                    wrap.Children.Add(new ActivityRed(GoodsNoList[Index].PicUrl, YouHuiMsg, GoodsNoList[Index].Price));
                    bs.Child = wrap;
                    GoodsGrid.Children.Add(bs);
                }
            }
        }

        #endregion

        #region 手机普通模版(新模板) 2016-10-22 BY:WL
        public void GridPannel(List<CreateImageModel> GoodsNoList)
        {
            ClearGoodsGrid();
            //每行几列
            int Column = 6;
            //得到行数
            int Rows = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(GoodsNoList.Count) / Column));
            //GRID布局
            for (int i = 0; i < Rows; i++)
            {
                RowDefinition row = new RowDefinition();
                row.Height = new GridLength(380);
                GoodsGrid.RowDefinitions.Add(row);
            }
            for (int j = 0; j < Column; j++)
            {
                ColumnDefinition col = new ColumnDefinition();
                col.Width = new GridLength(310);
                GoodsGrid.ColumnDefinitions.Add(col);
            }

            //添加控件
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Column; j++)
                {
                    int Index = (i * Column) + j;
                    if (Index == GoodsNoList.Count)
                    {
                        break;
                    }

                    //<StackPanel  Background="#FFFFFF" Orientation="Vertical" Name="imageTempate" Height="380" Width="300">
                    //    <Canvas Width="300" Height="310">
                    //        <Image Width="300" Height="310" Name="img_box" Margin="0 0 0 0"></Image>
                    //        <Canvas Width="60" Height="70" Margin="20 0 0 0">
                    //            <Canvas.Background>
                    //                <ImageBrush ImageSource="http://www.9myp.com/d/mttest.png" TileMode="Tile" Stretch="UniformToFill" />
                    //            </Canvas.Background>
                    //        </Canvas>
                    //    </Canvas>
                    //    <StackPanel Orientation="Horizontal" Margin="0,0,0,0" Height="80" Width="300">
                    //        <StackPanel Orientation="Vertical">
                    //            <TextBlock FontSize="16" FontFamily="微软雅黑" Text="吊牌价:1219" Foreground="#fff" TextDecorations="Strikethrough" Margin="20 10 0 0"></TextBlock>
                    //            <TextBlock FontSize="16" FontFamily="微软雅黑" Text="活动价" Foreground="#dc0032"  Margin="25 8 0 20"></TextBlock>
                    //        </StackPanel>
                    //        <StackPanel Orientation="Horizontal">
                    //            <TextBlock FontSize="18" FontFamily="微软雅黑" Text="￥" Foreground="#fff"  Margin="0 32 0 0"></TextBlock>
                    //            <TextBlock FontSize="48" FontFamily="微软雅黑" Text="85" Foreground="#fff"  Margin="0 0 0 0"></TextBlock>
                    //        </StackPanel>
                    //        <TextBlock FontSize="24" FontFamily="微软雅黑" Text="抢购" Foreground="#dc0032"  Margin="60 20 0 20"></TextBlock>
                    //        <StackPanel.Background>
                    //            <ImageBrush ImageSource="https://img.alicdn.com/imgextra/i1/73900259/TB2GB2.X9iJ.eBjSspiXXbqAFXa-73900259.jpg" TileMode="Tile" Stretch="UniformToFill" />
                    //        </StackPanel.Background>

                    //    </StackPanel>
                    //</StackPanel>

                    //-------------------生成模板

                    StackPanel stt = new StackPanel() { Orientation = Orientation.Vertical, Height = 380, Width = 304 };

                    WrapPanel wrap = new WrapPanel() { Background = new SolidColorBrush(Colors.White), Height = 380, Width = 304, Tag = GoodsNoList[Index].GoodsNo };

                    Canvas canva = new Canvas() { Width = 304, Height = 300 };
                    Image goodsimg = new Image()
                    {
                        Width = 304,
                        Height = 300,
                        Margin = new Thickness(0, 0, 0, 0),
                        Source = GoodsNoList[Index].GoodsUrl == null ? null : new BitmapImage(new Uri(String.Format(@"{0}", GoodsNoList[Index].GoodsUrl + "_300x300"), UriKind.Absolute))
                    };


                    canva.Children.Add(goodsimg);

                    //logo 水印
                    //Canvas canvaLogo = new Canvas()
                    //{
                    //    Width = 60,
                    //    Height = 70,
                    //    Margin = new Thickness(20, 0, 0, 0),
                    //    Background = new ImageBrush()
                    //    {
                    //        ImageSource = new BitmapImage(new Uri(String.Format(@"{0}", GoodsNoList[Index].WaterImgUrl), UriKind.Absolute)),
                    //        Stretch = Stretch.UniformToFill,
                    //        TileMode = TileMode.Tile
                    //    }
                    //};
                    //canva.Children.Add(canvaLogo);

                    wrap.Children.Add(canva);

                    StackPanel sp_children = new StackPanel()
                    {
                        Orientation = Orientation.Horizontal,
                        Height = 80,
                        Width = 304,
                        Margin = new Thickness(0, 0, 0, 0),
                        Background = new ImageBrush()
                        {
                            ImageSource = new BitmapImage(new Uri(String.Format(@"{0}", "https://img.alicdn.com/imgextra/i1/73900259/TB2nHTya7WM.eBjSZFhXXbdWpXa-73900259.jpg"), UriKind.Absolute)),
                            Stretch = Stretch.UniformToFill,
                            TileMode = TileMode.Tile
                        }
                    };

                    StackPanel sp_children_1 = new StackPanel() { Orientation = Orientation.Vertical };
                    sp_children_1.Children.Add(new TextBlock()
                    {
                        FontSize = 14,
                        FontFamily = new FontFamily("微软雅黑"),
                        Text = "零售价:" + Convert.ToInt32(Convert.ToDecimal(GoodsNoList[Index].DpPrice)).ToString(),
                        Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#fff")),
                        TextDecorations = TextDecorations.Strikethrough,
                        Margin = new Thickness(10, 10, 0, 0)
                    });
                    sp_children_1.Children.Add(new TextBlock()
                    {
                        FontSize = 20,
                        FontFamily = new FontFamily("微软雅黑"),
                        FontWeight = FontWeights.Bold,
                        Text = GoodsNoList[i].Text,
                        Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#dc0032")),
                        Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#fff517")),
                        HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                        Padding = new Thickness(3, 0, 3, 0),
                        Margin = new Thickness(10, 8, 0, 20)
                    });
                    sp_children.Children.Add(sp_children_1);


                    StackPanel sp_children_2 = new StackPanel() { Orientation = Orientation.Horizontal };
                    //sp_children_2.Children.Add(new TextBlock()
                    //{
                    //    FontSize = 18,
                    //    FontFamily = new FontFamily("微软雅黑"),
                    //    Text = "￥",
                    //    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#fff")),
                    //    Margin = new Thickness(0, 32, 0, 0)
                    //});

                    String p = GoodsNoList[Index].Price.Contains(".") ? GoodsNoList[Index].Price.Substring(GoodsNoList[Index].Price.IndexOf(".") + 1) : GoodsNoList[Index].Price;

                    sp_children_2.Children.Add(new TextBlock()
                    {
                        FontSize = 42,
                        FontFamily = new FontFamily("微软雅黑"),
                        FontWeight = FontWeights.Bold,
                        Text = (p == "00" || p == "0") ? Convert.ToDecimal(GoodsNoList[Index].Price).ToString("F0") : Convert.ToDecimal(GoodsNoList[Index].Price).ToString("F1"),
                        Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#fff")),
                        Margin = new Thickness(30, 15, 0, 0),
                        FontStretch = new FontStretch() { }

                    });
                    sp_children.Children.Add(sp_children_2);

                    //sp_children.Children.Add(new TextBlock()
                    //{

                    //    FontSize = 24,
                    //    FontFamily = new FontFamily("微软雅黑"),
                    //    Text = "抢购",
                    //    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#dc0032")),
                    //    Margin = new Thickness(60, 20, 0, 20)

                    //});

                    wrap.Children.Add(sp_children);

                    stt.Children.Add(wrap);

                    Border bs = new Border() { BorderThickness = new Thickness(1), BorderBrush = new SolidColorBrush(Colors.Gray), HorizontalAlignment = System.Windows.HorizontalAlignment.Center };
                    bs.Child = stt;
                    Grid.SetRow(bs, i);
                    Grid.SetColumn(bs, j);
                    GoodsGrid.Children.Add(bs);

                }
            }


        }

        #endregion

        #region 手机活动区模版 2016-12-03 BY:WL
        public void MobileActivityGridPannel(List<CreateImageModel> GoodsNoList)
        {
            ClearGoodsGrid();
            //每行几列
            int Column = 6;
            //得到行数
            int Rows = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(GoodsNoList.Count) / Column));
            //GRID布局
            for (int i = 0; i < Rows; i++)
            {
                RowDefinition row = new RowDefinition();
                row.Height = new GridLength(456);
                GoodsGrid.RowDefinitions.Add(row);
            }
            for (int j = 0; j < Column; j++)
            {
                ColumnDefinition col = new ColumnDefinition();
                col.Width = new GridLength(310);
                GoodsGrid.ColumnDefinitions.Add(col);
            }

            //添加控件
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Column; j++)
                {
                    try
                    {
                        int Index = (i * Column) + j;
                        if (Index == GoodsNoList.Count)
                        {
                            break;
                        }

                        //    <StackPanel  Background="#FFFFFF" Orientation="Vertical" Name="imageTempate" Height="420" Width="300">
                        //    <Image Width="300" Height="300" Name="img_box" Margin="0 0 0 0"></Image>
                        //    <Canvas  Margin="0,0,0,0" Height="120" Width="300">
                        //        <Label Content="30元" FontFamily="微软雅黑" FontSize="20" FontWeight="Bold" Margin="75 14 0 0"></Label>
                        //        <Label Content="30元" FontFamily="微软雅黑" FontSize="16" Margin="220 17 0 0"></Label>
                        //        <StackPanel Orientation="Horizontal" Margin="85 50 0 0" >
                        //            <Label Content="108" FontFamily="微软雅黑" FontSize="48" Margin="2 0 0 0" Foreground="#ffff00" FontWeight="Bold"></Label>
                        //            <Label Content=".00" FontFamily="微软雅黑" FontSize="28"  Foreground="#ffff00" FontWeight="Bold" VerticalAlignment="Bottom" Margin="-10 0 0 6"></Label>
                        //        </StackPanel>
                        //        <Canvas.Background>
                        //            <ImageBrush ImageSource="http://www.9myp.com/d/activity.png" TileMode="Tile" Stretch="UniformToFill" />
                        //        </Canvas.Background>
                        //    </Canvas>
                        //</StackPanel>
                        //-------------------生成模板

                        StackPanel stt = new StackPanel() { Orientation = Orientation.Vertical, Height = 456, Width = 304 };

                        WrapPanel wrap = new WrapPanel() { Background = new SolidColorBrush(Colors.White), Height = 456, Width = 304, Tag = GoodsNoList[Index].GoodsNo };

                        Image goodsimg = new Image()
                        {
                            Width = 300,
                            Height = 300,
                            Margin = new Thickness(2, 0, 0, 0),
                            Source = new BitmapImage(new Uri(String.Format(@"{0}", GoodsNoList[Index].GoodsUrl + "_300x300"), UriKind.Absolute)),
                        };

                        wrap.Children.Add(goodsimg);

                        Canvas canva = new Canvas()
                        {
                            Width = 304,
                            Height = 156,
                            Background = new ImageBrush()
                            {
                                ImageSource = new BitmapImage(new Uri(String.Format(@"{0}", GoodsNoList[Index].WaterImgUrl), UriKind.Absolute)),
                                Stretch = Stretch.UniformToFill,
                                TileMode = TileMode.Tile
                            }
                        };

                        Label lab1 = new Label()
                        {
                            FontSize = 20,
                            FontWeight = FontWeights.Bold,
                            Margin = new Thickness(78, 24, 0, 0),
                            FontFamily = new FontFamily("微软雅黑"),
                            Content = Convert.ToDecimal(Convert.ToDecimal(GoodsNoList[Index].DpPrice) - Convert.ToDecimal(GoodsNoList[Index].Price)).ToString("F1") + "元"
                        };

                        canva.Children.Add(lab1);

                        Label lab2 = new Label()
                        {
                            FontSize = 16,
                            Margin = new Thickness(222, 26, 0, 0),
                            FontFamily = new FontFamily("微软雅黑"),
                            Content = Convert.ToDecimal(GoodsNoList[Index].DpPrice).ToString("F1") + "元"
                        };
                        canva.Children.Add(lab2);

                        StackPanel sp = new StackPanel()
                        {

                            Orientation = System.Windows.Controls.Orientation.Horizontal,
                            Margin = new Thickness(88, 72, 0, 0)

                        };

                        string p = Convert.ToDecimal(GoodsNoList[Index].Price).ToString("F1");

                        Label lab3 = new Label()
                        {
                            FontSize = 48,
                            Margin = new Thickness(2, 0, 0, 0),
                            FontWeight = FontWeights.Bold,
                            FontFamily = new FontFamily("微软雅黑"),
                            Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ffff00")),
                            Content = p.Substring(0, p.IndexOf("."))
                        };

                        sp.Children.Add(lab3);

                        Label lab4 = new Label()
                        {
                            FontSize = 28,
                            Margin = new Thickness(-15, 0, 0, 6),
                            FontWeight = FontWeights.Bold,
                            FontFamily = new FontFamily("微软雅黑"),
                            Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ffff00")),
                            VerticalAlignment = System.Windows.VerticalAlignment.Bottom,
                            Content = p.Substring(p.IndexOf("."))
                        };

                        sp.Children.Add(lab4);

                        canva.Children.Add(sp);

                        wrap.Children.Add(canva);

                        stt.Children.Add(wrap);

                        Border bs = new Border() { BorderThickness = new Thickness(1), BorderBrush = new SolidColorBrush(Colors.Gray), HorizontalAlignment = System.Windows.HorizontalAlignment.Center };
                        bs.Child = stt;
                        Grid.SetRow(bs, i);
                        Grid.SetColumn(bs, j);
                        GoodsGrid.Children.Add(bs);
                    }
                    catch
                    { }
                }
            }


        }

        #endregion

        #region 手机买3免1模版 2016-12-05 BY:WL
        public void MobileFreeGridPannel(List<CreateImageModel> GoodsNoList)
        {
            ClearGoodsGrid();
            //每行几列
            int Column = 6;
            //得到行数
            int Rows = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(GoodsNoList.Count) / Column));
            //GRID布局
            for (int i = 0; i < Rows; i++)
            {
                RowDefinition row = new RowDefinition();
                row.Height = new GridLength(456);
                GoodsGrid.RowDefinitions.Add(row);
            }
            for (int j = 0; j < Column; j++)
            {
                ColumnDefinition col = new ColumnDefinition();
                col.Width = new GridLength(310);
                GoodsGrid.ColumnDefinitions.Add(col);
            }

            //添加控件
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Column; j++)
                {
                    int Index = (i * Column) + j;
                    if (Index == GoodsNoList.Count)
                    {
                        break;
                    }

                    //-------------------生成模板

                    StackPanel stt = new StackPanel() { Orientation = Orientation.Vertical, Height = 456, Width = 304 };

                    WrapPanel wrap = new WrapPanel() { Background = new SolidColorBrush(Colors.White), Height = 456, Width = 304, Tag = GoodsNoList[Index].GoodsNo };

                    Image goodsimg = new Image()
                    {
                        Width = 300,
                        Height = 300,
                        Margin = new Thickness(2, 0, 0, 0),
                        Source = new BitmapImage(new Uri(String.Format(@"{0}", GoodsNoList[Index].GoodsUrl + "_300x300"), UriKind.Absolute))
                    };

                    wrap.Children.Add(goodsimg);

                    Canvas canva = new Canvas()
                    {
                        Width = 304,
                        Height = 156,
                        Background = new ImageBrush()
                        {
                            ImageSource = new BitmapImage(new Uri(String.Format(@"{0}", "http://www.9myp.com/d/TemplateImage/free.png"), UriKind.Absolute)),
                            Stretch = Stretch.UniformToFill,
                            TileMode = TileMode.Tile
                        }
                    };

                    StackPanel sp = new StackPanel()
                    {

                        Orientation = System.Windows.Controls.Orientation.Horizontal,
                        Margin = new Thickness(85, 73, 0, 0)

                    };

                    Label lab3 = new Label()
                    {
                        FontSize = 48,
                        Margin = new Thickness(2, 0, 0, 0),
                        FontWeight = FontWeights.Bold,
                        FontFamily = new FontFamily("微软雅黑"),
                        Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ffff00")),
                        Content = Convert.ToDecimal(GoodsNoList[Index].Price).ToString("F0")
                    };

                    sp.Children.Add(lab3);

                    Label lab4 = new Label()
                    {
                        FontSize = 28,
                        Margin = new Thickness(-15, 0, 0, 6),
                        FontWeight = FontWeights.Bold,
                        FontFamily = new FontFamily("微软雅黑"),
                        Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ffff00")),
                        VerticalAlignment = System.Windows.VerticalAlignment.Bottom,
                        Content = ".00"
                    };

                    sp.Children.Add(lab4);

                    canva.Children.Add(sp);

                    wrap.Children.Add(canva);

                    stt.Children.Add(wrap);

                    Border bs = new Border() { BorderThickness = new Thickness(1), BorderBrush = new SolidColorBrush(Colors.Gray), HorizontalAlignment = System.Windows.HorizontalAlignment.Center };
                    bs.Child = stt;
                    Grid.SetRow(bs, i);
                    Grid.SetColumn(bs, j);
                    GoodsGrid.Children.Add(bs);

                }
            }


        }

        #endregion

        #region 手机新品模版 2016-12-05 BY:WL
        public void MobileNewGridPannel(List<CreateImageModel> GoodsNoList)
        {
            ClearGoodsGrid();
            //每行几列
            int Column = 6;
            //得到行数
            int Rows = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(GoodsNoList.Count) / Column));
            //GRID布局
            for (int i = 0; i < Rows; i++)
            {
                RowDefinition row = new RowDefinition();
                row.Height = new GridLength(456);
                GoodsGrid.RowDefinitions.Add(row);
            }
            for (int j = 0; j < Column; j++)
            {
                ColumnDefinition col = new ColumnDefinition();
                col.Width = new GridLength(310);
                GoodsGrid.ColumnDefinitions.Add(col);
            }

            //添加控件
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Column; j++)
                {
                    int Index = (i * Column) + j;
                    if (Index == GoodsNoList.Count)
                    {
                        break;
                    }

                    //-------------------生成模板

                    StackPanel stt = new StackPanel() { Orientation = Orientation.Vertical, Height = 456, Width = 304 };

                    WrapPanel wrap = new WrapPanel() { Background = new SolidColorBrush(Colors.White), Height = 456, Width = 304, Tag = GoodsNoList[Index].GoodsNo };

                    Image goodsimg = new Image()
                    {
                        Width = 300,
                        Height = 300,
                        Margin = new Thickness(2, 0, 0, 0),
                        Source = new BitmapImage(new Uri(String.Format(@"{0}", GoodsNoList[Index].GoodsUrl + "_300x300"), UriKind.Absolute))
                    };

                    wrap.Children.Add(goodsimg);

                    Canvas canva = new Canvas()
                    {
                        Width = 304,
                        Height = 156,
                        Background = new ImageBrush()
                        {
                            ImageSource = new BitmapImage(new Uri(String.Format(@"{0}", "http://www.9myp.com/d/TemplateImage/new.png"), UriKind.Absolute)),
                            Stretch = Stretch.UniformToFill,
                            TileMode = TileMode.Tile
                        }
                    };

                    Label lab2 = new Label()
                    {
                        FontSize = 16,
                        Margin = new Thickness(205, 26, 0, 0),
                        FontFamily = new FontFamily("微软雅黑"),
                        Content = Convert.ToDecimal(GoodsNoList[Index].DpPrice).ToString("F0") + "元"
                    };
                    canva.Children.Add(lab2);

                    StackPanel sp = new StackPanel()
                    {

                        Orientation = System.Windows.Controls.Orientation.Horizontal,
                        Margin = new Thickness(75, 66, 0, 0)

                    };

                    Label lab3 = new Label()
                    {
                        FontSize = 48,
                        Margin = new Thickness(2, 0, 0, 0),
                        FontWeight = FontWeights.Bold,
                        FontFamily = new FontFamily("微软雅黑"),
                        Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ffff00")),
                        Content = Convert.ToDecimal(GoodsNoList[Index].Price).ToString("F0")
                    };

                    sp.Children.Add(lab3);

                    Label lab4 = new Label()
                    {
                        FontSize = 28,
                        Margin = new Thickness(-15, 0, 0, 6),
                        FontWeight = FontWeights.Bold,
                        FontFamily = new FontFamily("微软雅黑"),
                        Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ffff00")),
                        VerticalAlignment = System.Windows.VerticalAlignment.Bottom,
                        Content = ".00"
                    };

                    sp.Children.Add(lab4);

                    canva.Children.Add(sp);

                    wrap.Children.Add(canva);

                    stt.Children.Add(wrap);

                    Border bs = new Border() { BorderThickness = new Thickness(1), BorderBrush = new SolidColorBrush(Colors.Gray), HorizontalAlignment = System.Windows.HorizontalAlignment.Center };
                    bs.Child = stt;
                    Grid.SetRow(bs, i);
                    Grid.SetColumn(bs, j);
                    GoodsGrid.Children.Add(bs);

                }
            }


        }

        #endregion

        #region 热销新品二级页 2017-04-05
        public void HotSaleTwoPage(List<CreateImageModel> GoodsNoList)
        {
            ClearGoodsGrid();

            //得到行数
            int Rows = GoodsNoList.Count;

            //GRID布局
            for (int i = 0; i < Rows; i++)
            {
                RowDefinition row = new RowDefinition();
                row.Height = new GridLength(304);
                GoodsGrid.RowDefinitions.Add(row);
            }

            ColumnDefinition col = new ColumnDefinition();
            col.Width = new GridLength(608);
            GoodsGrid.ColumnDefinitions.Add(col);

            #region
            //       <WrapPanel Grid.Row="1">

            //        <Canvas Width="608" Height="304">
            //            <Canvas.Background>
            //                <ImageBrush ImageSource="/Images/template/bg1.jpg"></ImageBrush>
            //            </Canvas.Background>
            //            <Canvas Width="240" Height="240" Margin="20 32 0 0">
            //                <Canvas.Background>
            //                    <ImageBrush ImageSource="https://gd4.alicdn.com/imgextra/i1/73900259/TB2qOMihYFlpuFjy0FgXXbRBVXa_!!73900259.jpg_240x240.jpg" TileMode="Tile" Stretch="UniformToFill" />
            //                </Canvas.Background>
            //            </Canvas>
            //            <Canvas Width="50" Height="50" Margin="15 27 0 0">
            //                <Canvas.Background>
            //                    <ImageBrush ImageSource="/Images/template/hot.png" TileMode="Tile" Stretch="UniformToFill" />
            //                </Canvas.Background>
            //            </Canvas>
            //            <Canvas Margin="288 165 0 0">
            //                <TextBlock Text="简约职业风的小西装，经典的同时又酝酿出女性的的优雅韵味" FontFamily="微软雅黑" Foreground="#3b3b3b" FontSize="20" Width="280" TextWrapping = "Wrap"></TextBlock>
            //            </Canvas>
            //        </Canvas>

            //</WrapPanel>
            #endregion

            //添加控件
            for (int i = 0; i < Rows; i++)
            {
                //-------------------生成模板

                WrapPanel wrap = new WrapPanel() { Tag = GoodsNoList[i].GoodsNo, Width = 608, Height = 304 };

                //背景图片
                Canvas topCanvas = new Canvas()
                {
                    Width = 608,
                    Height = 304,
                    Background = new ImageBrush()
                    {
                        ImageSource = new BitmapImage(new Uri(String.Format(@"{0}", "http://www.9myp.com/d/TemplateImage/bg1.jpg"), UriKind.Absolute)),
                    }
                };

                var ite = itemList.Where(x => x.OuterId.Equals(GoodsNoList[i].GoodsNo)).FirstOrDefault();
                ////商品图片 
                Canvas canvaImage = new Canvas()
                {
                    Width = 240,
                    Height = 240,
                    Margin = new Thickness(20, 32, 0, 0),
                    Background = new ImageBrush()
                    {

                        ImageSource = new BitmapImage(new Uri(String.Format(@"{0}", ite.ItemImgs.Count > 0 ? ite.ItemImgs[0].Url + "_240x240" : ""), UriKind.Absolute)),
                        Stretch = Stretch.UniformToFill,
                        TileMode = TileMode.Tile
                    }
                };

                topCanvas.Children.Add(canvaImage);

                ////热销图片
                Canvas hotImage = new Canvas()
                {
                    Width = 50,
                    Height = 50,
                    Margin = new Thickness(15, 27, 0, 0),
                    Background = new ImageBrush()
                    {
                        ImageSource = new BitmapImage(new Uri(String.Format(@"{0}", "http://www.9myp.com/d/TemplateImage/hot.png"), UriKind.Absolute)),
                        Stretch = Stretch.UniformToFill,
                        TileMode = TileMode.Tile
                    }
                };

                topCanvas.Children.Add(hotImage);

                ////产品描述
                Canvas goodsDesc = new Canvas()
                {
                    Margin = new Thickness(288, 165, 0, 0)
                };

                TextBlock t = new TextBlock()
                {
                    Text = GoodsNoList[i].Text, //"简约职业风的小西装，经典的同时又酝酿出女性的的优雅韵味",
                    FontFamily = new FontFamily("微软雅黑"),
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3b3b3b")),
                    FontSize = 20,
                    Width = 280,
                    TextWrapping = TextWrapping.Wrap
                };

                goodsDesc.Children.Add(t);
                topCanvas.Children.Add(goodsDesc);

                wrap.Children.Add(topCanvas);

                Border bs = new Border() { BorderThickness = new Thickness(1), BorderBrush = new SolidColorBrush(Colors.Gray), HorizontalAlignment = System.Windows.HorizontalAlignment.Center };
                Grid.SetRow(bs, i);
                Grid.SetColumn(bs, 0);
                bs.Child = wrap;
                GoodsGrid.Children.Add(bs);
            }
        }


        #endregion

        #region 新品评价 2017-04-05
        public void Evaluate(List<CreateImageModel> GoodsNoList)
        {
            #region
            //    <Canvas Width="304" Height="380">
            //    <Canvas.Background>
            //        <ImageBrush ImageSource="http://www.9myp.com/d/templateImage/bg2.jpg"></ImageBrush>
            //    </Canvas.Background>
            //    <Canvas Width="254" Height="254" Margin="25 30 0 0">
            //        <Canvas.Background>
            //            <ImageBrush ImageSource="https://gd4.alicdn.com/imgextra/i1/73900259/TB2qOMihYFlpuFjy0FgXXbRBVXa_!!73900259.jpg_240x240.jpg" TileMode="Tile" Stretch="UniformToFill" />
            //        </Canvas.Background>
            //    </Canvas>
            //    <Canvas Width="50" Height="50" Margin="15 27 0 0">
            //        <Canvas.Background>
            //            <ImageBrush ImageSource="http://www.9myp.com/d/templateImage/new2.png" TileMode="Tile" Stretch="UniformToFill" />
            //        </Canvas.Background>
            //    </Canvas>
            //    <Canvas Margin="40 290 0 0">
            //        <TextBlock Text="               可性感可优雅的大V领,穿上气质十足哦~" FontFamily="微软雅黑" Foreground="#3b3b3b" FontSize="18" Width="230" FontWeight="Bold"  TextWrapping = "Wrap"></TextBlock>
            //    </Canvas>
            //</Canvas>
            #endregion

            ClearGoodsGrid();
            //每行几列
            int Column = 6;
            //得到行数
            int Rows = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(GoodsNoList.Count) / Column));
            //GRID布局
            for (int i = 0; i < Rows; i++)
            {
                RowDefinition row = new RowDefinition();
                row.Height = new GridLength(380);
                GoodsGrid.RowDefinitions.Add(row);
            }
            for (int j = 0; j < Column; j++)
            {
                ColumnDefinition col = new ColumnDefinition();
                col.Width = new GridLength(304);
                GoodsGrid.ColumnDefinitions.Add(col);
            }

            //添加控件
            for (int i = 0; i < Rows; i++)
            {
                //-------------------生成模板

                for (int j = 0; j < Column; j++)
                {
                    int Index = (i * Column) + j;
                    if (Index == GoodsNoList.Count)
                    {
                        break;
                    }

                    WrapPanel wrap = new WrapPanel() { Tag = GoodsNoList[Index].GoodsNo, Width = 304, Height = 380 };

                    //背景图片
                    Canvas topCanvas = new Canvas()
                    {
                        Width = 304,
                        Height = 380,
                        Background = new ImageBrush()
                        {
                            ImageSource = new BitmapImage(new Uri(String.Format(@"{0}", "http://www.9myp.com/d/TemplateImage/bg2.jpg"), UriKind.Absolute)),
                        }
                    };
                    var ite = itemList.Where(x => x.OuterId.Equals(GoodsNoList[Index].GoodsNo)).FirstOrDefault();
                    ////商品图片 
                    Canvas canvaImage = new Canvas()
                    {
                        Width = 254,
                        Height = 254,
                        Margin = new Thickness(25, 30, 0, 0),
                        Background = new ImageBrush()
                        {
                            ImageSource = new BitmapImage(new Uri(String.Format(@"{0}", ite.ItemImgs.Count > 0 ? ite.ItemImgs[0].Url + "_240x240" : ""), UriKind.Absolute)),
                            Stretch = Stretch.UniformToFill,
                            TileMode = TileMode.Tile
                        }
                    };

                    topCanvas.Children.Add(canvaImage);

                    ////热销图片
                    Canvas newImage = new Canvas()
                    {
                        Width = 50,
                        Height = 50,
                        Margin = new Thickness(15, 27, 0, 0),
                        Background = new ImageBrush()
                        {
                            ImageSource = new BitmapImage(new Uri(String.Format(@"{0}", "http://www.9myp.com/d/TemplateImage/new2.png"), UriKind.Absolute)),
                            Stretch = Stretch.UniformToFill,
                            TileMode = TileMode.Tile
                        }
                    };

                    topCanvas.Children.Add(newImage);

                    ////产品描述
                    Canvas goodsDesc = new Canvas()
                    {
                        Margin = new Thickness(40, 290, 0, 0)
                    };

                    TextBlock t = new TextBlock()
                    {
                        Text = "               " + GoodsNoList[Index].Text, //"简约职业风的小西装，经典的同时又酝酿出女性的的优雅韵味",
                        FontFamily = new FontFamily("微软雅黑"),
                        Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3b3b3b")),
                        FontSize = 18,
                        Width = 230,
                        TextWrapping = TextWrapping.Wrap
                    };

                    goodsDesc.Children.Add(t);
                    topCanvas.Children.Add(goodsDesc);

                    wrap.Children.Add(topCanvas);

                    Border bs = new Border() { BorderThickness = new Thickness(1), BorderBrush = new SolidColorBrush(Colors.Gray), HorizontalAlignment = System.Windows.HorizontalAlignment.Center };
                    Grid.SetRow(bs, i);
                    Grid.SetColumn(bs, j);
                    bs.Child = wrap;
                    GoodsGrid.Children.Add(bs);
                }
            }
        }

        #endregion

        #region 模板下载
        private void Button_Download(object sender, RoutedEventArgs e)
        {

            String downloadURL = "http://www.9myp.com/d/mobileTemplate.xls";
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string Dir = dialog.SelectedPath;
                if (FileHelper.DownloadFile(downloadURL, System.IO.Path.Combine(Dir, "mobileTemplate.xls")))
                {
                    CCMessageBox.Show("模板已下载");
                }
                else
                {
                    CCMessageBox.Show("模板已下载失败");
                }

            }
        }

        #endregion

        #region 大促模板 2017-10-27

        public void MobileBigPromotionGridPannel(List<CreateImageModel> GoodsNoList)
        {
            ClearGoodsGrid();
            //每行几列
            int Column = 6;
            //得到行数
            int Rows = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(GoodsNoList.Count) / Column));
            //GRID布局
            for (int i = 0; i < Rows; i++)
            {
                RowDefinition row = new RowDefinition();
                row.Height = new GridLength(456);
                GoodsGrid.RowDefinitions.Add(row);
            }
            for (int j = 0; j < Column; j++)
            {
                ColumnDefinition col = new ColumnDefinition();
                col.Width = new GridLength(310);
                GoodsGrid.ColumnDefinitions.Add(col);
            }

            //添加控件
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Column; j++)
                {
                    try
                    {
                        int Index = (i * Column) + j;
                        if (Index == GoodsNoList.Count)
                        {
                            break;
                        }

                        //    <StackPanel  Background="#FFFFFF" Orientation="Vertical" Name="imageTempate" Height="420" Width="300">
                        //    <Image Width="300" Height="300" Name="img_box" Margin="0 0 0 0"></Image>
                        //    <Canvas  Margin="0,0,0,0" Height="120" Width="300">
                        //        <Label Content="30元" FontFamily="微软雅黑" FontSize="20" FontWeight="Bold" Margin="75 14 0 0"></Label>
                        //        <Label Content="30元" FontFamily="微软雅黑" FontSize="16" Margin="220 17 0 0"></Label>
                        //        <StackPanel Orientation="Horizontal" Margin="85 50 0 0" >
                        //            <Label Content="108" FontFamily="微软雅黑" FontSize="48" Margin="2 0 0 0" Foreground="#ffff00" FontWeight="Bold"></Label>
                        //            <Label Content=".00" FontFamily="微软雅黑" FontSize="28"  Foreground="#ffff00" FontWeight="Bold" VerticalAlignment="Bottom" Margin="-10 0 0 6"></Label>
                        //        </StackPanel>
                        //        <Canvas.Background>
                        //            <ImageBrush ImageSource="http://www.9myp.com/d/activity.png" TileMode="Tile" Stretch="UniformToFill" />
                        //        </Canvas.Background>
                        //    </Canvas>
                        //</StackPanel>
                        //-------------------生成模板

                        StackPanel stt = new StackPanel() { Orientation = Orientation.Vertical, Height = 456, Width = 304 };

                        WrapPanel wrap = new WrapPanel() { Background = new SolidColorBrush(Colors.White), Height = 456, Width = 304, Tag = GoodsNoList[Index].GoodsNo };

                        Image goodsimg = new Image()
                        {
                            Width = 300,
                            Height = 300,
                            Margin = new Thickness(2, 0, 0, 0),
                            Source = new BitmapImage(new Uri(String.Format(@"{0}", GoodsNoList[Index].GoodsUrl + "_300x300"), UriKind.Absolute)),
                        };

                        wrap.Children.Add(goodsimg);

                        Canvas canva = new Canvas()
                        {
                            Width = 304,
                            Height = 156,
                            Background = new ImageBrush()
                            {
                                ImageSource = new BitmapImage(new Uri(String.Format(@"{0}", GoodsNoList[Index].WaterImgUrl), UriKind.Absolute)),
                                Stretch = Stretch.UniformToFill,
                                TileMode = TileMode.Tile
                            }
                        };

                        //Label lab1 = new Label()
                        //{
                        //    FontSize = 20,
                        //    FontWeight = FontWeights.Bold,
                        //    Margin = new Thickness(78, 24, 0, 0),
                        //    FontFamily = new FontFamily("微软雅黑"),
                        //    Content = Convert.ToDecimal(Convert.ToDecimal(GoodsNoList[Index].DpPrice) - Convert.ToDecimal(GoodsNoList[Index].Price)).ToString("F1") + "元"
                        //};

                        //canva.Children.Add(lab1);

                        if (GoodsNoList[Index].DpPrice != "")
                        {
                            if (Convert.ToDecimal(GoodsNoList[Index].DpPrice) > 0)
                            {
                                Label lab2 = new Label()
                                {
                                    FontSize = 16,
                                    Margin = new Thickness(150, 16, 0, 0),
                                    FontFamily = new FontFamily("微软雅黑"),
                                   
                                    Content = "吊牌价 " + Convert.ToDecimal(GoodsNoList[Index].DpPrice).ToString("F1") + "元"
                                };
                                canva.Children.Add(lab2);
                            }
                        }

     
                        StackPanel sp = new StackPanel()
                        {

                            Orientation = System.Windows.Controls.Orientation.Horizontal,
                            Margin = new Thickness(88, 62, 0, 0)

                        };

                        string p = Convert.ToDecimal(GoodsNoList[Index].Price).ToString("F1");

                        Label lab3 = new Label()
                        {
                            FontSize = 48,
                            Margin = new Thickness(2, 0, 0, 0),
                            FontWeight = FontWeights.Bold,
                            FontFamily = new FontFamily("微软雅黑"),
                            Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ffff00")),
                            Content = p.Substring(0, p.IndexOf("."))
                        };

                        sp.Children.Add(lab3);

                        Label lab4 = new Label()
                        {
                            FontSize = 28,
                            Margin = new Thickness(-15, 0, 0, 6),
                            FontWeight = FontWeights.Bold,
                            FontFamily = new FontFamily("微软雅黑"),
                            Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ffff00")),
                            VerticalAlignment = System.Windows.VerticalAlignment.Bottom,
                            Content = p.Substring(p.IndexOf("."))
                        };

                        sp.Children.Add(lab4);

                        canva.Children.Add(sp);

                        wrap.Children.Add(canva);

                        stt.Children.Add(wrap);

                        Border bs = new Border() { BorderThickness = new Thickness(1), BorderBrush = new SolidColorBrush(Colors.Gray), HorizontalAlignment = System.Windows.HorizontalAlignment.Center };
                        bs.Child = stt;
                        Grid.SetRow(bs, i);
                        Grid.SetColumn(bs, j);
                        GoodsGrid.Children.Add(bs);
                    }
                    catch
                    { }
                }
            }


        }

        #endregion

        #region 图片模板选择

        private void comboxMobileItem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Com_Shop.SelectedIndex == 0)
            {
                CCMessageBox.Show("请选择店铺");
                return;
            }

            ShopModel shop = (ShopModel)Com_Shop.SelectedItem;
            string itemVal = comboxMobileItem.SelectedValue.ToString();
            bool isDefaultComboxVal = false;

            if (itemVal != "none")
            {

                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        if (itemVal == "bigPromotion")
                        {
                            filePahtItem.Visibility = System.Windows.Visibility.Visible;
                        }
                        else
                        {
                            filePahtItem.Visibility = System.Windows.Visibility.Collapsed;
                        }

                        switch (itemVal)
                        {
                            case "promotion":
                                #region 新模板 2016-10-22 BY WL

                                if (dt != null)
                                {
                                    if (dt.Columns.Count.Equals(5))
                                    {
                                        if (dt.Columns.Contains("款号") && dt.Columns.Contains("吊牌价") && dt.Columns.Contains("水印地址") && dt.Columns.Contains("文字"))
                                        {
                                            AsyncEventHandler asy = new AsyncEventHandler(this.PreviewImage);
                                            IAsyncResult ia = asy.BeginInvoke(null, null, dt, shop, false, null, null);
                                        }
                                    }
                                    else
                                    {
                                        CCMessageBox.Show("模板不匹配");
                                        isDefaultComboxVal = true;
                                    }
                                }
                                #endregion
                                break;
                            case "normal":
                                #region 旧模板

                                if (dt != null)
                                {
                                    if (string.IsNullOrEmpty(tb_youhui.Text))
                                    {
                                        CCMessageBox.Show("请填写优惠词");
                                        isDefaultComboxVal = true;
                                    }
                                    else
                                    {
                                        string YouHuiMsg = tb_youhui.Text;
                                        AsyncEventHandler asy = new AsyncEventHandler(this.PreviewImage);
                                        //异步调用开始，没有回调函数和AsyncState,都为null
                                        IAsyncResult ia = asy.BeginInvoke(YouHuiMsg, null, dt, shop, false, null, null);
                                    }
                                }

                                #endregion
                                break;
                            case "sys":
                                #region 系统推款
                                if (dt != null)
                                {
                                    if (string.IsNullOrEmpty(tb_youhui.Text))
                                    {
                                        CCMessageBox.Show("请填写优惠文字标题");
                                        isDefaultComboxVal = true;
                                    }
                                    else
                                    {
                                        string YouHuiMsg = tb_youhui.Text;
                                        // OnlineStoreAuthorization osa = CommonLogin.CommonUser.CShop;
                                        AsyncEventHandler asy = new AsyncEventHandler(NewPrieView);
                                        asy.BeginInvoke(YouHuiMsg, "", dt, shop, false, null, null);
                                    }
                                }
                                #endregion
                                break;
                            case "activity":
                                if (dt != null)
                                {
                                    AsyncEventHandler asy = new AsyncEventHandler(this.ActivityTemplatePre);
                                    IAsyncResult ia = asy.BeginInvoke(null, null, dt, shop, false, null, null);

                                }
                                break;
                            case "free":
                                if (dt != null)
                                {
                                    AsyncEventHandler asy = new AsyncEventHandler(this.ActivityTemplatePre);
                                    IAsyncResult ia = asy.BeginInvoke(null, null, dt, shop, false, null, null);

                                }
                                break;
                            case "new":
                                if (dt != null)
                                {
                                    AsyncEventHandler asy = new AsyncEventHandler(this.ActivityTemplatePre);
                                    IAsyncResult ia = asy.BeginInvoke(null, null, dt, shop, false, null, null);

                                }
                                break;
                            case "hot":
                                if (dt != null)
                                {
                                    AsyncEventHandler asy = new AsyncEventHandler(this.HotSaleView);
                                    IAsyncResult ia = asy.BeginInvoke(null, null, dt, shop, false, null, null);
                                }
                                break;
                            case "evaluate":
                                if (dt != null)
                                {
                                    AsyncEventHandler asy = new AsyncEventHandler(this.NewEvaluate);
                                    IAsyncResult ia = asy.BeginInvoke(null, null, dt, shop, false, null, null);
                                }
                                break;
                            case "none":
                                break;
                        }
                    }
                    else
                    {
                        CCMessageBox.Show("请先选择EXCEL模板");
                        isDefaultComboxVal = true;
                    }
                }
                else
                {
                    CCMessageBox.Show("请先选择EXCEL模板");
                    isDefaultComboxVal = true;
                }

                if (isDefaultComboxVal)
                {
                    comboxMobileItem.SelectedIndex = 0;
                }
            }
        }

        #endregion

        #region 生成图片
        private void Button_Click_CreateImage(object sender, RoutedEventArgs e)
        {
            List<WrapPanel> list = WPFControlsSearchHelper.GetChildObjects<WrapPanel>(GoodsGrid, "");
            if (list.Count == 0)
            {
                CCMessageBox.Show("没有可以保存的图片");
                return;
            }


            AsyncEventHandlerCreate asss = new AsyncEventHandlerCreate(this.CreateImage);
            //异步调用开始，没有回调函数和AsyncState,都为null
            IAsyncResult ias = asss.BeginInvoke(list, null, null);
        }
        #endregion

        #region 生成链接
        private void Button_Click_CreateLink(object sender, RoutedEventArgs e)
        {
            if (Com_Shop.SelectedIndex == 0)
            {
                CCMessageBox.Show("请选择店铺");
                return;
            }

            string ComboxVal = comboxLinkItem.SelectedValue.ToString();
            ShopModel shop = (ShopModel)Com_Shop.SelectedItem;
            switch (ComboxVal)
            {
                case "wireless":
                    RunExportUrl(true, shop);
                    break;
                case "pc":
                    RunExportUrl(false, shop);
                    break;
                case "none":
                    break;

            }
        }
        #endregion

        #region 生成HTML
        private void Button_Click_CreateHtml(object sender, RoutedEventArgs e)
        {
            string ComboxVal = comboxHtmlItem.SelectedValue.ToString();
            if (Com_Shop.SelectedIndex == 0)
            {
                CCMessageBox.Show("请选择店铺");
                return;
            }
            ShopModel shop = (ShopModel)Com_Shop.SelectedItem;
            switch (ComboxVal)
            {
                case "pc":
                    if (dt != null)
                    {
                        string YouHuiMsg = tb_youhui.Text;
                        string KaiQiangMsg = tb_kaiqiang.Text;

                        AsyncEventHandler asy = new AsyncEventHandler(CreateHTMLFile);
                        asy.BeginInvoke(YouHuiMsg, KaiQiangMsg, dt, shop, false, CreateHTMLFileCallBack, null);
                    }
                    break;
                case "9mg":
                    if (dt != null)
                    {
                        Ismg = true;
                        string YouHuiMsg = tb_youhui.Text;
                        string KaiQiangMsg = tb_kaiqiang.Text;
                        AsyncEventHandler asy = new AsyncEventHandler(CreateHTMLFile);

                        asy.BeginInvoke(YouHuiMsg, KaiQiangMsg, dt, shop, false, CreateHTMLFileCallBack, null);
                    }
                    break;
                case "jd":
                    if (dt != null)
                    {
                        string YouHuiMsg = tb_youhui.Text;
                        string KaiQiangMsg = tb_kaiqiang.Text;
                        AsyncEventHandler asy = new AsyncEventHandler(CreateHTMLFile);

                        asy.BeginInvoke(YouHuiMsg, KaiQiangMsg, dt, shop, true, CreateHTMLFileCallBack, null);
                    }
                    break;
                case "pcactivity":
                    if (dt != null)
                    {
                        //导入的LIST
                        List<CreateImageModel> list = new List<CreateImageModel>();
                        foreach (DataRow dr in dt.Rows)
                        {
                            CreateImageModel model = new CreateImageModel() { GoodsNo = dr["款号"].ToString(), Price = dr["价格"].ToString() };
                            list.Add(model);
                        }

                        List<string> rlist = list.Select(x => x.GoodsNo.Trim()).Distinct().ToList();
                        itemList = new TopSource(shop).GetItemList(rlist, "item_img.url,prop_img.url, pic_url,price,NumIid");

                        //导出的List
                        List<CustomItem> ExportList = new List<CustomItem>();

                        itemList.ForEach(delegate(Item i)
                        {
                            CreateImageModel m = list.Find(a => a.GoodsNo.Equals(i.OuterId));

                            CustomItem ci = new CustomItem();
                            ci.PicUrl = i.PicUrl;
                            ci.NewPrice = m.Price;
                            ci.OldPrice = Convert.ToDecimal(i.Price).ToString("F0");
                            ci.DiffPrice = Convert.ToDecimal(Convert.ToDecimal(ci.OldPrice) - Convert.ToDecimal(ci.NewPrice)).ToString("F0");
                            ci.DetailUrl = string.Format("http://item.taobao.com/item.htm?id={0}", i.NumIid);
                            ExportList.Add(ci);

                        });

                        EverydayUpdateModel ResultList = new EverydayUpdateModel();
                        ResultList.DateString = DateTime.Now.ToString("MM/dd");
                        ResultList.ItemList = ExportList;

                        string xmlEntity = ShopBll.Serialize(ResultList);
                        string res = ShopBll.Xslt(ShopType.CShop_9M, xmlEntity, ShopBll.GetTemplateDir_PcActivity);
                        if (!string.IsNullOrEmpty(res))
                        {
                            //保存文件
                            string file_name = file_name = string.Format("{0}_PC活动区模板1100", DateTime.Now.ToString("MM-dd"));
                            bool isSaveSuccess = ShopBll.InstanceEverydayUpdateBll().SaveFile(file_name, res.Replace(@"<b class=""bg"" >", @"<b class=""bg"" ></b>"), "EverydayUpdate");
                            if (isSaveSuccess)
                            {
                                CCMessageBox.Show("代码已生成");
                            }
                            else
                            {
                                CCMessageBox.Show("保存失败");
                            }

                        }
                    }
                    break;
                case "none":
                    break;

            }

        }
        #endregion

        #region HTML模板选择

        private void comboxHtmlItem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool isDefaultComboxVal = false;
            string itemVal = comboxHtmlItem.SelectedValue.ToString();

            if (itemVal != "none")
            {
                if (itemVal != "pcactivity")
                {
                    if (dt != null)
                    {
                        if (string.IsNullOrEmpty(tb_youhui.Text))
                        {
                            CCMessageBox.Show("请填写优惠文字标题");
                            isDefaultComboxVal = true;
                        }

                        if (itemVal == "9mg" || itemVal == "jd")
                        {
                            if (string.IsNullOrEmpty(tb_kaiqiang.Text))
                            {
                                CCMessageBox.Show("请填写开抢标题");
                                isDefaultComboxVal = true;
                            }
                        }
                    }
                    else
                    {
                        CCMessageBox.Show("请先选择EXCEL模板");
                        isDefaultComboxVal = true;
                    }
                }
            }

            if (isDefaultComboxVal)
            {
                comboxHtmlItem.SelectedIndex = 0;
            }
        }

        #endregion

        private void imageSelected(object sender, RoutedEventArgs e)
        {
            OpenFileDialog d = new OpenFileDialog();

            if (d.ShowDialog() == true)
            {
                txtfilePath.Text = d.FileName;
            }

            if (dt != null)
            {
                ShopModel shop = (ShopModel)Com_Shop.SelectedItem;
                AsyncEventHandler asy = new AsyncEventHandler(this.ActivityTemplatePre);
                IAsyncResult ia = asy.BeginInvoke(null, null, dt, shop, false, null, null);

            }

        }

        #region
        //private void CreateImage(string YouHuiMsg, string KaiQiangMsg, DataTable dt)
        //{
        //    //打开加载
        //    bar.LoadBar(true);
        //    //打开加载中的字符
        //    bar.Loading(true);
        //    //将TABLE转换成List
        //    list = new List<CreateImageModel>();
        //    foreach (DataRow dr in dt.Rows)
        //    {
        //        CreateImageModel model = new CreateImageModel() { GoodsNo = dr["款号"].ToString(), Price = Convert.ToDecimal(dr["价格"]).ToString("f2") };
        //        list.Add(model);
        //    }
        //    //获取当前用户桌面的路径
        //    string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
        //    //保存路径
        //    saveCodePath = desktopPath + string.Format(@"\{0}\", DateTime.Now.ToString("yyyyMMdd") + @"\" + System.Guid.NewGuid().ToString());
        //    //如果没有就创建文件
        //    if (!Directory.Exists(saveCodePath))
        //    {
        //        Application.Current.Dispatcher.BeginInvoke(new Action(() =>
        //        {
        //            Directory.CreateDirectory(saveCodePath);
        //        }));
        //    }
        //    //得到淘宝的数据集合
        //    OnlineStoreAuthorization osa = CommonLogin.CommonUser.CShop;
        //    Commodity com = new Commodity(osa.AppKey, osa.AppSecret, osa.SessionKey);
        //    itemList = com.GetItemList(list.Select(x => x.GoodsNo).Distinct().ToList(), "item_img.url,prop_img.url, pic_url", true);
        //    //TIMER加载图片问题需要加一个数据
        //    if (itemList.Count > 0)
        //    {
        //        itemList.Add(itemList[0]);
        //    }

        //    if (itemList.Count > 1)
        //    {
        //        timer.Start();
        //    }
        //    bar.Loading(false);

        //    //for (int i = 0; i < itemList.Count; i++)
        //    //{
        //    //    string Price = list.Where(x => x.GoodsNo.Equals(itemList[i].OuterId, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Price;


        //    //        //填入优惠词
        //    //        lab_youhui.Content = YouHuiMsg;
        //    //        //填入价格
        //    //        lab_price.Content = Price;
        //    //        //填入图片
        //    //        if (itemList[i].ItemImgs.Count > 0)
        //    //        {
        //    //            img_box.Source = new BitmapImage(new Uri(itemList[i].ItemImgs[0].Url, UriKind.RelativeOrAbsolute));
        //    //            ImageHelper.SaveToImage(imageTempate, saveCodePath + @"\" + itemList[i].OuterId + ".png");
        //    //        }

        //    //    //更新进度条
        //    //    int current = i == itemList.Count - 1 ? itemList.Count : i + 1;
        //    //    bar.UpdateBarValue(itemList.Count, current);
        //    //}
        //    //关闭进度条

        //}
        #endregion
    }

    public class CreateImageModel
    {
        public string GoodsNo { get; set; }
        public string Price { get; set; }
        public string DpPrice { get; set; }
        public string WaterImgUrl { get; set; }
        public string Text { get; set; }
        public string GoodsUrl { get; set; }
    }

    public class CombItem
    {
        public string text { get; set; }
        public string val { get; set; }
    }

    public class ComboxImageItem : ObservableCollection<CombItem>
    {
        public ComboxImageItem()
        {
            this.Add(new CombItem { text = "-手机图片模板选择-", val = "none" });
            this.Add(new CombItem { text = "促销模板", val = "promotion" });
            this.Add(new CombItem { text = "普通模板", val = "normal" });
            this.Add(new CombItem { text = "系统推款模板", val = "sys" });
            this.Add(new CombItem { text = "活动区模板", val = "activity" });
            this.Add(new CombItem { text = "买3免1模板", val = "free" });
            this.Add(new CombItem { text = "新品模板", val = "new" });
            this.Add(new CombItem { text = "热销新品二级页", val = "hot" });
            this.Add(new CombItem { text = "新品评价", val = "evaluate" });
            this.Add(new CombItem { text = "大促模板", val = "bigPromotion" });
        }
    }

    public class ComboxLinkItem : ObservableCollection<CombItem>
    {
        public ComboxLinkItem()
        {
            this.Add(new CombItem { text = "-链接模板选择-", val = "none" });
            this.Add(new CombItem { text = "生成无线端链接", val = "wireless" });
            this.Add(new CombItem { text = "PC端链接", val = "pc" });
        }
    }

    public class ComboxHtmlItem : ObservableCollection<CombItem>
    {
        public ComboxHtmlItem()
        {
            this.Add(new CombItem { text = "-HTML模板选择-", val = "none" });
            this.Add(new CombItem { text = "C店HTML", val = "pc" });
            this.Add(new CombItem { text = "天猫HTML", val = "9mg" });
            this.Add(new CombItem { text = "京东HTML", val = "jd" });
            this.Add(new CombItem { text = "C店活动HTML", val = "pcactivity" });
        }
    }
}
