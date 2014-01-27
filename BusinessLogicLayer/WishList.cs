using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.Properties;
using CardDataLayer.Models;
using CardDataLayer.Repositories;
using Framework;
using Framework.Interfaces;

namespace BusinessLogicLayer
{
    public class WishList
    {
		private static readonly string DeckFileLocation = IoC.Resolve<IApplicationSettings>().DeckFileLocation;
		private readonly MagicCardRepository _magicCardRepository = IoC.Resolve<MagicCardRepository>();
		private readonly PriceRepository _priceRepository = IoC.Resolve<PriceRepository>();

        public List<DeckCard> DeckCards { get; set; }
        public List<MagicCard> CollectionCards { get; set; }
		
		public class DeckCard
        {
            public int CardID { get; set; }
            public int ColumnID { get; set; }
        }

        public void LoadDeckFile()
        {
            var xmlSource = XDocument.Load(@"C:\Users\Druegor\AppData\Roaming\Wizards of the Coast\Magic Online\3.0\Decks\0WishList.dek");
            DeckCards =
                xmlSource
                    .Descendants("Cards")
                    .Select(c =>
                            new DeckCard
                                {
                                    CardID = int.Parse(c.Attribute("CatID").Value),
                                    ColumnID = int.Parse(c.Attribute("Col").Value)
                                })
                    .OrderBy(p => p.ColumnID)
                    .ToList();
        }

        public void LoadCardListFile(string filename)
        {
            filename = @"C:\Users\Druegor\AppData\Roaming\Wizards of the Coast\Magic Online\3.0\Export\" +
                       filename + ".csv";

            try
            {
                var cards = CollectionImporter.Read(filename);
				
                LoadDeckFile();
	            CollectionCards = cards
		            .Where(p => string.IsNullOrWhiteSpace(p.Set))
		            .Select(model => new MagicCard(model.CardName, model.Set, model.Premium, model.Rarity))
		            .ToList();

                var count = DeckCards.Count;
                if (count != CollectionCards.Count)
                {
                    throw new InvalidDataException("Deck file and collection file do not have same card count.");
                }

                CollectionCards = CollectionCards.OrderBy(p => p.Name).ToList();

                for (var i = 0; i < count; i++)
                {
                    var card = CollectionCards[i];
                    var deckItem = DeckCards[i];
	                card.MtgoId = deckItem.CardID;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

		public void WriteWishListFile(List<MagicCard> wishList)
        {
            var sb = new StringBuilder();
            sb.AppendLine(Resources.DeckFileFirstLine);
            sb.AppendLine(Resources.DeckFileSecondLine);
            sb.AppendLine(Resources.DeckFileThirdLine);
            sb.AppendLine(Resources.DeckListFourthLine);

            foreach (var card in wishList)
            {
                sb.AppendLine(string.Format(Resources.DeckListCardItem, card.MtgoId, 1));
            }

            sb.AppendLine(Resources.DeckListEndFile);

            using (var outfile = new StreamWriter(DeckFileLocation + "1WishListTest.dek"))
            {
                outfile.Write(sb.ToString());
            }
        }

        public void WriteToDatabase()
        {
			using (var connection = IoC.Resolve<IConnection>())
			{
				connection.BeginTransaction();
				
				foreach (var magicCard in CollectionCards)
				{
					var dbCard = _magicCardRepository.Get(magicCard);

					if (dbCard == null)
					{
						var card = new MagicCard(magicCard.Name, magicCard.Set, magicCard.Premium, magicCard.Rarity)
							{BuyPrice = -1, SellPrice = 999, OwnedAmount = 0};
						
						_magicCardRepository.Save(card);
						_priceRepository.Save(card);
					}
					else
					{
						_magicCardRepository.Update(magicCard);
					}
				}

				connection.CommitTransaction();
			}
        }
    }
}