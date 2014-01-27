using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Framework;
using Framework.Logging;

namespace BusinessLogicLayer.ImageDownloader
{
    public class DownloadImage
    {
        private ILogger _logger = IoC.GetLoggerFor<DownloadImage>();
        private readonly string _imageUrl;
        private Bitmap _bitmap;

        public DownloadImage(string imageUrl, string filename)
        {
            this._imageUrl = imageUrl;
            this._filename = filename;
        }

        public void Download()
        {
            if (!File.Exists(FileName))
            {
                try
                {
                    WebClient client = new WebClient();
                    _logger.Trace("Downloading " + _imageUrl);
                    Stream stream = client.OpenRead(_imageUrl);
                    if (stream != null)
                    {
                        _bitmap = new Bitmap(stream);
                        stream.Flush();
                        stream.Close();
                        SaveImage(ImageFormat.Jpeg);
                    }
                    else
                    {
                        _logger.Warn("No client stream for site " + _imageUrl);
                    }
                }
                catch (Exception e)
                {
                    _logger.Error(e.Message);
                }
            }
        }

        public Bitmap GetImage()
        {
            return _bitmap;
        }

        private readonly string _filename;
        public string FileName
        {
            get { return @"D:\Projects\Bot\Images\" + _filename; }
        }

        public void SaveImage(ImageFormat format)
        {
            if (_bitmap != null)
            {
                _bitmap.Save(FileName, format);
            }
        }
    }
}
