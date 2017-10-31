using _9M.Work.DbObject;
using _9M.Work.Model;
using _9M.Work.Model.Log;
using _9M.Work.Utility;
using _9M.Work.WPF_Common;
using _9M.Work.WPF_Common.Controls;
using _9M.Work.WPF_Common.WpfBind;
using _9M.Work.WPF_Main.ControlTemplate.PrintTemplate;
using _9M.Work.WPF_Main.FrameWork;
using _9M.Work.WPF_Main.Infrastrcture;
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
using System.Windows.Threading;

namespace _9M.Work.WPF_Main.Views.QualityCheck
{
    /// <summary>
    /// WaresSubsec.xaml 的交互逻辑
    /// </summary>
    public partial class WaresSubsec : UserControl
    {
        EDSDK_Help eh = new EDSDK_Help();
        BaseDAL dal = new BaseDAL();
        List<QualityBatchModel> qualist;
        List<CategoryModel> calist;
        List<ColorModel> colorlist;
        bool status = true;
        private string ShareImagePath = CommonLogin.RemoteDir;
        public string ImageGuid = string.Empty; //照片名字
        private bool IsGoodsPhoto; //切换拍照面板
        private DateTime PhotoTime; //记录拍照时间
        private LabelTemplate print = new LabelTemplate();
        private PrintHelper ph = new PrintHelper();
        private List<BrandModel> brandlist = null;
        public WaresSubsec()
        {
            InitializeComponent();
            //载入的时候生成一个GUID为图片保存做准备
            ImageGuid = GuidTo16String();
            qualist = dal.GetAll<QualityBatchModel>(new OrderModelField[] { new OrderModelField() { IsDesc = true, PropertyName = "Id" } });
            calist = dal.GetAll<CategoryModel>();
            colorlist = dal.GetAll<ColorModel>();
            brandlist = dal.GetAll<BrandModel>();
            InitValues();
            //清除共享连接
            bool cls = NetWorkUntity.clearState(CommonLogin.RemoteIp);
            //连接磁盘
            status = NetWorkUntity.connectState(ShareImagePath, CommonLogin.RemoteUser, CommonLogin.RemotePassWork);
            if (!status)
            {
                CCMessageBox.Show("网盘权限无法连接");
            }
            KeyValuePair<int, string> kvp = eh.InitLoadCamera();
            if (kvp.Value.Equals("相机已连接") && status == true)
            {
                InitText.Foreground = new SolidColorBrush(Colors.LightGreen);
            }
            string wangpantext = status ? "    网盘己连接" : "    网盘连接失败";
            InitText.Content = kvp.Value + wangpantext;
            eh.PhotoImageDownloaded += eh_ImageDownloaded;
        }

        public void InitValues()
        {
            ComboBoxBind.BindComboBox(Com_Batch, qualist, "BatchName", "Id");
            if (qualist.Count > 0)
            {
                Com_Batch.SelectedIndex = 0;
            }
            ComboBoxBind.BindComboBox(Com_category, calist, "CategoryName", "Id");
            if (calist.Count > 0)
            {
                Com_category.SelectedIndex = 0;
            }

            ComboBoxBind.BindComboBox(com_querybatch, qualist, "BatchName", "Id");
            if (qualist.Count > 0)
            {
                com_querybatch.SelectedIndex = 0;
            }
            ComboBoxBind.BindComboBox(com_querycategory, calist, "CategoryName", "Id");
            if (calist.Count > 0)
            {
                com_querycategory.SelectedIndex = 0;
            }

            ComboBoxBind.BindComboBox(Com_Color, colorlist, "Color", "Id");
            if (colorlist.Count > 0)
            {
                Com_Color.SelectedIndex = 0;
            }

            ComboBoxBind.BindComboBox(com_speccolorlist, colorlist, "Color", "Id");
            if (colorlist.Count > 0)
            {
                com_speccolorlist.SelectedIndex = 0;
            }


        }

        /// <summary>
        /// 拍照
        /// </summary>
        /// <param name="WareNo">款号</param>
        public void SubmitPhoto(string WareNo)
        {
            //判断有没有文件夹。没有就新建
            List<string> dirlist = Directory.GetDirectories(ShareImagePath).ToList();
            string WareDir = dirlist.Find(x =>
            {
                return x.Equals(WareNo, StringComparison.CurrentCultureIgnoreCase);
            });
            string Dir = string.Empty;
            if (!string.IsNullOrEmpty(WareDir))
            {
                Dir = ShareImagePath + "\\" + WareDir;

            }
            else
            {
                Dir = ShareImagePath + "\\" + WareNo;
                Directory.CreateDirectory(Dir);
            }
            //始终只拍一张照片
            if (File.Exists(System.IO.Path.Combine(Dir, ImageGuid + ".jpg")))
            {
                File.Delete(System.IO.Path.Combine(Dir, ImageGuid + ".jpg"));
            }

            EDSDK_Help.SaveDirecotryPath = Dir;
            EDSDK_Help.SavePhotoName = ImageGuid + ".jpg";
            KeyValuePair<int, string> kvp = eh.Camera_TakePicture();
            if (kvp.Key != 0)
            {
                InitText.Foreground = new SolidColorBrush(Colors.Red);
                InitText.Content = kvp.Value;
            }
            else
            {
                InitText.Content = kvp.Value;
            }
        }

