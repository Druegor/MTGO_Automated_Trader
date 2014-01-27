using System;
using System.Collections.Generic;
using System.Diagnostics;
using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.Properties;
using Framework;

namespace BusinessLogicLayer.Trader
{
    public class RareBulkBuyer : TraderBase
    {
        private const string CompleteTradeMessage = "When you are ready to complete this trade please type 'done' and then press the 'Confirm' button.";
        private const string InitialMessage = "I am going to take as many rares and mythics as I can find from your collection.  This may result in more than one ticket per trade.";
        private const string SelectedAmountMessage = "I have selected {0} cards for {1} value.";
        private const string TakeTicketMessage = "Please take an appropriate number of tickets for this trade. All remainder will be credited to our next trade.";
        private const string PressConfirmMessage = "Please press CONFIRM.";
        private IMagicCardList _magicCardList = IoC.Resolve<IMagicCardList>();

        public RareBulkBuyer(List<CardSet> setSearchOrder, TimeSpan maxTradeTime, string botName, string tradee)
            : base(setSearchOrder, maxTradeTime, botName, tradee)
        {
            _magicCardList.SetBulkPrices(this.ApplicationSettings);
            this.MagicCardList.RaritySets = new[]
                                                {
                                                    RaritySet.Rare.ToString(),
                                                    RaritySet.Mythic.ToString()
                                                };
        }

        public override void StartTrade()
        {
            base.StartTrade();
            this.MessageHandler.SendMessage(InitialMessage);
            var youGetAmount = 0;
            var youGetAmountChanged = true;
            int loadCount = 0;
            const int maxLoads = 2;
            int iterations = this.ApplicationSettings.OwnedLessThan/maxLoads;
            while (youGetAmount < 75 && youGetAmountChanged && loadCount < maxLoads)
            {
                var wishListCards = _magicCardList.GetComprehensiveRaresAndMythics(this.ApplicationSettings.OwnedLessThan - iterations * loadCount, iterations);
                this.MagicOnlineInterface.WriteWishListFile(wishListCards);
                this.MagicOnlineInterface.LoadWishList();
                AutoItX.Sleep(5000);
                var pastAmount = youGetAmount;
                this.MagicOnlineInterface.DetermineYouGetAmount();
                youGetAmount = this.MagicOnlineInterface.YouGetAmount;
                youGetAmountChanged = pastAmount != youGetAmount;
                loadCount++;
            }

            this.MagicOnlineInterface.DetermineCardsYouGet();
            this.CardsYouGet.Set(this.MagicOnlineInterface.CardsYouGet);
            var sum = this.CardsYouGet.BuySum();
            var count = this.CardsYouGet.CountOfCards();
            this.MaxYouGiveAmount = sum + this.Credit;

            this.MessageHandler.SendMessage(string.Format(SelectedAmountMessage, count, sum));

            while (this.WinManager.InTrade() && CheckTimer())
            {
                string message = string.Empty;
                var secondTimer = new Stopwatch();
                secondTimer.Start();

                if(this.MaxYouGiveAmount >= 1)
                {
                    this.MessageHandler.SendMessage(string.Format(Resources.MoreThanATicketFound, this.MaxYouGiveAmount));
                    this.MessageHandler.SendMessage(TakeTicketMessage);
                }
                else
                {
                    this.MessageHandler.SendMessage(string.Format(Resources.LessThanATicketFound, count, sum, this.MaxYouGiveAmount));
                }

                this.MessageHandler.SendMessage(CompleteTradeMessage);

                do
                {
                    this.MagicOnlineInterface.DetermineIfYouGiveAmountChanged(true);
                    if (secondTimer.Elapsed.Seconds > 10)
                    {
                        secondTimer = new Stopwatch();
                        secondTimer.Start();
                        message = this.MessageHandler.GetLastTradeChatMessage();
                    }
                } while ((!message.Contains(TradeeName.ToLower()) || !message.Contains(Constants.DoneMessage)) && CheckTimer() && this.WinManager.InTrade());

                if (!this.WinManager.InTrade()) return;

                DetermineCardsYouGiveAmount();
                NewCredit = this.CardsYouGet.BuySum() - (this.CardsYouGive.SellSum() - Credit);

                if (CardsYouGet.BuySum() >= (this.CardsYouGiveAmount - Credit))
                {
                    this.MessageHandler.SendMessage(PressConfirmMessage);

                    do
                    {
                        this.MagicOnlineInterface.PressConfirmButton();
                        AutoItX.Sleep(2000);
                    } while (!this.WinManager.OnTradeConfirmScreen() && CheckTimer() && this.WinManager.OnTradeScreen());

                    if (!this.WinManager.InTrade()) return;

                    ConfirmationScreenValidationChecks();

                    if (!this.WinManager.InTrade()) return;

                    CompleteConfirmationScreenTrade();
                    return;
                }
                
                this.MessageHandler.SendMessage(string.Format(Resources.MaxTradeAmountExceeded));
                AutoItX.Sleep(1000);
            }

            if (!CheckTimer())
            {
                this.MagicOnlineInterface.CancelTrade();
            }
        }
    }
}
