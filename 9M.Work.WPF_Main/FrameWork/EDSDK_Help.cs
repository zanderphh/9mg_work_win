using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;


public class EDSDK_Help
{


    public static bool IsSDKLoaded = false;

    /// <summary>
    /// 相机列表(默认Zero)
    /// </summary>
    public static IntPtr CameraList = IntPtr.Zero;

    /// <summary>
    /// 相机对象(默认Zero)
    /// </summary>
    public static IntPtr Camera = IntPtr.Zero;

    /// <summary>
    /// 错误返回对象(默认0)
    /// </summary>
    public static uint err = 0;

    /// <summary>
    /// 保存目录
    /// </summary>
    public static string SaveDirecotryPath { get; set; }

    /// <summary>
    /// 照片的文件名字
    /// </summary>
    public static string SavePhotoName { get; set; }

    /// <summary>
    /// 每拍一张就会触发一次
    /// </summary>
    public event EDSDK.EdsObjectEventHandler SDKObjectEvent;


    public delegate void TakePictureCompleteHandle(Object sender, TakePictureCompleteEventArgs e);
    public event TakePictureCompleteHandle PhotoImageDownloaded;

    public EDSDK_Help()
    {
        err = EDSDK.EdsInitializeSDK();
        if (err.Equals(0)) IsSDKLoaded = true;
        SDKObjectEvent += new EDSDK.EdsObjectEventHandler(Camera_SDKObjectEvent);
    }


    /// <summary>
    /// 拍摄时触发的处理
    /// </summary>
    /// <param name="inEvent"></param>
    /// <param name="inRef"></param>
    /// <param name="inContext"></param>
    /// <returns></returns>
    private uint Camera_SDKObjectEvent(uint inEvent, IntPtr inRef, IntPtr inContext)
    {
        if (inEvent == EDSDK.ObjectEvent_DirItemRequestTransfer)
            DownloadImage(inRef, SaveDirecotryPath, SavePhotoName);

        return EDSDK.EDS_ERR_OK;
    }