        public void BindSpecImagePanel(string WareNo)
        {
            string Sql = string.Format(@"select WareNo,Color,ImageKey from dbo.T_WareSpecList where WareNo = '{0}'
            group by WareNo,Color,ImageKey", WareNo);
            List<WareSpecModel> speclist = dal.QueryList<WareSpecModel>(Sql, new object[] { });
            if (speclist.Where(x => x.Color.Contains("有")).Count() > 0)
            {
                Btn_PeiShi.Content = "刷新配饰";
            }
            else
            {
                Btn_PeiShi.Content = "添加配饰";
            }

            lab_fyi.Content = dal.QuerySingle<WareModel>(string.Format(@"select * from T_WareList where wareno = '{0}'", WareNo), new object[] { }).OriginalFyiCode;

            DataTable dt = speclist.ConvertToDataTable<WareSpecModel>();
            specimagepanel.BindImage(dt, WareNo, "ImageKey", "Color", 1.33);
        }

        #region 新建款号

        #region Event
        //拍照完成时
        private void eh_ImageDownloaded(object sender, TakePictureCompleteEventArgs e)
        {
            //字节绑定不会占用图片。可以删除
            string filename = System.IO.Path.Combine(EDSDK_Help.SaveDirecotryPath, EDSDK_Help.SavePhotoName);
            if (File.Exists(filename))
            {
                byte[] buffer = System.IO.File.ReadAllBytes(filename);
                if (IsGoodsPhoto == true)
                {
                    Image_Box.Source = new ImageSourceConverter().ConvertFrom(buffer) as BitmapSource;
                }
                else
                {
                    img_specbox.Source = new ImageSourceConverter().ConvertFrom(buffer) as BitmapSource;
                }

            }

            //Uri uri = new Uri(System.IO.Path.Combine(EDSDK_Help.SaveDirecotryPath, EDSDK_Help.SavePhotoName), UriKind.RelativeOrAbsolute);
            //Image_Box.Source = new BitmapImage(uri);
        }

        //下拉类目时
        private void Com_category_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CategoryModel ms = (CategoryModel)(sender as ComboBox).SelectedItem;
            //动态生成特点
            DynamicBind.BindTeDianPanel(ms, DeDianPanel, false);
            //生成品名
            QualityBatchModel batch = (QualityBatchModel)Com_Batch.SelectedItem;
            if (batch != null)
            {
                tb_WareName.Text = batch.Brand + " " + ms.CategoryName;
            }
            tb_WareNo.Text = "";
        }

        private void Com_Batch_DropDownClosed(object sender, EventArgs e)
        {
            CategoryModel ms = (CategoryModel)Com_category.SelectedItem;
            //生成品名
            QualityBatchModel batch = (QualityBatchModel)Com_Batch.SelectedItem;
            if (batch != null)
            {
                tb_WareName.Text = batch.Brand + " " + ms.CategoryName;
            }
            tb_WareNo.Text = "";
        }
        //下拉批次时
        private void Com_Batch_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            tb_WareNo.Text = "";
            QualityBatchModel qbm = (QualityBatchModel)(sender as ComboBox).SelectedItem;
            int Radio = 0;
            if (qbm.Spring)
            {
                Radio = 1;
            }
            else if (qbm.Summer)
            {
                Radio = 2;
            }
            else if (qbm.Autumn)
            {
                Radio = 3;
            }
            else if (qbm.Winter)
            {
                Radio = 4;
            }

            SolidColorBrush colorbrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0B8BEE"));
            List<RadioButton> radiolist = WPFControlsSearchHelper.GetChildObjects<RadioButton>(rad_Season, "");
            radiolist[0].IsEnabled = qbm.Spring;
            radiolist[0].BorderBrush = qbm.Spring ? colorbrush : null;
            radiolist[1].IsEnabled = qbm.Summer;
            radiolist[1].BorderBrush = qbm.Summer ? colorbrush : null;
            radiolist[2].IsEnabled = qbm.Autumn;
            radiolist[2].BorderBrush = qbm.Autumn ? colorbrush : null;
            radiolist[3].IsEnabled = qbm.Winter;
            radiolist[3].BorderBrush = qbm.Winter ? colorbrush : null;
            GetRadio(Radio);
        }

