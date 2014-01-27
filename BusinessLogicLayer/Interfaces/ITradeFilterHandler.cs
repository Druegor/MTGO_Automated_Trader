using CardDataLayer.Models;

namespace BusinessLogicLayer.Interfaces
{
    public interface ITradeFilterHandler
    {
        void PickCardSet(CardSet cardSet);
        void PickRarity(RaritySet raritySet);
        void PickVersion(VersionSet versionSet);
        void PickLessThanFilter();
        void SetOwnedFilterNumberValue(int value);
        void Reset();
    }
}