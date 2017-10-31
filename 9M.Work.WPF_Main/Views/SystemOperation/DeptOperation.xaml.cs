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
    /// DeptOperation.xaml 的交互逻辑
    /// </summary>
    public partial class DeptOperation : UserControl, BaseDialog
    {
        private BaseDAL dal = new BaseDAL();
        private OperationStatus status;
        private DeptModel dept;
        public DeptOperation(OperationStatus status, DeptModel dept)
        {
            this.DataContext = this;
            this.status = status;
            this.dept = dept;
            InitializeComponent();
            if (status == OperationStatus.Edit)
            {
                Tb_Dept.Text = dept.DeptName;
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
            get { return status == OperationStatus.Edit ? "修改部门" : "添加部门"; }
        }

        public bool IsNavigationTarget(Microsoft.Practices.Prism.Regions.NavigationContext navigationContext)
        {
            return true;
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
                case 0:
                    bool bs;
                    string deptname = Tb_Dept.Text;
                    if (string.IsNullOrEmpty(Tb_Dept.Text))
                    {
                        CCMessageBox.Show("填写部门名称");
                        return;
                    }
                    if (status == OperationStatus.ADD)
                    {
                        bs = dal.Add<DeptModel>(new DeptModel() { DeptName = deptname });
                    }
                    else
                    {
                        dept.DeptName = deptname;
                        bs = dal.Update<DeptModel>(dept);
                    }
                    if (bs)
                    {
                        FormInit.CloseDialog(this);
                        AuthorityAllot.CommonAuthorityAllot.BindALL();
                    }
                    else
                    {
                        CCMessageBox.Show("编辑失败");
                    }
                    break;
                case 1:
                    FormInit.CloseDialog(this);
                    break;
            }
        }
    }
}
