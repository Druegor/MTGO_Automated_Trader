using System.Collections.Generic;
using System.Drawing;
using CardDataLayer.Models;

namespace BusinessLogicLayer.Interfaces
{
    public interface IMessageHandler
    {
        /// <summary>
        /// Submits a message to the Community Classifieds
        /// </summary>
        /// <param name="messageText">The message to be posted.</param>
        void PrintCommunityClassified(string message);
        void SendPriceMessage(List<MagicCard> cardsYouGet, bool printSellPrice);
        void SendMessage(string message);
        void SendMessage(string message, Point messageBoxLocation);
        string GetLastMessageFromPriceMessage(Point firstPrivateMessage, out string tradee);
        void SendPriceCheckMessage(List<MagicCard> cardList, Point messageTextBox);
        string GetTradeeName(string botName);
        string GetLastTradeChatMessage();
    }
}