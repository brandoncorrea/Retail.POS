using NUnit.Framework;
using Retail.POS.Common.Interfaces;
using Retail.POS.Tests.MockClasses;
using System.Linq;

namespace Retail.POS.Tests.TransactionTests
{
    public class TransactionVoidShould
    {

        private ITransaction Order { get; set; }

        [SetUp]
        public void SetUp()
        {
            Order = TestManager.CreateTransaction();
        }

        [Test]
        [TestCase(0, 0)]
        [TestCase(1, 0)]
        [TestCase(0, 1)]
        [TestCase(1, 1)]
        [TestCase(2, 1)]
        [TestCase(2, 2)]
        [TestCase(10, 3)]
        [TestCase(10, 10)]
        public void VoidItemsIndividually(int addItems, int voidItems)
        {
            var item = new MockItem()
            {
                ItemId = "123"
            };
            for (int i = 0; i < addItems; i++)
                Order.Add(item, 1);
            for (int i = 0; i < voidItems; i++)
                Order.Void(item, 1);
            var expectedCount = addItems - voidItems;

            if (expectedCount < 0)
                expectedCount = 0;

            Assert.AreEqual(expectedCount, Order.Items.Count());
        }
    }
}
