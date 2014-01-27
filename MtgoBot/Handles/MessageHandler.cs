using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using BusinessLogicLayer;
using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.MagicCards;
using BusinessLogicLayer.Properties;
using CardDataLayer.Models;
using Framework;
using Framework.Logging;

namespace AutoItBot.Handles
{
    public class MessageHandler : IMessageHandler
    {
        private ILogger _logger = IoC.GetLoggerFor<MessageHandler>();
        private IPixelBasedVariables _pbv;

        public MessageHandler() : this(IoC.Resolve<IPixelBasedVariables>()) { }

        public MessageHandler(IPixelBasedVariables pbv)
        {
            _pbv = pbv;
        }

        /// <summary>
        /// Submits a message to the Community Classifieds
        /// </summary>
        /// <param name="messageText">The message to be posted.</param>
        private void PrintCommunityClassifiedMessage(string messageText)
        {
            Constants.SetClipboard(messageText);
            AutoItX.DoubleClick(_pbv.ClassifiedMessageBox);
            AutoItX.Sleep(200);
            AutoItX.Send(Constants.PasteCommand);
            AutoItX.Sleep(1000);
            AutoItX.MouseClick(_pbv.ClassifiedPostingButton);
        }

        public void PrintCommunityClassified(string message)
        {
            AutoItX.MouseClick(_pbv.ClassifiedsTab);
            AutoItX.Sleep(2000);
            var sectionTitle = IoC.Resolve<IWindowManager>().GetSectionTitle();

            if (Levenshtein.Compute(sectionTitle, Constants.ClassifiedsTitle) < 4)
            {
                AutoItX.MouseClick(_pbv.EditPostingButton);
                AutoItX.Sleep(2000);
                PrintCommunityClassifiedMessage(message);
            }
            else
            {
                AutoItX.MouseClick(_pbv.HomeTabLocation);
                AutoItX.Sleep(5000);
                sectionTitle = IoC.Resolve<IWindowManager>().GetSectionTitle();
                if (Levenshtein.Compute(sectionTitle, Constants.HomeScreenText) > 2)
                {
                    _logger.Error("Could not find home screen quitting. Found: " + sectionTitle);
                    Program.ForceQuit();
                }

                AutoItX.Sleep(2000);
                _pbv.HomeScreen.CheckForOCRValue();
                AutoItX.Sleep(2000);
                PrintCommunityClassifiedMessage(message);
            }

            IoC.Resolve<IWindowManager>().MoveToDeckEditor();
        }

        public void SendPriceMessage(List<MagicCard> cardsYouGet, bool printSellPrice)
        {
            StringBuilder sb = new StringBuilder();
            foreach (MagicCard magicCard in cardsYouGet)
            {
                var price = printSellPrice ? magicCard.SellPrice : magicCard.BuyPrice;

                if (sb.Length > Constants.MaxMessageLength - 75)
                {
                    SendMessage(sb.ToString());
                    sb = new StringBuilder();
                }

                if (magicCard.CopiesOfCard > 1)
                {
                    sb.AppendFormat("[sV] {0}x {1} |{2}|  ", magicCard.CopiesOfCard, magicCard.Name,
                                    price * magicCard.CopiesOfCard);
                }
                else
                {
                    sb.AppendFormat("[sV] {0} |{1}|  ", magicCard.Name, price * magicCard.CopiesOfCard);
                }
            }

            _logger.Trace("Cards selected: " + sb);
            SendMessage(sb.ToString());
        }

        public void SendMessage(string message)
        {
            SendMessage(message, _pbv.TradeConversationMessageBox);
        }

        public void SendMessage(string message, Point messageBoxLocation)
        {
            if (string.IsNullOrEmpty(message)) return;

            _logger.Debug("<Sending>" + message);
            Constants.SetClipboard(message);
            AutoItX.DoubleClick(messageBoxLocation);
            AutoItX.Sleep(200);
            AutoItX.Send(Constants.PasteCommand);
            AutoItX.Sleep(200);
            AutoItX.Send(Constants.EnterKey);
        }

