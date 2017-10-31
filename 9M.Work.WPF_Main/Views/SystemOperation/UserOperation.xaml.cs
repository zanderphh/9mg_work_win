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
    /// UserOperation.xaml 的交互逻辑
    /// </summary>
    public partial class UserOperation : UserControl, BaseDialog
    {
        private BaseDAL dal = new BaseDAL();
        private DeptModel dept;
        private OperationStatus status;
        private UserInfoModel user;
        public UserOperation(OperationStatus status, UserInfoModel user, DeptModel dept)
        {
            this.dept = dept;
            this.status = status;
            this.user = user;
            this.DataContext = this;
            InitializeComponent();
            Init();
        }

        public void Init()
        {
            com_dept.Items.Add(dept.DeptName);
            com_dept.SelectedIndex = 0;
            com_dept.IsEnabled = false;
            if (status == OperationStatus.Edit)
            {
                tb_user.Text = user.UserName;
                tb_password.Password = user.PassWord;
                tb_Alias.Text = user.Alias;
                com_isamdin.SelectedIndex = user.IsDeptAdmin ? 1 : 0;
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
            get { return status == OperationStatus.Edit ? "修改用户" : "添加用户"; }
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
                case 0:  //确定 
                    bool bs;
                    if (string.IsNullOrEmpty(tb_password.Password) || string.IsNullOrEmpty(tb_user.Text))
                    {
                        CCMessageBox.Show("请完整的填写内容");
                        return;
                    }
                    UserInfoModel model = new UserInfoModel();
                    model.DeptId = dept.Id;
                    model.IsDeptAdmin = com_isamdin.SelectedIndex == 1;
                    model.PassWord = tb_password.Password;
                    model.UserName = tb_user.Text;
                    model.Alias = tb_Alias.Text;
                    if(!string.IsNullOrEmpty(model.Alias))
                    {
                        if (dal.QueryList<UserInfoModel>(string.Format("select * from dbo.T_userinfo where Alias='{0}'", model.Alias)).Count > 0)
                        {
                            CCMessageBox.Show("不可添加重复的代号");
                            return;
                        }
                    }
                    
                    if (status == OperationStatus.ADD)
                    {
                        bs = dal.Add<UserInfoModel>(model);
                    }
                    else
                    {
                        model.Id = user.Id;
                        bs = dal.Update<UserInfoModel>(model);
                    }
                    if (bs)
                    {
                        FormInit.CloseDialog(this);
                        AuthorityAllot.CommonAuthorityAllot.BindALL();
                    }
                    else
                    {
                        CCMessageBox.Show("操作失败");
                    }
                    break;
                case 1:  //取消
                    FormInit.CloseDialog(this);
                    break;
            }
        }
    }
}
