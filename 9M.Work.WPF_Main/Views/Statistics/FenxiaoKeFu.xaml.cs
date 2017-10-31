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

namespace _9M.Work.WPF_Main.Views.Statistics
{
    /// <summary>
    /// FenxiaoKeFu.xaml 的交互逻辑
    /// </summary>
    public partial class FenxiaoKeFu : UserControl, BaseDialog
    {
        private BaseDAL dal = new BaseDAL();
        public FenxiaoKeFu()
        {
            InitializeComponent();
            this.DataContext = this;
            Init();
        }

        public void Init()
        {
            List<FenxiaoKeFuModel> list = dal.QueryList<FenxiaoKeFuModel>("select * from T_FenxiaoKeFu", new object[] { });
            string Res = string.Empty;
            list.ForEach(x =>
            {
                Res += x.Nick + "\n";
            });
            Tb_Kefu.Text = Res.TrimEnd('\n');
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
            get { return "编辑客服"; }
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
                case 0:
                    List<string> KeFuList = Tb_Kefu.Text.Split('\n').Where(x => !string.IsNullOrEmpty(x)).ToList();
                    KeFuList = KeFuList.Select(x => x.Replace("\r", "").Trim()).Distinct().ToList();
                    List<string> sqllist = new List<string>();
                    sqllist.Add("delete from T_FenxiaoKeFu");
                    KeFuList.ForEach(x =>
                    {
                        sqllist.Add(string.Format("insert into T_FenxiaoKeFu values ('{0}')", x));
                    });
                    bool bs = dal.ExecuteTransaction(sqllist,null);
                    if (bs)
                    {
                        FormInit.CloseDialog(this);
                    }
                    else
                    {
                        CCMessageBox.Show("设置失败");
                    }
                    break;
                case 1:
                    CloseDialog();
                    break;
            }
        }
    }
}
