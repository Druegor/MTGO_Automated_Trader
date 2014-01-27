using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace AutoItBot
{
    public class TraderBase : ITrade
    {
        private bool warningFiveMinutesRemainingSent;
        private bool warningTwoMinutesRemainingSent;
        private bool warningOneMinuteRemainingSent;
        private bool warningOneMinuteOfInactivity;
        private bool inTrade;

        protected TraderBase(List<CardSet> setSearchOrder, TimeSpan maxTradeTime, string botName)
        {
            this.SetSearchOrder = setSearchOrder;
            this.MaxTradeTime = maxTradeTime;
            this.MagicOnlineInterface = new MtgoGui();
            this.PixelBasedVariables = this.MagicOnlineInterface.PixelBasedVariables;
            this.MagicOnlineInterface.YouGiveAmountChangedEvent += ExamineYouGiveCards;
            this.BotName = botName;
            this.TimeInTrade = new Stopwatch();
            this.TimeInTrade.Start();
            this.TimeSinceLastAction = new Stopwatch();
        }

        protected Stopwatch TimeInTrade { get; set; }
        protected Stopwatch TimeSinceLastAction { get; set; }
        protected string TradeeName { get; set; }
        protected string LastMessageInTradeConversation { get; set; }
        protected Dictionary<int, MagicCard> CardsYouGet { get; set; }
        protected List<MagicCard> CardsYouGive { get; set; }
        protected TimeSpan MaxTradeTime { get; set; }
        protected MtgoGui MagicOnlineInterface { get; set; }
        protected string BotName { get; set; }
        protected decimal Credit { get; set; }
        protected decimal NewCredit { get; set; }
        protected decimal MaxYouGiveAmount { get; set; }
        protected decimal CardsYouGiveAmount { get; set; }
        protected bool YouGiveAmountHasChanged { get; set; }
        protected List<CardSet> SetSearchOrder { get; set; }
        protected IPixelBasedVariables PixelBasedVariables { get; set; }
        protected Dictionary<int, MagicCard> TheirCollection { get; set; }
        protected int LessThanValue { get; set; }
        protected const int FindXCardsFromTheirCollection = 70;

        protected bool InTrade
        {
            get
            {
                this.MagicOnlineInterface.CloseTradeCancelledDialog();
                return this.inTrade && BotHomeScreen.RunningState && !this.MagicOnlineInterface.TradeCancelled;
            }
            set { this.inTrade = value; }
        }

        /// <summary>
        /// Accept button has already been pressed so now:
        /// - The MessageBox and Type Columns adjusted
        /// - The Filter Needs to be Set
        /// - The Credits for the Tradee displayed
        /// - The Collection needs to be read
        /// </summary>
        public virtual void StartTrade()
        {
            FindAndDockChatWindow();
            DetermineTradee();
            this.TimeSinceLastAction.Start();
        }

        private void FindAndDockChatWindow()
        {
            AutoItX.Sleep(2500);
            int exceptionCounter = 0;
            bool found;
            bool docked = false;
            this.InTrade = true;
            while (!docked && exceptionCounter++ < 30)
            {
                found = this.MagicOnlineInterface.FindTradeChatWindowAndDock();
                docked = !this.MagicOnlineInterface.FindTradeChatWindowAndDock();

                if (!found || docked) break;

                AutoItX.Sleep(200);
                DoTradeChecks();

                if (!InTrade) return;
            }
        }

        private void DetermineTradee()
        {
            if (!InTrade) return;

            this.MagicOnlineInterface.SendMessage(string.Format(
                "Loading bot settings..."));
            this.MagicOnlineInterface.SendMessage(string.Format(
                "Please feel free to let me know how things go at http://druegor.homeip.net"));

            if (!InTrade) return;

            this.TradeeName = this.MagicOnlineInterface.GetTradeeName(this.BotName);
            this.Credit = DatabaseInteractions.GetTradeCredits(this.TradeeName, this.BotName).Credit;
            this.MagicOnlineInterface.SendMessage(
                string.Format("Welcome {0}, you have {1} Tickets credit from previous trades.", this.TradeeName,
                              this.Credit));

            if (!InTrade) return;
        }

        private void ExamineYouGiveCards(object sender, EventArgs e)
        {
            this.MagicOnlineInterface.DetermineCardsYouGive();
            this.CardsYouGive = this.MagicOnlineInterface.CardsYouGive;
            this.YouGiveAmountHasChanged = true;
            this.TimeSinceLastAction = new Stopwatch();
            this.TimeSinceLastAction.Start();
            this.warningOneMinuteOfInactivity = false;
        }

        protected void DetermineCardsYouGiveAmount()
        {
            this.MagicOnlineInterface.DetermineYouGiveCards();
            this.CardsYouGive = this.MagicOnlineInterface.CardsYouGive;
            CardsYouGiveAmount = CardsYouGive.Sum(p => p.SellPrice * p.CopiesOfCard);
            if(CardsYouGive.Sum(p => p.CopiesOfCard) != this.MagicOnlineInterface.YouGiveAmount)
            {
                this.MagicOnlineInterface.SendMessage(
                    "I appear to have been unable to locate or determine all the cards you have selected so I will be exiting this trade, please try again later.");
                this.MagicOnlineInterface.CancelTrade();
            }
        }

        protected bool CheckTimer()
        {
            var timeLeft = this.MaxTradeTime - this.TimeInTrade.Elapsed;

            if(timeLeft < new TimeSpan(0, 5, 0) && !warningFiveMinutesRemainingSent )
            {
                this.MagicOnlineInterface.SendMessage("Five minutes remaining for this trade.");
                warningFiveMinutesRemainingSent = true;
            }

            if (timeLeft < new TimeSpan(0, 2, 0) && !warningTwoMinutesRemainingSent)
            {
                this.MagicOnlineInterface.SendMessage("Two minutes remaining for this trade.");
                warningTwoMinutesRemainingSent = true;
            }

            if (timeLeft < new TimeSpan(0, 1, 0) && !warningOneMinuteRemainingSent)
            {
                this.MagicOnlineInterface.SendMessage("One minutes remaining for this trade.");
                warningOneMinuteRemainingSent = true;
            }

            if(TimeSinceLastAction.Elapsed > new TimeSpan(0, 2, 0))
            {
                var sb = new StringBuilder();
                sb.AppendLine("The time for this trade has expired.");
                this.MagicOnlineInterface.SendMessage(sb.ToString());
                this.InTrade = false;
                this.MagicOnlineInterface.CancelTrade();
                return false;
            }

            if(TimeSinceLastAction.Elapsed > new TimeSpan(0, 1, 0) && !warningOneMinuteOfInactivity)
            {
                this.MagicOnlineInterface.SendMessage(
                    "You have not had any activity for one minute. After two minutes of inactivity the trade will be cancelled.");
                warningOneMinuteOfInactivity = true;
            }

            if (this.TimeInTrade.Elapsed > this.MaxTradeTime)
            {
                var sb = new StringBuilder();
                sb.AppendLine("The time for this trade has expired.");
                this.MagicOnlineInterface.SendMessage(sb.ToString());
                this.InTrade = false;
                this.MagicOnlineInterface.CancelTrade();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Saves the Time, Trade, You Get, You Give information for the trade into the database. 
        /// 
        /// Trade Table => TradeId, UserId, TimeSpentInTrade
        /// Traded Table => TradeId, CardId, PurchasedBit
        /// </summary>
        protected void SaveTradeInformation()
        {
            DatabaseInteractions.SaveCompletedTradeLog(this.BotName, this.TradeeName, this.CardsYouGet.Values.ToList(),
                                                       this.CardsYouGive, Credit, NewCredit);

            this.InTrade = false;
            AutoItX.MouseClick(PixelBasedVariables.CloseConversationAfterTrade);
            AutoItX.Sleep(1000);
            AutoItX.MouseClick(PixelBasedVariables.CloseConversationAfterTrade);
        }

        protected void AddToTheirCollection(IEnumerable<MagicCard> collection)
        {
            foreach (MagicCard magicCard in collection)
            {
                TheirCollection[magicCard.NumberId] = magicCard;
            }
        }

        protected bool CollectionCheck()
        {
            return this.TheirCollection.Values.Sum(p => p.CopiesOfCard) < FindXCardsFromTheirCollection;
        }

        protected void GetPartOfTheirCollection(CardSet cardSet, RaritySet rarity )
        {
            DoTradeChecks();
            if (!InTrade || !CollectionCheck())
            {
                return;
            }

            this.MagicOnlineInterface.PickCardSet(cardSet);
            AutoItX.Sleep(250);

            this.MagicOnlineInterface.PickRarity(rarity);
            if (CollectionCheck() && InTrade)
            {
                var collection = this.MagicOnlineInterface.ExamineCollection(FindXCardsFromTheirCollection, this.TheirCollection.Values.Sum(p => p.CopiesOfCard));
                AddToTheirCollection(collection);
            }
        }

        protected void DoTradeChecks()
        {           
            if (!string.IsNullOrEmpty(this.TradeeName))
            {
                this.MagicOnlineInterface.CheckForOtherWindows();
            }
        }

        protected static void AddCardToDictionary(MagicCard card, IDictionary<int, MagicCard> cardsYouGet)
        {
            if (cardsYouGet.ContainsKey(card.NumberId))
            {
                cardsYouGet[card.NumberId].CopiesOfCard++;
            }
            else
            {
                cardsYouGet[card.NumberId] = card;
            }
        }

        protected virtual void GetTheirCollection() { }
        protected virtual void SetFilters() { }
        protected virtual void SendPreConfirmMessage() { }


    }
}