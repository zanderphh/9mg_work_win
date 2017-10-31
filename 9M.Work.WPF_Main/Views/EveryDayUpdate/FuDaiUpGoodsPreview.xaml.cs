using _9M.Work.WPF_Main.Infrastrcture;
using System;
using System.Collections.Generic;
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
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;
using _9M.Work.Model;
using System.Collections.ObjectModel;
using _9M.Work.Utility;
using _9M.Work.DbObject;
using Top.Api.Domain;
using _9M.Work.TopApi;
using System.IO;
using _9M.Work.WPF_Main.FrameWork;
using Microsoft.Win32;
using _9M.Work.ErpApi;
using _9M.Work.Model.WdgjWebService;

namespace _9M.Work.WPF_Main.Views.EveryDayUpdate
{
    /// <summary>
    /// FuDaiUpGoodsPreview.xaml 的交互逻辑
    /// </summary>
    public partial class FuDaiUpGoodsPreview : UserControl, BaseDialog
    {
        private string userurl = string.Empty;//助理路径
        private BaseDAL dal = new BaseDAL();
        private ObservableCollection<FuDaiGoodsModel> dataSource;
        List<FuDaiGoodsModel> list;
        List<CategoryModel> categorylist;
        private TopSource top = new TopSource();
        public FuDaiUpGoodsPreview(List<FuDaiGoodsModel> list)
        {
            this.DataContext = this;
            this.list = list;
            dataSource = new ObservableCollection<FuDaiGoodsModel>();
            InitializeComponent();
            BindGrid();
            categorylist = dal.GetAll<CategoryModel>();
            LoadUrl();
        }

        private void LoadUrl()
        {
            //读取记住的密码
            RegistryKey regKey = Registry.CurrentUser.OpenSubKey("UpGoodsConfig");
            if (regKey != null)
            {
                // tb_imageurl.Text = regKey.GetValue("imageurl") != null ? regKey.GetValue("imageurl").ToString() : "";
                userurl = regKey.GetValue("userurl") != null ? regKey.GetValue("userurl").ToString() : "";
                regKey.Close();
            }
        }

        public void BindGrid()
        {
            dataSource.Clear();
            FuDaiGoodsGridlist.ItemsSource = null;
            FuDaiGoodsGridlist.ItemsSource = dataSource;
            list.ForEach(x =>
            {
                dataSource.Add(x);
            });
        }

        public DelegateCommand CancelCommand
        {
            get
            {
                return new DelegateCommand(CloseDialog);
            }
        }

        public string Title
        {
            get
            {
                return "上架预览";
            }
        }

        public void CloseDialog()
        {
            FormInit.CloseDialog(this);
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            throw new NotImplementedException();
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            throw new NotImplementedException();
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            throw new NotImplementedException();
        }

        //上架
        private void Button_Click(object sender, RoutedEventArgs e)
        {
          

            //排序生成CSV
            List<FuDaiGoodsModel> realList = list.OrderBy(x => x.Brand).ThenBy(x => Convert.ToInt32(x.SpecCode.Remove(x.SpecCode.Length - 1, 1))).ToList();
            GetCsv(realList);
        }

