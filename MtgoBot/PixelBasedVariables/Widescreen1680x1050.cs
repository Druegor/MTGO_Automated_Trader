using System.Collections.Generic;
using System.Drawing;
using AutoItBot.OCR;

namespace AutoItBot.PixelBasedVariables
{
    public class Widescreen1680x1050 : ScreenLocations
    {
        public Widescreen1680x1050()
        {
            #region Login Screen

            UserNameTextField = new Point(787, 514);
            PasswordTextField = new Point(787, 542);
            LogOnScreenText = "Username";

            #endregion

            #region Home Screen

            var homeScreenTopLeft = new Point(41, 546);
            var homeScreenBottomRight = new Point(559, 655);
            var buttonToOpenClassifiedsWindowFromHomeScreen = new Point(198, 595);

            HomeScreen = OcrImageArea.GetAreaFor(homeScreenTopLeft, homeScreenBottomRight, HomeScreenText, buttonToOpenClassifiedsWindowFromHomeScreen);


            #endregion

            #region Classifieds

            SubmitPostingTopLeft = new Point(1291, 122);
            SubmitPostingBottomRight = new Point(1436, 140);
            SubmitPostingText = "SUBMIT POSTING";

            EditPostingButtonTopLeft = new Point(1308, 50);
            EditPostingButtonBottomRight = new Point(1420, 68);
            EditPostingButtonText = "EDIT POSTING";

            ClassifiedMessageBox = new Point(220, 53);
            ClassifiedPostingButton = new Point(1353, 134);


            #endregion

            #region Trade Conversation and Chat Windows and Challenge Windows

            TradeConversationChangeInY = 226;
            TradeConversationChangeInX = 23;
            TradeConversationInitialLocation = new Point(1011, 60);

            PrivateChatCloseButtonLocation = new Point(1032, 60);

            TradeConversationUsername = new Point(1619, 825);
            TradeConversationMessageBox = new Point(1519, 770);

            var challengeTopLeft = new Point(738, 379);
            var challengeBottomRight = new Point(966, 410);
            var challengeDeclinePosition = new Point(907, 619);
            Challenge = OcrImageArea.GetAreaFor(challengeTopLeft, challengeBottomRight, ChallengeText, challengeDeclinePosition);


            #endregion

            #region Trade Request

            var tradeRequestTextTopLeft = new Point(710, 380);
            var tradeRequestTextBottomRight = new Point(975, 590);
            var tradeRequestYesButtonLocation = new Point(765, 617);
            var tradeCompleteButtonLocation = new Point(785, 617);

            TradeRequest = OcrImageArea.GetAreaFor(tradeRequestTextTopLeft, tradeRequestTextBottomRight, TradeRequestTextSearchString, tradeRequestYesButtonLocation);
            TradeComplete = OcrImageArea.GetAreaFor(tradeRequestTextTopLeft, tradeRequestTextBottomRight, TradeCompleteTextSearchString, tradeCompleteButtonLocation);

            #endregion

            TradeWindowListViewButton = new Point(1396, 98);

            YouGetFirstCardPosition = new Point(808, 821);
            YouGiveFirstCardPosition = new Point(114, 821);

            YouGetTypeColumnStartPosition = new Point(1144, 793);
            YouGetTypeColumnEndPosition = new Point(1028, 795);

            YouGiveTypeColumnStartPosition = new Point(436, 792);
            YouGiveTypeColumnEndPosition = new Point(319, 793);

            YouGetNumberOfCardsTopLeft = new Point(737, 760);
            YouGetNumberOfCardsBottomRight = new Point(822, 776);

            YouGetConfirmedNumberOfCardsTopLeft = new Point(24, 61);
            YouGetConfirmedNumberOfCardsBottomRight = new Point(124, 75);

            YouGiveNumberOfCardsTopLeft = new Point(29, 760);
            YouGiveNumberOfCardsBottomRight = new Point(114, 776);

            YouGiveConfirmedNumberOfCardsTopLeft = new Point(24, 521);
            YouGiveConfirmedNumberOfCardsBottomRight = new Point(124, 535);

            CardSelectedColor = 15658734;


            #region Trade Filters

            FilterBlankSpotPosition = new Point(1217, 20);
            FilterSetSelectorPosition = new Point(697, 17);
            FilterRaritySelectorPosition = new Point(541, 57);
            FilterVersionsSelectorPosition = new Point(686, 62);
            FilterOwnedSelectorPosition = new Point(816, 24);
            FilterResetButton = new Point(1033, 19);
            FilterOwnedTextBox = new Point(864, 20);

            #endregion

            ConfirmTradeFirstTimeTopLeft = new Point(424, 97);

            var confirmTradeSecondButtonTopLeft = new Point(387, 62);
            var confirmTradeSecondButtonBottomRight = new Point(513, 78);
            var confirmTradeSecondTimeButton = new Point(449, 70);
            ConfirmTradeSecondTime = OcrImageArea.GetAreaFor(confirmTradeSecondButtonTopLeft, confirmTradeSecondButtonBottomRight, ConfirmTradeText, confirmTradeSecondTimeButton);

            CancelTradeButton = new Point(575, 100);

            FilterSearchTextBoxPosition = new Point(217, 58);

            TheirCollectionFirstCardPosition = new Point(369, 160);

            CardsInCollectionListView = 22;
            CardsInGetOrGiveCollectionListView = 5;

            FirstCardInListViewNameTopLeft = new Point(22, 152);
            FirstCardInListViewNameBottomRight = new Point(231, 172);
            FirstCardInListViewNumberTopLeft = new Point(234, 152);
            FirstCardInListViewNumberBottomRight = new Point(284, 172);
            FirstCardInListViewSetTopLeft = new Point(392, 152);
            FirstCardInListViewSetBottomRight = new Point(474, 172);

            LastCardInListViewAfterArrayTopLeft = new Point(22, 651);
            LastCardInListViewAfterArrayBottomRight = new Point(231, 674);

            CardInListViewBottomRightX = new List<int>
                                             {
                                                           172,
                                                           196,
                                                           219,
                                                           242,
                                                           266,
                                                           289,
                                                           312,
                                                           335,
                                                           359,
                                                           382,
                                                           405,
                                                           428,
                                                           452,
                                                           475,
                                                           498,
                                                           521,
                                                           545,
                                                           568,
                                                           591,
                                                           615,
                                                           638,
                                                           661
                                                       };

            #region You Get Region

            YouGetFirstCardTopLeft = new Point(776, 814);
            YouGetFirstCardBottomRight = new Point(984, 834);

            YouGetSecondCardTopLeft = new Point(776, 835);
            YouGetSecondCardBottomRight = new Point(984, 858);

            YouGetThirdCardTopLeft = new Point(776, 859);
            YouGetThirdCardBottomRight = new Point(984, 881);

            YouGetFourthCardTopLeft = new Point(776, 882);
            YouGetFourthCardBottomRight = new Point(984, 904);

            YouGetFifthCardTopLeft = new Point(776, 905);
            YouGetFifthCardBottomRight = new Point(984, 928);

            YouGetAfterFifthCardTopLeft = new Point(776, 907);
            YouGetAfterFifthCardBottomRight = new Point(984, 930);

            YouGetCardSetTopLeft = new Point(1326, 814);
            YouGetCardSetBottomRight = new Point(1387, 834);

            YouGetCardNumberTopLeft = new Point(734, 814);
            YouGetCardNumberBottomRight = new Point(773, 834);

            YouGetCardBlankSpaceTopLeft = new Point(778, 814);
            YouGetCardBlankSpaceBottomRight = new Point(984, 817);

            YouGiveCardSetTopLeft = new Point(618, 814);

            YouGiveCardSetBottomRight = new Point(YouGetCardSetBottomRight.X - ListDifference,
                                                  YouGetFirstCardBottomRight.Y);

            YouGiveCardNumberTopLeft = new Point(YouGetCardNumberTopLeft.X - ListDifference,
                                                 YouGetFirstCardBottomRight.Y);

            YouGiveCardNumberBottomRight = new Point(YouGetCardNumberBottomRight.X - ListDifference,
                                                     YouGetFirstCardBottomRight.Y);

            ListDifference = 708;

            YouGetCardTopLocations =
                new List<Point>
                    {
                        YouGetFirstCardTopLeft,
                        YouGetSecondCardTopLeft,
                        YouGetThirdCardTopLeft,
                        YouGetFourthCardTopLeft,
                        YouGetFifthCardTopLeft,
                        YouGetAfterFifthCardTopLeft
                    };

            YouGetCardBottomLocations =
                new List<Point>
                    {
                        YouGetFirstCardBottomRight,
                        YouGetSecondCardBottomRight,
                        YouGetThirdCardBottomRight,
                        YouGetFourthCardBottomRight,
                        YouGetFifthCardBottomRight,
                        YouGetAfterFifthCardBottomRight
                    };

            YouGiveCardTopLocations =
                new List<Point>
                    {
                        new Point(YouGetFirstCardTopLeft.X - ListDifference, YouGetFirstCardTopLeft.Y),
                        new Point(YouGetSecondCardTopLeft.X - ListDifference, YouGetSecondCardTopLeft.Y),
                        new Point(YouGetThirdCardTopLeft.X - ListDifference, YouGetThirdCardTopLeft.Y),
                        new Point(YouGetFourthCardTopLeft.X - ListDifference, YouGetFourthCardTopLeft.Y),
                        new Point(YouGetFifthCardTopLeft.X - ListDifference, YouGetFifthCardTopLeft.Y),
                        new Point(YouGetAfterFifthCardTopLeft.X - ListDifference, YouGetAfterFifthCardTopLeft.Y),
                    };

            YouGiveCardBottomLocations =
                new List<Point>
                    {
                        new Point(YouGetFirstCardBottomRight.X - ListDifference, YouGetFirstCardBottomRight.Y),
                        new Point(YouGetSecondCardBottomRight.X - ListDifference, YouGetSecondCardBottomRight.Y),
                        new Point(YouGetThirdCardBottomRight.X - ListDifference, YouGetThirdCardBottomRight.Y),
                        new Point(YouGetFourthCardBottomRight.X - ListDifference, YouGetFourthCardBottomRight.Y),
                        new Point(YouGetFifthCardBottomRight.X - ListDifference, YouGetFifthCardBottomRight.Y),
                        new Point(YouGetAfterFifthCardBottomRight.X - ListDifference, YouGetAfterFifthCardBottomRight.Y),
                    };

            #endregion

            #region Trade Cancel Section

            var cancelTradeTopLeft = new Point(707, 444);
            var cancelTradeBottomRight = new Point(969, 497);
            var cancelButtonPosition = new Point(766, 618);
            ConfirmCancelTrade = OcrImageArea.GetAreaFor(cancelTradeTopLeft, cancelTradeBottomRight, CancelTradeText, cancelButtonPosition);


            var tradeSessionCancelledTopLeft = new Point(708, 458);
            var tradeSessionCancelledBottomRight = new Point(970, 477);
            var tradeSessionCancelledButton = new Point(837, 620);
            TradeCancelled = OcrImageArea.GetAreaFor(tradeSessionCancelledTopLeft, tradeSessionCancelledBottomRight, TradeSessionCancelledText, tradeSessionCancelledButton);

            #endregion

            LoadWishListButton = new Point(716, 102);
            WishListFirstFileLocation = new Point(640, 441);
            LoadDeckButton = new Point(654, 716);
            TradeConversationLastMessage = new Point(1560, 739);
            EditPostingButton = new Point(1361, 59);
            ClassifiedsTab = new Point(630, 1000);
            RemovePostingButton = new Point(1009, 58);
            DeckEditorTab = new Point(351, 998);

            SectionTitleTopLeft = new Point(60, 7);
            SectionTitleBottomRight = new Point(263, 31);

            var loadDeckTopLeft = new Point(755, 276);
            var loadDeckBottomRight = new Point(944, 310);
            var loadDeckCancelButton = new Point(1019, 715);
            CancelLoadDeck = OcrImageArea.GetAreaFor(loadDeckTopLeft, loadDeckBottomRight, LoadDeckSearchText, loadDeckCancelButton);
            LoadWishList = OcrImageArea.GetAreaFor(loadDeckTopLeft, loadDeckBottomRight, LoadDeckSearchText, Point.Empty);

            var systemAlertTopLeft = new Point(736, 333);
            var systemAlertBottomRight = new Point(953, 366);
            var systemAlertButton = new Point(840, 619);
            SystemAlert = OcrImageArea.GetAreaFor(systemAlertTopLeft, systemAlertBottomRight, SystemAlertText, systemAlertButton);

            var newCardsConfirmationWindowTopLeft = new Point(735, 256);
            var newCardsConfirmationWindowBottomRight = new Point(938, 287);
            var newCardsConfirmationButton = new Point(841, 725);
            NewCards = OcrImageArea.GetAreaFor(newCardsConfirmationWindowTopLeft, newCardsConfirmationWindowBottomRight, NewCardsText, newCardsConfirmationButton);

            TradeConversationDockWindowButtonColorArray = new List<int>() { 16645629, 16711422, 16777215 };

            DeckEditorDeckSectionTopLeft = new Point(512, 289);
            DeckEditorDeckSectionBottomRight = new Point(929, 442);
            DeckEditorDeckSectionChecksum = -226885631;
        }
    }
}
