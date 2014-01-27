namespace BusinessLogicLayer.Interfaces
{
    public interface IWindowManager
    {
        bool CheckForLoadDeckWindow();
        void CheckForPrivateConversationWindows();

        /// <summary>
        /// Find the trade window communication message box.  
        /// </summary>
        /// <returns>Returns true if the Chat Window is found.</returns>
        bool FindTradeChatWindowAndDock();

        bool CheckIfChallenge();
        bool CheckForTrade();
        bool CheckForCompleteTrade();
        bool CheckIfOnHomeScreen();
        bool CheckIfNewCardConfirmationScreen();
        bool CheckIfLoadWishListScreenIsVisiable();
        bool CheckIfSystemAlert();
        bool CloseTradeCancelledDialog();
        bool ConfirmCancelledTrade();

        /// <summary>
        /// Check for other user communication windows to close them if they are not the trade or disconnect window.
        /// Windows to check for: Challenge, Chat before Trade
        /// </summary>
        bool CheckForOtherWindows();

        void MoveToDeckEditor();
        bool InTrade();
        bool OnTradeScreen();
        bool OnTradeConfirmScreen();
        void CancelTradeFromConfirmationScreen();
        string GetSectionTitle();
        bool ProgramIsActive();
        bool CheckForConnectionProblem();
    }
}