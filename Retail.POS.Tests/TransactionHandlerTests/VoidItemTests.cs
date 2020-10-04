using NUnit.Framework;
using Retail.POS.Common.TransactionHandler;
using Retail.POS.Tests.Helpers;
using Retail.POS.Tests.MockClasses;
using System;
using System.Linq;

namespace Retail.POS.Tests.TransactionHandlerTests
{
    public class VoidItemTests
    {
        public MockItemRepository ItemRepo { get; private set; }
        public TransactionHandler Handler { get; private set; }

        [SetUp]
        public void SetUp()
        {
            ItemRepo = new MockItemRepository();
            Handler = new TransactionHandler(TestManager.Config, ItemRepo);
        }

        [Test]
        public void AddAndVoidOneItem()
        {
            foreach(var item in ItemRepo.Items)
            {
                Handler.AddItem(item.ItemId);
                Handler.VoidItem(item.ItemId);
                Assert.AreEqual(0, Handler.TaxTotal);
                Assert.AreEqual(0, Handler.NetTotal);
                Assert.AreEqual(0, Handler.GrossTotal);
                Assert.AreEqual(0, Handler.ItemCount);
            }
        }

        [Test]
        public void AddAndVoidItems()
        {
            foreach (var item in ItemRepo.Items)
            {
                Handler.AddItem(item.ItemId);
                Handler.AddItem(item.ItemId);
                Handler.VoidItem(item.ItemId);
                Handler.VoidItem(item.ItemId);
                Assert.AreEqual(0, Handler.TaxTotal);
                Assert.AreEqual(0, Handler.NetTotal);
                Assert.AreEqual(0, Handler.GrossTotal);
                Assert.AreEqual(0, Handler.ItemCount);

                Handler.AddItem(item.ItemId);
                Handler.VoidItem(item.ItemId);
                Handler.AddItem(item.ItemId);
                Handler.VoidItem(item.ItemId);
                Assert.AreEqual(0, Handler.TaxTotal);
                Assert.AreEqual(0, Handler.NetTotal);
                Assert.AreEqual(0, Handler.GrossTotal);
                Assert.AreEqual(0, Handler.ItemCount);
            }
        }

        [Test]
        public void AddAndVoidWeighedItems()
        {
            var random = new Random();
            var items = ItemRepo.Items.Where(i => i.Weighed);
            foreach (var item in ItemRepo.Items)
            {
                var weight = random.NextDouble() * 10;
                Handler.AddItem(item.ItemId, weight);
                Handler.VoidItem(item.ItemId, weight);
                Assert.AreEqual(0, Handler.TaxTotal);
                Assert.AreEqual(0, Handler.NetTotal);
                Assert.AreEqual(0, Handler.GrossTotal);
                Assert.AreEqual(0, Handler.ItemCount);
            }
        }

        [Test]
        public void VoidItemNotInTransaction()
        {
            foreach (var item in ItemRepo.Items)
            {
                Handler.VoidItem(item.ItemId);
                Assert.AreEqual(0, Handler.TaxTotal);
                Assert.AreEqual(0, Handler.NetTotal);
                Assert.AreEqual(0, Handler.GrossTotal);
                Assert.AreEqual(0, Handler.ItemCount);
            }
        }

        [Test]
        public void AddTwoAndVoidOneWeighedItem()
        {
            var random = new Random();
            var items = ItemRepo.Items.Where(i => i.Weighed);
            foreach (var item in items)
            {
                var handler = new TransactionHandler(TestManager.Config, ItemRepo);
                var weight = random.NextDouble() * 10;
                var unitPrice = Math.Round(item.SellPrice / item.SellMultiple, 2);
                
                var expectedTax = TransactionHandlerHelper.GetTaxAmount(item);
                var expectedNet = Math.Round(unitPrice * weight, 2);
                var expectedGross = expectedNet + expectedTax;

                handler.AddItem(item.ItemId, weight);
                handler.AddItem(item.ItemId, weight);
                handler.VoidItem(item.ItemId, weight);

                Assert.AreEqual(expectedTax, handler.TaxTotal);
                Assert.AreEqual(expectedNet, handler.NetTotal);
                Assert.AreEqual(expectedGross, handler.GrossTotal);
                Assert.AreEqual(1, handler.ItemCount);
            }
        }

