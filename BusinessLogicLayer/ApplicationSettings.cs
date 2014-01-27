using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using BusinessLogicLayer.Database;
using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.Models;
using BusinessLogicLayer.Trader;
using CardDataLayer;
using Framework;
using Framework.Logging;

namespace BusinessLogicLayer
{
    public class ApplicationSettings : IApplicationSettings
    {
        private ILogger _logger = IoC.GetLoggerFor<ApplicationSettings>();

        public string ClassifiedMessage { get; private set; }
        public string BotName { get; private set; }
        public int MaxTradeMinutes { get; private set; }
        public string DeckFileLocation { get; set; }
        public int Delay { get; private set; }
        public string BotType { get; private set; }
        public string TradeFileLocation { get; set; }
        public int OwnedLessThan { get; set; }
        public double Commons { get; set; }
        public double Uncommons { get; set; }
        public double Mythics { get; set; }
        public double Rares { get; set; }
        public string TesseractLocation { get; set; }
        public string ExecutableLocation { get; set; }
        public string Password { get; set; }
        public string OtherBots { get; set; }

        enum Setting
        {
            Password = 1,
            BotType,
            MaxTradeLength,
            ClassifiedMessage,
            InstallationDirectory,
            BotDelay,
            LessThanAmount,
            OcrLocation,
            Commons,
            Uncommons,
            Rares,
            Mythics,
            ExecutableLocatioin
        }

        public ApplicationSettings()
        {
            this.BotName = ConfigurationManager.AppSettings["botName"];
            this.Password = ConfigurationManager.AppSettings["password"];
            this.OtherBots = DatabaseInteractions.GetOtherBotNames(this.BotName, true);
            var settings = DatabaseInteractions.GetSettingsForBot(BotName);
            if (settings == null) throw new Exception("Settings for this bot do not exist");
            
            string installationDirectory = settings.Single(p => p.SettingId == (int)Setting.InstallationDirectory).Value;
            this.ClassifiedMessage = settings.Single(p => p.SettingId == (int)Setting.ClassifiedMessage).Value;
            this.MaxTradeMinutes = Convert.ToInt32(settings.Single(p => p.SettingId == (int)Setting.MaxTradeLength).Value);
            this.DeckFileLocation = installationDirectory + "Decks\\";
            this.TradeFileLocation = installationDirectory + "Export\\";
            this.Delay = Convert.ToInt32(settings.Single(p => p.SettingId == (int)Setting.BotDelay).Value);
            this.BotType = settings.Single(p => p.SettingId == (int)Setting.BotType).Value.ToLower();
            this.OwnedLessThan = Convert.ToInt32(settings.Single(p => p.SettingId == (int)Setting.LessThanAmount).Value);
            this.Commons = Convert.ToDouble(settings.Single(p => p.SettingId == (int)Setting.Commons).Value);
            this.Uncommons = Convert.ToDouble(settings.Single(p => p.SettingId == (int)Setting.Uncommons).Value);
            this.Rares = Convert.ToDouble(settings.Single(p => p.SettingId == (int)Setting.Rares).Value);
            this.Mythics = Convert.ToDouble(settings.Single(p => p.SettingId == (int)Setting.Mythics).Value);

            var executingAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            this.TesseractLocation = Path.GetDirectoryName(executingAssembly.Location); //settings.FirstOrDefault(p => p.SettingId == (int)Setting.OcrLocation).Value;

            var ocrLocation = settings.Single(p => p.SettingId == (int)Setting.OcrLocation).Value;
            this.TesseractLocation = ocrLocation; //TesseractLocation = Path.GetDirectoryName(executingAssembly.Location);

            this.ExecutableLocation = settings.Single(p => p.SettingId == (int)Setting.ExecutableLocatioin).Value;
        }

