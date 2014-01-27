using CardDataLayer.Models;

namespace BusinessLogicLayer.Interfaces
{
    public interface ITransferHandler
    {
        void Setup(Transfer transfer);
        void InitiateTrade(string tradee);
    }
}