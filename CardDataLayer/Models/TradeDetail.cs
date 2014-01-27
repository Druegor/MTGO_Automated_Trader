using Dapper;
using Framework.Interfaces;

namespace CardDataLayer.Models
{
	public class TradeDetail
	{
		public int TradeDetailId { get; set; }
		public int TradeId { get; set; }
		public int CardId { get; set; }
		public int NumberOfCard { get; set; }
		public decimal Price { get; set; }
		public bool Buying { get; set; }

		public void Save(IConnection connection)
		{
			connection
				.GetConnection()
				.Execute(
					"INSERT INTO [TradeDetail] ([TradeId],[CardId],[NumberOfCard],[Price],[Buying]) " +
					"VALUES(@TradeId, @CardId, @NumberOfCard, @Price, @Buying) ",
					this,
					connection.GetTransaction());
		}
	}
}