        public ITrader GetTrader(List<CardSet> cardSetSearchOrder, TimeSpan maxTime, string botName, string botType)
        {
            var tradee = InitializeTrade();
            _logger.Trace("Tradee found: " + tradee);

            _logger.Trace("Looking for transfers for " + tradee + " with tradee of " + botName);
            var transfers = DatabaseInteractions.GetTransfers(tradee);
            if (transfers.Any())
            {
                var transfer =
                    transfers.FirstOrDefault(p => p.Tradee.Name.Equals(botName, StringComparison.OrdinalIgnoreCase));
                if (transfer != null)
                {
                    return new TransferBot(cardSetSearchOrder, maxTime, botName, tradee, new TransferModel(transfer));
                }
            }

            switch (botType)
            {
                case "raretrader":
                    return new RareTrader(cardSetSearchOrder, maxTime, botName, tradee);

                case "commonbuyer":
                    return new CommonBuyer(cardSetSearchOrder, maxTime, botName, tradee);

                case "rarebulkbuyer":
                    return new RareBulkBuyer(cardSetSearchOrder, maxTime, botName, tradee);
            }

            throw new ArgumentOutOfRangeException("BotType " + botType + " not found");
        }

        public string InitializeTrade()
        {
            FindAndDockChatWindow();

            if (!this.WinManager.InTrade()) return string.Empty;

            MovingGiveGetColumns();

            if (!this.WinManager.InTrade()) return string.Empty;
            
            return DetermineTradee();
        }

        private void FindAndDockChatWindow()
        {
            _logger.Trace("Docking chat window.");
            AutoItX.Sleep(2500);
            int exceptionCounter = 0;
            while (exceptionCounter++ < 30 && this.WinManager.InTrade())
            {
                bool found = this.WinManager.FindTradeChatWindowAndDock();
                bool docked = !this.WinManager.FindTradeChatWindowAndDock();

                if (!found || docked) break;
                _logger.TraceFormat("Docking operation failed {0} times", exceptionCounter);
                AutoItX.Sleep(200);
            }

            if (!this.WinManager.InTrade()) return;
            _logger.Trace("Docked.");
            this.MessageHandler.SendMessage("Loading bot settings...");
            this.MessageHandler.SendMessage("Please feel free to let me know how things go at http://www.druegor.com");
        }

        private string DetermineTradee()
        {
            int i = 0;
            var tradee = string.Empty;
            while (string.IsNullOrWhiteSpace(tradee) && this.WinManager.InTrade())
            {
                _logger.Trace("Trying to find tradee.");
                tradee = this.MessageHandler.GetTradeeName(this.BotName);
                if (string.IsNullOrWhiteSpace(tradee))
                {
                    AutoItX.Sleep(2000);
                    if (i++ > 10)
                    {
                        _logger.Error("Unable to determine tradee name cancelling trade.");
                        this.MessageHandler.SendMessage("Unable to determine user cancelling trade.");
                        this.TradeHandler.CancelTrade();
                        return string.Empty;
                    }
                }
            }

            return tradee;
        }
        
        private void MovingGiveGetColumns()
        {
            _logger.Trace("Moving give and get columns to show SET.");
            this.TradeHandler.MoveTypeColumn(Pbv.YouGetTypeColumnStartPosition, Pbv.YouGetTypeColumnEndPosition);
            this.TradeHandler.MoveTypeColumn(Pbv.YouGiveTypeColumnStartPosition, Pbv.YouGiveTypeColumnEndPosition);
        }
        
        private IWindowManager _windowManager;
        public IWindowManager WinManager
        {
            get { return _windowManager ?? (_windowManager = IoC.Resolve<IWindowManager>()); }
        }

        private IMessageHandler _messageHandler;
        public IMessageHandler MessageHandler
        {
            get { return _messageHandler ?? (_messageHandler = IoC.Resolve<IMessageHandler>()); }
        }

        private ITradeHandler _tradeHandler;
        public ITradeHandler TradeHandler
        {
            get { return _tradeHandler ?? (_tradeHandler = IoC.Resolve<ITradeHandler>()); }
        }

        private IPixelBasedVariables _pbv;
        public IPixelBasedVariables Pbv
        {
            get { return _pbv ?? (_pbv = IoC.Resolve<IPixelBasedVariables>()); }
        }
    }
}
