using _9M.Work.DbObject;
using _9M.Work.Model;
using _9M.Work.WPF_Main.Infrastrcture;
using Microsoft.Practices.Prism.Commands;
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
    /// CategoryOperation.xaml 的交互逻辑
    /// </summary>
    public partial class CategoryOperation : UserControl, BaseDialog
    {
        BaseDAL dal = new BaseDAL();
        OperationStatus status;
        CategoryModel model;
        public CategoryOperation(OperationStatus status, CategoryModel model)
        {
            this.DataContext = this;
            this.status = status;
            this.model = model;
            InitializeComponent();
            Init();
        }

        public void Init()
        {
            if (status == OperationStatus.Edit)
            {
                tb_CategoryName.Text = model.CategoryName;
                tb_codemin.Text = model.CategoryCodeMin.ToString();
                tb_codemax.Text = model.CategoryCodeMax.ToString();
                cb_linxing.IsChecked = model.LingXing;
                cb_menjin.IsChecked = model.MenJin;
                cb_mianliao.IsChecked = model.MianLiao;
                cb_qita.IsChecked = model.QiTa;
                cb_xiuxing.IsChecked = model.XiuXing;
                cb_yanse.IsChecked = model.YanSe;
            }
        }
        public Microsoft.Practices.Prism.Commands.DelegateCommand CancelCommand
        {
            get { return new DelegateCommand(CloseDialog); }
        }

        public void CloseDialog()
        {
            FormInit.CloseDialog(this);
        }

        public string Title
        {
            get { return status == OperationStatus.ADD ? "添加分类" : "修改分类"; }
        }

        public bool IsNavigationTarget(Microsoft.Practices.Prism.Regions.NavigationContext navigationContext)
        {
            throw new NotImplementedException();
        }

        public void OnNavigatedFrom(Microsoft.Practices.Prism.Regions.NavigationContext navigationContext)
        {
            throw new NotImplementedException();
        }

        public void OnNavigatedTo(Microsoft.Practices.Prism.Regions.NavigationContext navigationContext)
        {
            throw new NotImplementedException();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int Tag = Convert.ToInt32((sender as Button).Tag);
            CategoryModel cm = new CategoryModel();

            switch (Tag)
            {

                case 0: //确定
                    bool bs = true;
                    if (string.IsNullOrEmpty(tb_CategoryName.Text) || string.IsNullOrEmpty(tb_codemin.Text) || string.IsNullOrEmpty(tb_codemax.Text))
                    {
                        CCMessageBox.Show("完整的填写信息");
                        return;
                    }
                    cm.CategoryName = tb_CategoryName.Text;
                    cm.CategoryCodeMin = Convert.ToInt32(tb_codemin.Text);
                    cm.CategoryCodeMax = Convert.ToInt32(tb_codemax.Text);
                    cm.LingXing = Convert.ToBoolean(cb_linxing.IsChecked);
                    cm.MenJin = Convert.ToBoolean(cb_menjin.IsChecked);
                    cm.MianLiao = Convert.ToBoolean(cb_mianliao.IsChecked);
                    cm.QiTa = Convert.ToBoolean(cb_qita.IsChecked);
                    cm.XiuXing = Convert.ToBoolean(cb_xiuxing.IsChecked);
                    cm.YanSe = Convert.ToBoolean(cb_yanse.IsChecked);
                    if (status == OperationStatus.ADD)
                    {
                        bs = dal.Add<CategoryModel>(cm);
                    }
                    else
                    {
                        cm.Id = model.Id;
                        bs = dal.Update<CategoryModel>(cm);
                    }
                    if(bs)
                    {
                        GoodsAttribute ga = (GoodsAttribute)FormInit.FindFather(this);
                        ga.BindCategoryList();
                        FormInit.CloseDialog(this);
                    }
                    break;
                case 1: //取消
                    this.CloseDialog();
                    break;
            }
        }
    }
}
