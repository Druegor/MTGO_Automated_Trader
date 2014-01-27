using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLogicLayer.Models;
using Framework;
using Framework.Logging;

namespace BusinessLogicLayer.Trader
{
    public class TransferBot : TraderBase
    {
        private ILogger _logger = IoC.GetLoggerFor<TransferBot>();
        public TransferModel Transfer { get; set; }

        public TransferBot(List<CardSet> setSearchOrder, TimeSpan maxTradeTime, string botName, string tradee,
                           TransferModel transfer)
            : base(setSearchOrder, maxTradeTime, botName, tradee)
        {
            this.Transfer = transfer;
        }

        public override void StartTrade()
        {
            base.StartTrade();

            if (!this.WinManager.InTrade()) return;

            this.MessageHandler.SendPriceMessage(Transfer.Cards.Values.ToList(), false);

            this.MagicOnlineInterface.WriteWishListFile(Transfer.Cards);
            this.MagicOnlineInterface.LoadWishList();
            AutoItX.Sleep(10000);
            this.MagicOnlineInterface.DetermineYouGetAmount();
            this.MagicOnlineInterface.DetermineCardsYouGet();
            this.CardsYouGet.Set(this.MagicOnlineInterface.CardsYouGet);
            var count = this.CardsYouGet.CountOfCards();

            if (count != Transfer.Cards.Sum(p => p.Value.CopiesOfCard))
            {
                _logger.ErrorFormat("{0} should match {1}", count, Transfer.Cards.Sum(p => p.Value.CopiesOfCard));
                this.MagicOnlineInterface.CancelTrade();
                return;
            }

            do
            {
                this.MagicOnlineInterface.PressConfirmButton();
                AutoItX.Sleep(2000);
            } while (!this.WinManager.OnTradeConfirmScreen() && CheckTimer() && this.WinManager.OnTradeScreen());

            if (!this.WinManager.InTrade()) return;

            var youGiveConfirmedAmount = this.MagicOnlineInterface.DetermineYouGiveConfirmedAmount();

            if (youGiveConfirmedAmount > 0)
            {
                _logger.Error("This bot should not be giving any cards away.");
                this.MagicOnlineInterface.CancelTrade();
            }

            if (!this.WinManager.InTrade()) return;

            CompleteConfirmationScreenTrade();
        }
    }
}