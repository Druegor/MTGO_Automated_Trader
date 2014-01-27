using System.Collections.Generic;
using System.Linq;
using CardDataLayer.Models;
using Dapper;
using Framework.Interfaces;

namespace CardDataLayer.Repositories
{
	public class MagicCardRepository
	{
		private readonly IConnection _connection;

		public MagicCardRepository(IConnection connection)
		{
			_connection = connection;
		}


		public MagicCard GetCardForBotGroup(int groupId, string cardName, string set, bool premium)
		{
			return _connection.GetConnection().Query<MagicCard>(
				"SELECT BotCardId as Id, c.CardName as Name, c.CardSet as [Set], c.Premium, p.BuyPrice, p.SellPrice, " +
				"c.CardRarity as Rarity, c.MtgoCardId as MtgoId, bc.OwnedAmount, c.CardSetNumber, b.BotGroupId, 0 as BotId " +
				"FROM Cards c " +
				"INNER JOIN Prices p on c.CardId = p.CardId AND c.CardName = @cardName AND c.CardSet = @set AND c.Premium = @premium " +
				"INNER JOIN BotGroup b on p.BotGroupId = b.BotGroupId AND b.BotGroupId = @groupId ",
				new { groupId, cardName, set, premium },
				_connection.GetTransaction())
				.FirstOrDefault();
		}

		public BotCard GetCardForBot(int botId, int cardId)
		{
			return _connection.GetConnection().Query<BotCard>(
				"SELECT [BotCardId],[BotId],[CardId],[OwnedAmount] " +
				"FROM [MagicOnlineBot].[dbo].[BotCards] " +
				"WHERE [BotId] = @botId AND [CardId] = @cardId",
				new { botId, cardId },
				_connection.GetTransaction())
				.FirstOrDefault();
		}

		public IList<MagicCard> GetAllCards()
		{
			return _connection.GetConnection().Query<MagicCard>(
				"SELECT CardId as Id, c.CardName as Name, c.CardSet as [Set], c.Premium, 0 as BuyPrice, 0 as SellPrice, " +
				"c.CardRarity as Rarity, c.MtgoCardId as MtgoId, 0 as OwnedAmount, c.CardSetNumber, 0 as BotId, 0 as BotGroupId " +
				"FROM Cards c " +
				"WHERE c.CardSetNumber > 0 AND c.MtgoCardId IS NOT NULL", 
				null,
				_connection.GetTransaction())
				.ToList();
		}

		public IList<MagicCard> GetForBot(int botId)
		{
			return _connection.GetConnection().Query<MagicCard>(
				"SELECT BotCardId as Id, c.CardName as Name, c.CardSet as [Set], c.Premium, p.BuyPrice, p.SellPrice, " +
				"c.CardRarity as Rarity, c.MtgoCardId as MtgoId, bc.OwnedAmount, c.CardSetNumber, @botId as BotId, tb.BotGroupId " +
				"FROM BotCards bc " +
				"INNER JOIN Cards c on bc.CardId = c.CardId AND c.CardSetNumber > 0 AND c.MtgoCardId IS NOT NULL " +
				"INNER JOIN Prices p on bc.CardId = p.CardId " +
				"INNER JOIN BotGroup b on p.BotGroupId = b.BotGroupId " +
				"INNER JOIN TradeBots tb on b.BotGroupId = tb.BotGroupId AND bc.BotId = tb.BotId " +
				"WHERE bc.BotId = @botId AND bc.OwnedAmount > 4 ",
				new {botId},
				_connection.GetTransaction())
				.ToList();
		}

