using Retail.POS.Common.Models;
using System.Collections.Generic;

namespace Retail.POS.Common.Interfaces
{
    public interface ITransaction
    {
        public IEnumerable<IItem> Items { get; }
        public IEnumerable<IItem> Refunds { get; }
        public IEnumerable<Tender> Tenders { get; }

        public double NetTotal { get; }
        public double TaxTotal { get; }
        public double GrossTotal { get; }
        public double Balance { get; }
        public object Identifier { get; }

        public void Add(IItem item, double quantity);
        public void Void(IItem item, double quantity);
        public void Refund(IItem item, double quantity);
        public void AddTender(string method, double amount);
        public void VoidTender(string reference);
    }
}
