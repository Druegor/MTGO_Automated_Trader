using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using BusinessLogicLayer.Database;
using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.MagicCards;
using CardDataLayer.Models;
using Framework;
using Framework.Logging;

namespace BusinessLogicLayer.Trader
{
	public class TraderBase : ITrader
	{
		private readonly ILogger _logger = IoC.GetLoggerFor<TraderBase>();
		private bool _warningFiveMinutesRemainingSent;
		private bool _warningTwoMinutesRemainingSent;
		private bool _warningOneMinuteRemainingSent;
		private bool _warningOneMinuteOfInactivity;
		protected int MaxTradeAmount = 75;
		private const string WelcomeCreditMessage = "Welcome {0}, you have {1} Tickets credit from previous trades.";
		private const string ErrorDeterminingCardsYouGive = "I appear to have been unable to determine the cards you have selected so I will be exiting this trade, please try again later.";
		private const string PressConfirmMessage = "Please press CONFIRM.";
		private const string ThankYouMesage = "Thank you for shopping.  Be sure to add me to your buddy list and please come again!";
		protected bool TwoMinuteInactivityWarning = true;

		protected TraderBase(List<CardSet> setSearchOrder, TimeSpan maxTradeTime, string botName, string tradee)
		{
			MagicCardList = IoC.Resolve<IMagicCardList>();
			SetSearchOrder = setSearchOrder;
			MaxTradeTime = maxTradeTime;
			MagicOnlineInterface = IoC.Resolve<ITradeHandler>();
			Pbv = IoC.Resolve<IPixelBasedVariables>();
			ApplicationSettings = IoC.Resolve<IApplicationSettings>();
			WinManager = IoC.Resolve<IWindowManager>();
			MessageHandler = IoC.Resolve<IMessageHandler>();
			TradeFilterHandler = IoC.Resolve<ITradeFilterHandler>();
			MagicOnlineInterface.YouGiveAmountChangedEvent += ExamineYouGiveCards;
			BotName = botName;
			TimeInTrade = new Stopwatch();
			TimeInTrade.Start();
			TimeSinceLastAction = new Stopwatch();
			CardsYouGive = new MagicCardCollection();
			CardsYouGet = new MagicCardCollection();
			TheirCollection = new MagicCardCollection();
			SelectionHasChanged = false;
			ConfirmMessageOnly = false;
			TradeeName = tradee;
			_logger.Trace("Tradee set to: " + tradee);
		}

		protected IMagicCardList MagicCardList { get; set; }
		protected Stopwatch TimeInTrade { get; set; }
		protected Stopwatch TimeSinceLastAction { get; set; }
		protected string TradeeName { get; set; }
		protected string LastMessageInTradeConversation { get; set; }
		protected MagicCardCollection CardsYouGet { get; set; }
		protected MagicCardCollection CardsYouGive { get; set; }
		protected TimeSpan MaxTradeTime { get; set; }
		protected ITradeHandler MagicOnlineInterface { get; set; }
		protected IWindowManager WinManager { get; set; }
		protected IMessageHandler MessageHandler { get; set; }
		protected ITradeFilterHandler TradeFilterHandler { get; set; }
		protected string BotName { get; set; }
		protected decimal Credit { get; set; }
		protected decimal NewCredit { get; set; }
		protected decimal MaxYouGiveAmount { get; set; }
		protected decimal CardsYouGiveAmount { get; set; }
		protected bool YouGiveAmountHasChanged { get; set; }
		protected List<CardSet> SetSearchOrder { get; set; }
		protected IPixelBasedVariables Pbv { get; set; }
		protected MagicCardCollection TheirCollection { get; set; }
		protected int LessThanValue { get; set; }
		protected const int FindXCardsFromTheirCollection = 70;
		protected bool TradeCancelled { get; set; }
		protected IApplicationSettings ApplicationSettings { get; set; }
		protected bool SelectionHasChanged { get; set; }
		protected bool ConfirmMessageOnly { get; set; }
		protected bool Saved { get; set; }
		
		/// <summary>
		/// Accept button has already been pressed so now:
		/// - The MessageBox and Type Columns adjusted
		/// - The Filter Needs to be Set
		/// - The Credits for the Tradee displayed
		/// - The Collection needs to be read
		/// </summary>
		public virtual void StartTrade()
		{
			if (string.IsNullOrWhiteSpace(TradeeName)) return;

			Credit = DatabaseInteractions.GetTradeCredits(TradeeName, BotName).Credit;
			MessageHandler.SendMessage(string.Format(WelcomeCreditMessage, TradeeName, Credit));
			MessageHandler.SendMessage(string.Format("Also be sure to visit our other bots: " + ApplicationSettings.OtherBots));
			
			TimeSinceLastAction.Start();
		}

