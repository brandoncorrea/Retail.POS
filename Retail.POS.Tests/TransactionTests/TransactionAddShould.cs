using NUnit.Framework;
using Retail.POS.Common.Interfaces;
using Retail.POS.Tests.MockClasses;
using System.Linq;

namespace Retail.POS.Tests.TransactionTests
{
    public class TransactionAddShould
    {
        private ITransaction Order { get; set; }

        [SetUp]
        public void SetUp()
        {
            Order = TestManager.CreateTransaction();
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(10)]
        public void AddItemsIndividually(int count)
        {
            for (int i = 0; i < count; i++)
                Order.Add(new MockItem(), 1);

            Assert.AreEqual(count, Order.Items.Count());
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(10)]
        public void AddItemsQuantity(int count)
        {
            Order.Add(new MockItem(), count);
            Assert.AreEqual(count, Order.Items.Count());
        }
    }
}
