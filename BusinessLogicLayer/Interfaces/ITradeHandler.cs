using System;
using System.Collections.Generic;
using System.Drawing;
using CardDataLayer.Models;

namespace BusinessLogicLayer.Interfaces
{
    public interface ITradeHandler
    {
        event EventHandler YouGiveAmountChangedEvent;
        //TODO: Might be better to move these dictionaries out of this object
        Dictionary<int, MagicCard> CardsYouGet { get; }
        Dictionary<int, MagicCard> CardsYouGive { get; }
        int YouGetAmount { get; }
        int YouGiveAmount { get; }
        bool YouGiveAmountChanged { get; }
        //TODO: Consolidation code to examine a list of cards
        void DetermineCardsYouGet();
        void DetermineUsersSelection();
        //void SetupForExaminingCards(int maxCopies = 75, int currentCopies = 0);
        Dictionary<int, MagicCard> ExamineCollection();
        Dictionary<int, MagicCard> ExamineYouGiveCardsOnConfirmedScreen();
        void MoveTypeColumn(Point startPosition, Point endPosition);
        void DetermineYouGetAmount();
        int DetermineYouGetConfirmedAmount();
        bool DetermineIfYouGiveAmountChanged(bool fireEvent);
        int DetermineYouGiveConfirmedAmount();
        void LoadWishList();
        void WriteWishListFile(Dictionary<int, MagicCard> wishList);
        void PressConfirmButton();
        void PressSecondConfirmButton();
        void CancelTrade();
        void CancelTradeFromConfirmationScreen();
        void DetermineCardsYouGive();
        bool DetermineIfMessageAreaHasChanged();
    }
}