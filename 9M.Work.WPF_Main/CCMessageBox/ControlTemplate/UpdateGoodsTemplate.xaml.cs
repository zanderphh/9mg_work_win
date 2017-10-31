using _9M.Work.DbObject;
using _9M.Work.Model;
using _9M.Work.TopApi;
using _9M.Work.Utility;
using _9M.Work.WPF_Common.Controls;
using _9M.Work.WPF_Main.Data;
using _9M.Work.WPF_Main.FrameWork;
using _9M.Work.YouZan;
using _9Mg.Work.TopApi;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Threading;
using Top.Api.Domain;

namespace _9M.Work.WPF_Main.ControlTemplate
{
    /// <summary>
    /// UpdateGoodsTemplate.xaml 的交互逻辑
    /// </summary>
    public partial class UpdateGoodsTemplate : UserControl
    {
        public UpdateGoodsSub CurrentSub { get; set; }
        public CustomProgressBar CurrentBar { get; set; }
        private BaseDAL dal = new BaseDAL();
        public ShopModel CurrentShop { get; set; }
        private TopSource tp;
        //private DistributorRecord dr = new DistributorRecord();
        //private Commodity com = null;
        //更新进度条委托
        private delegate void UpdateProgressBarDelegate(System.Windows.DependencyProperty dp, Object value);
        private ObservableCollection<GoodsUpdateBind> _numberDescriptions;
        private List<GoodsUpdateBind> bindlist;
        public delegate void SyncGoodsHandler(ShopModel Shop, UpdateGoodsSub model, bool IsErrorOperation, CustomProgressBar cpb);
        public UpdateGoodsTemplate()
        {
            InitializeComponent();

        }

        private void UpdateWareButton_Click(object sender, RoutedEventArgs e)
        {
            int Tag = Convert.ToInt32((sender as Button).Tag);
            switch (Tag)
            {
                case 0: //错误处理
                    if (CurrentSub != null)
                    {
                        BeginUpdate(CurrentShop, CurrentSub, true, CurrentBar);
                    }

                    break;
                case 1:  //导出日志记录
                    ExportLog();
                    break;
                case 2:  //过滤成功信息
                    FilterOk();
                    break;
            }
        }

        //更新表格
        private delegate void LoadNumberDelegate(int Index);
        private void LoadNumber(int Index)
        {
            if (Index == bindlist.Count)
            {
                slider.Value = 0;
                lab_curent.Content = 0;
                return;
            }
            _numberDescriptions.Add(bindlist[Index]);
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new LoadNumberDelegate(LoadNumber), ++Index);
            lab_count.Content = Index;
        }

