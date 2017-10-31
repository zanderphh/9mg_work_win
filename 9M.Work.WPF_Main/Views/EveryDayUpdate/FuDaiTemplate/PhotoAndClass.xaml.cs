using _9M.Work.DbObject;
using _9M.Work.Model;
using _9M.Work.Utility;
using _9M.Work.WPF_Common;
using _9M.Work.WPF_Common.WpfBind;
using _9M.Work.WPF_Main.FrameWork;
using _9M.Work.WPF_Main.Views.Print;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace _9M.Work.WPF_Main.Views.EveryDayUpdate.FuDaiTemplate
{
    /// <summary>
    /// PhotoAndClass.xaml 的交互逻辑
    /// </summary>
    public partial class PhotoAndClass : UserControl
    {
        public string ImageGuid = string.Empty; //照片名字
        private string ShareImagePath = CommonLogin.RemoteDir;
        EDSDK_Help eh = new EDSDK_Help();
        BaseDAL dal = new BaseDAL();
        PrintHelper ph = new PrintHelper();
        public PhotoAndClass()
        {
            InitializeComponent();
            BindBatch();
            BindCategory();
            InitNetAndPhone();
            ImageGuid = GuidTo16String();

        }

        public static string GuidTo16String()
        {
            long i = 1;
            foreach (byte b in Guid.NewGuid().ToByteArray())
                i *= ((int)b + 1);
            return string.Format("{0:x}", i - DateTime.Now.Ticks);
        }
        /// <summary>
        /// 拍照
        /// </summary>
        /// <param name="WareNo"></param>
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
        /// <summary>
        /// 绑定分类
        /// </summary>
        public void BindCategory()
        {
            List<CategoryModel> list = dal.GetAll<CategoryModel>();
            ComboBoxBind.BindComboBox(Com_Categroy, list, "CategoryName", "Id");
            Com_Categroy.SelectedIndex = list.Count > 0 ? 0 : -1;
        }

        /// <summary>
        /// 绑定批次
        /// </summary>
        public void BindBatch()
        {
            List<FuDaiBatchModel> list = dal.GetAll<FuDaiBatchModel>(new OrderModelField[] { new OrderModelField() { IsDesc = true, PropertyName = "CreateTime" } });
            ComboBoxBind.BindComboBox(com_batch, list, "BatchName", "ID");
            com_batch.SelectedIndex = list.Count > 0 ? 0 : -1;
        }
        /// <summary>
        /// 链接磁盘与相机
        /// </summary>
        public void InitNetAndPhone()
        {
            bool status = false;
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

        /// <summary>
        /// 匹配款号
        /// </summary>
        /// <returns></returns>
        public string FindGoodsALL(out string OutGoodsNo, out string OutSpecCode)
        {
            string GoodsNo = string.Empty;
            string FlagCode = string.Empty;
            string SizeCode = string.Empty;
            var RaidoList = WPFControlsSearchHelper.GetChildObjects<RadioButton>(Radio_Size, "");
            int Index = RaidoList.FindIndex(x => x.IsChecked == true);
            if (Index < 0)
            {
                CCMessageBox.Show("请选中尺码");
            }

            var Max = dal.QuerySingle<FuDaiGoodsModel>("select Top 1 * from T_FuDaiGoods order by Goodsno desc", new object[] { });
            if (Max == null) //如果是第一款
            {
                GoodsNo = "FD000001";
                FlagCode = "1";
            }
            else
            {
                //List<FuDaiGoodsModel> list = dal.GetAll<FuDaiGoodsModel>();
                //得到当前最大的款号
                string MaxGoodsNo = Max.GoodsNo;
                int MaxCode = dal.GetList<FuDaiGoodsModel>(x => x.GoodsNo == MaxGoodsNo).Max(x => Convert.ToInt32(x.SpecCode.Remove(x.SpecCode.Length - 1)));
                if (MaxCode < 33) //
                {
                    GoodsNo = MaxGoodsNo;
                    FlagCode = (MaxCode + 1).ToString();
                }
                else
                {
                    string Flag = (Convert.ToInt64(Regex.Replace(MaxGoodsNo, @"[A-Z]", "")) + 1).ToString();
                    string Suffix = string.Empty;
                    for (int i = 0; i < 6 - Flag.Length; i++)
                    {
                        Suffix += "0";
                    }
                    GoodsNo = "FD" + Suffix + Flag;
                    FlagCode = "1";
                }
            }
            SizeCode = (Index + 1).ToString();
            OutGoodsNo = GoodsNo;
            OutSpecCode = FlagCode + SizeCode;
            return GoodsNo + FlagCode + SizeCode;
        }

        private void eh_ImageDownloaded(object sender, TakePictureCompleteEventArgs e)
        {
            //字节绑定不会占用图片。可以删除
            string filename = System.IO.Path.Combine(EDSDK_Help.SaveDirecotryPath, EDSDK_Help.SavePhotoName);
            if (File.Exists(filename))
            {
                byte[] buffer = System.IO.File.ReadAllBytes(filename);
                Image_Box.Source = new ImageSourceConverter().ConvertFrom(buffer) as BitmapSource;
            }
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (com_batch.SelectedItem==null)
            {
                CCMessageBox.Show("请选择批次");
                return;
            }
            int Tag = Convert.ToInt32((sender as Button).Tag);
            string GoodsNo = string.Empty;
            string SpecCode = string.Empty;
            string GoodsNoAll = FindGoodsALL(out GoodsNo, out SpecCode);
            switch (Tag)
            {
                case 0: //拍照预览
                    SubmitPhoto(GoodsNo);
                    break;
                case 1: //确定新建
                    var ClassList = WPFControlsSearchHelper.GetChildObjects<RadioButton>(Radio_Class, "");
                    RadioButton check = ClassList.Where(x => x.IsChecked == true).FirstOrDefault();
                    if (check == null)
                    {
                        CCMessageBox.Show("请选中级别");
                        return;
                    }
                    if (string.IsNullOrEmpty(Com_Categroy.Text))
                    {
                        CCMessageBox.Show("请选择分类");
                        return;
                    }

                    RadioButton ra = WPFControlsSearchHelper.GetChildObjects<RadioButton>(Radio_Size, "").Where(x => x.IsChecked == true).FirstOrDefault();
                    if (ra==null)
                    {
                        CCMessageBox.Show("请选中尺码");
                        return;
                    }
                    bool Class = check.Content.ToString().Contains("冬");
                    FuDaiBatchModel fb = (FuDaiBatchModel)com_batch.SelectedItem;
                    FuDaiGoodsModel model = new FuDaiGoodsModel()
                    {
                        BatchID = fb.ID,
                        Brand = fb.Brand,
                        CategoryName = Com_Categroy.Text,
                        Class = Class ? "冬" : "春夏秋",
                        GoodsALL = GoodsNoAll,
                        GoodsNo = GoodsNo,
                        ImageUrl = ImageGuid,
                        IsSell = false,
                        Price = Class ? fb.PriceMax : fb.PriceMin,
                        SellMore = 0,
                        SpecCode = SpecCode,
                        Size = ra.Content.ToString(),
                        CreateTime = DateTime.Now,
                    };
                    bool b = dal.Add<FuDaiGoodsModel>(model);
                    if (b)
                    {
                       
                        ph.PrintLabel(new List<string>() { model.GoodsNo + model.SpecCode });
                        ImageGuid = GuidTo16String();
                        check.IsChecked = false;
                        ra.IsChecked = false;
                    }
                    CCMessageBox.Show(b ? "成功" : "失败");
                    break;
            }
        }
    }
}
