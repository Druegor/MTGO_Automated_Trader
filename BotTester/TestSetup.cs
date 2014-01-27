using System.Collections.Generic;
using AutoItBot;
using AutoItBot.Handles;
using AutoItBot.PixelBasedVariables;
using BusinessLogicLayer;
using BusinessLogicLayer.Interfaces;
using Framework;
using Framework.Logging;
using NUnit.Framework;

namespace BotTester
{
    [SetUpFixture]
    public class TestSetup
    {
        [SetUp]
        public void RunBeforeAnyTests()
        {
            //Note: This test setup works only on my monitor
            IoC.Register<ILoggerFactory>(
                new LoggerFactory(
                    new List<LoggerCtorDelegate>
                        {
                            (type, log) => new Log4NetLogger(type, log)
                        }));

            Log4NetLogger.ConfigureAndWatch("Log4Net.config");


            ApplicationSettings settings = new ApplicationSettings
                                               {
                                                   DeckFileLocation =
                                                       @"C:\Users\Druegor\AppData\Roaming\Wizards of the Coast\Magic Online\3.0\Decks\"
                                               };
            IoC.Register<IPixelBasedVariables>(new ScreenLocations().Load());
            IoC.Register<IApplicationSettings>(settings);
            IoC.Register<IMagicCardList>(new BusinessLogicLayer.MagicCards.MagicCardList());
            IoC.Register<IMessageHandler>(new MessageHandler());
            IoC.Register<IWindowManager>(new WindowManager());
            IoC.Register<ITradeFilterHandler>(new TradeFilterHandler());
            IoC.Register<ITradeHandler>(new TradeHandler(), false);
        }
    }
}