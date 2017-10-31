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
    /// CategoryPropertyEdit.xaml 的交互逻辑
    /// </summary>
    public partial class CategoryPropertyEdit : UserControl,BaseDialog
    {
        BaseDAL dal = new BaseDAL();
        OperationStatus status;
        CategoryPropertyModel model;
        public CategoryPropertyEdit(OperationStatus status,CategoryPropertyModel model)
        {
            this.status = status;
            this.model = model;
            this.DataContext = this;
            InitializeComponent();
            if(status == OperationStatus.Edit)
            {
                tb_property.Text = model.PropertyValue;
            }
        }

        public Microsoft.Practices.Prism.Commands.DelegateCommand CancelCommand
        {
            get {  return new DelegateCommand(CloseDialog);  }
        }

        public void CloseDialog()
        {
            FormInit.CloseDialog(this);
        }

        public string Title
        {
            get { return status == OperationStatus.Edit ? "修改特点" : "添加特点"; }
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
            switch(Tag)
            { 
                case 0:
                    if (string.IsNullOrEmpty(tb_property.Text))
                    {
                        CCMessageBox.Show("请填写特点");
                        return;
                    }
                    model.PropertyValue = tb_property.Text;
                    bool bs;
                    if (status == OperationStatus.ADD)
                    {
                        bs = dal.Add<CategoryPropertyModel>(model);
                    }
                    else
                    {
                        bs = dal.Update<CategoryPropertyModel>(model);
                    }
                    if (bs)
                    {
                        GoodsAttribute attr = (GoodsAttribute)FormInit.FindFather(this);
                        attr.BindTeDianListBox(model.PropertyType,model.CategoryId);
                        CloseDialog();
                    }
                    else
                    {
                        CCMessageBox.Show("修改失败");
                    }
                    break;
                case 1:
                    CloseDialog();
                    break;
            }
        }
    }
}
