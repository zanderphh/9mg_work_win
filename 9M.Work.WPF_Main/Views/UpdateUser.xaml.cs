using _9M.Work.DbObject;
using _9M.Work.Model;
using _9M.Work.WPF_Common;
using _9M.Work.WPF_Main.Infrastrcture;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
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

namespace _9M.Work.WPF_Main.Views
{
    /// <summary>
    /// UpdateUser.xaml 的交互逻辑
    /// </summary>
    public partial class UpdateUser : UserControl, BaseDialog
    {
        BaseDAL db = new BaseDAL();
        public UpdateUser()
        {
            this.DataContext = this;
            InitializeComponent();

          
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            int Tag = Convert.ToInt32(btn.Tag);
            switch (Tag)
            { 
                case 1:  //确定
                    string pwd = Pwd.Password;
                    string pwdagain = PwdAgain.Password;
                    if (string.IsNullOrEmpty(pwd) || string.IsNullOrEmpty(pwdagain))
                    {
                        CCMessageBox.Show("完整填写密码");
                        return;
                    }
                    else if(!pwd.Equals(pwdagain))
                    {
                        CCMessageBox.Show("两次密码输入不一致");
                        return;
                    }
                    SessionUserModel sum = CommonLogin.CommonUser;
                    UserModel model = new UserModel() { Id = sum.Id,UserName = sum.UserName,PassWord = sum.PassWord };
                    model.PassWord = pwd;

                    bool b =db.Update<UserModel>(model);
                    if (b )
                    {
                        FormInit.CloseDialog(this);
                    }
                    else
                    {
                        CCMessageBox.Show("修改出错");
                    }
                    break;
                case 2:  //取消
                    FormInit.CloseDialog(this);
                    break;
                case 3:  //关闭窗体
                    FormInit.CloseDialog(this);
                    break;
            
            }
        }


        public DelegateCommand CancelCommand
        {
            get { return new DelegateCommand(CloseDialog); }
        }


        public void CloseDialog()
        {
            FormInit.CloseDialog(this);
        }

        public bool IsNavigationTarget(Microsoft.Practices.Prism.Regions.NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(Microsoft.Practices.Prism.Regions.NavigationContext navigationContext)
        {
           
        }

        public void OnNavigatedTo(Microsoft.Practices.Prism.Regions.NavigationContext navigationContext)
        {
           
        }


        public string Title
        {
            get { return "修改密码"; } 
        }
    }
}
