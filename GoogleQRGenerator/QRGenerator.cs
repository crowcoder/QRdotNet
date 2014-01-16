using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Web;
using System.Net;

namespace GoogleQRGenerator
{
    public class QRGenerator
    {
        private readonly string urlFormat = "https://chart.googleapis.com/chart?cht=qr&chs={0}&chl={1}{2}{3}";
        private IQrParameters _theParameters;
        
        public QRGenerator(IQrParameters theParameters)
        {
            _theParameters = theParameters;
        }

        /// <summary>
        /// Returns an Image object from the response of the call to the google api
        /// </summary>
        /// <exception cref="System.ArgumentException">Stream is invalid</exception>
        /// <returns></returns>
        public Image GetQRCode()
        {
            string theurl = string.Format(urlFormat, _theParameters.chs, _theParameters.chl,
                string.IsNullOrWhiteSpace(_theParameters.choe) ? "" : "&choe=" + _theParameters.choe,
                string.IsNullOrWhiteSpace(_theParameters.chld) ? "" : "&chld=" + _theParameters.chld);

            System.Net.HttpWebRequest qrReq = (System.Net.HttpWebRequest)WebRequest.Create(theurl);
            System.Net.HttpWebResponse qrResp = (System.Net.HttpWebResponse)qrReq.GetResponse();

            using (System.IO.Stream str = qrResp.GetResponseStream())
            {
                return Image.FromStream(str);
            }
        }
    }

    public interface IQrParameters
    {
        /// <summary>
        /// Image Size
        /// </summary>
        string chs { get; set; }

        /// <summary>
        /// The data to encode. Data can be digits (0-9), alphanumeric characters, binary bytes of data, 
        /// or Kanji. You cannot mix data types within a QR code. The data must be UTF-8 URL-encoded. 
        /// Note that URLs have a 2K maximum length, so if you want to encode more than 2K bytes (minus 
        /// the other URL characters), you will have to send your data using POST.
        /// </summary>
        string chl { get; set; }

        /// <summary>
        /// OPTIONAL parameter
        /// How to encode the data in the QR code. Here are the available values: 
        /// UTF-8 [Default]
        /// Shift_JIS
        /// ISO-8859-1
        /// </summary>
        string choe { get; set; }

        /// <summary>
        /// OPTIONAL parameter
        /// error_correction_level - QR codes support four levels of error correction to enable 
        /// recovery of missing, misread, or obscured data. Greater redundancy is achieved at 
        /// the cost of being able to store less data. See the appendix for details. Here are 
        /// the supported values: ◦L - [Default] Allows recovery of up to 7% data loss
        /// M - Allows recovery of up to 15% data loss
        /// Q - Allows recovery of up to 25% data loss
        /// H - Allows recovery of up to 30% data loss
        ///margin - The width of the white border around the data portion of the code. 
        ///This is in rows, not in pixels. (See below to learn what rows are in a QR code.) The default value is 4.
        /// </summary>
        string chld { get; set; }
    }

    public struct QrParams : IQrParameters
    {
        private string _chs;
        public string chs
        {
            get { return _chs; }
            set
            {
                //match any number of digits then a literal "x" then any number of digits
                Regex chsMatcher = new Regex(@"^\d+x\d+$");
                if (chsMatcher.IsMatch(value))
                {
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        value = HttpUtility.HtmlEncode(value);
                    }
                    else
                    {
                        throw new ArgumentException("The chl parameter must be provided");
                    }
                    _chs = value;
                }
                else
                {
                    throw new ArgumentException("The chl parameter format is invalid. It must be like <width>x<height>" +
                        " where width and height are unsigned integral values.");
                }
            }
        }

        private string _chl;
        public string chl
        {
            get
            {
                return HttpUtility.HtmlEncode(_chl);
            }
            set
            {
                _chl = value;
            }
        }

        private string _choe;
        public string choe
        {
            get
            {
                return _choe;
            }
            set
            {
                string[] validvalues = { "UTF-8", "Shift_JIS", "ISO-8859-1" };
                if (validvalues.Contains(value))
                {
                    _choe = value;
                }
                else
                {
                    throw new ArgumentException("The choe parameter must be one of these: UTF-8, Shift_JIS, ISO-8859-1");
                }
            }
        }

        private string _chld;
        public string chld
        {
            get
            {
                return _chld;
            }
            set
            {
                string[] validvalues = { "L", "M", "Q", "H" };
                if (validvalues.Contains(value))
                {
                    _chld = value;
                }
                else
                {
                    throw new ArgumentException("The chld parameter must be one of these: L, M, Q, H");
                }
            }
        }
    }
}
