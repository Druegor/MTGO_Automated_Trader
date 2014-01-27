using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BusinessLogicLayer;
using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.MagicCards;
using BusinessLogicLayer.OCR;
using CardDataLayer;
using Framework;
using Framework.Logging;

namespace AutoItBot.Updater
{
    public class TradeFileHandler
    {
        private ILogger _logger = IoC.GetLoggerFor<TradeFileHandler>();

        private const string Header = "Card Name,Set,Premium,For Trade";
        private const string TradeFormat = "{0},{1},{2},{3}";
        private static readonly string TradeFileLocation = IoC.Resolve<IApplicationSettings>().TradeFileLocation;
        private string _tradeFile = TradeFileLocation + "TradeFile.csv";
        private IPixelBasedVariables _pbv = IoC.Resolve<IPixelBasedVariables>();

        public void WriteTradeFile(List<MagicCard> tradeList)
        {
            _logger.Trace("Writing csv to import for tradable amounts.");
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(Header);

            foreach (MagicCard card in tradeList)
            {
                sb.AppendLine(string.Format(TradeFormat, card.Name.Contains(',') ? "\"" + card.Name + "\"" : card.Name, card.Set, card.Premium ? "Yes" : "No", card.CopiesOfCard));
            }

            using (StreamWriter outfile = new StreamWriter(_tradeFile))
            {
                outfile.Write(sb.ToString());
            }
        }

        public void MarkCardsInExcessTradable(int excess, int max, string botName)
        {
            MarkCardsInExcessTradable(excess, max, botName,
                                      new List<RaritySet>()
                                          {RaritySet.Common, RaritySet.Uncommon, RaritySet.Rare, RaritySet.Mythic});
        }

        public void MarkCardsInExcessTradable(int excess, int max, string botName, List<RaritySet> rarities)
        {
            var dataContext = new MagicOnlineBotDb();
            var bot = dataContext.TradeBots.Single(p => p.Name == botName);

            var botCards =
                dataContext
                    .BotCards
                    .Where(p => p.BotId == bot.BotId
                                && p.OwnedAmount > excess
                                && rarities.Contains((RaritySet) p.Card.CardRarity))
                    .Select(p => new MagicCard(p.Card, p.OwnedAmount))
                    .ToList();

            WriteTradeFile(excess, max, botCards);
        }

        public void LoadTradableCards()
        {
            _logger.Trace("Loading tradable cards.");
            Constants.SetClipboard(_tradeFile);

            AutoItX.MouseClick(_pbv.CollectionTab);
            AutoItX.Sleep(2000);

            AutoItX.MouseClick(_pbv.CollectionListView);
            AutoItX.Sleep(2000);

            Point collectionFirstCard1 = _pbv.CollectionFirstCard;
            AutoItX.MouseClick(Constants.RightMouseButton, collectionFirstCard1.X, collectionFirstCard1.Y);
            AutoItX.Sleep(2000);

            Square importData1 = _pbv.ImportTradableOcr;
            var importOcr = importData1.GetOCRValue();

            if (Levenshtein.Compute(importOcr.ToLower(), Constants.ImportTradeCsv.ToLower()) < 4 )
            {
                AutoItX.MouseClick(importData1.MidPoint);
                AutoItX.Sleep(1000);

                AutoItX.MouseClick(_pbv.ImportFileNameTextBox);
                AutoItX.Sleep(200);
                AutoItX.Send(Constants.PasteCommand);
                AutoItX.Sleep(200);
                AutoItX.Send(Constants.EnterKey);
                AutoItX.Sleep(5000);
            }
        }

        public void ClearTradableCards(string botName)
        {
            _logger.Trace("Clearing tradable cards.");
            var dataContext = new MagicOnlineBotDb();
            var cards = dataContext.Cards;

            WriteTradeFile(0, 0, cards.Select(p => new MagicCard(p, 0)).ToList());
            LoadTradableCards();
            SetEventTicketsTradable(false);
        }

        public void SetEventTicketsTradable(bool markTradable)
        {
            Constants.SetClipboard(Constants.EventTicket);
            AutoItX.DoubleClick(_pbv.CollectionSearchBox);
            AutoItX.Sleep(200);
            AutoItX.Send(Constants.PasteCommand);
            AutoItX.Sleep(200);
            AutoItX.Send(Constants.EnterKey);
            AutoItX.Sleep(5000);

            AutoItX.MouseClick(_pbv.CollectionFirstCard);
            AutoItX.Sleep(1000);
            AutoItX.MouseClick(Constants.RightMouseButton, _pbv.CollectionFirstCard.X, _pbv.CollectionFirstCard.Y);
            AutoItX.Sleep(2000);

            Square ocr = markTradable ? _pbv.MarkSelectionAllTradable : _pbv.MarkSelectionAllUntradable;
            String textMatch = markTradable ? Constants.MarkSelectionTradeable : Constants.MarkSelectionUnTradeable;

            _logger.Trace("Attempting to get section for: " + ocr);
            var tradableOcr = ocr.GetOCRValue();
            _logger.Trace("Tradable selection = " + tradableOcr);

            if (Levenshtein.Compute(tradableOcr.ToLower(), textMatch.ToLower()) < 4)
            {
                AutoItX.MouseClick(ocr.MidPoint);
                AutoItX.Sleep(1000);
            }
        }

        private void WriteTradeFile(int excess, int max, List<MagicCard> botCards)
        {
            foreach (MagicCard magicCard in botCards)
            {
                var copies = (magicCard.OwnedAmount - excess);
                magicCard.CopiesOfCard = copies > max ? max : copies;
            }

            if (botCards.Any(p => p.NumberId == 1))
            {
                MagicCard magicCard = botCards.Single(p => p.NumberId == 1);
                magicCard.CopiesOfCard = magicCard.OwnedAmount;
            }

            WriteTradeFile(botCards);
        }
    }
}