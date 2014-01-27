using System.Collections.Generic;
using System.Linq;
using CardDataLayer.Models;
using Dapper;
using Framework.Interfaces;

namespace CardDataLayer.Repositories
{
	public class BotRepository
	{
		private readonly IConnection _connection;

		public BotRepository(IConnection connection)
		{
			_connection = connection;
		}

		public TradeBot GetBot(int botId)
		{
			return _connection.GetConnection().Query<TradeBot>(
				"SELECT [BotId] as Id, [Name], [BotGroupId] as GroupId, [SetupDate], [MonthlyFee], [AuthorizationToken], [Running] " +
				"FROM TradeBots tb " +
				"WHERE tb.BotId = @botId",
				new {botId},
				_connection.GetTransaction()
				).Single();
		}

		public TradeBot GetBot(string botName)
		{
			return _connection.GetConnection().Query<TradeBot>(
				"SELECT [BotId] as Id, [Name], [BotGroupId] as GroupId, [SetupDate], [MonthlyFee], [AuthorizationToken], [Running] " +
				"FROM TradeBots tb " +
				"WHERE tb.Name = @botName",
				new { botName },
				_connection.GetTransaction()
				).Single();
		}

		public void Update(TradeBot bot)
		{
			_connection.GetConnection().Execute(
				"UPDATE TradeBots " +
				"SET [Name] = @name, [BotGroupId] = @groupId, [SetupDate] = @setupDate, [MonthlyFee] = @monthlyFee, [AuthorizationToken] = @authorizationToken, [Running] = @running " +
				"FROM TradeBots tb " +
				"WHERE botId = @id",
				bot,
				_connection.GetTransaction()
				);
		}

		public List<TradeBot> GetBots(int groupId)
		{
			return _connection.GetConnection().Query<TradeBot>(
				"SELECT [BotId] as Id, [Name], [BotGroupId] as GroupId, [SetupDate], [MonthlyFee], [AuthorizationToken], [Running] " +
				"FROM TradeBots tb " +
				"WHERE tb.BotGroupId = @groupId",
				new { groupId },
				_connection.GetTransaction()
				).ToList();
		}
	}
}
