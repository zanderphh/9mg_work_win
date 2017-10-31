using _9M.Work.DbObject;
using _9M.Work.Model;
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
using _9M.Work.Utility;
using System.Windows.Threading;
using _9M.Work.WPF_Common;
using _9M.Work.WPF_Common.WpfBind;
using _9M.Work.WPF_Common.Controls;
using _9M.Work.WPF_Main.ControlTemplate.PrintTemplate;
using _9M.Work.WPF_Main.Infrastrcture;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;
using _9M.Work.AOP.Goods;
using _9M.Work.AOP;
using _9M.Work.Model.Log;
using _9M.Work.WPF_Main.Views.Print;
using _9M.Work.ErpApi;
using _9M.Work.WPF_Main.FrameWork;

namespace _9M.Work.WPF_Main.Views.QualityCheck
{
    /// <summary>
    /// WarePacking.xaml 的交互逻辑
    /// </summary>
    public partial class WarePacking : UserControl
    {

        //打印按钮间隔
        private int WaitSecondCount = 0;
        private int WaitSecond = 0;
        private DispatcherTimer timer;
        private BaseDAL dal = new BaseDAL();
        int UserId = CommonLogin.CommonUser.Id;
        string UserAlias = CommonLogin.CommonUser.Alias;
        private GoodsManager manager = new GoodsManager();
        //绩效的模式（包装）
        private int Performanceid = Convert.ToInt32(Performance.Packing);
        LabelTemplate print = new LabelTemplate();
        private List<BrandModel> brandlist = null;
        PrintHelper ph = new PrintHelper();

        //批量打印 wl
        PrintLabeModel batchPrintLableModel = new PrintLabeModel();
        IntercepGoodsLogModel batchLogModel = new IntercepGoodsLogModel();
        string batchSize = string.Empty;
        string batchTitlePath = string.Empty;

        public WarePacking()
        {
            InitializeComponent();
            Init();
            timer = new DispatcherTimer();
            timer.Tick += timer_Tick;
            brandlist = dal.GetAll<BrandModel>();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            //时间间隔从第二次开始（马上响应记时）
            if (WaitSecond == WaitSecondCount - 1)
            {
                timer.Interval = new TimeSpan(0, 0, 0, 0, 1000);
            }
            if (WaitSecond == 0)
            {
                //还原所有值
                timer.Interval = new TimeSpan(0, 0, 0, 0, 0);
                Radio_Panel.IsEnabled = true;
                Radio_Panel.Background = null;
                RadioBind.ClearSelect(Radio_Panel);
                lab_Timing.Content = "";
                //btn_Print.IsEnabled = true;
                //Label lb = new Label() { Content = "打印", FontSize = 15, Foreground = new SolidColorBrush(Colors.White), FontWeight = FontWeights.Bold };
                //btn_Print.Content = lb;
                //btn_Print.Background = new SolidColorBrush(Colors.Green);
                timer.Stop();
                WaitSecond = WaitSecondCount;

            }
            else
            {
                lab_Timing.Content = WaitSecond;
                //  btn_Print.Content = WaitSecond;
                WaitSecond--;
            }
        }

