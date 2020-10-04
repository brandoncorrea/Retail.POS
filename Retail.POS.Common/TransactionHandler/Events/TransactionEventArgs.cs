using Retail.POS.Common.Models.LineItems;
using System;

namespace Retail.POS.Common.TransactionHandler
{
    public class TransactionEventArgs : EventArgs
    {
        public IItem Item { get; set; }
        
        public TransactionEventArgs(IItem item)
        {
            Item = item;
        }
    }
}
