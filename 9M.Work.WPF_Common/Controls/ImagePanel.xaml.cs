using System;
using System.Collections;
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

namespace _9M.Work.WPF_Common.Controls
{
    /// <summary>
    /// ImagePanel.xaml 的交互逻辑
    /// </summary>
    public partial class ImagePanel : UserControl
    {
        /// <summary>
        /// 横向数量
        /// </summary>
        public int TransverseCount { get; set; }
        /// <summary>
        /// 纵向数量
        /// </summary>
        public int UprightCount { get; set; }
        /// <summary>
        /// 每张图片的横向间隔
        /// </summary>
        public double MarginRow { get; set; }

        /// <summary>
        /// 列之间的间距
        /// </summary>
        public double MarginColumn { get; set; }
        /// <summary>
        /// 属标悬停在图片上的样式
        /// </summary>
        public Cursor ImageCursor { get; set; }

        /// <summary>
        /// 是否启用选中事件
        /// </summary>
        public bool SelectedEvent { get; set; }

        public double TitleFontSize { get; set; }
        /// <summary>
        /// 图片是否在上边
        /// </summary>
        public bool ImageUp { get; set; }

        /// <summary>
        /// 图片放大功能
        /// </summary>
        public bool BigImage { get; set; }
        public ImagePanel()
        {
            InitializeComponent();
        }

        public void ClearImage()
        {
            ImageBox.Children.Clear();
        }
        /// <summary>
        /// 绑定图片控件
        /// </summary>
        /// <param name="dt"></param>
        ///  <param name="ChildDirFind">为空就在主目录上找图，不为空就在子目录下找图</param>
        /// <param name="ImagePath"></param>
        /// <param name="TitlePath"></param>
        public void BindImage(DataTable dt, string ChildDirFind, string ImagePath, string TitlePath, double WidthrateHeiht)
        {

            ImageBox.Children.Clear();
            string SharePath = CommonLogin.RemoteDir;
            //转换TABLE
            List<ImageBindModel> list = ConverttoList(dt, ImagePath, TitlePath);
            //计算所需的行数
            int RowCount = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(list.Count) / TransverseCount));

            //记得每一个Panel的宽度
            double width = (this.Width - (MarginRow * (TransverseCount + 1))) / TransverseCount;
            //计算Margin的值

            // double height = this.Height;

