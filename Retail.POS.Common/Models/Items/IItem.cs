namespace Retail.POS.Common.Models.LineItems
{
    public interface IItem
    {
        public string ItemId { get; }
        public string Description { get; }
        public double SellPrice { get; }
        public int SellMultiple { get; }
        public bool Weighed { get; }
        public bool Tax1 { get; }
        public bool Tax2 { get; }
        public bool Tax3 { get; }
        public bool Tax4 { get; }
        public bool FoodStamp { get; }
    }
}
