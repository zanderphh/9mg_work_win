using _9M.Work.DbObject;
using _9M.Work.ErpApi;
using _9M.Work.Model;
using _9M.Work.TopApi;
using _9M.Work.Utility;
using _9M.Work.WPF_Common.WpfBind;
using _9M.Work.YouZan;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
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

namespace _9M.Work.WPF_Main.Views.EveryDayUpdate
{
    /// <summary>
    /// WeiDianUpGoods.xaml 的交互逻辑
    /// </summary>
    public partial class WeiDianUpGoods : UserControl, INotifyPropertyChanged
    {
        private GoodsManager manager = new GoodsManager();
        private TopSource top = new TopSource();
        private YzApi api = new YzApi();
        private delegate Dictionary<string, object> Handler(object[] parm);
        private GoodsManager goodsmanager = new GoodsManager();
        private List<GoodsTag> TagList;
        private List<BrandModel> BrandList;
        private BaseDAL dal = new BaseDAL();
        public WeiDianUpGoods()
        {
            InitializeComponent();
            BindPostComboBox();
            BindGooodsTagsComboBox();
            GoodsList = new ObservableCollection<GridSourceModel>();
            GoodsGrid.ItemsSource = GoodsList;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        ObservableCollection<GridSourceModel> _goodslist;
        public ObservableCollection<GridSourceModel> GoodsList
        {
            get
            {
                return this._goodslist;
            }
            set
            {
                if (this._goodslist != value)
                {
                    this._goodslist = value;
                    OnPropertyChanged("GoodsList");
                }
            }
        }

        public void BindGooodsTagsComboBox()
        {
            var list = api.CategoriesTags();
            list.Insert(0, new GoodsTag() { id = 0, name = "请选择" });
            ComboBoxBind.BindComboBox(com_goodstags, list, "name", "id");
            com_goodstags.SelectedIndex = 0;
        }

        public void BindPostComboBox()
        {
            List<EnumEntity> list = new List<EnumEntity>();
            list.Add(new EnumEntity() { Text = "请选择", Value = 0 });
            list.Add(new EnumEntity() { Text = "普通模版", Value = 398373 });
            list.Add(new EnumEntity() { Text = "包邮模版", Value = 399613 });

            ComboBoxBind.BindComboBox(com_post, list, "Text", "Value");
            com_post.SelectedIndex = 1;
        }

        string BrandVal = string.Empty;
        string YearSeasonVal = string.Empty;
        string StockVal = string.Empty;
        string GoodsCountVal = string.Empty;
        string GoodsNosVal = string.Empty;

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            BrandVal = tb_brand.Text.Trim();
            YearSeasonVal = tb_yearseason.Text.Trim();
            StockVal = tb_stock.Text.Trim();
            GoodsCountVal = tb_goodscount.Text.Trim();

            object[] obj = null;
            if (!string.IsNullOrEmpty(GoodsCountVal))
            {
                obj = new object[4];
                obj[0] = BrandVal;
                obj[1] = YearSeasonVal;
                obj[2] = StockVal;
                obj[3] = GoodsCountVal;
                Handler handler = new Handler(ExportGoods);
                handler.BeginInvoke(obj, GoodsCallBack, null);
            }
        }

        private Dictionary<string, object> ExportGoods(object[] parm)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            try
            {
                bar.LoadBar(true);
                bar.Loading(true);
                //加载微店商品
                DataTable dt = new DataTable();
                dt.Columns.Add("款号");
                List<GoodsDetail> glist = new List<GoodsDetail>();
                glist.AddRange(api.OnLineGoodsList(true, "outer_id,tag_ids"));
                glist.AddRange(api.OnLineGoodsList(false, "outer_id,tag_ids"));
                var g = string.Join("\n", glist.Where(x => x.tag_ids.Contains("94842099")).Select(x => x.outer_id));
                int GoodsCount = 200;
                string SqlWhere = string.Empty;
                string sql = @"select Price1, GoodsNo,Stock,Brand,ClassName from  (select Price1, ClassName, goodsno, dbo.GET_STR(GoodsNO) as brand,dbo.Get_Year(GoodsNO)+dbo.Get_Season(GoodsNO) as [yearseason] ,CONVERT(int, stock) as stock from
            (select Price1,c.ClassName, a.GoodsNO,SUM(b.Stock-b.OrderCount-b.SndCount) as stock from G_Goods_GoodsList a 
            join G_Stock_Spec b on a.GoodsID = b.GoodsID 
            join G_Goods_GoodsClass c on a.ClassID = c.ClassID
            where len(a.goodsno)>7 and (b.Stock-b.OrderCount-b.SndCount)>0
            group by a.GoodsNO,a.price1,c.ClassName ) a) b
            where 1=1  ";
                if (!string.IsNullOrEmpty(parm[0].ToString()))
                {
                    SqlWhere += string.Format(@" and Brand = '{0}'", parm[0].ToString());
                }
                if (!string.IsNullOrEmpty(parm[1].ToString()))
                {
                    SqlWhere += string.Format(@" and yearseason = '{0}'", parm[1].ToString());
                }
                if (!string.IsNullOrEmpty(parm[2].ToString()))
                {
                    SqlWhere += string.Format(@" and stock > {0}", parm[2].ToString());
                }
                GoodsCount = Convert.ToInt32(parm[3]);
                sql += SqlWhere;

