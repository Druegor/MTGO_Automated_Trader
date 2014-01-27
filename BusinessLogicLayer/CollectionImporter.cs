using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLogicLayer.Models;
using CardDataLayer.Models;
using CardDataLayer.Repositories;
using Framework;
using Framework.Connections;
using Framework.Extensions;
using Framework.Interfaces;
using Framework.Logging;
using LinqToExcel;

namespace BusinessLogicLayer
{
    public class CollectionImporter
    {
        private static readonly ILogger Logger = IoC.GetLoggerFor<CollectionImporter>();
	    private static readonly BotRepository BotRepository = IoC.Resolve<BotRepository>();
	    private static readonly MagicCardRepository CardRepository = IoC.Resolve<MagicCardRepository>();

        public static void ImportCardsFromFile(string filename, int botGroupId, string botname)
        {
			var bot = BotRepository.GetBot(botname);
			var cards = Read(filename);

			using (IConnection connection = Connection.NewConnection())
			{
				connection.BeginTransaction();
				foreach (var model in cards)
				{
					try
					{
						var card = CardRepository.GetCardForBotGroup(bot.GroupId, model.CardName, model.Set, model.Premium) ??
						           new MagicCard(model.CardName, model.Set, model.Premium, model.Rarity)
							           {BuyPrice = -1, SellPrice = 999, BotGroupId = bot.GroupId};

						card.OwnedAmount += model.Online;

						if (card.CardSetNumber == 0 && !string.IsNullOrWhiteSpace(model.SetNumber))
						{
							int cardSetNumberOut;
							int.TryParse(model.SetNumber, out cardSetNumberOut);
							card.CardSetNumber = cardSetNumberOut;
						}

						card.Save(connection);

						var botCard = CardRepository.GetCardForBot(bot.Id, card.Id) ??
						              new BotCard {BotId = bot.Id, CardId = card.Id};

						botCard.OwnedAmount = model.Online;
						botCard.Save(connection);
					}
					catch (Exception ex)
					{
						Logger.Error(ex, "Error on line: " + model);
					}
				}

				connection.CommitTransaction();
			}
        }

        public static IList<CollectionImportModel> Read(string fileName)
        {
            var data = new ExcelQueryFactory(fileName);
            SetupColumnMapping(data);
            SetupTransforms(data);
            return data.Worksheet<CollectionImportModel>().ToList();
        }

        private static void SetupTransforms(ExcelQueryFactory data)
        {
            data.AddTransformation<CollectionImportModel>(x => x.Premium, cellValue => cellValue == "Yes");
            data.AddTransformation<CollectionImportModel>(x => x.Rarity, cellValue => cellValue.ParseEnum<CardRarity>());
            data.AddTransformation<CollectionImportModel>(x => x.SetNumber,
                                                          cellValue => !string.IsNullOrWhiteSpace(cellValue)
                                                                           ? cellValue.Substring(0, cellValue.IndexOf('/') > 0 ? cellValue.IndexOf('/') : 0)
                                                                           : string.Empty);
        }

        private static void SetupColumnMapping(ExcelQueryFactory data)
        {
            data.AddMapping<CollectionImportModel>(p => p.CardName, "Card Name");
            data.AddMapping<CollectionImportModel>(p => p.ForTrade, "For Trade");
            data.AddMapping<CollectionImportModel>(p => p.SetNumber, "No");
        }

        public static void UpdateSetNumber(string filename)
        {
	        /*
            var dataContext = new MagicOnlineBotDb();
            var collectionImportModels = Read(filename);

            foreach (var model in collectionImportModels)
            {
                try
                {
                    var cards = dataContext.Cards.Where(p => p.CardName == model.CardName && p.CardSet == model.Set);

                    if (cards.Count() < 1)
                    {
                        var card = MagicCardList.GetCardViaSuggestion(model.CardName, new List<string> {model.Set},
                                                                       new[]
                                                                           {
                                                                               ((RaritySet) ((int) model.Rarity)).
                                                                                   ToString()
                                                                           });
                        Logger.TraceFormat("Finding {0} => {1}", model.CardName, card.Name);
                        cards = dataContext.Cards.Where(p => p.CardName == card.Name && p.CardSet == card.Set);
                    }

                    if (!string.IsNullOrWhiteSpace(model.SetNumber))
                    {
                        int cardSetNumber;
                        int.TryParse(model.SetNumber, out cardSetNumber);
                        foreach (var card in cards)
                        {
                            card.CardSetNumber = cardSetNumber;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "Error on line: " + model);
                }
            }

            dataContext.SubmitChanges();
			 */
        }
    }
}
