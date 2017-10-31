using _9M.Work.DbObject;
using _9M.Work.WPF_Main.Infrastrcture;
using Microsoft.Practices.Prism.Commands;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace _9M.Work.WPF_Main.Views.DataCenter
{
    /// <summary>
    /// SetPriceLog.xaml 的交互逻辑
    /// </summary>
    public partial class SetPriceLog : UserControl,BaseDialog
    {
        private BaseDAL dal = new BaseDAL();
        private ObservableCollection<WareBindModel> warelist;
        public SetPriceLog()
        {
            InitializeComponent();
            this.DataContext = this;
            warelist = new ObservableCollection<WareBindModel>();
            UpdateGoodsGrid.ItemsSource = warelist;
            Init();
        }

        public void Init()
        {
            string sql = string.Format("select top 10  CONVERT(varchar(100), submittime,20) from  T_SetPrice group by submittime order by submittime desc");
            List<string> list = dal.QueryList<string>(sql, new object[] { });
            com_time.ItemsSource = list;
            if(list.Count>0)
            {
                com_time.SelectedIndex = 0;
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
            get { return "定价日志"; }
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
            warelist.Clear();
            string sql = string.Empty;
            string wareno = tb_wareno.Text;
            if (!string.IsNullOrEmpty(wareno))
            {
                sql = string.Format(@"select Wareno,warename,price from T_SetPrice where wareno = '{0}' ", tb_wareno.Text);
            }
            else
            {
                sql = string.Format(@"select Wareno,warename,price from T_SetPrice where submittime = '{0}' ", com_time.Text);
            }
            var list = dal.QueryList<WareBindModel>(sql, new object[] { });
            list.ForEach(x => {
                warelist.Add(x);
            });
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (CCMessageBox.Show("您是否要提交修改", "提示", CCMessageBoxButton.YesNo) == CCMessageBoxResult.Yes)
            {
                //请求WebServices
                UpdateGoodsServiceReference.GetGoodsServerSoapClient cliet = new UpdateGoodsServiceReference.GetGoodsServerSoapClient();
                string Res = cliet.UpDatePrice("E045443B12BC", JsonConvert.SerializeObject(warelist));
                Dictionary<string, string> dic = (Dictionary<string, string>)JsonConvert.DeserializeObject(Res, typeof(Dictionary<string, string>));
                string code = dic["code"];
                CCMessageBox.Show(dic["msg"]);
            }
        }
    }
}
