using Microsoft.Extensions.Configuration;
using Retail.POS.Common.Interfaces;
using Retail.POS.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Retail.POS.BL
{
    public class Transaction : ITransaction
    {
        private double TaxRate1 { get; set; }
        private double TaxRate2 { get; set; }
        private double TaxRate3 { get; set; }
        private double TaxRate4 { get; set; }

        private readonly IPaymentProcessor _paymentProcessor;
        private readonly List<IItem> _items;
        private readonly List<IItem> _refunds;
        private readonly List<Tender> _tenders;

        public IEnumerable<IItem> Items => _items;
        public IEnumerable<IItem> Refunds => _refunds;
        public IEnumerable<Tender> Tenders => _tenders;

        public double NetTotal => Items.Sum(i => CalculateItemNet(i));
        public double TaxTotal => _items.Sum(i => CalculateItemTaxes(i));
        public double GrossTotal => NetTotal + TaxTotal;
        public double Balance => GrossTotal - Tenders.Sum(i => i.Succeeded ? i.Amount : 0);
        public object Identifier { get; private set; }

        public Transaction(
            IConfiguration config, 
            IPaymentProcessor paymentProcessor)
        {
            Identifier = Guid.NewGuid().ToString();
            _paymentProcessor = paymentProcessor;
            _items = new List<IItem>();
            _refunds = new List<IItem>();
            _tenders = new List<Tender>();
            TaxRate1 = double.Parse(config.GetSection("Taxes:Rate1").Value);
            TaxRate2 = double.Parse(config.GetSection("Taxes:Rate2").Value);
            TaxRate3 = double.Parse(config.GetSection("Taxes:Rate3").Value);
            TaxRate4 = double.Parse(config.GetSection("Taxes:Rate4").Value);
        }

        public void Add(IItem item, double quantity)
        {
            if (item.Weighed)
                AddWeighedItem(item, quantity);
            else
                AddItemQuantity(item, quantity);
        }

        public void Void(IItem item, double quantity)
        {
            int itemIndex = _items.IndexOf(item);
            if (itemIndex >= 0)
                _items.RemoveAt(itemIndex);
        }

        public void AddTender(string method, double amount)
        {
            var tender = _paymentProcessor.ProcessPayment(method, amount);
            _tenders.Add(tender);
        }

        public void Refund(IItem item, double quantity)
        {
            if (item.Weighed)
                RefundWeighedItem(item, quantity);
            else
                RefundItemQuantity(item, quantity);
        }

        public void VoidTender(string reference)
        {
            int tenderIndex = _tenders.FindIndex(i => i.Reference == reference);
            if (tenderIndex < 0) return;
            var tender = _tenders[tenderIndex];
            var reversal = _paymentProcessor.ReversePayment(reference);
            _tenders.RemoveAt(tenderIndex);
            _tenders.Add(reversal);
        }

        private double CalculateItemTaxes(IItem item)
        {
            double rateTotal = 0;
            if (item.Tax1)
                rateTotal += TaxRate1;
            if (item.Tax2)
                rateTotal += TaxRate2;
            if (item.Tax3)
                rateTotal += TaxRate3;
            if (item.Tax4)
                rateTotal += TaxRate4;

            return CalculateItemNet(item) * rateTotal;
        }

        private double CalculateItemNet(IItem item)
            => item.SellPrice / item.SellMultiple;

        private void AddWeighedItem(IItem item, double quantity)
        {
            item.SellPrice *= quantity;
            _items.Add(item);
        }

        private void AddItemQuantity(IItem item, double quantity)
        {
            for (int i = 0; i < quantity; i++)
                _items.Add(item);
        }

        private void RefundWeighedItem(IItem item, double quantity)
        {
            item.SellPrice *= quantity;
            _refunds.Add(item);
        }

        private void RefundItemQuantity(IItem item, double quantity)
        {
            for (int i = 0; i < quantity; i++)
                _refunds.Add(item);
        }
    }
}
