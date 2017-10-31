using _9M.Work.DbObject;
using _9M.Work.ErpApi;
using _9M.Work.Model;
using _9M.Work.Utility;
using _9M.Work.WPF_Common;
using _9M.Work.WPF_Common.WpfBind;
using _9M.Work.WPF_Main.ControlTemplate;
using _9M.Work.WPF_Main.FrameWork;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Media;
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
using _9M.Work.Model.Log;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;
using _9M.Work.AOP;
using _9M.Work.AOP.Goods;
using _9M.Work.WPF_Main.Infrastrcture;

namespace _9M.Work.WPF_Main.Views.WareHouse
{
    /// <summary>
    /// GoodsInStock.xaml 的交互逻辑
    /// </summary>
    public partial class GoodsInStock : UserControl, INotifyPropertyChanged
    {
        GoodsManager manager = new GoodsManager();
        private List<WareHouseInModel> list;
        private SoundPlayer sp = new SoundPlayer();
        private BaseDAL dal = new BaseDAL();
        public GoodsInStock()
        {
            InitializeComponent();
            this.DataContext = this;
            radio_one.Checked += radio_one_Checked;
            radio_many.Checked += radio_one_Checked;
            //让文本框得到焦点
            this.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                 (Action)(() => { Keyboard.Focus(tb_Billd); }));
            Keyboard.Focus(tb_Billd);
            //绩效初始化
            new PerformancePanel().InitPerformance(Performance.StockIn);
            //时间
            DateTime now = DateTime.Now;
            date_start.Text = DateTime.Now.AddMonths(-1).ToShortDateString();
            date_end.Text = DateTime.Now.ToShortDateString();
            tb_user.Text = CommonLogin.CommonUser.UserName;
        }

        public ICommand QueryCommand
        {
            get { return new DelegateCommand(QueryStockInBiLL); }
        }

        public ICommand QueryCommandCode
        {
            get { return new DelegateCommand(QueryStockInGoodsNo); }
        }

