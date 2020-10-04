using NUnit.Framework;
using Retail.POS.Common.Models;
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
                var args = new AddItemArgs()
                {
                    ItemId = item.ItemId,
                    PriceOverride = false,
                    Quantity = 1,
                    SellMultiple = item.SellMultiple,
                    Weight = weight,
                    SellPrice = item.SellPrice
                };
                handler.AddItem(args);
                
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
                var args = new AddItemArgs()
                {
                    ItemId = item.ItemId,
                    SellPrice = item.SellPrice,
                    SellMultiple = item.SellMultiple,
                    Quantity = 1,
                    Weight = 0,
                    PriceOverride = false,
                };
                handler.AddItem(args);

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
                var args = new AddItemArgs()
                {
                    ItemId = item.ItemId,
                    SellPrice = item.SellPrice,
                    SellMultiple = item.SellMultiple,
                    Quantity = quantity,
                    Weight = 0,
                    PriceOverride = false,
                };
                handler.AddItem(args);

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
            var args = new AddItemArgs()
            {
                ItemId = item.ItemId,
                SellPrice = item.SellPrice,
                SellMultiple = item.SellMultiple,
                Quantity = 1,
                Weight = 0,
                PriceOverride = false,
            };
            for (int i = 0; i < 10; i++)
                Handler.AddItem(args);

            // Assert
            Assert.AreEqual(expectedNet, Handler.NetTotal, 0.0001);
            Assert.AreEqual(expectedTax, Handler.TaxTotal, 0.0001);
            Assert.AreEqual(10, Handler.ItemCount);
        }

        [Test]
        public void AddNotFoundItem()
        {
            var args = new AddItemArgs()
            {
                ItemId = "0",
            };
            Handler.AddItem(args);
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
                var args = new AddItemArgs()
                {
                    ItemId = item.ItemId,
                    SellPrice = item.SellPrice,
                    SellMultiple = item.SellMultiple,
                    Quantity = 1,
                    Weight = 1.0,
                    PriceOverride = false,
                };
                Handler.AddItem(args);
            }

            Assert.AreEqual(expectedNet, Handler.NetTotal, 0.0001);
            Assert.AreEqual(expectedTax, Handler.TaxTotal, 0.0001);
            Assert.AreEqual(expectedNet + expectedTax, Handler.GrossTotal, 0.0001);
            Assert.AreEqual(ItemRepository.Items.Count(), Handler.ItemCount);
        }

    }
}
