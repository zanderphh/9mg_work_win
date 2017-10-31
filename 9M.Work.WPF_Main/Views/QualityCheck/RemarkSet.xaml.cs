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

namespace _9M.Work.WPF_Main.Views.QualityCheck
{
    /// <summary>
    /// RemarkSet.xaml 的交互逻辑
    /// </summary>
    public partial class RemarkSet : UserControl, BaseDialog
    {
        private BaseDAL dal = new BaseDAL();
        private string WareNo = string.Empty;
        public RemarkSet(string WareNo)
        {
            InitializeComponent();
            this.DataContext = this;
            this.WareNo = WareNo;
            tb_remark.Text = dal.GetSingle<WareModel>(x => x.WareNo.Equals(WareNo)).Remark;
        }

        private void Btn_CommandClick(object sender, RoutedEventArgs e)
        {
            int Tag = Convert.ToInt32((sender as Button).Tag);
            switch (Tag)
            {
                case 0://撤销
                    string apendtext = com_text.Text;
                    if (!string.IsNullOrEmpty(apendtext))
                    {
                        tb_remark.Text = tb_remark.Text.Replace(apendtext, "");
                    }
                    break;
                case 1:
                    string sql = string.Format("update T_WareList set remark = '{0}' where wareno = '{1}'", tb_remark.Text, WareNo);
                    dal.ExecuteSql(sql);
                    CloseDialog();
                    break;
                case 2:
                    CloseDialog();
                    break;
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
            get { return "编辑备注"; }
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

        //下拉眶选择


        private void Com_category_DropDownClosed(object sender, EventArgs e)
        {
            string apendtext = com_text.Text;
            if (!string.IsNullOrEmpty(apendtext))
            {
                if (!tb_remark.Text.Contains(apendtext))
                {
                    if (string.IsNullOrEmpty(tb_remark.Text))
                    {
                        tb_remark.Text = apendtext;
                    }
                    else
                    {
                        tb_remark.Text = tb_remark.Text + " " + apendtext;
                    }

                }
            }
        }
    }
}
