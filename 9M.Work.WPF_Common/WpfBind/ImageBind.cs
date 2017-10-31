using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace _9M.Work.WPF_Common.WpfBind
{
    public class ImageBind
    {
        /// <summary>
        /// 绑定一个图片
        /// </summary>
        /// <param name="Image_Box"></param>
        /// <param name="fileName">路径</param>
        /// <param name="ByteBind">是否字节绑定（字节绑定不会占用文件）</param>
        public static void BindImageBox(Image Image_Box, string fileName ,bool ByteBind)
        {
            if (ByteBind)
            {
                byte[] buffer = System.IO.File.ReadAllBytes(fileName);
                Image_Box.Source = new ImageSourceConverter().ConvertFrom(buffer) as BitmapSource;
            }
            else
            {
                Uri uri = new Uri(fileName, UriKind.RelativeOrAbsolute);
                Image_Box.Source = new BitmapImage(uri);
            }
        }
    }
}
