using System;

namespace CardDataLayer.Models
{
    public class Transfer
    {
		public int TransferId { get; set; }
	    public int TradeBotId { get; set; }
        public int TradeeId { get; set; }
        public string WishList { get; set; }
        public bool Completed { get; set; }
        public DateTime TransferDate { get; set; }
    }
}
