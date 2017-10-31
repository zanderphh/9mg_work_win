using _9M.Work.DbObject;
using _9M.Work.Model;
using _9M.Work.WPF_Common;
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
    /// BrandOperation.xaml 的交互逻辑
    /// </summary>
    public partial class BrandOperation : UserControl, BaseDialog
    {
        private OperationStatus status;
        private BrandModel model;
        private BaseDAL dal = new BaseDAL();
        public BrandOperation(OperationStatus status, BrandModel model)
        {
            InitializeComponent();
            this.DataContext = this;
            this.status = status;
            this.model = model;
            if (status == OperationStatus.Edit)
            {
                tb_BrandCN.Text = model.BrandCN;
                tb_BrandEN.Text = model.BrandEN;
                tb_Order.Text = model.OrderId.ToString();
                com_Lvevl.Text = model.Level.ToString();
                rich_Remark.Text = model.SellPoint;
                LoadSizes(model.Sizes, model.SizeCodes);
            }
            else if (status == OperationStatus.ADD)
            {
                LoadSizes("XS,S,M,L,XL,XXL,XXXL", "0,1,2,3,4,5,6");
            }
        }

        public void LoadSizes(string Sizes, string SizeCodes)
        {
            List<TextBox> TextList = WPFControlsSearchHelper.GetChildObjects<TextBox>(sizepanel, string.Empty);
            List<string> list = Sizes.Split(',').Where(x => !string.IsNullOrEmpty(x)).ToList();
            List<int> indexlist = SizeCodes.Split(',').Where(x => !string.IsNullOrEmpty(x)).Select(x => Convert.ToInt32(x)).ToList();
            for (int i = 0; i < list.Count; i++)
            {
                TextList[indexlist[i]].Text = list[i];
            }
        }

        public string[] GetSizes()
        {

            string Text = string.Empty;
            string Codes = string.Empty;
            List<TextBox> TextList = WPFControlsSearchHelper.GetChildObjects<TextBox>(sizepanel, string.Empty);
            for (int i = 0; i < TextList.Count; i++)
            {
                if (!string.IsNullOrEmpty(TextList[i].Text))
                {
                    Text += TextList[i].Text + ",";
                    Codes += i + ",";
                }
            }
            string[] arry = new string[] { Text.TrimEnd(','), Codes.TrimEnd(',') };
            return arry;
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
            get { return "品牌编辑"; }
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
                    BrandModel editmodel = new BrandModel();
                    editmodel.BrandCN = tb_BrandCN.Text.Trim();
                    editmodel.BrandEN = tb_BrandEN.Text.Trim();
                    editmodel.OrderId = Convert.ToInt32(tb_Order.Text);
                    editmodel.Level = Convert.ToInt32(com_Lvevl.Text);
                    editmodel.SellPoint = rich_Remark.Text.Trim();
                    string[] arry = GetSizes();
                    editmodel.Sizes = arry[0];
                    editmodel.SizeCodes = arry[1];
                    if (string.IsNullOrEmpty(editmodel.BrandCN) || string.IsNullOrEmpty(editmodel.BrandCN))
                    {
                        CCMessageBox.Show("请填写完整");
                        return;
                    }
                    if (status == OperationStatus.Edit)
                    {

                        editmodel.Id = model.Id;
                        dal.Update<BrandModel>(editmodel);
                    }
                    else
                    {
                        int count = dal.QueryList<BrandModel>(string.Format("select * from T_Brand where Branden = '{0}'", editmodel.BrandEN), new object[] { }).Count;
                        if (count > 0)
                        {
                            CCMessageBox.Show("己存在品牌");
                            return;
                        }
                        dal.Add<BrandModel>(editmodel);
                    }
                    GoodsAttribute ass = (GoodsAttribute)FormInit.FindFather(this);
                    ass.BindBrandList();
                    CloseDialog();
                    break;
                case 1:
                    CloseDialog();
                    break;
            }
        }
    }
}
