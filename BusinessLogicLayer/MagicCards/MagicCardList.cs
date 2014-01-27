using System.Collections.Generic;
using System.Linq;
using BusinessLogicLayer.Interfaces;
using CardDataLayer.Models;
using CardDataLayer.Repositories;
using Framework;
using Framework.Extensions;
using Framework.Logging;

namespace BusinessLogicLayer.MagicCards
{
	public class MagicCardList : IMagicCardList
	{
		private readonly ILogger _logger = IoC.GetLoggerFor<MagicCardList>();
		private readonly MagicCardRepository _repository = IoC.Resolve<MagicCardRepository>();
		private readonly IApplicationSettings _applicationSettings = IoC.Resolve<IApplicationSettings>();

		public MagicCardList()
		{
			PopulateAllCards();
		}

		private List<MagicCard> _allCardsInSystem;
		public IEnumerable<MagicCard> AllCardsInSystem
		{
			get
			{
				if (_allCardsInSystem == null || !_allCardsInSystem.Any())
				{
					PopulateAllCards();
				}
				return _allCardsInSystem;
			}
		}

		private CardRarity[] _allRarities = new[] {CardRarity.C, CardRarity.U, CardRarity.R, CardRarity.M};

		public CardRarity[] RaritySets
		{
			get { return _allRarities; }
			set { _allRarities = value; }
		}

		private void PopulateAllCards()
		{
			_logger.Trace("Starting population of cards in memory.");
			_allCardsInSystem = _repository.GetAllCards().ToList();
			_logger.Trace("Ending population of cards in memory.");
		}

		public void InvalidateCache()
		{
			PopulateAllCards();
		}

		public List<MagicCard> GetCard(string name)
		{           
			var cards = AllCardsInSystem.Where(p => !p.Premium && p.Name.ToLower() == name.ToLower()).ToList();
			if(cards.Count == 0)
			{
				var maxDiff = name.Length/2 - 1;
				var otherCards = AllCardsInSystem.Where(p => !p.Premium && p.Name.Length + maxDiff >= name.Length).ToList();

				cards = new List<MagicCard>
							{
								(from magicCard in otherCards
								 let difference = Levenshtein.Compute(magicCard.Name.ToLower(), name.ToLower())
								 where difference < maxDiff - 1
								 select magicCard).FirstOrDefault()
							};
			}
			return cards;
		}

		public MagicCard GetCard(string name, string set)
		{
			return GetCard(name, set, _allRarities);
		}

		public MagicCard GetCard(string name, string set, CardRarity [] rarities)
		{
			var sets = FilterSet(set.ToUpper()).ToList();
			_logger.TraceFormat("Getting card {0}, from sets [{1}]", name, sets.JoinToString());
			var card = AllCardsInSystem.FirstOrDefault(p => !p.Premium && sets.Contains(p.Set) && p.Name == name);
			return NewCard(card ?? GetCardViaSuggestion(name, sets, rarities));
		}

		public MagicCard NewCard(MagicCard magicCard)
		{
			if (magicCard != null)
			{
				var newCard = magicCard.Clone();
				newCard.CopiesOfCard = 1;
				return newCard;
			}

			_logger.Error("Card was not found.");
			return null;
		}

		public MagicCard GetPriceForUpdate(string name, string set, bool premium)
		{
			var card = AllCardsInSystem
				.FirstOrDefault(p =>
								p.Premium == premium
								&& p.Set == set
								&& p.Name == name);

			return card ?? GetPriceViaSuggestion(name, set, premium);
		}

