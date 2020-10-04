using Retail.POS.Common.Models.LineItems;

namespace Retail.POS.Common.Models
{
    public class LineItem
    {
        public int Quantity { get; set; }
        public double Weight { get; set; }
        public IItem Item { get; set; }
    }
}
