using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using BusinessLogicLayer;
using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.OCR;
using CardDataLayer.Models;
using CardDataLayer.Repositories;
using Framework;
using Framework.Logging;

namespace AutoItBot.PixelBasedVariables
{
    public class ScreenLocations : IPixelBasedVariables
    {
        private readonly ILogger _logger = IoC.GetLoggerFor<ScreenLocations>();
		private readonly List<PixelLocation> _databaseLocations = IoC.Resolve<PixelLocationsRepository>().Get();
        
		public ScreenLocations()
        {
            SlowDownValue = 0;
            CardSelectedColor = 15658734;
            CardsInCollectionListView = 22;
            CardsInGetOrGiveCollectionListView = 5;
            DeckEditorDeckSectionChecksum = new List<int> { 152969558, -229437421, -71828847, //Win7 1920x1200
                -226885631 // Windows XP version 1680x1050
            };
            BuddySelectedColor = 13024441;
            MaxBuddyListCount = 14;
        }

        public ScreenLocations Load()
        {
            MessageBoxLocationYDiff = GetPoint("MessageBoxLocationYDiff").X;
            TradeConversationChange = GetPoint("TradeConversationChange"); new Point(29, 259);
            ListDifference = GetPoint("ListDifference").X;
            UserNameTextField = GetPoint("UserNameTextField");
            PasswordTextField = GetPoint("PasswordTextField");
            ClassifiedMessageBox = GetPoint("ClassifiedMessageBox");
            ClassifiedPostingButton = GetPoint("ClassifiedPostingButton");
            ButtonToOpenClassifiedsWindowFromHomeScreen = GetPoint("ButtonToOpenClassifiedsWindowFromHomeScreen");
            TradeConversationInitialLocation = GetPoint("TradeConversationInitialLocation");
            PrivateChatCloseButtonLocation = GetPoint("PrivateChatCloseButtonLocation");
            TradeConversationUsername = GetPoint("TradeConversationUsername");
            TradeConversationMessageBox = GetPoint("TradeConversationMessageBox");
            TradeWindowListViewButton = GetPoint("TradeWindowListViewButton");
            YouGetTypeColumnStartPosition = GetPoint("YouGetTypeColumnStartPosition");
            YouGetTypeColumnEndPosition = GetPoint("YouGetTypeColumnEndPosition");
            YouGiveTypeColumnStartPosition = GetPoint("YouGiveTypeColumnStartPosition");
            YouGiveTypeColumnEndPosition = GetPoint("YouGiveTypeColumnEndPosition");
            FilterBlankSpotPosition = GetPoint("FilterBlankSpotPosition");
            FilterSetSelectorPosition = GetPoint("FilterSetSelectorPosition");
            FilterRaritySelectorPosition = GetPoint("FilterRaritySelectorPosition");
            FilterVersionsSelectorPosition = GetPoint("FilterVersionsSelectorPosition");
            FilterOwnedSelectorPosition = GetPoint("FilterOwnedSelectorPosition");
            FilterResetButton = GetPoint("FilterResetButton");
            FilterOwnedTextBox = GetPoint("FilterOwnedTextBox");
            CancelTradeButton = GetPoint("CancelTradeButton");
            FilterSearchTextBoxPosition = GetPoint("FilterSearchTextBoxPosition");
            TheirCollectionFirstCardPosition = GetPoint("TheirCollectionFirstCardPosition");
            LoadWishListButton = GetPoint("LoadWishListButton");
            WishListFirstFileLocation = GetPoint("WishListFirstFileLocation");
            LoadDeckButton = GetPoint("LoadDeckButton");
            EditPostingButton = GetPoint("EditPostingButton");
            ClassifiedsTab = GetPoint("ClassifiedsTab");
            DeckEditorTab = GetPoint("DeckEditorTab");
            TradeConversationFirstMessage = GetPoint("TradeConversationFirstMessage");
            LoadDeckCancelButton = GetPoint("LoadDeckCancelButton");
            SystemAlertButton = GetPoint("SystemAlertButton");
            TradeSessionCancelledButton = GetPoint("TradeSessionCancelledButton");
            TradeRequestButton = GetPoint("TradeRequestButton");
            TradeCompleteButton = GetPoint("TradeCompleteButton");
            NewCardsButton = GetPoint("NewCardsButton");
            ChallengeDeclineButton = GetPoint("ChallengeDeclineButton");
            TradeConfirmationSecondTimeButton = GetPoint("TradeConfirmationSecondTimeButton");
            ConfirmTradeCancelButton = GetPoint("ConfirmTradeCancelButton");
            HomeTabLocation = GetPoint("HomeTabLocation");
            ConfirmCancelTradeButton = GetPoint("ConfirmCancelTradeButton");

            YouGetNumberOfCards = GetSquare("YouGetNumberOfCards");
            YouGetConfirmedNumberOfCards = GetSquare("YouGetConfirmedNumberOfCards");
            YouGiveNumberOfCards = GetSquare("YouGiveNumberOfCards");
            YouGiveConfirmedNumberOfCards = GetSquare("YouGiveConfirmedNumberOfCards");
            ConfirmTradeFirstTime = GetPoint("ConfirmTradeFirstTime");
            FirstCardInListViewName = GetSquare("FirstCardInListViewName");
            FirstCardInListViewNumber = GetSquare("FirstCardInListViewNumber");
            FirstCardInListViewSet = GetSquare("FirstCardInListViewSet");
            LastCardInListViewAfterArray = GetSquare("LastCardInListViewAfterArray");
            SectionTitle = GetSquare("SectionTitle");
            YouGetCardSet = GetSquare("YouGetCardSet");
            YouGetCardNumber = GetSquare("YouGetCardNumber");
            YouGetCardBlankSpace = GetSquare("YouGetCardBlankSpace");
            DeckEditorDeckSection = GetSquare("DeckEditorDeckSection");
            TradeSessionCancelledWindow = GetSquare("TradeSessionCancelledWindow");
            SystemAlertWindow = GetSquare("SystemAlertWindow");
            TradeRequestWindow = GetSquare("TradeRequestWindow");
            NewCardsWindow = GetSquare("NewCardsWindow");
            ChallengeWindow = GetSquare("ChallengeWindow");
            TradeConfirmationSecondTimeWindow = GetSquare("TradeConfirmationSecondTimeWindow");
            LoadDeckWindow = GetSquare("LoadDeckWindow");
            ConfirmCancelTradeWindow = GetSquare("ConfirmCancelTradeWindow");

            YouGiveAmountNumber = GetSquare("YouGiveAmountNumber");
            YouGiveAmountName = GetSquare("YouGiveAmountName");
            YouGiveAmountBlankSpace = GetSquare("YouGiveAmountBlankSpace");
            YouGetAmountNumber = GetSquare("YouGetAmountNumber");
            YouGetAmountName = GetSquare("YouGetAmountName");
            YouGetAmountBlankSpace = GetSquare("YouGetAmountBlankSpace");

            YouGetAmountNameConfirmed = GetSquare("YouGetAmountNameConfirmed");
            YouGetAmountNumberConfirmed = GetSquare("YouGetAmountNumberConfirmed");
            YouGetAmountBlankSpaceConfirmed = GetSquare("YouGetAmountBlankSpaceConfirmed");
            YouGiveAmountNameConfirmed = GetSquare("YouGiveAmountNameConfirmed");
            YouGiveAmountNumberConfirmed = GetSquare("YouGiveAmountNumberConfirmed");
            YouGiveAmountBlankSpaceConfirmed = GetSquare("YouGiveAmountBlankSpaceConfirmed");

            YouGiveCardListChecksumArea = GetSquare("YouGiveCardListChecksumArea");

            LaunchButton = GetSquare("LaunchButton");
            UlaAcceptButton = GetSquare("UlaAcceptButton");
            UlaSlider = GetSquare("UlaSlider");
            MaximizeWindowPosition = GetPoint("MaximizeWindowPosition");
            LogOnButton = GetSquare("LogOnButton");

            TradeContextMenuOption = GetSquare("TradeContextMenuOption");
            TopRightTabSelector = GetPoint("TopRightTabSelector");
            BuddyListSelector = GetPoint("BuddyListSelector");
            FirstBuddy = GetPoint("FirstBuddy");
            FirstBuddySelectedCheck = GetPoint("FirstBuddySelectedCheck");
            AddBuddyButton = GetPoint("AddBuddyButton");
            AddBuddyTextBox = GetPoint("AddBuddyTextBox");

            CollectionFirstCard = GetPoint("CollectionFirstCard");
            CollectionListView = GetPoint("CollectionListView");
            CollectionTab = GetPoint("CollectionTab");
            ImportFileNameTextBox = GetPoint("ImportFileNameTextBox");
            ImportTradableOcr = GetSquare("ImportTradableOcr");
            MarkSelectionAllTradable = GetSquare("MarkSelectionAllTradable");
            MarkSelectionAllUntradable = GetSquare("MarkSelectionAllUntradable");
            CollectionSearchBox = GetPoint("CollectionSearchBox");
            TradeContextDiff = GetPoint("TradeContextDiff");

            YouGiveCardSet = YouGetCardSet.Copy().MoveAlongXAxis(-ListDifference);
            YouGiveCardNumber = YouGetCardNumber.Copy().MoveAlongXAxis(-ListDifference);

            try
            {
                CardInListViewBottomRightValues = _databaseLocations
                    .Single(p => p.LocationId == "CardInListViewBottomRightValues")
                    .ArrayValue
                    .Split(',')
                    .Select(int.Parse)
                    .ToList();

                ConfirmYouGiveArray = _databaseLocations
                    .Single(p => p.LocationId == "ConfirmYouGiveCardName")
                    .ArrayValue
                    .Split(',')
                    .Select(int.Parse)
                    .ToList();

                TradeConversationDockWindowButtonColorArray =
                    _databaseLocations
                        .Single(p => p.LocationId == "TradeConversationDockWindowButtonColorArray")
                        .ArrayValue
                        .Split(',')
                        .Select(int.Parse)
                        .ToList();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Missing values for lists at this resolution.");
            }


            ConfirmYouGiveAfterArray = GetSquare("ConfirmYouGiveAfterArray");

            ConfirmScreenYouGetAmount = GetSquare("ConfirmScreenYouGetAmount");
            ConfirmScreenYouGiveAmount = GetSquare("ConfirmScreenYouGiveAmount");
            TradeMessageBoxChecksumArea = GetSquare("TradeMessageBoxChecksumArea");
            ConfirmYouGiveNumber = GetSquare("ConfirmYouGiveNumber");
            ConfirmYouGiveSet = GetSquare("ConfirmYouGiveSet");
            ConfirmYouGiveCardName = GetSquare("ConfirmYouGiveCardName");

            TradeFilterSetText = GetSquare("TradeFilterSetText");
            TradeFilterRarityText = GetSquare("TradeFilterRarityText");
            TradeFilterVersionText = GetSquare("TradeFilterVersionText");

            var youGetFirstCard = GetSquare("YouGetFirstCard");
            var youGetSecondCard = GetSquare("YouGetSecondCard");
            var youGetThirdCard = GetSquare("YouGetThirdCard");
            var youGetFourthCard = GetSquare("YouGetFourthCard");
            var youGetFifthCard = GetSquare("YouGetFifthCard");
            var youGetAfterFifthCard = GetSquare("YouGetAfterFifthCard");

            YouGetCardLocations =
                new List<Square>
                    {
                        youGetFirstCard,
                        youGetSecondCard,
                        youGetThirdCard,
                        youGetFourthCard,
                        youGetFifthCard,
                        youGetAfterFifthCard
                    };

            YouGiveCardLocations =
                new List<Square>
                    {
                        youGetFirstCard.Copy().MoveAlongXAxis(-ListDifference),
                        youGetSecondCard.Copy().MoveAlongXAxis(-ListDifference),
                        youGetThirdCard.Copy().MoveAlongXAxis(-ListDifference),
                        youGetFourthCard.Copy().MoveAlongXAxis(-ListDifference),
                        youGetFifthCard.Copy().MoveAlongXAxis(-ListDifference),
                        youGetAfterFifthCard.Copy().MoveAlongXAxis(-ListDifference),
                    };

            CancelLoadDeck = OcrImageArea.GetAreaFor(LoadDeckWindow, Constants.LoadDeckSearchText, LoadDeckCancelButton);
            ConfirmCancelTrade = OcrImageArea.GetAreaFor(ConfirmCancelTradeWindow, Constants.CancelTradeText, ConfirmCancelTradeButton);
            TradeCancelled = OcrImageArea.GetAreaFor(TradeSessionCancelledWindow, Constants.TradeSessionCancelledText, TradeSessionCancelledButton);
            SystemAlert = OcrImageArea.GetAreaFor(SystemAlertWindow, Constants.SystemAlertText, SystemAlertButton);
            NewCards = OcrImageArea.GetAreaFor(NewCardsWindow, Constants.NewCardsText, NewCardsButton);
            TradeRequest = OcrImageArea.GetAreaFor(TradeRequestWindow, Constants.TradeRequestTextSearchString, TradeRequestButton);
            TradeComplete = OcrImageArea.GetAreaFor(TradeRequestWindow, Constants.TradeCompleteTextSearchString, TradeCompleteButton);
            Challenge = OcrImageArea.GetAreaFor(ChallengeWindow, Constants.ChallengeText, ChallengeDeclineButton);
            HomeScreen = OcrImageArea.GetAreaFor(SectionTitle, Constants.HomeScreenText, ButtonToOpenClassifiedsWindowFromHomeScreen);
            ConfirmTradeSecondTime = OcrImageArea.GetAreaFor(TradeConfirmationSecondTimeWindow, Constants.ConfirmTradeText, TradeConfirmationSecondTimeButton);
            LoadWishList = OcrImageArea.GetAreaFor(LoadDeckWindow, Constants.LoadDeckSearchText, Point.Empty);
            ConnectionProblem = OcrImageArea.GetAreaFor(TradeRequestWindow, Constants.ConnectionProblem, Point.Empty);

            return this;
        }

