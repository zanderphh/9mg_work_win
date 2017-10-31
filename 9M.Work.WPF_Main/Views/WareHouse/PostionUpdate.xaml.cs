using _9M.Work.AOP;
using _9M.Work.DbObject;
using _9M.Work.ErpApi;
using _9M.Work.Model;
using _9M.Work.Model.Log;
using _9M.Work.WPF_Common;
using _9M.Work.WPF_Main.ControlTemplate;
using _9M.Work.WPF_Main.FrameWork;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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

namespace _9M.Work.WPF_Main.Views.WareHouse
{

    /// <summary>
    /// PostionUpdate.xaml 的交互逻辑
    /// </summary>
    public partial class PostionUpdate : UserControl
    {
        private GoodsManager manager = new GoodsManager();
        private ObservableCollection<WareHouseInModel> list;
        private bool AddPer = true;
        public PostionUpdate()
        {
            InitializeComponent();
            list = new ObservableCollection<WareHouseInModel>();
            BatchGrid.ItemsSource = list;
            //绩效初始化
            new PerformancePanel().InitPerformance(Performance.StockIn);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string Text = tb_GoodsNo.Text.Trim();
            if (Text.Length <= 8)
            {
                CCMessageBox.Show("请填写正确的值");
                return;
            }
            bool b = true;
            for (int i = 0; i < list.Count; i++)
            {
                if ((list[i].GoodsNo + list[i].SpecCode).Equals(Text, StringComparison.CurrentCultureIgnoreCase))
                {
                    b = false;
                    break;
                }
            }

            if (b == true)
            {
                GoodsModel ms = manager.GetGoodsAll(Text);
                if (ms != null)
                {
                    WareHouseInModel whi = new WareHouseInModel()
                    {
                        GoodsNo = ms.GoodsNo,
                        SpecCode = ms.SpecCode,
                        P_Position = ms.P_postion,
                        F_Position = ms.F_postion,
                        SpecName = ms.SpecName,
                    };
                    list.Add(whi);
                }
            }
            tb_GoodsNo.Clear();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(tb_Postion.Text))
            {
                for (int i = 0; i < BatchGrid.Items.Count; i++)
                {
                    TextBox tb = ControlHelper.FindGridTemplateControl(BatchGrid, 3, i, "tb_postion") as TextBox;
                    tb.Text = tb_Postion.Text.Trim();
                }
                tb_Postion.Clear();
            }

        }
        private void tb_KeyDown(object sender, KeyEventArgs e)
        {

        }


        private void Button_Clear(object sender, RoutedEventArgs e)
        {
            list.Clear();
            BatchGrid.ItemsSource = null;
            BatchGrid.ItemsSource = list;
            AddPer = true;
        }

        private void tb_Billd_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Button_Click(sender, null);
            }
        }

        private void tb_Pos_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Button_Click_1(sender, null);
            }
        }

        private void Button_Submit(object sender, RoutedEventArgs e)
        {
            List<GoodsLogModel> LogList = new List<GoodsLogModel>();
            List<PostSkuModel> list = new List<PostSkuModel>();
            //遍历GRID
            for (int i = 0; i < BatchGrid.Items.Count; i++)
            {
                string goods = (ControlHelper.FindGridControl(BatchGrid, 0, i) as TextBlock).Text;
                string code = (ControlHelper.FindGridControl(BatchGrid, 1, i) as TextBlock).Text;
                string specname = (ControlHelper.FindGridControl(BatchGrid, 2, i) as TextBlock).Text;
                string P_Position = (ControlHelper.FindGridTemplateControl(BatchGrid, 3, i, "tb_postion") as TextBox).Text;
                string F_Position = (ControlHelper.FindGridTemplateControl(BatchGrid, 4, i, "tb_postion") as TextBox).Text;
                PostSkuModel model = new PostSkuModel();
                model.GoodsNo = goods + code;
                model.P_postion = P_Position;
                model.F_postion = F_Position;
                model.UpdatePrimaryOnly = false;
                list.Add(model);
                string OldPosition = this.list.Where(x => (x.GoodsNo + x.SpecCode).Equals((goods + code), StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().P_Position;
                string NewPosition = P_Position;
                GoodsLogModel sim = new GoodsLogModel()
                {
                    GoodsNo = goods,
                    GoodsDetail = goods + code,
                    GoodsCount = 1,
                    LogTime = DateTime.Now,
                    LogType = Convert.ToInt32(GoodsLogType.UpdatePostion),
                    DoEvent = string.Format("货位更新:从【{0}】更新至【{1}】", OldPosition, NewPosition),
                    SpecName = specname,
                    TradeId = string.Empty,
                    UserName = CommonLogin.CommonUser.UserName
                };
                LogList.Add(sim);
            }
            List<string> SqlList = new List<string>();
            if (AddPer == true)
            {
                SqlList.Add(string.Format(@"update T_Performance set finishcount = finishcount+ {2} where CONVERT(varchar(100), dates, 111) = CONVERT(varchar(100), GETDATE(), 111)
                and userId = {0} and Performance = {1}", CommonLogin.CommonUser.Id, Convert.ToInt32(Performance.StockIn), BatchGrid.Items.Count));
            }
            IntercepGoodsLogModel LogModel = new IntercepGoodsLogModel() { LogList = LogList, SqlList = SqlList };

            #region wl 2017-05-26 替换下列方法
            //bool res = UnityAopFactory.GoodsServices.SubmitTrade(list, LogModel);
            #endregion

            bool res = WdgjSource.SavePositionTran(list);

            if (res)
            {
                try
                {
                    BaseDAL dal = new BaseDAL();
                    dal.AddList<GoodsLogModel>(LogList);
                    dal.ExecuteTransaction(SqlList, null);
                    AddPer = false;
                    CCMessageBox.Show("修改成功");
                }
                catch
                {
                    CCMessageBox.Show("操作日志写入遇到异常！");
                }
     
            }
            else
            {
                CCMessageBox.Show("修改失败");
            }
        }

        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            Button_Clear(sender, null);
            tb_GoodsNo.Focus();
        }
    }
}
