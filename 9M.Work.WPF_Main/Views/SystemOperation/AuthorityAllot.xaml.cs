using _9M.Work.DbObject;
using _9M.Work.Model;
using _9M.Work.WPF_Common;
using _9M.Work.WPF_Common.WpfBind;
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

namespace _9M.Work.WPF_Main.Views.SystemOperation
{
    /// <summary>
    /// AuthorityAllot.xaml 的交互逻辑
    /// </summary>
    public partial class AuthorityAllot : UserControl
    {
        private SessionUserModel User = CommonLogin.CommonUser;
        private BaseDAL dal = new BaseDAL();
        public AuthorityAllot()
        {
            InitializeComponent();
            CommonAuthorityAllot = this;
            BindALL();
        }

        public static AuthorityAllot CommonAuthorityAllot { get; set; }

        public void BindALL()
        {
            BindDept();
            BindUserInfo();
            BindUserPermission();
        }

        public void BindDept()
        {
            List<DeptModel> list = dal.GetAll<DeptModel>();
            ListBoxBind.BindListBox(List_Dept, list, "DeptName", "Id");
            if (list.Count > 0)
            {
                DeptModel dm=list.Where(x => x.Id == User.DeptId).Count()>0? list.Where(x => x.Id == User.DeptId).Single() : list[0];
                DeptModel selectItem = User.IsDeptAdmin ?dm: list[0];
                List_Dept.SelectedItem = selectItem;
            }
        }

        //绑定权限的TREE
        //public void BindPermission()
        //{
        //    List<PermissionModel> list = dal.GetAll<PermissionModel>().OrderBy(x => x.OrderId).ToList();
        //    List<TreeModel> treelist = TreeViewDataConverter.ConverterTreeFromPermission(list);
        //    PermissionTree.ItemsSourceData = treelist;
        //}

        //绑定用户
        public void BindUserInfo()
        {
            DeptModel ms = (DeptModel)List_Dept.SelectedItem;
            if (ms != null)
            {
                List<UserInfoModel> userlist = dal.GetList<UserInfoModel>(x => x.DeptId == ms.Id).ToList();
                ListBoxBind.BindListBox(List_User, userlist, "UserName", "Id");
                //if (userlist.Count > 0)
                //{
                //    List_User.SelectedIndex = 0;
                //}
            }
        }
        //得到用户的权限
        List<UserPermissionModel> UserPerlist;
        public void BindUserPermission()
        {
            UserInfoModel model = (UserInfoModel)List_User.SelectedItem;
            int userid = model == null ? 0 : model.Id;
            string sql = string.Format(@"select tba.*, case when tbb.id IS NULL then 0 else 1 end  as CanDo  from T_permission tba left join
(select a.id  from T_permission a join T_userPermission b on a.id = b.PermissionId
            join T_userinfo c on b.UserId = c.id where c.id = {0}) tbb on tba.id = tbb.id", userid);
            UserPerlist = dal.QueryList<UserPermissionModel>(sql, new object[] { });
            List<TreeModel> treelist = TreeViewDataConverter.ConverterTreeFromPermission(UserPerlist);
            PermissionTree.ItemsSourceData = treelist;
        }

        #region 部门
        private void Btn_DeptClick(object sender, RoutedEventArgs e)
        {
            int Tag = Convert.ToInt32((sender as Button).Tag);
            DeptModel selectitem = (DeptModel)List_Dept.SelectedItem;
            switch (Tag)
            {
                case 0: //新建部门
                    FormInit.OpenDialog(this, new DeptOperation(OperationStatus.ADD, selectitem), false);
                    break;
                case 1: //修改部门
                    if (List_Dept.SelectedItem == null)
                    {
                        CCMessageBox.Show("请选中部门");
                    }
                    else
                    {
                        FormInit.OpenDialog(this, new DeptOperation(OperationStatus.Edit, selectitem), false);
                    }
                    break;
                case 2: //删除部门
                    if (selectitem == null)
                    {
                        CCMessageBox.Show("请选中部门");
                    }
                    else
                    {
                        if (CCMessageBox.Show("是否要删除部门", "提示", CCMessageBoxButton.YesNo) == CCMessageBoxResult.Yes)
                        {
                            dal.Delete<DeptModel>(selectitem);
                            this.BindALL();
                        }
                    }
                    break;
            }
        }


        private void List_Dept_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox box = sender as ListBox;
            DeptModel ms = (DeptModel)box.SelectedItem;
            if (!User.IsAdmin)
            {
                if (User.IsDeptAdmin)
                {
                    BindUserInfo();
                    DeptModel Dmodel = (List_Dept.ItemsSource as List<DeptModel>).Where(x => x.Id == User.DeptId).Single();
                    if (ms != Dmodel)
                    {
                        List_Dept.SelectedItem = Dmodel;
                        if (ms != null)
                        {
                            CCMessageBox.Show("您没有权限编辑其他部门的权限");
                        }
                        return;
                    }

                }
                else
                {
                    CCMessageBox.Show("您不是部门管理员");
                }
            }
            else
            {
                BindUserInfo();
            }

        }
        #endregion

