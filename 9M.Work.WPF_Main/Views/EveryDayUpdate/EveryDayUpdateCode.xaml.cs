using _9M.Work.DbObject;
using _9M.Work.JosApi;
using _9M.Work.Model;
using _9M.Work.TopApi;
using _9M.Work.Utility;
using _9M.Work.WPF_Common;
using _9M.Work.WPF_Common.WpfBind;
using _9Mg.Work.TopApi;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Text.RegularExpressions;
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
using System.Xml;
using Top.Api.Domain;

namespace _9M.Work.WPF_Main.Views.EveryDayUpdate
{
    /// <summary>
    /// EveryDayUpdateCode.xaml 的交互逻辑
    /// </summary>
    public partial class EveryDayUpdateCode : UserControl, INotifyPropertyChanged
    {
        List<ShopModel> shoplist = null;
        private BaseDAL dal = new BaseDAL();
        private ShopBll Bll;
        //记录是获取默认价格还是自定义价格
        private PriceType priceType;
        //委托类
        public delegate List<string> DGGetGoodsList(string FilePath, Goods type);
        //上传文件名
        private string FileName = string.Empty;
        //标记是否上传文件
        private bool isUploadSuccess = false;

        //异步委托
        public delegate string AsyncEventHandler(string TemplatePath, WareManager wareutil, Dictionary<string, string> dic, bool IsCutomPrice);

        //异步委托
        public delegate string AsyncEventHandlerAino(string TemplatePath, TopSource wareutil, Dictionary<string, string> dic, bool IsCutomPrice);
        /* -----------------------------------------   专题款   --------------------------------------------------- */
        //上传文件里面的款
        private List<string> subjectList;

        //更新进度条委托
        private delegate void UpdateProgressBarDelegate(System.Windows.DependencyProperty dp, Object value);
        public EveryDayUpdateCode()
        {
            Bll = ShopBll.InstanceEverydayUpdateBll();
            InitializeComponent();
            BindShop();
            this.DataContext = this;     //进度条必须加载
        }

        public void BindShop()
        {
            shoplist = dal.GetList<ShopModel>(x => x.isHaveApi == true);
            shoplist.Insert(0, new ShopModel() { shopId = 0, shopName = "请选择" });
            ComboBoxBind.BindComboBox(Com_Shop, shoplist, "shopName", "shopId");
            Com_Shop.SelectedIndex = 0;
        }
        #region PublicMethod


        //生成京东的上新代码
        public string GetCode(string TemplatePath, WareManager wareutil, Dictionary<string, string> dic, bool IsCutomPrice)
        {
            bar.LoadBar(true);
            bar.Loading(true);
            string Res = string.Empty;
            try
            {
                Res = new TemplateUtil().NewCode(TemplatePath, wareutil, dic, IsCutomPrice);
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
            return Res;
        }

        //生成京东的上新代码
        public string GetAiNoCode(string TemplatePath, TopSource wareutil, Dictionary<string, string> dic, bool IsCutomPrice)
        {
            bar.LoadBar(true);
            bar.Loading(true);
            string Res = string.Empty;
            try
            {
                Res = new TemplateUtil().NewCode(TemplatePath, wareutil, dic, IsCutomPrice);
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
            return Res;
        }


        /// <summary>
        /// 生成回调
        /// </summary>
        /// <param name="ar"></param>
        public void CodeCallBack(IAsyncResult ar)
        {
            AsyncEventHandler handler = (AsyncEventHandler)((AsyncResult)ar).AsyncDelegate;
            string Code = handler.EndInvoke(ar);
            if (!string.IsNullOrEmpty(Code))
            {
                //保存文件
                bool isSaveSuccess = Bll.SaveFile("上新_" + DateTime.Now.ToString("MM-dd"), Code, "JdEverydayUpdate");
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    CCMessageBox.Show("生成成功");
                }));

            }
        }

