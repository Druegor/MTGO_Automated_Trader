using System.Collections.Generic;
using System.Linq;
using BusinessLogicLayer;
using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.MagicCards;
using Machine.Specifications;

namespace BotTester.MagicCardList
{
    [Subject(typeof(BusinessLogicLayer.MagicCards.MagicCardList))]
    public class excludes_cards_which_have_a_owned_amount_greater_than_the_owned_less_than_setting
    {
        Establish context = () =>
                                        {
                                            applicationSettings = new ApplicationSettings();
                                            cardList =
                                                new BusinessLogicLayer.MagicCards.MagicCardList().
                                                    GetComprehensiveCommonsAndUncommons(applicationSettings.OwnedLessThan,2);
                                        };

        It should_not_return_any_cards_with_count_more_than_setting = () => cardList.Values.Any(p => p.OwnedAmount >= applicationSettings.OwnedLessThan).ShouldBeFalse();

        private static Dictionary<int, MagicCard> cardList;
        private static IApplicationSettings applicationSettings;
    }
}