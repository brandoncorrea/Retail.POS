using NUnit.Framework;
using Retail.POS.Common.Interfaces;
using System.Linq;
using System.Net.WebSockets;

namespace Retail.POS.Tests.TransactionHandlerTests
{
    public class TransactionHandlerShould
    {
        private ITransactionHandler Handler { get; set; }

        [SetUp]
        public void SetUp()
        {
            Handler = TestManager.CreateTransactionHandler();
        }

        [Test]
        public void CreateOrders()
        {
            var transaction = Handler.Create();
            Assert.IsNotNull(transaction);
            Assert.IsNotNull(transaction.Identifier);
            Assert.AreEqual(0, transaction.Items.Count());
            Assert.AreEqual(0, transaction.Tenders.Count());
            Assert.AreEqual(0, transaction.Refunds.Count());
        }

        [Test]
        public void SaveOrders()
        {
            var transaction = Handler.Create();
            Handler.Save(transaction);
            var recalled = TestManager
                .MockTransactionRepository
                .Get(transaction.Identifier);
            Assert.AreEqual(transaction, recalled);
        }

        [Test]
        public void RecallOrders()
        {
            var transaction = Handler.Create();
            Handler.Save(transaction);
            var recalled = Handler.Recall(transaction.Identifier);
            Assert.AreEqual(transaction, recalled);
        }

        [Test]
        public void CompleteOrders()
        {
            var transaction = Handler.Create();
            Handler.Complete(transaction);
            var completed = TestManager
                .MockTransactionRepository
                .Get(transaction.Identifier);
            Assert.AreEqual(transaction, completed);
        }

    }
}
