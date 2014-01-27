using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using BusinessLogicLayer;
using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.MagicCards;
using BusinessLogicLayer.OCR;
using AutoItBot.PixelBasedVariables;
using AutoItBot.Properties;
using BusinessLogicLayer.Properties;
using Framework;
using Framework.Extensions;
using Framework.Logging;

namespace AutoItBot.Handles
{
    public class TradeHandler : ITradeHandler
    {
        private ILogger _logger = IoC.GetLoggerFor<TradeHandler>();
        private int _youGiveAmountChecksum;
        private IPixelBasedVariables _pbv;
        private IWindowManager _windowManager;
        private IMessageHandler _messageHandler;
        private string _deckFileLocation;
        private int _messageAreaChecksum = 0;
        private IApplicationSettings _applicationSettings;
        private int _youGiveCardListChecksum = 0;

        public event EventHandler YouGiveAmountChangedEvent;
        public Dictionary<int, MagicCard> CardsYouGet { get; private set; }
        public Dictionary<int, MagicCard> CardsYouGive { get; private set; }
        public int YouGetAmount { get; private set; }
        public int YouGiveAmount { get; private set; }
        public bool YouGiveAmountChanged { get; private set; }

        private IMagicCardList _magicCardList;
        private IMagicCardList MagicCardList { get { return _magicCardList ?? (_magicCardList = IoC.Resolve<IMagicCardList>()); } }

        public TradeHandler() : this(IoC.Resolve<IPixelBasedVariables>(), IoC.Resolve<IWindowManager>(), IoC.Resolve<IMessageHandler>(), IoC.Resolve<IApplicationSettings>()) { }

        public TradeHandler(IPixelBasedVariables pbv, IWindowManager windowManager, IMessageHandler messageHandler, IApplicationSettings applicationSettings)
        {
            CardsYouGet = new Dictionary<int, MagicCard>();
            CardsYouGive = new Dictionary<int, MagicCard>();
            _pbv = pbv;
            _windowManager = windowManager;
            _messageHandler = messageHandler;
            _applicationSettings = applicationSettings;
            _deckFileLocation = _applicationSettings.DeckFileLocation;
        }

        public Dictionary<int, MagicCard> DetermineCards(List<Square> cardLocations, Square setPosition, Square numberPosition, int slowDownAmount = 100, int currentCardCount = 0, int maxCardCount = 75)
        {
            Ocr ocr = new Ocr();

            _windowManager.CloseTradeCancelledDialog();

            AutoItX.Sleep(100);
            var firstPosition = cardLocations[0].MidPoint;
            AutoItX.MouseClick(firstPosition);
            AutoItX.Sleep(100);

            AutoItX.Send(Constants.HomeKey);
            AutoItX.Sleep(250);

            AutoItX.MouseMove(firstPosition.X,
                              firstPosition.Y - 35);

            Square currentCard = cardLocations[0].Copy();
            int lastCardNameChecksum = 0;
            int lastCardSetChecksum = 0;
            int currentCardSetChecksum = 1;
            int currentCardNameChecksum = 1;

            int i = 0;

            while ((!lastCardNameChecksum.Equals(currentCardNameChecksum) 
                || !lastCardSetChecksum.Equals(currentCardSetChecksum)) 
                   && currentCardCount++ <= maxCardCount)
            {
                var j = 0;
                while (AutoItX.PixelGetColor(currentCard.Copy().MoveAlongYAxis(1).TopLeft) != _pbv.CardSelectedColor &&
                       j < 50)
                {
                    AutoItX.Sleep(50);
                    if (j == 10)
                    {
                        if (i < 4)
                        {
                            return SplitCombinedImageFile(ocr, true);
                        }
                    }

                    if (++j == 50)
                    {
                        AutoItX.Send(Constants.DownArrow);
                        AutoItX.Sleep(100);
                        if (AutoItX.PixelGetColor(currentCard.TopLeft) != _pbv.CardSelectedColor)
                        {
                            return SplitCombinedImageFile(ocr, true);
                        }
                    }
                }

                lastCardNameChecksum = currentCardNameChecksum;
                currentCardNameChecksum = AutoItX.PixelChecksum(currentCard);

                lastCardSetChecksum = currentCardSetChecksum;
                currentCardSetChecksum = AutoItX.PixelChecksum(setPosition);

                ocr.AddToCollectionImage(currentCard, numberPosition, setPosition);
                AutoItX.Send(Constants.DownArrow);

                if (++i < cardLocations.Count)
                {
                    currentCard = cardLocations[i].Copy();
                    setPosition.MoveTopEdgeTo(currentCard.TopEdge);
                    setPosition.MoveBottomEdgeTo(currentCard.BottomEdge);
                    numberPosition.MoveTopEdgeTo(currentCard.TopEdge);
                    numberPosition.MoveBottomEdgeTo(currentCard.BottomEdge);
                }
                else
                {
                    AutoItX.Sleep(slowDownAmount);
                }
            }

            return SplitCombinedImageFile(ocr, true);
        }

