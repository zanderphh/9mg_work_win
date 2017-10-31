using _9M.Work.DbObject;
using _9M.Work.ErpApi;
using _9M.Work.Model;
using _9M.Work.Utility;
using _9M.Work.WPF_Main.FrameWork;
using _9M.Work.WPF_Main.Infrastrcture;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
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
    /// FenxiaoPerformance.xaml 的交互逻辑
    /// </summary>
    public partial class FenxiaoPerformance : UserControl
    {
        private BaseDAL dal = new BaseDAL();
        private StatisticsManager manager = new StatisticsManager();
        private DataTable exportTable = null;
        //异步委托
        public delegate void AsyncEventHandler(List<FenxiaoKeFuModel> kufulist, int Year, int Month, int PageNo, int Jiange, string StartTime, string EndTime);
        public FenxiaoPerformance()
        {
            InitializeComponent();
            Init();
        }

        public void Init()
        {
            DateTime now = DateTime.Now;
            com_month.SelectedIndex = DateTime.Now.Month - 2;
            date_start.Text = new DateTime(now.Year, now.Month == 1 ? 1 : now.Month - 1, 1).ToShortDateString();
            date_end.Text = new DateTime(now.Year, now.Month == 1 ? 2 : now.Month, 1).ToShortDateString();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int Tag = Convert.ToInt32((sender as Button).Tag);
            switch (Tag)
            {
                case 0:
                    FormInit.OpenDialog(this, new FenxiaoKeFu(), false);
                    break;
            }
        }

        private void Btn_CommandClick(object sender, RoutedEventArgs e)
        {
            int Tag = Convert.ToInt32((sender as Button).Tag);
            switch (Tag)
            {
                case 0:
                    //清空值
                    TuBiao.Child = null;
                    exportTable = null;
                    //得到所有客服
                    List<FenxiaoKeFuModel> kefulist = dal.QueryList<FenxiaoKeFuModel>("select * from T_FenxiaoKeFu", new object[] { });
                    if (kefulist.Count == 0)
                    {
                        CCMessageBox.Show("请设置客服名单");
                        return;
                    }
                    //得到需要查询的年与月
                    int Year = Convert.ToInt32(com_Year.Text);
                    int month = com_month.SelectedIndex + 1;
                    int pagenos = Convert.ToInt32(tb_pageno.Text);
                    int jiange = Convert.ToInt32(tb_jiange.Text);
                    string Start = date_start.Text;
                    string End = date_end.Text;
                    //先查询一次数据库里是否存在数据记录
                    List<StatisticsModel> datalist = dal.QueryList<StatisticsModel>(string.Format(@"select KefuNIck as Item,COUNT(*) as [Count] from T_FenxiaoKeFu_Performance
                                            where StatisticsYear = {0} and StatisticsMonth= {1} group by KefuNIck", Year, month));

                    if (datalist.Count > 0)
                    {
                        if (CCMessageBox.Show("选中月份的数据己存在,【确定】直接显示,【取消】刷新数据", "提示", CCMessageBoxButton.YesNo) == CCMessageBoxResult.Yes)
                        {
                            CraeteChartTable(kefulist, datalist);
                            return;
                        }
                    }
                    if (CCMessageBox.Show("刷新数据可能需要很长时间，确定要同步数据吗", "提示", CCMessageBoxButton.YesNo) == CCMessageBoxResult.No)
                    {
                        return;
                    }

                    //实例委托
                    AsyncEventHandler asy = new AsyncEventHandler(this.Load);
                    //异步调用开始，没有回调函数和AsyncState,都为null
                    IAsyncResult ia = asy.BeginInvoke(kefulist, Year, month, pagenos, jiange, Start, End, null, null);
                    break;

                case 1:
                    if (exportTable != null)
                    {
                        SaveFileDialog sf = new SaveFileDialog();
                        sf.Filter = "XLS|*.xls|XLSX|*.xlsx";
                        sf.FileName = com_month.Text + "月绩效";
                        if (sf.ShowDialog() == true)
                        {
                            ExcelNpoi.TableToExcel(exportTable, sf.FileName);
                        }
                        else
                        {
                            CCMessageBox.Show("请先查询");
                        }
                    }
                    break;
            }
        }

        public void Load(List<FenxiaoKeFuModel> kefulist, int Year, int month, int pageno, int jiange, string date_start, string date_end)
        {

            ShopModel shopModel = dal.GetAll<ShopModel>().Single(a => a.shopId.Equals(1003));
            _9M.Work.TopApi.TopSource _top = new _9M.Work.TopApi.TopSource(shopModel);


            //打开进度条
            bar.LoadBar(true);

            //订单数据集合
            List<FenXiaoTradeModel> tlist = new List<FenXiaoTradeModel>();
            //用一个条件集合来记录错误
            List<FenXiaoTradeQueryModel> ErrorList = new List<FenXiaoTradeQueryModel>();
            //得到起止的时间
            DateTime start = Convert.ToDateTime(date_start);
            DateTime end = Convert.ToDateTime(date_end);
            //计算请求时间(API只能输入七天之间的时差)
            double days = Convert.ToDouble((end - start).Days);
            int querycount = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(days / 7))) == 0 ? 1 : Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(days / 7)));
            DateTime d1;
            DateTime d2;
            //分段请求
            for (int i = 0; i < querycount; i++)
            {
                bar.SetNavigation(string.Format("分段【{0}】--当前【{1}】", querycount, i + 1));
                d1 = start.AddDays(+7 * i);
                if (i == querycount - 1)
                {
                    d2 = end;
                }
                else
                {
                    d2 = start.AddDays(+7 * (i + 1));
                }
                //请求的参数
                FenXiaoTradeQueryModel model = new FenXiaoTradeQueryModel();
                model.Start_created = d1;
                model.End_created = d2;
                model.Status = string.Empty;
                model.TradeId = 0;
                model.Fields = "fenxiao_id,supplier_memo,created,sub_purchase_orders";
                model.PageSize = 50;
                //读取总页码
                #region wl 2017-05-26 替换下列方法
                //int PageTotal = manager.GetTradeAllPage(model);
                #endregion

                int PageTotal = _top.GetAllPage(model);



                if (PageTotal == -1)
                {
                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                      {
                          CCMessageBox.Show("读取页码错误");
                      }));
                    return;
                }
                //需要循环的次数
                int count = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(PageTotal) / pageno));
                for (int j = 0; j < count; j++)
                {
                    List<int> ilist = new List<int>();
                    for (int z = j * pageno + 1; z <= (j + 1) * pageno; z++)
                    {
                        if (z <= PageTotal)
                        {
                            ilist.Add(z);
                        }
                    }
                    model.PageNos = ilist;

                    #region wl 2017-05-26 替换下列方法
                    //FenXiaoTradeReslutModel resmodel = manager.FenXiaoTrade(model);
                    #endregion

                    FenXiaoTradeReslutModel resmodel = _top.FromApiTrade(model);

                    //结果处理
                    //错误处理
                    if (resmodel.ErrorPage.Count > 0)
                    {
                        model.PageNos = resmodel.ErrorPage;
                        ErrorList.Add(model);
                    }
                    //订单数据
                    tlist.AddRange(resmodel.list);
                    //更新进度条
                    int current = j == count - 1 ? count : j + 1;
                    bar.UpdateBarValue(count, current);
                    Thread.Sleep(jiange * 1000);
                }
            }
            //处理错误的订单
            if (ErrorList.Count > 0)
            {


                int Ops = 0;
                while (ErrorList.Count > 0)
                {
                    Ops++;
                    if (CCMessageBox.Show("错误的订单数量【" + ErrorList.Count + "】,是否进行第【" + Ops + "】次处理", "提示", CCMessageBoxButton.YesNo) == CCMessageBoxResult.Yes)
                    {
                        bar.SetNavigation(string.Format("处理错误总数【{0}】", ErrorList.Count));
                        for (int i = 0; i < ErrorList.Count; i++)
                        {
                            #region wl 2017-05-26 替换下列方法
                            //FenXiaoTradeReslutModel resmodel = manager.FenXiaoTrade(ErrorList[i]);
                            #endregion

                            FenXiaoTradeReslutModel resmodel = _top.FromApiTrade(ErrorList[i]);

                            if (resmodel.ErrorPage.Count == 0)
                            {
                                tlist.AddRange(resmodel.list);
                                ErrorList.RemoveAt(i);
                            }
                            //更新进度条
                            int current = i == ErrorList.Count - 1 ? ErrorList.Count : i + 1;
                            bar.UpdateBarValue(ErrorList.Count, current);
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }


            //开始处理绩效
            bar.SetNavigation(string.Format("处理绩效"));
            //转换时间
            var qur = from pp in tlist
                      select new FenXiaoTradeModel()
                      {
                          FenxiaoId = pp.FenxiaoId,
                          SupplierMemo = pp.SupplierMemo.Replace("年", "-").Replace("月", "-").Replace("日", " ").Replace("\r", ""),
                          Created = pp.Created
                      };
            List<FenXiaoTradeModel> rlist = qur.ToList();
            //处理绩效
            //分割时间与事件
            String repreg = @"\d{4}-\d{1,2}-\d{1,2} \d{1,2}:\d{1,2}:\d{1,2}";
            DataTable dt = new DataTable();
            dt.Columns.Add("Id", typeof(int));
            dt.Columns.Add("FenXiao_Id", typeof(long));
            dt.Columns.Add("Created", typeof(DateTime));
            dt.Columns.Add("remark", typeof(string));
            dt.Columns.Add("kefunick", typeof(string));
            dt.Columns.Add("OpertionDate", typeof(DateTime));
            dt.Columns.Add("StatisticsYear", typeof(int));
            dt.Columns.Add("StatisticsMonth", typeof(int));
            Regex reg = new Regex(@"^*\d{4}-\d{1,2}-\d{1,2} \d{1,2}:\d{1,2}:\d{1,2}$");
            for (int i = 0; i < rlist.Count; i++)
            {
                //得到备注与处理
                string Remark = rlist[i].SupplierMemo;
                MatchCollection mc = Regex.Matches(Remark, repreg);
                foreach (Match m in mc)
                {
                    Remark = Regex.Replace(Remark, repreg, m.Value + "\n");
                }
                //得到数组
                string[] testreg = Remark.Split('\n').Where(x => !string.IsNullOrEmpty(x)).ToArray();

                for (int j = 0; j < testreg.Length; j++)
                {
                    string[] arry = RegexHelper.Split(testreg[j], reg);
                    //判断数据的格式
                    if (arry.Length == 2)
                    {
                        //判断处理时间,如果不是指定的月份，那么就忽略
                        DateTime trydt = DateTime.Parse("1990-01-01 00:00:00");
                        DateTime.TryParse(arry[1], out trydt);

                        if (trydt.Year != Year || trydt.Month != month)
                        {
                            continue;
                        }
                        //除时间以外的字符
                        string Other = arry[0];
                        FenxiaoKeFuModel User = kefulist.Where(x => Other.Contains(x.Nick)).FirstOrDefault();
                        DataRow dr = dt.NewRow();
                        dr["Id"] = DBNull.Value;
                        dr["FenXiao_id"] = rlist[i].FenxiaoId;
                        dr["Created"] = DateTime.Parse(rlist[i].Created);

                        dr["OpertionDate"] = trydt;
                        if (User != null)
                        {
                            dr["remark"] = Other.Replace(User.Nick, "").Trim();
                            dr["kefunick"] = User.Nick;
                        }
                        else
                        {
                            dr["remark"] = Other;
                            dr["kefunick"] = "";
                        }
                        dr["StatisticsYear"] = Year;
                        dr["StatisticsMonth"] = month;
                        dt.Rows.Add(dr);
                    }
                }
                //更新进度条
                int current = i == rlist.Count - 1 ? rlist.Count : i + 1;
                bar.UpdateBarValue(rlist.Count, current);
            }
            //保存事件记录
            dal.ExecuteSql(string.Format("delete from T_FenxiaoKeFu_Performance where StatisticsYear={0} and StatisticsMonth = {1}", Year, month));
            string Saves = dal.BulkCopy(dt, "T_FenxiaoKeFu_Performance");
            if (Saves.Equals("success"))
            {
                //组合数据
                List<StatisticsModel> slist = dal.QueryList<StatisticsModel>(string.Format(@"select KefuNIck as Item,COUNT(*) as [Count] from T_FenxiaoKeFu_Performance
                                            where StatisticsYear = {0} and StatisticsMonth= {1} group by KefuNIck", Year, month));
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                  {
                      CraeteChartTable(kefulist, slist);
                  }));
            }
            bar.SetNavigation("");
            bar.LoadBar(false);
        }

        /// <summary>
        /// 创建一个图像。并把数据填充到DataTable
        /// </summary>
        /// <param name="kefulist"></param>
        /// <param name="slist"></param>
        public void CraeteChartTable(List<FenxiaoKeFuModel> kefulist, List<StatisticsModel> slist)
        {
            List<string> xlist = kefulist.Select(x => x.Nick).ToList();
            List<string> ylist = new List<string>();
            //保存TABLE
            exportTable = new DataTable();
            exportTable.Columns.Add("客服", typeof(string));
            exportTable.Columns.Add("总数", typeof(int));
            kefulist.ForEach(x =>
            {
                DataRow dr = exportTable.NewRow();
                StatisticsModel model = slist.Find(y =>
                {
                    return x.Nick.Equals(y.Item);
                });
                ylist.Add(model == null ? "0" : model.Count.ToString());
                dr["客服"] = x.Nick;
                dr["总数"] = model == null ? 0 : model.Count;
                exportTable.Rows.Add(dr);
            });
            //创建图像
            ChartHelper.CreateChartColumn(TuBiao, com_month.Text + "月绩效", this.Width, 800, "单", xlist, ylist);
        }

    }
}
