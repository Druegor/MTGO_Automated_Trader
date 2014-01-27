using System;
using System.Collections.Generic;

namespace CardDataLayer.Models
{
	public class Visitor
	{
		public int TradeeId { get; set; }
		public string Name { get; set; }

		public IDictionary<DateTime, IList<MagicCard>> Cards { get; set; }
	}
}
