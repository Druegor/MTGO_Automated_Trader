using CardDataLayer.Models;
using Dapper;
using Framework.Interfaces;

namespace CardDataLayer.Repositories
{
	public class PriceRepository
	{
		private readonly IConnection _connection;

		public PriceRepository(IConnection connection)
		{
			_connection = connection;
		}

		public void Save(MagicCard magicCard)
		{
			_connection
				.GetConnection()
				.Execute(
					"INSERT INTO Prices ([BotGroupId],[CardId],[BuyPrice],[SellPrice],[OwnedAmount]) " +
					"VALUES(@BotGroupId, @Id, @BuyPrice, @SellPrice, @OwnedAmount) ",
					magicCard,
					_connection.GetTransaction());
		}

		public void Update(MagicCard magicCard)
		{
			_connection
				.GetConnection()
				.Execute(
					"UPDATE Prices " +
					"SET BuyPrice = @BuyPrice, " +
					"SellPrice = @SellPrice, " +
					"OwnedAmount = @OwnedAmount " +
					"WHERE BotGroupId = @BotGroupId " +
					"AND CardId = @Id",
					magicCard,
					_connection.GetTransaction());
		}
	}
}
