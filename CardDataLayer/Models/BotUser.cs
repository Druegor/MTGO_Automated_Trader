using System.Collections.Generic;
using CardDataLayer.Repositories;
using Framework;

namespace CardDataLayer.Models
{
	public class BotUser
	{
		public int Id { get; set; }
		public string Name { get; set; }

		private List<PriceCheck> _priceChecks;
		public List<PriceCheck> PriceChecks
		{
			get { return _priceChecks ?? (_priceChecks = IoC.Resolve<UserRepository>().GetPriceChecks(Id)); }
		}

		private List<Trade> _trades; 
		public List<Trade> Trades
		{
			get { return _trades ?? (_trades = IoC.Resolve<UserRepository>().GetTrades(Id)); }
		}

		private List<Transfer> _transfers; 
		public List<Transfer> Transfers
		{
			get { return _transfers ?? (_transfers = IoC.Resolve<UserRepository>().GetTransfersByTradee(Id)); }
		}

		private List<Credit> _credits; 
		public List<Credit> Credits
		{
			get { return _credits ?? (_credits = IoC.Resolve<UserRepository>().GetCredits(Id)); }
		}
	}
}

