namespace Retail.POS.Common.Models
{
    public class AddItemArgs
    {
        public string ItemId { get; set; }
        public int Quantity { get; set; }
        public double Weight { get; set; }
        public double SellPrice { get; set; }
        public int SellMultiple { get; set; }
        public bool PriceOverride { get; set; }
    }
}
