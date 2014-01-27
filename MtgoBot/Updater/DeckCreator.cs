using System.Drawing;
using BusinessLogicLayer;

namespace AutoItBot.Updater
{
    /// <summary>
    /// Before executing this code the ShowAllVersions and My Cards checkboxes should be handled.  Also the CardSet should be on ScarsOfMirrodin
    /// </summary>
    public class DeckCreator
    {
        private Point ShowAllVersions = new Point(555, 62);
        private Point MyCards = new Point(736, 62);
        private Point Tab = new Point(401, 1144);
        private Point SetSelector = new Point(802, 17);

        private Point CardPool_ContextMenu = new Point(75, 167);
        private Point CardPool_SelectAll = new Point(152, 300);
        private Point CardPool_AddOneCard = new Point(146, 175);

        private Point Deck_ContextMenu = new Point(9, 663); //new Point(532, 182);
        private Point Deck_SortByName = new Point(71, 701);

        private Point SaveAsButton = new Point(1018, 63);
        private Point WishListDeck = new Point(768, 510);
        private Point SaveButton = new Point(749, 821);
        private Point NewButton = new Point(1185, 69);
        private Point NoSaveButton = new Point(878, 713);
        private Point SaveOverwrite = new Point(867,709);
        private Point BlankPosition = new Point(1426, 20);
        
        private int _currentSet = (int) CardSet.ScarsOfMirrodin;

        public static readonly Point blankCardPosition = new Point(40, 700);
        public static readonly int blankColor = 0;
        public static readonly int waitingColor = 8025451;
        public static readonly Point alternateCardPosition = new Point(1652, 1000);

        public void SaveDeckFileForNewSet(CardSet cardSet)
        {
            AutoItX.MouseClick(Tab);
            AutoItX.Sleep(2000);

            AutoItX.MouseClick(NewButton);
            AutoItX.Sleep(1000);

            AutoItX.MouseClick(NoSaveButton);

            while (AutoItX.PixelGetColor(alternateCardPosition) == waitingColor)
            {
                AutoItX.Sleep(500);
            }

            AutoItX.Sleep(2000);
            Selector.PickFilterOption(this._currentSet, (int) cardSet, SetSelector);
            AutoItX.Sleep(1000);
            AutoItX.MouseClick(BlankPosition);
            AutoItX.Sleep(1000);

            AutoItX.MouseClick("right", CardPool_ContextMenu.X, CardPool_ContextMenu.Y);
            AutoItX.Sleep(1000);
            AutoItX.MouseClick(CardPool_SelectAll);
            AutoItX.Sleep(1000);
            AutoItX.MouseClick("right", CardPool_ContextMenu.X, CardPool_ContextMenu.Y);
            AutoItX.Sleep(1000);
            AutoItX.MouseClick(CardPool_AddOneCard);

            while (AutoItX.PixelGetColor(blankCardPosition) == blankColor)
            {
                AutoItX.Sleep(500);
            }

            AutoItX.MouseClick("right", Deck_ContextMenu.X, Deck_ContextMenu.Y);
            AutoItX.Sleep(1000);
            AutoItX.MouseClick(Deck_SortByName);

            while (AutoItX.PixelGetColor(blankCardPosition) != blankColor)
            {
                AutoItX.Sleep(500);
            }

            AutoItX.Sleep(1000);
            AutoItX.MouseClick(SaveAsButton);
            AutoItX.Sleep(1000);
            AutoItX.MouseClick(WishListDeck);
            AutoItX.Sleep(1000);
            AutoItX.MouseClick(SaveButton);
            AutoItX.Sleep(1000);
            AutoItX.MouseClick(SaveOverwrite);
            
            while (AutoItX.PixelGetColor(alternateCardPosition) == waitingColor)
            {
                AutoItX.Sleep(500);
            }
        }
    }
}