namespace CardDataLayer.Models
{
	public class Price
	{
		public int PriceId { get; set; }
		public int BotGroupId { get; set; }
		public int CardId { get; set; }
		public decimal BuyPrice { get; set; }
		public decimal SellPrice { get; set; }
		public int OwnedAmount { get; set; }
	}
}