        public bool IsOneRow
        {
            get { return radio_one.IsChecked == true; }
        }
        private void QueryStockInGoodsNo()
        {
            if (list != null)
            {

                string Loation = string.Empty;
                string GoodsDetail = tb_GoodsNoAll.Text.Trim();

                string sql = string.Format(@"

                                delete T_AndroidLock where DATEDIFF(mi,optime,getdate())>30

                                if not exists(select * from T_AndroidLock where sku='{0}') 
                                           begin 
                                           insert into T_AndroidLock(sku,operator,optime) values('{0}','{1}',getdate())
                                           end  
                                select id,operator from T_AndroidLock where sku='{0}' and operator<>'{1}'", GoodsDetail, CommonLogin.CommonUser.UserName);

                DataTable dt = dal.QueryDataTable(sql, new object[] { });
                if (dt.Rows.Count > 0)
                {
                    CCMessageBox.Show(string.Format("该商品已锁定，员工 {0} 正在操作", dt.Rows[0]["operator"].ToString().Trim()));
                    return;
                }

                int Index = list.FindIndex(x => (x.GoodsNo + x.SpecCode).Equals(GoodsDetail, StringComparison.CurrentCultureIgnoreCase));
                if (Index > -1)
                {
                    Loation = AppDomain.CurrentDomain.BaseDirectory + @"Sounds\Success.WAV";
                    BatchGrid.SelectedItem = list[Index];
                    DataGridRow dr = ControlHelper.GetRow(BatchGrid, Index);
                    dr.Background = new SolidColorBrush(Colors.Blue);
                    TextBox tb = ControlHelper.FindGridTemplateControl(BatchGrid, 4, Index, "tb_postion") as TextBox;
                    if (IsOneRow)
                    {
                        tb.Focus();
                        tb.SelectAll();
                    }
                    else
                    {
                        tb_GoodsNoAll.Clear();
                        tb_GoodsNoAll.Focus();
                    }
                }
                else //没有获取到货品
                {
                    Loation = AppDomain.CurrentDomain.BaseDirectory + @"Sounds\Error.WAV";
                    tb_GoodsNoAll.Clear();
                    tb_GoodsNoAll.Focus();
                }
                if (!System.IO.File.Exists(Loation))
                {
                    CCMessageBox.Show("没有音频文件");
                    return;
                }
                sp.SoundLocation = Loation;
                sp.Play();
            }
        }

        public void BindTradeMsg(WareHouseInModel w)
        {
            lab_count.Content = list.Sum(x => x.GoodsCount);
            lab_time.Content = w.CHKTime.ToString();
            lab_man.Content = w.RegOperator;
            lab_trade.Content = w.BillID;
        }

        private void QueryStockInBiLL()
        {
            string tradeid = tb_Billd.Text;
            if (!string.IsNullOrEmpty(tradeid))
            {
                list = manager.WareHouseInList(tradeid).ToList();
                if (list.Count > 0)
                {
                    BatchGrid.ItemsSource = list;
                    BindTradeMsg(list[0]);
                    tb_GoodsNoAll.Focus();
                }
            }
        }

        #region 焦点控制
        private void tb_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                DataGridRow dr = ControlHelper.GetRow(BatchGrid, BatchGrid.SelectedIndex);
                dr.Background = new SolidColorBrush(Colors.Red);
                tb_GoodsNoAll.Clear();
                tb_GoodsNoAll.Focus();

                if (ckRealtime.IsChecked == true)
                {
                    //修改货架位
                    string goods = (ControlHelper.FindGridControl(BatchGrid, 0, BatchGrid.SelectedIndex) as TextBlock).Text;
                    string code = (ControlHelper.FindGridControl(BatchGrid, 1, BatchGrid.SelectedIndex) as TextBlock).Text;
                    string specname = (ControlHelper.FindGridControl(BatchGrid, 2, BatchGrid.SelectedIndex) as TextBlock).Text;
                    string goodscount = (ControlHelper.FindGridControl(BatchGrid, 3, BatchGrid.SelectedIndex) as TextBlock).Text;
                    string P_Position = (ControlHelper.FindGridTemplateControl(BatchGrid, 4, BatchGrid.SelectedIndex, "tb_postion") as TextBox).Text;
                    string F_Position = (ControlHelper.FindGridTemplateControl(BatchGrid, 5, BatchGrid.SelectedIndex, "tb_postion") as TextBox).Text;

                    if (P_Position == "")
                    {
                        CCMessageBox.Show("新货位为空！");
                        return;
                    }

                    PostSkuModel model = new PostSkuModel();
                    model.GoodsNo = goods + code;
                    model.P_postion = P_Position;
                    model.F_postion = F_Position;
                    model.UpdatePrimaryOnly = false;
                    List<PostSkuModel> list = new List<PostSkuModel>();
                    list.Add(model);

                    // Call method
                    GoodsManager manager = new GoodsManager();
                    bool res = manager.SubmitTrade(list);
                    if (!res)
                    {
                        CCMessageBox.Show("实时保存失败！");
                    }
                    else
                    {
                        string OldPosition = this.list.Where(x => (x.GoodsNo + x.SpecCode).Equals((goods + code), StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().P_Position;
                        string NewPosition = P_Position;

                        //插入上架日志
                        GoodsLogModel log = new GoodsLogModel()
                        {
                            GoodsNo = goods,
                            GoodsDetail = goods + code,
                            GoodsCount = Convert.ToInt32(goodscount),
                            LogTime = DateTime.Now,
                            LogType = Convert.ToInt32(GoodsLogType.StockIn),
                            DoEvent = string.Format("货位更新:从【{0}】更新至【{1}】", OldPosition, NewPosition),
                            SpecName = specname,
                            TradeId = lab_trade.Content.ToString(),
                            UserName = CommonLogin.CommonUser.UserName
                        };
                        try
                        {
                            dal.Add<GoodsLogModel>(log);
                        }
                        catch (Exception err)
                        {
                            CCMessageBox.Show("实时保存失败！" + err.Message.ToString());
                        }
                    }
                }

            }
        }


        private void tb_Billd_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string Test = tb_Billd.Text.Trim();
                if (Test.Length > 8)
                {
                    if (Test.Substring(0, 2).Equals("RK"))//输入的是订单
                    {
                        QueryStockInBiLL();
                    }
                }
                else
                {
                    CCMessageBox.Show("无效值");
                }


            }
        }



        private void tb_GoodsNoAll_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                QueryStockInGoodsNo();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void radio_one_Checked(object sender, RoutedEventArgs e)
        {
            if (Convert.ToBoolean(radio_one.IsChecked))
            {
                wrap_many.Visibility = System.Windows.Visibility.Hidden;
            }
            else
            {
                wrap_many.Visibility = System.Windows.Visibility.Visible;
            }
        }


        private void tb_Postion_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Button_Click_Many(new object(), new RoutedEventArgs());
            }
        }
        #endregion
        private void Button_Click_Many(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(tb_Postion.Text))
            {
                //遍历GRID
                for (int i = 0; i < BatchGrid.Items.Count; i++)
                {
                    DataGridRow dr = ControlHelper.GetRow(BatchGrid, i);
                    if (dr.Background.ToString().Equals("#FF0000FF"))
                    {
                        TextBox tb = ControlHelper.FindGridTemplateControl(BatchGrid, 4, i, "tb_postion") as TextBox;
                        tb.Text = tb_Postion.Text;
                        dr.Background = new SolidColorBrush(Colors.Red);
                    }
                }
                tb_Postion.Clear();
                tb_GoodsNoAll.Focus();
            }
            else
            {
                CCMessageBox.Show("请输入货位");
            }

        }


        public void Button_KeepTrade(object sender, RoutedEventArgs e)
        {
            if (BatchGrid.ItemsSource == null)
            {
                return;
            }
            if (CCMessageBox.Show("是否保存入库单", "提示", CCMessageBoxButton.YesNo, CCMessageBoxImage.Question) == CCMessageBoxResult.Yes)
            {
                List<GoodsLogModel> LogList = new List<GoodsLogModel>();
                List<PostSkuModel> list = new List<PostSkuModel>();
                //遍历GRID
                for (int i = 0; i < BatchGrid.Items.Count; i++)
                {
                    string goods = (ControlHelper.FindGridControl(BatchGrid, 0, i) as TextBlock).Text;
                    string code = (ControlHelper.FindGridControl(BatchGrid, 1, i) as TextBlock).Text;
                    string specname = (ControlHelper.FindGridControl(BatchGrid, 2, i) as TextBlock).Text;
                    string goodscount = (ControlHelper.FindGridControl(BatchGrid, 3, i) as TextBlock).Text;
                    string P_Position = (ControlHelper.FindGridTemplateControl(BatchGrid, 4, i, "tb_postion") as TextBox).Text;
                    string F_Position = (ControlHelper.FindGridTemplateControl(BatchGrid, 5, i, "tb_postion") as TextBox).Text;
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
                        GoodsCount = Convert.ToInt32(goodscount),
                        LogTime = DateTime.Now,
                        LogType = Convert.ToInt32(GoodsLogType.StockIn),
                        DoEvent = string.Format("货位更新:从【{0}】更新至【{1}】", OldPosition, NewPosition),
                        SpecName = specname,
                        TradeId = lab_trade.Content.ToString(),
                        UserName = CommonLogin.CommonUser.UserName
                    };
                    LogList.Add(sim);
                }
                //bool res = manager.SubmitTrade(list);


                List<string> SqlList = new List<string>();
                string addsql = string.Format(@"  if not exists(select * from T_StockInPerCaChe where BillDate = '{3}' and BillID = '{4}')
                  update T_Performance set finishcount = finishcount+ {2} where CONVERT(varchar(100), dates, 111) = CONVERT(varchar(100), GETDATE(), 111)
                        and userId = {0} and Performance = {1}", CommonLogin.CommonUser.Id, Convert.ToInt32(Performance.StockIn), lab_count.Content, DateTime.Now.ToShortDateString(), lab_trade.Content);
                SqlList.Add(addsql);
                //绩效添加(一个订单一天只能保存一次。先保存记录在验证)
                string sql = string.Format(@"if not exists
                    (select * from T_StockInPerCaChe where  BillDate = '{1}' and BillID = '{0}')
                     insert into T_StockInPerCaChe values ('{1}','{0}')", lab_trade.Content, DateTime.Now.ToShortDateString());
                SqlList.Add(sql);





                //AOP写日志和绩效
                //IUnityContainer container = new UnityContainer();
                //container.AddNewExtension<Interception>();
                //container.RegisterType<GoodsInterface, GoodsImplements>
                //(new Interceptor<TransparentProxyInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
                // Resolve
                // GoodsInterface calc = container.Resolve<GoodsInterface>();
                IntercepGoodsLogModel LogModel = new IntercepGoodsLogModel() { LogList = LogList, SqlList = SqlList };
                // Call method

                #region wl 2017-05-26 替换下列方法
                //bool res = UnityAopFactory.GoodsServices.SubmitTrade(list, LogModel);
                #endregion

                bool res = WdgjSource.SavePositionTran(list);

                if (res)
                {
                    for (int i = 0; i < BatchGrid.Items.Count; i++)
                    {
                        DataGridRow dr = ControlHelper.GetRow(BatchGrid, i);
                        dr.Background = new SolidColorBrush(Colors.Green);
                    }

                    CCMessageBox.Show("保存成功");
                }
                else
                {
                    CCMessageBox.Show("保存出现错误");
                }
            }
        }

        private void Button_KeepTrade_Cache(object sender, RoutedEventArgs e)
        {
            if (lab_trade.Content != null)
            {
                if (CCMessageBox.Show("是否先保存记录下次在提交", "提示", CCMessageBoxButton.YesNo, CCMessageBoxImage.Question) == CCMessageBoxResult.Yes)
                {
                    string TradeNo = lab_trade.Content.ToString();
                    string DateNow = DateTime.Now.ToShortDateString();
                    var count = dal.ExecuteSql(string.Format(@"delete from T_StockInCaChe where BillID='{0}' and BillDate = '{1}'", TradeNo, DateNow), new object[] { });
                    DataTable dt = new DataTable();
                    dt.TableName = "T_StockInCaChe";
                    dt.Columns.Add("Id", typeof(int));
                    dt.Columns.Add("BillID", typeof(string));
                    dt.Columns.Add("BillDate", typeof(DateTime));
                    dt.Columns.Add("GoodsNo", typeof(string));
                    dt.Columns.Add("SpecCode", typeof(string));
                    dt.Columns.Add("SpecName", typeof(string));
                    dt.Columns.Add("GoodsCount", typeof(int));
                    dt.Columns.Add("P_Position", typeof(string));
                    dt.Columns.Add("F_Position", typeof(string));
                    dt.Columns.Add("GoodsStatus", typeof(int));
                    //遍历GRID
                    for (int i = 0; i < BatchGrid.Items.Count; i++)
                    {
                        DataRow drs = dt.NewRow();
                        drs["Id"] = DBNull.Value;
                        drs["BillID"] = lab_trade.Content.ToString();
                        drs["BillDate"] = DateNow;
                        drs["GoodsNo"] = (ControlHelper.FindGridControl(BatchGrid, 0, i) as TextBlock).Text;
                        drs["SpecCode"] = (ControlHelper.FindGridControl(BatchGrid, 1, i) as TextBlock).Text;
                        drs["SpecName"] = (ControlHelper.FindGridControl(BatchGrid, 2, i) as TextBlock).Text;
                        drs["GoodsCount"] = (ControlHelper.FindGridControl(BatchGrid, 3, i) as TextBlock).Text;
                        drs["P_Position"] = (ControlHelper.FindGridTemplateControl(BatchGrid, 4, i, "tb_postion") as TextBox).Text;
                        drs["F_Position"] = (ControlHelper.FindGridTemplateControl(BatchGrid, 5, i, "tb_postion") as TextBox).Text;
                        DataGridRow dr = ControlHelper.GetRow(BatchGrid, i);
                        int GoodsStatus = 0;
                        if (dr.Background.ToString().Equals(Colors.Red.ToString()))
                        {
                            GoodsStatus = 1;
                        }
                        drs["GoodsStatus"] = GoodsStatus;
                        dt.Rows.Add(drs);
                    }
                    bool bs = dal.BulkInsertAll(dt);
                    string Msg = bs ? "保存成功" : "保存失败";
                    CCMessageBox.Show(Msg);
                }
            }
        }


        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {

            var list = dal.QueryList<StockInCaCheModel>(string.Format("select BillID from T_StockInCaChe where BillDate = '{0}' Group by BillID ", (sender as DatePicker).SelectedDate), new object[] { });
            ComboBoxBind.BindComboBox(com_BillId, list, "BillID", "Id");
        }

        private void com_BillId_DropDownClosed(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(com_BillId.Text))
            {
                var slist = dal.QueryList<StockInCaCheModel>(string.Format("select * from T_StockInCaChe where BillDate = '{0}' and BillID='{1}' ", date_cache.SelectedDate, com_BillId.Text), new object[] { });
                list = new List<WareHouseInModel>();
                slist.ForEach(x =>
                {
                    WareHouseInModel hi = new WareHouseInModel();
                    hi.BillID = x.BillID;
                    hi.F_Position = x.F_Position;
                    hi.GoodsCount = x.GoodsCount;
                    hi.GoodsNo = x.GoodsNo;
                    hi.P_Position = x.P_Position;
                    hi.SpecCode = x.SpecCode;
                    hi.SpecName = x.SpecName;
                    list.Add(hi);
                });
                var ms = manager.WareHouseInList(com_BillId.Text).ToList();
                if (ms.Count > 0)
                {
                    BindTradeMsg(ms[0]);
                }
                BatchGrid.ItemsSource = list;
                //遍历GRID
                for (int i = 0; i < slist.Count; i++)
                {
                    DataGridRow dr = ControlHelper.GetRow(BatchGrid, i);
                    string GoodsNo = slist[i].GoodsNo;
                    string SpecCode = slist[i].SpecCode;
                    StockInCaCheModel sc = slist.Where(x => x.GoodsNo == GoodsNo && x.SpecCode == SpecCode).Single();
                    if (sc.GoodsStatus == true)
                    {
                        dr.Background = new SolidColorBrush(Colors.Red);
                    }
                }
                tb_GoodsNoAll.Focus();
            }
        }

        //绩效查询
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            StatisticsModel st = new PerformancePanel().StatisticsByTime(tb_user.Text, Performance.StockIn, Convert.ToDateTime(date_start.SelectedDate), Convert.ToDateTime(date_end.SelectedDate));
            tb_percount.Text = st.Count.ToString();
        }

        //查看日志
        private void Button_OpenLog(object sender, RoutedEventArgs e)
        {
            FormInit.OpenDialog(this, new LogDialog(), false);
        }

        //保存绩效
        private void Button_SaveJX(object sender, RoutedEventArgs e)
        {

            if (lab_count.Content != null)
            {

                if (Convert.ToInt32(lab_count.Content) > 0)
                {
                    //                    string addsql = string.Format(@"
                    //                                                   
                    //                                                 if exists(select * from T_Performance where Performance={1} and userId={0} and Convert(varchar(10),dates,121)='{3}')
                    //                                                 update T_Performance set finishcount=finishcount+{2} where Performance={1} and userId='{0}' and Convert(varchar(10),dates,121)='{3}'
                    //                                                 else
                    //                                                 insert into T_Performance(userId,dates,finishcount,Performance) values({0},'{3}',{2},{1})",
                    //                                                 CommonLogin.CommonUser.Id, Convert.ToInt32(Performance.StockIn), lab_count.Content, DateTime.Now.ToString("yyyy-MM-dd"));

                    //                    if (dal.ExecuteSql(addsql) > 0)
                    //                    {
                    //                        CCMessageBox.Show("绩效保存成功!");
                    //                    }
                    //                    else
                    //                    {
                    //                        CCMessageBox.Show("绩效保存失败!");
                    //                    }

                    List<string> SqlList = new List<string>();
                    string addsql = string.Format(@"  if not exists(select * from T_StockInPerCaChe where BillDate = '{3}' and BillID = '{4}')
                      begin
                                 if exists(select * from T_Performance where Performance={1} and userId={0} and Convert(varchar(10),dates,121)='{3}')
                                       begin
                                           update T_Performance set finishcount = finishcount+ {2} where CONVERT(varchar(100), dates, 111) = CONVERT(varchar(100), GETDATE(), 111) and userId = {0} and Performance = {1}
                                       end
                                 else
                                       begin
                                           insert into T_Performance(userId,dates,finishcount,Performance) values({0},'{3}',{2},{1}) 
                                       end
                      end", CommonLogin.CommonUser.Id, Convert.ToInt32(Performance.StockIn), lab_count.Content, DateTime.Now.ToShortDateString(), lab_trade.Content);

                    SqlList.Add(addsql);


                    //绩效添加(一个订单一天只能保存一次。先保存记录在验证)
                    string sql = string.Format(@"if not exists
                    (select * from T_StockInPerCaChe where  BillDate = '{1}' and BillID = '{0}')
                     insert into T_StockInPerCaChe values ('{1}','{0}')", lab_trade.Content, DateTime.Now.ToShortDateString());

                    SqlList.Add(sql);

                    if (dal.ExecuteTransaction(SqlList, null) == true)
                    {
                        CCMessageBox.Show("绩效保存成功!");
                    }
                    else
                    {
                        CCMessageBox.Show("绩效保存失败!");
                    }
                }
            }
            else
            {
                CCMessageBox.Show("没有要保存的绩效!");
            }
        }
    }
}