        public void GetCsv(List<FuDaiGoodsModel> list)
        {
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
            string list_time = string.Empty; //开始时间
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


            // string GoodsNo = "FD000000";

            #region CSV数据

            //类目

            CategoryModel cm = categorylist.Where(x => x.CategoryName.Equals("连衣裙")).FirstOrDefault();
            cid = cm != null ? cm.TbCid : 0;
            //价格
            price = Convert.ToDouble(list.Max(x => x.Price));
            //数量
            num = list.Count;

            //生成描述
            description = GlobalUtil.FuDaiDesc(list);
            //商家编码
            outer_id = "FD000000";
            //标题
            title = "FD000000";

            #region 卖点

            string subtitle = "这是一个好宝贝"; //宝贝卖点

            #endregion

            #region 上架状态

            approve_status = 1;

            #endregion

            #region 销售属性
            string input_custom_cpv = "";//自定义属性
            string skuProps = string.Empty; //销售属性组合
            if (cid > 0)
            {
                //List<SpecModel> splist = list.Select(x => new SpecModel
                //{
                //    SpecName1 = x.Brand,
                //    SpecName2 = x.Size,
                //    GoodsNo = x.GoodsNo,
                //    SpecCode = x.SpecCode,
                //    Stock =1,
                //    //这里转换对象GoodsName作为价格
                //    GoodsName = x.Price.ToString(),
                //}).ToList() as List<SpecModel>;
                //得到淘宝的颜色和尺码
                List<PropValue> colorprop = top.GetPropDetail(cid, "1627207:0");
                List<PropValue> sizeprop = top.GetPropDetail(cid, string.Format("{0}:0", cm.TbSkuSizeProp));
                for (int i = 0; i < sizeprop.Count; i++)
                {
                    if (sizeprop[i].Name == "2XL")
                    {
                        sizeprop[i].Name = "XXL";
                    }
                }

                //var colorlist = splist.GroupBy(x => x.SpecName1).Select(x => x.Key).ToList();
                // var sizelist = list.GroupBy(x => x.Size).Select(x => x.Key).ToList();
                var sizelist = new List<string>() { "S", "M", "L", "XL", "XXL" };
                Dictionary<string, long> SizeDic = new Dictionary<string, long>();


                //匹配颜色尺码
                int colorcutomid = -1000; //自定义颜色ID
                int sizecutomid = -1000;   //自定义尺码ID
                bool flag = true;
                list.ForEach(x =>
                {
                    long colorvid = 0;
                    long dicvid = 0;
                    long sizevid = 0;
                    string skuouterid = string.Empty;
                    int stock = 0;

                    colorcutomid = colorcutomid - 1;
                    colorvid = colorcutomid;
                    string Color = x.Brand + Convert.ToInt64(x.GoodsNo.Replace("FD", "")) + "-" + x.SpecCode.Remove(x.SpecCode.Length - 1, 1);
                    input_custom_cpv += string.Format("1627207:{0}:{1};", colorcutomid, Color);
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

                        if (x.Size == y)
                        {
                            stock = 1;
                            skuouterid = x.GoodsNo + x.SpecCode;
                        }
                        else
                        {
                            stock = 0;
                            skuouterid = "";
                        }
                        sizevid = SizeDic[y];
                        //70:20:61SC122612:1627207:3594022;20509:28314;
                        skuProps += string.Format("{0}:{1}:{2}:1627207:{3};{5}:{4};", x.Price, stock, skuouterid, colorvid, sizevid, cm.TbSkuSizeProp);
                    });
                    flag = false;
                });
            }
            #endregion

            #region 橱窗图
            string picture = string.Empty;//新图片
            //ImageUrl = imgurl;//得到目录的图片
            //List<string> imagelist = Directory.GetFiles(ImageUrl).ToList().Where(x => x.ToUpper().Contains(GoodsNo)).OrderBy(x => x).ToList();
            //string ZhuLiUrl = userurl + @"\Images\";//得到淘宝助理的目录
            //for (int j = 0; j < imagelist.Count; j++)
            //{
            //    string Guid = System.Guid.NewGuid().ToString();
            //    File.Copy(imagelist[j], ZhuLiUrl + Guid + ".jpg", true); //移动图片到助理
            //    picture += Guid + ":1:" + j + ":|;";
            //}
            #endregion

            #region 无线描述图
            bool IsWanlan = false;
            string wireless_desc = string.Empty;  //无线详情
            if (IsWanlan == true)
            {
                if (string.IsNullOrEmpty(userurl))
                {
                    CCMessageBox.Show("请到上新货品设置助理路径");
                    return;
                }
                string imageappend = string.Empty;
                string WanLessFile = userurl + @"\Images4wap\"; //得到淘宝助理的无线图片路径
                List<string> wanlist = GlobalUtil.FuDaiImageUrl(list); //得到描述图片
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
            string Desk = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            ExcelNpoi.WriteCSV(Desk + @"\FuDai.Csv", ls);
            CCMessageBox.Show("FuDai.CSV己生成在桌面");
            CloseDialog();
        }
    }
}
