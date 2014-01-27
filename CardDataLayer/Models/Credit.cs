using System;
using Dapper;
using Framework.Connections;

namespace CardDataLayer.Models
{
	public class Credit
	{
		public int CreditId { get; set; }
		public int TradeeId { get; set; }
		public int BotGroupId { get; set; }
		public decimal CreditAmount { get; set; }
		public int NumOfTrades { get; set; }
		public DateTime LastTradeTime { get; set; }

		public void Save()
		{
			using(var connection = Connection.NewConnection())
			{
				if(CreditId == 0)
				{
					connection
						.GetConnection()
						.Execute(
							"INSERT INTO BotCredits([TradeeId],[BotGroupId],[Credit],[NumOfTrades],[LastTradeTime]) " +
							"VALUES (@TradeeId, @BotGroupId, @CreditAmount, @NumOfTrades, @LastTradeTime)", this);
				}
				else
				{
					connection
						.GetConnection()
						.Execute(
							"UPDATE BotCredits " +
							"SET Credit = @CreditAmount, " +
							"NumOfTrades = @NumOfTrades, " +
							"LastTradeTime = @LastTradeTime " +
							"WHERE CreditId = @CreditId", this);
				}
			}
		}
	}
}