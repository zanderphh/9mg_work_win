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
    /// ColorOperation.xaml 的交互逻辑
    /// </summary>
    public partial class ColorOperation : UserControl ,BaseDialog
    {
        private BaseDAL dal = new BaseDAL();
        OperationStatus status;
        ColorModel model;
        public ColorOperation(OperationStatus status,ColorModel model)
        {
            this.status = status;
            this.model = model;
            InitializeComponent();
            this.DataContext = this;
            if(status == OperationStatus.Edit)
            {
                Tb_Color.Text = model.Color;
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
            get { return status == OperationStatus.ADD ? "添加颜色" : "修改颜色"; }
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
            switch (Tag)
            {
                case 0:
                    bool bs;
                    string color = Tb_Color.Text;
                    if (string.IsNullOrEmpty(Tb_Color.Text))
                    {
                        CCMessageBox.Show("填写颜色名称");
                        return;
                    }
                    if (status == OperationStatus.ADD)
                    {
                        bs = dal.Add<ColorModel>(new ColorModel() { Color = color });
                    }
                    else
                    {
                        model.Color = color;
                        bs = dal.Update<ColorModel>(model);
                    }
                    if (bs)
                    {
                       GoodsAttribute attr =   (GoodsAttribute) FormInit.FindFather(this);
                       attr.BindColorList();
                        FormInit.CloseDialog(this);
                        
                    }
                    else
                    {
                        CCMessageBox.Show("编辑失败");
                    }
                    break;
                case 1:
                    FormInit.CloseDialog(this);
                    break;
            }
        }
    }
}
