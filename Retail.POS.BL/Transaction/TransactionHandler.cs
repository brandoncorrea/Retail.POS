using Retail.POS.Common.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Retail.POS.BL.Transaction
{
    public class TransactionHandler : ITransactionHandler
    {
        private readonly List<IItem> _items;
        private readonly List<IItem> _refunds;
        private readonly List<ITender> _tenders;

        public IEnumerable<IItem> Items => _items;
        public IEnumerable<IItem> Refunds => _refunds;
        public IEnumerable<ITender> Tenders => _tenders;

        public double NetTotal => Items.Sum(i => i.SellPrice / i.SellMultiple);
        public double TaxTotal => 0;
        public double GrossTotal => NetTotal + TaxTotal;

        public TransactionHandler()
        {
            _items = new List<IItem>();
            _refunds = new List<IItem>();
            _tenders = new List<ITender>();
        }

        public void Add(IItem item, double quantity)
        {
            if (item.Weighed)
                item.SellPrice *= quantity;
            _items.Add(item);
        }

        public void AddTender(ITender tender)
        {
            throw new System.NotImplementedException();
        }

        public void RecallOrder()
        {
            throw new System.NotImplementedException();
        }

        public void Refund(IItem item, double quantity)
        {
            _refunds.Add(item);
        }

        public void SaveOrder()
        {
            throw new System.NotImplementedException();
        }

        public void Void(IItem item, double quantity)
        {
            int firstIndex = _items.IndexOf(item);
            if (firstIndex >= 0)
                _items.RemoveAt(firstIndex);
        }

        public void VoidOrder()
        {
            throw new System.NotImplementedException();
        }

        public void VoidTender(ITender tender)
        {
            throw new System.NotImplementedException();
        }
    }
}
