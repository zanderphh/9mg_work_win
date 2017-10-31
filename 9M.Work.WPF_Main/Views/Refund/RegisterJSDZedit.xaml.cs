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
    /// Interaction logic for RegisterJSDZedit.xaml
    /// </summary>
    public partial class RegisterJSDZedit : UserControl, BaseDialog
    {

        BaseDAL dal = new BaseDAL();

        RegisterJSDZModel Golbal_Model = null;

        public RegisterJSDZedit(RegisterJSDZModel rm)
        {
            InitializeComponent();
            this.DataContext = this;
            cmbRegType.SelectedValue = -1;
            Golbal_Model = rm;
            InfoBind();
        }

        #region 信息绑定

        private void InfoBind()
        {
            if (Golbal_Model != null)
            {
                txtDiscName.Text = Golbal_Model.distributorNick;
                txtRefundMoney.Text = Golbal_Model.refundMoney.ToString();
                txtRemark.Text = Golbal_Model.remark;
                txtTradeNo.Text = Golbal_Model.tradeNo;
                txtSku.Text = Golbal_Model.sku;
                cmbRegType.SelectedValue = Golbal_Model.regType;
            }
        }

        #endregion

        #region 保存提交

        private void btn_sure(object sender, RoutedEventArgs e)
        {
            bool isAdd = false;

            if (txtRefundMoney.Text.Equals("")) { MessageBox.Show("退款金额不能为空！", "提示"); return; }
            if (txtSku.Text.Equals("")) { MessageBox.Show("货品编号！", "提示"); return; }
            if (txtDiscName.Text.Equals("")) { MessageBox.Show("分销商不能为空！", "提示"); return; }

            if (MessageBox.Show("确认提交并保存?", "提示", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                if (Golbal_Model == null)
                {
                    Golbal_Model = new RegisterJSDZModel();
                    isAdd = true;
                }

                Golbal_Model.distributorNick = txtDiscName.Text;
                Golbal_Model.refundMoney = Convert.ToDecimal(txtRefundMoney.Text);
                Golbal_Model.remark = txtRemark.Text;
                Golbal_Model.tradeNo = txtTradeNo.Text;
                Golbal_Model.sku = txtSku.Text;
                Golbal_Model.regType =(int)cmbRegType.SelectedValue;

                if (isAdd)
                {
                    Golbal_Model.registerOperator = WPF_Common.CommonLogin.CommonUser.UserName;
                    Golbal_Model.registerTime = DateTime.Now;

                    try
                    {
                        if (dal.Add(Golbal_Model))
                        {
                            MessageBox.Show("提交保存成功！", "提示");
                        }
                        else
                        {
                            MessageBox.Show("提交保存失败！", "提示");
                        }
                    }
                    catch (Exception err)
                    {
                        MessageBox.Show(string.Format("错误提示:{0}", err.Message.ToString()), "提示");
                    }
                }
                else
                {
                    try
                    {
                        if (dal.Update(Golbal_Model))
                        {
                            MessageBox.Show("提交保存成功！", "提示");
                        }
                        else
                        {
                            MessageBox.Show("提交保存失败！", "提示");
                        }
                    }
                    catch (Exception err)
                    {
                        MessageBox.Show(string.Format("错误提示:{0}", err.Message.ToString()), "提示");
                    }
                }
            }

            RegisterJSDZ rl = ((RegisterJSDZ)FormInit.FindFather(this));
            rl.SourceDataBind(rl.PageIndex, rl.PageSize);
            CloseDialog();
        }

        #endregion

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
            get { return "添加商品"; }
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
