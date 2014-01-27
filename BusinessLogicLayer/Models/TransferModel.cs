using System.Collections.Generic;
using System.Linq;
using BusinessLogicLayer.Interfaces;
using CardDataLayer.Models;
using Framework;

namespace BusinessLogicLayer.Models
{
    public class TransferModel
    {
        private IMagicCardList _magicCardList;
        private IMagicCardList MagicCardList
        {
            get { return _magicCardList ?? (_magicCardList = IoC.Resolve<IMagicCardList>()); }
        }

        public Dictionary<int, MagicCard> Cards = new Dictionary<int, MagicCard>();
        public Transfer Transfer { get; private set; }

        public TransferModel(Transfer transfer)
        {
            Transfer = transfer;
            var cardsToTransfer = transfer.WishList.Split(',').Select(int.Parse);

            foreach (var cardId in cardsToTransfer)
            {
                MagicCard card;
                if (!Cards.TryGetValue(cardId, out card))
                {
                    card = MagicCardList.GetCardByMtgoId(cardId);
                    card.CopiesOfCard = 0;
                    Cards.Add(cardId, card);
                }

                card.CopiesOfCard++;
            }
        }
    }
}