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

namespace _9M.Work.WPF_Main.Views.Refund
{
    /// <summary>
    /// Interaction logic for CsRemark.xaml
    /// </summary>
    public partial class CsRemark : UserControl, BaseDialog
    {

        string Golbal_refundNo = string.Empty;
        _9M.Work.DbObject.BaseDAL dal = new DbObject.BaseDAL();


        public CsRemark(string refundNo)
        {
            InitializeComponent();
            this.DataContext = this;
            Golbal_refundNo = refundNo;

            

            List<ExpressionModelField> listWhere = new List<ExpressionModelField>();
            listWhere.Add(new ExpressionModelField() { Name = "refundNo", Value = refundNo });
            List<RefundModel> datasource = dal.GetList<RefundModel>(listWhere.ToArray(), new OrderModelField[] { new OrderModelField() { PropertyName = "id", IsDesc = true } });
            tRemark.Text = datasource[0].address;
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string sql = string.Format("update t_refund set address='{0}' where refundNo='{1}'", tRemark.Text, Golbal_refundNo);
            if (dal.ExecuteSql(sql)>0)
            {
                MessageBox.Show("保存成功","提示");

                Refundlist rl = ((Refundlist)FormInit.FindFather(this));
                rl.LoadRefundData(Convert.ToInt32(rl.CurrentPage), rl.PageSize);
                this.CloseDialog();
            }
        }
    }
}
