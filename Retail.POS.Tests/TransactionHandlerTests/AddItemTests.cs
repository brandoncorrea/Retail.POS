using NUnit.Framework;
using Retail.POS.Common.Models.LineItems;
using Retail.POS.Common.TransactionHandler;
using Retail.POS.Tests.Helpers;
using Retail.POS.Tests.MockClasses;
using System;
using System.Linq;

namespace Retail.POS.Tests.TransactionHandlerTests
{
    public class AddItemTests
    {
        public TransactionHandler Handler;
        public MockItemRepository ItemRepository;

        [SetUp]
        public void SetUp()
        {
            ItemRepository = new MockItemRepository();
            Handler = new TransactionHandler(TestManager.Config, ItemRepository);
        }

        [Test]
        public void AddOneWeighedItem()
        {
            var random = new Random();
            var items = ItemRepository.Items.Where(i => i.Weighed);
            foreach (var item in items)
            {
                // Setup
                var handler = new TransactionHandler(TestManager.Config, ItemRepository);
                var weight = Math.Round(random.NextDouble() * 10, 2);
                double expectedNet = (item.SellPrice / item.SellMultiple) * weight;
                double expectedTax = TransactionHandlerHelper.GetTaxAmount(item, weight);

                // Act
                handler.AddItem(item.ItemId, weight);
                
                // Assert
                Assert.AreEqual(expectedTax, handler.TaxTotal);
                Assert.AreEqual(Math.Round(expectedNet, 2), Math.Round(handler.NetTotal, 2), 0.0001);
                Assert.AreEqual(1, handler.ItemCount);
            }
        }

        [Test]
        public void AddOneUnweighedItem()
        {
            var random = new Random();
            var items = ItemRepository.Items.Where(i => !i.Weighed);
            foreach (var item in items)
            {
                // Setup
                var handler = new TransactionHandler(TestManager.Config, ItemRepository);
                double expectedNet = item.SellPrice / item.SellMultiple;
                double expectedTax = TransactionHandlerHelper.GetTaxAmount(item);

                // Act
                handler.AddItem(item.ItemId);

                // Assert
                Assert.AreEqual(expectedTax, handler.TaxTotal);
                Assert.AreEqual(Math.Round(expectedNet, 2), Math.Round(handler.NetTotal, 2), 0.0001);
                Assert.AreEqual(1, handler.ItemCount);
            }
        }

        [Test]
        public void AddManyUnweighedItems()
        {
            var random = new Random();
            var items = ItemRepository.Items.Where(i => !i.Weighed);
            foreach (var item in items)
            {
                // Setup
                var handler = new TransactionHandler(TestManager.Config, ItemRepository);
                int quantity = random.Next(1, 100);
                double expectedNet = Math.Round(item.SellPrice / item.SellMultiple, 2) * quantity;
                double expectedTax = TransactionHandlerHelper.GetTaxAmount(item, quantity);

                // Act
                handler.AddItem(item.ItemId, quantity);

                // Assert
                Assert.AreEqual(expectedTax, handler.TaxTotal, 0001);
                Assert.AreEqual(Math.Round(expectedNet, 2), Math.Round(handler.NetTotal, 2), 0.0001);
                Assert.AreEqual(quantity, handler.ItemCount);
            }
        }

        [Test]
        public void AddTenItems()
        {
            // SetUp
            var item = ItemRepository.Get("4141500163");
            var expectedNet = (item.SellPrice / item.SellMultiple) * 10;
            var expectedTax = TransactionHandlerHelper.GetTaxAmount(item) * 10;

            // Act
            for (int i = 0; i < 10; i++)
                Handler.AddItem(item.ItemId);

            // Assert
            Assert.AreEqual(expectedNet, Handler.NetTotal, 0.0001);
            Assert.AreEqual(expectedTax, Handler.TaxTotal, 0.0001);
            Assert.AreEqual(10, Handler.ItemCount);
        }

        [Test]
        public void AddNotFoundItem()
        {
            Handler.AddItem("0");
            Assert.AreEqual(0, Handler.NetTotal);
            Assert.AreEqual(0, Handler.ItemCount);
            Assert.AreEqual(0, Handler.GrossTotal);
            Assert.AreEqual(0, Handler.TaxTotal);
        }

        [Test]
        public void AddAllItems()
        {
            var expectedNet = ItemRepository.Items
                .Select(i => Math.Round(i.SellPrice / i.SellMultiple, 2))
                .Sum();

            var expectedTax = ItemRepository.Items
                .Select(i => TransactionHandlerHelper.GetTaxAmount(i))
                .Sum();

            foreach (var item in ItemRepository.Items)
            {
                if (item.Weighed)
                    Handler.AddItem(item.ItemId, 1.0);
                else
                    Handler.AddItem(item.ItemId);
            }

            Assert.AreEqual(expectedNet, Handler.NetTotal, 0.0001);
            Assert.AreEqual(expectedTax, Handler.TaxTotal, 0.0001);
            Assert.AreEqual(expectedNet + expectedTax, Handler.GrossTotal, 0.0001);
            Assert.AreEqual(ItemRepository.Items.Count(), Handler.ItemCount);
        }

    }
}
