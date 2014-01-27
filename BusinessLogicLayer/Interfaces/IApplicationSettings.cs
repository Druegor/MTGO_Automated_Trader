using System;
using System.Collections.Generic;

namespace BusinessLogicLayer.Interfaces
{
    public interface IApplicationSettings
    {
        string ClassifiedMessage { get; }
        string BotName { get; }
        int MaxTradeMinutes { get; }
        string DeckFileLocation { get; }
        int Delay { get; }
        string BotType { get; }
        string TradeFileLocation { get; set; }
        int OwnedLessThan { get; set; }
        string TesseractLocation { get; set; }
        double Commons { get; set; }
        double Uncommons { get; set; }
        double Mythics { get; set; }
        double Rares { get; set; }
        string ExecutableLocation { get; set; }
        string Password { get; set; }
        string OtherBots { get; set; }
        ITrader GetTrader(List<CardSet> cardSetSearchOrder, TimeSpan maxTime, string botName, string botType);
        string InitializeTrade();
    }
}