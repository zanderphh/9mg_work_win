using _9M.Work.DbObject;
using _9M.Work.Model;
using _9M.Work.Utility;
using _9M.Work.WPF_Common.Controls;
using _9M.Work.WPF_Main.Infrastrcture;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
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

namespace _9M.Work.WPF_Main.Views.EveryDayUpdate
{
    /// <summary>
    /// ImageFind.xaml 的交互逻辑
    /// </summary>
    public partial class ImageFind : UserControl
    {
        //需要下载的款号
        private List<string> GoodsNoList = new List<string>();
        //保存的路径
        private string SavePath = string.Empty;
        //提取路径
        private string GetPath = string.Empty;
        bool Netbs = true;
        public ImageFind()
        {
            InitializeComponent();
            ConnNetDisk();
        }


        public void ConnNetDisk()
        {
            //连接网盘
            string LocalName = "Z:";
            string RemotePath = @"\\192.168.1.5\拾玖映画";
            string UserName = @"admin";
            string PassWord = "199711";
            try
            {
                //连接网盘
                int lengh = 20;
                int Conn = DriveReflection.WNetGetConnection(LocalName, new StringBuilder(RemotePath), ref lengh);
                if (Conn != 0)
                {
                    //清空共享连接
                    DriveReflection.RemoveShareNetConnect("192.168.1.5", "");
                    bool bb = DriveReflection.WNetDisconnectDrive(LocalName, true);
                    Netbs = DriveReflection.WNetReflectDrive(LocalName, RemotePath, UserName, PassWord);
                }
            }
            catch (Exception)
            {
                Netbs = false;
            }
            if (!Netbs)
            {
                CCMessageBox.Show("网盘连接失败");
                ControlDock(true);
                return;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string ButtonContent = (sender as Button).Tag.ToString();
            switch (ButtonContent)
            {
                case "0": //填写款号
                    ClearGoodsGrid();
                    GoodsNos.Text = "";
                    ImageBorder.Visibility = System.Windows.Visibility.Collapsed;
                    AddGoods.Visibility = System.Windows.Visibility.Visible;
                    break;
                case "1":  //导入文件
                    OpenFileDialog op = new OpenFileDialog();
                    op.Filter = "文本文件|*.txt";
                    if (op.ShowDialog() == true)
                    {
                        GoodsNoList.Clear();
                        StreamReader sr = new StreamReader(op.FileName, Encoding.Default);
                        String line;
                        while ((line = sr.ReadLine()) != null)
                        {
                            GoodsNoList.Add(line.ToString().Trim().TrimEnd('\n').TrimEnd('\r'));
                        }
                        sr.Close();
                    }
                    GridPannel(GoodsNoList.Distinct().ToList());
                    break;
                case "2":  //设置保存
                    SavePath = new BrowserDialog().GetUrl();
                    break;
                case "3":  //关闭款号框

                    AddGoods.Visibility = System.Windows.Visibility.Collapsed;
                    ImageBorder.Visibility = System.Windows.Visibility.Visible;
                    break;
                case "4":  //款号确定
                    GoodsNoList.Clear();
                    string[] str = GoodsNos.Text.Split('\n');
                    foreach (string s in str)
                    {
                        if (!string.IsNullOrEmpty(s))
                        {
                            GoodsNoList.Add(s.Trim().TrimEnd('\r'));
                        }
                    }
                    ImageBorder.Visibility = System.Windows.Visibility.Visible;
                    AddGoods.Visibility = System.Windows.Visibility.Collapsed;
                    GridPannel(GoodsNoList.Distinct().ToList());
                    break;
                case "5": //提取路径
                    
                    

                    GetPath = new BrowserDialog().GetUrl();
                    break;
            }
        }

        #region 下载图片

        /// <summary>
        /// 锁定或打开导航栏
        /// </summary>
        /// <param name="b"></param>
        public void ControlDock(bool b)
        {
            if (b == false)
            {
                SetDock.Background = new SolidColorBrush(Colors.WhiteSmoke);
            }
            else
            {
                SetDock.Background = new SolidColorBrush(Colors.White);
            }
            SetDock.IsEnabled = b;
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ControlDock(false);
            if (CCMessageBox.Show("是否下载图片", "提示", CCMessageBoxButton.YesNo) == CCMessageBoxResult.Yes)
            {
                //实例委托
                AsyncEventHandler asy = new AsyncEventHandler(this.DownImage);
                //异步调用开始，没有回调函数和AsyncState,都为null
                IAsyncResult ia = asy.BeginInvoke(GoodsNoList, null, null);
            }
            else
            {
                ControlDock(true);
            }

        }
        #endregion

        public void ClearGoodsGrid()
        {
            GoodsGrid.Children.Clear();
            GoodsGrid.RowDefinitions.Clear();
            GoodsGrid.ColumnDefinitions.Clear();
        }

        public void GridPannel(List<string> GoodsNoList)
        {
            ClearGoodsGrid();
            //每行几列
            int Column = 5;
            //得到行数
            int Rows = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(GoodsNoList.Count) / Column));
            //GRID布局
            for (int i = 0; i < Rows; i++)
            {
                RowDefinition row = new RowDefinition();
                row.Height = new GridLength(30);
                GoodsGrid.RowDefinitions.Add(row);
            }
            for (int j = 0; j < Column; j++)
            {
                ColumnDefinition col = new ColumnDefinition();
                GoodsGrid.ColumnDefinitions.Add(col);
            }

