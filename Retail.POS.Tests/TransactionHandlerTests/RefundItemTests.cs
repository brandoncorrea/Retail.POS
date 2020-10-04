using NUnit.Framework;
using Retail.POS.Common.Models;
using Retail.POS.Common.TransactionHandler;
using Retail.POS.Tests.Helpers;
using Retail.POS.Tests.MockClasses;
using System;
using System.Linq;

namespace Retail.POS.Tests.TransactionHandlerTests
{
    public class RefundItemTests
    {
        public TransactionHandler Handler;
        public MockItemRepository ItemRepo;

        [SetUp]
        public void SetUp()
        {
            ItemRepo = new MockItemRepository();
            Handler = new TransactionHandler(TestManager.Config, ItemRepo);
        }

        [Test]
        public void RefundOneWeighedItem()
        {
            var random = new Random();
            var items = ItemRepo.Items.Where(i => i.Weighed);
            foreach (var item in items)
            {
                // Setup
                var handler = new TransactionHandler(TestManager.Config, ItemRepo);
                var weight = Math.Round(random.NextDouble() * 10, 2);
                double expectedNet = (item.SellPrice / item.SellMultiple) * weight * -1;
                double expectedTax = TransactionHandlerHelper.GetTaxAmount(item, weight) * -1;

                // Act
                var args = new AddItemArgs()
                {
                    ItemId = item.ItemId,
                    SellPrice = item.SellPrice,
                    SellMultiple = item.SellMultiple,
                    Quantity = 1,
                    Weight = weight,
                    PriceOverride = false,
                };
                handler.RefundItem(args);

                // Assert
                Assert.AreEqual(expectedTax, handler.TaxTotal);
                Assert.AreEqual(Math.Round(expectedNet, 2), Math.Round(handler.NetTotal, 2), 0.0001);
                Assert.AreEqual(0, handler.ItemCount);
            }
        }

        [Test]
        public void RefundOneUnweighedItem()
        {
            var random = new Random();
            var items = ItemRepo.Items.Where(i => !i.Weighed);
            foreach (var item in items)
            {
                // Setup
                var handler = new TransactionHandler(TestManager.Config, ItemRepo);
                double expectedNet = (item.SellPrice / item.SellMultiple) * -1;
                double expectedTax = TransactionHandlerHelper.GetTaxAmount(item) * -1;

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
                handler.RefundItem(args);

                // Assert
                Assert.AreEqual(expectedTax, handler.TaxTotal);
                Assert.AreEqual(Math.Round(expectedNet, 2), Math.Round(handler.NetTotal, 2), 0.0001);
                Assert.AreEqual(0, handler.ItemCount);
            }
        }

        [Test]
        public void RefundManyUnweighedItems()
        {
            var random = new Random();
            var items = ItemRepo.Items.Where(i => !i.Weighed);
            foreach (var item in items)
            {
                // Setup
                var handler = new TransactionHandler(TestManager.Config, ItemRepo);
                int quantity = random.Next(1, 100);
                double expectedNet = Math.Round(item.SellPrice / item.SellMultiple, 2) * quantity * -1;
                double expectedTax = TransactionHandlerHelper.GetTaxAmount(item, quantity) * -1;

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
                handler.RefundItem(args);

                // Assert
                Assert.AreEqual(expectedTax, handler.TaxTotal, 0001);
                Assert.AreEqual(Math.Round(expectedNet, 2), Math.Round(handler.NetTotal, 2), 0.0001);
                Assert.AreEqual(0, handler.ItemCount);
            }
        }

        [Test]
        public void RefundTenItems()
        {
            // SetUp
            var item = ItemRepo.Get("4141500163");
            var expectedNet = (item.SellPrice / item.SellMultiple) * -10;
            var expectedTax = TransactionHandlerHelper.GetTaxAmount(item) * -10;

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
                Handler.RefundItem(args);

            // Assert
            Assert.AreEqual(expectedNet, Handler.NetTotal, 0.0001);
            Assert.AreEqual(expectedTax, Handler.TaxTotal, 0.0001);
            Assert.AreEqual(0, Handler.ItemCount);
        }

        [Test]
        public void AddAndRefundItemMultipleTimes()
        {
            // SetUp
            var item = ItemRepo.Get("4141500163");

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
            Handler.AddItem(args);
            Handler.RefundItem(args);
            Handler.AddItem(args);
            Handler.RefundItem(args);

            // Assert
            Assert.AreEqual(0, Handler.NetTotal, 0.0001);
            Assert.AreEqual(0, Handler.TaxTotal, 0.0001);
            Assert.AreEqual(0, Handler.GrossTotal, 0.0001);
            Assert.AreEqual(2, Handler.ItemCount);
        }

        [Test]
        public void AddItemTwiceAndRefundOnce()
        {
            // SetUp
            var item = ItemRepo.Get("4141500163");
            var unitPrice = item.SellPrice / item.SellMultiple;
            var expectedTax = TransactionHandlerHelper.GetTaxAmount(item);

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
            Handler.AddItem(args);
            Handler.AddItem(args);
            Handler.RefundItem(args);

            // Assert
            Assert.AreEqual(unitPrice, Handler.NetTotal, 0.0001);
            Assert.AreEqual(expectedTax, Handler.TaxTotal, 0.0001);
            Assert.AreEqual(unitPrice + expectedTax, Handler.GrossTotal, 0.0001);
            Assert.AreEqual(2, Handler.ItemCount);
        }

        [Test]
        public void RefundItemTwiceAndAddOnce()
        {
            // SetUp
            var item = ItemRepo.Get("4141500163");
            var unitPrice = item.SellPrice / item.SellMultiple * -1;
            var expectedTax = TransactionHandlerHelper.GetTaxAmount(item) * -1;

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
            Handler.RefundItem(args);
            Handler.RefundItem(args);
            Handler.AddItem(args);

            // Assert
            Assert.AreEqual(unitPrice, Handler.NetTotal, 0.0001);
            Assert.AreEqual(expectedTax, Handler.TaxTotal, 0.0001);
            Assert.AreEqual(unitPrice + expectedTax, Handler.GrossTotal, 0.0001);
            Assert.AreEqual(1, Handler.ItemCount);
        }

        [Test]
        public void RefundNotFoundItem()
        {
            var args = new AddItemArgs()
            {
                ItemId = "0",
            };
            Handler.RefundItem(args);
            Assert.AreEqual(0, Handler.NetTotal);
            Assert.AreEqual(0, Handler.ItemCount);
            Assert.AreEqual(0, Handler.GrossTotal);
            Assert.AreEqual(0, Handler.TaxTotal);
        }

        [Test]
        public void RefundAllItems()
        {
            var expectedNet = ItemRepo.Items
                .Select(i => Math.Round(i.SellPrice / i.SellMultiple, 2))
                .Sum() * -1;

            var expectedTax = ItemRepo.Items
                .Select(i => TransactionHandlerHelper.GetTaxAmount(i))
                .Sum() * -1;

            foreach (var item in ItemRepo.Items)
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
                Handler.RefundItem(args);
            }

            Assert.AreEqual(expectedNet, Handler.NetTotal, 0.0001);
            Assert.AreEqual(expectedTax, Handler.TaxTotal, 0.0001);
            Assert.AreEqual(expectedNet + expectedTax, Handler.GrossTotal, 0.0001);
            Assert.AreEqual(0, Handler.ItemCount);
        }
    }
}
