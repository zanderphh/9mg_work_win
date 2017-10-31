using _9M.Work.DbObject;
using _9M.Work.Model;
using _9M.Work.WPF_Common.WpfBind;
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

namespace _9M.Work.WPF_Main.Views.QualityCheck
{
    /// <summary>
    /// BatchOperation.xaml 的交互逻辑
    /// </summary>
    public partial class BatchOperation : UserControl, BaseDialog
    {
        BaseDAL dal = new BaseDAL();
        OperationStatus status;
        QualityBatchModel model;
        public BatchOperation(OperationStatus status, QualityBatchModel model)
        {
            this.status = status;
            this.model = model;
            InitializeComponent();
            this.DataContext = this;
            Init();
        }

        public void Init()
        {
            List<BrandModel> bmlist = dal.GetAll<BrandModel>();
            ComboBoxBind.BindComboBox(cb_Brand, bmlist, "BrandCN", "BrandEN");
            dp_Distinguishdate.Text = DateTime.Now.ToShortDateString();
            dp_Instockdate.Text = DateTime.Now.ToShortDateString();
            if (status == OperationStatus.Edit)
            {
                tb_BatchName.Text = model.BatchName;
                tb_ImperfectCount.Text = model.ImperfectCount.ToString();
                tb_ImperfectRate.Text = model.Imperfectrate.ToString();
                tb_OneOnOne.Text = model.OneononeCount.ToString();
                tb_UnitCount.Text = model.UnitCount.ToString();
                tb_Year1.Text = model.Year1;
                tb_Year2.Text = model.Year2;
                tb_Year3.Text = model.Year3;
                tb_Year4.Text = model.Year4;
                tbWareCount.Text = model.WareCount.ToString();
                chb_1.IsChecked = model.Spring;
                chb_2.IsChecked = model.Summer;
                chb_3.IsChecked = model.Autumn;
                chb_4.IsChecked = model.Winter;
                cb_Brand.Text = model.Brand;
                dp_Distinguishdate.Text = model.DistinguishDate.ToShortDateString();
                dp_Instockdate.Text = model.IntstockDate.ToShortDateString();
                rich_Remark.Text = model.Remark;

              
                if (bmlist.Count > 0)
                {
                    cb_Brand.SelectedItem = bmlist.Where(x => x.BrandCN.Equals(model.Brand)).Single();
                }

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
            get { return status == OperationStatus.Edit ? "编辑批次" : "添加批次"; }
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
            if (((Boolean)chb_1.IsChecked && string.IsNullOrEmpty(tb_Year1.Text)) || ((Boolean)chb_2.IsChecked && string.IsNullOrEmpty(tb_Year2.Text)) ||
                ((Boolean)chb_3.IsChecked && string.IsNullOrEmpty(tb_Year3.Text)) || ((Boolean)chb_4.IsChecked && string.IsNullOrEmpty(tb_Year4.Text)))
            {
                CCMessageBox.Show("季节对应的年份为必须填写");
                return;
            }
            bool b = true;
            QualityBatchModel model = new QualityBatchModel();
            int Tag = Convert.ToInt32((sender as Button).Tag);
            if (Tag == 1)
            {
                FormInit.CloseDialog(this);
            }
            model.BatchName = tb_BatchName.Text;
            model.ImperfectCount = string.IsNullOrEmpty(tb_ImperfectCount.Text) ? 0 : Convert.ToInt32(tb_ImperfectCount.Text);
            model.Imperfectrate = string.IsNullOrEmpty(tb_ImperfectRate.Text) ? 0 : Convert.ToDecimal(tb_ImperfectRate.Text);
            model.OneononeCount = string.IsNullOrEmpty(tb_OneOnOne.Text) ? 0 : Convert.ToInt32(tb_OneOnOne.Text);
            model.UnitCount = string.IsNullOrEmpty(tb_UnitCount.Text) ? 0 : Convert.ToInt32(tb_UnitCount.Text);
            model.Year1 = tb_Year1.Text;
            model.Year2 = tb_Year2.Text;
            model.Year3 = tb_Year3.Text;
            model.Year4 = tb_Year4.Text;
            model.WareCount = string.IsNullOrEmpty(tbWareCount.Text) ? 0 : Convert.ToInt32(tbWareCount.Text);
            model.Spring = (Boolean)chb_1.IsChecked;
            model.Summer = (Boolean)chb_2.IsChecked;
            model.Autumn = (Boolean)chb_3.IsChecked;
            model.Winter = (Boolean)chb_4.IsChecked;
            model.Brand = cb_Brand.Text.ToString();
            model.BrandEn = cb_Brand.SelectedValue.ToString();
            model.DistinguishDate = DateTime.Parse(dp_Distinguishdate.Text);
            model.IntstockDate = DateTime.Parse(dp_Instockdate.Text);
            model.Remark = rich_Remark.Text;
            switch (Tag)
            {
                case 0:
                    if (status == OperationStatus.Edit)
                    {
                        model.Id = this.model.Id;
                    }
                    b = status == OperationStatus.ADD ? dal.Add<QualityBatchModel>(model) : dal.Update<QualityBatchModel>(model);
                    if (b)
                    {
                        BatchManagement ba = (BatchManagement)FormInit.FindFather(this);
                        ba.BindGrid();
                        FormInit.CloseDialog(this);
                    }
                    break;

            }
        }
    }
}