        public void Init()
        {
            //读取冷取时间
            //WaitSecondCount = dal.QuerySingle<SetValueModel>("select * from T_SetValues", new object[] { }).Value + 1;

            // bl_User.Text = CommonLogin.CommonUser.UserName;
            //如果没有绩效信息。那就加入一条
            string checkSql = string.Format(@"if not exists
  (select * from T_Performance
  where  CONVERT(varchar(100), dates, 111) = CONVERT(varchar(100), GETDATE(), 111)
  and userId = {0} and Performance = {1})
  insert into T_Performance values ({2},CONVERT(varchar(100), GETDATE(), 111)  ,0,{3})", UserId, Performanceid, UserId, Performanceid);
            List<string> sqllist = new List<string>();
            sqllist.Add(checkSql);
            bool bs = dal.ExecuteTransaction(sqllist, null);
            if (!bs)
            {
                CCMessageBox.Show("绩效初始化失败,请及时与管理员联系");
                return;
            }
            //绑定绩效
            PerPanel.BindPerformance(CommonLogin.CommonUser.Id);
            //让文本框得到焦点
            this.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                 (Action)(() => { Keyboard.Focus(tb_WareNo); }));
            Keyboard.Focus(tb_WareNo);
        }
        public void BindSpecImagePanel(string WareNo)
        {
            string Sql = string.Format(@"select WareNo,Color,ImageKey from dbo.T_WareSpecList where WareNo = '{0}'
            group by WareNo,Color,ImageKey  order by MAX(id) ", WareNo);
            List<WareSpecModel> speclist = dal.QueryList<WareSpecModel>(Sql, new object[] { });

            DataTable dt = speclist.ConvertToDataTable<WareSpecModel>();
            ImageBox.BindImage(dt, WareNo, "ImageKey", "Color", 1.33);
            string sql = string.Format("select Color from dbo.T_WareSpecList where WareNo= '{0}' group by Color having Color  like '%有%'", WareNo);
            int count = dal.QueryList<string>(sql, new object[] { }).Count;
            Btn_PeiShi.Content = count == 0 ? "添加配饰" : "刷新配饰";
            Stack_PeiShi.Visibility = count == 0 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
        }

        #region Event
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            lab_fyi.Content = null;
            string WareNo = tb_WareNo.Text.Trim();
            if (string.IsNullOrEmpty(WareNo))
            {
                CCMessageBox.Show("款号不能为空");
                return;
            }
            lab_WareNo.Content = tb_WareNo.Text.ToUpper();
            BindSpecImagePanel(WareNo);
            string sql = string.Format(@"select * from dbo.T_WareList where WareNo = '{0}'", WareNo);
            WareModel model = dal.QuerySingle<WareModel>(sql, new object[] { });
            if (model != null)
            {
                lab_fyi.Content = model.OriginalFyiCode;
                //动态生成尺码
                Radio_Panel.Children.Clear();
                string BrandEn = GoodsHelper.BrandEn(WareNo);
                BrandModel ms = brandlist.Where(x => x.BrandEN.Equals(BrandEn, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                if (ms != null)
                {
                    List<string> SizeList = ms.Sizes.Split(',').ToList();
                    for (int i = 0; i < SizeList.Count; i++)
                    {
                        RadioButton radio = new RadioButton();

                        radio.Style = this.FindResource("BoxRadioButton") as Style;
                        int Left = (i == 0 || i == 7) ? 0 : 26;
                        radio.Margin = new Thickness(Left, 2, 0, 0);
                        radio.Checked += RadioButton_Checked;
                        radio.Content = SizeList[i];
                        Color color = Colors.Yellow;

                        if (i == 2)
                        {
                            color = Colors.Red;
                        }
                        else if (i == 3)
                        {
                            color = Colors.Green;
                        }
                        else if (i == 4)
                        {
                            color = Colors.Blue;
                        }
                        else if (i > 4)
                        {
                            color = Colors.Black;
                        }
                        radio.Foreground = new SolidColorBrush(color);
                        if (SizeList[i].Equals("XXL"))
                        {
                            radio.Width = 190;
                        }
                        else if (SizeList[i].Equals("XXXL"))
                        {
                            radio.Width = 240;
                        }
                        Radio_Panel.Children.Add(radio);
                    }
                }
            }



        }




        //回车事件
        private void tb_WareNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.Button_Click(new object(), new RoutedEventArgs());
            }
        }
        #endregion

