using NUnit.Framework;
using Retail.POS.Common.Interfaces;
using System.Linq;

namespace Retail.POS.Tests.TransactionTests
{
    public class TransactionVoidTenderShould
    {
        private ITransaction Order { get; set; }

        [SetUp]
        public void SetUp()
        {
            Order = TestManager.CreateTransaction();
        }

        [Test]
        public void ReturnsCorrectCount()
        {
            Order.AddTender("CASH", 1.00);
            Order.AddTender("CASH", 2.00);
            Order.AddTender("CASH", 3.00);
            Order.AddTender("CASH", 4.00);
            Order.AddTender("CASH", 5.00);
            var reference = Order.Tenders.First().Reference;
            Order.VoidTender(reference);
            reference = Order.Tenders.First().Reference;
            Order.VoidTender(reference);
            Assert.AreEqual(5, Order.Tenders.Count());
        }

        [Test]
        public void CannotVoidTenderThatDoesNotExist()
        {
            Order.AddTender("CASH", 1.00);
            Order.VoidTender("fake reference");
            Assert.AreEqual(1, Order.Tenders.Count());
        }

        [Test]
        public void ReverseThePayment()
        {
            Order.AddTender("CASH", 1.00);
            var reference = Order.Tenders.First().Reference;
            Order.VoidTender(reference);
            var tender = Order.Tenders.FirstOrDefault(i => i.Reference == reference);
            Assert.IsTrue(tender.Reversed);
        }
    }
}