        private Square GetSquare(string locationId)
        {
            try
            {
                var location = _databaseLocations.Single(p => p.LocationId == locationId);
                return Square.Initialize(location.Top.GetValueOrDefault(), location.Left.GetValueOrDefault(),
                                         location.Bottom.GetValueOrDefault(), location.Right.GetValueOrDefault());
            }
            catch (Exception e)
            {
                _logger.Error(e, e.Message + " LocationId: " + locationId);
                return Square.Initialize(new Point(), new Point());
            }
        }

        private Point GetPoint(string locationId)
        {
            try
            {
                return new Point(X(locationId), Y(locationId));
            }
            catch (Exception e)
            {
                _logger.Error(e, e.Message + " LocationId: " + locationId);
                return new Point();
            }
        }

        private int X(string locationId)
        {
            return _databaseLocations.Single(p => p.LocationId == locationId).Left.GetValueOrDefault();
        }

        private int Y(string locationId)
        {
            return _databaseLocations.Single(p => p.LocationId == locationId).Top.GetValueOrDefault();
        }

        public Point ClassifiedMessageBox { get; set; }
        public Point PasswordTextField { get; set; }
        public Point UserNameTextField { get; set; }
        public Point ClassifiedPostingButton { get; set; }
        public Point ButtonToOpenClassifiedsWindowFromHomeScreen { get; set; }
        public Point TradeConversationInitialLocation { get; set; }
        public Point PrivateChatCloseButtonLocation { get; set; }
        public Point TradeConversationUsername { get; set; }
        public Point TradeConversationMessageBox { get; set; }
        public Point TradeWindowListViewButton { get; set; }
        public Point YouGetTypeColumnStartPosition { get; set; }
        public Point YouGetTypeColumnEndPosition { get; set; }
        public Point YouGiveTypeColumnStartPosition { get; set; }
        public Point YouGiveTypeColumnEndPosition { get; set; }
        public Point FilterBlankSpotPosition { get; set; }
        public Point FilterSetSelectorPosition { get; set; }
        public Point FilterRaritySelectorPosition { get; set; }
        public Point FilterVersionsSelectorPosition { get; set; }
        public Point FilterOwnedSelectorPosition { get; set; }
        public Point FilterResetButton { get; set; }
        public Point FilterOwnedTextBox { get; set; }
        public Point CancelTradeButton { get; set; }
        public Point FilterSearchTextBoxPosition { get; set; }
        public Point TheirCollectionFirstCardPosition { get; set; }
        public Point LoadWishListButton { get; set; }
        public Point WishListFirstFileLocation { get; set; }
        public Point LoadDeckButton { get; set; }
        public Point EditPostingButton { get; set; }
        public Point ClassifiedsTab { get; set; }
        public Point DeckEditorTab { get; set; }
        public Point TradeConversationFirstMessage { get; set; }
        public Point LoadDeckCancelButton { get; set; }
        public Point SystemAlertButton { get; set; }
        public Point TradeSessionCancelledButton { get; set; }
        public Point TradeRequestButton { get; set; }
        public Point TradeCompleteButton { get; set; }
        public Point NewCardsButton { get; set; }
        public Point ChallengeDeclineButton { get; set; }
        public Point TradeConversationChange { get; set; }
        public Point TradeConfirmationSecondTimeButton { get; set; }
        public Point ConfirmTradeCancelButton { get; set; }
        public Point ConfirmCancelTradeButton { get; set; }
		public Point ConfirmTradeFirstTime { get; set; }
		public Point HomeTabLocation { get; set; }
		public Point MaximizeWindowPosition { get; set; }
		public Point TopRightTabSelector { get; set; }
		public Point BuddyListSelector { get; set; }
		public Point FirstBuddy { get; set; }
		public Point FirstBuddySelectedCheck { get; set; }
		public Point AddBuddyButton { get; set; }
		public Point AddBuddyTextBox { get; set; }
		public Point CollectionFirstCard { get; set; }
		public Point CollectionListView { get; set; }
		public Point CollectionTab { get; set; }
		public Point ImportFileNameTextBox { get; set; }
		public Point CollectionSearchBox { get; set; }
		public Point TradeContextDiff { get; set; }

