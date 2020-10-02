namespace Retail.POS.Common.Models.LineItems
{
    public class PosPromotion : ILineItem
    {
        public string Description { get; set; }
        public decimal PromotionAmount { get; set; }
        public string[] LinkedGtins { get; set; }
        public bool Tax1 { get; set; }
        public bool Tax2 { get; set; }
        public bool Tax3 { get; set; }
        public bool Tax4 { get; set; }
    }
}
