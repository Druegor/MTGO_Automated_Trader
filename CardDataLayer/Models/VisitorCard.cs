using System;
using Dapper;
using Framework.Interfaces;

namespace CardDataLayer.Models
{
	public class VisitorCard
	{
		public int CardAvailableId { get; set; }
		public int TradeeId { get; set; }
		public int CardId { get; set; }
		public int NumberOfCard { get; set; }
		public DateTime FoundAt { get; set; }

		public void Save(IConnection connection)
		{
			connection
				.GetConnection()
				.Execute(
					"INSERT INTO TradeeCards ([TradeeId],[CardId],[NumberOfCard],[FoundAt]) " +
					"VALUES(@TradeeId, @CardId, @NumberOfCard, @FoundAt) ",
					this,
					connection.GetTransaction());
		}
	}
}
