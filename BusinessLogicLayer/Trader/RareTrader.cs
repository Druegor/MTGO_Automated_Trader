using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BusinessLogicLayer.Database;
using BusinessLogicLayer.MagicCards;
using CardDataLayer.Models;
using Framework;
using Framework.Connections;
using Framework.Interfaces;
using Framework.Logging;

namespace BusinessLogicLayer.Trader
{
	public class RareTrader : TraderBase
	{
		private readonly ILogger _logger = IoC.GetLoggerFor<RareTrader>();

		private const string ExaminingCollection = "Examining your collection. This takes approximately 1 minute, please be patient.  Afterwards I will list prices for all your selections.";
		private const string PleasePressConfirm = "Please press CONFIRM.";
		private const string SelectFewerMessage = "You have selected more tickets worth of cards/tickets than I found in your collection.  Please select {0} Tickets or less.";
		private const string YouGiveMessage = "YOU GIVE: {1} tickets ({0} total - {2} saved credits)";
		private const string CurrentCreditMessage = "You have {0} saved credits.  I will take {1} tickets worth of cards to cover the remaining cost of these cards.";
		private const string FutureCreditMessage = "After we've finished the trade, you will have {0} saved credits to use towards future purchases.";
		private const string SelectMoreMessage = "Please type 'done' when you are done selecting cards.";
		private const string ErrorSelectingCards = "It appears I have encountered an error, please take {0} tickets / cards or less for this trade and try again for any remainder.";
		private const string CollectionFoundMessage = "I have found {0} tickets worth of cards in your collection for this trade, so please select {1} tickets or less.  I will take only the cards required to meet the amount that you select from my account.";
		private const string NothingTradableMessage = "I do not sell cards I only trade cards for cards or purchase cards for tickets.  Please make more cards available and open the trade again.";

		public RareTrader(List<CardSet> setSearchOrder, TimeSpan maxTradeTime, string botName, string tradee)
			: base(setSearchOrder, maxTradeTime, botName, tradee)
		{
			MagicCardList.RaritySets = new[] {RaritySet.Rare, RaritySet.Mythic};
			TradeFilterHandler.Reset();
			TwoMinuteInactivityWarning = false;
		}

		public override void StartTrade()
		{
			base.StartTrade();

			if (!WinManager.InTrade()) return;

			MessageHandler.SendMessage(ExaminingCollection);

			if (!WinManager.InTrade()) return;

			SetFilters();
			GetTheirCollection(ApplicationSettings.OwnedLessThan);

			if (!WinManager.InTrade()) return;

			CardsYouGet = new MagicCardCollection();

			while (WinManager.InTrade() && CheckTimer())
			{
				string message;
				MagicOnlineInterface.DetermineIfMessageAreaHasChanged();
				//Watch for user selecting cards to price check
				do
				{
					message = string.Empty;
					MagicOnlineInterface.DetermineIfYouGiveAmountChanged(true);
					if (MagicOnlineInterface.DetermineIfMessageAreaHasChanged())
					{
						message = MessageHandler.GetLastTradeChatMessage();
					}
					AutoItX.Sleep(200);
				} while ((string.IsNullOrWhiteSpace(message) 
					|| !message.Contains(TradeeName.ToLower()) 
					|| !message.ToLower().Contains(Constants.DoneMessage.ToLower())) 
					&& CheckTimer() 
					&& WinManager.InTrade());

				if (!WinManager.InTrade()) return;

				DetermineCardsYouGiveAmount();

				//If I have already either choosen the cards to buy or they have lots of credit
				if (CardsYouGet.BuySum() >= (CardsYouGiveAmount - Credit) && CardsYouGiveAmount > 0)
				{
					if (CardsYouGet.CountOfCards() <= 0 || YouGiveAmountHasChanged)
					{
						SelectionHasChanged = false;
						ConfirmMessageOnly = true;
						SendPreConfirmMessage();
					}

					do
					{
						if (WinManager.OnTradeScreen() && CheckTimer())
						{
							MagicOnlineInterface.PressConfirmButton();
							AutoItX.Sleep(10000);
						}
					} while (!WinManager.OnTradeConfirmScreen() && CheckTimer() && WinManager.OnTradeScreen());

					if (!WinManager.InTrade()) return;

					ConfirmationScreenValidationChecks();

					if (!WinManager.InTrade()) return;

					CompleteConfirmationScreenTrade();
					return;
				}

				_logger.TraceFormat("CardsYouGiveAmount: {0}, MaxYouGiveAmount: {1}", CardsYouGiveAmount, MaxYouGiveAmount);
				if (CardsYouGiveAmount <= MaxYouGiveAmount && CardsYouGiveAmount > 0)
				{
					DetermineCardsYouTake();
					SendPreConfirmMessage();
					YouGiveAmountHasChanged = false;
				}
				else
				{
					MessageHandler.SendMessage(string.Format(SelectFewerMessage, MaxYouGiveAmount));
				}

				AutoItX.Sleep(1000);
			}

			if (!CheckTimer())
			{
				MagicOnlineInterface.CancelTrade();
			}
		}

