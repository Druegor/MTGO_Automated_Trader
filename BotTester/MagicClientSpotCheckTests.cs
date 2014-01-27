using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using AutoItBot.Handles;
using BusinessLogicLayer;
using BusinessLogicLayer.Database;
using BusinessLogicLayer.ImageDownloader;
using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.MagicCards;
using BusinessLogicLayer.OCR;
using AutoItBot.Updater;
using BusinessLogicLayer.PriceUpdaters;
using CardDataLayer;
using FluentAssertions;
using Framework;
using Framework.Logging;
using NUnit.Framework;

namespace BotTester
{
    [TestFixture]
    public class MagicClientSpotCheckTests
    {
        private IPixelBasedVariables _screenLocations;
        private ILogger _logger = IoC.GetLoggerFor<MagicClientSpotCheckTests>();

        [SetUp]
        public void Initialize()
        {
            _screenLocations = IoC.Resolve<IPixelBasedVariables>();
        }

        [Test]
        public void TradeHandlerCardSegments()
        {
            var tradeHandler = new TradeHandler();
            string cardName;
            int cardNumber;
            string cardSet;
            int length;
            var cardSegments = tradeHandler.CardSegments("Shrine of Limitless Powerl NPH", out cardName, out cardNumber, out cardSet, out length);
            Console.Out.WriteLine(string.Join(",",cardSegments.ToArray()));

            MagicCard card = IoC.Resolve<IMagicCardList>().GetCard(cardName, cardSet);
            Console.Out.WriteLine(card);

            cardSegments = tradeHandler.CardSegments("Event Ticket 18", out cardName, out cardNumber, out cardSet, out length);
            Console.Out.WriteLine(string.Join(",", cardSegments.ToArray()));

            card = IoC.Resolve<IMagicCardList>().GetCard(cardName, cardSet);
            Console.Out.WriteLine(card);
        }

        [Test]
        public void UpdateCardCountsFromCollectionFiles()
        {
            DatabaseInteractions.ClearOwnedAmountsForBot(1);        }

        [Test]
        public void CollectionImporterMap()
        {
            CollectionImporter.UpdateSetNumber(@"D:\Projects\Bot\csv files\allcards.csv");
        }

        [Test]
        public void WriteTradeableFile()
        {
            TradeFileHandler handler = new TradeFileHandler();
            handler.MarkCardsInExcessTradable(4, 4, " ", new List<RaritySet>() {RaritySet.Rare, RaritySet.Mythic});
        }

        [Test]
        public void LoadTradeableCards()
        {
            TradeFileHandler handler = new TradeFileHandler();
            string botName = " ";

            handler.ClearTradableCards(botName);
            handler.MarkCardsInExcessTradable(4, 4, botName, new List<RaritySet>() { RaritySet.Rare, RaritySet.Mythic });
            handler.LoadTradableCards();
            handler.SetEventTicketsTradable(true);
        }

        [Test]
        public void ImportNewCardSet()
        {
            string cardSet = "MasquesBlock";
            
            WishList wishList = new WishList();
            wishList.LoadDeckFile();
            wishList.LoadCardListFile(cardSet);
            var writeList = new List<MagicCard>();

            Random rand = new Random();
            for (int i = 0; i < 10; i++)
            {
                var index = rand.Next(1, wishList.CollectionCards.Count);
                var card = wishList.CollectionCards[index];
                writeList.Add(card);
            }

            foreach (var magicCard in writeList.OrderBy(p => p.Name))
            {
                Console.WriteLine(magicCard);
            }

            wishList.WriteWishListFile(writeList);

            //var writeList = new List<MagicCard>();

            //Random rand = new Random();
            //for (int i = 0; i < 100; i++)
            //{
            //    writeList = new List<MagicCard>();
            //    var index = rand.Next(1, wishList.CollectionCards.Count);
            //    var card = wishList.CollectionCards[index];
            //    writeList.Add(card);
            //    wishList.WriteWishListFile(writeList);

            //    // Validate single card by opening wishlist file
            //    // then saving out again in textfile mode
            //    // then open text file and validate names match
            //    // Assert.True();
            //}

            //wishList.WriteToDatabase();
        }