		public IList<MagicCard> GetCardsInSetForBot(int botId, string set)
		{
			return _connection.GetConnection().Query<MagicCard>(
				"SELECT BotCardId as Id, c.CardName as Name, c.CardSet as [Set], c.Premium, p.BuyPrice, p.SellPrice, " +
				"c.CardRarity as Rarity, c.MtgoCardId as MtgoId, bc.OwnedAmount, c.CardSetNumber, @botId as BotId, tb.BotGroupId " +
				"FROM BotCards bc " +
				"INNER JOIN Cards c on bc.CardId = c.CardId AND c.CardSetNumber > 0 AND c.MtgoCardId IS NOT NULL AND c.CardSet = @set " +
				"INNER JOIN Prices p on bc.CardId = p.CardId " +
				"INNER JOIN BotGroup b on p.BotGroupId = b.BotGroupId " +
				"INNER JOIN TradeBots tb on b.BotGroupId = tb.BotGroupId AND bc.BotId = tb.BotId " +
				"WHERE bc.BotId = @botId AND bc.OwnedAmount > 4 ",
				new { botId, set },
				_connection.GetTransaction())
				.ToList();
		}

		public IList<string> GetSetsForBot(int botId)
		{
			return _connection.GetConnection().Query<string>(
				"SELECT DISTINCT c.CardSet " +
				"FROM BotCards bc " +
				"INNER JOIN Cards c on bc.CardId = c.CardId AND c.CardSetNumber > 0 AND c.MtgoCardId IS NOT NULL " +
				"INNER JOIN Prices p on bc.CardId = p.CardId " +
				"INNER JOIN BotGroup b on p.BotGroupId = b.BotGroupId " +
				"INNER JOIN TradeBots tb on b.BotGroupId = tb.BotGroupId AND bc.BotId = tb.BotId " +
				"WHERE bc.BotId = @botId AND bc.OwnedAmount > 4 ",
				new { botId },
				_connection.GetTransaction())
				.ToList();
		}

		public IList<MagicCard> GetAllCards(string botName)
		{
			return _connection.GetConnection().Query<MagicCard>(
				"SELECT p.CardId as Id, c.CardName as Name, c.CardSet as [Set], c.Premium, p.BuyPrice, p.SellPrice, " +
				"c.CardRarity as Rarity, c.MtgoCardId as MtgoId, p.OwnedAmount, c.CardSetNumber, tb.BotId, tb.BotGroupId " +
				"FROM Prices p " +
				"INNER JOIN Cards c on p.CardId = c.CardId " +
				"INNER JOIN BotGroup b on p.BotGroupId = b.BotGroupId " +
				"INNER JOIN TradeBots tb on b.BotGroupId = tb.BotGroupId AND tb.Name = @botName ",
				new { botName },
				_connection.GetTransaction())
				.ToList();
		}

		public MagicCard Get(MagicCard card)
		{
			return _connection.GetConnection().Query<MagicCard>(
				"SELECT CardId as Id, c.CardName as Name, c.CardSet as [Set], c.Premium, 0 as BuyPrice, 0 as SellPrice, " +
				"c.CardRarity as Rarity, c.MtgoCardId as MtgoId, 0 as OwnedAmount, c.CardSetNumber, @BotId as BotId, @BotGroupId as BotGroupId " +
				"FROM Cards c " +
				"WHERE c.MtgoCardId = @MtgoId OR (c.CardName = @Name AND c.CardSet = @Set AND c.Premium = @Premium)", 
				card,
				_connection.GetTransaction())
				.Single();
		}

		public void Save(MagicCard magicCard)
		{
			_connection
				.GetConnection()
				.Execute(
					"INSERT INTO Cards ([CardName],[CardSet],[CardRarity],[Premium],[MtgoCardId],[CardSetNumber]) " +
					"VALUES(@Name, @Set, @Rarity, @Premium, @MtgoId, @CardSetNumber) ",
					magicCard,
					_connection.GetTransaction());
		}

		public void Update(MagicCard magicCard)
		{
			_connection
				.GetConnection()
				.Execute(
					"UPDATE Cards " +
					"SET CardName = @Name, " +
					"CardSet = @Set, " +
					"CardRarity = @Rarity, " +
					"Premium = @Premium, " +
					"MtgoCardId = @MtgoId " +
					"WHERE CardId = @Id ",
					magicCard,
					_connection.GetTransaction());
		}
	}
}