		private void ExamineYouGiveCards(object sender, EventArgs e)
		{
			SelectionHasChanged = true;
			DoTradeChecks();
			MagicOnlineInterface.DetermineUsersSelection();
			CardsYouGive.Set(MagicOnlineInterface.CardsYouGive);
			YouGiveAmountHasChanged = true;
			TimeSinceLastAction = new Stopwatch();
			TimeSinceLastAction.Start();
			_warningOneMinuteOfInactivity = false;
			MagicOnlineInterface.DetermineIfMessageAreaHasChanged();
		}

		protected void DetermineCardsYouGiveAmount()
		{
			DoTradeChecks();
			YouGiveAmountHasChanged = true;
			MagicOnlineInterface.DetermineUsersSelection();
			CardsYouGive.Set(MagicOnlineInterface.CardsYouGive);
			CardsYouGiveAmount = CardsYouGive.SellSum();
			TimeSinceLastAction = new Stopwatch();
			TimeSinceLastAction.Start();
			_warningOneMinuteOfInactivity = false;
			if(CardsYouGive.CountOfCards() != MagicOnlineInterface.YouGiveAmount)
			{
				MessageHandler.SendMessage(ErrorDeterminingCardsYouGive);
				MagicOnlineInterface.CancelTrade();
			}
		}

