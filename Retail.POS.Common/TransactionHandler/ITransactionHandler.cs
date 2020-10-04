using Retail.POS.Common.Models;
using System;

namespace Retail.POS.Common.TransactionHandler
{
    public interface ITransactionHandler
    {
        public event EventHandler<ItemEventArgs> ItemAdded;
        public event EventHandler<ItemEventArgs> ItemVoided;
        public event EventHandler<ItemEventArgs> ItemRefunded;
        public event EventHandler<ItemEventArgs> RefundVoided;
        public event EventHandler<ItemErrorEventArgs> AddError;
        public event EventHandler<ItemErrorEventArgs> VoidError;
        public event EventHandler<ItemErrorEventArgs> RefundError;
        public event EventHandler<ItemErrorEventArgs> VoidRefundError;

        public int ItemCount { get; }
        public double NetTotal { get; }
        public double TaxTotal { get; }
        public double GrossTotal { get; }

        public void AddItem(AddItemArgs args);
        public void VoidItem(AddItemArgs args);
        public void RefundItem(AddItemArgs args);
        public void VoidRefund(AddItemArgs args);
    }
}
