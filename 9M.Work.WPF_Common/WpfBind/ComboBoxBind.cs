using _9M.Work.Model;
using _9M.Work.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace _9M.Work.WPF_Common.WpfBind
{
    public class ComboBoxBind
    {
        public static void BindComboBox(ComboBox cmb, IEnumerable Data, string DisplayMemberPath, string SelectedValuePath)
        {
            cmb.ItemsSource = Data;
            cmb.DisplayMemberPath = DisplayMemberPath;
            cmb.SelectedValuePath = SelectedValuePath;
        }
        /// <summary>
        /// 绑定一个店铺的ComboBox
        /// </summary>
        /// <param name="cmb"></param>
        /// <param name="BindFX"></param>
        /// <param name="BindJD"></param>
        public static void BindShopBox(ComboBox cmb, bool BindFX, bool BindJD)
        {
            List<EnumEntity> entitylist = EnumHelper.GetEnumList(typeof(EnumRefundSendStatus));
            for (int i = entitylist.Count - 1; i >= 0; i--)
            {
                int value = entitylist[i].Value;
                if ((!BindFX && value == Convert.ToInt32(EnumRefundSendStatus.FX)) || (!BindJD && value == Convert.ToInt32(EnumRefundSendStatus.JD)))
                {
                    entitylist.RemoveAt(i);
                }
            }
            cmb.ItemsSource = entitylist;
        }

        public static void BindGoodsStatusBox(ComboBox cmb)
        {
            List<EnumEntity> entitylist = EnumHelper.GetEnumList(typeof(GoodsStatus));
            cmb.ItemsSource = entitylist;
        }



        /// <summary>
        /// 根椐下拉框来得到商品的状态
        /// </summary>
        /// <param name="cmb"></param>
        /// <returns></returns>
        public static GoodsStatus ReloadGoodsStatus(ComboBox cmb)
        {
            GoodsStatus status = GoodsStatus.Onsell;
            EnumEntity entity = (EnumEntity)cmb.SelectedItem;
            if (entity.Value == 2)
            {
                status = GoodsStatus.InStock;
            }
            else if (entity.Value == 3)
            {
                status = GoodsStatus.All;
            }
            return status;
        }

   

        public static ShopType ShopTypeByCmb(ComboBox cmb)
        {
            ShopType shop = ShopType.BShop_9MG;
            string Text = cmb.Text;
            switch (Text)
            {
                case "9魅季旗舰店":
                    shop = ShopType.BShop_9MG;
                    break;
                case "魅季歌儿旗舰店":
                    shop = ShopType.BShop_Mezingr;
                    break;
                case "9魅":
                    shop = ShopType.CShop_9M;
                    break;
            }
            return shop;
        }

        /// <summary>
        /// 绑定枚举的下拉框
        /// </summary>
        /// <param name="com"></param>
        /// <param name="t"></param>
        public static void BindEnum(ComboBox com, Type t, bool bindfirst)
        {
            List<EnumEntity> list = EnumHelper.GetEnumList(t);
            com.ItemsSource = list;
            com.DisplayMemberPath = "Text";
            com.SelectedValuePath = "Value";
            if (bindfirst == true && list.Count > 0)
            {
                com.SelectedIndex = 0;
            }
        }
    }
}