		protected bool CheckTimer()
		{
			var timeLeft = MaxTradeTime - TimeInTrade.Elapsed;
			var oneMinute = new TimeSpan(0, 1, 0);
			var twoMinutes = new TimeSpan(0, 2, 0);
			var fourMinutes = new TimeSpan(0, 4, 0);
			var fiveMinutes = new TimeSpan(0, 5, 0);

			if(timeLeft < fiveMinutes && !_warningFiveMinutesRemainingSent )
			{
				MessageHandler.SendMessage("Five minutes remaining for this trade.");
				_warningFiveMinutesRemainingSent = true;
			}

			if (timeLeft < twoMinutes && !_warningTwoMinutesRemainingSent)
			{
				MessageHandler.SendMessage("Two minutes remaining for this trade.");
				_warningTwoMinutesRemainingSent = true;
			}

			if (timeLeft < oneMinute && !_warningOneMinuteRemainingSent)
			{
				MessageHandler.SendMessage("One minutes remaining for this trade.");
				_warningOneMinuteRemainingSent = true;
			}

			if(TimeSinceLastAction.Elapsed > (TwoMinuteInactivityWarning ? twoMinutes : fiveMinutes))
			{
				_logger.Trace("Cancelling trade do to inactivity for user: " + TradeeName);
				var sb = new StringBuilder();
				sb.AppendLine("Your inactivity has caused this trade to be cancelled.");
				MessageHandler.SendMessage(sb.ToString());
				MagicOnlineInterface.CancelTrade();
				return false;
			}

			if(TwoMinuteInactivityWarning && TimeSinceLastAction.Elapsed > oneMinute && !_warningOneMinuteOfInactivity)
			{
				MessageHandler.SendMessage(
					"You have not had any activity for one minute. After two minutes of inactivity the trade will be cancelled.");
				_warningOneMinuteOfInactivity = true;
			}
			else if (!TwoMinuteInactivityWarning && TimeSinceLastAction.Elapsed > fourMinutes && !_warningOneMinuteOfInactivity)
			{
				MessageHandler.SendMessage(
					"You have not had any activity for four minutes. After five minutes of inactivity the trade will be cancelled.");
				_warningOneMinuteOfInactivity = true;
			}

			if (TimeInTrade.Elapsed > MaxTradeTime)
			{
				_logger.Trace("Cancelling trade do to time expired for user: " + TradeeName);
				var sb = new StringBuilder();
				sb.AppendLine("The time for this trade has expired.");
				MessageHandler.SendMessage(sb.ToString());
				MagicOnlineInterface.CancelTrade();
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
			_logger.Trace("Save trade information.");
			DatabaseInteractions.SaveCompletedTradeLog(BotName, TradeeName, CardsYouGet.ToList,CardsYouGive.ToList, Credit, NewCredit);
		}

		protected void AddToTheirCollection(IEnumerable<MagicCard> collection)
		{
			foreach (MagicCard magicCard in collection)
			{
				_logger.TraceFormat("Adding card {0} to their collection.", magicCard);
				TheirCollection[magicCard.Id] = magicCard;
			}
		}

		protected bool CollectionCheck()
		{
			return TheirCollection.CountOfCards() < FindXCardsFromTheirCollection;
		}

		protected Dictionary<int, MagicCard> GetPartOfTheirCollection(CardSet cardSet, RaritySet rarity )
		{
			DoTradeChecks();
			TradeFilterHandler.PickRarity(rarity);
			TradeFilterHandler.PickCardSet(cardSet);
			return MagicOnlineInterface.ExamineCollection();
		}

		/// <summary>
		/// Ensure no other windows are interupting our interactions with the trade.
		/// </summary>
		protected void DoTradeChecks()
		{
			WinManager.CheckForOtherWindows();
		}

		protected void ConfirmationScreenValidationChecks()
		{
			DoTradeChecks();

			var youGiveConfirmedAmount = MagicOnlineInterface.DetermineYouGiveConfirmedAmount();
			if (youGiveConfirmedAmount != MagicOnlineInterface.YouGiveAmount)
			{
				MagicOnlineInterface.CancelTradeFromConfirmationScreen();
			}

			var confirmedCards = MagicOnlineInterface.ExamineYouGiveCardsOnConfirmedScreen();
			var youGiveCards = CardsYouGive.ToDictionary;
			
			if (confirmedCards.Count != youGiveCards.Count)
			{
				MagicOnlineInterface.CancelTradeFromConfirmationScreen();
				return;
			}

			if (confirmedCards.Values.Count != youGiveCards.Values.Count)
			{
				_logger.Error("Confirmation screen dictionaries do not match.");
				return;
			}

			foreach (var card in confirmedCards.Values.Where(card => !youGiveCards.ContainsKey(card.Id) || youGiveCards[card.Id].CopiesOfCard != card.CopiesOfCard))
			{
				_logger.InfoFormat("Card <{0}> not found with same number in preconfirmed card collection.", card);
				MagicOnlineInterface.CancelTradeFromConfirmationScreen();
				return;
			}

			foreach (var card in youGiveCards.Values.Where(card => !confirmedCards.ContainsKey(card.Id) || confirmedCards[card.Id].CopiesOfCard != card.CopiesOfCard))
			{
				_logger.InfoFormat("Card <{0}> not found with same number in preconfirmed card collection.", card);
				MagicOnlineInterface.CancelTradeFromConfirmationScreen();
				return;
			}

			_logger.Trace("Confirmation Screen Validation Finished.");
		}

		protected void CompleteConfirmationScreenTrade()
		{
			var sb = new StringBuilder();
			sb.AppendLine(ThankYouMesage);
			MessageHandler.SendMessage(sb.ToString());

			sb = new StringBuilder();
			sb.AppendLine("Also check out my other bots since all credit is shared: " + ApplicationSettings.OtherBots);
			MessageHandler.SendMessage(sb.ToString());

			sb = new StringBuilder();
			sb.AppendLine(PressConfirmMessage);
			MessageHandler.SendMessage(sb.ToString());

			do
			{
				_logger.Trace("Pressing second confirm button");
				MagicOnlineInterface.PressSecondConfirmButton();
				//DoTradeChecks();
				AutoItX.Sleep(500);
			} while (WinManager.OnTradeConfirmScreen());

			_logger.Trace("Trade confirmation screen has disappeared.");
			if (CardsYouGet.CountOfCards() > 0)
			{
				_logger.Trace("New card screen should appear if not cancelled searching...");
				var i = 0;
				while (!Pbv.NewCards.CheckForOCRValue(false) && !Pbv.TradeCancelled.CheckForOCRValue(false))
				{
					AutoItX.Sleep(500);
					if (i++ > 30) return; //Cancelled in confirm screen doesn't seem to show cancelled message
				}
			}
			else
			{
				_logger.Trace("Trade complete screen or cancelled screen should appear searching...");
				while (!Pbv.TradeComplete.CheckForOCRValue(false) && !Pbv.TradeCancelled.CheckForOCRValue(false))
				{
					AutoItX.Sleep(500);
				}
			}

			if (Pbv.TradeCancelled.CheckForOCRValue()) return;

			_logger.Trace("Trade not cancelled apparently looking for Trade complete or New cards searching...");
			var timer = new Stopwatch();
			timer.Start();

			if (WinManager.CheckIfNewCardConfirmationScreen() || Pbv.TradeComplete.CheckForOCRValue() || timer.Elapsed.TotalSeconds > 20)
			{
				if (timer.Elapsed.TotalSeconds > 20) return;
				SaveTradeInformation();
				Saved = true;
			}
		}

		protected void AddCardToDictionary(MagicCard card, Dictionary<int, MagicCard> cardsYouGet)
		{
			if (card == null) return;

			_logger.TraceFormat("Adding card {0} to cards you get.", card);
			if (cardsYouGet.ContainsKey(card.Id))
			{
				cardsYouGet[card.Id].CopiesOfCard++;
			}
			else
			{
				cardsYouGet[card.Id] = card;
			}
		}

		internal virtual void GetTheirCollection(int ownLessThan) { }
		internal virtual void SetFilters() { }
		protected virtual void SendPreConfirmMessage() { }
	}
}