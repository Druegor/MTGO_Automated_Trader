using System.Collections.Generic;
using System.Linq;
using CardDataLayer.Models;
using Dapper;
using Framework.Interfaces;

namespace CardDataLayer.Repositories
{
	public class UserRepository
	{
		private readonly IConnection _connection;

		public UserRepository(IConnection connection)
		{
			_connection = connection;
		}

		public BotUser GetVisitor(int id)
		{
			return _connection.GetConnection().Query<BotUser>(
				"SELECT [TradeeId] as Id, [Name] " +
				"FROM Tradees " +
				"WHERE TradeeId = @id",
				new {id},
				_connection.GetTransaction()
				).Single();
		}

		public BotUser GetVisitor(string name)
		{
			return _connection.GetConnection().Query<BotUser>(
				"SELECT [TradeeId] as Id, [Name] " +
				"FROM Tradees " +
				"WHERE Name = @name",
				new { name },
				_connection.GetTransaction()
				).Single();
		}

		public List<PriceCheck> GetPriceChecks(int id)
		{
			return _connection.GetConnection().Query<PriceCheck>(
				"SELECT CardId, TimeChecked " +
				"FROM PriceChecks " +
				"WHERE TradeeId = @id",
				new { id },
				_connection.GetTransaction()
				).ToList();
		}

		public List<Trade> GetTrades(int id)
		{
			return _connection.GetConnection().Query<Trade>(
				"SELECT TradeId as Id, TradeeId, BotId, TimeOfTrade, CreditBeforeTrade " +
				"FROM Trades " +
				"WHERE TradeeId = @id",
				new { id },
				_connection.GetTransaction()
				).ToList();
		}

		public List<Transfer> GetTransfersByTradee(int id)
		{
			return _connection.GetConnection().Query<Transfer>(
				"SELECT TransferId, TradeBotId, TradeeId, WishList, Completed, TransferDate " +
				"FROM Transfers " +
				"WHERE TradeeId = @id",
				new { id },
				_connection.GetTransaction()
				).ToList();
		}

		public List<Credit> GetCredits(int id)
		{
			return _connection.GetConnection().Query<Credit>(
				"SELECT CreditId, TradeeId, BotGroupId, Credit as CreditAmount, NumOfTrades, LastTradeTime " +
				"FROM BotCredits " +
				"WHERE TradeeId = @id",
				new { id },
				_connection.GetTransaction()
				).ToList();
		}

		public void Save(string userName)
		{
			_connection.GetConnection().Execute(
				"INSERT INTO Tradees ([Name]) " +
				"VALUES (@@userName)",
				new {@userName},
				_connection.GetTransaction()
				);
		}
	}
}