        public string GetLastMessageFromPriceMessage(Point firstPrivateMessage, out string tradee)
        {
            const string boldStrStart = "[b]";
            const string boldStr = "[/b]:";
            int i = 0;
            while (i++ < 5)
            {
                string message = string.Empty;
                try
                {
                    AutoItX.MouseClick(firstPrivateMessage);
                    AutoItX.Send(Constants.EndKey);
                    AutoItX.Sleep(250);
                    AutoItX.Send(Constants.CopyCommand);
                    AutoItX.Sleep(500);
                    message = Clipboard.GetText();

                    var tradeNameStart = message.IndexOf(boldStrStart) + boldStrStart.Length;
                    var location = message.IndexOf(boldStr);
                    tradee = message.Substring(tradeNameStart, location - tradeNameStart);
                    if (location <= 0) continue;

                    var priceCheck = message.Substring(location + boldStr.Length, message.Length - (location + boldStr.Length)).Trim().ToLower();
                    _logger.Trace("Price Check Message: " + priceCheck);
                    return priceCheck;
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "price check message failure for: " + message);
                    AutoItX.Sleep(100);
                }
            }

            tradee = string.Empty;
            return string.Empty;
        }

        public void SendPriceCheckMessage(List<MagicCard> cardList, Point messageTextBox)
        {
            StringBuilder sb = new StringBuilder();
            if (cardList.Count == 0)
            {
                sb.AppendLine(Resources.UnknownCard);
            }
            else
            {
                foreach (MagicCard magicCard in cardList)
                {
                    if (sb.Length > Constants.MaxMessageLength - 75)
                    {
                        SendMessage(sb.ToString(), messageTextBox);
                        sb = new StringBuilder();
                    }

                    sb.AppendFormat(Resources.PriceCheckResult,
                                    magicCard.Name,
                                    magicCard.BuyPrice,
                                    magicCard.SellPrice, magicCard.Set);
                }
            }

            _logger.Trace("Price check message: " + sb);
            SendMessage(sb.ToString(), messageTextBox);
        }

        public string GetTradeeName(string botName)
        {
            Clipboard.Clear();
            var name = botName;
            while (name.Equals(botName, StringComparison.OrdinalIgnoreCase))
            {
                AutoItX.MouseClick(_pbv.TradeConversationUsername);
                int i = 0;
                while (i++ < 5)
                {
                    try
                    {
                        AutoItX.Send(Constants.CopyCommand);
                        AutoItX.Sleep(250);
                        name = Clipboard.GetText();
                        if (name.Equals(botName, StringComparison.OrdinalIgnoreCase))
                        {
                            AutoItX.Send(Constants.DownArrow);
                            while (i++ < 5)
                            {
                                try
                                {
                                    AutoItX.Send(Constants.CopyCommand);
                                    AutoItX.Sleep(250);
                                    name = Clipboard.GetText();
                                    break;
                                }
                                catch
                                {
                                    AutoItX.Sleep(200);
                                }
                            }

                        }
                        break;
                    }
                    catch
                    {
                        AutoItX.Sleep(200);
                    }
                }
            }

            _logger.Trace("Tradee Name Found: " + name);

            var specialCharStartLocation = name.IndexOf('[');
            var specialCharEndLocation = name.IndexOf(']');

            if (specialCharStartLocation >= 0 && specialCharEndLocation >= 0)
            {
                return name.Substring(0, specialCharStartLocation - 1);
            }

            return name;
        }

        public string GetLastTradeChatMessage()
        {
            AutoItX.MouseClick(_pbv.TradeConversationFirstMessage);
            AutoItX.Send("{END}");
            AutoItX.Sleep(200);

            int i = 0;
            while (i++ < 10)
            {
                try
                {
                    AutoItX.Send(Constants.CopyCommand);
                    var lastMessage = Clipboard.GetText();
                    return lastMessage.ToLower();
                }
                catch(Exception ex)
                {
                    AutoItX.Sleep(200);
                    if (i >= 8)
                    {
                        _logger.Error(ex, "Get last message failed it may be due to another window being open.");
                    }
                }
            }
            return string.Empty;
        }
    }
}