		public IEnumerable<string> FilterSet(string set)
		{
			switch (set)
			{
				case "18":
				case "1N":
					return new[]{"IN"};
				case "75":
					return new[]{"7E", "PS", "TE"};
				case "105":
				case "1OE":
				case "IOE":
				case "I0E":
					return new[]{"10E"};
				case "5115":
					return new[]{"EVE"};
				case "5116":
					return new[]{"EVG"};
				case "550":
				case "BED":
				case "SED":
				case "8E0":
				case "850":
				case "85O":
					return new[]{"8ED"};
				case "QED":
				case "950":
					return new[]{"9ED"};
				case "1105":
				case "305":
				case "R0E":
					return new[]{"ROE"};
				case "TDO":
					return new[]{"TD0"};
				case "541":
				case "M1":
					return new[]{"MI"};
				case "5453":
					return new[]{"ME3"};
				case "51":
					return new[]{"EX"};
				case "111":
				case "V1":
					return new[]{"VI"};
				case "5410":
				case "M1O":
				case "MI0":
				case "MIO":
				case "MID":
				case "MLO":
					return new[]{"M10"};
				case "SDN":
					return new[] {"5DN"};
				case "SOI(":
					return new[] {"SOK"};
				default:
					return new[]{set};
			}
		}

		public MagicCard GetCard(int cardId)
		{
			return AllCardsInSystem.Single(p => p.Id == cardId).Clone();
		}

		public MagicCard GetCardByMtgoId(int mtgoId)
		{
			return AllCardsInSystem.Single(p => p.MtgoId == mtgoId).Clone();
		}

//        public MagicCard GetCardViaSuggestion(string name, IEnumerable<string> sets)
//        {
//            _allRarities = new[] { RaritySet.Common.ToString(), RaritySet.Uncommon.ToString(), RaritySet.Rare.ToString(), RaritySet.Mythic.ToString() };
//            return GetCardViaSuggestion(name, sets, _allRarities);
//        }

		public MagicCard GetCardViaSuggestion(string name, List<string> sets, params CardRarity [] rarity)
		{
			_logger.WarnFormat("Cardname {0} not found in system, starting search through sets {1}.", name, sets.JoinToString());

			int maxDiff = name.Length/2 - 1;
			var maxLenDiff = (maxDiff - 1);

			int maxLength = name.Length + maxLenDiff;

			var cardViaSuggestion =
				GetResultFromSuggestion(name, maxDiff,
										AllCardsInSystem
											.Where(p =>
												   !p.Premium
												   && sets.Contains(p.Set)
												   && rarity.Contains(p.Rarity)
												   && p.Name.StartsWith(name[0].ToString())
												   && p.Name.Length <= maxLength
												   && p.Name.Length + maxLenDiff >= name.Length))

				?? GetResultFromSuggestion(name, maxDiff,
										   AllCardsInSystem
											   .Where(p =>
													  !p.Premium
													  && sets.Contains(p.Set)
													  && rarity.Contains(p.Rarity)
													  && p.Name.Length <= maxLength
													  && p.Name.Length + maxLenDiff >= name.Length))

				?? GetResultFromSuggestion(name, maxDiff,
										AllCardsInSystem
											.Where(p =>
												   !p.Premium
												   && sets.Contains(p.Set)
												   && p.Name.StartsWith(name[0].ToString())
												   && p.Name.Length <= maxLength
												   && p.Name.Length + maxLenDiff >= name.Length))

				?? GetResultFromSuggestion(name, maxDiff,
										   AllCardsInSystem
											   .Where(p =>
													  !p.Premium
													  && rarity.Contains(p.Rarity)
													  && p.Name.Length <= maxLength
													  && p.Name.Length + maxLenDiff >= name.Length));

			_logger.Trace(cardViaSuggestion != null ? "Card found: " + cardViaSuggestion.Name : "Card not found for: " + name);

			return cardViaSuggestion;
		}

		private MagicCard GetResultFromSuggestion(string name, int maxDiff, IEnumerable<MagicCard> cards)
		{
			cards = cards.ToList();

			_logger.TraceFormat(
				"{0} Cards found with a maxDiff in length of {1} starting with the same letter as {2} Cards: {3}",
				cards.Count(), maxDiff, name[0], cards.JoinToString());

			var cardDiffList = cards
				.Select(p => new {NameDiff = Levenshtein.Compute(p.Name, name), MtgoCard = p.Clone()})
				.Where(p => p.NameDiff < maxDiff)
				.ToList();
			
			if(!cardDiffList.Any())
			{
				return null;
			}

			return cardDiffList
				.OrderBy(p => p.NameDiff)
				.First()
				.MtgoCard;
		}

