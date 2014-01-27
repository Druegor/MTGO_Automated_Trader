using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLogicLayer.Database;
using BusinessLogicLayer.MagicCards;
using BusinessLogicLayer.Models;
using Framework;
using Framework.Logging;

namespace BusinessLogicLayer.Trader
{
    public class TransfererBot : TraderBase
    {
        private ILogger _logger = IoC.GetLoggerFor<TransfererBot>();
        public TransferModel Transfer { get; set; }

        public TransfererBot(List<CardSet> setSearchOrder, TimeSpan maxTradeTime, string botName, string tradee, TransferModel transfer) : base(setSearchOrder, maxTradeTime, botName, tradee)
        {
            this.Transfer = transfer;
        }

        public override void StartTrade()
        {
            if (string.IsNullOrWhiteSpace(this.TradeeName))
            {
                this.MagicOnlineInterface.CancelTrade();
                AutoItX.Sleep(10000);
                return;
            }

            this.MessageHandler.SendMessage(string.Format("Starting transfer."));
            
            do
            {
                this.MagicOnlineInterface.PressConfirmButton();
                AutoItX.Sleep(10000);
            } while (!this.WinManager.OnTradeConfirmScreen() && this.WinManager.OnTradeScreen());

            if (!this.WinManager.InTrade()) return;

            var youGiveConfirmedAmount = this.MagicOnlineInterface.DetermineYouGiveConfirmedAmount();
            this.CardsYouGive.Set(this.Transfer.Cards);
            if (youGiveConfirmedAmount != this.CardsYouGive.CountOfCards()) 
            {
                _logger.ErrorFormat("{0} does not equate to {1} which is how many should be traded", youGiveConfirmedAmount, this.CardsYouGive.CountOfCards());
                this.MagicOnlineInterface.CancelTradeFromConfirmationScreen();
                return;
            }

            var confirmedCards = this.MagicOnlineInterface.ExamineYouGiveCardsOnConfirmedScreen();

            if (confirmedCards.Values.Sum(p=> p.CopiesOfCard) != this.Transfer.Cards.Values.Sum(p => p.CopiesOfCard))
            {
                _logger.ErrorFormat("Number of cards I give do not match what I should give {0} : {1}",
                        confirmedCards.Values.Sum(p => p.CopiesOfCard), this.Transfer.Cards.Values.Sum(p => p.CopiesOfCard));
                this.MagicOnlineInterface.CancelTradeFromConfirmationScreen();
                return;
            }

            if (confirmedCards.Values.Count != this.Transfer.Cards.Values.Count)
            {
                _logger.Error("Confirmation screen dictionaries do not match.");
                return;
            }

            foreach (MagicCard card in confirmedCards.Values)
            {
                if (!this.Transfer.Cards.ContainsKey(card.MtgoId))
                {
                    _logger.ErrorFormat("No card found in transfer for {0}", card);
                    return;
                }
                else if (this.Transfer.Cards[card.MtgoId].CopiesOfCard != card.CopiesOfCard)
                {
                    _logger.InfoFormat("Card <{0}> not found with same number in preconfirmed card collection. {1}", card, this.Transfer.Cards[card.MtgoId]);
                    this.MagicOnlineInterface.CancelTradeFromConfirmationScreen();
                    return;
                }
            }

            foreach (MagicCard card in this.Transfer.Cards.Values)
            {
                if (!confirmedCards.ContainsKey(card.NumberId) || confirmedCards[card.NumberId].CopiesOfCard != card.CopiesOfCard)
                {
                    _logger.InfoFormat("Card <{0}> not found with same number in preconfirmed card collection.", card);
                    this.MagicOnlineInterface.CancelTradeFromConfirmationScreen();
                    return;
                }
            }

            _logger.Trace("Confirmation Screen Validation Finished.");

            CompleteConfirmationScreenTrade();

            if(this.Saved)
            {
                AutoItX.Sleep(15000);
                DatabaseInteractions.CompleteTransfer(this.Transfer);
            }
        }
    }
}