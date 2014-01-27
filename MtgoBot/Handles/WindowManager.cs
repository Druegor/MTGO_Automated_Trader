using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BusinessLogicLayer;
using BusinessLogicLayer.Database;
using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.MagicCards;
using BusinessLogicLayer.Properties;
using Framework;
using Framework.Logging;

namespace AutoItBot.Handles
{
    public class WindowManager : IWindowManager
    {
        private ILogger _logger = IoC.GetLoggerFor<WindowManager>();
        private IPixelBasedVariables _pbv;
        private IMessageHandler _messageHandler;

        public WindowManager() : this(IoC.Resolve<IPixelBasedVariables>(), IoC.Resolve<IMessageHandler>()) { }

        public WindowManager(IPixelBasedVariables pbv, IMessageHandler messageHandler)
        {
            _pbv = pbv;
            _messageHandler = messageHandler;
        }

        public bool CheckForLoadDeckWindow()
        {
            _logger.Trace("Checking for load deck window.");
            return _pbv.CancelLoadDeck.CheckForOCRValue();
        }

        public void CheckForPrivateConversationWindows()
        {
            Point location;
            if (!TryFindChatMessage(out location)) return;

            location.X += _pbv.PrivateChatCloseButtonLocation.X -
                          _pbv.TradeConversationInitialLocation.X;

            string tradee;
            var message = _messageHandler.GetLastMessageFromPriceMessage(new Point(location.X - 200, location.Y + 45), out tradee);
            var messageBoxLocation = new Point(location.X - 100, location.Y + _pbv.MessageBoxLocationYDiff);
            var botName = IoC.Resolve<IApplicationSettings>().BotName;
            if (message.ToLower().StartsWith("pc "))
            {
                _logger.Trace("Private message: " + message);
                var priceChecksAvailable = DatabaseInteractions.RemainingPriceChecks(tradee);
                if (priceChecksAvailable > 0)
                {
                    var cardSubstring = message.Substring(3, message.Length - 3);
                    var cards = cardSubstring.Split(';').Take(priceChecksAvailable);
                    var cardList = new List<MagicCard>();
                    foreach (var cardName in cards)
                    {
                        _logger.Trace("Cardname: " + cardName);
                        var magicCards = IoC.Resolve<IMagicCardList>().GetCard(cardName.Trim());
                        cardList.AddRange(magicCards.Where(card => card != null));
                    }

                    DatabaseInteractions.SavePriceChecksForUser(tradee, cardList);
                    _messageHandler.SendPriceCheckMessage(cardList, messageBoxLocation);
                    AutoItX.Sleep(250);
                }
                else
                {
                    _messageHandler.SendMessage(Resources.PriceCheckLimit, messageBoxLocation);
                }
            }
            else
            {
                if (!tradee.Equals(botName, StringComparison.OrdinalIgnoreCase))
                {
                    _messageHandler.SendMessage(Resources.BotMode, messageBoxLocation);
                }
            }
            AutoItX.Sleep(500);
            _logger.Trace("Closing chat window.");
            AutoItX.MouseClick(location);
        }

        private bool TryFindChatMessage(out Point location)
        {
            var yMax = Screen.PrimaryScreen.Bounds.Height;
            
            for (
                Point newLocation = new Point(_pbv.TradeConversationInitialLocation.X, _pbv.TradeConversationInitialLocation.Y);
                newLocation.Y < yMax;
                newLocation.X += _pbv.TradeConversationChange.X,
                newLocation.Y += _pbv.TradeConversationChange.Y
                )
            {
                var color = AutoItX.PixelGetColor(newLocation);
                //if (color != 0 && color != 15198183)
                //{
                //    _logger.TraceFormat("Color: {0} at Point: {1},{2}", color, newLocation.X, newLocation.Y);
                //    AutoItX.MouseMove(newLocation);
                //    AutoItX.Sleep(5000);
                //}
                if (_pbv.TradeConversationDockWindowButtonColorArray.Contains(color))
                {
                    _logger.TraceFormat("Chat Window Found at {0}, {1} color: {2}", newLocation.X, newLocation.Y, color);
                    location = newLocation;
                    return true;
                }
            }

            location = new Point();
            return false;
        }
        
        /// <summary>
        /// Find the trade window communication message box.  
        /// </summary>
        /// <returns>Returns true if the Chat Window is found.</returns>
        public bool FindTradeChatWindowAndDock()
        {
            Point location;
            if(TryFindChatMessage(out location))
            {
                _logger.Trace("Chat window found - docking.");
                AutoItX.MouseClick(location);
                AutoItX.Sleep(100);
                AutoItX.MouseMove(_pbv.FilterBlankSpotPosition);
                AutoItX.Sleep(150);
                AutoItX.MouseClick(_pbv.TradeConversationUsername);
                AutoItX.Sleep(100);
                return true;
            }

            return false;
        }

        public bool CheckIfChallenge()
        {
            _logger.Trace("Checking for challenge window.");
            return _pbv.Challenge.CheckForOCRValue();
        }

