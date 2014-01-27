using Dapper;
using Framework.Interfaces;

namespace CardDataLayer.Models
{
	public class MagicCard
	{
		public int BotId { get; set; }
		public int BotGroupId { get; set; }
		public int Id { get; set; }
		public string Name { get; set; }
		public string Set { get; set; }
		public bool Premium { get; set; }
		public decimal BuyPrice { get; set; }
		public decimal SellPrice { get; set; }
		public CardRarity Rarity { get; set; }
		public int MtgoId { get; set; }
		public int OwnedAmount { get; set; }
		public int CardSetNumber { get; set; }
		public int CopiesOfCard { get; set; }

		public MagicCard(int id, string cardName, string cardSet, bool premium, decimal buyPrice, decimal sellPrice, CardRarity rarity, int mtgoId, int ownedAmount, int cardSetNumber, int copiesOfCard, int botId, int botGroupId)
		{
			Id = id;
			Name = cardName;
			Set = cardSet;
			Premium = premium;
			BuyPrice = buyPrice;
			SellPrice = sellPrice;
			Rarity = rarity;
			MtgoId = mtgoId;
			OwnedAmount = ownedAmount;
			CardSetNumber = cardSetNumber;
			CopiesOfCard = copiesOfCard;
			BotId = botId;
			BotGroupId = botGroupId;
		}

		public MagicCard() : this(string.Empty, string.Empty, false, CardRarity.C)
		{}

		public MagicCard(string cardName, string cardSet, bool premium, CardRarity rarity)
			: this(0, cardName, cardSet, premium, 0, 0, rarity, 0, 0, 0, 0, 0, 0)
		{}

		public override string ToString()
		{
			return
				string.Format(
					"{3} Name: {0} Rarity: {1} Set: {2} BuyPrice: {4} SellPrice: {5} MtgoId: {6} OwnedAmount: {7} DbId: {8} CardSetId: {9}",
					Name, Rarity, Set, Premium ? "*" : string.Empty, BuyPrice, SellPrice, MtgoId, OwnedAmount, Id,
					CardSetNumber);
		}

		public MagicCard Clone()
		{
			return (MagicCard) MemberwiseClone();
		}

		public void Save(IConnection connection)
		{
			if (Id == 0)
			{
				connection
				.GetConnection()
				.Execute(
					"INSERT INTO Cards ([CardName],[CardSet],[CardRarity],[Premium],[MtgoCardId],[CardSetNumber]) " +
					"VALUES(@Name, @Set, @Rarity, @Premium, @MtgoId, @CardSetNumber) ",
					this,
					connection.GetTransaction());

				connection
				.GetConnection()
				.Execute(
					"INSERT INTO Prices ([BotGroupId],[CardId],[BuyPrice],[SellPrice],[OwnedAmount]) " +
					"VALUES(@BotGroupId, @Id, @BuyPrice, @SellPrice, @OwnedAmount) ",
					this,
					connection.GetTransaction());
			}
			else
			{
				connection
					.GetConnection()
					.Execute(
						"UPDATE Cards " +
						"SET CardName = @Name, " +
						"CardSet = @Set, " +
						"CardRarity = @Rarity, " +
						"Premium = @Premium, " +
						"MtgoCardId = @MtgoId " +
						"WHERE CardId = @Id ",
						this,
						connection.GetTransaction());

				connection
					.GetConnection()
					.Execute(
						"UPDATE Prices " +
						"SET BuyPrice = @BuyPrice, " +
						"SellPrice = @SellPrice, " +
						"OwnedAmount = @OwnedAmount " +
						"WHERE BotGroupId = @BotGroupId " +
						"AND CardId = @Id",
						this,
						connection.GetTransaction());
			}
		}
	}
}