        public void DetermineCardsYouGet()
        {
            int i = 0;
            int slowDownAmount = 50;
            DetermineYouGetAmount();
            var youGetAmount = 0;
            while (youGetAmount != this.YouGetAmount ||
                   this.CardsYouGet.Values.Sum(p => p.CopiesOfCard) != this.YouGetAmount &&
                   i++ < 2
                   && _windowManager.InTrade())
            {
                youGetAmount = this.YouGetAmount;
                CardsYouGet = DetermineCards(_pbv.YouGetCardLocations,
                                             _pbv.YouGetCardSet.Copy(),
                                             _pbv.YouGetCardNumber.Copy(), 
                                             slowDownAmount
                                             );

                DetermineYouGetAmount();
                if (youGetAmount != this.YouGetAmount || this.CardsYouGet.Values.Sum(p => p.CopiesOfCard) != this.YouGetAmount)
                {
                    i = 0;
                    slowDownAmount += 50;
                }

            }
        }

        public void DetermineUsersSelection()
        {
            //TODO: Figure out why this doesn't appear to be working on 'Done' its always true
            if (YouGiveAmountChanged)
            {
                _logger.Trace("Determining Users Selection.");
                var preCheckCards = CardsYouGive.Values.Select(p => new MagicCard(p)).ToList();
                DetermineCardsYouGive();

                bool cardSelectionHasChanged = preCheckCards.Count == 0 ||
                    preCheckCards.Any(
                        card =>
                        !CardsYouGive.ContainsKey(card.NumberId) ||
                        CardsYouGive[card.NumberId].CopiesOfCard != card.CopiesOfCard)
                        ;

                if (cardSelectionHasChanged)
                {
                    _messageHandler.SendPriceMessage(CardsYouGive.Values.ToList(), true);

                    if (CardsYouGive.Count > 0)
                    {
                        _messageHandler.SendMessage(string.Format("Total: {0} tickets",
                                                                  CardsYouGive.Values.Sum(
                                                                      p => p.SellPrice*p.CopiesOfCard)));
                        _messageHandler.SendMessage("Type 'done' if you are done selecting cards.");
                    }
                }
                _youGiveCardListChecksum = AutoItX.PixelChecksum(_pbv.YouGiveCardListChecksumArea);
                YouGiveAmountChanged = false;
            }
        }

