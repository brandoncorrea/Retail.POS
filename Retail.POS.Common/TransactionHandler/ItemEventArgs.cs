using Retail.POS.Common.Models.LineItems;
using System;

namespace Retail.POS.Common.TransactionHandler
{
    public class ItemEventArgs : EventArgs
    {
        public PosItem Item { get; set; }
    }
}
