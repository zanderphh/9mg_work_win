using _9M.Work.DbObject;
using _9M.Work.ErpApi;
using _9M.Work.Model;
using _9M.Work.TopApi;
using _9M.Work.Utility;
using _9M.Work.WPF_Main.FrameWork;
using _9Mg.Work.TopApi;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
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

namespace _9M.Work.WPF_Main.Views.EveryDayUpdate
{
    /// <summary>
    /// UpGoodsData.xaml 的交互逻辑
    /// </summary>
    public partial class UpGoodsData : UserControl
    {
        private BaseDAL dal = new BaseDAL();
        private GoodsManager manager = new GoodsManager();
        private TopSource shopdetail = new TopSource();
        private List<string> GoodsList;
        private string ImageUrl = string.Empty;
        //委托类
        public delegate void UpGoodsHandler(List<string> GoodsList);
        public UpGoodsData()
        {
            InitializeComponent();
            LoadUrl();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int Tag = Convert.ToInt32((sender as Button).Tag);
            switch (Tag)
            {
                case 0:
                    DataTable dt = GlobalUtil.ReadExcel(new OpenFileDialog());
                    if (dt == null)
                    {
                        CCMessageBox.Show("请选择Excel");
                        return;
                    }
                    GoodsList = new List<string>();
                    foreach (DataRow dr in dt.Rows)
                    {
                        GoodsList.Add(dr[0].ToString());
                    }
                    CCMessageBox.Show("共导入【" + GoodsList.Count + "】件商品");
                    break;
                case 1:
                    System.Windows.Forms.FolderBrowserDialog op = new System.Windows.Forms.FolderBrowserDialog();
                    if (op.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        tb_imageurl.Text = op.SelectedPath;
                    }
                    break;
                case 2:
                    System.Windows.Forms.FolderBrowserDialog ops = new System.Windows.Forms.FolderBrowserDialog();
                    if (ops.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        tb_usrurl.Text = ops.SelectedPath;
                    }
                    break;
            }

        }

        private int selectedindex = 0;
        private string list_time = string.Empty;
        private string imgurl = string.Empty;
        private string userurl = string.Empty;
        private bool IsWanlan;
        private void Btn_GetData(object sender, RoutedEventArgs e)
        {
            selectedindex = Com_GoodsStatus.SelectedIndex;
            if (selectedindex == 1 && listpick.SelectedDate == null)
            {
                CCMessageBox.Show("请选择一个定时上架的时间");
                return;
            }
            if (selectedindex == 1)
            {
                //上架时间
                list_time = listpick.SelectedDate.ToString();
            }
            imgurl = tb_imageurl.Text;
            userurl = tb_usrurl.Text;
            IsWanlan = (bool)chk_wanless.IsChecked;
            if (string.IsNullOrEmpty(imgurl) || string.IsNullOrEmpty(userurl))
            {
                CCMessageBox.Show("完整的填写目录");
                return;
            }
            if (GoodsList != null)
            {

                RemeberUrl(imgurl, userurl);
                UpGoodsHandler uh = new UpGoodsHandler(GetCsv);
                IAsyncResult ar = uh.BeginInvoke(GoodsList, null, null);

            }

        }