        public Square YouGiveAmountNumber { get; set; }
        public Square YouGiveAmountName { get; set; }
        public Square YouGiveAmountBlankSpace { get; set; }
        public Square YouGetAmountNumber { get; set; }
        public Square YouGetAmountName { get; set; }
        public Square YouGetAmountBlankSpace { get; set; }
        public Square YouGetAmountNameConfirmed { get; set; }
        public Square YouGetAmountNumberConfirmed { get; set; }
        public Square YouGetAmountBlankSpaceConfirmed { get; set; }
        public Square YouGiveAmountNameConfirmed { get; set; }
        public Square YouGiveAmountNumberConfirmed { get; set; }
        public Square YouGiveAmountBlankSpaceConfirmed { get; set; }
        public Square YouGiveCardListChecksumArea { get; set; }
        public Square YouGiveCardSet { get; set; }
        public Square YouGiveCardNumber { get; set; }
        public Square YouGetNumberOfCards { get; set; }
        public Square YouGetConfirmedNumberOfCards { get; set; }
        public Square YouGiveNumberOfCards { get; set; }
        public Square YouGiveConfirmedNumberOfCards { get; set; }
        public Square FirstCardInListViewName { get; set; }
        public Square FirstCardInListViewNumber { get; set; }
        public Square FirstCardInListViewSet { get; set; }
        public Square LastCardInListViewAfterArray { get; set; }
        public Square SectionTitle { get; set; }
        public Square YouGetCardSet { get; set; }
        public Square YouGetCardNumber { get; set; }
        public Square YouGetCardBlankSpace { get; set; }
        public Square DeckEditorDeckSection { get; set; }
        public Square LoadDeckWindow { get; set; }
        public Square TradeSessionCancelledWindow { get; set; }
        public Square ConfirmCancelTradeWindow { get; set; }
        public Square SystemAlertWindow { get; set; }
        public Square TradeRequestWindow { get; set; }
        public Square NewCardsWindow { get; set; }
        public Square ChallengeWindow { get; set; }
		public Square TradeConfirmationSecondTimeWindow { get; set; }
		public Square TradeFilterSetText { get; set; }
		public Square TradeFilterRarityText { get; set; }
		public Square TradeFilterVersionText { get; set; }
		public Square ConfirmScreenYouGetAmount { get; set; }
		public Square ConfirmScreenYouGiveAmount { get; set; }
		public Square TradeMessageBoxChecksumArea { get; set; }
		public Square ConfirmYouGiveNumber { get; set; }
		public Square ConfirmYouGiveSet { get; set; }
		public Square ConfirmYouGiveCardName { get; set; }
		public Square ConfirmYouGiveAfterArray { get; set; }
		public Square LaunchButton { get; set; }
		public Square UlaAcceptButton { get; set; }
		public Square UlaSlider { get; set; }
		public Square LogOnButton { get; set; }
		public Square TradeContextMenuOption { get; set; }
		public Square ImportTradableOcr { get; set; }
		public Square MarkSelectionAllTradable { get; set; }
		public Square MarkSelectionAllUntradable { get; set; }

