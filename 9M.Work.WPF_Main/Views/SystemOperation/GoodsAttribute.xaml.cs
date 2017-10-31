using _9M.Work.DbObject;
using _9M.Work.Model;
using _9M.Work.WPF_Common;
using _9M.Work.WPF_Common.WpfBind;
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

namespace _9M.Work.WPF_Main.Views.SystemOperation
{
    /// <summary>
    /// Category.xaml 的交互逻辑
    /// </summary>
    public partial class GoodsAttribute : UserControl
    {
        private BaseDAL dal = new BaseDAL();
        public GoodsAttribute()
        {
            InitializeComponent();
            BindCategoryList();
            BindColorList();
            BindBrandList();
        }

        #region 分类编辑
        public void BindCategoryList()
        {
            List<CategoryModel> list = dal.GetAll<CategoryModel>();
            ListBoxBind.BindListBox(List_Category, list, "CategoryName", "Id");
            if (list.Count > 0)
            {
                List_Category.SelectedItem = list[0];
            }
        }

        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            CategoryModel model = (CategoryModel)List_Category.SelectedItem;
            int Tag = Convert.ToInt32((sender as Button).Tag);
            switch (Tag)
            {
                case 0: //添加分类
                    FormInit.OpenDialog(this, new CategoryOperation(OperationStatus.ADD, model), false);
                    break;
                case 1:  //修改分类
                    if (model == null)
                    {
                        CCMessageBox.Show("请选择分类");
                        return;
                    }
                    FormInit.OpenDialog(this, new CategoryOperation(OperationStatus.Edit, model), false);
                    break;
                case 2:  //删除分类
                    if (model == null)
                    {
                        CCMessageBox.Show("请选择分类");
                        return;
                    }
                    if (CCMessageBox.Show("您是否要删除分类", "提示", CCMessageBoxButton.YesNo) == CCMessageBoxResult.Yes)
                    {
                        dal.Delete<CategoryModel>(model);
                        BindCategoryList();
                    }
                    break;
            }
        }

