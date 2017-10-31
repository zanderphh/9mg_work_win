using _9M.Work.DbObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


using _9M.Work.Model;
using _9M.Work.WPF_Common;
namespace _9M.Work.WPF_Main.Views
{
    /// <summary>
    /// BottomView.xaml 的交互逻辑
    /// </summary>
    public partial class BottomView : UserControl
    {
        public BottomView()
        {
            InitializeComponent();
            SessionUserModel User = CommonLogin.CommonUser;
            string DeptName = !User.IsAdmin ?new BaseDAL().GetSingle<DeptModel>(x => x.Id == CommonLogin.CommonUser.DeptId).DeptName:"管理员";
            string Text = string.Format("部门 ({0})   服务器:  {1}", DeptName, Login.TextConnIp);
            InfoMation.Text = Text;
            CommonBottonView = this;
        }

        public static BottomView CommonBottonView { get; set; }

    }
}
