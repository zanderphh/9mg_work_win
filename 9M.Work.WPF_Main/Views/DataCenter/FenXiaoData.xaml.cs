using _9M.Work.ErpApi;
using _9M.Work.Model;
using _9M.Work.TopApi;
using _9M.Work.Utility;
using _9M.Work.WPF_Common;
using _9M.Work.WPF_Main.Data;
using _9M.Work.WPF_Main.FrameWork;
using _9Mg.Work.TopApi;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
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
using System.Windows.Threading;
using Top.Api.Domain;

namespace _9M.Work.WPF_Main.Views.DataCenter
{
    /// <summary>
    /// FenXiaoData.xaml 的交互逻辑
    /// </summary>
    public partial class FenXiaoData : UserControl
    {
        private DataTable posttable = new DataTable();
        private TopSource detail = new TopSource();
        private List<DataBindModel> bindlist = new List<DataBindModel>();
        private ObservableCollection<DataBindModel> _numberDescriptions;
        public delegate void SyncGoodsHandler(UpdateGoodsSub model, bool IsErrorOperation);
        private GoodsManager Manager = new GoodsManager();
        //更新进度条委托
        private delegate void UpdateProgressBarDelegate(System.Windows.DependencyProperty dp, Object value);
        public FenXiaoData()
        {
            InitializeComponent();
            int SH = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
            scrolls.Height = SH - 470;
        }

        #region 商品更新

