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

namespace _9M.Work.WPF_Main.Views.Refund
{
    /// <summary>
    /// Interaction logic for RefundScanReceiptRemark.xaml
    /// </summary>
    public partial class RefundScanReceiptRemark : UserControl, BaseDialog
    {
        string Golbal_ExpressNo = string.Empty;
        _9M.Work.DbObject.BaseDAL dal = new DbObject.BaseDAL();
        public RefundScanReceiptRemark(string expressNo)
        {
            InitializeComponent();
            this.DataContext = this;
            Golbal_ExpressNo = expressNo;
            this.GetRemark();
        }


        private void GetRemark()
        {
            List<AndroidScanReceiptModel> model = dal.GetList<AndroidScanReceiptModel>(x => x.scanNo.Equals(Golbal_ExpressNo));
            tRemark.Text = model[0].remark != null ? model[0].remark : "";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dal.ExecuteSql(string.Format("update T_AndroidScanReceipt set remark='{0}' where scanNo='{1}'",tRemark.Text,Golbal_ExpressNo)) > 0)
                {
                    MessageBox.Show("保存成功", "提示");
                    RefundScanReceipt rl = ((RefundScanReceipt)FormInit.FindFather(this));
                    rl.DataBind(rl.PageSize,Convert.ToInt32(rl.CurrentPage));
                    this.CloseDialog();
                }
            }
            catch
            {

            }
        }

        #region Dialog
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
            get { return "客服备注"; }
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

        #endregion
    }
}