        public Dictionary<int, MagicCard> ExamineCollection()
        {
            Square cardNumber = _pbv.FirstCardInListViewNumber.Copy();
            Square cardSet = _pbv.FirstCardInListViewSet.Copy();
            Square cardName = _pbv.FirstCardInListViewName.Copy();
            List<int> bottomRightValues = _pbv.CardInListViewBottomRightValues;
            Square lastCard = _pbv.LastCardInListViewAfterArray.Copy();

            var cardLocations = new List<Square>();
            cardLocations.Add(cardName);
            var moveTarget = cardName.Copy();

            for (int i = 1; i < bottomRightValues.Count; i++)
            {
                moveTarget = cardName.Copy().MoveTopEdgeTo(moveTarget.BottomEdge).MoveBottomEdgeTo(bottomRightValues[i]);
                cardLocations.Add(moveTarget);
            }
            cardLocations.Add(lastCard);
            //_logger.Trace(cardLocations.Select(p => string.Format("Left: {0} Top: {1} Right: {2} Bottom: {3}", p.LeftEdge, p.TopEdge, p.RightEdge, p.BottomEdge)).JoinToString());
            return DetermineCards(cardLocations, cardSet, cardNumber).Where(p => p.Value.Set != "VAN" && p.Value.MtgoId > 0).ToDictionary(p => p.Key, c => c.Value);
        }

        public Dictionary<int, MagicCard> ExamineYouGiveCardsOnConfirmedScreen()
        {
            Square cardNumber = _pbv.ConfirmYouGiveNumber.Copy();
            Square cardSet = _pbv.ConfirmYouGiveSet.Copy();
            Square cardName = _pbv.ConfirmYouGiveCardName.Copy();
            List<int> bottomRightValues = _pbv.ConfirmYouGiveArray;
            Square lastCard = _pbv.ConfirmYouGiveAfterArray.Copy();

            var cardLocations = new List<Square> {cardName};
            var moveTarget = cardName.Copy();

            for (int i = 1; i < bottomRightValues.Count; i++)
            {
                moveTarget = cardName.Copy().MoveTopEdgeTo(moveTarget.BottomEdge + 1).MoveBottomEdgeTo(bottomRightValues[i]);
                cardLocations.Add(moveTarget);
            }
            cardLocations.Add(lastCard);
            
            int j = 0;
            int slowDownAmount = 50;
            DetermineYouGetAmount();
            var youGiveAmount = 0;
            int determineYouGiveConfirmedAmount = this.DetermineYouGiveConfirmedAmount();
            while (youGiveAmount != determineYouGiveConfirmedAmount ||
                   this.CardsYouGive.Values.Sum(p => p.CopiesOfCard) != determineYouGiveConfirmedAmount &&
                   j++ < 2
                   && _windowManager.InTrade() 
                   && slowDownAmount < 500)
            {
                youGiveAmount = determineYouGiveConfirmedAmount;
                CardsYouGive = DetermineCards(cardLocations, cardSet, cardNumber, slowDownAmount);

                DetermineYouGetAmount();
                if (youGiveAmount != determineYouGiveConfirmedAmount || this.CardsYouGive.Values.Sum(p => p.CopiesOfCard) != determineYouGiveConfirmedAmount)
                {
                    j = 0;
                    slowDownAmount += 50;
                }

            }

            return DetermineCards(cardLocations, cardSet, cardNumber);
        }

        public void MoveTypeColumn(Point startPosition, Point endPosition)
        {
            AutoItX.MouseMove(startPosition);
            AutoItX.Sleep(500);
            AutoItX.MouseDown(Constants.LeftMouseButton);
            AutoItX.Sleep(500);
            AutoItX.MouseMove(endPosition);
            AutoItX.Sleep(100);
            AutoItX.MouseUp(Constants.LeftMouseButton);
        }

        public void DetermineYouGetAmount()
        {
            var ocr = new Ocr();
            ocr.AddToCollectionImage(_pbv.YouGetAmountNumber, _pbv.YouGetAmountBlankSpace, _pbv.YouGetAmountName);
            var text = ocr.ExtractTextFromScreen();

            try
            {
                YouGetAmount = GetAmountFromString(text);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Conversion of you get amount failed for: " + text);
                _windowManager.CloseTradeCancelledDialog();
            }
        }

