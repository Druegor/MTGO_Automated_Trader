using CardDataLayer;

namespace BusinessLogicLayer.MagicCards
{
	public class MagicCard
	{
		public int NumberId { get; private set; }
		public string Name { get; private set; }
		public string Set { get; private set; }
		public bool Premium { get; private set; }
		public decimal BuyPrice { get; private set; }
		public decimal SellPrice { get; private set; }
		public string Rarity { get; private set; }
		public int MtgoId { get; private set; }
		public int CopiesOfCard { get; set; }
		public int OwnedAmount { get; set; }
		public int CardSetNumber { get; set; }

		public MagicCard(int numberId, string name, string set, bool premium, decimal purchasePrice, decimal salePrice, string rarity, int mtgoId, int cardSetNumber) 
			: this(numberId, name, set, premium, purchasePrice, salePrice, rarity, mtgoId, 0, cardSetNumber) { }

		public MagicCard(MagicCard card) 
			: this(card.NumberId, card.Name, card.Set, card.Premium, card.BuyPrice, card.SellPrice, card.Rarity, card.MtgoId, card.OwnedAmount, card.CardSetNumber) { }

		public MagicCard(Card card, int ownedAmount) 
			: this(card.CardId, card.CardName, card.CardSet, card.Premium, -1, 999, card.Rarity.Name, card.MtgoCardId.GetValueOrDefault(0), ownedAmount, card.CardSetNumber.GetValueOrDefault(0)) { }

		public MagicCard(int numberId, string name, string set, bool premium, decimal purchasePrice, decimal salePrice, string rarity, int mtgoId, int ownedAmount, int cardSetNumber)
		{
			this.NumberId = numberId;
			this.Name = name;
			this.Set = set;
			this.Premium = premium;
			this.BuyPrice = purchasePrice;
			this.SellPrice = salePrice;
			this.Rarity = rarity;
			this.MtgoId = mtgoId;
			this.OwnedAmount = ownedAmount;
			this.CardSetNumber = cardSetNumber;
		}
		
		public void SetMtgoId(int mtgoId)
		{
			this.MtgoId = mtgoId;
		}

		public override string ToString()
		{
			return
				string.Format(
					"{3} Name: {0} Rarity: {1} Set: {2} BuyPrice: {4} SellPrice: {5} MtgoId: {6} OwnedAmount: {7} DbId: {8} Copies: {9} CardSetId: {10}",
					this.Name, this.Rarity, this.Set,
					this.Premium ? "*" : string.Empty, this.BuyPrice, this.SellPrice, this.MtgoId, this.OwnedAmount,
					this.NumberId, this.CopiesOfCard, this.CardSetNumber);
		}

		public void ChangeBuyPrice(double price)
		{
			this.BuyPrice = (decimal)price;
		}
	}
}