        [Test]
        public void SetupTransactionsForBulkBuyers()
        {
            var dataContext = new MagicOnlineBotDb();
            var druegbotCards =
                dataContext.BotCards.Where(
                    p => p.BotId == 4 && !p.Card.Premium && p.Card.MtgoCardId.GetValueOrDefault(0) > 0 && p.Card.CardSet != "DPA").ToList();
            var cardsToTransfer = new List<Card>();

            foreach (var druegbotCard in druegbotCards)
            {
                for (int i = 0; i < druegbotCard.OwnedAmount && i < 12; i++)
                {
                    cardsToTransfer.Add(druegbotCard.Card);
                }
            }

            var count = cardsToTransfer.Count;
            var transferCount = count/75;

            _logger.Trace("Count" + count);
            _logger.Trace("TransferCount" + transferCount);

            for (int i = 0; i < transferCount; i++)
            {
                var transfer = new Transfer
                                   {
                                       Completed = false,
                                       TradeBotId = 4,
                                       TradeeId = 173,
                                       WishList =
                                           string.Join(",",
                                                       cardsToTransfer.Skip(i*75).Take(75).Select(p => p.MtgoCardId))
                                   };

                dataContext.Transfers.InsertOnSubmit(transfer);
            }

            dataContext.SubmitChanges();
        }

        [Test]
        public void GetCommonsAndUncommons()
        {
            Dictionary<int, MagicCard> wishListCards = IoC.Resolve<IMagicCardList>().GetComprehensiveCommonsAndUncommons(8, 2);
            var subset = wishListCards.Where(p => p.Value.Set=="9ED").Select(p => p.Value)
                //.Skip(400)
                //.Take(400)
                ;

            WishList wishList = new WishList();
            wishList.WriteWishListFile(subset.ToList());
        }

        [Test]
        public void WriteDekFileToLoadForUpdatedPricing()
        {
            var dataContext = new MagicOnlineBotDb(ConfigurationManager.ConnectionStrings["CardDataLayer.Properties.Settings.MagicOnlineBotConnectionString"].ToString());
            var cards = dataContext
                .Prices
                .Where(p => p.SellPrice == 999 && p.OwnedAmount > 0)
                .Select(p => new MagicCard(p))
                //.Skip(450)
                .Take(75)
                .ToList();

            WishList wishList = new WishList();
            wishList.WriteWishListFile(cards);
        }

        
        [Test]
        public void ClickMouseAtLocation()
        {
            WindowManager windowManager = new WindowManager();
            var found = windowManager.FindTradeChatWindowAndDock();
            found.Should().Be(true);
        }

        [Test]
        [Explicit]
        public void DetermineCardsYouGive()
        {
            TradeHandler tradeHandler = new TradeHandler();
            tradeHandler.DetermineCardsYouGive();
        }

        [Test]
        [Explicit]
        public void DetermineCardsYouGet()
        {
            TradeHandler tradeHandler = new TradeHandler();
            tradeHandler.DetermineCardsYouGet();
        }        
        
        [Test]
        [Explicit]
        public void ExamineYouGiveCardsOnConfirmedScreen()
        {
            TradeHandler tradeHandler = new TradeHandler();
            var cards = tradeHandler.ExamineYouGiveCardsOnConfirmedScreen();
            foreach (var magicCard in cards)
            {
                Console.WriteLine(magicCard);
            }
        }

        [Test]
        [Explicit]
        public void ExamineCollection()
        {
            TradeHandler tradeHandler = new TradeHandler();
            var cards = tradeHandler.ExamineCollection();
            foreach (var magicCard in cards)
            {
                Console.WriteLine(magicCard);
            }
        }

        [Test]
        public void SetTradeFilterForCardBlock()
        {
            TradeFilterHandler filterHandler = new TradeFilterHandler();
            filterHandler.PickCardSet(BusinessLogicLayer.CardSet.Standard);
            filterHandler.PickLessThanFilter();
            filterHandler.PickRarity(RaritySet.Rare);
            filterHandler.PickVersion(VersionSet.Regular);

            AutoItX.Sleep(2000);
            filterHandler.PickCardSet(BusinessLogicLayer.CardSet.AllCards);
            filterHandler.PickRarity(RaritySet.AnyRarity);
            filterHandler.PickVersion(VersionSet.AllVersions);
            
            AutoItX.Sleep(200);
            AutoItX.MouseClick(_screenLocations.FilterResetButton);
        }

