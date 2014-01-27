using System;
using System.Collections.Generic;

namespace CardDataLayer.Models
{
	public class Trade
	{
		public int TradeId { get; set; }
		public int TradeeId { get; set; }
		public int BotId { get; set; }
		public DateTime TimeOfTrade { get; set; }
		public decimal CreditBeforeTrade { get; set; }

		public List<MagicCard> ReceivedCards { get; set; }
		public List<MagicCard> SentCards { get; set; }
	}
}