            //添加控件
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Column; j++)
                {
                    int Index = (i * Column) + j;
                    if (Index == GoodsNoList.Count)
                    {
                        break;
                    }
                    string GoodsNo = GoodsNoList[Index];
                    StackPanel panel = new StackPanel();
                    panel.Orientation = Orientation.Horizontal;
                    panel.Children.Add(new Label() { Content = GoodsNo, HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center, VerticalContentAlignment = System.Windows.VerticalAlignment.Center, Width = 80 });
                    panel.Children.Add(new ProgressBar() { Width = 100, Height = 20, Background = new SolidColorBrush(Colors.AliceBlue) });
                    panel.Children.Add(new Label() { Content = "", HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center, Foreground = new SolidColorBrush(Colors.SteelBlue) });
                    Grid.SetRow(panel, i);
                    Grid.SetColumn(panel, j);
                    GoodsGrid.Children.Add(panel);
                }
            }
        }

        //异步委托
        public delegate void AsyncEventHandler(List<string> GoodsNoList);
        //更新进度条委托
        private delegate void UpdateProgressBarDelegate(System.Windows.DependencyProperty dp, Object value);
        public void DownImage(List<string> GoodsNoList)
        {

            System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (!Netbs)
                {
                    CCMessageBox.Show("网盘连接故障,请打开关闭该功能从新连接");
                    ControlDock(true);
                    return;
                }
                string GoodsNos = string.Empty;
                GoodsNoList.ForEach(x =>
                {
                    GoodsNos += x + ",";
                });

                if (string.IsNullOrEmpty(GoodsNos))
                {
                    CCMessageBox.Show("请填写或导入款号");
                    ControlDock(true);
                    return;
                }
                //是否设置保存路径
                if (string.IsNullOrEmpty(SavePath))
                {

                    CCMessageBox.Show("没有设置保存路径");
                    ControlDock(true);
                    return;
                }

                int FWidth = 0;
                int FHeight = 0;
                int Thumbnail_Size = 0;
                int Thumbnail_Pecent = 0;
                //如果勾选了尺寸过滤


                if (Convert.ToBoolean(Chk_SizeFilter.IsChecked))
                {
                    int.TryParse(FilterWidth.Text, out FWidth);
                    int.TryParse(FilterHeight.Text, out FHeight);
                    if (FWidth == 0 && FHeight == 0)
                    {
                        CCMessageBox.Show("请正确填写过滤尺寸");
                        ControlDock(true);
                        return;
                    }
                }

                //如果勾选了压缩
                if (Convert.ToBoolean(Chk_Thumbnail.IsChecked))
                {
                    int.TryParse(ThumbnailSize.Text, out Thumbnail_Size);
                    int.TryParse(ThumbnailPecent.Text, out Thumbnail_Pecent);
                    if (Thumbnail_Size == 0 && Thumbnail_Pecent == 0)
                    {
                        CCMessageBox.Show("请正确填写压缩尺寸");
                        ControlDock(true);
                        return;
                    }
                }

                //得到图片地址
                string sql = string.Format("select * from picture where GoodsNo in ('{0}')", GoodsNos.TrimEnd(',').Replace(",", "','"));
                if (!string.IsNullOrEmpty(GetPath))
                {
                    sql = string.Format("select * from picture where GoodsNo in ('{0}') and Url like '%{1}%'", GoodsNos.TrimEnd(',').Replace(",", "','"), GetPath);
                }
                List<PictureModel> piclist = new BaseDAL().QueryList<PictureModel>(string.Format(sql));
                Dictionary<string, List<string>> dic = new Dictionary<string, List<string>>();

                //验证不包含图片的款号
                DataTable dt = new DataTable();
                dt.Columns.Add("款号", typeof(string));
                dt.Columns.Add("错误", typeof(string));
                string TelMsg = string.Empty;
                GoodsNoList.ForEach(x =>
                {
                    List<string> plist = piclist.Where(y => y.GoodsNo.Equals(x, StringComparison.CurrentCultureIgnoreCase)).Select(z => z.Url).ToList();
                    if (plist.Count > 0)
                    {
                        dic.Add(x, plist);
                    }
                    else
                    {
                        TelMsg += x + "\n";
                        DataRow dr = dt.NewRow();
                        dr["款号"] = x;
                        dr["错误"] = "不存在";
                        dt.Rows.Add(dr);
                    }
                });
                if (!string.IsNullOrEmpty(TelMsg))
                {

                    if (CCMessageBox.Show("(己复制)发现没有图片的款号\n" + TelMsg + "是否继续下载", "提示", CCMessageBoxButton.YesNoCancel) == CCMessageBoxResult.No)
                    {
                        return;
                    }
                }
                //创建文件夹

                if (dic.Count > 0)
                {
                    string Dir = SavePath + "\\DownImage";
                    string SheYingDir = SavePath + "\\DownImage\\放大镜";
                    bool IsSheYing = Convert.ToBoolean(check_Sheying.IsChecked);
                    if (!Directory.Exists(Dir))
                    {
                        Directory.CreateDirectory(Dir);
                    }
                    //如果勾选了摄影部的需求
                    if (IsSheYing == true)
                    {
                        if (!Directory.Exists(SheYingDir))
                        {
                            Directory.CreateDirectory(SheYingDir);
                        }
                    }
                    //开始下载

                    for (int i = 0; i < GoodsNoList.Count; i++)
                    {
                        List<string> plist = piclist.Where(y => y.GoodsNo.Equals(GoodsNoList[i], StringComparison.CurrentCultureIgnoreCase)).Select(z => z.Url).ToList();
                        if (plist.Count == 0)
                        {
                            continue;
                        }
                        //得到控件
                        string PanelName = "panel" + i;
                        StackPanel panel = GoodsGrid.Children[i] as StackPanel;
                        //寻找路径
                        var list = dic.Where(x => x.Key.Equals(GoodsNoList[i], StringComparison.CurrentCultureIgnoreCase)).ToList();
                        if (list.Count == 0)
                        {
                            continue;
                        }
                        //得到进度条与提示
                        ProgressBar bar = panel.Children[1] as ProgressBar;
                        Label label = panel.Children[2] as Label;
                        if (list.Count > 0)
                        {
                            int ImageCount = 0;
                            //为款号创建文件夹
                            string GoodsDir = Dir + "\\" + GoodsNoList[i];
                            if (!Directory.Exists(GoodsDir))
                            {
                                Directory.CreateDirectory(GoodsDir);
                            }
                            //得到图片路径
                            List<string> UrlList = list[0].Value;
                            bar.Value = 0;
                            bar.Maximum = UrlList.Count;
                            UpdateProgressBarDelegate updatePbDelegate = new UpdateProgressBarDelegate(bar.SetValue);
                            int NameIndex = 0;

                            //用一个值来记录1200放大镜的地址
                            List<string> UniqueList = new List<string>();
                            string BigUrl = string.Empty;
                            string BaseUrl = string.Empty;
                            for (int j = 0; j < UrlList.Count; j++)
                            {
                                if (File.Exists(UrlList[j]))
                                {
                                    BitmapImage bmp = new BitmapImage(new Uri(UrlList[j], UriKind.RelativeOrAbsolute));
                                    //图片尺寸过滤
                                    if (Convert.ToBoolean(Chk_SizeFilter.IsChecked))
                                    {
                                        //图片最大尺寸偏移度
                                        int deviation = 50;
                                        //得到图片的尺寸
                                        double phasewidth = FWidth - bmp.PixelWidth < 0 ? (FWidth - bmp.PixelWidth) * -1 : FWidth - bmp.PixelWidth;
                                        double phaseheight = FHeight - bmp.PixelHeight < 0 ? (FHeight - bmp.PixelHeight) * -1 : FHeight - bmp.PixelHeight;

                                        //如果只填写了宽度
                                        if (FWidth != 0 && FHeight == 0)
                                        {
                                            if (phasewidth > deviation)
                                            {
                                                continue;
                                            }
                                        }
                                        //如果只填写了高度
                                        else if (FWidth == 0 && FHeight != 0)
                                        {
                                            if (phaseheight > deviation)
                                            {
                                                continue;
                                            }
                                        }
                                        //如果都有填写
                                        else if (phasewidth > deviation || phaseheight > deviation)
                                        {
                                            continue;
                                        }
                                    }
                                    //压缩保存图片
                                    //得到图片名字
                                    NameIndex++;
                                    // string FilePath = GoodsDir + "\\" + GoodsNoList[i] + "-" + NameIndex + ".jpg";
                                    //string FilePath = GoodsDir + "\\" + System.IO.Path.GetFileName(UrlList[j]);
                                    string RealDir = GoodsDir;

                                    //判断是否勾选摄影模式来分配指定的目录
                                    if (IsSheYing == true)
                                    {

                                        UrlList[j] = UrlList[j].ToUpper();
                                        //指定目录
                                        if (!UrlList[j].Contains(@"Z:\后期工作区\成品（上新用）\2015\平铺") && !UrlList[j].Contains(@"Z:\后期工作区\成品（上新用）\2015\模特") && !UrlList[j].Contains(@"Z:\后期工作区\成品（上新用）\2016\平铺") && !UrlList[j].Contains(@"Z:\后期工作区\成品（上新用）\2016\模特") && !UrlList[j].Contains(@"Z:\后期工作区\成品（上新用）\2017"))
                                        {
                                            continue;
                                        }

                                        if (bmp.PixelWidth >= 800 && bmp.PixelWidth <= 860)
                                        {
                                            if (bmp.PixelHeight == 800 || bmp.PixelHeight == 1200)
                                            {
                                                RealDir = SheYingDir;
                                                if (bmp.PixelHeight == 1200)//暂时不下载放大镜的图片(只选一张)
                                                {
                                                    UniqueList.Add(UrlList[j]);
  
                                                    continue;
                                                }
                                            }
                                            else
                                            {
                                                continue;
                                            }
                                        }
                                        else
                                        {
                                            if (bmp.PixelWidth==750 && (bmp.PixelHeight>400 && bmp.PixelHeight<430) && System.IO.Path.GetFileName(UrlList[j]).Contains("-0")) //
                                            {
                                                goto DownImage;
                                            }
                                            if (System.IO.Path.GetFileName(UrlList[j]).ToUpper().Contains("-A") || System.IO.Path.GetFileName(UrlList[j]).ToUpper().Contains("MT-") || System.IO.Path.GetFileName(UrlList[j]).Contains("-0") || System.IO.Path.GetFileName(UrlList[j].ToUpper()).Contains("-C"))
                                            {
                                                continue;
                                            }
                                            if (bmp.PixelWidth != 750 && bmp.PixelWidth != 680 && bmp.PixelWidth != 331 && bmp.PixelWidth != 660)
                                            {
                                                continue;
                                            }
                                        }

                                    }
                                    DownImage:
                                    {
                                        //将带有补图的字符的图片放到子文件
                                        if (IsSheYing == true && UrlList[j].Contains("补"))
                                        {
                                            RealDir = Dir;
                                        }
                                        string FilePath = RealDir + "\\" + System.IO.Path.GetFileName(UrlList[j]);
                                        //如果存在同名的。自动生成一个文件名 
                                        if (File.Exists(FilePath))
                                        {
                                            FilePath = RealDir + "\\" + System.IO.Path.GetFileNameWithoutExtension(UrlList[j]) + "_" + GuidTo16String() + ".jpg";
                                        }
                                        FilePath = FilePath.ToUpper();
                                        if (Convert.ToBoolean(Chk_Thumbnail.IsChecked))
                                        {
                                            ImageWatermark.GetPicThumbnail(UrlList[j], FilePath, Convert.ToInt32(bmp.PixelHeight * (Convert.ToDouble(Thumbnail_Size) / 100)), Convert.ToInt32(bmp.PixelWidth * (Convert.ToDouble(Thumbnail_Size) / 100)), Thumbnail_Pecent);
                                        }
                                        else //直接保存图片
                                        {
                                            File.Copy(UrlList[j], FilePath, true);
                                        }
                                        ImageCount++;
                                    }
                                }
                                label.Content = Convert.ToInt32((Convert.ToDouble(j + 1) / UrlList.Count) * 100) + "%";
                                Dispatcher.Invoke(updatePbDelegate, System.Windows.Threading.DispatcherPriority.Background, new object[] { System.Windows.Controls.ProgressBar.ValueProperty, Convert.ToDouble(j + 1) });
                            }
                            if (IsSheYing == true)
                            {
                                //如果数据小于5张那么做费
                                if (ImageCount < 5)
                                {
                                    DataRow dr = dt.NewRow();
                                    dr["款号"] = GoodsNoList[i];
                                    dr["错误"] = "少于五张";
                                    dt.Rows.Add(dr);
                                    Directory.Delete(GoodsDir, true);
                                }
                                else
                                {
                                    //if (!string.IsNullOrEmpty(BigUrl)) //下载放大镜图片(1张)
                                    //{
                                    //    File.Copy(BaseUrl, BigUrl, true);
                                    //}

                                    if (UniqueList.Count > 0)//下载放大镜图片(1张)
                                    {
                                        bool bs = false;
                                        foreach (var item in UniqueList)
                                        {
                                            if (System.IO.Path.GetFileName(item).Contains("(") || System.IO.Path.GetFileName(item).Contains("（"))
                                            {
                                                bs = true;
                                                BigUrl = SheYingDir + "\\" + System.IO.Path.GetFileName(item);
                                                BaseUrl = item;
                                                break;
                                            }
                                        }
                                        if(bs == false)
                                        {
                                            BigUrl = SheYingDir + "\\" + System.IO.Path.GetFileName(UniqueList[0]);
                                            BaseUrl = UniqueList[0];
                                        }
                                        File.Copy(BaseUrl, BigUrl, true);
                                    }
                                }
                            }
                            bar.Value = 0;
                            label.Content = "完成";
                        }
                        else
                        {
                            label.Content = "未能找到图片";
                        }

                    }
                }
                //不存在图片的款保存EXCEl
                if (!string.IsNullOrEmpty(TelMsg))
                {
                    if (check_Sheying.IsChecked == true)
                    {
                        ExcelNpoi.TableToExcelForXLSX(dt, SavePath + "\\DownImage\\NotImage.xlsx");
                    }
                }
                ControlDock(true);
            }));
        }
        public static string GuidTo16String()
        {
            long i = 1;
            foreach (byte b in Guid.NewGuid().ToByteArray())
                i *= ((int)b + 1);
            return string.Format("{0:x}", i - DateTime.Now.Ticks);
        }
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("款号", typeof(string));
            dt.Columns.Add("是否存在", typeof(string));
            string res = string.Empty;
            GoodsNoList.ForEach(x =>
            {
                res += x + ",";
            });
            string sql = string.Format(@"select GoodsNo,'0' as  Url from Picture where GoodsNo in ('{0}') group by GoodsNo", res.TrimEnd(',').Replace(",", "','"));
            List<PictureModel> list = new BaseDAL().QueryList<PictureModel>(sql, new object[] { });
            GoodsNoList.ForEach(x =>
            {
                DataRow dr = dt.NewRow();
                dr["款号"] = x;
                if (list.Where(y => y.GoodsNo.Equals(x, StringComparison.CurrentCultureIgnoreCase)).Count() > 0)
                {

                    dr["是否存在"] = "是";
                }
                else
                {
                    dr["是否存在"] = "否";
                }
                dt.Rows.Add(dr);
            });

            SaveFileDialog sf = new SaveFileDialog();
            sf.Filter = "XLS|*.xls|XLSX|*.xlsx";
            sf.FileName = DateTime.Now.ToShortDateString();
            if (sf.ShowDialog() == true)
            {
                ExcelNpoi.TableToExcel(dt, sf.FileName);
            }
            else
            {
                CCMessageBox.Show("请先查询");
            }

        }

    }
}
