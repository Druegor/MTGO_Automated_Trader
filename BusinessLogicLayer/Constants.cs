using System;
using System.Windows.Forms;
using Framework;
using Framework.Logging;

namespace BusinessLogicLayer
{
    public class Constants
    {
        public const string MagicOnlineProgramName = "Magic Online";
        public const string EventTicket = "Event Ticket";
        public const string ConnectionProblem = "Connection Problem";
        public const string RightMouseButton = "right";
        public const string Logon = "Log On";
        public const string AcceptUla = "I Accept";
        public const string DoneMessage = "done";
        public const string LeftMouseButton = "left";
        public const string DownArrow = "{DOWN}";
        public const string UpArrow = "{UP}";
        public const string EnterKey = "{ENTER}";
        public const string HomeKey = "{HOME}";
        public const string EndKey = "{END}";
        public const string CopyCommand = "^C";
        public const string PasteCommand = "^V";
        public const int MaxMessageLength = 1000;

        public const string DeckEditorTitle = "CARD Deck Editor";
        public const string LoadDeckSearchText = "load deck";
        public const string SystemAlertText = "system";
        public const string NewCardsText = "new";
        public const string NewCardsFull = "NEW CARDS";
        public const string TradeRequestTextSearchString = "accept";
        public const string TradeCompleteTextSearchString = "trade complete";
        public const string ChallengeText = "Challenge"; //ChaHenge - Tesseract misprint
        public const string HomeScreenText = "HOME";
        public const string CancelTradeText = "Are you sure";
        public const string TradeSessionCancelledText = "canc";
        public const string ConfirmTradeText = "Confirm Trade";
        public const string ClassifiedsTitle = "COMMUNITY Classifieds";
        public const string TradeTitle = "CARD Trade";
        public const string TradeConfirmTitle = "CARD Trade Confirm";
        public const string LaunchButton = "Launch";
        public const string UpdateButton = "Update";
        public const string TradeMenuOption = "TRADE";
        public const string RemoveBuddyMenuOption = "REMOVE BUDDY";

        public const string MarkSelectionTradeable = "Mark selection all tradable";
        public const string MarkSelectionUnTradeable = "Mark selection all untradable";
        public const string ImportTradeCsv = "Import trade data from .csv";

        public static void SetClipboard(string value)
        {
            var logger = IoC.GetLoggerFor<Constants>();
            int i = 0;
            while (i++ < 5)
            {
                try
                {
                    Clipboard.SetText(value);
                    break;
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Clipboard error");
                    AutoItX.Sleep(200);
                }
            }
        }
    }

    public enum VersionSet
    {
        AllVersions = 0,
        Regular,
        Premium,
        Packs
    }

    public enum CardSet
    {
        EVENT = -2,
        Tokens,
        AllCards = 0,
        Standard,
        Extended,
        Classic,
        Legacy,
        Pauper,
        Kaleidoscope,
        ScarsOfMirrodin = 7,
        Zendikar = 11,
        ShardsOfAlara = 15,
        LorwynShadowmoor = 19,
        TimeSpiral = 24,
        IceAge = 29,
        Ravnica = 33,
        Kamigawa = 37,
        Mirrodin = 41,
        Onslaught = 45,
        Odyssey = 49,
        Invasion = 53,
        Urza = 57,
        Tempest = 59,
        Mirage = 63,
        CoreSet = 67,
        M7,
        M8,
        M9,
        X,
        M10,
        M11,
        Masters = 74,
        Special = 79,
        PremiumDecks = 84,
        DuelDecks = 87,
        FromTheVault = 94
    }
}