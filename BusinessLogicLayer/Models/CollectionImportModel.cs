using CardDataLayer.Models;

namespace BusinessLogicLayer.Models
{
    public class CollectionImportModel
    {
        //Card Name	Online	For Trade	Rarity	Set	No.	Premium
        public string CardName { get; set; }
        public int Online { get; set; }
        public int ForTrade { get; set; }
        public CardRarity Rarity { get; set; }
        public string Set { get; set; }
        public string SetNumber { get; set; }
        public bool Premium { get; set; }

        public override string ToString()
        {
	        return string.Format("{0} {1} {2} {3} {4} {5} {6}",
	                             this.CardName, this.Online, this.ForTrade, this.Rarity,
	                             this.Set, this.SetNumber, this.Premium);
        }
    }
}
