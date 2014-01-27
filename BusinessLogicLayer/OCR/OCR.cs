using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Framework;
using Framework.Logging;

namespace BusinessLogicLayer.OCR
{
    public class Ocr
    {
        private ILogger _logger = IoC.GetLoggerFor<Ocr>();
        private List<Bitmap> _images;
        private Tesseract _tesseract;

        private ColorMatrix _colorMatrix = new ColorMatrix(
            new[]
                {
                    new[] {.3f, .3f, .3f, 0, 0},
                    new[] {.59f, .59f, .59f, 0, 0},
                    new[] {.11f, .11f, .11f, 0, 0},
                    new[] {0f, 0, 0, 1, 0},
                    new[] {0f, 0, 0, 0, 1}
                });

        public Ocr()
        {
            _images = new List<Bitmap>();
            _tesseract = new Tesseract();
        }

        public void AddToCollectionImage(List<Point> topLefts, List<Point> bottomRights)
        {
            if (topLefts.Count != bottomRights.Count)
            {
                _logger.WarnFormat(
                    "Collection points for image aggregation do not match. {0} TopLefts & {1} BottomRights",
                    topLefts.Count, bottomRights.Count);
                return;
            }

            List<Bitmap> singleCard = new List<Bitmap>();

            try
            {
                singleCard.AddRange(topLefts.Select((t, i) => GetScreenShot(t, bottomRights[i])));

                int width = 0;
                int height = 0;

                foreach (Bitmap bitmap in singleCard)
                {
                    width += bitmap.Width;
                    height = bitmap.Height > height ? bitmap.Height : height;
                }

                Bitmap finalImage = new Bitmap(width, height);

                using (var g = Graphics.FromImage(finalImage))
                {
                    g.Clear(Color.Black);

                    var offset = 0;
                    foreach (var image in singleCard)
                    {
                        g.DrawImage(image, new Rectangle(offset, 0, image.Width, image.Height));
                        offset += image.Width;
                    }
                }

                _images.Add(finalImage);
            }
            finally
            {
                foreach (Bitmap image in singleCard)
                {
                    image.Dispose();
                }
            }
        }

        public string ExtractTextFromScreen()
        {
            Bitmap finalImage = null;

            try
            {
                int width = 0;
                int height = 0;

                foreach (Bitmap bitmap in _images)
                {
                    width = bitmap.Width;
                    height += bitmap.Height;
                }

                finalImage = new Bitmap(width, height);

                using (var g = Graphics.FromImage(finalImage))
                {
                    g.Clear(Color.Black);

                    var offset = 0;
                    foreach (var image in _images)
                    {
                        g.DrawImage(image, new Rectangle(0, offset, image.Width, image.Height));
                        offset += image.Height;
                    }
                }

                return ExtractText(finalImage);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error extracting text for image.");
                if (finalImage != null)
                {
                    finalImage.Dispose();
                }
            }
            finally
            {
                foreach (Bitmap image in _images)
                {
                    image.Dispose();
                }

                if (finalImage != null)
                {
                    finalImage.Dispose();
                }
            }
            return string.Empty;
        }

        public string ExtractTextFromScreen(Point topLeft, Point bottomRight)
        {
            var bmpScreenshot = GetScreenShot(topLeft, bottomRight);

            try
            {
                return ExtractText(bmpScreenshot);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error extracting text from the screen.");
                return string.Empty;
            }
        }

        private Bitmap GetScreenShot(Point topLeft, Point bottomRight)
        {
            int i = 0;
            while (i++ < 5)
            {
                try
                {
                    Size rectangleSize = new Size(bottomRight.X - topLeft.X, bottomRight.Y - topLeft.Y);
                    var bmpScreenshot = new Bitmap(rectangleSize.Width, rectangleSize.Height,
                                                   PixelFormat.Format32bppArgb);
                    var gfxScreenshot = Graphics.FromImage(bmpScreenshot);
                    gfxScreenshot.CopyFromScreen(topLeft.X, topLeft.Y, 0, 0, rectangleSize,
                                                 CopyPixelOperation.SourceCopy);
                    return bmpScreenshot;
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Error from screen capture.");
                }
            }
            return null;
        }

        private string ExtractText(Image image)
        {
            var temporaryFile = Path.GetTempFileName();
            var tmpFileTif = temporaryFile.Replace(".tmp", ".tif");

            try
            {
                const double increaseAmount = 3;
                var width = (int) (image.Width*increaseAmount);
                var height = (int) (image.Height*increaseAmount);

                ImageAttributes attributes = new ImageAttributes();
                attributes.SetColorMatrix(_colorMatrix);

                Bitmap bmp = new Bitmap(width, height);
                Graphics g = Graphics.FromImage(bmp);
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                //g.DrawImage(image, 0, 0, width, height);
                g.DrawImage(image, new Rectangle(0, 0, width, height), 0, 0, image.Width, image.Height,
                            GraphicsUnit.Pixel, attributes);
                g.Dispose();

                ImageCodecInfo myImageCodecInfo = GetEncoderInfo("image/tiff");
                var myEncoder = System.Drawing.Imaging.Encoder.Compression;
                EncoderParameters myEncoderParameters = new EncoderParameters(1);

                // Save the bitmap as a TIFF file with LZW compression.
                EncoderParameter myEncoderParameter =
                    new EncoderParameter(myEncoder, (long) EncoderValue.CompressionLZW);

                myEncoderParameters.Param[0] = myEncoderParameter;
                bmp.Save(tmpFileTif, myImageCodecInfo, myEncoderParameters);

                var waitTime = Math.Max(height/2, 50);
                //_logger.Trace("Calling Extract Text on Image with wait time of " + waitTime);
                return _tesseract.ConvertImageToText(tmpFileTif, waitTime);
            }
            finally
            {
                
                try
                {
                    File.Delete(tmpFileTif);
                    File.Delete(temporaryFile);
                }
                catch (IOException ex)
                {
                    _logger.Error(ex, "Error deleting temporary files.");
                }
                catch (UnauthorizedAccessException ex)
                {
                    _logger.Error(ex, "Error deleting temporary files.");
                }

                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }

        public void AddToCollectionImage(params Square [] areas)
        {
            var topLefts = new List<Point>();
            var bottomRights = new List<Point>();

            foreach (Square square in areas)
            {
                topLefts.Add(square.TopLeft);
                bottomRights.Add(square.BottomRight);
            }

            AddToCollectionImage(topLefts, bottomRights);
        }
    }
}