        //选择季节单选时
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            tb_WareNo.Text = "";
            string RadioContent = (sender as RadioButton).Content.ToString();
            QualityBatchModel ms = Com_Batch.SelectedItem as QualityBatchModel;
            if (ms != null)
            {
                string flag = string.Empty;
                switch (RadioContent)
                {

                    case "春":
                        flag = ms.Year1;
                        break;
                    case "夏":
                        flag = ms.Year2;
                        break;
                    case "秋":
                        flag = ms.Year3;
                        break;
                    case "冬":
                        flag = ms.Year4;
                        break;
                }
                tb_Year.Text = flag;
            }
        }

        //按钮事件
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int Tag = Convert.ToInt32((sender as Button).Tag);
            switch (Tag)
            {
                case 0:  //自动匹配款号
                    RandomWareNo();
                    break;
                case 1:  //添加款号
                    bool HasImage = Image_Box.Source.ToString().Contains("nopic.jpg");
                    if (HasImage == true)
                    {
                        CCMessageBox.Show("请拍照在添加款号");
                        return;
                    }
                    if (status == false)
                    {
                        CCMessageBox.Show("网盘无法连接");
                        return;
                    }
                    if (string.IsNullOrEmpty(tb_WareNo.Text))
                    {
                        CCMessageBox.Show("请填写款号");
                        return;
                    }
                    bool checkgoods = dal.QuerySingle<WareModel>(string.Format("select top 1 * from dbo.T_WareList where WareNo = '{0}'", tb_WareNo.Text.Trim()), new object[] { }) == null;
                    if (!checkgoods)
                    {
                        ImageGuid = GuidTo16String();
                        CCMessageBox.Show("款号重复 请重新生成");
                        return;
                    }

                    if (string.IsNullOrEmpty(Com_Color.Text))
                    {
                        CCMessageBox.Show("请选定一个颜色");
                        return;
                    }
                    //else  //如果没有颜色那么自动添加一条
                    //{
                    //    if (colorlist.Where(x => x.Color.Equals(Com_Color.Text)).Count() == 0)
                    //    {
                    //        ColorModel model = new ColorModel() { Color = Com_Color.Text };
                    //        dal.Add<ColorModel>(model);
                    //        colorlist = dal.GetAll<ColorModel>();
                    //    }
                    //}

                    QualityBatchModel qb = (QualityBatchModel)Com_Batch.SelectedItem;
                    if (qb == null)
                    {
                        CCMessageBox.Show("请选中批次");
                        return;
                    }
                    if (qb.IsLock == true)
                    {
                        CCMessageBox.Show("该批次被锁定,不能新建");
                        return;
                    }

                    CategoryModel categorymodel = (CategoryModel)Com_category.SelectedItem;
                    if (categorymodel == null)
                    {
                        CCMessageBox.Show("请选择分类");
                        return;
                    }

                    //内部号(为填充)
                    int GroupID = 1;
                    //WareModel wm = dal.QuerySingle<WareModel>(string.Format("select top 1 * from dbo.T_WareList where Batchid ={0} order by InSideGroupId Desc", qb.Id), new object[] { });
                    //if (wm != null)
                    //{
                    //    GroupID = wm.InSideGroupId + 1;
                    //}
                    List<WareModel> GroupIdList = dal.QueryList<WareModel>(string.Format(@"select * from dbo.T_WareList  where Batchid = {0} order by InSideGroupId", qb.Id), new object[] { });
                    if (GroupIdList.Count > 0)
                    {
                        List<int> intlist = GroupIdList.Select(x => x.InSideGroupId).Distinct().ToList();
                        bool b = false;
                        for (int i = 0; i < intlist.Count; i++)
                        {
                            if (i != intlist.Count - 1)
                            {
                                if (intlist[i + 1] - intlist[i] != 1)
                                {
                                    GroupID = intlist[i] + 1;
                                    b = true;
                                    break;
                                }
                            }
                        }
                        if (b == false)
                        {
                            GroupID = intlist[intlist.Count - 1] + 1;
                        }
                    }
                    WareModel waremodel = new WareModel();
                    waremodel.BatchId = qb.Id;
                    waremodel.CategoryId = categorymodel.Id;
                    waremodel.InSideGroupId = GroupID;
                    //添加特点的值
                    DynamicBind.GetWareDeDian(DeDianPanel, waremodel);
                    waremodel.OriginalFyiCode = tb_FyiCode.Text;
                    waremodel.Season = GetSelectedRadio();
                    waremodel.WareName = tb_WareName.Text;
                    waremodel.WareNo = tb_WareNo.Text.Trim();
                    waremodel.Years = tb_Year.Text;
                    waremodel.ImageKey = ImageGuid;
                    waremodel.HouDu = Com_HouDu.Text;
                    waremodel.Remark = rich_Remark.Text;
                    waremodel.Price = Convert.ToDecimal(tb_Price.Text);
                    List<string> sqllist = new List<string>();
                    sqllist.Add(string.Format(@"insert into dbo.T_WareList values({0},'{1}','{2}',{3},'{4}','{5}',{6},{7},'{8}','{9}','{10}','{11}','{12}','{13}','{14}',0,'{15}','{16}','{17}',{18},'0')",
                       waremodel.BatchId, waremodel.WareNo, waremodel.WareName, waremodel.InSideGroupId, waremodel.OriginalFyiCode, waremodel.Years, waremodel.Season, waremodel.CategoryId,
                       waremodel.XiuXing, waremodel.MianLiao, waremodel.YanSe, waremodel.MenJin, waremodel.LingXing, waremodel.QiTa, waremodel.ImageKey, DateTime.Now.ToString(), waremodel.HouDu, waremodel.Remark, waremodel.Price));

                    //用品牌来得到尺码
                    string Brand = GoodsHelper.BrandEn(waremodel.WareNo);
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
                    // List<string> SizeList = new List<string>() { "XS", "S", "M", "L", "XL", "XXL", "XXXL" };

                    //====== 2017-09-02 by wl 

                    //得到SPEC最大的ID
                    //WareSpecModel sp = dal.QuerySingle<WareSpecModel>(string.Format("select top 1 * from T_WareSpecList where WareNo ='{0}' order by SpecCode Desc", waremodel.WareNo), new object[] { });
                    //int ColorBegin = sp == null ? 0 : sp.SpecCode / 10;

                    System.Data.DataTable Count = dal.QueryDataTable(string.Format("select Color from T_WareSpecList where WareNo='{0}' group by Color", waremodel.WareNo), new object[] { });
                    int ColorBegin = Count == null ? 0 : Count.Rows.Count;
                    //====== 2017-09-02 by wl 


                    int SizeBegin = -1;

                    ColorBegin++;
                    SizeList.ForEach(x =>
                    {
                        SizeBegin++;
                        sqllist.Add(string.Format(@"insert into T_WareSpecList values('{0}','{1}','{2}',{3},'{4}',0,0)", waremodel.WareNo, Com_Color.Text.Trim(), x, ColorBegin.ToString() + SizeCodeList[SizeBegin].ToString(), ImageGuid));
                        sqllist.Add(string.Format(@"insert into dbo.T_WareLog(WareNo,Color,Size,SpecCode,RecordTime,Operator,logType) values('{0}','{1}','{2}',{3},getdate(),'{4}','{5}')", waremodel.WareNo, Com_Color.Text.Trim(), x, ColorBegin.ToString() + SizeCodeList[SizeBegin].ToString(), CommonLogin.CommonUser.UserName, "新建款"));
                    });


                    FormInit.OpenDialog(this, new PhotoDialog(PhotoTime, DateTime.Now, sqllist, Com_Color.Text.Trim()), false);
                    //bool bs = dal.ExecuteTransaction(sqllist, null);
                    //if (!bs)
                    //{
                    //    CCMessageBox.Show("添加失败");

                    //    return;
                    //}
                    //else
                    //{

                    //    CCMessageBox.Show("添加成功");
                    //    //保存成功的时候生成一个GUID为图片保存做准备
                    //    ImageGuid = GuidTo16String();
                    //    tb_WareNo.Text = "";
                    //    tb_FyiCode.Text = "";
                    //    Image_Box.Source = new BitmapImage(new Uri(@"/9M.Work.WPF_Main;component/Images/nopic.jpg", UriKind.RelativeOrAbsolute));
                    //}
                    break;
                case 2: //取消
                    tb_FyiCode.Text = "";
                    tb_WareNo.Text = "";
                    tb_WareName.Text = "";
                    Com_Color.Text = "";
                    break;
                case 3:  //拍照
                    string WareNo = tb_WareNo.Text.Trim();
                    if (string.IsNullOrEmpty(WareNo))
                    {
                        CCMessageBox.Show("请填写款号在拍照");
                        return;
                    }
                    PhotoTime = DateTime.Now;
                    IsGoodsPhoto = true;
                    //拍照
                    SubmitPhoto(WareNo);
                    break;
            }
        }

        /// <summary>
        /// 下拉选择完成自动生成款号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Com_category_DropDownClosed(object sender, EventArgs e)
        {
            RandomWareNo();
        }

        #endregion

        #region Method
        /// <summary>
        /// 选中第几个单选框
        /// </summary>
        /// <param name="index"></param>
        public void GetRadio(int index)
        {
            List<RadioButton> radiolist = WPFControlsSearchHelper.GetChildObjects<RadioButton>(rad_Season, "");
            if (index > 0)
            {
                radiolist[index - 1].IsChecked = true;
            }
        }

        /// <summary>
        /// 得到单选按钮选中的值
        /// </summary>
        /// <returns></returns>
        public int GetSelectedRadio()
        {
            int Value = 0;
            List<EnumEntity> list = EnumHelper.GetEnumList(typeof(YearAttr));
            List<RadioButton> radiolist = WPFControlsSearchHelper.GetChildObjects<RadioButton>(rad_Season, "");
            string SelectedText = string.Empty;
            foreach (var item in radiolist)
            {
                if (item.IsChecked == true)
                {
                    SelectedText = item.Content.ToString();
                    break;
                }
            }
            if (!string.IsNullOrEmpty(SelectedText))
            {
                Value = list.Where(x => x.Text.Equals(SelectedText)).Single().Value;
            }
            return Value;
        }

        public static string GuidTo16String()
        {
            long i = 1;
            foreach (byte b in Guid.NewGuid().ToByteArray())
                i *= ((int)b + 1);
            return string.Format("{0:x}", i - DateTime.Now.Ticks);
        }

        public void RandomWareNo()
        {
            QualityBatchModel qb = (QualityBatchModel)Com_Batch.SelectedItem;
            if (qb == null)
            {
                CCMessageBox.Show("请选中批次");
                return;
            }
            if (string.IsNullOrEmpty(tb_Year.Text))
            {
                CCMessageBox.Show("年份不能为空");
                return;
            }
            int sea = GetSelectedRadio();
            if (sea == 0)
            {
                CCMessageBox.Show("季节不能为空");
                return;
            }
            if (string.IsNullOrEmpty(qb.BrandEn))
            {
                CCMessageBox.Show("品牌代号不能为空");
                return;
            }
            CategoryModel categorymodel = (CategoryModel)Com_category.SelectedItem;
            if (categorymodel == null)
            {
                CCMessageBox.Show("请选择分类");
                return;
            }

            string Year = string.Empty;
            string Season = string.Empty;
            string Brand = string.Empty;
            string CategoryCode = string.Empty;
            string LastCode = string.Empty;
            WareModel ware = dal.QuerySingle<WareModel>(string.Format("select top 1 * from dbo.T_WareList where Batchid = {0} order by id desc", qb.Id), new object[] { });
            //生成年份(第一位)
            string yea = tb_Year.Text.Trim();
            Year = yea[yea.Length - 1].ToString();
            //生成第二位季节
            Season = sea.ToString();
            //生成三四位的品牌
            Brand = qb.BrandEn;
            //得到分类码范围值获取最后四位的范围取值
            int Min = categorymodel.CategoryCodeMin;
            int Max = categorymodel.CategoryCodeMax;
            int start = Min * 100;
            int end = (Max + 1) * 100;
            //得到目前分类编码头的后四位
            List<string> ListGoodsList = dal.QueryList<string>(string.Format("select WareNo from dbo.T_WareList where CategoryId = {0} and  WareNo  like '{1}%' order by WareNo", categorymodel.Id, Year + Season + Brand), new object[] { }).Where(x => x.Length >= 8).Select(x => x.Substring(x.Length - 4, 4)).ToList(); ;
            //转换数据Int集合
            List<int> NumList = new List<int>();
            foreach (string str in ListGoodsList)
            {
                int outvalue = 0;
                int.TryParse(str, out outvalue);
                if (outvalue > 0)
                {
                    NumList.Add(outvalue);
                }
            }
            //获取当前值
            NumList = NumList.Where(x => x > start && x < end).OrderBy(x => x).ToList();
            //匹配四位可用数字
            int Reslut = start + 1;
            bool OutMax = true;
            for (int i = start + 1; i < end; i++)
            {
                int res = NumList.Find(x =>
                {
                    return i == x;
                });
                if (res == 0)
                {
                    OutMax = false;
                    Reslut = i;
                    break;
                }
            }
            if (OutMax)
            {
                CCMessageBox.Show("己超出设置的范围 不能生成款号");
                return;
            }
            //生成固定的四位值
            string LastWareNo = string.Empty;
            switch (Reslut.ToString().Length)
            {
                case 1:  //个位
                    LastWareNo = "000" + Reslut;
                    break;
                case 2:
                    LastWareNo = "00" + Reslut;
                    break;
                case 3:
                    LastWareNo = "0" + Reslut;
                    break;
                case 4:
                    LastWareNo = Reslut.ToString();
                    break;
            }
            tb_WareNo.Text = Year + Season + Brand + LastWareNo;
        }

        #endregion



        #endregion

        #region 看图找款
        #region Event
        //下拉类目
        private void com_querycategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CategoryModel ms = (CategoryModel)(sender as ComboBox).SelectedItem;
            //动态生成特点
            DynamicBind.BindTeDianPanel(ms, wrap_querytedie, true);
        }

        //搜索数据与图片
        int CurrentPage = 1;
        private void Btn_QueryImage(object sender, RoutedEventArgs e)
        {
            CurrentPage = 1;
            Query(ImageBox.TransverseCount * 2, CurrentPage);
        }

        private void x_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //切换到详情
            this.Dispatcher.BeginInvoke(DispatcherPriority.Background,
               (Action)(() => { TabControlIt.SelectedIndex = 2; }));
            Image imagebox = sender as Image;
            ////绑定主图
            //WareModel ms = dal.GetSingle<WareModel>(x => x.WareNo.Equals(WareNo, StringComparison.CurrentCultureIgnoreCase));
            //string filename = ShareImagePath + @"\" + ms.WareNo + @"\" + ms.ImageKey + ".jpg";
            //if (File.Exists(filename))
            //{
            //    ImageBind.BindImageBox(img_specbox, filename, false);
            //}
            StackPanel stack = WPFControlsSearchHelper.GetParentObject<StackPanel>(imagebox, "");
            string WareNo = (stack.Children[1] as Label).Content.ToString();

            lab_specwareno.Content = WareNo;
            ImageBind.BindImageBox(img_specbox, @"/9M.Work.WPF_Main;component/Images/nopic.jpg", false);
            BindSpecImagePanel(WareNo);
        }


        public void Query(int PageSize, int PageNo)
        {
            string fyicdoe = tb_fyitext.Text;
            QualityBatchModel batchmodel = (QualityBatchModel)com_querybatch.SelectedItem;
            CategoryModel categorymodel = (CategoryModel)com_querycategory.SelectedItem;
            if (batchmodel == null)
            {
                CCMessageBox.Show("请选择一个批次");
                return;
            }
            if (categorymodel == null)
            {
                CCMessageBox.Show("请选择一个类目");
                return;
            }
            //获取特点数据
            WareModel tedians = new WareModel();
            DynamicBind.GetWareDeDian(wrap_querytedie, tedians);
            string SqlWhere = string.Empty;
            if (!string.IsNullOrEmpty(tedians.XiuXing))
            {
                SqlWhere += string.Format(@"and XiuXing = '{0}'", tedians.XiuXing);
            }
            if (!string.IsNullOrEmpty(tedians.MianLiao))
            {
                SqlWhere += string.Format(@"and MianLiao = '{0}'", tedians.MianLiao);
            }
            if (!string.IsNullOrEmpty(tedians.YanSe))
            {
                SqlWhere += string.Format(@"and YanSe = '{0}'", tedians.YanSe);
            }
            if (!string.IsNullOrEmpty(tedians.MenJin))
            {
                SqlWhere += string.Format(@"and MenJin = '{0}'", tedians.MenJin);
            }
            if (!string.IsNullOrEmpty(tedians.LingXing))
            {
                SqlWhere += string.Format(@"and LingXing = '{0}'", tedians.LingXing);
            }
            if (!string.IsNullOrEmpty(tedians.QiTa))
            {
                SqlWhere += string.Format(@"and QiTa = '{0}'", tedians.QiTa);
            }
            if (!string.IsNullOrEmpty(fyicdoe))
            {
                SqlWhere += string.Format(@"or OriginalFyiCode = '{0}'", fyicdoe);
            }
            string CountSql = string.Format(@"select  count(*) as Item from dbo.T_WareList where Batchid = {0} and CategoryId = {1} {2} ", batchmodel.Id, categorymodel.Id, SqlWhere);
            int Count = dal.QueryList<QueryUniqueModel>(CountSql, new object[] { })[0].Item;
            bool HasNext = PageSize * PageNo < Count;
            Btn_Next.Visibility = HasNext ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            Btn_Up.Visibility = PageNo > 1 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            string Sql = string.Format(@"select top {3} *  from(
            select ROW_NUMBER() OVER (ORDER BY id Desc) AS RowNumber,* from dbo.T_WareList where Batchid = {0} and CategoryId = {1} {2} 
            ) as a where RowNumber>{4}*{5}", batchmodel.Id, categorymodel.Id, SqlWhere, PageSize, PageSize, PageNo - 1);
            if (!string.IsNullOrEmpty(fyicdoe))
            {
                Sql = string.Format("select * from dbo.T_WareList where OriginalFyiCode='{0}'", fyicdoe);
            }
            List<WareModel> modellist = dal.QueryList<WareModel>(Sql, new object[] { });
            DataTable dt = modellist.ConvertToDataTable<WareModel>();
            ImageBox.BindImage(dt, null, "ImageKey", "WareNo", 1.33);
            //注册事件
            List<Image> list = WPFControlsSearchHelper.GetChildObjects<Image>(ImageBox, "");
            list.ForEach(x =>
            {
                x.MouseLeftButtonDown += x_MouseLeftButtonDown;
            });
        }
        private void Btn_PageUp(object sender, RoutedEventArgs e)
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                Query(ImageBox.TransverseCount * 2, CurrentPage);
            }
        }

        private void Btn_PageDown(object sender, RoutedEventArgs e)
        {
            CurrentPage++;
            Query(ImageBox.TransverseCount * 2, CurrentPage);
        }

        //删除
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ImageBindModel model = ImageBox.ReadSelectedImage();
            if (model == null)
            {
                CCMessageBox.Show("请选中要删除的款");
                return;
            }
            string WareNo = model.TitlePath;
            List<string> sqlist = new List<string>();
            sqlist.Add(string.Format(@"delete from dbo.T_WareList where WareNo = '{0}'", WareNo));
            sqlist.Add(string.Format(@"delete from dbo.T_WareSpecList where WareNo = '{0}'", WareNo));
            bool bs = dal.ExecuteTransaction(sqlist, null);
            if (bs)
            {
                CurrentPage = 1;
                Query(ImageBox.TransverseCount * 2, CurrentPage);
            }
        }

        #endregion



        #endregion

        #region 规格详情
        private void Btn_SpecClick(object sender, RoutedEventArgs e)
        {
            if (lab_specwareno.Content == null)
            {
                CCMessageBox.Show("请选择一个款号");
                return;
            }
            int Tag = Convert.ToInt32((sender as Button).Tag);
            string WareNo = lab_specwareno.Content.ToString();
            string colorText = com_speccolorlist.Text.Trim();
            switch (Tag)
            {
                case 1: //拍照

                    if (string.IsNullOrEmpty(colorText))
                    {
                        CCMessageBox.Show("请指定一个颜色");
                        return;
                    }
                    //else
                    //{
                    //    if (colorlist.Where(x => x.Color.Equals(com_speccolorlist.Text)).Count() == 0)
                    //    {
                    //        ColorModel model = new ColorModel() { Color = com_speccolorlist.Text };
                    //        dal.Add<ColorModel>(model);
                    //        colorlist = dal.GetAll<ColorModel>();
                    //    }
                    //}
                    int speccount = dal.QueryList<WareSpecModel>(string.Format(@"select * from dbo.T_WareSpecList where WareNo = '{0}' and Color = '{1}'", WareNo, colorText), new object[] { }).Count;

                    if (speccount > 0)
                    {
                        CCMessageBox.Show("己存在该颜色");
                        return;
                    }

                    IsGoodsPhoto = false;
                    SubmitPhoto(WareNo);
                    break;
                case 2: //添加规格
                    //bool HasImage = img_specbox.Source.ToString().Contains("nopic.jpg");
                    //if (HasImage)
                    //{
                    //    CCMessageBox.Show("请先拍照后在添加规格");
                    //    return;
                    //}
                    int speccounts = dal.QueryList<WareSpecModel>(string.Format(@"select * from dbo.T_WareSpecList where WareNo = '{0}' and Color = '{1}'", WareNo, colorText), new object[] { }).Count;

                    if (speccounts > 0)
                    {
                        CCMessageBox.Show("己存在该颜色");
                        return;
                    }
                    List<string> sqllist = new List<string>();
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
                    //得到SPEC最大的ID wl 2017-08-03
                    //WareSpecModel sp = dal.QuerySingle<WareSpecModel>(string.Format("select top 1 * from T_WareSpecList where WareNo ='{0}' order by SpecCode Desc", WareNo), new object[] { });
                    //int ColorBegin = sp == null ? 0 : sp.SpecCode/10;

                    System.Data.DataTable Count = dal.QueryDataTable(string.Format("select Color from T_WareSpecList where WareNo='{0}' group by Color", WareNo), new object[] { });
                    int ColorBegin = Count == null ? 0 : Count.Rows.Count;
                    //---------------- wl 2017-08-03


                    int SizeBegin = -1;
                    ColorBegin++;

                    SizeList.ForEach(x =>
                    {
                        SizeBegin++;
                        sqllist.Add(string.Format(@"insert into T_WareSpecList values('{0}','{1}','{2}',{3},'{4}',0,0)", WareNo, com_speccolorlist.Text.Trim(), x, ColorBegin.ToString() + SizeCodeList[SizeBegin].ToString(), ImageGuid));
                        sqllist.Add(string.Format(@"insert into dbo.T_WareLog(WareNo,Color,Size,SpecCode,RecordTime,Operator,logType) values('{0}','{1}','{2}',{3},getdate(),'{4}','{5}')", WareNo, Com_Color.Text.Trim(), x, ColorBegin.ToString() + SizeCodeList[SizeBegin].ToString(), CommonLogin.CommonUser.UserName, "新建款"));
                    });

                    bool bs = dal.ExecuteTransaction(sqllist, null);
                    if (!bs)
                    {
                        CCMessageBox.Show("添加失败");
                        return;
                    }
                    else
                    {
                       DataTable dt=   dal.QueryDataTable(string.Format("select COUNT(1) as num from dbo.T_PhotographyDetail where goodsno='{0}'", WareNo));
                       if (dt.Rows.Count > 0)
                       {
                           if (Convert.ToInt32(dt.Rows[0]["num"]) > 0)
                           {
                               CCMessageBox.Show("拍照中,请快速处理","提示");
                           }
                       }
                    }

                    ImageGuid = GuidTo16String();
                    BindSpecImagePanel(WareNo);
                    ph.PrintSample(WareNo, com_speccolorlist.Text.Trim());
                    ImageBind.BindImageBox(img_specbox, @"/9M.Work.WPF_Main;component/Images/nopic.jpg", false);
                    com_speccolorlist.Text = "";
                    break;
                case 3:  //打印
                    ph.PrintGoods(new List<string>() { WareNo });
                    break;
                case 4: //删除
                    ImageBindModel ms = specimagepanel.ReadSelectedImage();
                    if (ms == null)
                    {
                        CCMessageBox.Show("请选中要删除的规格");
                        return;
                    }
                    if (CCMessageBox.Show("是否要删除规格", "删除", CCMessageBoxButton.YesNo) == CCMessageBoxResult.Yes)
                    {
                        string Color = ms.TitlePath;
                        string deletesql = string.Format(@"delete from T_WareSpecList where WareNo='{0}' and Color = '{1}'", WareNo, Color);
                        dal.ExecuteSql(deletesql);
                        BindSpecImagePanel(WareNo);
                    }
                    break;
                case 5:  //备注
                    FormInit.OpenDialog(this, new RemarkSet(WareNo), false);
                    break;
                case 6: //打印样品 
                    PrintSample(WareNo);
                    break;
                case 7: //双打 
                    ph.PrintGoods(new List<string>() { WareNo });
                    PrintSample(WareNo);
                    break;
                case 8: //修改照片
                    bool CheckImage = img_specbox.Source.ToString().Contains("nopic.jpg");
                    if (CheckImage)
                    {
                        CCMessageBox.Show("请先拍照后在添加规格");
                        return;
                    }
                    ImageBindModel mss = specimagepanel.ReadSelectedImage();
                    if (mss == null)
                    {
                        CCMessageBox.Show("请选中要删除的规格");
                        return;
                    }
                    string Colors = mss.TitlePath;
                    string updatesql = string.Format(@"update T_WareSpecList set ImageKey = '{2}' where WareNo='{0}' and Color = '{1}'", WareNo, Colors, ImageGuid);
                    dal.ExecuteSql(updatesql);
                    BindSpecImagePanel(WareNo);
                    ImageGuid = GuidTo16String();
                    break;
            }
        }

        /// <summary>
        /// 打印样品标
        /// </summary>
        /// <param name="WareNo"></param>
        public void PrintSample(string WareNo)
        {
            string ColorSql = string.Format(@"select Color from  dbo.T_WareSpecList where WareNo = '{0}'  group by Color", WareNo);
            var list = dal.QueryList<string>(ColorSql, new object[] { });
            foreach (var item in list)
            {
                ph.PrintSample(WareNo, item);
            }
        }
        private void Btn_QuerySpec(object sender, RoutedEventArgs e)
        {
            string WareNo = tb_query.Text;
            if (!string.IsNullOrEmpty(WareNo))
            {
                lab_specwareno.Content = WareNo;
                ImageBind.BindImageBox(img_specbox, @"/9M.Work.WPF_Main;component/Images/nopic.jpg", false);
                BindSpecImagePanel(WareNo);
            }
        }

        private void tb_query_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.Btn_QuerySpec(new object(), new RoutedEventArgs());
            }
        }
        #endregion

        #region  快捷键
        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            string WareNo = lab_specwareno.Content.ToString();
            ph.PrintGoods(new List<string>() { WareNo });
        }

        private void tb_fyitext_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.Btn_QueryImage(new object() { }, new RoutedEventArgs());
            }
        }


        //刷新配饰
        private void Btn_AddPeiShi(object sender, RoutedEventArgs e)
        {
            string Peishi = RadioBind.ReadSelectedContent(Stack_PeiShi);
            string WareNo = lab_specwareno.Content == null ? string.Empty : lab_specwareno.Content.ToString();
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
            //得到SPEC最大的ID
            WareSpecModel sp = dal.QuerySingle<WareSpecModel>(string.Format("select top 1 * from T_WareSpecList where WareNo ='{0}' order by SpecCode Desc", WareNo), new object[] { });

            int b = sp.SpecCode.ToString().Length == 2 ? 10 : 100;

            int ColorBegin = sp == null ? 0 : sp.SpecCode / b;
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
                //string LogSql = string.Format(@"insert into T_QualityCheck_Log values('{0}','{1}','{2}','{3}')", CommonLogin.CommonUser.UserName, WareNo, "刷新配饰", DateTime.Now);
                //dal.ExecuteSql(LogSql);
                GoodsLogModel log = new GoodsLogModel()
                {
                    GoodsNo = WareNo,
                    LogType = Convert.ToInt32(GoodsLogType.Subsec),
                    LogTime = DateTime.Now,
                    UserName = CommonLogin.CommonUser.UserName,
                    DoEvent = string.Format("新建货品;【{0}】", WareNo),
                };
                dal.Add<GoodsLogModel>(log);
            }
            //添加数据
            bool adres = dal.ExecuteTransaction(addlist, null);
            BindSpecImagePanel(WareNo);
        }
        #endregion

        //合并款号
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

        }

        private void CommandBinding_createNewTakePhoto_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Button btn = new Button();
            btn.Tag = "3";
            Button_Click(btn, null);
        }

        private void CommandBinding_specTakePhoto_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Button btn = new Button();
            btn.Tag = "1";
            Btn_SpecClick(btn, null);
        }




    }
}
