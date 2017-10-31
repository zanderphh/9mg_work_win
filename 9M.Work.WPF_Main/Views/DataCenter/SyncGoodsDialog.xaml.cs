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
using _9M.Work.DbObject;
using _9M.Work.WPF_Common.WpfBind;
using _9M.Work.WPF_Main.FrameWork;
using System.Runtime.Remoting.Messaging;
using _9M.Work.TopApi;
using Top.Api.Domain;
using System.Text.RegularExpressions;
using System.IO;

namespace _9M.Work.WPF_Main.Views.DataCenter
{
    /// <summary>
    /// SyncGoodsDialog.xaml 的交互逻辑
    /// </summary>
    public partial class SyncGoodsDialog : UserControl, BaseDialog
    {
        private BaseDAL dal = new BaseDAL();
        private List<ShopModel> shoplist = null;
        List<string> goodsnolist = null;
        public delegate Dictionary<string, object> Handler(object[] parm);

        public SyncGoodsDialog()
        {
            InitializeComponent();
            this.DataContext = this;
            BindShop();
        }

        public void BindShop()
        {
            shoplist = dal.GetList<ShopModel>(x => x.invokeUrl == "C" && x.shopId != 1000);
            //  shoplist.Insert(0, new ShopModel() { shopId = 0, shopName = "请选择" });
            ComboBoxBind.BindComboBox(Com_Shop, shoplist, "shopName", "shopId");
            Com_Shop.SelectedIndex = shoplist.FindIndex(x => x.shopId == 1030);
        }

        public DelegateCommand CancelCommand
        {
            get { return new DelegateCommand(CloseDialog); }
        }

        public string Title
        {
            get { return "数据同步"; }
        }

        public void CloseDialog()
        {
            FormInit.CloseDialog(this);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string Tag = (sender as Button).Tag.ToString();
            switch (Tag)
            {
                case "0":
                    goodsnolist = GlobalUtil.GetImport();
                    break;
                case "1":
                    if (goodsnolist == null || goodsnolist.Count == 0)
                    {
                        CCMessageBox.Show("请导入数据");
                        return;
                    }
                    SyncModel syncmodel = new SyncModel()
                    {
                        RemoveDesclink = Convert.ToBoolean(chk_removedesclink.IsChecked),
                        SyncDesc = Convert.ToBoolean(chk_syncdesc.IsChecked),
                        SyncDis = Convert.ToBoolean(chk_syncdis.IsChecked),
                        SyncPrice = Convert.ToBoolean(chk_syncprice.IsChecked),
                        SyncTile = Convert.ToBoolean(chk_synctitle.IsChecked),
                        TitleReplace = Convert.ToBoolean(chk_replacetitle.IsChecked),
                        TitleBefor = tb_titlebefor.Text.Trim(),
                        TitleAfter = tb_titleafter.Text.Trim()
                    };
                    ShopModel shop = Com_Shop.SelectedItem as ShopModel;
                    Handler handler = new Handler(SyncGoods);
                    handler.BeginInvoke(new object[] { syncmodel, shop }, TradeCallBack, null);
                    break;
            }
        }


        private void TradeCallBack(IAsyncResult ar)
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

        public Dictionary<string, object> SyncGoods(object[] obj)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            StreamWriter sw = File.CreateText(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + @"\\同步日志.txt");
            bar.LoadBar(true);
            bar.Loading(true);
            try
            {
                SyncModel syncmodel = obj[0] as SyncModel;
                ShopModel shop = obj[1] as ShopModel;
                TopSource ctop = new TopSource();
                TopSource top = new TopSource(shop);

                //组装修改参数
                UpdateGoodsSub ugs = new UpdateGoodsSub();
                ugs.TitleStatus = syncmodel.SyncTile ? 4 : 0;
                ugs.DescStatus = syncmodel.SyncDesc ? 3 : 0;
                ugs.SyncPrice = syncmodel.SyncPrice;
                //查询NUMIID
                List<Item> numiidlist = ctop.GetItemList(goodsnolist, string.Empty);
                //查询商品
                List<Item> citemlist = ctop.GetItemListDetail(numiidlist.Select(x => x.NumIid.ToString()).ToList(), "sku,desc,has_discount");

                List<Item> list = top.GetItemList(goodsnolist, "sku");
                bar.Loading(false);
                
                for (int i = 0; i < list.Count; i++)
                {
                    try
                    {
                        Item item = citemlist.Find(x =>
                        {
                            return x.OuterId.Equals(list[i].OuterId);
                        });
                        if (item != null)
                        {
                            //组装修改数据
                            ugs.Price = item.Price;
                            ugs.AllDesc = item.Desc;
                            if (syncmodel.TitleReplace)
                            {
                                ugs.AllTitle = item.Title.Replace(syncmodel.TitleBefor, syncmodel.TitleAfter);
                            }
                            else
                            {
                                ugs.AllTitle = item.Title;
                            }

                            if (syncmodel.RemoveDesclink)//是否去除A标签
                            {
                                string regval = @"<a(?:(?!href=).)*href=(['""]?)(?<url>[^""'\s>]*)\1[^>]*>(?<text>(?:(?!</a>).)*)</a>";
                                ugs.AllDesc = Regex.Replace(item.Desc, regval, "");
                            }
                            else
                            {
                                ugs.AllDesc = item.Desc;
                            }
                            if (syncmodel.SyncDis)
                            {
                                ugs.Dis = item.HasDiscount ? 1 : 2;
                            }
                            string res = top.UpdateItemByModel(list[i], ugs, new List<NoPayModel>());
                            if (!res.Equals("success"))
                            {
                                sw.WriteLine(list[i].OuterId+":" +res);
                            }
                        }
                    }
                    catch(Exception ex)
                    {
                        sw.WriteLine(list[i].OuterId + ":" + ex.Message);
                    }
                    finally
                    {
                        bar.UpdateBarValue(list.Count, i + 1);
                    }
                   
                }
                goodsnolist.Clear();
                dic.Add("","");
            }
            catch (Exception ex)
            {
                sw.WriteLine(ex.Message);
                dic.Add(ex.Message, "");
            }
            finally
            {
                sw.Close();
                bar.LoadBar(false);
            }
           
            return dic;
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


    }

    public class SyncModel
    {
        public bool SyncTile { get; set; }
        public bool SyncDesc { get; set; }
        public bool SyncDis { get; set; }
        public bool SyncPrice { get; set; }
        public bool RemoveDesclink { get; set; }
        public bool TitleReplace { get; set; }
        public string TitleBefor { get; set; }
        public string TitleAfter { get; set; }
    }
}