        public void BindDataGrid(List<GoodsUpdateBind> list)
        {
            bindlist = list;
            _numberDescriptions = new ObservableCollection<GoodsUpdateBind>();
            UpdateGoodsGrid.ItemsSource = _numberDescriptions;
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new LoadNumberDelegate(LoadNumber), 0);
        }

        private void UpdateGoods(ShopModel shopmodel, UpdateGoodsSub model, bool IsErrorOperateion, CustomProgressBar cpb)
        {
            if (!shopmodel.shopId.Equals(1028))
            {
                tp = new TopSource(shopmodel);
                cpb.LoadBar(true);
                cpb.Loading(true);
                List<NoPayModel> tradelist = null;
                List<Item> itemlist = null;
                List<ProductCat> catlist = null;
                string ApiName = shopmodel.invokeUrl;
                if (ApiName == "C" || ApiName == "TMALL")
                {
                    List<string> outeridlist = bindlist.Select(x => x.GoodsNo).ToList();
                    cpb.SetNavigation("正在查询淘宝数据");
                    itemlist = tp.GetItemList(outeridlist, "sku, seller_cids,props,input_str,input_pids,props_name,properties_name,properties");
                }
                else
                {
                    catlist = tp.getProductLine();
                }
                if (model.SyncStock)
                {

                    cpb.SetNavigation("正在查询未付的订单");
                    tradelist = new CommonTrade().GetNoPayList().Where(x => !string.IsNullOrEmpty(x.OuterSkuId)).ToList();

                }
                cpb.SetNavigation("");
                cpb.LoadBar(false);
                cpb.Loading(false);


                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    slider.Value = 0;
                    slider.Maximum = UpdateGoodsGrid.Items.Count;
                    lab_curent.Content = UpdateGoodsGrid.Items.Count;
                    lab_Shop.Content = shopmodel.shopName;
                    for (int i = 0; i < UpdateGoodsGrid.Items.Count; i++)
                    {
                        TextBlock rescolum = (ControlHelper.FindGridControl(UpdateGoodsGrid, 4, i) as TextBlock);
                        string Reslut = string.Empty;
                        StackPanel stack = ControlHelper.FindGridTemplateControl(UpdateGoodsGrid, 3, i, "syncpro") as StackPanel;
                        ProgressBar bar = stack.Children[0] as ProgressBar;
                        UpdateProgressBarDelegate updatePbDelegate = new UpdateProgressBarDelegate(bar.SetValue);
                        Label label = stack.Children[1] as Label;
                        bar.Value = 0;
                        bar.Maximum = 100;
                        try
                        {
                            lab_curent.Content = (i + 1).ToString();

                            if (IsErrorOperateion == true)
                            {
                                if (rescolum.Text.Equals("修改成功") || rescolum.Text.Equals("没有获取到商品信息"))
                                {
                                    slider.Value = i + 1;
                                    Reslut = rescolum.Text;
                                    continue;
                                }
                            }

                            switch (ApiName)
                            {
                                case "FX":
                                    FenxiaoProduct item = tp.GetScItemByCode(bindlist[i].GoodsNo, "skus");
                                    Dispatcher.Invoke(updatePbDelegate, System.Windows.Threading.DispatcherPriority.Background, new object[] { System.Windows.Controls.ProgressBar.ValueProperty, Convert.ToDouble(50) });
                                    label.Content = Convert.ToInt32((Convert.ToDouble(50) / 100) * 100) + "%";
                                    if (item != null)
                                    {
                                        //价格
                                        TextBlock pricecolum = (ControlHelper.FindGridControl(UpdateGoodsGrid, 2, i) as TextBlock);
                                        model.Price = string.IsNullOrEmpty(pricecolum.Text) ? "0" : pricecolum.Text;
                                        List<FenXiaoPropModel> porplist = dal.QueryList<FenXiaoPropModel>(@"select * from T_FenXiaoProp", new object[] { });
                                        string Res = tp.UpdateProductByModel(item, model, tradelist, catlist, porplist);
                                        //string Res = "success";
                                        Reslut = Res.Equals("success") ? "修改成功" : Res;
                                    }
                                    else
                                    {
                                        Reslut = "没有获取到商品信息";
                                    }
                                    break;
                                case "C":
                                    Item citem = itemlist.Where(x => x.OuterId.Equals(bindlist[i].GoodsNo, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                                    Dispatcher.Invoke(updatePbDelegate, System.Windows.Threading.DispatcherPriority.Background, new object[] { System.Windows.Controls.ProgressBar.ValueProperty, Convert.ToDouble(50) });
                                    label.Content = Convert.ToInt32((Convert.ToDouble(50) / 100) * 100) + "%";
                                    if (citem != null)
                                    {
                                        //价格
                                        TextBlock pricecolum = (ControlHelper.FindGridControl(UpdateGoodsGrid, 2, i) as TextBlock);
                                        model.Price = string.IsNullOrEmpty(pricecolum.Text) ? "0" : pricecolum.Text;
                                        string Res = string.Empty;
                                        if (shopmodel.shopId == 1022) //如果是9魅季，只提供分类的修改
                                        {
                                            List<string> sellercidslist = (citem.SellerCids.TrimEnd(',') + "," + model.SellerCids).Split(',').ToList();

                                            string XML = tp.Seller_CidsXmlData("seller_cids", sellercidslist);
                                            Res = tp.TmallUpdateGoods_Increment(citem.NumIid, XML);
                                        }
                                        else
                                        {
                                            Res = tp.UpdateItemByModel(citem, model, tradelist);
                                        }
                                        //string Res = "success";
                                        Reslut = Res.Equals("success") ? "修改成功" : Res;
                                    }
                                    else
                                    {
                                        Reslut = "没有获取到商品信息";
                                    }
                                    break;
                                case "TMALL":
                                    Item titem = itemlist.Where(x => x.OuterId.Equals(bindlist[i].GoodsNo, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                                    Dispatcher.Invoke(updatePbDelegate, System.Windows.Threading.DispatcherPriority.Background, new object[] { System.Windows.Controls.ProgressBar.ValueProperty, Convert.ToDouble(50) });
                                    label.Content = Convert.ToInt32((Convert.ToDouble(50) / 100) * 100) + "%";
                                    if (titem != null)
                                    {
                                        //价格
                                        TextBlock pricecolum = (ControlHelper.FindGridControl(UpdateGoodsGrid, 2, i) as TextBlock);
                                        model.Price = string.IsNullOrEmpty(pricecolum.Text) ? "0" : pricecolum.Text;
                                        string Res = string.Empty;

                                        List<string> sellercidslist = (titem.SellerCids.TrimEnd(',') + "," + model.SellerCids).Split(',').ToList();

                                        string XML = tp.Seller_CidsXmlData("seller_cids", sellercidslist);
                                        Res = tp.TmallUpdateGoods_Increment(titem.NumIid, XML);


                                        Reslut = Res.Equals("success") ? "修改成功" : Res;
                                    }
                                    else
                                    {
                                        Reslut = "没有获取到商品信息";
                                    }
                                    break;
                            }
                        }
                        catch (Exception ex)
                        {
                            Reslut = ex.Message;
                        }
                        finally
                        {
                            Dispatcher.Invoke(updatePbDelegate, System.Windows.Threading.DispatcherPriority.Background, new object[] { System.Windows.Controls.ProgressBar.ValueProperty, Convert.ToDouble(100) });
                            label.Content = Convert.ToInt32((Convert.ToDouble(100) / 100) * 100) + "%";
                            rescolum.Text = Reslut;
                            slider.Value = i + 1;
                        }
                    }
                }));
            }
            else
            {

                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    slider.Value = 0;
                    slider.Maximum = UpdateGoodsGrid.Items.Count;
                    lab_curent.Content = UpdateGoodsGrid.Items.Count;
                    lab_Shop.Content = shopmodel.shopName;
                    for (int i = 0; i < UpdateGoodsGrid.Items.Count; i++)
                    {
                        TextBlock rescolum = (ControlHelper.FindGridControl(UpdateGoodsGrid, 4, i) as TextBlock);//处理结果列
                        string Reslut = string.Empty;
                        StackPanel stack = ControlHelper.FindGridTemplateControl(UpdateGoodsGrid, 3, i, "syncpro") as StackPanel;//进度条列
                        ProgressBar bar = stack.Children[0] as ProgressBar;
                        UpdateProgressBarDelegate updatePbDelegate = new UpdateProgressBarDelegate(bar.SetValue);
                        Label label = stack.Children[1] as Label;
                        bar.Value = 0;
                        bar.Maximum = 100;
                        try
                        {
                            lab_curent.Content = (i + 1).ToString();

                            if (IsErrorOperateion == true)
                            {
                                if (rescolum.Text.Equals("修改成功") || rescolum.Text.Equals("没有获取到商品信息"))
                                {
                                    slider.Value = i + 1;
                                    Reslut = rescolum.Text;
                                    continue;
                                }
                            }

                            Dispatcher.Invoke(updatePbDelegate, System.Windows.Threading.DispatcherPriority.Background, new object[] { System.Windows.Controls.ProgressBar.ValueProperty, Convert.ToDouble(50) });
                            label.Content = Convert.ToInt32((Convert.ToDouble(50) / 100) * 100) + "%";
                            //价格
                            TextBlock pricecolum = (ControlHelper.FindGridControl(UpdateGoodsGrid, 2, i) as TextBlock);
                            model.Price = string.IsNullOrEmpty(pricecolum.Text) ? "0" : pricecolum.Text;

                            Reslut = (YZSDKHelp.UpdateYZGoodsPrice(bindlist[i].GoodsNo, model.Price)==true) ? "修改成功" : "修改失败";
                        
                        }
                        catch (Exception ex)
                        {
                            Reslut = ex.Message;
                        }
                        finally
                        {
                            Dispatcher.Invoke(updatePbDelegate, System.Windows.Threading.DispatcherPriority.Background, new object[] { System.Windows.Controls.ProgressBar.ValueProperty, Convert.ToDouble(100) });
                            label.Content = Convert.ToInt32((Convert.ToDouble(100) / 100) * 100) + "%";
                            rescolum.Text = Reslut;
                            slider.Value = i + 1;
                        }
                    }
                }));
            }

        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="type"></param>
        /// <param name="model"></param>
        /// <param name="IsErrorOperateion"></param>
        /// <param name="cpb"></param>
        public void BeginUpdate(ShopModel shop, UpdateGoodsSub model, bool IsErrorOperateion, CustomProgressBar cpb)
        {
            SyncGoodsHandler sgh = new SyncGoodsHandler(this.UpdateGoods);
            IAsyncResult ir = sgh.BeginInvoke(shop, model, IsErrorOperateion, cpb, null, null);
        }

        /// <summary>
        /// 过滤成功信息
        /// </summary>
        public void FilterOk()
        {
            for (int i = UpdateGoodsGrid.Items.Count - 1; i >= 0; i--)
            {
                TextBlock rescolum = (ControlHelper.FindGridControl(UpdateGoodsGrid, 4, i) as TextBlock);
                if (rescolum.Text.Equals("修改成功"))
                {
                    _numberDescriptions.RemoveAt(i);
                }
            }
        }

        public void ExportLog()
        {
            SaveFileDialog op = new SaveFileDialog();
            op.Filter = "文本文件|*.txt|所有文件|*.*";
            if (op.ShowDialog() == true)
            {
                string filename = op.FileName;
                StreamWriter sw = new StreamWriter(filename);
                for (int i = 0; i < UpdateGoodsGrid.Items.Count; i++)
                {
                    sw.WriteLine((ControlHelper.FindGridControl(UpdateGoodsGrid, 0, i) as TextBlock).Text + "       " + (ControlHelper.FindGridControl(UpdateGoodsGrid, 4, i) as TextBlock).Text);
                }
                sw.Close();
            }
        }
    }
}
