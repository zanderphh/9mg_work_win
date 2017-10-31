using _9M.Work.DbObject;
using _9M.Work.Model;
using _9M.Work.TopApi;
using _9M.Work.WPF_Common.WpfBind;
using _9M.Work.WPF_Main.FrameWork;
using _9M.Work.WPF_Main.Infrastrcture;
using _9M.Work.YouZan;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
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

namespace _9M.Work.WPF_Main.Views.DataCenter
{
    /// <summary>
    /// TaobaoData.xaml 的交互逻辑
    /// </summary>
    public partial class TaobaoData : UserControl
    {
        private DataTable dt;
        private UpdateGoodsSub sub = null;
        private BaseDAL dal = new BaseDAL();
        private List<ShopModel> shoplist = null;
        public TaobaoData()
        {
            InitializeComponent();
            int SH = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
            UpdateTemplate.scrolls.Height = SH - 460;
            BindShop();
            CHeader.BindPostCombo(shoplist.Where(x => x.shopId == 1000).First());
        }

        public void BindShop()
        {
            shoplist = dal.GetList<ShopModel>(x => x.appKey == "12253100" || x.appKey == "21740642");
            //  shoplist.Insert(0, new ShopModel() { shopId = 0, shopName = "请选择" });
            ComboBoxBind.BindComboBox(com_shop, shoplist, "shopName", "shopId");
            shoplist.Add(new ShopModel() { shopId = 1028, shopName = "有赞微店", appKey = "", invokeUrl = "", isHaveApi = true, sessionKey = "", appSecret = "" });
            com_shop.SelectedIndex = shoplist.FindIndex(x => x.shopId == 1000);
            CHeader.CurrentShop = shoplist.Where(x => x.shopId == 1000).FirstOrDefault();
        }

        private void UpdateWareButton_Click(object sender, RoutedEventArgs e)
        {
            int Index = Convert.ToInt32((sender as Button).Tag);
            switch (Index)
            {
                case 0:
                    dt = GlobalUtil.ReadExcel(new OpenFileDialog());
                    if (dt != null)
                    {
                        CCMessageBox.Show("共导入【" + dt.Rows.Count + "】件商品");
                    }
                    break;
                case 1: //预览修改
                    if (dt != null)
                    {
                        List<GoodsUpdateBind> list = null;
                        int ShopId = Convert.ToInt32(com_shop.SelectedValue);
                        if (ShopId == 1003)
                        {
                            list = FxHeader.GetBindList(dt);
                            sub = FxHeader.GetUpdateCondition();
                        }
                        else
                        {
                            list = CHeader.GetBindList(dt);
                            sub = CHeader.GetUpdateCondition();
                        }
                        UpdateTemplate.BindDataGrid(list);
                    }
                    break;
                case 2: //开始修改
                    if (sub != null)
                    {
                        ShopModel shop = (ShopModel)com_shop.SelectedItem;
                        if (shop.shopId == 1022)
                        {
                            if (CCMessageBox.Show("天猫只提供分类的修改.是否修改", "提示", CCMessageBoxButton.YesNo) == CCMessageBoxResult.No)
                            {
                                return;
                            }
                        }
                        //对像传值给控件（错误处理）
                        UpdateTemplate.CurrentSub = sub;
                        UpdateTemplate.CurrentBar = bar;
                        UpdateTemplate.CurrentShop = shop;
                        UpdateTemplate.BeginUpdate(shop, sub, false, bar);
                    }
                    break;
                case 3: //同步商品
                    FormInit.OpenDialog(this, new SyncGoodsDialog(), false);
                    break;
                case 4: //微店同步

                    SyncYZGoodsPriceHandle handle = new SyncYZGoodsPriceHandle(SyncYZGoodsPrice);
                    handle.BeginInvoke(null, null);
                    break;
            }
        }



        delegate void SyncYZGoodsPriceHandle();
        /// <summary>
        /// 同步有赞商品价格
        /// </summary>
        private void SyncYZGoodsPrice()
        {
            //打开加载
            bar.LoadBar(true);
            //打开加载中的字符
            bar.Loading(true);
            try
            {
                List<OnSaleItems> update_items = new List<OnSaleItems>();

                //获取有赞店铺的商品
                List<OnSaleItems> YZ_items = YZSDKHelp.GetOnSaleItems();
                //获取商品编号列
                var GoodsnoList = YZ_items.Select(x => x.outer_id).ToList<String>();
                //获取淘宝店铺该款号商品的价格
                TopSource topSource = new TopSource(shoplist.Find(a => a.shopId.Equals(1000)));
                List<Top.Api.Domain.Item> TB_items = topSource.GetItemList(GoodsnoList, null);

                //对比两个店铺的价格是否相等
                YZ_items.ForEach(delegate(OnSaleItems yz_item)
                {
                    Top.Api.Domain.Item tb_item = TB_items.Find(a => a.OuterId.Equals(yz_item.outer_id));
                    if (tb_item != null)
                    {
                        if (yz_item.price != Convert.ToDecimal(tb_item.Price))
                        {
                            yz_item.price = Convert.ToDecimal(tb_item.Price);
                            update_items.Add(yz_item);
                        }
                    }
                });

                if (update_items.Count > 0)
                {
                    update_items.ForEach(delegate(OnSaleItems item)
                    {
                        YZSDKHelp.UpdateYZGoodsPrice(item.outer_id, item.price.ToString());
                    });
                }
            }
            catch
            { }

            //关闭加载加载中的字符
            bar.Loading(false);
            //关闭加载
            bar.LoadBar(false);

            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                MessageBox.Show("刷新完成","提示");
            }));

        }

        private void ComboBox_DropDownClosed(object sender, EventArgs e)
        {
            var Shopmodel = (sender as ComboBox).SelectedItem as ShopModel;
            int ShopId = Shopmodel.shopId;
            if (Shopmodel.invokeUrl.Equals("C"))
            {
                CHeader.BindPostCombo(Shopmodel);
            }
            if (ShopId == 1003)
            {
                CHeader.Visibility = System.Windows.Visibility.Collapsed;
                FxHeader.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                CHeader.CurrentShop = shoplist.Where(x => x.shopId == ShopId).FirstOrDefault();
                CHeader.Visibility = System.Windows.Visibility.Visible;
                FxHeader.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

    }
}
