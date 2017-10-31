using _9M.Work.DbObject;
using _9M.Work.Model;
using _9M.Work.Utility;
using _9M.Work.WPF_Common;
using _9M.Work.WPF_Common.ValueObjects;
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
    /// Interaction logic for FinanceJSDZ.xaml
    /// </summary>
    public partial class FinanceJSDZ : UserControl, BaseDialog
    {
        BaseDAL dal = new BaseDAL();
        int Golbal_RefundDetailId = 0;
        string Golbal_RefundNo = string.Empty;

        public FinanceJSDZ(int refundDetailId, string nickName, string goodsno, decimal? refundPrice, string refundNo)
        {
            InitializeComponent();
            this.DataContext = this;
            txtDiscName.Text = nickName;
            txtGoodsNo.Text = goodsno;
            txtRefundMoney.Text = refundPrice.ToString();
            Golbal_RefundDetailId = refundDetailId;
            Golbal_RefundNo = refundNo;
        }


        private void btn_sure(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("请再次确认金额， 继续转入?", "提示", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    //修改明细表中退款完成字段和即时到帐字段
                    //明细表是否全部处理完成(修改字段financeRefundStatus->FinanceRefundStatusEnum 未退款，退款中,退款完成)
                    //财务全部处理完成 改为查看状态(完成)并修改完成时间
                    List<string> sql = new List<string>();
                    sql.Add(string.Format(@"insert into T_RegisterJSDZ(tradeNo,sku,distributorNick,refundMoney,remark,registerTime,registerOperator,checkOperator,isCheck,regType)
                    select refundNo,sku,'{0}','{1}','{2}',getdate(),'{3}','',0,{5} from T_RefundDetail where id={4}", txtDiscName.Text, txtRefundMoney.Text, "退货转入", WPF_Common.CommonLogin.CommonUser.UserName, Golbal_RefundDetailId,(int)JSDZ_RegisterTypeEnum.TH));

                    sql.Add(string.Format("update T_RefundDetail set isFinanceEnd=1,IsJSDZ=1 where id={0}", Golbal_RefundDetailId));
                    sql.Add(string.Format(@"  if not exists(select id from T_RefundDetail where isFinanceEnd=0 and refundNo='{0}') --是否有未处理完的明细单
                                              begin
                                                   update T_Refund set refundStatus={2},financeRefundStatus={3},endTime=GETDATE() where refundNo='{0}' 
                                              end
                                            else 
                                              begin
                                                   if exists(select id from T_RefundDetail where confirmReceipt={6} and isFinanceEnd=0 and refundNo='{0}')
                                                   begin
                                                         update T_Refund set refundStatus={7},financeRefundStatus={1} where refundNo='{0}' 
                                                   end
                                                   else if exists(select id from T_RefundDetail where confirmReceipt={4} and refundNo='{0}')
                                                    begin
                                                       update T_Refund set refundStatus={5},financeRefundStatus={1} where refundNo='{0}' 
                                                    end
                      
                                              end",
                                                  Golbal_RefundNo,
                                                      (int)FinanceRefundStatusEnum.Refunding,
                                                      (int)RefundHandleStatusEnum.look,
                                                      (int)FinanceRefundStatusEnum.RefundEnd,
                                                      (int)ReceiptStatus.watting,
                                                      (int)RefundSatausEnum.noUnpacking,
                                                      (int)ReceiptStatus.yes,
                                                      (int)RefundSatausEnum.financeOperator
                                                      ));

                    sql.Add(SQLTxt.GetInserLogSql(CommonLogin.CommonUser.UserName, EnumHelper.GetEnumTextVal((int)RefundOperatorEnum.Receive_No, typeof(RefundOperatorEnum)), Golbal_RefundNo,""));
                    if (dal.ExecuteTransaction(sql, null))
                    {
                        MessageBox.Show("操作成功", "提示");
                        var uc = ((UnpackingCheck)FormInit.FindFather(this, typeof(UnpackingCheck)));
                        uc.dataBind(Golbal_RefundNo);

                    }
                    else
                    {
                        MessageBox.Show("操作失败", "提示");
                    }
                }
                catch (Exception err)
                {
                    MessageBox.Show("失败原因:" + err.Message.ToString(), "提示");
                }
            }
        }

        #region Dialog
        public Microsoft.Practices.Prism.Commands.DelegateCommand CancelCommand
        {
            get { return new DelegateCommand(CloseDialog); }
        }

        public void CloseDialog()
        {
            FormInit.CloseDialog(this, 2);
        }

        public string Title
        {
            get { return "分销退货转即时到帐"; }
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
