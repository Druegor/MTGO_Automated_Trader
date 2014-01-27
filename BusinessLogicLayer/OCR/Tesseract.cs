using System;
using System.Diagnostics;
using System.IO;
using BusinessLogicLayer.Interfaces;
using Framework;
using Framework.Logging;

namespace BusinessLogicLayer.OCR
{
    public interface IOcr
    {
        string ConvertImageToText(string imageLocation, int waitTime = 0);
    }

    public class Tesseract : IOcr
    {
        private ILogger _logger = IoC.GetLoggerFor<Tesseract>();
        private string _tesseractExe;
        private string _outputLocation;
        private string _outputFile;
        private string _path;

        public Tesseract()
        {
            _path = IoC.Resolve<IApplicationSettings>().TesseractLocation;
            string tesseractOcr = "Tesseract-Ocr";
            _outputFile = Path.Combine(_path, tesseractOcr, "output.txt");
            _outputLocation = " \"" + Path.Combine(_path, tesseractOcr, "output") + "\"";
            _tesseractExe = "\"" + Path.Combine(_path, tesseractOcr, "tesseract.exe") + "\"";
        }

        public string ConvertImageToText(string imageLocation, int waitTime = 0)
        {
            Process process = null;

            ProcessStartInfo processStartInfo =
                new ProcessStartInfo
                    {
                        FileName = _tesseractExe,
                        Arguments = "\"" + imageLocation + "\"" + _outputLocation,
                        WindowStyle = ProcessWindowStyle.Hidden,
                        CreateNoWindow = true,
                        UseShellExecute = false,
                        RedirectStandardOutput = false
                    };

            
            try
            {
                process = Process.Start(processStartInfo);
                process.WaitForExit();
                AutoItX.Sleep(waitTime);

                var streamReader =
                new StreamReader(_outputFile);

                string myString = streamReader.ReadToEnd();
                streamReader.Close();

                return myString;
            }
            catch (Exception ex)
            {            
                _logger.Trace("Process Start Info: " + processStartInfo.FileName + " " + processStartInfo.Arguments);
                _logger.Error(ex, "An exception was thrown during Tesseract processing");
                throw;
            }
            finally
            {
                if (process != null)
                {
                    process.Dispose();
                }
            }
        }
    }
}
