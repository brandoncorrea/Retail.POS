using NUnit.Framework;
using Retail.POS.Common.Interfaces;
using System.Linq;

namespace Retail.POS.Tests.TransactionTests
{
    public class TransactionShould
    {
        private ITransaction Order { get; set; }

        [SetUp]
        public void SetUp()
        {
            Order = TestManager.CreateTransaction();
        }

        [Test]
        public void BeEmptyAtStart()
        {
            Assert.AreEqual(0, Order.Items.Count());
            Assert.AreEqual(0, Order.Refunds.Count());
            Assert.AreEqual(0, Order.Tenders.Count());
            Assert.AreEqual(0, Order.NetTotal);
            Assert.AreEqual(0, Order.TaxTotal);
            Assert.AreEqual(0, Order.GrossTotal);
        }
    }
}
