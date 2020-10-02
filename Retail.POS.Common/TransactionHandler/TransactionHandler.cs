using Microsoft.Extensions.Configuration;
using Retail.POS.Common.Models.LineItems;
using Retail.POS.Common.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Retail.POS.Common.TransactionHandler
{
    public class TransactionHandler : ITransactionHandler
    {
        // Transaction Data
        private List<PosItem> AddedItems { get; set; }
        private List<PosItem> VoidedItems { get; set; }
        private List<PosItem> RefundedItems { get; set; }
        private List<PosPromotion> AddedPromotions { get; set; }
        private List<PosPromotion> VoidedPromotions { get; set; }
        private List<PosPromotion> RefundedPromotions { get; set; }
        
        // Dependencies
        private readonly IConfiguration _config;
        private readonly IItemRepository<PosItem> _itemRepository;

        // Events
        public event EventHandler ItemAdded;
        public event EventHandler ItemVoided;
        public event EventHandler ItemRefunded;
        public event EventHandler PromotionAdded;
        public event EventHandler PromotionRemoved;
        public event EventHandler TenderReversed;
        public event EventHandler TenderAdded;
        public event EventHandler TransactionCompleted;
        public event EventHandler TransactionVoided;
        public event EventHandler ItemAddedError;
        public event EventHandler ItemVoidedError;
        public event EventHandler ItemRefundedError;
        public event EventHandler TenderError;

        public TransactionHandler(
            IConfiguration config,
            IItemRepository<PosItem> itemRepository)
        {
            _config = config;
            _itemRepository = itemRepository;
            AddedItems = new List<PosItem>();
            VoidedItems = new List<PosItem>();
            RefundedItems = new List<PosItem>();
            AddedPromotions = new List<PosPromotion>();
            VoidedPromotions = new List<PosPromotion>();
        }

        public IEnumerable<PosItem> GetAddedItems() => AddedItems;
        public IEnumerable<PosItem> GetVoidedItems() => VoidedItems;
        public IEnumerable<PosItem> GetRefundedItems() => RefundedItems;
        public IEnumerable<PosPromotion> GetAddedPromotions() => AddedPromotions;
        public IEnumerable<PosPromotion> GetVoidedPromotions() => VoidedPromotions;
        public IEnumerable<PosPromotion> GetRefundedPromotions() => RefundedPromotions;
        public PosItem GetLastItemAdded() => AddedItems.LastOrDefault();
        public PosItem GetLastItemRefunded() => RefundedItems.LastOrDefault();
        public PosItem GetLastItemVoided() => VoidedItems.LastOrDefault();
        public PosPromotion GetLastPromotionAdded() => AddedPromotions.LastOrDefault();
        public PosPromotion GetLastPromotionRefunded() => RefundedPromotions.LastOrDefault();
        public PosPromotion GetLastPromotionVoided() => VoidedPromotions.LastOrDefault();

        public void AddItem(object id)
        {
            var item = _itemRepository.Get(id);
            AddedItems.Add(item);
            var args = new TransactionEventArgs() 
            { 
                Item = item 
            };
            ItemAdded.Invoke(this, args);
        }

        public void VoidItem(object id)
        {
            var item = _itemRepository.Get(id);
            var lastIndex = AddedItems.FindLastIndex(i => i.GTIN == item.GTIN);
            var voidItem = AddedItems[lastIndex];
            VoidedItems.Add(voidItem);
            AddedItems.RemoveAt(lastIndex);
            var args = new TransactionEventArgs()
            {
                Item = voidItem
            };
            ItemVoided.Invoke(this, args);
        }

        public void RefundItem(object id)
        {
            var item = _itemRepository.Get(id);
            RefundedItems.Add(item);
            var args = new TransactionEventArgs()
            {
                Item = item
            };
            ItemRefunded.Invoke(this, args);
        }

        public decimal GetGrossTotal() => 
            GetNetTotal() + GetTaxTotal();

        public decimal GetNetTotal()
        {
            var addedTotal = AddedItems
                .Select(i => (i.Price * i.Quantity) * (i.Weighed ? i.Weight : 1))
                .Sum();

            var refundedTotal = RefundedItems
                .Select(i => (i.Price * i.Quantity) * (i.Weighed ? i.Weight : 1))
                .Sum();

            return addedTotal - refundedTotal;
        }

        public decimal GetTaxTotal()
        {
            decimal taxRate1 = decimal.Parse(_config.GetSection("Taxes:Rate1").Value);
            decimal taxRate2 = decimal.Parse(_config.GetSection("Taxes:Rate2").Value);
            decimal taxRate3 = decimal.Parse(_config.GetSection("Taxes:Rate3").Value);
            decimal taxRate4 = decimal.Parse(_config.GetSection("Taxes:Rate4").Value);

            decimal calculateTax(PosItem item) =>
                (item.Tax1 ? item.Price * taxRate1 : 0) +
                (item.Tax2 ? item.Price * taxRate2 : 0) +
                (item.Tax3 ? item.Price * taxRate3 : 0) +
                (item.Tax4 ? item.Price * taxRate4 : 0);

            var addedTotal = AddedItems
                .Select(i => calculateTax(i))
                .Sum();

            var refundedTotal = RefundedItems
                .Select(i => calculateTax(i))
                .Sum();

            return addedTotal - refundedTotal;
        }

        public IEnumerable<ILineItem> GetLineItems()
        {
            throw new NotImplementedException();
        }

        public decimal GetPromotionAmount()
        {
            throw new NotImplementedException();
        }

        public decimal GetCouponAmount()
        {
            throw new NotImplementedException();
        }

        public void AddItem(object id, int quantity)
        {
            throw new NotImplementedException();
        }

        public void AddItem(object id, decimal weight)
        {
            throw new NotImplementedException();
        }

        public void VoidItem(object id, int quantity)
        {
            throw new NotImplementedException();
        }

        public void VoidItem(object id, decimal weight)
        {
            throw new NotImplementedException();
        }

        public void RefundItem(object id, int quantity)
        {
            throw new NotImplementedException();
        }

        public void RefundItem(object id, decimal weight)
        {
            throw new NotImplementedException();
        }

        public void AddCoupon(object id)
        {
            throw new NotImplementedException();
        }

        public void VoidCoupon(object id)
        {
            throw new NotImplementedException();
        }

        public void RefundCoupon(object id)
        {
            throw new NotImplementedException();
        }
    }
}
