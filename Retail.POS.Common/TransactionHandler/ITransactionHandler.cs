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
        
        public void AddItem(object id);
        public void AddItem(object id, int quantity);
        public void AddItem(object id, double weight);
        
        public void VoidItem(object id);
        public void VoidItem(object id, int quantity);
        public void VoidItem(object id, double weight);
        
        public void RefundItem(object id);
        public void RefundItem(object id, int quantity);
        public void RefundItem(object id, double weight);

        public void VoidRefund(object id);
        public void VoidRefund(object id, int quantity);
        public void VoidRefund(object id, double weight);
    }
}
