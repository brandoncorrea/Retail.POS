namespace Retail.POS.Common.Models.LineItems
{
    public class PosItem : ILineItem
    {
        public string GTIN { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal Weight { get; set; }
        public int Quantity { get; set; }
        public bool Weighed { get; set; }
        public bool Tax1 { get; set; }
        public bool Tax2 { get; set; }
        public bool Tax3 { get; set; }
        public bool Tax4 { get; set; }
        public bool FoodStamp { get; set; }
    }
}
