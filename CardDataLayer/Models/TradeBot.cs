using System;
using System.Collections.Generic;
using CardDataLayer.Repositories;
using Framework;

namespace CardDataLayer.Models
{
	public class TradeBot
	{
		private List<BotSetting> _settings;
		public int Id { get; set; }
		public string Name { get; set; }
		public int GroupId { get; set; }
		public DateTime SetupDate { get; set; }
		public decimal MonthlyFee { get; set; }
		public string AuthorizationToken { get; set; }
		public bool Running { get; set; }

		public List<BotSetting> Settings{get { return _settings ?? (_settings = IoC.Resolve<SettingsRepository>().Get(Id)); }}
	}
}