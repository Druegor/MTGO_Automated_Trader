using System.Collections.Generic;
using CardDataLayer.Models;

namespace BusinessLogicLayer.Interfaces
{
	public interface IMagicCardList
	{
		IEnumerable<MagicCard> AllCardsInSystem { get; }
		CardRarity[] RaritySets { get; set; }
		void InvalidateCache();
		List<MagicCard> GetCard(string name);
		MagicCard GetCard(string name, string set);
		MagicCard GetCard(string name, string set, CardRarity[] rarities);
		MagicCard NewCard(MagicCard magicCard);
		MagicCard GetPriceForUpdate(string name, string set, bool premium);
		//MagicCard GetCardViaSuggestion(string name, IEnumerable<string> sets);
		MagicCard GetCardViaSuggestion(string name, List<string> sets, params CardRarity[] rarity);
		Dictionary<int, MagicCard> GetComprehensiveCommonsAndUncommons(int ownedLessThan, int copiesOfCard);
		IEnumerable<string> FilterSet(string set);
		MagicCard GetCard(int mtgoId);
		Dictionary<int, MagicCard> GetComprehensiveRaresAndMythics(int ownedLessThan, int copiesOfCard);
		void SetBulkPrices(IApplicationSettings applicationSettings);
		MagicCard GetCardByMtgoId(int mtgoId);
	}
}