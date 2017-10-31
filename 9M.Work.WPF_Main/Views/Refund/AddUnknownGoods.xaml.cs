using _9M.Work.DbObject;
using _9M.Work.Model;
using _9M.Work.Utility;
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
    /// Interaction logic for AddUnknownGoods.xaml
    /// </summary>
    public partial class AddUnknownGoods : UserControl, BaseDialog
    {

        BaseDAL dal = new BaseDAL();

        public UnknownGoodsModel model
        { get; set; }

        public AddUnknownGoods(UnknownGoodsModel pModel)
        {
            InitializeComponent();
            this.DataContext = this;
            cbxCompany.ItemsSource = EnumHelper.GetEnumList(typeof(ExpressCompanyEnum));
            model = pModel;
            if (string.IsNullOrEmpty(model.ExpressCompany))
            {
                model.ExpressCompany = "-请选择-";
            }
        }

        #region 保存

        private void btn_Save(object sender, RoutedEventArgs e)
        {
            if (model.id.Equals(0))
            {
                try
                {
                    model.regTime = DateTime.Now;
                    model.regEmployee = WPF_Common.CommonLogin.CommonUser.UserName;
                    if (dal.Add(model))
                    {
                        MessageBox.Show("添加成功", "提示");
                        unknownGoods rl = ((unknownGoods)FormInit.FindFather(this));
                        rl.ItemSourceBind();
                        CloseDialog();
                    }
                    else
                    {
                        MessageBox.Show("添加失败", "提示");
                    }
                }
                catch (Exception err)
                {
                    MessageBox.Show("错误提示" + err.Message.ToString(), "提示");
                }
            }
            else
            {
                try
                {
                    if (dal.Update(model))
                    {
                        MessageBox.Show("编辑成功", "提示");
                        unknownGoods rl = ((unknownGoods)FormInit.FindFather(this));
                        rl.ItemSourceBind();
                        CloseDialog();
                    }
                    else
                    {
                        MessageBox.Show("编辑失败", "提示");
                    }
                }
                catch (Exception err)
                {
                    MessageBox.Show("错误提示" + err.Message.ToString(), "提示");
                }
            }
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
            get { return "未知包裹管理"; }
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
