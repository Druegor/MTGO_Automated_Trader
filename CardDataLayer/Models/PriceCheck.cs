using System;

namespace CardDataLayer.Models
{
	public class PriceCheck
	{
		public int PriceCheckId { get; set; }
		public int TradeeId { get; set; }
		public int CardId { get; set; }
		public DateTime TimeChecked { get; set; }
	}
}