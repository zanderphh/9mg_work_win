using _9M.Work.Model;
using _9M.Work.TopApi;
using _9M.Work.WPF_Common.WpfBind;
using _9M.Work.WPF_Main.Infrastrcture;
using _9M.Work.WPF_Main.Views.Dialog;
using _9Mg.Work.TopApi;
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
using Top.Api.Domain;

namespace _9M.Work.WPF_Main.ControlTemplate
{
    /// <summary>
    /// CUpdateGoodsHeader.xaml 的交互逻辑
    /// </summary>
    public partial class CUpdateGoodsHeader : UserControl
    {
        public CUpdateGoodsHeader()
        {
            InitializeComponent();
            BindCombobox();
            com_title.SelectionChanged += com_title_SelectionChanged;
        }

        public void BindCombobox()
        {
            com_brandVal.ItemsSource = new List<ComboboxItemModel>()
            {
                new ComboboxItemModel() { Text = "空",Value=""},
                new ComboboxItemModel() { Text = "9魅家",Value="1114524348"},
            };
            com_brandVal.SelectedIndex = 0;
        }

        public ShopModel CurrentShop { get; set; }
        public string SellerCids { get; set; }
        private void com_title_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ShowHideControls((sender as ComboBox).SelectedIndex);
        }

        public void ShowHideControls(int Index)
        {
            if (Index == 0)
            {
                Wrap_add.Visibility = System.Windows.Visibility.Collapsed;
                Wrap_append.Visibility = System.Windows.Visibility.Collapsed;
            }
            else if (Index == 1 || Index == 3)
            {
                Wrap_add.Visibility = System.Windows.Visibility.Visible;
                Wrap_append.Visibility = System.Windows.Visibility.Collapsed;
            }
            else if (Index == 2)
            {
                Wrap_add.Visibility = System.Windows.Visibility.Collapsed;
                Wrap_append.Visibility = System.Windows.Visibility.Visible;
            }
        }

        public List<GoodsUpdateBind> GetBindList(DataTable dt)
        {
            List<GoodsUpdateBind> list = new List<GoodsUpdateBind>();
            foreach (DataRow dr in dt.Rows)
            {
                GoodsUpdateBind model = new GoodsUpdateBind()
                {
                    GoodsNo = dr[0].ToString(),
                    Price = dt.Columns.Count > 1 && check_price.IsChecked == true ? dr[1].ToString() : string.Empty,
                    IsUpdateBrand = com_brand.SelectedIndex > 0,
                    IsUpdateDesc = false,
                    IsUpdateDis = com_dis.SelectedIndex > 0,
                    IsUpdatePost = com_post.SelectedIndex > 0,
                    IsUpdatePrice = Convert.ToBoolean(check_price.IsChecked),
                    IsUpdateProductNum = false,
                    IsUpdateSellerCid = !string.IsNullOrEmpty(tb_cids.Text),
                    IsUpdateStatus = com_status.SelectedIndex > 0,
                    IsUpdateStock = Convert.ToBoolean(check_syncstock.IsChecked),
                    IsUpdateTitle = com_title.SelectedIndex > 0,
                    IsUpdateMaterials = Convert.ToBoolean(check_miaoliao.IsChecked),
                    IsUpdateSellPoint = com_sellpoint.SelectedIndex > 0
                };
                list.Add(model);
            }
            return list;
        }

        public UpdateGoodsSub GetUpdateCondition()
        {
            UpdateGoodsSub fxp = new UpdateGoodsSub();
            fxp.Pid = 0;
            fxp.SyncPrice = Convert.ToBoolean(check_price.IsChecked);
            fxp.SyncStock = Convert.ToBoolean(check_syncstock.IsChecked);
            fxp.PostStatus = Convert.ToInt32( com_post.SelectedValue);
            fxp.TitleStatus = com_title.SelectedIndex;
            fxp.GoodsStatus = com_status.SelectedIndex;
            fxp.UpTime = date_uptime.SelectedDate;
            fxp.Dis = com_dis.SelectedIndex;
            fxp.AppendTitle = tb_titleadd.Text;
            fxp.ReplaceTitleValue = tb_titleval.Text;
            fxp.ReplaceTitleResult = tb_titleres.Text;
            fxp.SellerCids = SellerCids;
            fxp.Brand = com_brand.SelectedIndex > 0 ? "20000:29534" : "";
            fxp.BrandVal = com_brandVal.SelectedValue.ToString();
            fxp.SetMaterialsOther = Convert.ToBoolean(check_miaoliao.IsChecked);
            fxp.SyncSellPoint = com_sellpoint.SelectedIndex > 0;
            fxp.SellPointStr = tb_sellPoint.Text;
            return fxp;
        }

        //列表
        private void Button_Click(object sender, RoutedEventArgs e)
        {

            TopSource sd = new TopSource(CurrentShop);
            List<SellerCat> list = sd.GetShopCat();
            List<TreeModel> modelist = TreeViewDataConverter.ConverterTreeFromSellerCat(list);
            FormInit.OpenDialog(this, new TreeDialog(this, modelist), false);
        }

        public void BindPostCombo(ShopModel shop)
        {
            TopSource top = new TopSource(shop);
            List<DeliveryTemplate> list = top.GetPostTemplate();
            list.Insert(0, new DeliveryTemplate() { TemplateId = 0, Name = "请选择" });
            ComboBoxBind.BindComboBox(com_post, list, "Name", "TemplateId");
            com_post.SelectedIndex = 0;
        }
    }
}