        public int DetermineYouGetConfirmedAmount()
        {
            var ocr = new Ocr();
            ocr.AddToCollectionImage(_pbv.YouGetAmountNumberConfirmed, _pbv.YouGetAmountBlankSpaceConfirmed, _pbv.YouGetAmountNameConfirmed);
            var text = ocr.ExtractTextFromScreen();

            try
            {
                return GetAmountFromString(text);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Conversion of you get amount failed on confirmation screen for: " + text);
                _windowManager.CloseTradeCancelledDialog();
            }
            return 0;
        }

        public bool DetermineIfYouGiveAmountChanged(bool fireEvent)
        {
            if (YouGiveAmountChanged) return YouGiveAmountChanged;
            SetYouGiveAmountChecksum();
            return !YouGiveAmountChanged ? YouGiveAmountChanged : DetermineYouGiveAmount(fireEvent);
        }

        public int DetermineYouGiveConfirmedAmount()
        {
            var ocr = new Ocr();
            ocr.AddToCollectionImage(_pbv.YouGiveAmountNumberConfirmed, _pbv.YouGiveAmountBlankSpaceConfirmed, _pbv.YouGiveAmountNameConfirmed);
            var text = ocr.ExtractTextFromScreen();
            try
            {
                return GetAmountFromString(text);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Conversion of you give amount failed on confirmation screen for: " + text);
            }
            return 0;
        }

        public void LoadWishList()
        {
            AutoItX.MouseClick(_pbv.LoadWishListButton);

            while (!_windowManager.CheckIfLoadWishListScreenIsVisiable() && _windowManager.InTrade())
            {
                AutoItX.Sleep(1000);
                AutoItX.MouseClick(_pbv.LoadWishListButton);
                _windowManager.CloseTradeCancelledDialog();
            }

            AutoItX.MouseClick(_pbv.WishListFirstFileLocation);
            AutoItX.Sleep(250);
            AutoItX.MouseClick(_pbv.LoadDeckButton);
        }

        public void WriteWishListFile(Dictionary<int, MagicCard> wishList)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(Resources.DeckFileFirstLine);
            sb.AppendLine(Resources.DeckFileSecondLine);
            sb.AppendLine(Resources.DeckFileThirdLine);
            sb.AppendLine(Resources.DeckListFourthLine);

            foreach (MagicCard card in wishList.Values)
            {
                sb.AppendLine(string.Format(Resources.DeckListCardItem, card.MtgoId, card.CopiesOfCard));
            }
            sb.AppendLine(Resources.DeckListEndFile);

            using (StreamWriter outfile =
                new StreamWriter(_deckFileLocation + Resources.DeckListFileName))
            {
                outfile.Write(sb.ToString());
            }
        }

        public void PressConfirmButton()
        {
            if (_windowManager.OnTradeScreen())
            {
                AutoItX.MouseClick(_pbv.ConfirmTradeFirstTime);
            }
        }

        public void PressSecondConfirmButton()
        {
            if (_windowManager.OnTradeConfirmScreen())
            {
                _pbv.ConfirmTradeSecondTime.CheckForOCRValue();
            }
        }

        public void CancelTrade()
        {
            _logger.Trace("Canceling Trade");
            while (_windowManager.InTrade())
            {
                _windowManager.ConfirmCancelledTrade();
                AutoItX.MouseClick(_pbv.CancelTradeButton);
                AutoItX.Sleep(1500);
            }

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            var timespan = new TimeSpan(0, 0, 0, 10);
            while (!_windowManager.CloseTradeCancelledDialog() && stopwatch.Elapsed < timespan)
            {
                AutoItX.Sleep(1000);
            }
            _logger.Trace("Trade Sucessfully Cancelled");
        }

        public void CancelTradeFromConfirmationScreen()
        {
            _logger.Info("Cancelling Trade From Confirmation Screen");
            while (!_windowManager.ConfirmCancelledTrade() && _windowManager.InTrade())
            {
                _windowManager.CloseTradeCancelledDialog();
                AutoItX.MouseClick(_pbv.ConfirmTradeCancelButton);
                AutoItX.Sleep(2000);
            }

            int i = 0;
            while (!_windowManager.CloseTradeCancelledDialog() && i++ < 10)
            {
                AutoItX.Sleep(1000);
            }
        }

