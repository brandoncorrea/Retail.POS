using Retail.POS.Common.Models.LineItems;
using System;

namespace Retail.POS.Common.TransactionHandler
{
    public class ItemErrorEventArgs : EventArgs
    {
        public PosItem Item { get; set; }
        public string Message { get; set; }
    }
}
