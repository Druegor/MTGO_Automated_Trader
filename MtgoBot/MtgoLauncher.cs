using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using BusinessLogicLayer;
using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.MagicCards;
using Framework;
using Framework.Logging;

namespace AutoItBot
{
    public class MtgoLauncher : IMtgoLauncher
    {
        private ILogger _logger = IoC.GetLoggerFor<MtgoLauncher>();
        private IApplicationSettings _applicationSettings = IoC.Resolve<IApplicationSettings>();
        private IPixelBasedVariables _pbv = IoC.Resolve<IPixelBasedVariables>(); 

        private bool StartMtgoProcess()
        {
            Process process = null;
            ProcessStartInfo processStartInfo;

            processStartInfo = new ProcessStartInfo();

            processStartInfo.FileName = _applicationSettings.ExecutableLocation + "Renamer.exe";
            processStartInfo.WorkingDirectory = _applicationSettings.ExecutableLocation;

            processStartInfo.Arguments = "";
            processStartInfo.WindowStyle = ProcessWindowStyle.Normal;
            processStartInfo.UseShellExecute = true;

            try
            {
                process = Process.Start(processStartInfo);
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error executing mtgo client");
                Program.ForceQuit();
                return false;
            }
            finally
            {
                if (process != null)
                {
                    process.Dispose();
                }
            }
        }

        public void LaunchXP()
        {
            AutoItX.MouseClick(new Point(0, 0));

            if (!StartMtgoProcess()) return;

            LoadCardsIntoMemory();

            var ula = _pbv.UlaAcceptButton.GetOCRValue();
            var logon = _pbv.LogOnButton.GetOCRValue();

            while (Levenshtein.Compute(ula.ToLower(), Constants.AcceptUla.ToLower()) > 2
                   && Levenshtein.Compute(logon.ToLower(), Constants.Logon.ToLower()) > 2)
            {
                AutoItX.DoubleClick(_pbv.LaunchButton.MidPoint);
                AutoItX.MouseMove(0, 0);
                AutoItX.Sleep(5000);
                ula = _pbv.UlaAcceptButton.GetOCRValue();
                logon = _pbv.LogOnButton.GetOCRValue();
                _logger.Trace("Ula ocr = " + ula);
                _logger.Trace("Logon ocr = " + logon);
            }

            Login();
        }

        public void Launch()
        {
            if (Environment.OSVersion.Version.Major == 5)
            {
                LaunchXP();
                return;
            }

            AutoItX.MouseClick(new Point(0, 0));

            if (!StartMtgoProcess()) return;

            var launch = _pbv.LaunchButton.GetOCRValue();
            while (Levenshtein.Compute(launch.ToLower(), Constants.LaunchButton.ToLower()) > 2
                && Levenshtein.Compute(launch.ToLower(), Constants.UpdateButton.ToLower()) > 2)
            {
                AutoItX.MouseMove(0, 0);
                AutoItX.Sleep(5000);
                launch = _pbv.LaunchButton.GetOCRValue();
                _logger.Trace("Launch ocr = " + launch);
            }

            LoadCardsIntoMemory();
            AutoItX.Sleep(5000);

            while (Levenshtein.Compute(launch.ToLower(), Constants.LaunchButton.ToLower()) < 2
                || Levenshtein.Compute(launch.ToLower(), Constants.UpdateButton.ToLower()) < 2)
            {
                AutoItX.MouseClick(new Point(882, 769));
                AutoItX.MouseMove(0, 0);
                AutoItX.Sleep(2000);
                launch = _pbv.LaunchButton.GetOCRValue();
                _logger.Trace("Launch ocr = " + launch);
            }

            AutoItX.Sleep(10000);
            
            Login();
        }