		protected override void SendPreConfirmMessage()
		{
			var sb = new StringBuilder();
			if (SelectionHasChanged)
			{
				SelectionHasChanged = false;

				sb.AppendLine(string.Format(YouGiveMessage,
											CardsYouGive.SellSum(),
											CardsYouGive.SellSum() - Credit,
											Credit));

				MessageHandler.SendMessage(sb.ToString());

				sb = new StringBuilder();
				sb.AppendFormat(
					CurrentCreditMessage,
					Credit, CardsYouGet.BuySum());
				MessageHandler.SendMessage(sb.ToString());
			}

			NewCredit = CardsYouGet.BuySum() - (CardsYouGive.SellSum() - Credit);

			sb = new StringBuilder();
			sb.AppendFormat(FutureCreditMessage, NewCredit);
			MessageHandler.SendMessage(sb.ToString());

			sb = new StringBuilder();
			sb.AppendLine(!ConfirmMessageOnly ? SelectMoreMessage : PleasePressConfirm);
			MessageHandler.SendMessage(sb.ToString()); 
		}

		private void DetermineCardsYouTake()
		{
			var actualValue = CardsYouGiveAmount - Credit - CardsYouGet.BuySum();
			_logger.Trace("this.CardsYouGiveAmount - Credit - CardsYouGet.BuySum() = " + actualValue);
			var value = actualValue + 1;
			var purchaseCards = TheirCollection.ToList.Where(p => p.BuyPrice < value && p.CopiesOfCard > 0).OrderByDescending(p => p.BuyPrice);
			if (!purchaseCards.Any() || purchaseCards.Sum(p => p.BuyPrice * p.CopiesOfCard) < actualValue)
			{
				_logger.Trace("Found no cards with a buy price less than the value of their selection. Reverting to all cards.");
				purchaseCards = TheirCollection.ToList.Where(p => p.CopiesOfCard > 0).OrderByDescending(p => p.BuyPrice);
			}

			var remainingCount = TheirCollection.ToList.OrderByDescending(p => p.BuyPrice).Where(p => p.CopiesOfCard > 0).Take(MaxTradeAmount).Count();
			if (remainingCount > 0)
			{
				int youGetAmount;
				var loops = 0;
				do
				{
					youGetAmount = MagicOnlineInterface.YouGetAmount;
					actualValue = GetCardsForTrade(actualValue, purchaseCards, value);
					purchaseCards = TheirCollection.ToList.Where(p => p.CopiesOfCard > 0).OrderByDescending(p => p.BuyPrice);
					loops++;
				} while (purchaseCards.Any() &&
						 purchaseCards.Sum(p => p.BuyPrice*p.CopiesOfCard) >= value && actualValue > 0 &&
						 MagicOnlineInterface.YouGetAmount < MaxTradeAmount && WinManager.InTrade() &&
						 youGetAmount != MagicOnlineInterface.YouGetAmount);

				if(loops > 1)
				{
					_logger.Trace("Determining cards you get looped more than once. Loops: " + loops);
					PopulateCardsYouGet();
				}

				_logger.Trace("You get amount: " + CardsYouGet.BuySum());
				if ((CardsYouGiveAmount - Credit) > CardsYouGet.BuySum())
				{
					MaxYouGiveAmount = CardsYouGet.BuySum() + Credit;
					MessageHandler.SendMessage(string.Format(ErrorSelectingCards, MaxYouGiveAmount));
					MessageHandler.SendPriceMessage(CardsYouGet.ToList, false);
					return;
				}
			}
			MessageHandler.SendPriceMessage(CardsYouGet.ToList, false);
		}