		internal MagicCard GetPriceViaSuggestion(string name, string set, bool premium)
		{
			_logger.WarnFormat("Price for card {0} not found in system, starting search through set: {1} premium: {2}.",
							  name, set, premium);

			var maxLenDiff = name.Length/2 - 2;
			var cards = AllCardsInSystem.Where(p =>
											 p.Premium == premium
											 && p.Set == set
											 && p.Name.StartsWith(name[0].ToString())
											 && p.Name.Length + maxLenDiff >= name.Length);

			MagicCard priceViaSuggestion = (from magicCard in cards
										let difference = Levenshtein.Compute(magicCard.Name, name)
										where difference <= maxLenDiff
										select magicCard).FirstOrDefault();

			if (priceViaSuggestion != null)
			{
				_logger.Trace("Card found: " + priceViaSuggestion.Name);
			}

			return priceViaSuggestion;
		}

		public Dictionary<int, MagicCard> GetComprehensiveCommonsAndUncommons(int ownedLessThan, int copiesOfCard)
		{
			var excludeCards = AllCardsInSystem
				.Where(p => p.OwnedAmount > ownedLessThan)
				.Select(p => p.Name)
				.Distinct();

			var cardsForWishList =
				AllCardsInSystem
					.Where(p => (p.Rarity == CardRarity.C || p.Rarity == CardRarity.U)
								&& p.OwnedAmount < ownedLessThan
								&& !p.Premium
								&& p.MtgoId > 0
								&& p.Name != "Plains"
								&& p.Name != "Island"
								&& p.Name != "Forest"
								&& p.Name != "Mountain"
								&& p.Name != "Swamp"
								&& excludeCards.All(c => c != p.Name)
					)
					.OrderByDescending(p => p.Id)
					.Select(p => p.Clone())
					.ToList();

			var cardDictionary = new Dictionary<int, MagicCard>();

			foreach (var magicCard in cardsForWishList)
			{
				magicCard.CopiesOfCard = copiesOfCard;
				cardDictionary[magicCard.Id] = magicCard;
			}

			return cardDictionary;
		}
		
		public Dictionary<int, MagicCard> GetComprehensiveRaresAndMythics(int ownedLessThan, int copiesOfCard)
		{
			var excludeCards = AllCardsInSystem
				.Where(p => p.OwnedAmount >= ownedLessThan + 1)
				.Select(p => p.Name)
				.Distinct();

			var cardsForWishList =
				AllCardsInSystem
					.Where(p => ((p.Rarity == CardRarity.R && p.SellPrice > (1/(decimal) _applicationSettings.Rares))
					             || (p.Rarity == CardRarity.M && p.SellPrice > (1/(decimal) _applicationSettings.Mythics)))
					            && p.OwnedAmount < ownedLessThan
					            && !p.Premium
					            && p.MtgoId > 0
					            && p.Set != "PRM"
					            && excludeCards.All(c => c != p.Name)
					)
					.OrderByDescending(p => p.SellPrice)
					.ThenByDescending(p => p.Id)
					.Select(p => p.Clone())
					.ToList();

			var cardDictionary = new Dictionary<int, MagicCard>();

			foreach (var magicCard in cardsForWishList)
			{
				magicCard.CopiesOfCard = copiesOfCard;
				cardDictionary[magicCard.Id] = magicCard;
			}

			return cardDictionary;
		}

		private bool _pricesSet;

		public void SetBulkPrices(IApplicationSettings applicationSettings)
		{
			if (_pricesSet) return;

			SetBuyPriceForList(applicationSettings.Commons, CardRarity.C);
			SetBuyPriceForList(applicationSettings.Uncommons, CardRarity.U);
			SetBuyPriceForList(applicationSettings.Rares, CardRarity.R);
			SetBuyPriceForList(applicationSettings.Mythics, CardRarity.M);

			_pricesSet = true;
		}

		private void SetBuyPriceForList(double quantityPerTicket, CardRarity rarity)
		{
			var cards = AllCardsInSystem.Where(p => p.Rarity == rarity);
			var price = 1/quantityPerTicket;

			foreach (var magicCard in cards)
			{
				magicCard.BuyPrice = (decimal)price;
			}
		}
	}
}
