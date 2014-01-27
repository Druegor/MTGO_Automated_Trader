using System;
using System.Drawing;
using System.Linq;
using AutoItBot.PixelBasedVariables;
using BusinessLogicLayer;
using BusinessLogicLayer.Interfaces;
using Framework;
using Framework.Extensions;
using Framework.Logging;

namespace AutoItBot.Handles
{
    public class TradeFilterHandler : ITradeFilterHandler
    {
        private ILogger _logger = IoC.GetLoggerFor<TradeFilterHandler>();
        private IPixelBasedVariables _pbv;
        private CardSet _currentSelectedSet;
        private RaritySet _currentSelectedRarity;
        private VersionSet _currentSelectedVersion;
        private const string LogMessageFormat = "Current: {0} - New: {1}";

        public TradeFilterHandler() : this(IoC.Resolve<IPixelBasedVariables>()) { }

        public TradeFilterHandler(IPixelBasedVariables pbv)
        {
            _pbv = pbv;
            this.Reset();
        }

        public void PickCardSet(CardSet cardSet)
        {
//            for (int i = 0; i < 5; i++)
//            {
                _logger.TraceFormat(LogMessageFormat, this._currentSelectedSet, cardSet);
                this._currentSelectedSet =
                    (CardSet)PickFilterOption((int)this._currentSelectedSet, (int)cardSet,
                                              _pbv.FilterSetSelectorPosition);

                //var set = _pbv.TradeFilterSetText.GetOCRValue().Enumerize();
                //try
                //{
                //    AutoItX.Sleep(i > 2 ? 5000 : 3000);
                //    var cardSets = Enum.GetNames(typeof (CardSet));
                //    var diffSet = (from s in cardSets
                //                   let diff = Levenshtein.Compute(set, s)
                //                   where diff < 4
                //                   select s).FirstOrDefault();

                //    if (diffSet != null)
                //    {
                //        this._currentSelectedSet = (CardSet) Enum.Parse(typeof (CardSet), diffSet);
                //        if (this._currentSelectedSet != cardSet)
                //        {
                //            _logger.Info("Incorrect filter set for set trying again.");
                //        }
                //        else
                //        {
                //            break;
                //        }
                //    }
                //}
                //catch (Exception ex)
                //{
                //    if (set.StartsWith(CardSet.AllCards.ToString()))
                //    {
                //        this._currentSelectedSet = CardSet.AllCards;
                //        break;
                //    }
                //    _logger.Error(ex, "Exception parsing enum value from screen");
                //}
//            }
        }

        public void PickRarity(RaritySet raritySet)
        {
//            for (int i = 0; i < 5; i++)
//            {
                _logger.TraceFormat(LogMessageFormat, this._currentSelectedRarity, raritySet);
                this._currentSelectedRarity =
                    (RaritySet) PickFilterOption((int) this._currentSelectedRarity, (int) raritySet,
                                                 _pbv.FilterRaritySelectorPosition);

                //try
                //{
                //    AutoItX.Sleep(i > 2 ? 5000 : 2000);

                //    var rarity = _pbv.TradeFilterRarityText.GetOCRValue().Enumerize();
                //    var rarityNames = Enum.GetNames(typeof(RaritySet));
                //    var diffSet = (from r in rarityNames
                //                   let diff = Levenshtein.Compute(rarity, r)
                //                   where diff < 4
                //                   select r).FirstOrDefault();

                //    if (diffSet != null)
                //    {
                //        this._currentSelectedRarity = (RaritySet) Enum.Parse(typeof (RaritySet), diffSet);
                //        if (this._currentSelectedRarity != raritySet)
                //        {
                //            _logger.Info("Incorrect filter set for rarity trying again.");
                //        }
                //        else
                //        {
                //            break;
                //        }
                //    }
                //}
                //catch (Exception ex)
                //{
                //    _logger.Error(ex, "Exception parsing enum value from screen");
                //}
//            }
        }

        public void PickVersion(VersionSet versionSet)
        {
//            for (int i = 0; i < 5; i++)
//            {
                _logger.TraceFormat(LogMessageFormat, this._currentSelectedVersion, versionSet);
                this._currentSelectedVersion =
                    (VersionSet)PickFilterOption((int)this._currentSelectedVersion, (int)versionSet,
                                                 _pbv.FilterVersionsSelectorPosition);

                //var version = _pbv.TradeFilterVersionText.GetOCRValue().Enumerize();
                //try
                //{
                //    AutoItX.Sleep(i > 2 ? 5000 : 1000);
                //    var versionNames = Enum.GetNames(typeof(VersionSet));
                //    var diffSet = (from v in versionNames
                //                   let diff = Levenshtein.Compute(version, v)
                //                   where diff < 4
                //                   select v).FirstOrDefault();

                //    if (diffSet != null)
                //    {
                //        this._currentSelectedVersion = (VersionSet) Enum.Parse(typeof (VersionSet), diffSet);
                //        if (this._currentSelectedVersion != versionSet)
                //        {
                //            _logger.Info("Incorrect filter set for version trying again.");
                //        }
                //        else
                //        {
                //            break;
                //        }
                //    }
                //}
                //catch (Exception ex)
                //{
                //    if (version.StartsWith(VersionSet.Premium.ToString()))
                //    {
                //        this._currentSelectedVersion = VersionSet.Premium;
                //        break;
                //    }
                    
                //    if (version.StartsWith(VersionSet.Packs.ToString()))
                //    {
                //        this._currentSelectedVersion = VersionSet.Packs;
                //        break;
                //    }

                //    _logger.Error(ex, "Exception parsing enum value from screen");
                //}
//            }
        }

        public void PickLessThanFilter()
        {
            _logger.Trace("Setting less than filter.");
            AutoItX.MouseClick(_pbv.FilterOwnedSelectorPosition);

            for (int i = (int)this._currentSelectedSet; i < 10; i++)
            {
                AutoItX.Send(Constants.DownArrow);
            }

            AutoItX.MouseClick(_pbv.FilterBlankSpotPosition);
        }

        public void SetOwnedFilterNumberValue(int value)
        {
            _logger.Trace("Setting owned filter to: " + value);
            AutoItX.DoubleClick(_pbv.FilterOwnedTextBox);
            AutoItX.Sleep(200);
            AutoItX.Send(value.ToString());
            AutoItX.Sleep(200);
            AutoItX.MouseClick(_pbv.FilterBlankSpotPosition);
        }

        public void Reset()
        {
            this._currentSelectedSet = CardSet.AllCards;
            this._currentSelectedRarity = RaritySet.AnyRarity;
            this._currentSelectedVersion = VersionSet.AllVersions;
        }

        private int PickFilterOption(int currentValue, int nextValue, Point selectorPosition)
        {
            if (currentValue == nextValue)
            {
                return currentValue;
            }

            AutoItX.MouseClick(selectorPosition);
            AutoItX.Sleep(500);
            if (nextValue > currentValue)
            {
                for (int i = currentValue; i < nextValue; i++)
                {
                    AutoItX.Send(Constants.DownArrow);
                    AutoItX.Sleep(Math.Max(_pbv.SlowDownValue * 2, 50));
                }
            }
            else
            {
                for (int i = currentValue; i > nextValue; i--)
                {
                    AutoItX.Send(Constants.UpArrow);
                    AutoItX.Sleep(Math.Max(_pbv.SlowDownValue * 2, 50));
                }
            }

            AutoItX.MouseClick(_pbv.FilterBlankSpotPosition);

            return nextValue;
        }
    }
}