		private decimal GetCardsForTrade(decimal actualValue, IOrderedEnumerable<MagicCard> purchaseCards, decimal runningValue)
		{
			var cardsSelected = new Dictionary<int, MagicCard>();
			var remainingCount = TheirCollection.ToList.Count(p => p.CopiesOfCard > 0);
			
			while (cardsSelected.Values.Sum(p => p.CopiesOfCard * p.BuyPrice) < actualValue && remainingCount > 0)
			{
				var card = GetCard(purchaseCards.ToList(), cardsSelected);
				_logger.Trace("Selecting Card: " + card);

				runningValue -= card.BuyPrice;
				purchaseCards = TheirCollection.ToList.Where(p => p.BuyPrice < runningValue && p.CopiesOfCard > 0).OrderByDescending(p => p.BuyPrice);
				remainingCount = TheirCollection.ToList.Count(p => p.CopiesOfCard > 0);

				while (!purchaseCards.Any() && remainingCount > 0 && purchaseCards.Sum(p => p.BuyPrice*p.CopiesOfCard) < runningValue)
				{
					runningValue += (decimal) .10;
					purchaseCards = TheirCollection.ToList.Where(p => p.BuyPrice < runningValue && p.CopiesOfCard > 0).OrderByDescending(p => p.BuyPrice);
				}
			}

			MagicOnlineInterface.WriteWishListFile(cardsSelected);
			MagicOnlineInterface.LoadWishList();

			AutoItX.Sleep(5000);
			MagicOnlineInterface.DetermineYouGetAmount();

			if (MagicOnlineInterface.YouGetAmount != CardsYouGet.CountOfCards())
			{
				PopulateCardsYouGet();
			}

			return CardsYouGiveAmount - Credit - CardsYouGet.BuySum();
		}

		private void PopulateCardsYouGet()
		{
			MagicOnlineInterface.DetermineCardsYouGet();
			CardsYouGet = new MagicCardCollection();
			foreach (var cardYouGet in MagicOnlineInterface.CardsYouGet.Values)
			{
				CardsYouGet[cardYouGet.Id] = cardYouGet;
			}
		}

		private MagicCard GetCard(IList<MagicCard> purchaseCards, Dictionary<int, MagicCard> cardsSelected)
		{
			var buyPrice = purchaseCards.Select(p => p.BuyPrice).Max();
			var purchaseCard = purchaseCards.FirstOrDefault(p => p.BuyPrice == buyPrice);

			if (purchaseCard == null)
			{
				return null;
			}
			
			TheirCollection[purchaseCard.Id].CopiesOfCard--;

			//Have to create a new instance of the card object or else it modifies the Copies of card for all instances
			AddCardToDictionary(MagicCardList.NewCard(purchaseCard), CardsYouGet.ToDictionary);
			AddCardToDictionary(MagicCardList.NewCard(purchaseCard), cardsSelected);

			return purchaseCard;
		}

		/// <summary>
		/// Goes through the List View of their collection to get the first 25 Mythic and Rares to be used to price out trades quickly.
		/// </summary>
		internal override void GetTheirCollection(int ownLessThan)
		{
			TheirCollection = new MagicCardCollection();

			LessThanValue = ownLessThan;
			TradeFilterHandler.SetOwnedFilterNumberValue(LessThanValue);
			
			AddToTheirCollection(GetPartOfTheirCollection(CardSet.AllCards, RaritySet.Mythic).Values);
			AddToTheirCollection(GetPartOfTheirCollection(CardSet.AllCards, RaritySet.Rare).Values);
			
			var sum =
				TheirCollection.ToList.OrderByDescending(p => p.BuyPrice).Take(MaxTradeAmount).Sum(
					p => p.BuyPrice*p.CopiesOfCard);

			MaxYouGiveAmount = sum + Credit;
			MessageHandler.SendPriceMessage(
				TheirCollection.ToList.OrderByDescending(p => p.BuyPrice).Take(MaxTradeAmount).ToList(), false);

			MessageHandler.SendMessage(
				string.Format(
					CollectionFoundMessage,
					sum, MaxYouGiveAmount));

			if (MaxYouGiveAmount == 0)
			{
				MessageHandler.SendMessage(NothingTradableMessage);
				AutoItX.Sleep(5000);
				if (WinManager.InTrade())
				{
					MagicOnlineInterface.CancelTrade();
				}
				return;
			}

			var databaseInteractions = new DatabaseInteractions();
			using (var connection = Connection.NewConnection())
			{
				connection.BeginTransaction();
				foreach (var card in TheirCollection.ToList)
				{
					databaseInteractions.AddCardForTrader(TradeeName, card.Id, card.CopiesOfCard, connection);
				}

				connection.CommitTransaction();
			}
		}

		internal override void SetFilters()
		{
			TradeFilterHandler.PickLessThanFilter();
			AutoItX.Sleep(500);
			AutoItX.DoubleClick(Pbv.TradeWindowListViewButton);
			AutoItX.Sleep(500);
			TradeFilterHandler.PickVersion(VersionSet.Regular);
		}
	}
}
