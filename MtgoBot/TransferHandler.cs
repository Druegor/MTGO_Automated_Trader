using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using AutoItBot.Updater;
using BusinessLogicLayer;
using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.MagicCards;
using BusinessLogicLayer.Models;
using BusinessLogicLayer.OCR;
using BusinessLogicLayer.Trader;
using CardDataLayer;
using Framework;
using Framework.Logging;

namespace AutoItBot
{
    public class TransferHandler : ITransferHandler
    {
        private ILogger _logger = IoC.GetLoggerFor<TransferHandler>();
        private TradeFileHandler _tradeFileHandler = new TradeFileHandler();
        private IWindowManager _windowManager = IoC.Resolve<IWindowManager>();
        private IApplicationSettings _applicationSettings = IoC.Resolve<IApplicationSettings>();
        private IPixelBasedVariables _pbv = IoC.Resolve<IPixelBasedVariables>();
        private bool _buddyAdded;
        private const int HeightOfBuddies = 18;

        public void Setup(Transfer transfer)
        {
            _logger.Trace("Starting transfer setup");
            TransferModel = new TransferModel(transfer);
            AutoItX.MouseClick(_pbv.CollectionTab);
            _tradeFileHandler.ClearTradableCards(_applicationSettings.BotName);
            _tradeFileHandler.WriteTradeFile(TransferModel.Cards.Values.ToList());
            _tradeFileHandler.LoadTradableCards();
        }

        protected TransferModel TransferModel { get; set; }

        public void InitiateTrade(string tradee)
        {
            SelectTopBuddy(tradee);
            Square tradeText = _pbv.TradeContextMenuOption.Copy();
            int downArrowCount = DetermineBuddyLocation(tradee);

            var namePoint = new Point(_pbv.FirstBuddySelectedCheck.X, _pbv.FirstBuddySelectedCheck.Y);
            namePoint.Y += (downArrowCount * HeightOfBuddies);

            AutoItX.MouseClick(Constants.RightMouseButton, namePoint.X, namePoint.Y);
            AutoItX.Sleep(2000);

            tradeText = tradeText
                .MoveAlongYAxis(namePoint.Y)
                .MoveAlongYAxis(_pbv.TradeContextDiff.Y)
                .MoveAlongXAxis(namePoint.X - _pbv.TradeContextDiff.X);

            var ocrValue = tradeText.GetOCRValue();
            if (Levenshtein.Compute(ocrValue, Constants.TradeMenuOption) < 2)
            {
                AutoItX.MouseClick(tradeText.MidPoint);
                while (!_windowManager.InTrade() && !_windowManager.CloseTradeCancelledDialog() && _windowManager.ProgramIsActive() && BotHomeScreen.RunningState)
                {
                    AutoItX.Sleep(1000);
                }

                if (!_windowManager.InTrade()) return;
                
                AutoItX.Sleep(10000);
                var initializedTradee = _applicationSettings.InitializeTrade();

                ITrader trader = new TransfererBot(null, new TimeSpan(1), _applicationSettings.BotName, initializedTradee, TransferModel);
                trader.StartTrade();
            }
            else if (Levenshtein.Compute(ocrValue, Constants.RemoveBuddyMenuOption) < 2)
            {
                _logger.Info(tradee + " is not currently online.");
            }
            else
            {
                _logger.ErrorFormat("Ocr Value: <{0}> Bounding Box: <{1}>", ocrValue, tradeText);
            }

            //While Trade(s) in DB !marked as complete
            if (_buddyAdded)
            {
                //Remove Buddy
            }
        }

        private int DetermineBuddyLocation(string tradee)
        {
            int downArrowCount = 0;
            int i = 0;
            while (i++ < 5)
            {
                try
                {
                    AutoItX.Send(Constants.CopyCommand);
                    AutoItX.Sleep(250);
                    string name = Clipboard.GetText();
                    string lastName = " ";

                    while (!name.Equals(tradee, StringComparison.OrdinalIgnoreCase)
                           && !(name.Equals(lastName, StringComparison.OrdinalIgnoreCase))
                           && downArrowCount < _pbv.MaxBuddyListCount)
                    {
                        AutoItX.Send(Constants.DownArrow);
                        downArrowCount++;
                        try
                        {
                            AutoItX.Send(Constants.CopyCommand);
                            AutoItX.Sleep(250);
                            lastName = name;
                            name = Clipboard.GetText();
                        }
                        catch
                        {
                            _logger.Trace("Error in getting name from Clipboard");
                            lastName = " ";
                            AutoItX.Sleep(250);
                        }
                    }

                    //Buddies exist but not the one requested so add it and start over at the top of the list
                    if ((name.Equals(lastName, StringComparison.OrdinalIgnoreCase)))
                    {
                        AddBuddy(tradee);
                        downArrowCount = 0;
                        throw new Exception("just to restart the list finder");
                    }
                    break;
                }
                catch
                {
                    AutoItX.Sleep(250);
                }
            }

            return downArrowCount;
        }

        private void SelectTopBuddy(string tradee)
        {
            AutoItX.MouseClick(_pbv.TopRightTabSelector);
            AutoItX.Sleep(200);

            AutoItX.MouseClick(_pbv.BuddyListSelector);
            AutoItX.Sleep(200);

            AutoItX.MouseClick(_pbv.FirstBuddy);
            AutoItX.Sleep(200);

            if (AutoItX.PixelGetColor(_pbv.FirstBuddySelectedCheck) != _pbv.BuddySelectedColor)
            {
                AddBuddy(tradee);
            }
        }

        private void AddBuddy(string tradee)
        {
            AutoItX.MouseClick(_pbv.AddBuddyButton);
            AutoItX.Sleep(200);
            AutoItX.DoubleClick(_pbv.AddBuddyTextBox);
            AutoItX.Sleep(200);
            AutoItX.Send(tradee + Constants.EnterKey);

            if (IsInvalidBuddyName())
            {
                //QUIT
            }

            _buddyAdded = true;

            //Check for error message, if error message abort transfer bot and log message

            AutoItX.Sleep(200);
            AutoItX.MouseClick(_pbv.FirstBuddy);
            AutoItX.Sleep(200);
        }

        private void RemoveBuddy(string tradee)
        {
            AutoItX.MouseClick(_pbv.AddBuddyButton);
            AutoItX.Sleep(200);
            AutoItX.DoubleClick(_pbv.AddBuddyTextBox);
            AutoItX.Sleep(200);
            AutoItX.Send(tradee + Constants.EnterKey);

            if (IsInvalidBuddyName())
            {
                //QUIT
            }

            _buddyAdded = true;

            //Check for error message, if error message abort transfer bot and log message

            AutoItX.Sleep(200);
            AutoItX.MouseClick(_pbv.FirstBuddy);
            AutoItX.Sleep(200);
        }

        private bool IsInvalidBuddyName()
        {
            Square invalidBuddyName = Square.Initialize(447, 838, 481, 1082);
            return Levenshtein.Compute(invalidBuddyName.GetOCRValue(), "Invalid Buddy Name") > 2;
        }
    }
}