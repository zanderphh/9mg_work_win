using _9M.Work.Model;
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

namespace _9M.Work.WPF_Main.ControlTemplate
{
    /// <summary>
    /// FxUpdateGoodsHeader.xaml 的交互逻辑
    /// </summary>
    public partial class FxUpdateGoodsHeader : UserControl
    {
        public FxUpdateGoodsHeader()
        {
            InitializeComponent();
            com_title.SelectionChanged += com_title_SelectionChanged;
            com_desc.SelectionChanged += com_desc_SelectionChanged;
        }

        private void com_desc_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ShowHideControls(false, (sender as ComboBox).SelectedIndex);
        }

        private void com_title_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ShowHideControls(true, (sender as ComboBox).SelectedIndex);
        }

        /// <summary>
        /// 得到图表绑定对象
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public List<GoodsUpdateBind> GetBindList(DataTable dt)
        {
            List<GoodsUpdateBind> list = new List<GoodsUpdateBind>();
            foreach (DataRow dr in dt.Rows)
            {
                GoodsUpdateBind model = new GoodsUpdateBind()
                {
                    GoodsNo = dr[0].ToString(),
                    Price = dt.Columns.Count > 1 && check_price.IsChecked == true ? dr[1].ToString() : string.Empty,
                    IsUpdateBrand = !string.IsNullOrEmpty(tb_brand.Text),
                    IsUpdateDesc = com_desc.SelectedIndex > 0,
                    IsUpdateDis = false,
                    IsUpdatePost = com_post.SelectedIndex > 0,
                    IsUpdatePrice = Convert.ToBoolean(check_price.IsChecked),
                    IsUpdateProductNum = !string.IsNullOrEmpty(tb_huohao.Text),
                    IsUpdateSellerCid = false,
                    IsUpdateStatus = com_status.SelectedIndex > 0,
                    IsUpdateStock = Convert.ToBoolean(check_syncstock.IsChecked),
                    IsUpdateTitle = com_title.SelectedIndex > 0,
                    IsUpdateSku = Convert.ToBoolean(check_Sku.IsChecked),
                    IsUpdateMaterials = Convert.ToBoolean(check_miaoliao.IsChecked),
                };
                list.Add(model);
            }
            return list;
        }

        /// <summary>
        /// 得到修改的条件
        /// </summary>
        /// <returns></returns>
        public UpdateGoodsSub GetUpdateCondition()
        {
            UpdateGoodsSub fxp = new UpdateGoodsSub();
            fxp.Pid = 0;
            fxp.Brand = tb_brand.Text;
            fxp.ProductNum = tb_huohao.Text;
            fxp.SyncPrice = Convert.ToBoolean(check_price.IsChecked);
            fxp.SyncStock = Convert.ToBoolean(check_syncstock.IsChecked);
            fxp.SyncSku = Convert.ToBoolean(check_Sku.IsChecked);
            fxp.PostStatus = com_post.SelectedIndex;
            fxp.TitleStatus = com_title.SelectedIndex;
            fxp.GoodsStatus = com_status.SelectedIndex;
            fxp.DescStatus = com_desc.SelectedIndex;
            fxp.AppendTitle = tb_titleadd.Text;
            fxp.ReplaceTitleValue = tb_titleval.Text;
            fxp.ReplaceTitleResult = tb_titleres.Text;
            fxp.AppendDesc = tb_descadd.Text;
            fxp.ReplaceDescValue = tb_descval.Text;
            fxp.ReplaceDescResult = tb_descres.Text;
            fxp.SyncRetailPriceOnly = Convert.ToBoolean(check_retialprice.IsChecked);
            fxp.ActivityTitle = Convert.ToBoolean(check_activytitle.IsChecked);
            fxp.SetMaterialsOther = Convert.ToBoolean(check_miaoliao.IsChecked);
            return fxp;
        }

        public void ShowHideControls(bool IsTitleControl, int Tag)
        {
            if (IsTitleControl)
            {
                if (Tag == 0)
                {
                    Wrap_add.Visibility = System.Windows.Visibility.Collapsed;
                    Wrap_append.Visibility = System.Windows.Visibility.Collapsed;
                }
                else if (Tag == 1)
                {
                    Wrap_add.Visibility = System.Windows.Visibility.Visible;
                    Wrap_append.Visibility = System.Windows.Visibility.Collapsed;
                }
                else if (Tag == 2)
                {
                    Wrap_add.Visibility = System.Windows.Visibility.Collapsed;
                    Wrap_append.Visibility = System.Windows.Visibility.Visible;
                }
            }
            else
            {
                if (Tag == 0)
                {
                    WrapDesc_add.Visibility = System.Windows.Visibility.Collapsed;
                    WrapDesc_append.Visibility = System.Windows.Visibility.Collapsed;
                }
                else if (Tag == 1 || Tag == 3)
                {
                    WrapDesc_add.Visibility = System.Windows.Visibility.Visible;
                    WrapDesc_append.Visibility = System.Windows.Visibility.Collapsed;
                }
                else if (Tag == 2 )
                {
                    WrapDesc_add.Visibility = System.Windows.Visibility.Collapsed;
                    WrapDesc_append.Visibility = System.Windows.Visibility.Visible;
                }
            }
        }

        private void check_Sku_Checked(object sender, RoutedEventArgs e)
        {
            if (check_price != null && check_syncstock != null && check_Sku != null)
            {
                if (check_price.IsChecked == true)
                {
                    panel_price.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    panel_price.Visibility = System.Windows.Visibility.Collapsed;
                }
                if ((check_price.IsChecked == true || check_syncstock.IsChecked == true) && check_Sku.IsChecked == true)
                {

                    CCMessageBox.Show("同步C店SKU的时候不能直接同步库存与价格");
                    check_Sku.IsChecked = false;
                }
            }
        }
    }
}