        [Test]
        public void AddTwoAndVoidOneUnweighedItem()
        {
            var items = ItemRepo.Items.Where(i => !i.Weighed);
            foreach (var item in items)
            {
                var handler = new TransactionHandler(TestManager.Config, ItemRepo);

                var expectedNet = Math.Round(item.SellPrice / item.SellMultiple, 2);
                var expectedTax = TransactionHandlerHelper.GetTaxAmount(item);
                var expectedGross = expectedNet + expectedTax;

                handler.AddItem(item.ItemId);
                handler.AddItem(item.ItemId);
                handler.VoidItem(item.ItemId);

                Assert.AreEqual(expectedTax, handler.TaxTotal);
                Assert.AreEqual(expectedNet, handler.NetTotal);
                Assert.AreEqual(expectedGross, handler.GrossTotal);
                Assert.AreEqual(1, handler.ItemCount);
            }
        }

        [Test]
        public void AddAndVoidItemsWithQuantity()
        {
            var random = new Random();
            var items = ItemRepo.Items.Where(i => !i.Weighed);
            foreach (var item in items)
            {
                var handler = new TransactionHandler(TestManager.Config, ItemRepo);
                var quantity = random.Next(2, 100);

                var expectedNet = Math.Round(item.SellPrice / item.SellMultiple, 2) * quantity;
                var expectedTax = TransactionHandlerHelper.GetTaxAmount(item) * quantity;
                var expectedGross = expectedNet + expectedTax;

                handler.AddItem(item.ItemId, quantity);
                handler.AddItem(item.ItemId, quantity);
                handler.VoidItem(item.ItemId, quantity);

                Assert.AreEqual(expectedTax, handler.TaxTotal, 0.0001);
                Assert.AreEqual(expectedNet, handler.NetTotal);
                Assert.AreEqual(expectedGross, handler.GrossTotal);
                Assert.AreEqual(quantity, handler.ItemCount);
            }
        }

        [Test]
        public void AddAndVoidItemsWithQuantityLessThanAdded()
        {
            var random = new Random();
            var items = ItemRepo.Items.Where(i => !i.Weighed);
            foreach (var item in items)
            {
                var handler = new TransactionHandler(TestManager.Config, ItemRepo);
                var quantity = random.Next(2, 100);

                var expectedNet = Math.Round(item.SellPrice / item.SellMultiple, 2) * (quantity + 1);
                var expectedTax = TransactionHandlerHelper.GetTaxAmount(item) * (quantity + 1);
                var expectedGross = expectedNet + expectedTax;

                handler.AddItem(item.ItemId, quantity);
                handler.AddItem(item.ItemId, quantity + 1);
                handler.VoidItem(item.ItemId, quantity);

                Assert.AreEqual(expectedTax, handler.TaxTotal, 0.0001);
                Assert.AreEqual(expectedNet, handler.NetTotal, 0.0001);
                Assert.AreEqual(expectedGross, handler.GrossTotal, 0.0001);
                Assert.AreEqual(quantity + 1, handler.ItemCount);
            }
        }

        [Test]
        public void VoidItemsGreaterThanTotalInOrder()
        {
            var items = ItemRepo.Items.Where(i => !i.Weighed);
            foreach (var item in items)
            {
                var handler = new TransactionHandler(TestManager.Config, ItemRepo);

                var expectedNet = Math.Round(item.SellPrice / item.SellMultiple, 2) * 11;
                var expectedTax = TransactionHandlerHelper.GetTaxAmount(item) * 11;
                var expectedGross = expectedNet + expectedTax;

                handler.AddItem(item.ItemId, 5);
                handler.AddItem(item.ItemId, 6);
                handler.VoidItem(item.ItemId, 12);

                Assert.AreEqual(expectedTax, handler.TaxTotal, 0.0001);
                Assert.AreEqual(expectedNet, handler.NetTotal, 0.0001);
                Assert.AreEqual(expectedGross, handler.GrossTotal, 0.0001);
                Assert.AreEqual(11, handler.ItemCount);
            }
        }

    }
}
