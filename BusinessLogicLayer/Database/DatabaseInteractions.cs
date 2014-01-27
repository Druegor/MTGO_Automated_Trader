using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLogicLayer.Models;
using CardDataLayer.Models;
using CardDataLayer.Repositories;
using Framework;
using Framework.Connections;
using Framework.Interfaces;
using Framework.Logging;

namespace BusinessLogicLayer.Database
{
	//TODO split this apart into its functional areas, will probably still need to pass around the datacontext to do saves.
	public class DatabaseInteractions
	{
		private static readonly ILogger Logger = IoC.GetLoggerFor<DatabaseInteractions>();
		private readonly BotRepository _botRepository = IoC.Resolve<BotRepository>();
		private readonly UserRepository _userRepository = IoC.Resolve<UserRepository>();
		private readonly MagicCardRepository _cardRepository = IoC.Resolve<MagicCardRepository>();

		public bool CheckIsBotRunning(string botName)
		{
			return _botRepository.GetBot(botName).Running;
		}

		public void SetBotRunningStatus(string botName, bool runningStatus)
		{
			Logger.TraceFormat("Update bot running status for {0} to {1}", botName, runningStatus);
			var bot = _botRepository.GetBot(botName);
			bot.Running = runningStatus;
			_botRepository.Update(bot);
		}

		public void SavePriceChecksForUser(string userName, List<MagicCard> cards)
		{
			var tradee = GetTradee(userName);
			tradee
				.PriceChecks
				.AddRange(
					cards
						.Select(magicCard =>
						        new PriceCheck
						        	{
						        		CardId = magicCard.Id,
										TimeChecked = DateTime.Now
						        	}));


		}

		public int RemainingPriceChecks(string userName)
		{
			var tradee = GetTradee(userName);
			var priceChecks =
				tradee.PriceChecks.Count(
					p => p.TimeChecked > DateTime.Now.AddMinutes(-5));

			return 70 - priceChecks;
		}

		public void AddCardForTrader(string userName, int cardId, int copiesOfCard, IConnection connection)
		{
			var tradee = GetTradee(userName);
			var card = new VisitorCard
				{
					CardId = cardId,
					TradeeId = tradee.Id,
					NumberOfCard = copiesOfCard,
					FoundAt = DateTime.Now
				};

			card.Save(connection);
		}

		public Credit GetTradeCredits(string userName, string botName)
		{
			var bot = _botRepository.GetBot(botName);
			var tradee = GetTradee(userName);
			var botCredit = tradee
				                .Credits
				                .FirstOrDefault(p => p.BotGroupId == bot.GroupId)
			                ??
			                new Credit
				                {
					                BotGroupId = bot.GroupId,
					                CreditAmount = 0,
					                NumOfTrades = 0,
				                };

			botCredit.NumOfTrades++;
			botCredit.LastTradeTime = DateTime.Now;
			botCredit.Save();

			Logger.TraceFormat("Botname: {0}, CreditBeforeTrade: {1}, NumberOfTrades: {2}", botName, botCredit.CreditAmount, botCredit.NumOfTrades);
			return botCredit;
		}

		public void SaveCompletedTradeLog(string botName, string userName, List<MagicCard> youGet, List<MagicCard> youGive, decimal creditBeforeTrade, decimal creditAfterTrade)
		{
			Logger.TraceFormat("Botname: {0}, CreditBeforeTrade: {1}, CreditAfterTrade: {2}", botName, creditBeforeTrade, creditAfterTrade);

			var bot = _botRepository.GetBot(botName);
			var tradee = GetTradee(userName);
			var trade = new Trade
							{
								BotId = bot.Id,
								CreditBeforeTrade = creditBeforeTrade,
								TimeOfTrade = DateTime.Now,
								TradeeId = tradee.Id
							};

			UpdateForCardsYouGive(bot, youGive, trade);
			UpdateForCardsYouGet(bot, trade, youGet);

			var tradeCredit = tradee.Credits.First(p => p.BotGroupId == bot.GroupId);
			tradeCredit.CreditAmount = creditAfterTrade;
			tradeCredit.Save();
		}

