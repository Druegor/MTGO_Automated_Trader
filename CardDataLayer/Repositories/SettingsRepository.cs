using System.Collections.Generic;
using System.Linq;
using CardDataLayer.Models;
using Dapper;
using Framework.Interfaces;

namespace CardDataLayer.Repositories
{
	public class SettingsRepository
	{
		private readonly IConnection _connection;

		public SettingsRepository(IConnection connection)
		{
			_connection = connection;
		}

		public List<BotSetting> Get(int botId)
		{
			return _connection.GetConnection().Query<BotSetting>(
				"SELECT [BotId] as Id, [Name], [BotGroupId] as GroupId, [SetupDate], [MonthlyFee], [AuthorizationToken], [Running] " +
				"FROM TradeBots tb " +
				"WHERE tb.BotId = @botId",
				new {botId},
				_connection.GetTransaction()
				)
				.ToList();
		}
	}
}