        public List<Square> YouGetCardLocations { get; set; }
        public List<Square> YouGiveCardLocations { get; set; }
        
		public List<int> TradeConversationDockWindowButtonColorArray { get; set; }
        public List<int> CardInListViewBottomRightValues { get; set; }
		public List<int> DeckEditorDeckSectionChecksum { get; set; }
		public List<int> ConfirmYouGiveArray { get; set; }
	    
		public int ListDifference { get; set; }
	    public int SlowDownValue { get; set; }
	    public int CardsInGetOrGiveCollectionListView { get; set; }
        public int CardsInCollectionListView { get; set; }
        public int CardSelectedColor { get; set; }
		public int MessageBoxLocationYDiff { get; set; }
		public int BuddySelectedColor { get; set; }
		public int MaxBuddyListCount { get; set; }

        public OcrImageArea CancelLoadDeck { get; set; }
        public OcrImageArea ConfirmCancelTrade { get; set; }
        public OcrImageArea TradeCancelled { get; set; }
        public OcrImageArea SystemAlert { get; set; }
        public OcrImageArea NewCards { get; set; }
        public OcrImageArea TradeRequest { get; set; }
        public OcrImageArea TradeComplete { get; set; }
        public OcrImageArea Challenge { get; set; }
        public OcrImageArea HomeScreen { get; set; }
        public OcrImageArea ConfirmTradeSecondTime { get; set; }
        public OcrImageArea LoadWishList { get; set; }
        public OcrImageArea ConnectionProblem { get; set; }
    }
}
