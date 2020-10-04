using System;
using System.Collections.Generic;
using System.Text;

namespace Retail.POS.Common.TransactionHandler.Events
{
    public class ItemNotFoundEventArgs : ItemErrorEventArgs
    {
        public ItemNotFoundEventArgs(string id)
        {
            ItemId = id;
            Message = "Item not found.";
        }
    }
}