        /// <summary>
        /// 生成回调
        /// </summary>
        /// <param name="ar"></param>
        public void CodeAinoCallBack(IAsyncResult ar)
        {
            AsyncEventHandlerAino handler = (AsyncEventHandlerAino)((AsyncResult)ar).AsyncDelegate;
            string Code = handler.EndInvoke(ar);
            if (!string.IsNullOrEmpty(Code))
            {
                //保存文件
                bool isSaveSuccess = Bll.SaveFile("上新_" + DateTime.Now.ToString("MM-dd"), Code, "AinoEverydayUpdate");
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    CCMessageBox.Show("生成成功");
                }));

            }
        }
        #endregion

        #region Event

        //上新款生成
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            int tag = Convert.ToInt32(btn.Tag);


            if (tag.Equals(1))
            {

                try
                {
                    OpenFileDialog ofp = new OpenFileDialog();
                    //过滤器
                    ofp.Filter = "XLS|*.xls|XLSX|*.xlsx|TXT|*.txt";
                    if (ofp.ShowDialog() == true) //选择完成之后
                    {
                        //检查是否有数据
                        //int count = Bll.GetTableByFilePath(ofp.FileName).Rows.Count;
                        //if (count < 0)
                        //{
                        //    CCMessageBox.Show("没有找到数据");
                        //    return;
                        //}
                        //记录上传文件路径
                        FileName = ofp.FileName;
                        //标记成功
                        isUploadSuccess = true;
                        CCMessageBox.Show("导入成功");
                    }
                }
                catch (Exception ex)
                {
                    CCMessageBox.Show(ex.Message);
                }
            }
            else if (tag > 10) //上新确定
            {
                try
                {
                    if (System.IO.Path.GetExtension(FileName).ToLower().Equals(".txt") && chk_setprice.IsChecked == true)
                    {
                        CCMessageBox.Show("Txt文件不支持自定义价格");
                        return;
                    }
                    //如果有数据才往下执行
                    if (!isUploadSuccess)
                    {
                        CCMessageBox.Show("请选择文件");
                        return;
                    }

                    ShopType shopType = ComboBoxBind.ShopTypeByCmb(Com_Shop);
                    //创建委托
                    DGGetGoodsList dg = new DGGetGoodsList(Bll.GetGoodsnoList);
                    //获取合格款
                    List<string> goodList = dg.Invoke(FileName, Goods.Good);

                    //获取不合格款
                    List<string> badList = dg.Invoke(FileName, Goods.Bad);
                    //根据不同的key来，获取不同店铺的数据
                    // OnlineStoreAuthorization osa = ComboBoxBind.GetAuthorization(Com_Shop);
                    ShopModel shops = shoplist.Where(x => x.shopId == Convert.ToInt32(Com_Shop.SelectedValue)).FirstOrDefault();
                    //如果是京东
                    ShopModel entity = (ShopModel)Com_Shop.SelectedItem;
                    if (entity.shopId == 1023)
                    {
                        Dictionary<string, string> dic = new Dictionary<string, string>();
                        DataTable table = ExcelNpoi.ExcelToDataTable("sheet1", true, FileName);
                        foreach (DataRow dr in table.Rows)
                        {
                            string Price = table.Columns.Count == 1 ? "" : Convert.ToDouble(dr["价格"]).ToString("f2");
                            dic.Add(dr["款号"].ToString(), Price);
                        }
                        WareManager wareutil = new WareManager(shops.appKey, shops.appSecret, shops.sessionKey);
                        string TemplatePath = System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + "\\templates\\JdEverydayUpdate.txt";
                        bool IsCutomPrice = Convert.ToBoolean(chk_setprice.IsChecked);
                        //string Code = new TemplateUtil().NewCode(TemplatePath, wareutil, dic, IsCutomPrice);
                        //实例委托
                        AsyncEventHandler asy = new AsyncEventHandler(this.GetCode);
                        //异步调用开始，没有回调函数和AsyncState,都为null
                        IAsyncResult ia = asy.BeginInvoke(TemplatePath, wareutil, dic, IsCutomPrice, CodeCallBack, null);
                    }
                    else if (entity.shopId == 1027) //如果是爱侬
                    {
                        Dictionary<string, string> dic = new Dictionary<string, string>();
                        DataTable table = ExcelNpoi.ExcelToDataTable("sheet1", true, FileName);
                        foreach (DataRow dr in table.Rows)
                        {
                            string Price = table.Columns.Count == 1 ? "" : Convert.ToDouble(dr["价格"]).ToString("f2");
                            dic.Add(dr["款号"].ToString(), Price);
                        }
                        TopSource com = new TopSource(shops);
                        string TemplatePath = System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + "\\templates\\AinoEverydayUpdate.txt";
                        bool IsCutomPrice = Convert.ToBoolean(chk_setprice.IsChecked);
                        //string Code = new TemplateUtil().NewCode(TemplatePath, wareutil, dic, IsCutomPrice);
                        //实例委托
                        AsyncEventHandlerAino asy = new AsyncEventHandlerAino(this.GetAiNoCode);
                        //异步调用开始，没有回调函数和AsyncState,都为null
                        IAsyncResult ia = asy.BeginInvoke(TemplatePath, com, dic, IsCutomPrice, CodeAinoCallBack, null);
                    }
                    else //淘宝上新
                    {
                        TopSource com = new TopSource(shops);
                        List<Item> tempList = com.GetItemList(goodList, "item_img.url,prop_img.url, pic_url");
                        //按导入的顺序排序
                        List<Item> itemList = new List<Item>();
                        foreach (string s in goodList)
                        {
                            itemList.Add(tempList.Find(a => a.OuterId.Equals(s)));
                        }


                        //如果是自定义价格
                        if (chk_setprice.IsChecked == true) priceType = PriceType.Custom;
                        List<UploadExcelModel> list = Bll.GetGoodsnoAndPriceList(FileName, goodList, chk_setprice);
                        List<CustomItem> customItemsList = Bll.ConvertToCustomItem(itemList, list, shopType, priceType);


                        //添加连续序号
                        int Serial = 1;
                        foreach (CustomItem c in customItemsList)
                        {
                            c.SerialNo = Serial.ToString();
                            if (tag == 11 || tag == 12)
                            {
                                //c.FivePicUrl = "http://tb.9mg.cn/design/2016/fx/" + DateTime.Now.ToString("yyyyMMdd") + "ZT" + "/" + c.OuterId + ".jpg";
                            }
                            else
                            {
                                c.FivePicUrl = "http://tb.9mg.cn/design/2016/fx/" + DateTime.Now.ToString("yyyyMMdd") + "ZT" + "/" + c.OuterId + ".jpg";
                            }
                            Serial++;
                        }

                        //加载model数据
                        EverydayUpdateModel model = new EverydayUpdateModel();
                        model.DateString = DateTime.Now.ToString("MM/dd");
                        model.ItemList = customItemsList;
                        int mod = model.ItemList.Count % 4;
                        //拼凑完整行(4个单元格为一行)
                        for (int i = 0; i < 4 - mod; i++)
                        {
                            model.ItemList.Add(new CustomItem());
                        }

                        //Model转换XML
                        string res = string.Empty;
                        if (tag.Equals(11))
                        {
                            string xmlEntity = ShopBll.Serialize(model);
                            res = ShopBll.Xslt(shopType, xmlEntity, ShopBll.GetTemplateDir_EverydayUpdateForCShop);
                        }
                        else if (tag.Equals(12))//专题1100 16-05-11
                        {
                            string xmlEntity = ShopBll.Serialize(model);
                            res = ShopBll.Xslt(shopType, xmlEntity, ShopBll.GetTemplateDir_EverydayUpdateForCShopByZT);
                        }
                        else if (tag.Equals(13))//分销专题 16-05-11
                        {

                            Dictionary<string, string> dicts = new Dictionary<string, string>();
                            //重新编辑XML便于分销专题模板(table布局)xslt的查找
                            EverydayUpdateModelByFXtable m = new EverydayUpdateModelByFXtable();
                            m.DateString = model.DateString;
                            m.ItemList = new List<List<CustomItem>>();
                            //序号：4个为一组
                            int index = 1;
                            List<CustomItem> lc = new List<CustomItem>();
                            foreach (CustomItem c in model.ItemList)
                            {
                                //为拼凑的单元格赋值
                                if (c.OuterId == null)
                                { c.OuterId = System.Guid.NewGuid().ToString(); }

                                dicts.Add(c.OuterId, c.PicUrl);
                                lc.Add(c);
                                if (index < 4) { index++; } else { m.ItemList.Add(lc); index = 1; lc = new List<CustomItem>(); }
                            }

                            downloadFileByHTTPHandle handle = new downloadFileByHTTPHandle(downloadFileByHTTP);
                            handle.BeginInvoke(dicts, null, null);

                            string xmlEntity = ShopBll.Serialize(m);
                            res = ShopBll.Xslt(shopType, xmlEntity, ShopBll.GetTemplateDir_EverydayUpdateForFXShopByZT);
                        }
                        else if (tag.Equals(14))
                        {
                            Dictionary<string, string> olddic = new Dictionary<string, string>();
                            Dictionary<string, string> dic = new Dictionary<string, string>();
                            DataTable table = ExcelNpoi.ExcelToDataTable("sheet1", true, FileName);
                            foreach (DataRow dr in table.Rows)
                            {
                                String Price = "";
                                String RealPrice = "";
                                if (table.Columns.Count > 1)
                                {
                                    String p = dr["价格"].ToString().Contains(".") ? dr["价格"].ToString().Substring(dr["价格"].ToString().IndexOf(".") + 1) : dr["价格"].ToString();
                                    Price = (p == "00" || p == "0") ? Convert.ToDecimal(dr["价格"]).ToString("F0") : Convert.ToDecimal(dr["价格"]).ToString("F1");

                                    String RP = dr["吊牌价"].ToString().Contains(".") ? dr["吊牌价"].ToString().Substring(dr["吊牌价"].ToString().IndexOf(".") + 1) : dr["吊牌价"].ToString();
                                    RealPrice = (p == "00" || p == "0") ? Convert.ToDecimal(dr["吊牌价"]).ToString("F0") : Convert.ToDecimal(dr["吊牌价"]).ToString("F1");

                                    dic.Add(dr["款号"].ToString(), Price);
                                    olddic.Add(dr["款号"].ToString(), RealPrice);
                                }
                            }

                            model.ItemList.ForEach(delegate(CustomItem c)
                            {
                                c.NewPrice = dic.ToList().Find(a => a.Key.Equals(c.OuterId)).Value;
                                //原价用表格的原价
                                c.OldPrice = olddic.ToList().Find(a => a.Key.Equals(c.OuterId)).Value;
                            });

                            string xmlEntity = ShopBll.Serialize(model);
                            res = ShopBll.Xslt(shopType, xmlEntity, ShopBll.GetTemplateDir_EverydayUpdateForCShopByCX);
                        }

                        if (!string.IsNullOrEmpty(res))
                        {
                            //保存文件
                            string file_name = string.Empty;
                            if (tag.Equals(12))
                            {
                                file_name = string.Format("{0}_专题1100", DateTime.Now.ToString("MM-dd"));
                            }
                            else if (tag.Equals(13))
                            {
                                file_name = string.Format("{0}_专题(分销)950", DateTime.Now.ToString("MM-dd"));
                            }
                            else if (tag.Equals(14))
                            {
                                file_name = string.Format("{0}_促销1100", DateTime.Now.ToString("MM-dd"));
                            }
                            else
                            {
                                file_name = string.Format("{0}_上新", DateTime.Now.ToString("MM-dd"));
                            }
                            bool isSaveSuccess = Bll.SaveFile(file_name, res.Replace(@"<b class=""bg"" >", @"<b class=""bg"" ></b>"), "EverydayUpdate");
                            if (isSaveSuccess)
                            {
                                //不合格款
                                string badGoodsStr = string.Empty;
                                if (badList.Count > 0)
                                {
                                    foreach (string s in badList)
                                    {
                                        badGoodsStr += s + ",";
                                    }
                                    badGoodsStr = badGoodsStr.TrimEnd(',');
                                }

                                int total = goodList.Count + badList.Count;
                                CCMessageBox.Show("执行完成~\r\n 总共： " + total + "款\r\n" + " 输出： " + goodList.Count + "款" + "\r\n不合法款：" + badList.Count + "\r\n" + badGoodsStr);
                            }
                            else
                            {
                                CCMessageBox.Show("保存失败");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    CCMessageBox.Show(ex.Message);
                }
            }
            else if (tag.Equals(3)) //专题款导入
            {
                try
                {
                    OpenFileDialog ofp = new OpenFileDialog();
                    //过滤器
                    ofp.Filter = "文本文档|*.txt";
                    if (ofp.ShowDialog() == true) //选择完成之后
                    {
                        //检查是否有数据
                        subjectList = Bll.GetListByFilePath(ofp.FileName);
                        if (subjectList.Count <= 0)
                        {
                            CCMessageBox.Show("没有找到数据");
                            return;
                        }
                    }
                }
                catch (Exception ex)
                {
                    CCMessageBox.Show(ex.Message);
                }
            }

        }


        delegate void downloadFileByHTTPHandle(Dictionary<string, string> dicts);

        /// <summary>
        /// 下载图片(默认保存至桌面\EverydayUpdate\img\)
        /// </summary>
        /// <param name="dicts">图片集合</param>
        /// <returns></returns>
        void downloadFileByHTTP(Dictionary<string, string> dicts)
        {
            try
            {
                if (Connect())
                {
                    foreach (KeyValuePair<string, string> kvp in dicts)
                    {

                        if (kvp.Key.Length < 36)
                        {
                            //获取当前用户桌面的路径
                            //string imageSavePath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + @"\EverydayUpdate\img\";//本地

                            string folderName = DateTime.Now.ToString("yyyyMMdd") + "ZT";
                            string imageSavePath = @"\\192.168.1.2\fx\" + folderName;//局域多网
                            WebClient wc = new WebClient();

                            if (!System.IO.Directory.Exists(imageSavePath)) { System.IO.Directory.CreateDirectory(imageSavePath); }
                            wc.DownloadFile(kvp.Value + "_270x270.jpg", imageSavePath + "\\" + kvp.Key + ".jpg");
                        }
                    }
                }
            }
            catch
            {

            }

        }

        public bool Connect()
        {
            bool Flag = true;
            Process proc = new Process();
            proc.StartInfo.FileName = "cmd.exe";
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardInput = true;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.CreateNoWindow = true;
            try
            {
                proc.Start();


                string command = @"net use \\192.168.1.2\fx ""admin@www.9mg.cn"" /user:""administrator""";
                proc.StandardInput.WriteLine(command);
                command = "exit";
                proc.StandardInput.WriteLine(command);
                while (proc.HasExited == false)
                {
                    proc.WaitForExit(1000);
                }
                string errormsg = proc.StandardError.ReadToEnd();
                if (errormsg != "")
                    Flag = false;
                proc.StandardError.Close();
            }
            catch (Exception ex)
            {
                Flag = false;
            }
            finally
            {
                proc.Close();
                proc.Dispose();
            }
            return Flag;
        }



        //专题款生成
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //如果是京东
            EnumEntity entity = (EnumEntity)Com_Shop.SelectedItem;
            if (entity.Value == 5)
            {
                CCMessageBox.Show("京东无法生成专题款");
                return;
            }
            if (subjectList.Count <= 0)
            {
                CCMessageBox.Show("没有找到数据");
                return;
            }

            //合格款号
            List<string> goodList = new List<string>();
            //非法款号
            List<string> badList = new List<string>();
            foreach (string goodsno in subjectList)
            {
                //匹配款号是否合法
                string pattern = @"^\d{2}\w{2,4}\d{0,5}$";
                Match m = Regex.Match(goodsno, pattern);
                if (m.Success)
                {
                    goodList.Add(goodsno);
                }
                else
                {
                    badList.Add(goodsno);
                }
            }

            //根据不同的key来，获取不同店铺的数据
            // OnlineStoreAuthorization osa = ComboBoxBind.GetAuthorization(Com_Shop);
            var shop = shoplist.Where(x => x.shopId == Convert.ToInt32(Com_Shop.SelectedValue)).FirstOrDefault();
            TopSource com = new TopSource(shop);
            List<Item> itemList = com.GetItemList(goodList, "pic_url");

            //排序后的款号
            List<Item> sortList = Bll.SortItemList(itemList, goodList);

            StringBuilder sb = new StringBuilder(10);
            foreach (Item item in sortList)
            {
                sb.AppendLine(item.OuterId);
                sb.AppendLine(item.Title);
                sb.AppendLine(item.Price);
                sb.AppendLine(Bll.GetDetailUrl(ComboBoxBind.ShopTypeByCmb(Com_Shop), item.NumIid.ToString()));
                sb.AppendLine().AppendLine();
            }
            if (Bll.SaveFile("专题款_" + DateTime.Now.ToString("HH-mm-ss"), sb.ToString(), "EverydayUpdate"))
            {
                //不合格款
                string badGoodsStr = string.Empty;
                if (badList.Count > 0)
                {
                    foreach (string s in badList)
                    {
                        badGoodsStr += s + ",";
                    }
                    badGoodsStr = badGoodsStr.TrimEnd(',');
                }
                CCMessageBox.Show("执行完成~\r\n 总共： " + subjectList.Count + "款\r\n" +
                                " 输出： " + itemList.Count + "款" + "\r\n不合法款：" +
                                badList.Count + "\r\n" + badGoodsStr);
            }
            else
            {
                CCMessageBox.Show("保存失败");
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
    }
}