		public List<BotSetting> GetSettingsForBot(string botName)
		{
			return _botRepository.GetBot(botName).Settings;
		}

		private BotUser GetTradee(string userName)
		{
			return _userRepository.GetVisitor(userName) ?? AddNewTradee(userName);
		}

		private BotUser AddNewTradee(string userName)
		{
			_userRepository.Save(userName);
			return _userRepository.GetVisitor(userName);
		}

		private void UpdateForCardsYouGet(TradeBot bot, Trade trade, IEnumerable<MagicCard> youGet)
		{
			using (IConnection connection = Connection.NewConnection())
			{
				connection.BeginTransaction();

				foreach (var mCard in youGet)
				{
					var magicCard = mCard;

					var card = _cardRepository.Get(magicCard);
					Logger.Trace("You get - " + magicCard + " OwnedAmount: " + card.OwnedAmount);
					card.OwnedAmount += magicCard.CopiesOfCard;
					card.Save(connection);

					var botCard = _cardRepository.GetCardForBot(bot.Id, magicCard.Id);
					if (botCard == null)
					{
						botCard = new BotCard {BotId = bot.Id, CardId = magicCard.Id};
						botCard.Save(connection);
						botCard.OwnedAmount = 0;
					}

					botCard.OwnedAmount += magicCard.CopiesOfCard;
					botCard.Save(connection);

					var detail = new TradeDetail
						{
							Buying = true,
							CardId = magicCard.Id,
							NumberOfCard = magicCard.CopiesOfCard,
							Price = magicCard.BuyPrice,
							TradeId = trade.TradeId
						};

					detail.Save(connection);
				}

				connection.CommitTransaction();
			}
		}

		private void UpdateForCardsYouGive(TradeBot bot, IEnumerable<MagicCard> youGive, Trade trade)
		{
			using (IConnection connection = Connection.NewConnection())
			{
				connection.BeginTransaction();
				foreach (var mCard in youGive)
				{
					var magicCard = mCard;

					Logger.Trace("You give - " + magicCard + " OwnedAmount: " + magicCard.OwnedAmount);
					magicCard.OwnedAmount -= magicCard.CopiesOfCard;
					magicCard.Save(connection);

					var botCard = _cardRepository.GetCardForBot(bot.Id, magicCard.Id);
					botCard.OwnedAmount -= magicCard.CopiesOfCard;
					botCard.Save(connection);

					var detail = new TradeDetail
						{
							Buying = false,
							CardId = magicCard.Id,
							NumberOfCard = magicCard.CopiesOfCard,
							Price = magicCard.SellPrice
						};

					detail.Save(connection);
				}

				connection.CommitTransaction();
			}
		}

		public bool AreBotsInSameGroup(string botName, string tradee)
		{
			var bot = _botRepository.GetBot(botName);
			var otherBot = _botRepository.GetBot(tradee);
			return otherBot != null && bot.GroupId == otherBot.GroupId;
		}

		public string GetOtherBotNames(string botName, bool onlineOnly = false)
		{
			var bot = _botRepository.GetBot(botName);
			var bots = _botRepository.GetBots(bot.GroupId).Where(p => p.Id != bot.Id && (bot.Running || bot.Running == onlineOnly)).ToList();
			return !bots.Any() ? string.Empty : string.Join(", ", bots.Select(p => p.Name));
		}

		public List<Transfer> GetTransfers(string botName)
		{
			var bot = _botRepository.GetBot(botName);
			return bot == null ? new List<Transfer>() : bot.Transfers.Where(p => !p.Completed).ToList();
		}

		public void ClearOwnedAmountsForBot(int botId)
		{
			// Execute against db to just zero out the owned amount for the bot in the bot cards table
		}

		public void CompleteTransfer(TransferModel transfer)
		{
			var trans = dataContext.Transfers.Single(p => p.TransferId == transfer.Transfer.TransferId);
			trans.Completed = true;
			trans.TransferDate = DateTime.Now;
		}
	}
}