        private void List_Category_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SolidColorBrush colorbrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0B8BEE"));
            SolidColorBrush colorbrushfalse = new SolidColorBrush(Colors.LightGray);
            CategoryModel cm = (sender as ListBox).SelectedItem as CategoryModel;
            if (cm != null)
            {
                lab_CategoryName.Content = cm.CategoryName;
                tb_codemin.Text = cm.CategoryCodeMin.ToString();
                tb_codemax.Text = cm.CategoryCodeMax.ToString();
                cb_linxing.IsEnabled = cm.LingXing;

                cb_linxing.BorderBrush = cm.LingXing ? colorbrush : colorbrushfalse;
                cb_menjin.IsEnabled = cm.MenJin;
                cb_menjin.BorderBrush = cm.MenJin ? colorbrush : colorbrushfalse;
                cb_mianliao.IsEnabled = cm.MianLiao;
                cb_mianliao.BorderBrush = cm.MianLiao ? colorbrush : colorbrushfalse;
                cb_qita.IsEnabled = cm.QiTa;
                cb_qita.BorderBrush = cm.QiTa ? colorbrush : colorbrushfalse;
                cb_xiuxing.IsEnabled = cm.XiuXing;
                cb_xiuxing.BorderBrush = cm.XiuXing ? colorbrush : colorbrushfalse;
                cb_yanse.IsEnabled = cm.YanSe;
                cb_yanse.BorderBrush = cm.YanSe ? colorbrush : colorbrushfalse;
                //得到第一个激活的单选框
                RadioButton propery = GetProperyRadio(false);
                if (propery != null)
                {
                    propery.IsChecked = true;
                    BindTeDianListBox(Convert.ToInt32(propery.Tag), cm.Id);
                }
            }
        }

        private void Btn_EditCom(object sender, RoutedEventArgs e)
        {
            int Tag = Convert.ToInt32((sender as Button).Tag);
            int property = GetProperyId(true);
            List<RadioButton> radioList = WPFControlsSearchHelper.GetChildObjects<RadioButton>(Radiopanel, "");
            if (property == 0)
            {
                CCMessageBox.Show("请选择特点");
                return;
            }
            int categoryid = ((CategoryModel)List_Category.SelectedItem).Id;
            //CategoryPropertyModel model = new CategoryPropertyModel();
            //model.CategoryId = categoryid;
            //model.PropertyType = property;
            CategoryPropertyModel model = (CategoryPropertyModel)TeDianListBox.SelectedItem;
            switch (Tag)
            {
                case 0:  //添加下拉属性
                    //为添加的类型添加数据

                    model = new CategoryPropertyModel();
                    model.PropertyType = property;
                    model.CategoryId = categoryid;

                    FormInit.OpenDialog(this, new CategoryPropertyEdit(OperationStatus.ADD, model), false);
                    break;
                case 1: //修改下拉属性
                    if (model == null)
                    {
                        CCMessageBox.Show("请选中特点值");
                        return;
                    }
                    FormInit.OpenDialog(this, new CategoryPropertyEdit(OperationStatus.Edit, model), false);
                    break;
                case 2:  //删除下拉属性
                    if (model == null)
                    {
                        CCMessageBox.Show("请选中特点值");
                        return;
                    }
                    if(CCMessageBox.Show("您是否要删除特点","提示",CCMessageBoxButton.YesNo) == CCMessageBoxResult.Yes)
                    {
                        dal.Delete<CategoryPropertyModel>(model);
                        BindTeDianListBox(model.PropertyType, model.CategoryId);
                    }
                    break;
            }
        }



        private void RadioCheck(object sender, RoutedEventArgs e)
        {
            int Tag = Convert.ToInt32((sender as RadioButton).Tag);
            int categoryid = ((CategoryModel)List_Category.SelectedItem).Id;
            BindTeDianListBox(Tag,categoryid);
        }

        public void BindTeDianListBox(int Tag, int categoryid)
        {
            List<CategoryPropertyModel> list = dal.GetList<CategoryPropertyModel>(x => x.PropertyType == Tag && x.CategoryId == categoryid);
            ListBoxBind.BindListBox(TeDianListBox, list, "PropertyValue", "Id");
        }

        /// <summary>
        /// 得到激活或选中的单选框ID
        /// </summary>
        /// <param name="GetActiveAndChecked"></param>
        /// <returns></returns>
        public int GetProperyId(bool GetActiveAndChecked)
        {
            int property = 0;
            List<RadioButton> radioList = WPFControlsSearchHelper.GetChildObjects<RadioButton>(Radiopanel, "");
            foreach (var item in radioList)
            {
                bool flag = GetActiveAndChecked ? item.IsEnabled && item.IsChecked == true : item.IsEnabled;
                if (flag)
                {
                    property = Convert.ToInt32(item.Tag);
                    break;
                }
            }
            return property;
        }

        /// <summary>
        /// 得到激活或选中的单选框
        /// </summary>
        /// <param name="GetActiveAndChecked"></param>
        /// <returns></returns>
        public RadioButton GetProperyRadio(bool GetActiveAndChecked)
        {
            RadioButton radio = null;
            List<RadioButton> radioList = WPFControlsSearchHelper.GetChildObjects<RadioButton>(Radiopanel, "");
            foreach (var item in radioList)
            {
                bool flag = GetActiveAndChecked ? item.IsEnabled && item.IsChecked == true : item.IsEnabled;
                if (flag)
                {
                    radio = item;
                    break;
                }
            }
            return radio;
        }

        #endregion

        #region 颜色编辑
        public void BindColorList()
        {
            List<ColorModel> list = dal.GetAll<ColorModel>();
            ListBoxBind.BindListBox(List_Color, list, "Color", "Id");
            if (list.Count > 0)
            {
                List_Color.SelectedItem = list[0];
            }
        }

        //颜色添加修改删除
        private void ColorBtn_Click(object sender, RoutedEventArgs e)
        {
            ColorModel model = (ColorModel)List_Color.SelectedItem;
            int Tag = Convert.ToInt32((sender as Button).Tag);
            switch (Tag)
            {
                case 0: //添加分类
                    FormInit.OpenDialog(this, new ColorOperation(OperationStatus.ADD, model), false);
                    break;
                case 1:  //修改分类
                    if (model == null)
                    {
                        CCMessageBox.Show("请选择颜色");
                        return;
                    }
                    FormInit.OpenDialog(this, new ColorOperation(OperationStatus.Edit, model), false);
                    break;
                case 2:  //删除分类
                    if (model == null)
                    {
                        CCMessageBox.Show("请选择颜色");
                        return;
                    }
                    if (CCMessageBox.Show("您是否要删除颜色", "提示", CCMessageBoxButton.YesNo) == CCMessageBoxResult.Yes)
                    {
                        dal.Delete<ColorModel>(model);
                        BindColorList();
                    }
                    break;
            }
        }
        #endregion

        #region 品牌编辑
        public void BindBrandList()
        {
            List<BrandModel> list = dal.GetAll<BrandModel>();
            ListBoxBind.BindListBox(List_Brand, list, "BrandCN", "Id");
            if (list.Count > 0)
            {
                List_Brand.SelectedItem = list[0];
            }
        }

        private void BrandBtn_Click(object sender, RoutedEventArgs e)
        {
            BrandModel model = (BrandModel)List_Brand.SelectedItem;
            int Tag = Convert.ToInt32((sender as Button).Tag);
            switch (Tag)
            {
                case 0: //添加分类
                    FormInit.OpenDialog(this, new BrandOperation(OperationStatus.ADD, model), false);
                    break;
                case 1:  //修改分类
                    if (model == null)
                    {
                        CCMessageBox.Show("请选择品牌");
                        return;
                    }
                    FormInit.OpenDialog(this, new BrandOperation(OperationStatus.Edit, model), false);
                    break;
                case 2:  //删除分类
                    if (model == null)
                    {
                        CCMessageBox.Show("请选择品牌");
                        return;
                    }
                    if (CCMessageBox.Show("您是否要删除品牌", "提示", CCMessageBoxButton.YesNo) == CCMessageBoxResult.Yes)
                    {
                        dal.Delete<BrandModel>(model);
                        BindBrandList();
                    }
                    break;
            }
        }
        #endregion

      

       
    }
}