        private void GetCsv(List<string> GoodsList)
        {
            try
            {
                //打开加载
                bar.LoadBar(true);
                //打开加载中的字符
                bar.Loading(true);
                List<BrandModel> brandlist = dal.GetAll<BrandModel>(); //品牌集合
                List<string[]> ls = new List<string[]>();
                string[] firstrow = new string[] { "version 1.00" };
                string[] secondrow = new string[] { "title", "cid", "seller_cids", "stuff_status", "location_state", "location_city", "item_type", "price", "auction_increment", "num", "valid_thru", "freight_payer", "post_fee", "ems_fee", "express_fee", "has_invoice", "has_warranty", "approve_status", "has_showcase", "list_time", "description", "cateProps", "postage_id", "has_discount", "modified", "upload_fail_msg", "picture_status", "auction_point", "picture", "video", "skuProps", "inputPids", "inputValues", "outer_id", "propAlias", "auto_fill", "num_id", "local_cid", "navigation_type", "user_name", "syncStatus", "is_lighting_consigment", "is_xinpin", "foodparame", "features", "buyareatype", "global_stock_type", "global_stock_country", "sub_stock_type", "item_size", "item_weight", "sell_promise", "custom_design_flag", "wireless_desc", "barcode", "sku_barcode", "newprepay", "subtitle", "cpv_memo", "input_custom_cpv", "qualification", "add_qualification", "o2o_bind_service" };
                string[] thirdrow = new string[] { "宝贝名称", "宝贝类目", "店铺类目", "新旧程度", "省", "城市", "出售方式", "宝贝价格", "加价幅度", "宝贝数量", "有效期", "运费承担", "平邮", "EMS", "快递", "发票", "保修", "放入仓库", "橱窗推荐", "开始时间", "宝贝描述", "宝贝属性", "邮费模版ID", "会员打折", "修改时间", "上传状态", "图片状态", "返点比例", "新图片", "视频", "销售属性组合", "用户输入ID串", "用户输入名-值对", "商家编码", "销售属性别名", "代充类型", "数字ID", "本地ID", "宝贝分类", "用户名称", "宝贝状态", "闪电发货", "新品", "食品专项", "尺码库", "采购地", "库存类型", "国家地区", "库存计数", "物流体积", "物流重量", "退换货承诺", "定制工具", "无线详情", "商品条形码", "sku 条形码", "7天退货", "宝贝卖点", "属性值备注", "自定义属性值", "商品资质", "增加商品资质", "关联线下服务" };
                ls.Add(firstrow);
                ls.Add(secondrow);
                ls.Add(thirdrow);

                string title = string.Empty; //宝贝名称
                long cid = 0;  //宝贝类目
                               //  string seller_cids = "391775800,391775799,391775801,391775802,";//店铺类目
                string seller_cids = "391775800";
                string stuff_status = "1"; //新旧程度
                string location_state = "湖北"; //省
                string location_city = "武汉";  //城市
                string item_type = "1"; //出售方式
                double price = 0; //宝贝价格
                string auction_increment = string.Empty;//加价幅度
                long num = 0; //宝贝数量
                string valid_thru = "7";//有效期
                int freight_payer = 2;//运费承担
                string post_fee = "0.00"; //平邮
                string ems_fee = "0.00"; //EMS
                string express_fee = "0.00";//快递
                string has_invoice = "0";//发票
                string has_warranty = "0";//保修
                int approve_status = 1;//放入仓库
                string has_showcase = "0";//橱窗推荐
                                          // string list_time = string.Empty; //开始时间
                string description = string.Empty;//宝贝描述
                string cateProps = string.Empty; //宝贝属性
                long postage_id = 1683490;//邮费模版ID
                int has_discount = 1;//会员打折
                DateTime modified = DateTime.Now;  //修改时间
                string upload_fail_msg = "200"; //上传状态
                string picture_status = "2;2;2;2;2;";//图片状态
                string auction_point = "0";//返点比例
                string video = string.Empty; //视频
                string inputPids = string.Empty; //用户输入ID串
                string inputValues = string.Empty;//用户输入名-值对
                string outer_id = string.Empty; //商家编码
                string propAlias = string.Empty; //销售属性别名
                string auto_fill = "0";//代充类型
                string num_id = "0";//数字ID
                string local_cid = "0";//本地ID;
                string navigation_type = "1"; //宝贝分类
                string user_name = "magic_girls";//用户名称
                string syncStatus = "2";//宝贝状态
                string is_lighting_consigment = "24"; //闪电发货
                string is_xinpin = "243"; //新品
                string foodparame = "product_date_end:;product_date_start:;stock_date_end:;stock_date_start:";//食品专项
                string features = "mysize_tp:-1;sizeGroupId:136553091;sizeGroupType:women_tops"; //尺码库
                string buyareatype = "0";//采购地
                string global_stock_type = "-1";//库存类型
                string global_stock_country = string.Empty; //国家地区
                string sub_stock_type = string.Empty; //库存记数
                string item_size = "bulk:0.000000";//物流体积
                string item_weight = "0"; //物流重量
                string sell_promise = "1"; //退换货承诺
                string custom_design_flag = string.Empty; //制定工具
                string barcode = string.Empty; //商品条码
                string sku_barcode = string.Empty; //SKu条形码
                string newprepay = "1"; //7天退货

                string cpv_memo = string.Empty; //属性值备注
                string qualification = string.Empty; //商品资质
                string add_qualification = "0"; //增加商品资质
                string o2o_bind_service = "0"; // 关联线下服务
                                               //加载类目库
                List<CategoryModel> categorylist = dal.GetAll<CategoryModel>();
                //开始读取进度条
                bar.Loading(false);
                for (int i = 0; i < GoodsList.Count; i++)
                {

                    string GoodsNo = GoodsList[i].ToUpper();
                    //得到款号的详情
                    GoodsModel gm = manager.GoodsInformation(GoodsNo);

                    //得到SPEC的详情
                    List<SpecModel> splist = manager.SpecList(GoodsNo);
                    #region CSV数据

                    //类目

                    CategoryModel cm = categorylist.Where(x => x.CategoryName.Equals(gm.ClassName)).FirstOrDefault();
                    cid = cm != null ? cm.TbCid : 0;
                    //价格
                    price = GoodsHelper.GetMaxPrice(gm.Reserved3);
                    //数量
                    num = splist.Sum(x => x.Stock);

                    //生成描述
                    description = GlobalUtil.Desc(GoodsNo);
                    //商家编码
                    outer_id = GoodsList[i];
                    //标题
                    title = GoodsList[i];

                    #region 卖点

                    string subtitle = string.Empty; //宝贝卖点
                    string BrandEn = GoodsHelper.BrandEn(GoodsNo);
                    foreach (BrandModel b in brandlist)
                    {
                        if (b.BrandEN.Equals(BrandEn, StringComparison.CurrentCultureIgnoreCase))
                        {
                            subtitle = b.SellPoint;
                            break;
                        }
                    }
                    #endregion

                    #region 上架状态

                    approve_status = selectedindex == 0 ? 1 : 2;

                    #endregion

                    #region 销售属性
                    string input_custom_cpv = "";//自定义属性
                    string skuProps = string.Empty; //销售属性组合
                    if (cid > 0)
                    {
                        //得到淘宝的颜色和尺码
                        List<PropValue> colorprop = shopdetail.GetPropDetail(cid, "1627207:0");
                        List<PropValue> sizeprop = shopdetail.GetPropDetail(cid, string.Format("{0}:0", cm.TbSkuSizeProp));

                        if (colorprop == null || sizeprop == null)
                        {
                            continue;
                        }
                        //统一淘宝的尺码表
                        //统一淘宝的尺码表
                        sizeprop.ForEach(x =>
                        {
                            if (x.Name.Equals("XXL"))
                            {
                                x.Name = "2XL";
                            }
                            else if (x.Name.Equals("XXXL"))
                            {
                                x.Name = "3XL";
                            }
                        });

                        for (int j = 0; j < splist.Count; j++)
                        {
                            if (splist[j].SpecName2.Equals("XXL"))
                            {
                                splist[j].SpecName2 = "2XL";
                            }
                            if (splist[j].Equals("XXXL"))
                            {
                                splist[j].SpecName2 = "3XL";
                            }
                        }
                        var colorlist = splist.GroupBy(x => x.SpecName1).Select(x => x.Key).ToList();
                        var sizelist = splist.GroupBy(x => x.SpecName2).Select(x => x.Key).ToList();
                        Dictionary<string, long> SizeDic = new Dictionary<string, long>();


                        //匹配颜色尺码
                        int colorcutomid = -1000; //自定义颜色ID
                        int sizecutomid = -1000;   //自定义尺码ID
                        bool flag = true;
                        colorlist.ForEach(x =>
                        {
                            long colorvid = 0;
                            long dicvid = 0;
                            long sizevid = 0;
                            string skuouterid = string.Empty;
                            int stock = 0;
                            PropValue prop = colorprop.Find(z =>
                            {
                                return z.Name.Replace("色", "").Equals(x.Replace("色", ""));
                            });
                            //得到颜色的VID(如果是自定义颜色那么自定义颜色ID减1)
                            if (prop != null)
                            {
                                colorvid = prop.Vid;
                            }
                            else
                            {
                                colorcutomid = colorcutomid - 1;
                                colorvid = colorcutomid;
                                input_custom_cpv += string.Format("1627207:{0}:{1};", colorcutomid, x);
                            }
                            sizelist.ForEach(y =>
                            {
                                if (flag == true) //尺码只执行一次
                                {
                                    //匹配尺码的VID
                                    PropValue sprop = sizeprop.Find(s =>
                                        {
                                            return y.ToUpper().Equals(s.Name);
                                        });
                                    if (sprop != null)
                                    {
                                        dicvid = sprop.Vid;
                                    }
                                    else
                                    {
                                        sizecutomid = sizecutomid - 1;
                                        dicvid = sizecutomid;
                                        input_custom_cpv += string.Format("{2}:{0}:{1};", colorcutomid, y, cm.TbSkuSizeProp);
                                    }
                                    SizeDic.Add(y, dicvid);
                                }

                                //匹配编码与库存价格
                                SpecModel model = splist.Find(h =>
                                    {
                                        return h.SpecName1.Equals(x) && h.SpecName2.Equals(y);
                                    });
                                if (model != null)
                                {
                                    stock = model.Stock;
                                    skuouterid = model.GoodsNo + model.SpecCode;
                                }
                                sizevid = SizeDic[y];
                                //70:20:61SC122612:1627207:3594022;20509:28314;
                                skuProps += string.Format("{0}:{1}:{2}:1627207:{3};{5}:{4};", price, stock, skuouterid, colorvid, sizevid, cm.TbSkuSizeProp);
                            });
                            flag = false;
                        });
                    }
                    #endregion

                    #region 橱窗图
                    string picture = string.Empty;//新图片
                    ImageUrl = imgurl;//得到目录的图片
                    List<string> imagelist = Directory.GetFiles(ImageUrl).ToList().Where(x => x.ToUpper().Contains(GoodsNo)).OrderBy(x => x).ToList();
                    string ZhuLiUrl = userurl + @"\Images\";//得到淘宝助理的目录
                    for (int j = 0; j < imagelist.Count; j++)
                    {
                        string Guid = System.Guid.NewGuid().ToString();
                        File.Copy(imagelist[j], ZhuLiUrl + Guid + ".jpg", true); //移动图片到助理
                        picture += Guid + ":1:" + j + ":|;";
                    }
                    #endregion

                    #region 无线描述图
                    string wireless_desc = string.Empty;  //无线详情
                    if (IsWanlan == true)
                    {
                        string imageappend = string.Empty;
                        string WanLessFile = userurl + @"\Images4wap\"; //得到淘宝助理的无线图片路径
                        List<string> wanlist = GlobalUtil.DescImageList(GoodsList[i]); //得到描述图片
                        wanlist.ForEach(x =>
                        {
                            if (HttpHelper.GetHead(x).Equals("200"))
                            {
                                string Guid = System.Guid.NewGuid().ToString();
                                System.Drawing.Image image = FileHelper.ReadImageFormUrl(x);
                                int Height = (int)(620 / Convert.ToDouble(image.Width) * image.Height);
                                string newimageurl = WanLessFile + Guid + ".jpg";
                                bool b = ImageWatermark.GetPicThumbnail(image, newimageurl, Height, 620, 80);
                                if (b)
                                {
                                    imageappend += string.Format("<img>{0}</img>", newimageurl);
                                }
                            }
                        });
                        wireless_desc = string.Format("<wapDesc>{0}</wapDesc>", imageappend);
                    }
                    #endregion

                    #endregion
                    string[] arry = new string[] { title, cid.ToString(), seller_cids, stuff_status.ToString(), location_state, location_city, item_type, price.ToString(), auction_increment, num.ToString(), valid_thru, freight_payer.ToString(), post_fee, ems_fee, express_fee, has_invoice, has_warranty, approve_status.ToString(), has_showcase, list_time, description, cateProps, postage_id.ToString(), has_discount.ToString(), modified.ToString(), upload_fail_msg, picture_status, auction_point, picture, video, skuProps, inputPids, inputValues, outer_id, propAlias, auto_fill, num_id, local_cid, navigation_type, user_name, syncStatus, is_lighting_consigment, is_xinpin, foodparame, features, buyareatype, global_stock_type, global_stock_country, sub_stock_type, item_size, item_weight, sell_promise, custom_design_flag, wireless_desc, barcode, sku_barcode, newprepay, subtitle, cpv_memo, input_custom_cpv, qualification, add_qualification, o2o_bind_service };
                    ls.Add(arry);
                    //更新进度条
                    int current = i == GoodsList.Count - 1 ? GoodsList.Count : i + 1;
                    bar.UpdateBarValue(GoodsList.Count, current);

                }
               // string FileName = DateTime.Now.ToShortTimeString().Replace(":", "");
                ExcelNpoi.WriteCSV(string.Format(@"{0}\Go.Csv", Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)), ls);
                //关闭进度条
                bar.LoadBar(false);
                GoodsList = null;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void LoadUrl()
        {
            //读取记住的密码
            RegistryKey regKey = Registry.CurrentUser.OpenSubKey("UpGoodsConfig");
            if (regKey != null)
            {
                tb_imageurl.Text = regKey.GetValue("imageurl") != null ? regKey.GetValue("imageurl").ToString() : "";
                tb_usrurl.Text = regKey.GetValue("userurl") != null ? regKey.GetValue("userurl").ToString() : "";
                regKey.Close();
            }
        }

        private void RemeberUrl(string imageurl, string userurl)
        {
            //保存密码
            RegistryKey regKey = Registry.CurrentUser.CreateSubKey("UpGoodsConfig");
            regKey.SetValue("imageurl", imageurl);
            regKey.SetValue("userurl", userurl);
            regKey.Close();
        }
    }
}