        #region 员工
        private void Btn_UserClick(object sender, RoutedEventArgs e)
        {
            int Tag = Convert.ToInt32((sender as Button).Tag);
            UserInfoModel selectitem = (UserInfoModel)List_User.SelectedItem;
            DeptModel selectdept = (DeptModel)List_Dept.SelectedItem;
            switch (Tag)
            {
                case 0: //新建员工
                    FormInit.OpenDialog(this, new UserOperation(OperationStatus.ADD, selectitem, selectdept), false);
                    break;
                case 1:  //修改员工
                    if (selectitem != null && selectdept != null)
                    {
                        FormInit.OpenDialog(this, new UserOperation(OperationStatus.Edit, selectitem, selectdept), false);
                    }
                    break;
                case 2:  //删除员工
                    if (selectitem == null)
                    {
                        CCMessageBox.Show("请选中用户");
                    }
                    else
                    {
                        if (CCMessageBox.Show("是否要删除用户", "提示", CCMessageBoxButton.YesNo) == CCMessageBoxResult.Yes)
                        {
                            dal.Delete<UserInfoModel>(selectitem);
                            this.BindALL();
                        }
                    }
                    break;
            }
        }

        private void List_User_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.BindUserPermission();
        }
        #endregion

        #region 权限
        private void Btn_AuthorityClick(object sender, RoutedEventArgs e)
        {
            if (!User.IsAdmin)
            {
                CCMessageBox.Show("您没有管理员权限");
                return;
            }
            int Tag = Convert.ToInt32((sender as Button).Tag);
            TreeModel nodel = PermissionTree.SelectNode;
            if (nodel == null && Tag != 0)
            {
                CCMessageBox.Show("请选中一个节点");
                return;
            }
            int Id = nodel != null ? Convert.ToInt32(nodel.Id) : 0;
            PermissionModel model = null;
            switch (Tag)
            {
                case 0:  //新建父权限
                    FormInit.OpenDialog(this, new PermissionOperation(OperationStatus.ADD, model), false);
                    break;
                case 1: //新建子权限
                    if (nodel.ParentId != 0)
                    {
                        CCMessageBox.Show("只能新建两层的子集");
                        return;
                    }
                    model = dal.GetSingle<PermissionModel>(x => x.Id == Id);
                    FormInit.OpenDialog(this, new PermissionOperation(OperationStatus.ADDChild, model), false);
                    break;
                case 2:  //修改权限
                    model = dal.GetSingle<PermissionModel>(x => x.Id == Id);
                    FormInit.OpenDialog(this, new PermissionOperation(OperationStatus.Edit, model), false);
                    break;
                case 3:  //删除权限
                    if (CCMessageBox.Show("是否要删除权限", "提示", CCMessageBoxButton.YesNo) == CCMessageBoxResult.Yes)
                    {
                        model = dal.GetSingle<PermissionModel>(x => x.Id == Id);
                        dal.Delete<PermissionModel>(model);
                        this.BindUserPermission();
                    }
                    break;
            }
        }
        #endregion

        private void SubmitAuthority(object sender, RoutedEventArgs e)
        {
            UserInfoModel selectitem = (UserInfoModel)List_User.SelectedItem;
            if (selectitem == null)
            {
                CCMessageBox.Show("请选择用户");
                return;
            }
            IList<TreeModel> list = PermissionTree.CheckedItemsIgnoreRelation();
            //为选中的节点用户修改权限
            List<string> SqlList = new List<string>();
            SqlList.Add("delete from T_userPermission where UserId = " + selectitem.Id);
            foreach (TreeModel m in list)
            {
                if (m.ParentId != 0)
                {
                    SqlList.Add(string.Format(@"insert into T_userPermission values({0},{1})", selectitem.Id, m.Id));
                }
            }
            bool bs = dal.ExecuteTransaction(SqlList, null);
            if (!bs)
            {
                CCMessageBox.Show("修改失败");
            }
            else
            {
                CCMessageBox.Show("设置成功");
            }
        }



    }
}
