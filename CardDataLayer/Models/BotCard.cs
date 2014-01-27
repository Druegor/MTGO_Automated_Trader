using Dapper;
using Framework.Interfaces;

namespace CardDataLayer.Models
{
	public class BotCard
	{
		public int BotCardId { get; set; }
		public int BotId { get; set; }
		public int CardId { get; set; }
		public int OwnedAmount { get; set; }

		public void Save(IConnection connection)
		{
			if (BotCardId == 0)
			{
				connection
					.GetConnection()
					.Execute(
						"INSERT INTO BotCards ([BotId],[CardId],[OwnedAmount]) " +
						"VALUES(@BotId, @CardId, @OwnedAmount) ",
						this,
						connection.GetTransaction());
			}
			else
			{
				connection
					.GetConnection()
					.Execute(
						"UPDATE BotCards " +
						"SET [BotId] = @BotId," +
						"[CardId] = @CardId," +
						"[OwnedAmount] = @OwnedAmount " +
						"WHERE BotCardId = @BotCardId ",
						this,
						connection.GetTransaction());
			}
		}
	}
}
