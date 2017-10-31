using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using ThoughtWorks.QRCode.Codec;

namespace _9M.Work.Utility
{
    public class QrCodeHelper
    {
 
        /// <summary>
        /// 生成二维码
        /// </summary>
        /// <param name="QRCodeScale"></param>
        /// <param name="strTxt"></param>
        /// <returns></returns>
        public static Bitmap GenerateQrCode(int QRCodeScale, String strTxt)
        {
            Bitmap bmp = null;
            try
            {
                QRCodeEncoder qrCodeEncoder = new QRCodeEncoder
                {
                    QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE,
                    QRCodeScale = QRCodeScale,
                    QRCodeVersion = 3,
                    QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.L
                };
                bmp = qrCodeEncoder.Encode(strTxt);
            }
            catch (Exception)
            {
                //MessageBox.Show("Invalid version !");
            }
            return bmp;
        }


    }
}