        private Dictionary<int, MagicCard> SplitCombinedImageFile(Ocr ocr, bool includeTickets, bool limitAmount = false)
        {
            var listOfCards = new Dictionary<int, MagicCard>();

            var cards = ocr.ExtractTextFromScreen();
            _logger.Trace("Card Image Block Returned: " + cards);
            return SplitCombinedImageString(includeTickets, limitAmount, listOfCards, cards);
        }

        public Dictionary<int, MagicCard> SplitCombinedImageString(bool includeTickets, bool limitAmount, Dictionary<int, MagicCard> listOfCards, string cards)
        {
            var rows = cards.Replace("|", "l").Replace("\r\n", "|").Split('|');

            foreach (string row in rows.Where(row => !string.IsNullOrWhiteSpace(row)))
            {
                GetCard(includeTickets, row, listOfCards, limitAmount);
            }

            return listOfCards;
        }

        public void GetCard(bool includeTickets, string row, Dictionary<int, MagicCard> listOfCards, bool limitAmount = false)
        {
            string cardName;
            int cardNumber;
            string cardSet;
            int length;
            var cardSegments = CardSegments(row, out cardName, out cardNumber, out cardSet, out length);

            try
            {             
                MagicCard card = MagicCardList.GetCard(cardName, cardSet);
                
                if (card.MtgoId > 0 && limitAmount)
                {
                    card.CopiesOfCard = Math.Min(cardNumber,
                                                 Math.Max(_applicationSettings.OwnedLessThan - card.OwnedAmount, 0));
                }
                else
                {
                    card.CopiesOfCard = cardNumber;
                }
                
                if (includeTickets || (card.MtgoId > 0))
                {
                    _logger.Trace("Adding: " + card);
                    listOfCards.Add(card.NumberId, card);
                }
            }
            catch (Exception exception)
            {
                _logger.ErrorFormat(exception, "Cardname: {0}, Cardnumber: {1}, Cardset: {2}, CardSegments: {3}",
                                    cardName, cardSegments[length - 1], cardSegments[length], string.Join(",", cardSegments));
            }
        }

        public List<string> CardSegments(string row, out string cardName, out int cardNumber, out string cardSet, out int length)
        {
            List<string> cardSegments = row.Split(' ').ToList();
            length = cardSegments.Count - 1;

            cardSet = cardSegments[length].Trim();
            bool setIsValid = MagicCardList.FilterSet(cardSet).Count() > 0;

            if (!setIsValid)
            {
                cardSet = (cardSegments[length - 1].Trim() + cardSegments[length].Trim()).Enumerize();
                if (MagicCardList.FilterSet(cardSet).Count() > 0)
                {
                    cardSegments[length] = cardSet;
                    cardSegments.RemoveAt(length - 1);
                    length--;
                }
            }

            int index;
            bool foundNumber = false;
            for (index = 0; index < cardSegments[length - 1].Length; index++)
            {
                if (char.IsNumber(cardSegments[length - 1][index]))
                {
                    foundNumber = true;
                    break;
                }
            }

            if (foundNumber)
            {
                var namepart = cardSegments[length - 1].Substring(0, index);
                var number = cardSegments[length - 1].Substring(index, cardSegments[length - 1].Length - index);
                cardSegments[length - 1] = namepart;
                cardSegments.Insert(length, number);
                length++;
            }

            if (!int.TryParse(cardSegments[length - 1].Trim(), out cardNumber))
            {
                _logger.Trace("Using second int parse algorithm");
                var integerPart = cardSegments[length - 1];
                var namepart = integerPart.Substring(0, integerPart.Length - 1);
                var number = integerPart.Substring(integerPart.Length - 1, 1)
                    .Replace("A", "4")
                    .Replace("l", "1")
                    .Replace("B", "8")
                    .Replace("I", "1")
                    .Replace("T", "7")
                    .Replace("S", "5")
                    .Replace("Z", "2")
                    .Replace("G", "6");
                cardSegments[length - 1] = namepart;
                cardSegments.Insert(length, number);
                length++;
            }
            
            cardName = string.Join(" ", cardSegments.Take(length - 1)).Trim();
            
            try
            {
                cardNumber = int.Parse(cardSegments[length - 1].Trim());
                
            }
            catch (Exception exception)
            {
                _logger.ErrorFormat(exception, "Cardname: {0}, Cardnumber: {1}, Cardset: {2}, CardSegments: {3}",
                                    cardName, cardSegments[length - 1], cardSegments[length], string.Join(",", cardSegments));
            }

            return cardSegments;
        }

