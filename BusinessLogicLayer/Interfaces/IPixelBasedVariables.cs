using System.Collections.Generic;
using System.Drawing;
using BusinessLogicLayer.OCR;

namespace BusinessLogicLayer.Interfaces
{
    public interface IPixelBasedVariables
    {
        Point ClassifiedMessageBox { get; set; }
        Point PasswordTextField { get; set; }
        Point UserNameTextField { get; set; }
        Point ClassifiedPostingButton { get; set; }
        Point ButtonToOpenClassifiedsWindowFromHomeScreen { get; set; }
        Point TradeConversationInitialLocation { get; set; }
        Point PrivateChatCloseButtonLocation { get; set; }
        Point TradeConversationUsername { get; set; }
        Point TradeConversationMessageBox { get; set; }
        Point TradeWindowListViewButton { get; set; }
        Point YouGetTypeColumnStartPosition { get; set; }
        Point YouGetTypeColumnEndPosition { get; set; }
        Point YouGiveTypeColumnStartPosition { get; set; }
        Point YouGiveTypeColumnEndPosition { get; set; }
        Point FilterBlankSpotPosition { get; set; }
        Point FilterSetSelectorPosition { get; set; }
        Point FilterRaritySelectorPosition { get; set; }
        Point FilterVersionsSelectorPosition { get; set; }
        Point FilterOwnedSelectorPosition { get; set; }
        Point FilterResetButton { get; set; }
        Point FilterOwnedTextBox { get; set; }
        Point CancelTradeButton { get; set; }
        Point FilterSearchTextBoxPosition { get; set; }
        Point TheirCollectionFirstCardPosition { get; set; }
        Point LoadWishListButton { get; set; }
        Point WishListFirstFileLocation { get; set; }
        Point LoadDeckButton { get; set; }
        Point EditPostingButton { get; set; }
        Point ClassifiedsTab { get; set; }
        Point DeckEditorTab { get; set; }
        Point TradeConversationFirstMessage { get; set; }
        Point LoadDeckCancelButton { get; set; }
        Point SystemAlertButton { get; set; }
        Point TradeSessionCancelledButton { get; set; }
        Point TradeRequestButton { get; set; }
        Point TradeCompleteButton { get; set; }
        Point NewCardsButton { get; set; }
        Point ChallengeDeclineButton { get; set; }
        Point TradeConversationChange { get; set; }
        Point TradeConfirmationSecondTimeButton { get; set; }

        /// <summary>
        /// Used to cancel a trade from the confirmation screen.
        /// </summary>
        Point ConfirmTradeCancelButton { get; set; }

