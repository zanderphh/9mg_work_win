using _9M.Work.Model;
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
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;
using _9M.Work.DbObject;
using _9M.Work.WPF_Common.WpfBind;
using _9M.Work.WPF_Main.Views.EveryDayUpdate.FuDaiTemplate;

namespace _9M.Work.WPF_Main.Views.EveryDayUpdate
{
    /// <summary>
    /// FuDaiBatchOperat.xaml 的交互逻辑
    /// </summary>
    public partial class FuDaiBatchOperat : UserControl, BaseDialog
    {
        BaseDAL dal = new BaseDAL();
        OperationStatus status;
        FuDaiBatchModel model;
        public FuDaiBatchOperat(OperationStatus status, FuDaiBatchModel model)
        {
            this.status = status;
            this.model = model;
            InitializeComponent();
            this.DataContext = this;
            if (status == OperationStatus.Edit)
            {
                tb_batchname.Text = model.BatchName;
                tb_big.Text = model.PriceMax.ToString();
                tb_small.Text = model.PriceMin.ToString();
                rich_Remark.Text = model.Remark;
            }
            BindBrand();
        }

        public void BindBrand()
        {
            var list = dal.GetAll<BrandModel>();
            ComboBoxBind.BindComboBox(com_Brand, list, "BrandCN", "BrandEN");
            if (status == OperationStatus.ADD)
            {

                com_Brand.SelectedIndex = list.Count > 0 ? 0 : -1;
            }
            else
            {
                com_Brand.SelectedIndex = list.FindIndex(x => x.BrandCN == model.Brand);
            }
        }

        public DelegateCommand CancelCommand
        {
            get
            {
                return new DelegateCommand(CloseDialog);
            }
        }

        public string Title
        {
            get
            {
                return status == OperationStatus.Edit ? "编辑批次" : "添加批次";
            }
        }

        public void CloseDialog()
        {
            FormInit.CloseDialog(this);
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            throw new NotImplementedException();
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            throw new NotImplementedException();
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            throw new NotImplementedException();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int Tag = Convert.ToInt32((sender as Button).Tag);
            switch (Tag)
            {
                case 0:
                    bool b = false;
                    FuDaiBatchModel fb = new FuDaiBatchModel();
                    fb.BatchName = tb_batchname.Text;
                    fb.PriceMax = Convert.ToDecimal(tb_big.Text);
                    fb.PriceMin = Convert.ToDecimal(tb_small.Text);
                    fb.Remark = rich_Remark.Text;
                    fb.Brand = com_Brand.Text;
                    fb.CreateTime = DateTime.Now;
                    fb.ImportErp = false;
                    if (status == OperationStatus.Edit)
                    {
                        fb.CreateTime = model.CreateTime;
                        fb.ID = model.ID;
                        fb.SellCount = model.SellCount;
                        fb.ImportErp = model.ImportErp;
                        fb.Num = model.Num;
                        b = dal.Update<FuDaiBatchModel>(fb);
                    }
                    else
                    {
                        b = dal.Add<FuDaiBatchModel>(fb);
                    }
                   
                    CCMessageBox.Show(b ? "成功" : "失败");
                    if (b)
                    {
                        Batch batch = (FormInit.FindFather(this) as FuDaiGoodsUp).temp_batch;
                        batch.LoadBatchData(1,30);
                        this.CloseDialog();
                    }
                    break;
                case 1:
                    this.CloseDialog();
                    break;
            }
        }
    }
}
