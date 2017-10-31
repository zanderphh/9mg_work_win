using _9M.Work.DbObject;
using _9M.Work.Model;
using _9M.Work.Utility;
using _9M.Work.WPF_Common;
using _9M.Work.WPF_Common.WpfBind;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace _9M.Work.WPF_Main.Infrastrcture
{
    public class DynamicBind
    {
        public static void BindTeDianPanel(CategoryModel category, WrapPanel wrappanel, bool IsVerticalAlignment)
        {
            wrappanel.Children.Clear();
            //得到下拉眶的值
            List<CategoryPropertyModel> propertylist = new BaseDAL().GetList<CategoryPropertyModel>(x => x.CategoryId == category.Id);
            Dictionary<string, int> dic = new Dictionary<string, int>();
            //得到枚举集合
            List<EnumEntity> enlist = EnumHelper.GetEnumList(typeof(CategoryTraitAttr));
            if (category.XiuXing)
            {
                int XiuXingId = Convert.ToInt32(CategoryTraitAttr.XiuXing);
                dic.Add(enlist.Where(x => x.Value == XiuXingId).Single().Text, XiuXingId);
            }
            if (category.MianLiao)
            {
                int MianLiaoId = Convert.ToInt32(CategoryTraitAttr.MianLiao);
                dic.Add(enlist.Where(x => x.Value == MianLiaoId).Single().Text, MianLiaoId);
            }
            if (category.YanSe)
            {
                int YanSeId = Convert.ToInt32(CategoryTraitAttr.YanSe);
                dic.Add(enlist.Where(x => x.Value == YanSeId).Single().Text, YanSeId);
            }
            if (category.MenJin)
            {
                int MenJinId = Convert.ToInt32(CategoryTraitAttr.MenJin);
                dic.Add(enlist.Where(x => x.Value == MenJinId).Single().Text, MenJinId);
            }
            if (category.LingXing)
            {
                int LingXingId = Convert.ToInt32(CategoryTraitAttr.LingXing);
                dic.Add(enlist.Where(x => x.Value == LingXingId).Single().Text, LingXingId);
            }
            if (category.QiTa)
            {
                int QiTaId = Convert.ToInt32(CategoryTraitAttr.QiTa);
                dic.Add(enlist.Where(x => x.Value == QiTaId).Single().Text, QiTaId);
            }
            var list = dic.ToList();
            int k = 0;
            list.ForEach(x =>
            {

                WrapPanel panel = new WrapPanel();
                Label b = new Label() { Content = x.Key };
                List<CategoryPropertyModel> calist = propertylist.Where(y => y.PropertyType == x.Value).ToList();
                ComboBox com = new ComboBox() { Width = 120 };
                ComboBoxBind.BindComboBox(com, calist, "PropertyValue", "Id");
                panel.Children.Add(b);
                panel.Children.Add(com);
                if (IsVerticalAlignment && k > 0)
                {
                    panel.Margin = new System.Windows.Thickness(0, 20, 0, 0);
                }
                wrappanel.Children.Add(panel);
                k++;
            });

        }

        /// <summary>
        /// 得到特点组的值
        /// </summary>
        /// <param name="wrappanel"></param>
        /// <returns></returns>
        public static void GetWareDeDian(WrapPanel wrappanel, WareModel model)
        {

            //得到枚举集合

            List<EnumEntity> enlist = EnumHelper.GetEnumList(typeof(CategoryTraitAttr));
            List<WrapPanel> wraplist = WPFControlsSearchHelper.GetChildObjects<WrapPanel>(wrappanel, "");
            wraplist.ForEach(x =>
            {
                string key = (x.Children[0] as Label).Content.ToString();
                EnumEntity enity = enlist.Find(z =>
                {
                    return key.Equals(z.Text);
                });
                if (enity != null)
                {
                    string ComboVal = (x.Children[1] as ComboBox).Text;
                    if (enity.Value.Equals(Convert.ToInt32(CategoryTraitAttr.LingXing)))
                    {
                        model.LingXing = ComboVal;
                    }
                    else if (enity.Value.Equals(Convert.ToInt32(CategoryTraitAttr.MenJin)))
                    {
                        model.MenJin = ComboVal;
                    }
                    else if (enity.Value.Equals(Convert.ToInt32(CategoryTraitAttr.MianLiao)))
                    {
                        model.MianLiao = ComboVal;
                    }
                    else if (enity.Value.Equals(Convert.ToInt32(CategoryTraitAttr.QiTa)))
                    {
                        model.QiTa = ComboVal;
                    }
                    else if (enity.Value.Equals(Convert.ToInt32(CategoryTraitAttr.XiuXing)))
                    {
                        model.XiuXing = ComboVal;
                    }
                    else if (enity.Value.Equals(Convert.ToInt32(CategoryTraitAttr.YanSe)))
                    {
                        model.YanSe = ComboVal;
                    }
                }
            });
        }
    }
}
