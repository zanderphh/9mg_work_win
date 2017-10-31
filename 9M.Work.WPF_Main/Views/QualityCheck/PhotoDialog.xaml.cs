using _9M.Work.DbObject;
using _9M.Work.Model;
using _9M.Work.Model.Log;
using _9M.Work.Utility;
using _9M.Work.WPF_Common;
using _9M.Work.WPF_Common.WpfBind;
using _9M.Work.WPF_Main.ControlTemplate.PrintTemplate;
using _9M.Work.WPF_Main.FrameWork;
using _9M.Work.WPF_Main.Infrastrcture;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Threading;

namespace _9M.Work.WPF_Main.Views.QualityCheck
{
    /// <summary>
    /// PhotoDialog.xaml 的交互逻辑
    /// </summary>
    public partial class PhotoDialog : UserControl, BaseDialog
    {
        private string Color = string.Empty;
        private List<string> sqllist;
        private BaseDAL dal = new BaseDAL();
        private PrintHelper ph = new PrintHelper();
        public PhotoDialog(DateTime PhotoTime, DateTime SubmitTime, List<string> sqllist, string Color)
        {
            InitializeComponent();
            this.DataContext = this;
            this.sqllist = sqllist;
            this.Color = Color;
            string Sql = string.Format("select  * from  dbo.T_WareList where createTime>='{0}' and createTime<='{1}'", PhotoTime, SubmitTime);
            List<WareModel> modellist = dal.QueryList<WareModel>(Sql, new object[] { });
            DataTable dt = ListToDataTable.ConvertToDataTable<WareModel>(modellist);
            ImageBox.BindImage(dt, null, "ImageKey", "WareNo", 1.33);
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
            get { return "查看图片"; }
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
        public static string GuidTo16String()
        {
            long i = 1;
            foreach (byte b in Guid.NewGuid().ToByteArray())
                i *= ((int)b + 1);
            return string.Format("{0:x}", i - DateTime.Now.Ticks);
        }
        private void Btn_CommandClick(object sender, RoutedEventArgs e)
        {
            int Tag = Convert.ToInt32((sender as Button).Tag);
            switch(Tag)
            {
                case 1 : //新建
                    bool bs = dal.ExecuteTransaction(sqllist, null);
                    if (!bs)
                    {
                        CCMessageBox.Show("添加失败");
                        return;
                    }
                    else
                    {
                       
                        WaresSubsec ws = FormInit.FindFather(this) as WaresSubsec;
                        string WareNo = ws.tb_WareNo.Text;
                        //日志
                       // string LogSql = string.Format(@"insert into T_QualityCheck_Log values('{0}','{1}','{2}','{3}')", CommonLogin.CommonUser.UserName, WareNo,"新建",DateTime.Now);
                        //dal.ExecuteSql(LogSql);
                        GoodsLogModel log = new GoodsLogModel() {
                            GoodsNo = WareNo,
                            LogType = Convert.ToInt32(GoodsLogType.Subsec),
                            LogTime = DateTime.Now,
                            UserName = CommonLogin.CommonUser.UserName,
                            DoEvent = string.Format("新建货品;【{0}】", WareNo),
                        };
                        dal.Add<GoodsLogModel>(log);
                        //打印一个标签
                        ph.PrintGoods (new List<string>() { WareNo });
                        //打印一个样品标
                        PrintLabeModel m = new PrintLabeModel();
                        m.WareNo = WareNo;
                        m.SpecName = Color;
                        ph.PrintSample(WareNo, Color);
                        //保存成功的时候生成一个GUID为图片保存做准备
                        ws.ImageGuid = GuidTo16String();
                        ws.tb_FyiCode.Text = "";
                        ws.Image_Box.Source = new BitmapImage(new Uri(@"/9M.Work.WPF_Main;component/Images/nopic.jpg", UriKind.RelativeOrAbsolute));

                        //切换到详情
                        ws.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                           (Action)(() => { ws.TabControlIt.SelectedIndex = 2; }));
                        ws.lab_specwareno.Content = WareNo;
                        ws.tb_query.Text = WareNo;
                        ImageBind.BindImageBox(ws.img_specbox, @"/9M.Work.WPF_Main;component/Images/nopic.jpg", false);
                        ws.BindSpecImagePanel(WareNo);
                        ws.tb_WareNo.Text = "";
                        CloseDialog();
                    }
                    break;
                case 2:  //取消
                    CloseDialog();
                    break;
            }
        }
    }
}
