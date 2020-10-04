using Microsoft.Extensions.Configuration;
using Retail.POS.Common.Models;
using Retail.POS.Common.Repositories;
using Retail.POS.Common.TransactionHandler.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Retail.POS.Common.TransactionHandler
{
    public class TransactionHandler : ITransactionHandler
    {
        // Class Properties
        public int ItemCount { get => Items.Select(i => i.Quantity).Sum(); }
        private List<LineItem> Items { get; set; }
        private List<LineItem> Refunds { get; set; }
        public double GrossTotal => NetTotal + TaxTotal;
        public double NetTotal
        {
            get
            {
                return Items
                    .Select(i =>
                    {
                        var unitPrice = Math.Round(i.Item.SellPrice / i.Item.SellMultiple, 2);
                        if (i.Item.Weighed)
                            return Math.Round(unitPrice * i.Weight, 2) * i.Quantity;
                        return unitPrice * i.Quantity;
                    })
                    .Sum();
            }
        }
        public double TaxTotal
        {
            get
            {
                return Items
                    .Select(i => CalculateTax(i))
                    .Sum();
            }
        }

        // Dependencies
        private readonly IConfiguration _config;
        private readonly IItemRepository _itemRepository;

        // Events
        public event EventHandler<ItemEventArgs> ItemAdded = delegate { };
        public event EventHandler<ItemEventArgs> ItemVoided = delegate { };
        public event EventHandler<ItemEventArgs> ItemRefunded = delegate { };
        public event EventHandler<ItemEventArgs> RefundVoided = delegate { };
        public event EventHandler<ItemErrorEventArgs> AddError = delegate { };
        public event EventHandler<ItemErrorEventArgs> VoidError = delegate { };
        public event EventHandler<ItemErrorEventArgs> RefundError = delegate { };
        public event EventHandler<ItemErrorEventArgs> VoidRefundError = delegate { };

        public TransactionHandler(
            IConfiguration config,
            IItemRepository itemRepository)
        {
            _config = config;
            _itemRepository = itemRepository;
            Items = new List<LineItem>();
            Refunds = new List<LineItem>();
        }

        public void AddItem(object id)
        {
            var lineItem = new LineItem()
            {
                Item = _itemRepository.Get(id),
                Quantity = 1,
                Weight = 0
            };
            AddLineItem(id, lineItem);
        }
        public void AddItem(object id, double weight)
        {
            var lineItem = new LineItem()
            {
                Item = _itemRepository.Get(id),
                Quantity = 1,
                Weight = weight,
            };
            AddLineItem(id, lineItem);
        }
        public void AddItem(object id, int quantity)
        {
            var lineItem = new LineItem()
            {
                Item = _itemRepository.Get(id),
                Quantity = quantity,
                Weight = 0,
            };
            AddLineItem(id, lineItem);
        }

        public void VoidItem(object id)
        {
            var itemId = id.ToString();
            var lastIndex = Items.FindLastIndex(i => i.Item.ItemId == itemId);
            VoidItemAtIndex(itemId, lastIndex);
        }
        public void VoidItem(object id, int quantity)
        {
            var itemId = id.ToString();
            var totalInOrder = Items
                .Where(i => i.Item.ItemId == itemId)
                .Select(i => i.Quantity)
                .Sum();

            if (totalInOrder < quantity)
            {
                var args = new ItemErrorEventArgs()
                {
                    ItemId = itemId,
                    Message = "Quantity exceeded items in transaction.",
                };
                VoidError.Invoke(this, args);
            }
            else
                VoidQuantityItems(itemId, quantity);
        }
        public void VoidItem(object id, double weight)
        {
            var itemId = id.ToString();
            var lastIndex = Items
                .FindLastIndex(i => i.Item.ItemId == itemId && i.Weight == weight);
            VoidItemAtIndex(itemId, lastIndex);
        }

        public void RefundItem(object id)
        {
            var item = _itemRepository.Get(id);
            if (item == null)
            {
                var args = new ItemNotFoundEventArgs(id.ToString());
                RefundError.Invoke(this, args);
            }
            else
            {
                var lineItem = new LineItem()
                {
                    Item = item,
                    Quantity = 1,
                    Weight = 0,
                };
                Refunds.Add(lineItem);
                var args = new ItemEventArgs(lineItem);
                ItemRefunded.Invoke(this, args);
            }
        }
        public void RefundItem(object id, int quantity)
        {
            throw new NotImplementedException();
        }
        public void RefundItem(object id, double weight)
        {
            throw new NotImplementedException();
        }

        public void VoidRefund(object id)
        {
            throw new NotImplementedException();
        }
        public void VoidRefund(object id, int quantity)
        {
            throw new NotImplementedException();
        }
        public void VoidRefund(object id, double weight)
        {
            throw new NotImplementedException();
        }

        #region Helpers
        // Adds a LineItem to the Items list
        private void AddLineItem(object id, LineItem lineItem)
        {
            if (lineItem.Item == null)
            {
                var args = new ItemNotFoundEventArgs(id.ToString());
                AddError.Invoke(this, args);
            }
            else
            {
                Items.Add(lineItem);
                var args = new ItemEventArgs(lineItem);
                ItemAdded.Invoke(this, args);
            }
        }
        
        // Voids an item from the Items list
        private void VoidItemAtIndex(string itemId, int index)
        {
            if (index < 0)
            {
                var args = new ItemErrorEventArgs()
                {
                    ItemId = itemId,
                    Message = "Item not in current order.",
                };
                VoidError.Invoke(this, args);
            }
            else
            {
                var voidItem = Items[index];
                Items.RemoveAt(index);
                var args = new ItemEventArgs(voidItem);
                ItemVoided.Invoke(this, args);
            }
        }

        // Removes items from the transaction until the quantity has been reached
        private void VoidQuantityItems(string itemId, int quantity)
        {
            var item = Items
                .Select(i => i.Item)
                .FirstOrDefault(i => i.ItemId == itemId);

            var lineItem = new LineItem()
            {
                Item = item,
                Quantity = quantity,
                Weight = 0
            };

            while (quantity > 0)
            {
                var lastIndex = Items.FindLastIndex(i => i.Item.ItemId == itemId);
                var qtyAtIndex = Items[lastIndex].Quantity;
                if (quantity >= qtyAtIndex)
                    Items.RemoveAt(lastIndex);
                else
                    Items[lastIndex].Quantity -= quantity;
                quantity -= qtyAtIndex;
            }

            var args = new ItemEventArgs(lineItem);
            ItemVoided.Invoke(this, args);
        }
        
        // Returns the tax total for a given item
        private double CalculateTax(LineItem lineItem)
        {
            // Get tax rates
            double taxRate1 = double.Parse(_config.GetSection("Taxes:Rate1").Value);
            double taxRate2 = double.Parse(_config.GetSection("Taxes:Rate2").Value);
            double taxRate3 = double.Parse(_config.GetSection("Taxes:Rate3").Value);
            double taxRate4 = double.Parse(_config.GetSection("Taxes:Rate4").Value);

            var item = lineItem.Item;
            var unitPrice = Math.Round(item.SellPrice / item.SellMultiple, 2);
            
            // Get tax totals
            var tax1Total = (item.Tax1 ? unitPrice * taxRate1 : 0);
            var tax2Total = (item.Tax2 ? unitPrice * taxRate2 : 0);
            var tax3Total = (item.Tax3 ? unitPrice * taxRate3 : 0);
            var tax4Total = (item.Tax4 ? unitPrice * taxRate4 : 0);

            // Calculate taxes
            tax1Total = Math.Round(tax1Total * lineItem.Quantity, 2);
            tax2Total = Math.Round(tax2Total * lineItem.Quantity, 2);
            tax3Total = Math.Round(tax3Total * lineItem.Quantity, 2);
            tax4Total = Math.Round(tax4Total * lineItem.Quantity, 2);

            if (item.Weighed)
            {
                tax1Total *= lineItem.Weight;
                tax2Total *= lineItem.Weight;
                tax3Total *= lineItem.Weight;
                tax4Total *= lineItem.Weight;
            }

            // Return tax sum
            return (tax1Total + tax2Total + tax3Total + tax4Total) / 100;
        }
        #endregion
    }
}