        [Test]
        public void MoveTypeColumns()
        {
            TradeHandler guiInteraction = new TradeHandler();

            guiInteraction.MoveTypeColumn(
                _screenLocations.YouGetTypeColumnStartPosition,
               _screenLocations.YouGetTypeColumnEndPosition);

            guiInteraction.MoveTypeColumn(
                _screenLocations.YouGiveTypeColumnStartPosition,
                _screenLocations.YouGiveTypeColumnEndPosition);
        }

        [Test]
        public void TestForOtherWindows()
        {
            TimeSpan timeSpan = new TimeSpan(0, 3, 0);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            WindowManager windowManager = new WindowManager();

            while (stopwatch.Elapsed < timeSpan)
            {
                windowManager.CheckForOtherWindows();
                windowManager.CheckForTrade();
                AutoItX.Sleep(250);
            }
        }

        [Test]
        public void DetermineLastMessage()
        {
            TimeSpan timeSpan = new TimeSpan(0, 0, 20);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            MessageHandler messageHandler = new MessageHandler();

            string message;
            do
            {
                message = messageHandler.GetLastTradeChatMessage();
                AutoItX.Sleep(1000);
            } while ((!message.Contains(" ") || !message.Contains("done")) && stopwatch.Elapsed < timeSpan);
            messageHandler.SendMessage("Done Found.");
        }

        [Test]
        public void PostClassifiedMessage()
        {
            TradeHandler mtgoGui = new TradeHandler();
            mtgoGui.DetermineCardsYouGet();

            foreach (MagicCard magicCard in mtgoGui.CardsYouGet.Values)
            {
                Console.WriteLine(magicCard);
            }
        }

        [Test]
        [Explicit]
        public void TestAlternateImageFinder()
        {
            IoC.Resolve<IWindowManager>().CloseTradeCancelledDialog();
        }

        [Test]
        public void TestScreenHeaderTextGrab()
        {
            Point screenHeaderTopLeft = new Point(73, 10);
            Point screenHeaderBottomRight = new Point(274, 34);

            Ocr ocr = new Ocr();
            string extractTextFromScreen = ocr.ExtractTextFromScreen(screenHeaderTopLeft, screenHeaderBottomRight);

            _logger.Trace(extractTextFromScreen);
        }

        [Test]
        public void SaveAllImagesForCards()
        {
            var dataContext = new MagicOnlineBotDb();
            var cardSets = dataContext.Cards
                .Select(p => p.CardSet)
                .Distinct()
                .ToList();

            cardSets = cardSets.Where(p => !string.IsNullOrWhiteSpace(p) && p != "DPA").ToList();
            var sets = dataContext.CardSets.ToList();

            foreach (var directory in cardSets)
            {
                string path = @"d:\Projects\Bot\Images\" + directory.Replace("CON", "CFX").ToLower();
                _logger.Trace("Creating directory for " + path);
                if (!Directory.Exists(path))
                {
                    try
                    {
                        Directory.CreateDirectory(path);
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex);
                    }
                   
                }
            }

            foreach (var cardSet in cardSets.Where(p => sets.Any(c => c.CardSetAbrv == p && !string.IsNullOrWhiteSpace(c.WebsiteAbrv))))
            {
                var set = sets.First(c => c.CardSetAbrv == cardSet);
                var max = dataContext.Cards.Count(p => !p.Premium && p.CardSet == cardSet);

                for (int i = 1; i <= max; i++)
                {
                    string filename =
                        string.Format("{0}\\{1}.jpg", set.CardSetAbrv.Replace("CON", "CFX"), i).ToLower();
                    string imageUrl = string.Format("http://magiccards.info/scans/en/{0}/{1}.jpg", set.WebsiteAbrv,
                                                    i).ToLower();

                    var imageDownload = new DownloadImage(imageUrl, filename);
                    imageDownload.Download();
                }
            }
        }

    }
}