        public bool CheckForTrade()
        {
            _logger.Trace("Checking for trade.");
            return _pbv.TradeRequest.CheckForOCRValue();
        }

        public bool CheckForCompleteTrade()
        {
            _logger.Trace("Checking for complete trade window.");
            return _pbv.TradeComplete.CheckForOCRValue();
        }

        public bool CheckForConnectionProblem()
        {
            _logger.Trace("Checking for connection problem.");
            return _pbv.ConnectionProblem.CheckForOCRValue();
        }

        public bool CheckIfOnHomeScreen()
        {
            _logger.Trace("Checking if on home screen.");
            return _pbv.HomeScreen.CheckForOCRValue();
        }

        public bool CheckIfNewCardConfirmationScreen()
        {
            bool found = false;
            _logger.Trace("Checking for new card confirmation screen.");
            while (Levenshtein.Compute(_pbv.NewCardsWindow.GetOCRValue(), Constants.NewCardsFull) <= 3)
            {
                _logger.Trace("New Card Confirmation Screen is Visible.");
                AutoItX.MouseClick(_pbv.NewCardsButton);
                found = true;
                AutoItX.Sleep(200);
            }
            return found;
        }

        public bool CheckIfLoadWishListScreenIsVisiable()
        {
            _logger.Trace("Checking for load wish list window.");
            var checkIfLoadWishListScreenIsVisiable = _pbv.LoadWishList.CheckForOCRValue();
            if (checkIfLoadWishListScreenIsVisiable) _logger.Trace("Load wish list screen visible.");
            return checkIfLoadWishListScreenIsVisiable;
        }

        public bool CheckIfSystemAlert()
        {
            _logger.Trace("Checking for system alert.");
            var checkIfSystemAlert = _pbv.SystemAlert.CheckForOCRValue();
            if(checkIfSystemAlert) _logger.Trace("System Alert Found.");
            return checkIfSystemAlert;
        }

        public bool CloseTradeCancelledDialog()
        {
            _logger.Trace("Checking for trade cancelled dialog.");
            if(_pbv.TradeCancelled.CheckForOCRValue())
            {
                _logger.Trace("Trade cancelled dialog found.");
                return true;
            }
            return false;
        }

        public bool ConfirmCancelledTrade()
        {
            _logger.Trace("Checking to confirm trade cancelled dialog");
            return _pbv.ConfirmCancelTrade.CheckForOCRValue();
        }

        /// <summary>
        /// Check for other user communication windows to close them if they are not the trade or disconnect window.
        /// Windows to check for: Challenge, Chat before Trade
        /// </summary>
        public bool CheckForOtherWindows()
        {
            CheckForPrivateConversationWindows();
            var checkForOtherWindows = CheckIfChallenge() ||
                                       CheckIfSystemAlert() ||
                                       CheckForLoadDeckWindow();
            if (checkForOtherWindows)
            {
                _logger.Trace("Other windows found.");
            }

            if (CheckForConnectionProblem())
            {
                _logger.Error("Connection problem found force quiting application and bot.");
                Program.ForceQuit();
            }

            return  checkForOtherWindows;
        }

        public void MoveToDeckEditor()
        {
            _logger.Trace("Checking if need to deck editor.");
            if (Levenshtein.Compute(GetSectionTitle(),Constants.DeckEditorTitle) > 4)
            {
                _logger.Trace("Moving to deck editor.");
                AutoItX.DoubleClick(_pbv.DeckEditorTab);
                AutoItX.Sleep(1000);
                AutoItX.MouseMove(_pbv.FilterBlankSpotPosition);
            }
        }

        public string GetSectionTitle()
        {
            var title = _pbv.SectionTitle.GetOCRValue();
            _logger.Trace("Section: " + title);
            return title;
        }

        public bool InTrade()
        {
            if (ProgramIsActive())
            {
                return (OnTradeScreen() || OnTradeConfirmScreen()) && BotHomeScreen.RunningState;
            }
            _logger.Trace("Not in trade.");
            return false;
        }

        public bool ProgramIsActive()
        {
            if (AutoItX.WinExists(Constants.MagicOnlineProgramName) != 1)
            {
                return false;
            }

            AutoItX.WinSetTopmost(Constants.MagicOnlineProgramName);
            return true;
        }

        public bool OnTradeScreen()
        {
            return Levenshtein.Compute(GetSectionTitle(), Constants.TradeTitle) < 4;
        }

        public bool OnTradeConfirmScreen()
        {
            return Levenshtein.Compute(GetSectionTitle(), Constants.TradeConfirmTitle) < 4;
        }

        public void CancelTradeFromConfirmationScreen()
        {
            while (OnTradeConfirmScreen())
            {
                _logger.Trace("Cancelling from trade confirmation screen.");
                AutoItX.MouseClick(_pbv.ConfirmTradeCancelButton);
                AutoItX.Sleep(1000);
            }
        }
    }
}