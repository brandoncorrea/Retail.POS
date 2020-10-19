using System;
using System.Collections.Generic;

namespace Retail.POS.Common.Interfaces
{
    public interface ITransactionHandler
    {
        public IEnumerable<IItem> Items { get; }
        public IEnumerable<IItem> Refunds { get; }
        public IEnumerable<ITender> Tenders { get; }

        public double NetTotal { get; }
        public double TaxTotal { get; }
        public double GrossTotal { get; }

        public void Add(IItem item, double quantity);
        public void Void(IItem item, double quantity);
        public void Refund(IItem item, double quantity);
        public void VoidOrder();
        public void SaveOrder();
        public void RecallOrder();
        public void AddTender(ITender tender);
        public void VoidTender(ITender tender);
    }
}