        public int GetRadioButtonIndex(DependencyObject obj)
        {
            int Index = 0;
            List<RadioButton> list = WPFControlsSearchHelper.GetChildObjects<RadioButton>(obj, "");
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].IsChecked == true)
                {
                    Index = i;
                    break;
                }
            }
            return Index;
        }
        public void ShowHideControls(bool IsTitleControl, int Tag)
        {
            if (IsTitleControl)
            {
                if (Tag == 1)
                {
                    Wrap_add.Visibility = System.Windows.Visibility.Collapsed;
                    Wrap_append.Visibility = System.Windows.Visibility.Collapsed;
                }
                else if (Tag == 2)
                {
                    Wrap_add.Visibility = System.Windows.Visibility.Visible;
                    Wrap_append.Visibility = System.Windows.Visibility.Collapsed;
                }
                else if (Tag == 3)
                {
                    Wrap_add.Visibility = System.Windows.Visibility.Collapsed;
                    Wrap_append.Visibility = System.Windows.Visibility.Visible;
                }
            }
            else
            {
                if (Tag == 4)
                {
                    WrapDesc_add.Visibility = System.Windows.Visibility.Collapsed;
                    WrapDesc_append.Visibility = System.Windows.Visibility.Collapsed;
                }
                else if (Tag == 5)
                {
                    WrapDesc_add.Visibility = System.Windows.Visibility.Visible;
                    WrapDesc_append.Visibility = System.Windows.Visibility.Collapsed;
                }
                else if (Tag == 6)
                {
                    WrapDesc_add.Visibility = System.Windows.Visibility.Collapsed;
                    WrapDesc_append.Visibility = System.Windows.Visibility.Visible;
                }
            }
        }
        //public void InitProductLine()
        //{
        //    Wrap_Line.Children.Clear();
        //    List<ProductCat> list =  dr.getProductLine();
        //    if(list.Count>0)
        //    {

        //        list.ForEach(x =>
        //        {
        //            Wrap_Line.Children.Add(new RadioButton() { Content = x.Name, Tag = x.Id, Margin = new Thickness(8, 0, 0, 0) });
        //        });
        //        Wrap_Line.Children.Add(new RadioButton() { Content = "不处理", Tag = 0, Margin = new Thickness(8, 0, 0, 0), IsChecked = true });
        //    }

        //}

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            int Tag = Convert.ToInt32((sender as RadioButton).Tag);
            ShowHideControls(Tag < 4, Tag);
        }

        //更新表格
        private delegate void LoadNumberDelegate(int Index);
        private void LoadNumber(int Index)
        {
            if (Index == bindlist.Count)
            {
                slider.Value = 0;
                lab_curent.Content = 0;
                return;
            }
            _numberDescriptions.Add(bindlist[Index]);
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new LoadNumberDelegate(LoadNumber), ++Index);
            lab_count.Content = Index;
        }
        public void Import()
        {
            bindlist.Clear();
            OpenFileDialog op = new OpenFileDialog();
            //过滤器
            op.Filter = "XLS|*.xls|XLSX|*.xlsx";
            if (op.ShowDialog() == true) //选择完成之后
            {

                DataTable dt = ExcelNpoi.ExcelToDataTable("sheet1", true, op.FileName);
                if (!dt.Columns[0].ColumnName.Equals("款号"))
                {
                    CCMessageBox.Show("列头为款号");
                    return;
                }
                foreach (DataRow dr in dt.Rows)
                {
                    DataBindModel model = new DataBindModel()
                    {
                        GoodsNo = dr["款号"].ToString().Trim(),
                        // Price = dr["价格"] != null ? dr["价格"].ToString() : string.Empty
                    };
                    bindlist.Add(model);
                }

                bindlist = bindlist.Distinct().Where(x => !string.IsNullOrEmpty(x.GoodsNo)).ToList();
                if (bindlist.Count > 0)
                {
                    CCMessageBox.Show("导入商品件数:" + bindlist.Count);
                }
            }
        }
        UpdateGoodsSub fxp = new UpdateGoodsSub();
        private void UpdateWareButton_Click(object sender, RoutedEventArgs e)
        {

            int Tag = Convert.ToInt32((sender as Button).Tag);
            switch (Tag)
            {
                case 0: //导款
                    Import();
                    break;
                case 1: //预览修改
                    fxp.Pid = 0;
                    fxp.Brand = tb_brand.Text;
                    fxp.SyncPrice = Convert.ToBoolean(check_price.IsChecked);
                    fxp.SyncStock = Convert.ToBoolean(check_syncstock.IsChecked);
                    fxp.PostStatus = GetRadioButtonIndex(radio_post);
                    fxp.TitleStatus = GetRadioButtonIndex(Wrap_title);
                    fxp.GoodsStatus = GetRadioButtonIndex(Wrap_Updown);
                    fxp.DescStatus = GetRadioButtonIndex(Wrap_Desc);
                    fxp.AppendTitle = tb_titleadd.Text;
                    fxp.ReplaceTitleValue = tb_titleval.Text;
                    fxp.ReplaceTitleResult = tb_titleres.Text;
                    fxp.AppendDesc = tb_descadd.Text;
                    fxp.ReplaceDescValue = tb_descval.Text;
                    fxp.ReplaceDescResult = tb_descres.Text;
                    bindlist.ForEach(x =>
                    {
                        x.IsUpdateBrand = !string.IsNullOrEmpty(fxp.Brand);
                        x.IsUpdateDesc = fxp.DescStatus > 0;
                        x.IsUpdatePost = fxp.PostStatus > 0;
                        x.IsUpdatePrice = fxp.SyncPrice;
                        x.IsUpdateStatus = fxp.GoodsStatus > 0;
                        x.IsUpdateStock = fxp.SyncStock;
                        x.IsUpdateTitle = fxp.TitleStatus > 0;
                    });
                    _numberDescriptions = new ObservableCollection<DataBindModel>();
                    UpdateGoodsGrid.ItemsSource = _numberDescriptions;
                    Dispatcher.BeginInvoke(DispatcherPriority.Background, new LoadNumberDelegate(LoadNumber), 0);
                    break;
                case 2: //开始修改
                    SyncGoodsHandler sgh = new SyncGoodsHandler(this.UpdateGoods);
                    IAsyncResult ir = sgh.BeginInvoke(fxp, false, null, null);
                    break;
                case 3:  //处理错误
                    SyncGoodsHandler sg = new SyncGoodsHandler(this.UpdateGoods);
                    IAsyncResult irs = sg.BeginInvoke(fxp, true, null, null);
                    break;
                case 4: //日志导出
                    SaveFileDialog op = new SaveFileDialog();
                    op.Filter = "文本文件|*.txt|所有文件|*.*";
                    if (op.ShowDialog() == true)
                    {
                        string filename = op.FileName;
                        StreamWriter sw = new StreamWriter(filename);
                        for (int i = 0; i < UpdateGoodsGrid.Items.Count; i++)
                        {
                            sw.WriteLine((ControlHelper.FindGridControl(UpdateGoodsGrid, 0, i) as TextBlock).Text + "       " + (ControlHelper.FindGridControl(UpdateGoodsGrid, 3, i) as TextBlock).Text);
                        }
                        sw.Close();
                    }
                    break;
                case 5: //过滤成功记录
                    for (int i = UpdateGoodsGrid.Items.Count - 1; i >= 0; i--)
                    {
                        TextBlock rescolum = (ControlHelper.FindGridControl(UpdateGoodsGrid, 3, i) as TextBlock);
                        if (rescolum.Text.Equals("修改成功"))
                        {
                            _numberDescriptions.RemoveAt(i);
                        }
                    }
                    break;
            }

        }



        private void UpdateGoods(UpdateGoodsSub model, bool IsErrorOperateion)
        {
            List<NoPayModel> tradelist = null;
            if (model.SyncStock)
            {
                bar.LoadBar(true);
                bar.Loading(true);
                bar.SetNavigation("正在查询未付的订单");
                tradelist = new CommonTrade().GetNoPayList().Where(x=>!string.IsNullOrEmpty(x.OuterSkuId)).ToList();
                bar.SetNavigation("");
                bar.LoadBar(false);
                bar.Loading(false);
            }
    
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                slider.Value = 0;
                slider.Maximum = UpdateGoodsGrid.Items.Count;
                lab_curent.Content = UpdateGoodsGrid.Items.Count;
                for (int i = 0; i < UpdateGoodsGrid.Items.Count; i++)
                {
                    lab_curent.Content = (i + 1).ToString();
                    TextBlock rescolum = (ControlHelper.FindGridControl(UpdateGoodsGrid, 3, i) as TextBlock);
                    if (IsErrorOperateion == true)
                    {
                        if (rescolum.Text.Equals("修改成功") || rescolum.Text.Equals("没有获取到商品信息"))
                        {
                            slider.Value = i + 1;
                            continue;
                        }
                    }
                    string Reslut = string.Empty;
                    StackPanel stack = ControlHelper.FindGridTemplateControl(UpdateGoodsGrid, 2, i, "syncpro") as StackPanel;
                    ProgressBar bar = stack.Children[0] as ProgressBar;
                    UpdateProgressBarDelegate updatePbDelegate = new UpdateProgressBarDelegate(bar.SetValue);
                    Label label = stack.Children[1] as Label;
                    bar.Value = 0;
                    bar.Maximum = 100;
                    FenxiaoProduct item = detail.GetScItemByCode(bindlist[i].GoodsNo,string.Empty);
                    Dispatcher.Invoke(updatePbDelegate, System.Windows.Threading.DispatcherPriority.Background, new object[] { System.Windows.Controls.ProgressBar.ValueProperty, Convert.ToDouble(50) });
                    label.Content = Convert.ToInt32((Convert.ToDouble(50) / 100) * 100) + "%";
                    if (item != null)
                    {
                        string Res = detail.UpdateProductByModel(item, model, tradelist, new List<ProductCat>(),new List<FenXiaoPropModel>());
                        //string Res = "success";
                        Reslut = Res.Equals("success") ? "修改成功" : Res;
                    }
                    else
                    {
                        Reslut = "没有获取到商品信息";
                    }
                    Dispatcher.Invoke(updatePbDelegate, System.Windows.Threading.DispatcherPriority.Background, new object[] { System.Windows.Controls.ProgressBar.ValueProperty, Convert.ToDouble(100) });
                    label.Content = Convert.ToInt32((Convert.ToDouble(100) / 100) * 100) + "%";
                    rescolum.Text = Reslut;
                    slider.Value = i + 1;
                }
            }));
        }

        #endregion

        #region 发货处理
        //异步委托
        public delegate void AsyncEventHandler(DataTable dt);
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int Tag = Convert.ToInt32((sender as Button).Tag);
            switch (Tag)
            {
                case 0: //导入表格
                    bool b = false;
                    OpenFileDialog op = new OpenFileDialog();
                    //过滤器
                    op.Filter = "XLS|*.xls|XLSX|*.xlsx";
                    if (op.ShowDialog() == true)
                    {
                        posttable = ExcelNpoi.ExcelToDataTable("sheet1", true, op.FileName);
                        if (posttable.Columns.Count >= 3)
                        {
                            if (posttable.Columns[0].ColumnName.Equals("订单编号") && posttable.Columns[1].ColumnName.Equals("快递方式") && posttable.Columns[2].ColumnName.Equals("快递单号"))
                            {
                                b = true;
                            }
                        }
                    }
                    else
                    {
                        return;
                    }
                    if (b == false)
                    {
                        CCMessageBox.Show("您导入的数据格式不正确");
                    }
                    else
                    {
                        CCMessageBox.Show("数据导入成功");
                    }
                    break;
                case 1:
                    if (CCMessageBox.Show("您确定要发货吗", "提示", CCMessageBoxButton.YesNo) == CCMessageBoxResult.Yes)
                    {
                        if (posttable.Rows.Count > 0)
                        {
                            //实例委托
                            AsyncEventHandler asy = new AsyncEventHandler(this.SendPost);
                            //异步调用开始，没有回调函数和AsyncState,都为null
                            IAsyncResult ia = asy.BeginInvoke(posttable, null, null);
                        }
                    }
                    break;
            }

        }

        private void SendPost(DataTable dt)
        {
            //创建一个TXT文件记录错误
            string filename = (DateTime.Now.ToShortDateString() + DateTime.Now.ToShortTimeString() + DateTime.Now.Second.ToString()).Replace("-", "").Replace(":", "");
            StreamWriter sw = new StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\发货错误日志-" + filename + ".txt");
            //打开加载
            bar.LoadBar(true);
            //打开加载中的字符
            bar.Loading(true);
            //初始化快递信息
            List<LogisticsCompany> CompanyList = detail.GetLogisticsCompanys();
            bar.Loading(false);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                int current = i == dt.Rows.Count - 1 ? dt.Rows.Count : i + 1;
                long Tid = Convert.ToInt64(dt.Rows[i]["订单编号"]);
                string SendString = dt.Rows[i]["快递方式"].ToString().Trim();
                string PostId = dt.Rows[i]["快递单号"].ToString().Trim();
                LogisticsCompany Com = CompanyList.Where(x => x.Name.Equals(SendString)).FirstOrDefault();
                if (Com == null) 
                {
                    //更新进度条

                    bar.UpdateBarValue(dt.Rows.Count, current);
                    sw.WriteLine(Tid + "--------" + "快递方式不存在");
                    continue;
                }
                PurchaseOrder order = detail.GetTradeByTid(Tid, string.Empty);
                if (order == null)
                {
                    //更新进度条

                    bar.UpdateBarValue(dt.Rows.Count, current);
                    sw.WriteLine(Tid + "--------" + "没有查询到订单");
                    continue;
                }
                string Res = detail.SendPost(order.FenxiaoId, PostId, Com.Code);
                if (!Res.Equals("success"))
                {
                    sw.WriteLine(Tid + "--------" + Res);
                }
                //更新进度条
                bar.UpdateBarValue(dt.Rows.Count, current);
            }
            sw.Close();
            bar.Loading(false);
            bar.LoadBar(false);
            posttable.Clear();
        }

        #endregion
    }

    public class DataBindModel
    {
        public string GoodsNo { get; set; }
        public string Price { get; set; }
        public bool IsUpdateBrand { get; set; }
        public bool IsUpdatePrice { get; set; }
        public bool IsUpdateStock { get; set; }
        public bool IsUpdatePost { get; set; }
        public bool IsUpdateStatus { get; set; }
        public bool IsUpdateTitle { get; set; }
        public bool IsUpdateDesc { get; set; }
    }
}
