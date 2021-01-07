using Retail.POS.Common.Models;
using System.Collections.Generic;

namespace Retail.POS.Common.Interfaces
{
    public interface ITransaction
    {
        /// <summary>
        /// All the items that have been added to the order
        /// </summary>
        public IEnumerable<IItem> Items { get; }

        /// <summary>
        /// All the refunds that have been added to the order
        /// </summary>
        public IEnumerable<IItem> Refunds { get; }

        /// <summary>
        /// All the tenders that have been applied to the order
        /// </summary>
        public IEnumerable<Tender> Tenders { get; }

        /// <summary>
        /// The Net Total of the transaction. 
        /// This is the sum of all the items without taxes applied
        /// </summary>
        public double NetTotal { get; }

        /// <summary>
        /// The sum of all the taxes applied in the transaction
        /// </summary>
        public double TaxTotal { get; }

        /// <summary>
        /// The Sum of the NetTotal and TaxTotal
        /// </summary>
        public double GrossTotal { get; }

        /// <summary>
        /// The remaining balance due of the transaction
        /// </summary>
        public double Balance { get; }

        /// <summary>
        /// A unique id for the transaction
        /// </summary>
        public object Identifier { get; }

        /// <summary>
        /// Adds an item to the order
        /// </summary>
        public void Add(IItem item, double quantity);
        
        /// <summary>
        /// Voids an item from the order
        /// </summary>
        public void Void(IItem item, double quantity);

        /// <summary>
        /// Refunds an item to the order
        /// </summary>
        public void Refund(IItem item, double quantity);

        /// <summary>
        /// Applies a tender for the specified amount
        /// </summary>
        public void AddTender(string method, double amount);
        
        /// <summary>
        /// Voids a tender by the reference number
        /// </summary>
        public void VoidTender(string reference);
    }
}