    /// <summary>
    /// 初始化加载相机
    /// </summary>
    /// <returns></returns>
    public KeyValuePair<int, string> InitLoadCamera()
    {
        KeyValuePair<int, string> errKVP = new KeyValuePair<int, string>(0, "相机已连接");

        try
        {
            int count = 0;

            if (err.Equals(0))
            {
                err = EDSDK.EdsGetCameraList(out CameraList);

                if (err.Equals(0))
                {
                    err = EDSDK.EdsGetChildCount(CameraList, out count);

                    if (count.Equals(0))
                    {
                        errKVP = new KeyValuePair<int, string>(1, "请检查相机是否已与电脑连接！确保相机和电脑连接并开关设置ON后再启动本应用程序");
                    }
                    else
                    {
                        err = EDSDK.EdsGetChildAtIndex(CameraList, 0, out Camera);

                        if (err.Equals(0))
                        {
                            EDSDK.EdsDeviceInfo deviceInfo;
                            err = EDSDK.EdsGetDeviceInfo(Camera, out deviceInfo);

                            if (err.Equals(0) && Camera == null)
                            {
                                errKVP = new KeyValuePair<int, string>(2, "未能加载到相机信息！确保相机和电脑连接并开关设置ON后再启动本应用程序");
                            }
                            else
                            {
                                err = EDSDK.EdsOpenSession(Camera);

                                if (err.Equals(0))
                                {
                                    //设置相机事件
                                    EDSDK.EdsSetObjectEventHandler(Camera, EDSDK.ObjectEvent_All, SDKObjectEvent, Camera);
                                    //给相机的属性设置值
                                    SetSetting(EDSDK.PropID_SaveTo, (uint)EDSDK.EdsSaveTo.Host);
                                    SetSetting(EDSDK.PropID_ImageQuality, (uint)EDSDK.ImageQuality.EdsImageQuality_SJN);
                                }
                                else
                                {
                                    errKVP = new KeyValuePair<int, string>(2, "未能打开相机，打开失败");
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                errKVP = new KeyValuePair<int, string>(2, "相机SDK未加载成功,请关闭当前程序！确保相机和电脑连接并开关设置ON后再启动本应用程序");
            }
        }
        catch (Exception exp) { errKVP = new KeyValuePair<int, string>(99, exp.Message.ToString()); }

        return errKVP;
    }

    /// <summary>
    /// 拍照操作
    /// </summary>
    public KeyValuePair<int, string> Camera_TakePicture()
    {
        KeyValuePair<int, string> kvp = new KeyValuePair<int, string>(0, "拍照成功！");

        try
        {
            if (Camera != IntPtr.Zero)
            {
                err = EDSDK.EdsSendCommand(Camera, EDSDK.CameraCommand_TakePicture, 0);
                if (err != 0)
                {
                    kvp = new KeyValuePair<int, string>(1, "重启相机后再启动本应用程序！");
                }
            }
            else
            {
                kvp = new KeyValuePair<int, string>(1, "请检查相机是否已与电脑连接！确保相机和电脑连接并开关设置ON后再启动本应用程序");
            }
        }
        catch
        {
            kvp = new KeyValuePair<int, string>(99, "请检查相机是否已与电脑连接！确保相机和电脑连接并开关设置ON后再启动本应用程序");
        }

        return kvp;
    }

    /// <summary>
    /// 下载照片到指定目录
    /// </summary>
    /// <param name="ObjectPointer"></param>
    /// <param name="directory"></param>
    public void DownloadImage(IntPtr ObjectPointer, string directory, string photoName)
    {
        EDSDK.EdsDirectoryItemInfo dirInfo;
        IntPtr streamRef;

        err = EDSDK.EdsGetDirectoryItemInfo(ObjectPointer, out dirInfo);
        string CurrentPhoto = Path.Combine(directory, photoName);

        err = EDSDK.EdsCreateFileStream(CurrentPhoto,
        EDSDK.EdsFileCreateDisposition.CreateAlways, EDSDK.EdsAccess.ReadWrite, out streamRef);
        uint blockSize = 1024 * 1024;
        uint remainingBytes = dirInfo.Size;

        do
        {
            if (remainingBytes < blockSize) { blockSize = (uint)(remainingBytes / 512) * 512; }
            remainingBytes -= blockSize;
            err = EDSDK.EdsDownload(ObjectPointer, blockSize, streamRef);
        } while (remainingBytes > 512);

        err = EDSDK.EdsDownload(ObjectPointer, remainingBytes, streamRef);
        err = EDSDK.EdsDownloadComplete(ObjectPointer);
        err = EDSDK.EdsRelease(ObjectPointer);
        err = EDSDK.EdsRelease(streamRef);

        if (PhotoImageDownloaded != null)
        {
            int Width = 450;
            int Height = 600;
            //等比缩放
            Bitmap bit = GetFileThumb(System.IO.Path.Combine(EDSDK_Help.SaveDirecotryPath, EDSDK_Help.SavePhotoName));
            Image img = ImageHelp.MakeThumbnail(bit, System.IO.Path.Combine(EDSDK_Help.SaveDirecotryPath, EDSDK_Help.SavePhotoName), Width, Height, "W", true);

            ////按指定像素裁剪
            Image sourceImg = Bitmap.FromFile(System.IO.Path.Combine(EDSDK_Help.SaveDirecotryPath, EDSDK_Help.SavePhotoName));
            Bitmap bit2 = new Bitmap(sourceImg);
            sourceImg.Dispose();

            //Image rtnImage = ImageHelp.MakeThumbnail(bit2, System.IO.Path.Combine(EDSDK_Help.SaveDirecotryPath, EDSDK_Help.SavePhotoName), 900, 1200, "CUT", false);
            Image rtnImage = ImageHelp.CutImage(bit2, System.IO.Path.Combine(EDSDK_Help.SaveDirecotryPath, EDSDK_Help.SavePhotoName), Width, Height);

            PhotoImageDownloaded(null, new TakePictureCompleteEventArgs(rtnImage));
        }

    }
    /// <summary>  
    /// 为给定的属性标识设置值 
    /// </summary>  
    /// <param name="PropID">属性ID</param>  
    /// <param name="Value">要设置的值</param>  
    public bool SetSetting(uint PropID, uint Value)
    {
        bool IsSetting = false;

        if (Camera != IntPtr.Zero)
        {
            int propsize;
            EDSDK.EdsDataType proptype;
            //获得此属性的大小
            err = EDSDK.EdsGetPropertySize(Camera, PropID, 0, out proptype, out propsize);
            //设置属性 
            err = EDSDK.EdsSetPropertyData(Camera, PropID, 0, propsize, Value);
            IsSetting = true;
        }

        return IsSetting;
    }


    /// <summary>
    /// 获取图片缩略图
    /// </summary>
    /// <param name="filepath"></param>
    /// <returns></returns>
    public Bitmap GetFileThumb(string filepath)
    {
        IntPtr stream;
        err = EDSDK.EdsCreateFileStream(filepath, EDSDK.EdsFileCreateDisposition.OpenExisting, EDSDK.EdsAccess.Read, out stream);
        return GetImage(stream, EDSDK.EdsImageSource.FullView);
    }

    /// <summary>
    /// 返回图片对象
    /// </summary>
    /// <param name="img_stream"></param>
    /// <param name="imageSource"></param>
    /// <returns></returns>
    private Bitmap GetImage(IntPtr img_stream, EDSDK.EdsImageSource imageSource)
    {
        IntPtr stream = IntPtr.Zero;
        IntPtr img_ref = IntPtr.Zero;
        IntPtr streamPointer = IntPtr.Zero;
        EDSDK.EdsImageInfo imageInfo;

        try
        {
            err = EDSDK.EdsCreateImageRef(img_stream, out img_ref);
            err = EDSDK.EdsGetImageInfo(img_ref, imageSource, out imageInfo);

            EDSDK.EdsSize outputSize = new EDSDK.EdsSize();
            outputSize.width = imageInfo.EffectiveRect.width;
            outputSize.height = imageInfo.EffectiveRect.height;
            int datalength = outputSize.height * outputSize.width * 3;
            byte[] buffer = new byte[datalength];
            err = EDSDK.EdsCreateMemoryStreamFromPointer(buffer, (uint)datalength, out stream);
            err = EDSDK.EdsGetImage(img_ref, imageSource, EDSDK.EdsTargetImageType.RGB, imageInfo.EffectiveRect, outputSize, stream);
            Bitmap bmp = new Bitmap(outputSize.width, outputSize.height, PixelFormat.Format24bppRgb);
            unsafe
            {
                BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, bmp.PixelFormat);

                byte* outPix = (byte*)data.Scan0;
                fixed (byte* inPix = buffer)
                {
                    for (int i = 0; i < datalength; i += 3)
                    {
                        outPix[i] = inPix[i + 2];
                        outPix[i + 1] = inPix[i + 1];
                        outPix[i + 2] = inPix[i];
                    }
                }
                bmp.UnlockBits(data);
            }

            return bmp;
        }
        finally
        {
            if (img_stream != IntPtr.Zero) EDSDK.EdsRelease(img_stream);
            if (img_ref != IntPtr.Zero) EDSDK.EdsRelease(img_ref);
            if (stream != IntPtr.Zero) EDSDK.EdsRelease(stream);
        }
    }

}

public class TakePictureCompleteEventArgs : EventArgs
{
    private Image photo;
    public TakePictureCompleteEventArgs(Image photo)
    {
        this.photo = photo;
    }
    public Image Photo
    {
        get { return photo; }
    }
}

public class ImageHelp
{
    /// <summary>
    /// 保存指定图片
    /// </summary>
    /// <param name="ibitmap"></param>
    /// <param name="thumbnailPath"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="mode"></param>
    /// <returns></returns>
    public static Image MakeThumbnail(Bitmap ibitmap, string thumbnailPath, int width, int height, string mode, bool IsRotate)
    {
        if (IsRotate)
        {
            ibitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
        }

        System.Drawing.Image originalImage = ibitmap;

        System.Drawing.Imaging.ImageFormat thisFormat = originalImage.RawFormat;

        int towidth = width;
        int toheight = height;

        int x = 0;
        int y = 0;
        int ow = originalImage.Width;
        int oh = originalImage.Height;

        if (ow >= width)
        {
            switch (mode)
            {
                case "HW":
                    break;
                case "W": //指定宽高比例来缩放
                    toheight = originalImage.Height * width / originalImage.Width;
                    break;
                case "H": //指定高宽比例来缩放
                    towidth = originalImage.Width * height / originalImage.Height;
                    break;
                case "CUT": //指定宽高裁剪
                    if ((double)originalImage.Width / (double)originalImage.Height > (double)towidth / (double)toheight)
                    {
                        oh = originalImage.Height;
                        ow = originalImage.Height * towidth / toheight;
                        y = 0;
                        x = (originalImage.Width - ow) / 2;
                    }
                    else
                    {
                        ow = originalImage.Width;
                        oh = originalImage.Width * height / towidth;
                        x = 0;
                        y = (originalImage.Height - oh) / 2;
                    }
                    break;
                default:
                    break;
            }
        }
        else
        {
            towidth = ow;
            toheight = oh;
        }

        //新建一个bmp的图片
        System.Drawing.Image bitmap = new Bitmap(towidth, toheight);
        //新建一个画板
        System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap);



        #region 10.7 新算法
        // 设置画布的描绘质量
        //g.CompositingQuality = CompositingQuality.HighQuality;
        //g.SmoothingMode = SmoothingMode.HighQuality;
        //g.InterpolationMode = InterpolationMode.HighQualityBicubic;

        g.DrawImage(originalImage, new Rectangle(0, 0, towidth, toheight),
            0, 0, originalImage.Width, originalImage.Height, GraphicsUnit.Pixel);
        g.Dispose();

        // 以下代码为保存图片时，设置压缩质量
        EncoderParameters encoderParams = new EncoderParameters();
        long[] quality = new long[1];
        quality[0] = 60;

        EncoderParameter encoderParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
        encoderParams.Param[0] = encoderParam;

        //获得包含有关内置图像编码解码器的信息的ImageCodecInfo 对象。
        ImageCodecInfo[] arrayICI = ImageCodecInfo.GetImageEncoders();
        ImageCodecInfo jpegICI = null;
        for (int n = 0; n < arrayICI.Length; n++)
        {
            if (arrayICI[n].FormatDescription.Equals("JPEG"))
            {
                jpegICI = arrayICI[n];//设置JPEG编码
                break;
            }
        }

        if (jpegICI != null)
        {
            bitmap.Save(thumbnailPath, jpegICI, encoderParams);
        }
        else
        {
            bitmap.Save(thumbnailPath, thisFormat);
        }


        originalImage.Dispose();
        bitmap.Dispose();

        return bitmap;

        #endregion



    }

    /// <summary>
    /// 裁剪图片
    /// </summary>
    /// <param name="ibitmap"></param>
    /// <param name="thumbnailPath"></param>
    /// <param name="iwith"></param>
    /// <param name="iheight"></param>
    /// <returns></returns>
    public static Image CutImage(Bitmap ibitmap, string thumbnailPath, int iwith, int iheight)
    {
        Image originalImage = ibitmap;
        int ow = originalImage.Width;
        int oh = originalImage.Height;

        Rectangle cropArea = new Rectangle();

        int x = 0;
        int y = (oh - iheight) / 2;

        int width = iwith;
        int height = iheight;

        cropArea.X = x;
        cropArea.Y = y;
        cropArea.Width = width;
        cropArea.Height = height;

        Bitmap bmpImage = new Bitmap(originalImage);
        Bitmap bmpCrop = bmpImage.Clone(cropArea, bmpImage.PixelFormat);

        Image img = bmpCrop;
        img.Save(thumbnailPath);
        originalImage.Dispose();
        bmpImage.Dispose();
        bmpCrop.Dispose();
        img.Dispose();

        return originalImage;

    }
}

