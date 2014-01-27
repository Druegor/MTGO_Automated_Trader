using System.Collections.Generic;
using System.Linq;
using CardDataLayer.Models;

namespace BusinessLogicLayer.MagicCards
{
	public class MagicCardCollection
	{
		private Dictionary<int, MagicCard> _magicCards;
	
		public MagicCardCollection()
		{
			_magicCards = new Dictionary<int, MagicCard>();
		}

		public Dictionary<int, MagicCard> ToDictionary
		{
			get { return _magicCards; }
		}

		public List<MagicCard> ToList
		{
			get { return _magicCards.Values.ToList(); }
		}

		public void Set(Dictionary<int, MagicCard> cards)
		{
			_magicCards = cards;
		}

		public decimal BuySum()
		{
			return _magicCards.Values.Sum(p => p.CopiesOfCard*p.BuyPrice);
		}

		public decimal SellSum()
		{
			return _magicCards.Values.Sum(p => p.CopiesOfCard*p.SellPrice);
		}

		public int CountOfDistinctCards()
		{
			return _magicCards.Values.Count();
		}

		public int CountOfCards()
		{
			return _magicCards.Values.Sum(p => p.CopiesOfCard);
		}

		public MagicCard this[int index]
		{
			get { return _magicCards[index]; }
			set { _magicCards[index] = value; }
		}
	}
}
