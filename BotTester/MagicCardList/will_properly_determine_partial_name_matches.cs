using System.Collections.Generic;
using BusinessLogicLayer;
using BusinessLogicLayer.Interfaces;
using Machine.Specifications;

namespace BotTester.MagicCardList
{
    [Subject(typeof(BusinessLogicLayer.MagicCards.MagicCardList))]
    public class will_properly_determine_partial_name_matches
    {
        private Establish context = () =>
                                        {
                                            _rarities = new[] {RaritySet.Rare.ToString(), RaritySet.Mythic.ToString()};
                                            _magicCardList = new BusinessLogicLayer.MagicCards.MagicCardList();
                                        };

        private It should_return_Mutavault = () => GetName("M utava ult;0]").ShouldEqual("Mutavault");
        private It should_return_Knight_Exemplar = () => GetName("Knight Exemplar").ShouldEqual("Knight Exemplar");
        private It should_return_Archive_Trap = () => GetName("ArchiveTrap").ShouldEqual("Archive Trap");
        private It should_return_Weirding_Shaman = () => GetName("Weirding Sham").ShouldEqual("Weirding Shaman");
        private It should_return_Sorin_Markov = () => GetName("Sorin Marko`1234").ShouldEqual("Sorin Markov");
        private It should_return_Arcbound_Reclaimer = () => GetName("Archound Reclaimer").ShouldEqual("Arcbound Reclaimer");

        private It should_return_Null = () =>
                                            {
                                                try
                                                {
                                                    GetName("This is a card that does not exist.", new[] {"ZEN"});
                                                    false.ShouldBeTrue();
                                                }
                                                catch
                                                {
                                                    true.ShouldBeTrue();
                                                }
                                            };

        private static string GetName(string match, params string [] setNames)
        {
            return _magicCardList
                .GetCardViaSuggestion(match, new List<string>(setNames), _rarities)
                .Name;
        }

        private static string[] _rarities;
        private static IMagicCardList _magicCardList;
    }
}
