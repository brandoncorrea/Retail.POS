using Retail.POS.Common.Interfaces;

namespace Retail.POS.Common.Models
{
    public class Item : IItem
    {
        public string ItemId { get; set; }
        public string Description { get; set; }
        public double SellPrice { get; set; }
        public int SellMultiple { get; set; }
        public bool Weighed { get; set; }
        public bool Tax1 { get; set; }
        public bool Tax2 { get; set; }
        public bool Tax3 { get; set; }
        public bool Tax4 { get; set; }
        public bool FoodStamp { get; set; }
    }
}
