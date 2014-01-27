using System;
using System.Collections.Generic;
using System.Drawing;
using AutoItBot.OCR;
using Framework;

namespace AutoItBot.PixelBasedVariables
{
    public class XPWidescreen1680x1050 : ScreenLocations
    {
        public XPWidescreen1680x1050()
        {
            TradeConversationChangeInY = 228;
            TradeConversationChangeInX = 23;

            YouGetFirstCardPosition = new Point(850, 831);
            YouGiveFirstCardPosition = new Point(114, 832);

            CardSelectedColor = 15592941;

            CardsInCollectionListView = 22;
            CardsInGetOrGiveCollectionListView = 5;

            //Below here

            LastCardInListViewAfterArrayTopLeft = new Point(22, 658);
            LastCardInListViewAfterArrayBottomRight = new Point(231, 680);

            #region Trade Cancel Section

            var cancelTradeTopLeft = new Point(707, 455);
            var cancelTradeBottomRight = new Point(969, 508);
            var cancelButtonPosition = new Point(766, 629);
            ConfirmCancelTrade = OcrImageArea.GetAreaFor(cancelTradeTopLeft, cancelTradeBottomRight, CancelTradeText, cancelButtonPosition);

            #endregion
        }
    }
}