        private void Login()
        {
            var logon = HandleUserLicenseAgreement();

            AutoItX.DoubleClick(_pbv.UserNameTextField);
            AutoItX.Sleep(200);
            AutoItX.Send(_applicationSettings.BotName);
            AutoItX.Sleep(200);
            AutoItX.DoubleClick(_pbv.PasswordTextField);
            AutoItX.Sleep(200);
            if (!string.IsNullOrWhiteSpace(_applicationSettings.Password))
            {
                AutoItX.Send(_applicationSettings.Password);
                AutoItX.Sleep(200);

                while (Levenshtein.Compute(logon.ToLower(), Constants.Logon.ToLower()) < 2)
                {
                    AutoItX.DoubleClick(_pbv.LogOnButton.MidPoint);
                    AutoItX.MouseMove(0, 0);
                    AutoItX.Sleep(5000);
                    logon = _pbv.LogOnButton.GetOCRValue();
                    _logger.Trace("Log On ocr = " + logon);
                }
            }
            else
            {
                _logger.Trace("Waiting for user to login before starting bot.");
                while (Levenshtein.Compute(logon.ToLower(), Constants.Logon.ToLower()) < 2)
                {
                    AutoItX.Sleep(5000);
                    logon = _pbv.LogOnButton.GetOCRValue();
                    _logger.Trace("Log On ocr = " + logon);
                }
                _logger.Trace("Logged in starting bot.");
            }

            AutoItX.Sleep(1000);

            MaximizeWindow();
        }

        public void MaximizeWindow()
        {
            while (Levenshtein.Compute(_pbv.SectionTitle.GetOCRValue(), Constants.HomeScreenText) > 2)
            {
                _logger.Trace("Maximizing Window");
                AutoItX.DoubleClick(_pbv.MaximizeWindowPosition);
                AutoItX.Sleep(10000);
            }
        }

        public void Exit()
        {
            Process[] processes =
                Process.GetProcesses().Where(p => p.MainWindowTitle == Constants.MagicOnlineProgramName).ToArray();

            foreach (Process proc in processes)
            {
                proc.CloseMainWindow();
                proc.WaitForExit();
            }
        }

        private string HandleUserLicenseAgreement()
        {
            var ula = _pbv.UlaAcceptButton.GetOCRValue();
            var logon = _pbv.LogOnButton.GetOCRValue();
            while (Levenshtein.Compute(ula.ToLower(), Constants.AcceptUla.ToLower()) > 2
                   && Levenshtein.Compute(logon.ToLower(), Constants.Logon.ToLower()) > 2)
            {
                AutoItX.MouseMove(0, 0);
                AutoItX.Sleep(5000);
                ula = _pbv.UlaAcceptButton.GetOCRValue();
                logon = _pbv.LogOnButton.GetOCRValue();
                _logger.Trace("Ula ocr = " + ula);
                _logger.Trace("Logon ocr = " + logon);
            }

            if (Levenshtein.Compute(ula.ToLower(), Constants.AcceptUla.ToLower()) < 2)
            {
                AutoItX.MouseMove(_pbv.UlaSlider.TopLeft);
                AutoItX.Sleep(200);
                AutoItX.MouseDown("left");
                AutoItX.Sleep(200);
                AutoItX.MouseMove(_pbv.UlaSlider.BottomRight);
                AutoItX.Sleep(200);
                AutoItX.MouseUp("left");
                AutoItX.Sleep(200);

                while (Levenshtein.Compute(ula.ToLower(), Constants.AcceptUla.ToLower()) < 2)
                {
                    AutoItX.MouseClick(_pbv.UlaAcceptButton.MidPoint);
                    AutoItX.MouseMove(0, 0);
                    AutoItX.Sleep(5000);
                    ula = _pbv.UlaAcceptButton.GetOCRValue();
                    _logger.Trace("Ula ocr = " + ula);
                }

                while (Levenshtein.Compute(logon.ToLower(), Constants.Logon.ToLower()) > 2)
                {
                    AutoItX.MouseMove(0, 0);
                    AutoItX.Sleep(5000);
                    logon = _pbv.LogOnButton.GetOCRValue();
                    _logger.Trace("Log On ocr = " + logon);
                }
            }
            return logon;
        }

        private void LoadCardsIntoMemory()
        {
            var magicCardList = new MagicCardList();
            magicCardList.InvalidateCache();
            IoC.Register<IMagicCardList>(magicCardList);
        }
    }
}