        public WareSpecModel CancelModel = new WareSpecModel();
        private void Btn_CommandClick(object sender, RoutedEventArgs e)
        {
            int Tag = Convert.ToInt32((sender as Button).Tag);
            string Size = RadioBind.ReadSelectedContent(Radio_Panel);

            int count = Convert.ToInt32(tb_count.Text);
            switch (Tag)
            {

                case 1: //撤销上一次
                    if (!string.IsNullOrEmpty(CancelModel.WareNo))
                    {
                        string sql = string.Format(" select top 1 * from  T_WareSpecList where WareNo = '{0}' and Color = '{1}' and Size = '{2}'", lab_WareNo.Content.ToString(), CancelModel.Color, CancelModel.Size);
                        WareSpecModel spec = dal.QuerySingle<WareSpecModel>(sql, new object[] { });
                        List<string> sqllist = new List<string>();
                        sqllist.Add(string.Format(" update  T_WareSpecList set activity = 0 ,stock = stock-{3} where WareNo = '{0}' and Color = '{1}' and Size = '{2}'", lab_WareNo.Content.ToString(), CancelModel.Color, CancelModel.Size, tb_count.Text));
                        if (spec.Activity == true)  //正常撤消要减去绩效
                        {
                            sqllist.Add(string.Format(@"update T_Performance set finishcount = finishcount-{2} where CONVERT(varchar(100), dates, 111) = CONVERT(varchar(100), GETDATE(), 111)
                            and userId = {0} and Performance = {1}", UserId, Performanceid, tb_count.Text));
                        }
                        bool b = dal.ExecuteTransaction(sqllist, null);
                        if (b) //撤销成功（刷新绩效）
                        {
                            //打开打印按钮
                            timer.Interval = new TimeSpan(0, 0, 0, 0, 0);
                            //btn_Print.IsEnabled = true;
                            //Label lb = new Label() { Content = "打印", FontSize = 15, Foreground = new SolidColorBrush(Colors.White), FontWeight = FontWeights.Bold };
                            //btn_Print.Content = lb;
                            //btn_Print.Background = new SolidColorBrush(Colors.Green);
                            Radio_Panel.IsEnabled = true;
                            Radio_Panel.Background = null;
                            RadioBind.ClearSelect(Radio_Panel);
                            lab_Timing.Content = "";
                            timer.Stop();
                            WaitSecond = WaitSecondCount;
                            //绑定绩效
                            PerPanel.BindPerformance(CommonLogin.CommonUser.Id);
                        }
                        else
                        {
                            CCMessageBox.Show("撤销失败");
                        }
                    }
                    break;
                case 2:  //打印
                    #region  这里打印标签
                    if (string.IsNullOrEmpty(Size))
                    {
                        CCMessageBox.Show("请选中尺码");
                        return;
                    }
                    ImageBindModel imagemodel = ImageBox.ReadSelectedImage();
                    if (imagemodel == null)
                    {
                        CCMessageBox.Show("请选中颜色");

                        return;
                    }

                    List<PrintLabeModel> modelList = SqlDalHelp.getWareFlagCode(UserAlias, lab_WareNo.Content.ToString(), imagemodel.TitlePath, Size);
                    if (modelList.Count > 0)
                    {

                        bool DoublePrint = Convert.ToBoolean(Check_Double.IsChecked);
                        PrintLabeModel model = modelList[0];
                        if (tb_printck.IsChecked == true)
                        {
                            //   GoodsModel ms = manager.GetGoodsAll(model.WareNo + model.SpecCode);
                            // ps.PrintRefundLabel(ms);
                        }
                        else
                        {

                        }
                        ph.PrintWareLabel(modelList);
                        List<string> submitlist = new List<string>();
                        //改变规格状态和插入绩效
                        submitlist.Add(string.Format(@"update  T_WareSpecList set activity = 1,stock = stock+1 where WareNo = '{0}' and Color = '{1}' and Size = '{2}'", lab_WareNo.Content.ToString(), imagemodel.TitlePath, Size));
                        submitlist.Add(string.Format(@"update T_Performance set finishcount = finishcount+ {2} where CONVERT(varchar(100), dates, 111) = CONVERT(varchar(100), GETDATE(), 111)
                        and userId = {0} and Performance = {1}", UserId, Performanceid, count));


                        bool bs = dal.ExecuteTransaction(submitlist, null);
                        if (bs) //打印成功（刷新绩效）
                        {
                            PerPanel.BindPerformance(CommonLogin.CommonUser.Id);
                            CancelModel.WareNo = lab_WareNo.Content.ToString();
                            CancelModel.Size = Size;
                            CancelModel.Color = imagemodel.TitlePath;
                        }
                        else
                        {
                            CCMessageBox.Show("打印失败");
                        }
                        //锁定按钮
                        WaitSecond = WaitSecondCount;
                        timer.Start();
                        btn_Print.IsEnabled = false;
                        btn_Print.Background = new SolidColorBrush(Colors.Gray);
                    }
                    else
                    {
                        CCMessageBox.Show("失败,没有获取到数据");
                    }

                    #endregion
                    break;
                case 3:
                    if (lab_WareNo.Content != null)
                    {
                        if (!string.IsNullOrEmpty(lab_WareNo.Content.ToString()))
                        {
                            FormInit.OpenDialog(this, new RemarkSet(lab_WareNo.Content.ToString()), false);
                        }
                    }
                    break;
                case 4:  //查看日志
                    if (lab_WareNo.Content != null)
                    {
                        if (!string.IsNullOrEmpty(lab_WareNo.Content.ToString()))
                        {
                            FormInit.OpenDialog(this, new LogDialog(lab_WareNo.Content.ToString()), false);
                        }
                    }
                    break;
            }
        }

        private void Btn_AddPeiShi(object sender, RoutedEventArgs e)
        {
            string Peishi = RadioBind.ReadSelectedContent(Stack_PeiShi);
            string WareNo = lab_WareNo.Content == null ? string.Empty : lab_WareNo.Content.ToString();
            if (string.IsNullOrEmpty(WareNo))
            {
                CCMessageBox.Show("请选择款号");
                return;
            }
            List<string> addlist = new List<string>();
            //查询规格
            List<WareSpecModel> splist = dal.QueryList<WareSpecModel>(string.Format("select * from dbo.T_WareSpecList where WareNo= '{0}'", WareNo), new object[] { });
            //分析
            //需要添加的颜色
            List<string> addcolorlist = splist.GroupBy(x => x.Color).Select(x => x.Key).Where(x => !x.Contains("有")).ToList();
            //得到需要添加的配饰
            string PeiShi = string.Empty;
            if (Btn_PeiShi.Content.ToString().Equals("添加配饰"))
            {
                PeiShi = RadioBind.ReadSelectedContent(Stack_PeiShi);
            }
            else
            {
                string Color = splist.Where(x => x.Color.Contains("有")).FirstOrDefault().Color;
                PeiShi = Color.Substring(Color.IndexOf('有'), Color.Length - Color.IndexOf('有'));
            }
            if (string.IsNullOrEmpty(PeiShi))
            {
                CCMessageBox.Show("没有指定的配饰");
                return;
            }
            //Spec
            string Brand = GoodsHelper.BrandEn(WareNo);
            BrandModel bmodel = brandlist.Where(x => x.BrandEN.Equals(Brand, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            if (bmodel == null)
            {
                CCMessageBox.Show("没有品牌尺码信息");
                return;
            }
            List<string> SizeList = bmodel.Sizes.Split(',').Where(x => !string.IsNullOrEmpty(x)).ToList();
            List<string> SizeCodeList = bmodel.SizeCodes.Split(',').Where(x => !string.IsNullOrEmpty(x)).ToList();
            if (SizeList.Count == 0)
            {
                CCMessageBox.Show("请录入品牌尺码的数据");
                return;
            }

            //=============== 2017-09-02 by wl 
            //得到SPEC最大的ID
            //WareSpecModel sp = dal.QuerySingle<WareSpecModel>(string.Format("select top 1 * from T_WareSpecList where WareNo ='{0}' order by SpecCode Desc", WareNo), new object[] { });
            //int ColorBegin = sp == null ? 0 : sp.SpecCode / 10;

            System.Data.DataTable cnt = dal.QueryDataTable(string.Format("select Color from T_WareSpecList where WareNo='{0}' group by Color", WareNo), new object[] { });
            int ColorBegin = cnt == null ? 0 : cnt.Rows.Count;

            //=============== 2017-09-02 by wl

            addcolorlist.ForEach(x =>
            {
                int Count = splist.Where(y => y.Color.Contains(x + "有")).Count();
                if (Count == 0) //如果没有就添加
                {
                    string ImageKey = splist.Where(zs => zs.Color.Equals(x)).FirstOrDefault().ImageKey;
                    ColorBegin++;
                    int SizeBegin = -1;
                    SizeList.ForEach(z =>
                    {
                        SizeBegin++;
                        addlist.Add(string.Format(@"insert into T_WareSpecList values('{0}','{1}','{2}',{3},'{4}',0,0)", WareNo, x + PeiShi, z, ColorBegin.ToString() + SizeCodeList[SizeBegin].ToString(), ImageKey));
                    });
                }
            });
            //日志
            if (addlist.Count > 0)
            {
                GoodsLogModel log = new GoodsLogModel()
                {
                    GoodsNo = WareNo,
                    UserName = CommonLogin.CommonUser.UserName,
                    LogTime = DateTime.Now,
                    LogType = Convert.ToInt32(GoodsLogType.Subsec),
                    DoEvent = string.Format(@"刷新配饰:【{0}】", WareNo),
                };
                dal.Add<GoodsLogModel>(log);
                //string LogSql = string.Format(@"insert into T_QualityCheck_Log values('{0}','{1}','{2}','{3}')", CommonLogin.CommonUser.UserName, WareNo, "刷新配饰", DateTime.Now);
                //dal.ExecuteSql(LogSql);
            }
            //添加数据
            bool adres = dal.ExecuteTransaction(addlist, null);
            BindSpecImagePanel(WareNo);
        }

        private void Btn_Clear(object sender, RoutedEventArgs e)
        {
            tb_WareNo.Text = "";
            //让文本框得到焦点
            this.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                 (Action)(() => { Keyboard.Focus(tb_WareNo); }));
            Keyboard.Focus(tb_WareNo);
            Stack_PeiShi.Visibility = System.Windows.Visibility.Visible;
            lab_WareNo.Content = "";
            ImageBox.ClearImage();
        }

        private List<PrintLabeModel> doublelist = new List<PrintLabeModel>();
        //选中尺码的时候
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            List<string> submitlist = new List<string>();
            ImageBindModel imagemodel = ImageBox.ReadSelectedImage();
            if (imagemodel == null)
            {
                CCMessageBox.Show("请选中颜色");
                (sender as RadioButton).IsChecked = false;
                return;
            }
            string Size = (sender as RadioButton).Content.ToString();
            int count = Convert.ToInt32(tb_count.Text);


            List<PrintLabeModel> modelList = SqlDalHelp.getWareFlagCode("", lab_WareNo.Content.ToString(), imagemodel.TitlePath, Size);

            if (modelList.Count > 0)
            {
                //读取锁定时间
                WaitSecondCount = dal.QuerySingle<SetValueModel>("select * from T_SetValues", new object[] { }).Value + 1;
                bool DoublePrint = Convert.ToBoolean(Check_Double.IsChecked);

                //这里用AOP写入日志
                var p = UnityAopFactory.PrintLabServices;
                IntercepGoodsLogModel LogModel = new IntercepGoodsLogModel();
                PrintLabeModel mm = modelList[0];


                LogModel.LogList = new List<GoodsLogModel>() {
                    new GoodsLogModel() {
                        UserName = CommonLogin.CommonUser.UserName,
                        GoodsNo = mm.WareNo,
                        GoodsDetail = mm.SpecCode,
                        SpecName=mm.SpecName,
                        LogTime = DateTime.Now,
                        LogType = Convert.ToInt32(GoodsLogType.Packing),
                        DoEvent = string.Format("标签打印:{0} 规格【{1}】",mm.SpecCode,mm.SpecName),
                    }
                };

                if (DoublePrint == true)
                {
                    doublelist.Add(modelList[0]);
                    if (doublelist.Count == 2)
                    {
                        p.PrintLabe(doublelist, true, LogModel);
                        // print.PrintLabe(doublelist, true);
                        doublelist.Clear();
                    }
                }
                else
                {
                    PrintLabeModel model = modelList[0];
                    // print.PrintLabe(model, count, DoublePrint);
                    // p.PrintLabe(model, count, DoublePrint, LogModel);

                    if (tb_printck.IsChecked == true)
                    {

                        GoodsModel ms = manager.GetGoodsAll(model.SpecCode.Contains(model.WareNo) ? model.SpecCode : model.WareNo + model.SpecCode);

                        //model.FlagCode = ms == null ? "" :  ms.P_postion;
                        model.WarehouseFlagCode = ms == null ? "" : ms.P_postion;
                    }

                    //批量打印 wl170722
                    if (ckIsBatchPrint.IsChecked.Equals(true))
                    {
                        batchPrintLableModel = model;
                        batchLogModel = LogModel;
                        batchSize = Size;
                        batchTitlePath = imagemodel.TitlePath;
                        txtNumber.Text = "0";
                        ShowModalDialog(true);
                        return;
                    }
                    else if (ckIsBatchPrint.IsChecked.Equals(false))
                    {
                        p.PrintLabe(model, 1, false, LogModel);
                        //插入日志
                        submitlist.Add(string.Format(@"insert into dbo.T_WareLog(WareNo,Color,Size,SpecCode,RecordTime,Operator,logType) values('{0}','{1}','{2}','{3}',getdate(),'{4}','{5}')",
                            model.WareNo, model.Color, model.Size, model.SpecCode.Replace(model.WareNo,""), CommonLogin.CommonUser.UserName, "标签打印"));
                    }

                }

                //改变规格状态和插入绩效
                submitlist.Add(string.Format(@"update  T_WareSpecList set activity = 1,stock=stock+1 where WareNo = '{0}' and Color = '{1}' and Size = '{2}'", lab_WareNo.Content.ToString(), imagemodel.TitlePath, Size));
                submitlist.Add(string.Format(@"update T_Performance set finishcount = finishcount+ {2} where CONVERT(varchar(100), dates, 111) = CONVERT(varchar(100), GETDATE(), 111)
                        and userId = {0} and Performance = {1}", UserId, Performanceid, count));



                bool bs = dal.ExecuteTransaction(submitlist, null);
                if (bs) //打印成功（刷新绩效）
                {
                    // ImageBox.ClearSelect();
                    CancelModel.WareNo = lab_WareNo.Content.ToString();
                    CancelModel.Size = Size;
                    CancelModel.Color = imagemodel.TitlePath;
                    tb_count.Text = "1";
                    PerPanel.BindPerformance(CommonLogin.CommonUser.Id);
                }
                else
                {
                    CCMessageBox.Show("打印失败");
                }
                //清空选中
                ImageBox.ClearSelect();
                //锁定按钮
                WaitSecond = WaitSecondCount;
                timer.Start();
                //btn_Print.IsEnabled = false;
                //btn_Print.Background = new SolidColorBrush(Colors.Gray);
                Radio_Panel.IsEnabled = false;
                Radio_Panel.Background = new SolidColorBrush(Colors.Snow);
            }
            else
            {
                CCMessageBox.Show("失败,没有获取到数据");
            }
        }


        #region dialog
        private void ShowModalDialog(bool bShow)
        {
            this.ModalDialog.IsOpen = bShow;
        }

        private void Dlg_BtnClose_Click(object sender, RoutedEventArgs e)
        {
            ShowModalDialog(false);
        }

        private void Dlg_BtnOK_Click(object sender, RoutedEventArgs e)
        {
            ShowModalDialog(false);
        }

        #endregion

        private void btnNumClick(object sender, RoutedEventArgs e)
        {
            string oper = ((Button)sender).Tag.ToString();

            string initNum = txtNumber.Text.Trim();
            if (initNum.Equals("0"))
            {
                initNum = oper;
            }
            else
            {
                initNum = initNum + oper;
            }

            txtNumber.Text = initNum;
        }

        private void btnDelNumber(object sender, RoutedEventArgs e)
        {
            txtNumber.Text = StringHelper.RemoveLast(txtNumber.Text);
        }

        private void btnBatchPrint(object sender, RoutedEventArgs e)
        {
            if (txtNumber.Text.Trim() != "")
            {
                if (StringHelper.IsInteger(txtNumber.Text))
                {
                    var p = UnityAopFactory.PrintLabServices;
                    p.PrintLabe(batchPrintLableModel, Convert.ToInt32(txtNumber.Text.Trim()), false, batchLogModel);

                    List<string> submitlist = new List<string>();
                    //改变规格状态和插入绩效
                    submitlist.Add(string.Format(@"update  T_WareSpecList set activity = 1,stock=stock+{3} where WareNo = '{0}' and Color = '{1}' and Size = '{2}'", lab_WareNo.Content.ToString(), batchTitlePath, batchSize, txtNumber.Text.Trim()));
                    submitlist.Add(string.Format(@"update T_Performance set finishcount = finishcount+ {2} where CONVERT(varchar(100), dates, 111) = CONVERT(varchar(100), GETDATE(), 111)
                        and userId = {0} and Performance = {1}", UserId, Performanceid, Convert.ToInt32(txtNumber.Text.Trim())));
                    //插入日志
                    submitlist.Add(string.Format(@"insert into dbo.T_WareLog(WareNo,Color,Size,SpecCode,RecordTime,Operator,logType) values('{0}','{1}','{2}','{3}',getdate(),'{4}','{5}')",
                        batchPrintLableModel.WareNo, batchPrintLableModel.Color, batchPrintLableModel.Size, batchPrintLableModel.SpecCode.Replace(batchPrintLableModel.WareNo,""), CommonLogin.CommonUser.UserName, " 标签批量打印(张) " + txtNumber.Text));

                    bool bs = dal.ExecuteTransaction(submitlist, null);
                    if (bs) //打印成功（刷新绩效）
                    {
                        // ImageBox.ClearSelect();
                        CancelModel.WareNo = lab_WareNo.Content.ToString();
                        CancelModel.Size = batchSize;
                        CancelModel.Color = batchTitlePath;
                        tb_count.Text = txtNumber.Text.Trim();
                        PerPanel.BindPerformance(CommonLogin.CommonUser.Id);
                    }
                    else
                    {
                        CCMessageBox.Show("打印失败");
                    }
                    //清空选中
                    ImageBox.ClearSelect();
                    //锁定按钮
                    WaitSecond = WaitSecondCount;
                    timer.Start();
                    //btn_Print.IsEnabled = false;
                    //btn_Print.Background = new SolidColorBrush(Colors.Gray);
                    Radio_Panel.IsEnabled = false;
                    Radio_Panel.Background = new SolidColorBrush(Colors.Snow);

                    batchTitlePath = string.Empty;
                    batchSize = string.Empty;
                    batchPrintLableModel = new PrintLabeModel();
                    batchLogModel = new IntercepGoodsLogModel();

                    ShowModalDialog(false);
                    txtNumber.Text = "0";
                }
            }


        }




    }
}