        Square YouGiveCardSet { get; set; }
        Square YouGiveCardNumber { get; set; }
        Square YouGetNumberOfCards { get; set; }
        Square YouGetConfirmedNumberOfCards { get; set; }
        Square YouGiveNumberOfCards { get; set; }
        Square YouGiveConfirmedNumberOfCards { get; set; }
        Point ConfirmTradeFirstTime { get; set; }
        Square FirstCardInListViewName { get; set; }
        Square FirstCardInListViewNumber { get; set; }
        Square FirstCardInListViewSet { get; set; }
        Square LastCardInListViewAfterArray { get; set; }
        Square SectionTitle { get; set; }
        Square YouGetCardSet { get; set; }
        Square YouGetCardNumber { get; set; }
        Square YouGetCardBlankSpace { get; set; }
        Square DeckEditorDeckSection { get; set; }
        Square LoadDeckWindow { get; set; }
        Square TradeSessionCancelledWindow { get; set; }
        Square ConfirmCancelTradeWindow { get; set; }
        Square SystemAlertWindow { get; set; }
        Square TradeRequestWindow { get; set; }
        Square NewCardsWindow { get; set; }
        Square ChallengeWindow { get; set; }
        Square TradeConfirmationSecondTimeWindow { get; set; }
        List<Square> YouGetCardLocations { get; set; }
        List<Square> YouGiveCardLocations { get; set; }
        List<int> TradeConversationDockWindowButtonColorArray { get; set; }
        List<int> CardInListViewBottomRightValues { get; set; }
        int ListDifference { get; set; }
        int SlowDownValue { get; set; }
        List<int> DeckEditorDeckSectionChecksum { get; set; }
        int CardsInGetOrGiveCollectionListView { get; set; }
        int CardsInCollectionListView { get; set; }
        int CardSelectedColor { get; set; }
        OcrImageArea CancelLoadDeck { get; set; }
        OcrImageArea ConfirmCancelTrade { get; set; }
        OcrImageArea TradeCancelled { get; set; }
        OcrImageArea SystemAlert { get; set; }
        OcrImageArea NewCards { get; set; }
        OcrImageArea TradeRequest { get; set; }
        OcrImageArea TradeComplete { get; set; }
        OcrImageArea Challenge { get; set; }
        OcrImageArea HomeScreen { get; set; }
        OcrImageArea ConfirmTradeSecondTime { get; set; }
        OcrImageArea LoadWishList { get; set; }
        int MessageBoxLocationYDiff { get; set; }
        Square ConfirmYouGiveAfterArray { get; set; }
        Square ConfirmScreenYouGetAmount { get; set; }
        Square ConfirmScreenYouGiveAmount { get; set; }
        Square TradeMessageBoxChecksumArea { get; set; }
        Square ConfirmYouGiveNumber { get; set; }
        Square ConfirmYouGiveSet { get; set; }
        Square ConfirmYouGiveCardName { get; set; }
        List<int> ConfirmYouGiveArray { get; set; }
        Square TradeFilterSetText { get; set; }
        Square TradeFilterRarityText { get; set; }
        Square TradeFilterVersionText { get; set; }
        Point HomeTabLocation { get; set; }

        /// <summary>
        /// Used for Confirmation Question when cancelling a trade
        /// </summary>
        Point ConfirmCancelTradeButton { get; set; }

        Square YouGiveAmountNumber { get; set; }
        Square YouGiveAmountName { get; set; }
        Square YouGiveAmountBlankSpace { get; set; }
        Square YouGetAmountNumber { get; set; }
        Square YouGetAmountName { get; set; }
        Square YouGetAmountBlankSpace { get; set; }

        Square YouGetAmountNameConfirmed { get; set; }
        Square YouGetAmountNumberConfirmed { get; set; }
        Square YouGetAmountBlankSpaceConfirmed { get; set; }
        Square YouGiveAmountNameConfirmed { get; set; }
        Square YouGiveAmountNumberConfirmed { get; set; }
        Square YouGiveAmountBlankSpaceConfirmed { get; set; }
        Square YouGiveCardListChecksumArea { get; set; }
        Square LaunchButton { get; set; }
        Square UlaAcceptButton { get; set; }
        Square UlaSlider { get; set; }
        Point MaximizeWindowPosition { get; set; }
        Square LogOnButton { get; set; }
        int BuddySelectedColor { get; set; }
        int MaxBuddyListCount { get; set; }

        Square TradeContextMenuOption { get; set; }
        Point TopRightTabSelector { get; set; }
        Point BuddyListSelector { get; set; }
        Point FirstBuddy { get; set; }
        Point FirstBuddySelectedCheck { get; set; }
        Point AddBuddyButton { get; set; }
        Point AddBuddyTextBox { get; set; }
        OcrImageArea ConnectionProblem { get; set; }
        Point CollectionFirstCard { get; set; }
        Point CollectionListView { get; set; }
        Point CollectionTab { get; set; }
        Point ImportFileNameTextBox { get; set; }
        Square ImportTradableOcr { get; set; }
        Square MarkSelectionAllTradable { get; set; }
        Square MarkSelectionAllUntradable { get; set; }
        Point CollectionSearchBox { get; set; }
        Point TradeContextDiff { get; set; }
    }
}