using Retail.POS.Common.Models;
using Retail.POS.Common.Models.LineItems;
using System;

namespace Retail.POS.Common.TransactionHandler
{
    public class ItemEventArgs : EventArgs
    {
        public IItem Item { get; private set; }
        public int Quantity { get; private set; }
        public double Weight { get; private set; }

        public ItemEventArgs(LineItem lineItem)
        {
            Item = lineItem.Item;
            Quantity = lineItem.Quantity;
            Weight = lineItem.Weight;
        }
    }
}
