using Retail.POS.Common.Models.LineItems;
using System;
using System.Collections.Generic;

namespace Retail.POS.Common.TransactionHandler
{
    public interface ITransactionHandler
    {
        event EventHandler ItemAdded;
        event EventHandler ItemVoided;
        event EventHandler ItemRefunded;
        event EventHandler PromotionAdded;
        event EventHandler PromotionRemoved;
        event EventHandler TenderReversed;
        event EventHandler TenderAdded;
        event EventHandler TransactionCompleted;
        event EventHandler TransactionVoided;
        event EventHandler ItemAddedError;
        event EventHandler ItemVoidedError;
        event EventHandler ItemRefundedError;
        event EventHandler TenderError;

        public IEnumerable<ILineItem> GetLineItems();
        public decimal GetNetTotal();
        public decimal GetTaxTotal();
        public decimal GetPromotionAmount();
        public decimal GetCouponAmount();
        public void AddItem(object id);
        public void AddItem(object id, int quantity);
        public void AddItem(object id, decimal weight);
        public void VoidItem(object id);
        public void VoidItem(object id, int quantity);
        public void VoidItem(object id, decimal weight);
        public void RefundItem(object id);
        public void RefundItem(object id, int quantity);
        public void RefundItem(object id, decimal weight);
        public void AddCoupon(object id);
        public void VoidCoupon(object id);
        public void RefundCoupon(object id);
    }
}
