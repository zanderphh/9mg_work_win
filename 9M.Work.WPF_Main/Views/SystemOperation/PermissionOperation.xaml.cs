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
    /// PermissionOperation.xaml 的交互逻辑
    /// </summary>
    public partial class PermissionOperation : UserControl, BaseDialog
    {
        private BaseDAL dal = new BaseDAL();
        private OperationStatus type;
        private PermissionModel model;
        public PermissionOperation(OperationStatus type, PermissionModel model)
        {
            this.DataContext = this;
            this.type = type;
            this.model = model;
            InitializeComponent();
            InitControl();
        }


        public void InitControl()
        {
            if (type == OperationStatus.Edit)
            {
                PermissName.Text = model.Pname;
                PermissUrl.Text = model.Url;
                PermissIco.Text = model.Ico;
                PermissOrder.Text = model.OrderId.ToString();
                if (model.ParentId != 0)
                {
                    PermissFather.Text = dal.GetSingle<PermissionModel>(x => x.Id == model.Id).Pname;
                }
            }
            else if (type == OperationStatus.ADDChild)
            {
                PermissFather.Text = model.Pname;
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
            get { return type == OperationStatus.Edit ? "权限编辑" : "权限添加"; }
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

        private void OpenIco(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int Tag = Convert.ToInt32((sender as Button).Tag);
            switch (Tag)
            {
                case 0:
                   bool bs;
                    if (string.IsNullOrEmpty(PermissName.Text))
                    {
                        CCMessageBox.Show("请完整的填写内容");
                        return;
                    }
                    PermissionModel Opmodel = new PermissionModel();
                    Opmodel.OrderId = string.IsNullOrEmpty(PermissOrder.Text) ? 0 : Convert.ToInt32(PermissOrder.Text);
                    Opmodel.Ico = PermissIco.Text;
                    if (type == OperationStatus.ADDChild)
                    {
                        Opmodel.ParentId = model.Id;
                    }
                    else
                    {
                        Opmodel.ParentId = type != OperationStatus.ADD ? model.ParentId : 0;
                    }
                    Opmodel.Pname = PermissName.Text;
                    Opmodel.Url = PermissUrl.Text;
                    
                    if (type == OperationStatus.ADD || type == OperationStatus.ADDChild)
                    {
                        bs = dal.Add<PermissionModel>(Opmodel);
                    }
                    else
                    {
                        Opmodel.Id = model.Id;
                        bs = dal.Update<PermissionModel>(Opmodel);
                    }
                    if (bs)
                    {
                        FormInit.CloseDialog(this);
                       // AuthorityAllot.CommonAuthorityAllot.BindALL();
                        AuthorityAllot.CommonAuthorityAllot.BindUserPermission();
                    }
                    else
                    {
                        CCMessageBox.Show("操作失败");
                    }
                    break;
                case 1: //关闭窗体
                    CloseDialog();
                    break;
            }
        }
    }
}
