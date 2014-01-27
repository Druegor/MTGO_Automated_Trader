using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;
using BusinessLogicLayer;
using BusinessLogicLayer.Database;
using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.PriceUpdaters;
using BusinessLogicLayer.Trader;
using Framework;
using Framework.Logging;

namespace AutoItBot
{
    public partial class BotHomeScreen : Form
    {
        private ILogger _logger = IoC.GetLoggerFor<BotHomeScreen>();

        public static bool RunningState
        {
            get { return DatabaseInteractions.CheckIsBotRunning(_botName); }
            private set { DatabaseInteractions.SetBotRunningStatus(_botName, value); }
        }

        private Stopwatch _stopwatch = new Stopwatch();
        private static string _botName;
        private string _classifiedMessage;
        private TimeSpan _maxTime;
        private IPixelBasedVariables _pbv = IoC.Resolve<IPixelBasedVariables>();
        private IApplicationSettings _applicationSettings = IoC.Resolve<IApplicationSettings>();
        private IMtgoLauncher _launcher = IoC.Resolve<IMtgoLauncher>();
        private IWindowManager _windowManager = IoC.Resolve<IWindowManager>();

        public BotHomeScreen()
        {
            InitializeComponent();
            int maxMinutes = _applicationSettings.MaxTradeMinutes;
            _maxTime = new TimeSpan(0, 0, maxMinutes, 0);
            _botName = _applicationSettings.BotName;
            _classifiedMessage = _applicationSettings.ClassifiedMessage;
            //StartMTGOProcess();
        }

        private void BtnStartClick(object sender, EventArgs e)
        {
            _launcher.Launch();
            this.btnStart.Enabled = false;
            RunningState = true;
            lblRunningStatus.Text = "Status: Running";
            if(chkSingleThread.Checked)
            {
                StartTradeLoop();
            }
            else
            {
                ThreadStart job = StartTradeLoop;
                Thread thread = new Thread(job);
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
            }
        }

        private void StartTradeLoop()
        {
            _stopwatch = new Stopwatch();
            _stopwatch.Start();
            var messageHandler = IoC.Resolve<IMessageHandler>();

            messageHandler.PrintCommunityClassified(_classifiedMessage);
            
            _logger.Trace("Running State: " + RunningState);
            _logger.Trace("Is Window Active: " + _windowManager.ProgramIsActive());
            var stopwatch = new Stopwatch();
            while (RunningState && _windowManager.ProgramIsActive())
            {
                AttemptTransfer();

                int checksum = AutoItX.PixelChecksum(_pbv.DeckEditorDeckSection);
                _logger.Trace("Deck Editor Checksum: " + checksum);

                while (_pbv.DeckEditorDeckSectionChecksum.Contains(checksum))
                {
                    AutoItX.Sleep(1000);
                    checksum = AutoItX.PixelChecksum(_pbv.DeckEditorDeckSection);
                   
                    _windowManager.CheckForPrivateConversationWindows();
                    if (_stopwatch.Elapsed.Minutes > 5)
                    {
                        if (!RunningState)
                        {
                            _logger.Info("Calling force quit from the checksum loop because running state is off.");
                            Program.ForceQuit();
                        }

                        _stopwatch.Restart();
                        if (_windowManager.ProgramIsActive() && this.chkHaltScreenSaver.Checked)
                        {
                            messageHandler.PrintCommunityClassified(_classifiedMessage);
                        }

                        AttemptTransfer();
                    }
                }

                while (_windowManager.CheckForOtherWindows())
                {
                    _logger.Trace("Other windows found.");
                    AutoItX.Sleep(250);
                }

                _windowManager.CloseTradeCancelledDialog();

                var tradeFound = false;
                while (_windowManager.CheckForTrade())
                {
                    _logger.TraceFormat("{0}: Trade request found.", _botName);
                    tradeFound = true;
                    stopwatch = new Stopwatch();
                    stopwatch.Start();
                    AutoItX.Sleep(500);
                }
                
                while (tradeFound && !_windowManager.InTrade() && !_windowManager.CloseTradeCancelledDialog() && stopwatch.ElapsedMilliseconds < 20000)
                {
                    _logger.Trace("Waiting for Trade to start or cancelled dialog to show up.");
                    AutoItX.Sleep(1000);
                }

                if (tradeFound && _windowManager.InTrade())
                {
                    List<CardSet> cardSetSearchOrder = new List<CardSet>
                                                           {
                                                               CardSet.Standard,
                                                               CardSet.Extended,
                                                               CardSet.AllCards
                                                           };

                    ITrader trader = _applicationSettings.GetTrader(cardSetSearchOrder, _maxTime, _botName, _applicationSettings.BotType);
                    trader.StartTrade();
                    _logger.InfoFormat("Trade finished for {0}", _botName);
                }

                _windowManager.MoveToDeckEditor();
            }

            _logger.Info("Calling force quit from outside the loop, program exited loop due to mtgo client closing or database setting.");
            Program.ForceQuit();
        }

        private void AttemptTransfer()
        {
            var transfers = DatabaseInteractions.GetTransfers(this._applicationSettings.BotName);
            while (transfers.Any() && RunningState && _windowManager.ProgramIsActive())
            {
                _logger.Info("Transfers exist");
                var transfer = transfers.First();
                
                var handler = IoC.Resolve<ITransferHandler>();
                handler.Setup(transfer);
                handler.InitiateTrade(transfer.Tradee.Name);

                while (_windowManager.CheckForOtherWindows())
                {
                    _logger.Trace("Other windows found.");
                    AutoItX.Sleep(250);
                }

                _windowManager.CloseTradeCancelledDialog();

                transfers = DatabaseInteractions.GetTransfers(this._applicationSettings.BotName);
            }
        }

        private void BtnUpdatePricesClick(object sender, EventArgs e)
        {
            _logger.Trace("Attempting to do some price updating from a 3rd party website.");
            //Do an update
            _logger.Trace("Trade update completed.");
        }

        private void BtnTestClick(object sender, EventArgs e)
        {
            var handler = IoC.Resolve<ITradeHandler>();
            var cards = handler.ExamineYouGiveCardsOnConfirmedScreen();
            foreach (var card in cards)
            {
                _logger.Trace(card.Value.ToString());
            }
        }

        private void LaunchPixelFinder(object sender, EventArgs e)
        {
            var commands = new AutoItCommands();
            commands.ShowDialog();
        }
    }
}
