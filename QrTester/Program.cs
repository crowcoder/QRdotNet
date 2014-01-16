using System.Drawing;

namespace QrTester
{
    class Program
    {
        static void Main(string[] args)
        {
            GoogleQRGenerator.QrParams qrParams = new GoogleQRGenerator.QrParams();
            qrParams.chs = "400x400";
            qrParams.chl = "Hello spaces World";
            qrParams.chld = "H";
            qrParams.choe = "UTF-8";
            GoogleQRGenerator.QRGenerator genny = new GoogleQRGenerator.QRGenerator(qrParams);
            Bitmap qr = (Bitmap)genny.GetQRCode();
            qr.Save(@"c:\deleteme\myQR.png", System.Drawing.Imaging.ImageFormat.Png);

        }
    }
}
