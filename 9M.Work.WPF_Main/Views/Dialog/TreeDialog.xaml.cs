using _9M.Work.Model;
using _9M.Work.WPF_Main.ControlTemplate;
using _9M.Work.WPF_Main.Infrastrcture;
using _9M.Work.WPF_Main.Views.DataCenter;
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

namespace _9M.Work.WPF_Main.Views.Dialog
{
    /// <summary>
    /// TreeDialog.xaml 的交互逻辑
    /// </summary>
    public partial class TreeDialog : UserControl, BaseDialog
    {
        private object baseForm;
        public TreeDialog(object obj, List<TreeModel> modelist)
        {
            this.DataContext = this;
            InitializeComponent();
            CategoryTree.ItemsSourceData = modelist;
            baseForm = obj;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string Tag = (sender as Button).Tag.ToString();
            switch (Tag)
            {
                case "1":  //确定选中

                    string Name = baseForm.GetType().Name;
                    switch (Name)
                    {
                       
                        case "CUpdateGoodsHeader":
                            CUpdateGoodsHeader c = (CUpdateGoodsHeader)baseForm;
                            IList<TreeModel> clist = CategoryTree.CheckedItemsIgnoreRelation().Where(x => x.ParentId != 0).ToList();
                            string cids = string.Empty;
                            string Text = string.Empty;
                            foreach (TreeModel t in clist)
                            {
                                cids += t.Id + ",";
                                Text += t.Name + ",";
                            }
                            c.SellerCids = cids.TrimEnd(',');
                            c.tb_cids.Text = Text.TrimEnd(',');
                            FormInit.CloseDialog(this);
                            break;
                    }
                    break;
                case "2":
                    FormInit.CloseDialog(this);
                    break;
                case "3":
                    FormInit.CloseDialog(this);
                    break;
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


        public string Title
        {
            get { return "分类选择"; }
        }
    }
}