            //如果设置了图片放大功能
            if (BigImage)
            {
                Panel_BigImage.Width = width * 3;
                Panel_BigImage.Height = width * WidthrateHeiht * 3;
                //  Panel_BigImage.MouseDown += Panel_BigImage_MouseDown;
                Panel_BigImage.MouseRightButtonDown += Panel_BigImage_MouseRightButtonDown;
            }
            //绑定控件
            for (int i = 0; i < RowCount; i++)
            {
                StackPanel stack = new StackPanel() { Orientation = Orientation.Horizontal };
                if (i > 0)
                {
                    stack.Margin = new Thickness(0, MarginColumn, 0, 0);
                }
                for (int j = i * TransverseCount; j < (i + 1) * TransverseCount; j++)
                {
                    if (j == list.Count)
                    {
                        break;
                    }
                    Border b = new Border() { BorderThickness = new Thickness(2), BorderBrush = new SolidColorBrush(Colors.LightGray), CornerRadius = new CornerRadius(2) };
                    b.Margin = new Thickness(MarginRow, 0, 0, 0);
                    StackPanel child = new StackPanel() { Orientation = Orientation.Vertical };
                    Image image = new Image() { Width = width, Height = width * WidthrateHeiht };
                    image.Margin = new Thickness(0, 10, 0, 0);
                    image.Cursor = ImageCursor;
                    //如果设置了图片放大功能
                    if (BigImage)
                    {
                        image.MouseRightButtonDown += image_MouseRightButtonDown;
                    }
                    //是否添加选中事件
                    if (SelectedEvent)
                    {
                        //image.MouseLeftButtonDown += image_MouseLeftButtonDown;
                        b.MouseLeftButtonDown += b_MouseLeftButtonDown;
                    }
                    //是否添加图片放大功能

                    string filepath;
                    if (string.IsNullOrEmpty(ChildDirFind))
                    {
                        filepath = SharePath + @"\" + list[j].TitlePath + @"\" + list[j].ImagePath + ".jpg";
                    }
                    else
                    {
                        filepath = SharePath + @"\" + ChildDirFind + @"\" + list[j].ImagePath + ".jpg";
                    }
                    if (File.Exists(filepath))
                    {
                        image.Source = new BitmapImage(new Uri(filepath, UriKind.RelativeOrAbsolute));
                    }
                    Label label = new Label() { FontSize = TitleFontSize, FontWeight = FontWeights.Bold, HorizontalAlignment = System.Windows.HorizontalAlignment.Center, Content = list[j].TitlePath };
                    //child.Margin = new Thickness(MarginRow, 0, 0, 0);

                    if (ImageUp == false)
                    {
                        child.Children.Add(label);
                        child.Children.Add(image);

                    }
                    else
                    {
                        child.Children.Add(image);
                        child.Children.Add(label);

                    }
                    b.Child = child;
                    stack.Children.Add(b);
                }
                ImageBox.Children.Add(stack);

    
            }
        }

        private void b_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Border bs = (sender as Border);
            Image image = WPFControlsSearchHelper.GetChildObject<Image>(bs, "");
            //Image image = (sender as Border);
            //如果是选中状态。在次点就取消选中
            Border activeBorder = (image.Parent as StackPanel).Parent as Border;
            if (activeBorder.Background != null)
            {
                activeBorder.BorderBrush = new SolidColorBrush(Colors.LightGray);
                activeBorder.Background = null;
            }
            else //如果不是就选中他(并取消其他的选中)
            {
                List<Image> imagelist = WPFControlsSearchHelper.GetChildObjects<Image>(ImageBox, "");

                //#FFD3D3D3    #FF0000

                foreach (Image i in imagelist)
                {
                    Border border = (i.Parent as StackPanel).Parent as Border;
                    if (i.Equals(image))
                    {
                        SolidColorBrush colorbrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF5552"));
                        // #FF5552
                        border.Background = colorbrush;
                        border.BorderBrush = colorbrush;
                    }
                    else
                    {
                        if (border.Background != null)
                        {
                            border.Background = null;
                            border.BorderBrush = new SolidColorBrush(Colors.LightGray);
                        }
                    }
                }
            }
        }

        private void image_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Image image = sender as Image;
            if (e.ClickCount == 2)
            {
                Panel_BigImage.Background = new ImageBrush() { ImageSource = new BitmapImage(new Uri(image.Source.ToString(), UriKind.RelativeOrAbsolute)) };
                Panel_BigImage.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void Panel_BigImage_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                Panel_BigImage.Visibility = System.Windows.Visibility.Collapsed;
            }
        }


        //选中图片
        private void image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Image image = sender as Image;
            //如果是选中状态。在次点就取消选中
            Border activeBorder = (image.Parent as StackPanel).Parent as Border;
            if (activeBorder.Background != null)
            {
                activeBorder.BorderBrush = new SolidColorBrush(Colors.LightGray);
                activeBorder.Background = null;
            }
            else //如果不是就选中他(并取消其他的选中)
            {
                List<Image> imagelist = WPFControlsSearchHelper.GetChildObjects<Image>(ImageBox, "");

                //#FFD3D3D3    #FF0000

                foreach (Image i in imagelist)
                {
                    Border border = (i.Parent as StackPanel).Parent as Border;
                    if (i.Equals(image))
                    {
                        SolidColorBrush colorbrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF5552"));
                        // #FF5552
                        border.Background = colorbrush;
                        border.BorderBrush = colorbrush;
                    }
                    else
                    {
                        if (border.Background != null)
                        {
                            border.Background = null;
                            border.BorderBrush = new SolidColorBrush(Colors.LightGray);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 得到选中的图片
        /// </summary>
        /// <returns></returns>
        public ImageBindModel ReadSelectedImage()
        {
            ImageBindModel model = null;
            List<Image> imagelist = WPFControlsSearchHelper.GetChildObjects<Image>(ImageBox, "");
            foreach (Image i in imagelist)
            {
                StackPanel stack = i.Parent as StackPanel;
                Border border = stack.Parent as Border;

                if (border.Background != null)
                {

                    Image img = WPFControlsSearchHelper.GetChildObject<Image>(stack, "");
                    Label label = WPFControlsSearchHelper.GetChildObject<Label>(stack, "");
                    model = new ImageBindModel();
                    if ((System.Windows.Media.Imaging.BitmapImage)img.Source != null)
                    {
                        Uri uri = ((System.Windows.Media.Imaging.BitmapImage)img.Source).UriSource;
                        model.ImagePath = uri.LocalPath;
                    }
                    else
                    {
                        model.ImagePath = string.Empty;
                    }

                    model.TitlePath = label.Content.ToString();

                    break;
                }
            }
            return model;
        }

        public void ClearSelect()
        {
            List<Image> imagelist = WPFControlsSearchHelper.GetChildObjects<Image>(ImageBox, "");
            foreach (Image i in imagelist)
            {
                StackPanel stack = i.Parent as StackPanel;
                Border border = stack.Parent as Border;
                border.Background = null;
                border.BorderBrush = null;
            }
        }

        public List<ImageBindModel> ConverttoList(DataTable dt, string ImagePath, string TitlePath)
        {
            List<ImageBindModel> list = new List<ImageBindModel>();
            foreach (DataRow dr in dt.Rows)
            {
                ImageBindModel bm = new ImageBindModel() { ImagePath = dr[ImagePath].ToString(), TitlePath = dr[TitlePath].ToString() };
                list.Add(bm);
            }
            return list;
        }
    }

    public class ImageBindModel
    {
        public string TitlePath { get; set; }
        public string ImagePath { get; set; }
    }
}
