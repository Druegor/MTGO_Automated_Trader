using System;
using System.Collections.Generic;
using System.Windows.Forms;
using AutoItBot.Handles;
using AutoItBot.PixelBasedVariables;
using BusinessLogicLayer;
using BusinessLogicLayer.Database;
using BusinessLogicLayer.Interfaces;
using Framework;
using Framework.Logging;

namespace AutoItBot
{
    public class Program
    {
        private static ILogger logger;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Setup();

            Application.ApplicationExit += OnApplicationExit;
            Application.Run(new BotHomeScreen());            
        }
        
        private static void OnApplicationExit(object sender, EventArgs e)
        {
            ForceQuit();
        }

        private static void Setup()
        {
            IoC.Register<ILoggerFactory>(
                new LoggerFactory(
                    new List<LoggerCtorDelegate>
                        {
                            (type, log) => new Log4NetLogger(type, log)
                        }));

            Log4NetLogger.ConfigureAndWatch("Log4Net.config");
            logger = IoC.GetLoggerFor<Program>();
            logger.Trace("Test Logger");
            RegisterVariablesForResolution();
            RegistrationModule();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ThreadException += ApplicationThreadException;
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            // Add the event handler for handling non-UI thread exceptions to the event. 
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            logger.Trace(
                "Setup complete all IoC registrations finished, this log file proves that IoC is now working correctly.");
        }

        private static void RegistrationModule()
        {
            IoC.Register<IApplicationSettings>(new ApplicationSettings());
            IoC.Register<IMessageHandler>(new MessageHandler());
            IoC.Register<IWindowManager>(new WindowManager());
            IoC.Register<ITradeFilterHandler>(new TradeFilterHandler());
            IoC.Register<ITradeHandler>(new TradeHandler(), false);
            IoC.Register<IMtgoLauncher>(new MtgoLauncher());
            IoC.Register<ITransferHandler>(new TransferHandler(), false);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                Exception ex = (Exception)e.ExceptionObject;
                MessageBox.Show("An Error Occured: " + ex.Message + "\n\nStack Trace:\n" + ex.StackTrace);
            }
            catch (Exception exc)
            {
                try
                {
                    MessageBox.Show("Fatal Non-UI Error",
                        "Fatal Non-UI Error. Could not write the error to the event log. Reason: "
                        + exc.Message, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
                finally
                {
                    Application.Exit();
                }
            }
        }

        private static void ApplicationThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            var log = IoC.GetLoggerFor<Program>();
            log.Error(e.Exception, "Error caught by application thread exception.");
            MessageBox.Show(e.Exception.Message + "\n " + e.Exception.StackTrace);
            //ForceQuit();
        }

        private static void RegisterVariablesForResolution()
        {
            var screenLocations = new ScreenLocations();
            screenLocations.Load();
            IoC.Register<IPixelBasedVariables>(screenLocations);
        }

        public static void ForceQuit()
        {
            logger.Error("Quiting the application.");
            DatabaseInteractions.SetBotRunningStatus(IoC.Resolve<IApplicationSettings>().BotName, false);
            IoC.Resolve<IMtgoLauncher>().Exit();
            Application.Exit();
        }
    }
}
