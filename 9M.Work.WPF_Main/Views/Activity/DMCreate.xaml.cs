using _9M.Work.DbObject;
using _9M.Work.WPF_Main.FrameWork;
using _9M.Work.WPF_Main.Infrastrcture;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.Data;
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

namespace _9M.Work.WPF_Main.Views.Activity
{
    /// <summary>
    /// Interaction logic for DMCreate.xaml
    /// </summary>
    public partial class DMCreate : UserControl
    {
        BaseDAL dal = new BaseDAL();

        public DMCreate()
        {
            InitializeComponent();
            this.DataContext = this;
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
            get { return "打印DM暗号"; }
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

        private void btn_check(object sender, RoutedEventArgs e)
        {
            if (txtCode.Password.Trim().Length.Equals(0))
            {
                MessageBox.Show("验证码不能为空", "提示");
                return;
            }
            else
            {
                if (txtCode.Password.Trim().Equals("1990"))
                {
                    gConfirm.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    MessageBox.Show("验证码错误", "提示");
                }
            }
        }

        private void btn_print(object sender, RoutedEventArgs e)
        {
            try
            {
                string money = txtMoney.Text.ToString().Trim();
                string number = txtNumber.Text.ToString().Trim();

                if (money.Length > 0 && number.Length > 0)
                {
                    string sql = string.Format("select top {0} code as [暗码],{1} as [金额] from T_FxDM where isGrant=0 order by id asc", number.Trim(), money.Trim());
                    DataTable dt = dal.QueryDataTable(sql, new object[] { });
                    PrintHelper help = new PrintHelper();
                    help.PrintDM(dt);

                    sql = string.Format(@"update  T_FxDM set isGrant=1,grantTime=getdate(),money={0} where 
                                           id in(select top {1} id from T_FxDM where isGrant=0 order by id asc)", money.Trim(), number.Trim());
                    if (dal.ExecuteSql(sql) > 0)
                    {
                        MessageBox.Show("创建完成，正在打印标签");
                    }
                    else
                    {
                        MessageBox.Show("创建失败！");
                    }
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message.ToString());
            }
        }
    }
}
