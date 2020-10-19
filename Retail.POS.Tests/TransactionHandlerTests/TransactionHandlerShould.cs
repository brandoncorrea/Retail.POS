using NUnit.Framework;
using Retail.POS.BL.Transaction;
using Retail.POS.Common.Interfaces;
using Retail.POS.Tests.MockClasses;
using System;
using System.Linq;

namespace Retail.POS.Tests.TransactionHandlerTests
{
    public class TransactionHandlerShould
    {
        private TransactionHandler Handler { get; set; }

        [SetUp]
        public void SetUp()
        {
            Handler = new TransactionHandler();
        }

        [Test]
        public void BeEmptyAtStart()
        {
            Assert.AreEqual(0, Handler.Items.Count());
            Assert.AreEqual(0, Handler.Refunds.Count());
            Assert.AreEqual(0, Handler.Tenders.Count());
            Assert.AreEqual(0, Handler.NetTotal);
            Assert.AreEqual(0, Handler.TaxTotal);
            Assert.AreEqual(0, Handler.GrossTotal);
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(10)]
        public void AddItemsIndividually(int count)
        {
            for (int i = 0; i < count; i++)
                Handler.Add(new MockItem(), 1);

            Assert.AreEqual(count, Handler.Items.Count());
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
                Handler.Add(item, 1);
            for (int i = 0; i < voidItems; i++)
                Handler.Void(item, 1);
            var expectedCount = addItems - voidItems;

            if (expectedCount < 0)
                expectedCount = 0;

            Assert.AreEqual(expectedCount, Handler.Items.Count());
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(10)]
        public void RefundItemsIndividually(int count)
        {
            for (int i = 0; i < count; i++)
                Handler.Refund(new MockItem(), 1);

            Assert.AreEqual(count, Handler.Refunds.Count());
        }

        [Test]
        public void ReturnCorrectNetTotalAfterAddingUntaxedItems()
        {
            // Price, Multiple
            var unitScenarios = new(double, int, int)[]
            {
                ( 1.25, 1, 1),
                (-1.00, 1, 1),
                (-1.00, 2, 1),
                ( 3.00, 2, 1),
                ( 0.00, 1, 1),
                ( 0.00, 2, 1),
                ( 0.69, 1, 1),
                ( 0.69, 2, 1),
                ( 0.69, 1, 1),
                ( 0.69, 2, 1),
            }
            .Select(i => new Tuple<MockItem, int>(
                new MockItem()
                {
                    ItemId = "123",
                    SellPrice = i.Item1,
                    SellMultiple = i.Item2,
                },
                i.Item3
            ));

            // Price, Multiple, Weight
            var weightScenarios = new (double, int, double)[]
            {
                ( 1.25, 1,  0.12),
                (-1.00, 1,  1.12),
                (-1.00, 2,  1.00),
                ( 3.00, 2,  0.00),
                ( 0.00, 1,  0.50),
                ( 0.00, 2, 10.00),
                ( 0.69, 1,  3.25),
                ( 0.69, 2,  0.45),
                ( 0.69, 1,  0.99),
                ( 0.69, 2,  1.01),
            }
            .Select(i => new Tuple<MockItem, double>(
                new MockItem()
                {
                    ItemId = "123",
                    SellPrice = i.Item1,
                    SellMultiple = i.Item2,
                    Weighed = true,
                },
                i.Item3
            ));

            foreach (var scenario in unitScenarios)
                Handler.Add(scenario.Item1, scenario.Item2);

            foreach (var scenario in weightScenarios)
                Handler.Add(scenario.Item1, scenario.Item2);

            double calculatePrice(MockItem item, double qty) =>
                (item.SellPrice / item.SellMultiple) * qty;

            var expectedTotal =
                unitScenarios.Sum(i => calculatePrice(i.Item1, i.Item2)) +
                weightScenarios.Sum(i => calculatePrice(i.Item1, i.Item2));

            Assert.AreEqual(expectedTotal, Handler.NetTotal, 0.0001);
            Assert.AreEqual(expectedTotal, Handler.GrossTotal, 0.0001);
            Assert.AreEqual(0, Handler.TaxTotal, 0.0001);
        }
    }
}
