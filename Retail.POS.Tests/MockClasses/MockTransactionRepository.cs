using Retail.POS.Common.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Retail.POS.Tests.MockClasses
{
    public class MockTransactionRepository : ITransactionRepository
    {
        private List<ITransaction> SavedTransactions { get; set; }
        private List<ITransaction> CompletedTransactions { get; set; }
        private List<ITransaction> VoidedTransactions { get; set; }

        public MockTransactionRepository()
        {
            SavedTransactions = new List<ITransaction>();
            CompletedTransactions = new List<ITransaction>();
            VoidedTransactions = new List<ITransaction>();
        }

        public ITransaction Get(object id)
        {
            var transaction = SavedTransactions.FirstOrDefault(i => i.Identifier == id);
            if (transaction == null)
                transaction = CompletedTransactions.FirstOrDefault(i => i.Identifier == id);
            if (transaction == null)
                transaction = VoidedTransactions.FirstOrDefault(i => i.Identifier == id);
            if (transaction == null)
                throw new KeyNotFoundException($"{id} not found in the Transaction store.");
            return transaction;
        }

        public void StoreCompleted(ITransaction transaction)
        {
            DeleteTransaction(transaction);
            CompletedTransactions.Add(transaction);
        }

        public void StoreIncompleted(ITransaction transaction)
        {
            DeleteTransaction(transaction);
            SavedTransactions.Add(transaction);
        }

        public void StoreVoided(ITransaction transaction)
        {
            DeleteTransaction(transaction);
            VoidedTransactions.Add(transaction);
        }

        private void DeleteTransaction(ITransaction transaction)
        {
            SavedTransactions.RemoveAll(i => i.Identifier == transaction.Identifier);
            VoidedTransactions.RemoveAll(i => i.Identifier == transaction.Identifier);
            CompletedTransactions.RemoveAll(i => i.Identifier == transaction.Identifier);
        }
    }
}
