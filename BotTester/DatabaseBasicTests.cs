using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using BusinessLogicLayer.Database;
using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.MagicCards;
using CardDataLayer;
using CardDataLayer.Models;
using CardDataLayer.Repositories;
using FluentAssertions;
using Framework;
using NUnit.Framework;

namespace BotTester
{
    [TestFixture]
    public class DatabaseBasicTests
    {
        private string _botName = " ";

        [Test]
        public void TestPixelLocationsRetrieval()
        {
            var list = IoC.Resolve<PixelLocationsRepository>().Get();
            list.Should().NotBeNull();
            list.Should().NotBeEmpty();
        }             

        [Test]
        public void TradeeAndBotCreditIsCreatedIfDoNotExist()
        {
            string tradee = DateTime.Now.Ticks.ToString();
            BotCredit botCredit = DatabaseInteractions.GetTradeCredits(tradee, _botName);
            botCredit.Should().NotBeNull();
            botCredit.NumOfTrades.Should().Be(1);
            botCredit.Credit.Should().Be((decimal)0.0);

            botCredit = DatabaseInteractions.GetTradeCredits(tradee, _botName);
            botCredit.Should().NotBeNull();
            botCredit.NumOfTrades.Should().Be(2);
            botCredit.Credit.Should().Be((decimal)0.0);
            
            //Cleanup
            var dataContext =
                new MagicOnlineBotDb();
            
            var tradeeRecord = dataContext.Tradees.Single(p => p.Name == tradee);
            var botCreditRecord = dataContext.BotCredits.Single(p => p.TradeeId == tradeeRecord.TradeeId);

            dataContext.BotCredits.DeleteOnSubmit(botCreditRecord);
            dataContext.Tradees.DeleteOnSubmit(tradeeRecord);
            dataContext.SubmitChanges();
        }

        [Test]
        public void SaveCompletedTradeLogUpdatesTradeCreditsOnCompletion()
        {
            string tradee = DateTime.Now.Ticks.ToString();
            BotCredit botCredit = DatabaseInteractions.GetTradeCredits(tradee, _botName);

            var creditAfterTrade = 0.375m;
            DatabaseInteractions.SaveCompletedTradeLog(_botName, tradee, new List<MagicCard>(), new List<MagicCard>(), 0.0m, creditAfterTrade);
            botCredit = DatabaseInteractions.GetTradeCredits(tradee, _botName);
            botCredit.Credit.Should().Be(creditAfterTrade);
            
            //Cleanup
            var dataContext =
                new MagicOnlineBotDb(
                    ConfigurationManager.ConnectionStrings[
                        "CardDataLayer.Properties.Settings.MagicOnlineBotConnectionString"].
                        ToString());
            
            var tradeeRecord = dataContext.Tradees.Single(p => p.Name == tradee);
            var botCreditRecord = dataContext.BotCredits.Single(p => p.TradeeId == tradeeRecord.TradeeId);
            var tradeRecord = dataContext.Trades.Single(p => p.TradeeId == tradeeRecord.TradeeId);

            dataContext.Trades.DeleteOnSubmit(tradeRecord);
            dataContext.BotCredits.DeleteOnSubmit(botCreditRecord);
            dataContext.Tradees.DeleteOnSubmit(tradeeRecord);
            dataContext.SubmitChanges();
        }
        
        [Test]
        public void OwnedAmountIsPopulatedInCardList()
        {
            var dataContext = new MagicOnlineBotDb();

            var card = dataContext.Prices.First(p => p.CardId == 20956);
            var magicCard = new MagicCard(card);
            Console.WriteLine("You get - " + magicCard + " OwnedAmount: " + card.OwnedAmount);
            card.OwnedAmount += 7;
            dataContext.SubmitChanges();

            var wishListCards = IoC.GetInstance<IMagicCardList>().GetComprehensiveCommonsAndUncommons(8, 2);
            Console.WriteLine(wishListCards.FirstOrDefault(p => p.Value.OwnedAmount > 0));

            card = dataContext.Prices.First(p => p.CardId == 12);
            magicCard = new MagicCard(card);
            Console.WriteLine("You get - " + magicCard + " OwnedAmount: " + card.OwnedAmount);
            card.OwnedAmount -= 7;
            dataContext.SubmitChanges();

            IoC.GetInstance<IMagicCardList>().InvalidateCache();

            wishListCards = IoC.GetInstance<IMagicCardList>().GetComprehensiveCommonsAndUncommons(8, 2);
            Console.WriteLine(wishListCards.FirstOrDefault(p => p.Value.OwnedAmount > 0));
        }

        [Test]
        public void GetCardValue()
        {
            var dataContext = new MagicOnlineBotDb();

            var card = dataContext.Prices.First(p => p.CardId == 20956);
            var magicCard = new MagicCard(card);
            Console.WriteLine("You get - " + magicCard + " OwnedAmount: " + card.OwnedAmount);
        }
    }
}