                #region wl 2017-05-26 替换下列方法
                //string val = goodsmanager.JsonForSql(System.Web.HttpUtility.UrlEncode(sql));
                #endregion

                //string val = _9M.Work.ErpApi.WdgjSource.CallDataJsonString(sql);
                //var Gjlist = JsonConvert.DeserializeObject<List<GjModel>>(val);

                var Gjlist = _9M.Work.ErpApi.CallWdgjServer.CallData<GjModel>(sql);

                //删除微店己经上过架的商品
                Gjlist.RemoveAll(x => glist.Select(z => z.outer_id).Contains(x.GoodsNo));

                int PageNo = (int)Math.Ceiling(Convert.ToDouble(Gjlist.Count) / 40);
                int UpCount = 0;
                for (int i = 1; i <= PageNo; i++)
                {
                    if (UpCount > GoodsCount)
                    {
                        break;
                    }
                    //取款号
                    List<string> goodslist = Gjlist.Select(x => x.GoodsNo).Skip((i - 1) * 40).Take(40).ToList();
                    //取淘宝ID
                    List<string> numiidlist = top.GetItemList(goodslist, string.Empty).Select(x => x.NumIid.ToString()).ToList();
                    //查询商品
                    var itemlist = top.GetItemListDetail(numiidlist, "item_img.url,prop_img.url, pic_url,desc,post_fee");
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        foreach (var x in itemlist)
                        {
                            UpCount++;
                            if (UpCount > GoodsCount)
                            {
                                break;
                            }
                            DataRow dr = dt.NewRow();
                            dr["款号"] = x.OuterId;
                            dt.Rows.Add(dr);
                        }
                    }));
                }
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                ExcelNpoi.TableToExcelForXLS(dt, desktopPath + @"\微店导出款.xls");
                dic.Add("status", "OK");
            }
            catch (Exception ex)
            {
                dic.Add("status", ex.Message);
            }
            finally
            {
                bar.LoadBar(false);
                bar.Loading(false);
            }
            return dic;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            object[] obj = null;
            GoodsNosVal = rich_GoodsNos.Text.Trim();
            if (!string.IsNullOrEmpty(GoodsNosVal))
            {
                obj = new object[1];
                obj[0] = GoodsNosVal;
                GoodsList.Clear();
                Handler handler = new Handler(InitGoods);
                handler.BeginInvoke(obj, GoodsCallBack, null);
            }
        }

        private void GoodsCallBack(IAsyncResult ar)
        {
            Handler handler = (Handler)((AsyncResult)ar).AsyncDelegate;
            var dic = handler.EndInvoke(ar);
            if (!dic["status"].Equals("OK"))
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    CCMessageBox.Show(dic["status"].ToString());
                }));
            }
        }

        private Dictionary<string, object> InitGoods(object[] parm)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            try
            {
                bar.LoadBar(true);
                bar.Loading(true);
                TagList = api.CategoriesTags();
                BrandList = dal.GetAll<BrandModel>();
                //上架数量默认最多200
                int GoodsCount = 200;
                //加载微店商品
                List<GoodsDetail> glist = new List<GoodsDetail>();
                glist.AddRange(api.OnLineGoodsList(true, "outer_id"));
                glist.AddRange(api.OnLineGoodsList(false, "outer_id"));
                string SqlWhere = string.Empty;
                //加载管家商品
                string sql = @"select Price1, GoodsNo,Stock,Brand,ClassName from  (select Price1, ClassName, goodsno, dbo.GET_STR(GoodsNO) as brand,dbo.Get_Year(GoodsNO)+dbo.Get_Season(GoodsNO) as [yearseason] ,CONVERT(int, stock) as stock from
            (select Price1,c.ClassName, a.GoodsNO,SUM(b.Stock-b.OrderCount-b.SndCount) as stock from G_Goods_GoodsList a 
            join G_Stock_Spec b on a.GoodsID = b.GoodsID 
            join G_Goods_GoodsClass c on a.ClassID = c.ClassID
            where len(a.goodsno)>7 and (b.Stock-b.OrderCount-b.SndCount)>0
            group by a.GoodsNO,a.price1,c.ClassName ) a) b
            where 1=1  ";

                var ls = parm[0].ToString().Replace("\r", "").Split('\n').Where(x => !string.IsNullOrEmpty(x)).Distinct().Select(x => x.Trim()).ToList();
                SqlWhere += string.Format(@"and GoodsNo in ('{0}')", string.Join(",", ls).Replace(",", "','"));

                sql += SqlWhere;

                #region wl 2017-05-26 替换下列方法
                //string val = goodsmanager.JsonForSql(System.Web.HttpUtility.UrlEncode(sql));
                #endregion

                //string val = _9M.Work.ErpApi.WdgjSource.CallDataJsonString(sql);
                //var Gjlist = JsonConvert.DeserializeObject<List<GjModel>>(val);

                var Gjlist = _9M.Work.ErpApi.CallWdgjServer.CallData<GjModel>(sql);
          
               

                //删除微店己经上过架的商品
                Gjlist.RemoveAll(x => glist.Select(z => z.outer_id).Contains(x.GoodsNo));

                int PageNo = (int)Math.Ceiling(Convert.ToDouble(Gjlist.Count) / 40);
                int UpCount = 0;
                for (int i = 1; i <= PageNo; i++)
                {
                    if (UpCount > GoodsCount)
                    {
                        break;
                    }
                    //取款号
                    List<string> goodslist = Gjlist.Select(x => x.GoodsNo).Skip((i - 1) * 40).Take(40).ToList();
                    //取淘宝ID
                    List<string> numiidlist = top.GetItemList(goodslist, string.Empty).Select(x => x.NumIid.ToString()).ToList();
                    //查询商品
                    var itemlist = top.GetItemListDetail(numiidlist, "item_img.url,prop_img.url, pic_url,desc,post_fee");
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        foreach (var x in itemlist)
                        {
                            try
                            {
                                UpCount++;
                                if (UpCount > GoodsCount)
                                {
                                    break;
                                }
                                //删除指定图片
                                if (x.ItemImgs.Count == 6)
                                {
                                    x.ItemImgs.RemoveAt(5);
                                    x.ItemImgs.RemoveAt(3);
                                }
                                else
                                {
                                    x.ItemImgs.RemoveAt(3);
                                }
                                //组装标题
                                string Title = string.Empty;
                                //去除描述的a标签
                                string regval = @"<a(?:(?!href=).)*href=(['""]?)(?<url>[^""'\s>]*)\1[^>]*>(?<text>(?:(?!</a>).)*)</a>";
                                var gjModel = Gjlist.Where(z => z.GoodsNo.Equals(x.OuterId)).First();
                                //匹配品牌TAGS
                                string categoryTagValues = string.Empty;
                                string categoryTagTexts = string.Empty;
                                var br = BrandList.Where(z => z.BrandEN.Equals(gjModel.Brand)).FirstOrDefault();
                                if (br != null)
                                {
                                    var ta = TagList.Where(z => z.name.Contains(br.BrandCN)).FirstOrDefault();
                                    if (ta != null)
                                    {
                                        categoryTagValues += ta.id + ",";
                                        categoryTagTexts += ta.name + ",";
                                        Title += ta.name; //标题加上品牌
                                    }
                                }

                                //标题加上年份季节
                                string flag = gjModel.GoodsNo.Substring(0, 2);
                                if (flag[0].ToString().Equals("7"))
                                {
                                    Title += "201" + flag[0].ToString();
                                }

                                switch (flag[1].ToString())
                                {
                                    case "1":
                                        Title += "春装";
                                        break;
                                    case "2":
                                        Title += "夏装";
                                        break;
                                    case "3":
                                        Title += "秋装";
                                        break;
                                    case "4":
                                        Title += "冬装";
                                        break;
                                }

                                //标题追加分类
                                Title += gjModel.ClassName;
                                //标题追加款号
                                Title += GoodsHelper.TrueGoodsNo(gjModel.GoodsNo, true);

                                //匹配分类TAGS
                                var qita = TagList.Where(z => z.name.Equals("其它")).First();

                                if (gjModel.ClassName.Contains("蕾丝衫") || gjModel.ClassName.Contains("雪纺衫"))
                                {
                                    gjModel.ClassName = "蕾丝衫/雪纺衫";
                                }
                                var tb = TagList.Where(z => z.name.Equals(gjModel.ClassName)).FirstOrDefault();
                                if (tb != null)
                                {
                                    categoryTagValues += tb.id + ",";
                                    categoryTagTexts += tb.name + ",";
                                }
                                else
                                {
                                    categoryTagValues += qita.id + ",";
                                    categoryTagTexts += qita.name + ",";
                                }
                                categoryTagValues = categoryTagValues.TrimEnd(',');
                                categoryTagTexts = categoryTagTexts.TrimEnd(',');


                                GoodsList.Add(new GridSourceModel()
                                {
                                    PosttempLate = new EnumEntity() { Text = "普通模版", Value = 398373 },
                                    CategoryText = categoryTagTexts,
                                    CategoryValue = categoryTagValues,
                                    FyiPrice = gjModel.Price1.ToString("f2"),
                                    GoodsNo = x.OuterId,
                                    PostFee = x.PostFee,
                                    Title = Title,
                                    Price = x.Price,
                                    UpStatus = "放入仓库",
                                    Images = x.ItemImgs.Select(z => z.Url + "_100x100").ToList(),
                                    Desc = Regex.Replace(x.Desc, regval, ""),
                                });

                            }
                            catch (Exception)
                            {
                                int ks = 1;
                            }
                        }
                    }));
                }
                dic.Add("status", "OK");
            }
            catch (Exception ex)
            {
                dic.Add("status", ex.Message);
            }
            finally
            {
                bar.Loading(false);
                bar.LoadBar(false);
            }
            return dic;
        }

        //批量修改
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            int PostTempLateId = Convert.ToInt32(com_post.SelectedValue);
            string Text = RadioBind.ReadSelectedContent(wrap_Radio);

            string order = tb_imgorder.Text;

            foreach (var goods in GoodsList)
            {
                if (!order.Equals("1234"))
                {
                    List<string> newimglist = new List<string>();
                    List<string> imglist = goods.Images;
                    foreach (var item in order)
                    {
                        newimglist.Add(imglist[Convert.ToInt32(item.ToString()) - 1]);
                    }
                    goods.Images = newimglist;
                }
                if (!string.IsNullOrEmpty(tb_descprefix.Text))
                {
                    goods.Desc = tb_descprefix.Text + goods.Desc;
                }
                goods.UpStatus = Text;
                goods.PosttempLate = com_post.SelectedItem as EnumEntity;
                if (com_goodstags.SelectedIndex > 0)
                {
                    GoodsTag g = com_goodstags.SelectedItem as GoodsTag;
                    if (!goods.CategoryValue.Split(',').Contains(g.id.ToString()))
                    {

                        goods.CategoryText += "," + g.name;
                        goods.CategoryValue += "," + g.id;
                    }
                }
            }
        }

        //上架
        int DescImageCount = 0;
        bool IsOlnySku = true;
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (GoodsList.Count > 0)
            {
                bool b = false;
                if (GoodsList[0].CategoryText.Split(',').Length < 3)
                {
                    if (CCMessageBox.Show("没有追加商品组,是否继续上架", CCMessageBoxButton.OKCancel) == CCMessageBoxResult.OK)
                    {
                        b = true;
                    }
                }
                else
                {
                    b = true;
                }
                if (b)
                {
                    DescImageCount = Convert.ToInt32(tb_desccount.Text);
                    IsOlnySku = Convert.ToBoolean(chk_skucheck.IsChecked);
                    object[] obj = new object[] { GoodsList };
                    Handler handler = new Handler(UpGoods);
                    handler.BeginInvoke(obj, UpGoodsCallBack, null);
                }
            }
        }

        private void UpGoodsCallBack(IAsyncResult ar)
        {
            Handler handler = (Handler)((AsyncResult)ar).AsyncDelegate;
            var dic = handler.EndInvoke(ar);
            Dispatcher.BeginInvoke(new Action(() =>
            {
                GoodsList.Clear();
                if (!dic["status"].Equals("OK"))
                {
                    CCMessageBox.Show(dic["status"].ToString());
                }
            }));
        }

        private Dictionary<string, object> UpGoods(object[] parm)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            bar.LoadBar(true);
            try
            {
                var list = parm[0] as ObservableCollection<GridSourceModel>;
                //得到管家的SKU
                for (int i = 0; i < list.Count; i++)
                {
                    GridSourceModel model = list[i];
                    //得到管家的SKU
                    List<SpecModel> splist = manager.SpecList(model.GoodsNo);
                    List<WeiDianskus> skulist = new List<WeiDianskus>();
                    splist.ForEach(x =>
                    {
                        WeiDianskus sku = new WeiDianskus()
                        {
                            sku_outer_id = x.GoodsNo + x.SpecCode,
                            sku_price = Convert.ToDecimal(model.Price),
                            sku_property = new sku_property() { 颜色 = x.SpecName1, 尺码 = x.SpecName2 },
                            sku_quantity = x.Stock < 0 ? 0 : x.Stock,
                        };
                        skulist.Add(sku);
                    });
                    if (IsOlnySku)
                    {
                        skulist = skulist.Where(z => z.sku_quantity > 0).ToList();
                    }

                    Itemparm p = new Itemparm()
                    {
                        tag_ids = model.CategoryValue,
                        quantity = skulist.Sum(x => x.sku_quantity).ToString(),
                        auto_listing_time = model.UpStatus.Equals("直接上架") ? "0" : "1",
                        delivery_template_id = model.PosttempLate.Value.ToString(),
                        desc = model.Desc,
                        images = model.Images,
                        is_display = model.UpStatus.Equals("直接上架") ? "1" : "0",
                        outer_id = model.GoodsNo,
                        post_fee = model.PostFee,
                        price = model.Price,
                        title = model.Title,
                        origin_price = model.FyiPrice,
                        skus_with_json = JsonConvert.SerializeObject(skulist),
                        DescImageCount = DescImageCount,
                    };
                    bool up = api.AddItem(p);
                    Thread.Sleep(500);
                    bar.UpdateBarValue(list.Count, i + 1);
                }
                dic.Add("status", "OK");
            }
            catch (Exception ex)
            {
                dic.Add("status", ex.Message);
            }
            finally
            {
                bar.LoadBar(false);

            }
            return dic;
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {

        }
    }

    public class GjModel
    {
        public string GoodsNo { get; set; }
        public int Stock { get; set; }
        public decimal Price1 { get; set; }
        public string Brand { get; set; }
        public string ClassName { get; set; }
    }

    public class WeiDianskus
    {
        public sku_property sku_property { get; set; }
        public decimal sku_price { get; set; }
        public int sku_quantity { get; set; }
        public string sku_outer_id { get; set; }

    }

    public class sku_property
    {
        public string 颜色 { get; set; }
        public string 尺码 { get; set; }
    }

    public class GridSourceModel : INotifyPropertyChanged
    {

        public GridSourceModel()
        {
            PosttempLate = new EnumEntity();
        }
        public string GoodsNo { get; set; }

        private EnumEntity _posttempLate;
        public EnumEntity PosttempLate
        {
            get { return _posttempLate; }
            set
            {
                if (_posttempLate != value)
                {
                    _posttempLate = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("PosttempLate"));
                }
            }
        }

        private string _categoryText;
        public string CategoryText
        {
            get { return _categoryText; }
            set
            {
                if (_categoryText != value)
                {
                    _categoryText = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("CategoryText"));
                }
            }
        }

        private string _categoryValue;
        public string CategoryValue
        {
            get { return _categoryValue; }
            set
            {
                if (_categoryValue != value)
                {
                    _categoryValue = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("CategoryValue"));
                }
            }
        }




        private string _upStatus;
        public string UpStatus
        {
            get { return _upStatus; }
            set
            {
                if (_upStatus != value)
                {
                    _upStatus = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("UpStatus"));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private int _goodsCount;
        public int GoodsCount
        {
            get { return _goodsCount; }
            set
            {
                if (_goodsCount != value)
                {
                    _goodsCount = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("GoodsCount"));
                }
            }
        }


        private string _desc;
        public string Desc
        {
            get { return _desc; }
            set
            {
                if (_desc != value)
                {
                    _desc = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("Desc"));
                }
            }
        }

        private string _title;
        public string Title
        {
            get { return _title; }
            set
            {
                if (_title != value)
                {
                    _title = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("Title"));
                }
            }
        }

        private string _price;
        public string Price
        {
            get { return _price; }
            set
            {
                if (_price != value)
                {
                    _price = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("Price"));
                }
            }
        }

        private string _fyiPrice;
        public string FyiPrice
        {
            get { return _fyiPrice; }
            set
            {
                if (_fyiPrice != value)
                {
                    _fyiPrice = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("FyiPrice"));
                }
            }
        }

        private string _postFee;
        public string PostFee
        {
            get { return _postFee; }
            set
            {
                if (_postFee != value)
                {
                    _postFee = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("PostFee"));
                }
            }
        }

        private List<string> _images;
        public List<string> Images
        {
            get { return _images; }
            set
            {
                if (_images != value)
                {
                    _images = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("Images"));
                }
            }
        }
    }
}