        public void DetermineCardsYouGive()
        {
            int i = 0;
            int slowDownAmount = 50;
            DetermineYouGiveAmount(false);
            var youGiveAmount = 0;
            while (youGiveAmount != this.YouGiveAmount ||
                   this.CardsYouGive.Values.Sum(p => p.CopiesOfCard) != this.YouGiveAmount &&
                   i++ < 2
                   && _windowManager.InTrade())
            {
                youGiveAmount = this.YouGiveAmount;
                CardsYouGive = DetermineCards(_pbv.YouGiveCardLocations,
                                              _pbv.YouGiveCardSet.Copy(),
                                              _pbv.YouGiveCardNumber.Copy(),
                                              slowDownAmount);

                DetermineYouGiveAmount(false);
                if (youGiveAmount != this.YouGiveAmount || this.CardsYouGive.Values.Sum(p => p.CopiesOfCard) != youGiveAmount)
                {
                    i = 0;
                    slowDownAmount += 50;
                }
            }

            SetYouGiveAmountChecksum();
        }

        public bool DetermineIfMessageAreaHasChanged()
        {
            var checksum = AutoItX.PixelChecksum(_pbv.TradeMessageBoxChecksumArea);
            var changed = _messageAreaChecksum != checksum;
            _messageAreaChecksum = checksum;
            return changed;
        }

        private int GetAmountFromString(string amountString)
        {
            _logger.TraceFormat("Amount string: <{0}>", amountString);
            var start = amountString.ToLower().IndexOf("y");
            var numberPortion = amountString.Substring(0, start).Replace('O', '0').Replace('U', '0').Replace('I', '1').Replace('l', '1').Enumerize();
            _logger.TraceFormat("Number portion: <{0}>", numberPortion);
            return Convert.ToInt32(numberPortion);
        }

        private void SetYouGiveAmountChecksum()
        {
            var amountChecksum = AutoItX.PixelChecksum(_pbv.YouGiveNumberOfCards);
            var cardAreaChecksum = AutoItX.PixelChecksum(_pbv.YouGiveCardListChecksumArea);
            YouGiveAmountChanged = amountChecksum != _youGiveAmountChecksum || cardAreaChecksum != _youGiveCardListChecksum;
            _youGiveAmountChecksum = amountChecksum;
            _youGiveCardListChecksum = cardAreaChecksum;
        }

        private bool DetermineYouGiveAmount(bool fireEvent)
        {
            int i = 0;
            while (i++ < 10 && _windowManager.InTrade())
            {
                var ocr = new Ocr();
                ocr.AddToCollectionImage(_pbv.YouGiveAmountNumber, _pbv.YouGiveAmountBlankSpace, _pbv.YouGiveAmountName);
                var text = ocr.ExtractTextFromScreen();
                try
                {
                    YouGiveAmount = GetAmountFromString(text);
                    return YouGiveAmountChanged;
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Conversion of you give amount failed for: " + text);
                    _windowManager.CloseTradeCancelledDialog();
                }
                finally
                {
                    if (YouGiveAmountChangedEvent != null && fireEvent)
                    {
                        YouGiveAmountChangedEvent(this, EventArgs.Empty);
                    }
                }
            }
            return YouGiveAmountChanged;
        }
    }
}