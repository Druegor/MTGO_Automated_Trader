using System;
using System.Collections.Generic;

namespace BusinessLogicLayer.Trader
{
    public class YesBot : TraderBase
    {
        public YesBot(List<CardSet> setSearchOrder, TimeSpan maxTradeTime, string botName, string tradee) 
            : base(setSearchOrder, maxTradeTime, botName, tradee)
        {
        }

        public override void StartTrade()
        {
            base.StartTrade();
            this.MessageHandler.SendMessage(
                "Registered as a bot in my group so starting in slave mode, will confirm any trade.");

            do
            {
                this.MagicOnlineInterface.PressConfirmButton();
                AutoItX.Sleep(10000);
            } while (!this.WinManager.OnTradeConfirmScreen() && this.WinManager.OnTradeScreen());

            if (!this.WinManager.InTrade()) return;

            this.CardsYouGive.Set(this.MagicOnlineInterface.ExamineYouGiveCardsOnConfirmedScreen());

            if (!this.WinManager.InTrade()) return;

            CompleteConfirmationScreenTrade();
            return;
        }
    }
}
