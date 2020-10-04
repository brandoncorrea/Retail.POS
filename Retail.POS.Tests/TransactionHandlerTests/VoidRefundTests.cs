using NUnit.Framework;
using Retail.POS.Common.Models;
using Retail.POS.Common.TransactionHandler;
using Retail.POS.Tests.Helpers;
using Retail.POS.Tests.MockClasses;
using System;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;

namespace Retail.POS.Tests.TransactionHandlerTests
{
    public class VoidRefundTests
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
        public void RefundAndVoidRefundOneItem()
        {
            foreach (var item in ItemRepo.Items)
            {
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
                Handler.VoidRefund(args);
                Assert.AreEqual(0, Handler.TaxTotal);
                Assert.AreEqual(0, Handler.NetTotal);
                Assert.AreEqual(0, Handler.GrossTotal);
                Assert.AreEqual(0, Handler.ItemCount);
            }
        }

        [Test]
        public void RefundAndVoidRefundItems()
        {
            foreach (var item in ItemRepo.Items)
            {
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
                Handler.VoidRefund(args);
                Handler.VoidRefund(args);
                Assert.AreEqual(0, Handler.TaxTotal);
                Assert.AreEqual(0, Handler.NetTotal);
                Assert.AreEqual(0, Handler.GrossTotal);
                Assert.AreEqual(0, Handler.ItemCount);

                Handler.RefundItem(args);
                Handler.VoidRefund(args);
                Handler.RefundItem(args);
                Handler.VoidRefund(args);
                Assert.AreEqual(0, Handler.TaxTotal);
                Assert.AreEqual(0, Handler.NetTotal);
                Assert.AreEqual(0, Handler.GrossTotal);
                Assert.AreEqual(0, Handler.ItemCount);
            }
        }

        [Test]
        public void RefundAndVoidRefundWeighedItems()
        {
            var random = new Random();
            var items = ItemRepo.Items.Where(i => i.Weighed);
            foreach (var item in ItemRepo.Items)
            {
                var args = new AddItemArgs()
                {
                    ItemId = item.ItemId,
                    SellPrice = item.SellPrice,
                    SellMultiple = item.SellMultiple,
                    Quantity = 1,
                    Weight = random.NextDouble() * 10,
                    PriceOverride = false,
                };
                Handler.RefundItem(args);
                Handler.VoidRefund(args);
                Assert.AreEqual(0, Handler.TaxTotal);
                Assert.AreEqual(0, Handler.NetTotal);
                Assert.AreEqual(0, Handler.GrossTotal);
                Assert.AreEqual(0, Handler.ItemCount);
            }
        }

        [Test]
        public void VoidRefundNotInTransaction()
        {
            foreach (var item in ItemRepo.Items)
            {
                var args = new AddItemArgs()
                {
                    ItemId = item.ItemId,
                    SellPrice = item.SellPrice,
                    SellMultiple = item.SellMultiple,
                    Quantity = 1,
                    Weight = 0,
                    PriceOverride = false,
                };
                Handler.VoidRefund(args);
                Assert.AreEqual(0, Handler.TaxTotal);
                Assert.AreEqual(0, Handler.NetTotal);
                Assert.AreEqual(0, Handler.GrossTotal);
                Assert.AreEqual(0, Handler.ItemCount);
            }
        }

        [Test]
        public void RefundTwoAndVoidRefundOneWeighedItem()
        {
            var random = new Random();
            var items = ItemRepo.Items.Where(i => i.Weighed);
            foreach (var item in items)
            {
                var handler = new TransactionHandler(TestManager.Config, ItemRepo);
                var weight = random.NextDouble() * 10;
                var unitPrice = Math.Round(item.SellPrice / item.SellMultiple, 2);

                var expectedTax = TransactionHandlerHelper.GetTaxAmount(item) * -1;
                var expectedNet = Math.Round(unitPrice * weight, 2) * -1;
                var expectedGross = expectedNet + expectedTax;

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
                handler.RefundItem(args);
                handler.VoidRefund(args);

                Assert.AreEqual(expectedTax, handler.TaxTotal);
                Assert.AreEqual(expectedNet, handler.NetTotal);
                Assert.AreEqual(expectedGross, handler.GrossTotal);
                Assert.AreEqual(0, handler.ItemCount);
            }
        }

        [Test]
        public void RefundTwoAndVoidRefundOneUnweighedItem()
        {
            var items = ItemRepo.Items.Where(i => !i.Weighed);
            foreach (var item in items)
            {
                var handler = new TransactionHandler(TestManager.Config, ItemRepo);

                var expectedNet = Math.Round(item.SellPrice / item.SellMultiple, 2) * -1;
                var expectedTax = TransactionHandlerHelper.GetTaxAmount(item) * -1;
                var expectedGross = expectedNet + expectedTax;

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
                handler.RefundItem(args);
                handler.VoidRefund(args);

                Assert.AreEqual(expectedTax, handler.TaxTotal);
                Assert.AreEqual(expectedNet, handler.NetTotal);
                Assert.AreEqual(expectedGross, handler.GrossTotal);
                Assert.AreEqual(0, handler.ItemCount);
            }
        }

        [Test]
        public void RefundAndVoidRefundItemsWithQuantity()
        {
            var random = new Random();
            var items = ItemRepo.Items.Where(i => !i.Weighed);
            foreach (var item in items)
            {
                var handler = new TransactionHandler(TestManager.Config, ItemRepo);
                var quantity = random.Next(2, 100);

                var expectedNet = Math.Round(item.SellPrice / item.SellMultiple, 2) * quantity * -1;
                var expectedTax = TransactionHandlerHelper.GetTaxAmount(item) * quantity * -1;
                var expectedGross = expectedNet + expectedTax;

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
                handler.RefundItem(args);
                handler.VoidRefund(args);

                Assert.AreEqual(expectedTax, handler.TaxTotal, 0.0001);
                Assert.AreEqual(expectedNet, handler.NetTotal, 0.0001);
                Assert.AreEqual(expectedGross, handler.GrossTotal);
                Assert.AreEqual(0, handler.ItemCount);
            }
        }

        [Test]
        public void RefundAndVoidRefundItemsWithQuantityLessThanAdded()
        {
            var random = new Random();
            var items = ItemRepo.Items.Where(i => !i.Weighed);
            foreach (var item in items)
            {
                var handler = new TransactionHandler(TestManager.Config, ItemRepo);
                var quantity = random.Next(2, 100);

                var expectedNet = Math.Round(item.SellPrice / item.SellMultiple, 2) * (quantity + 1) * -1;
                var expectedTax = TransactionHandlerHelper.GetTaxAmount(item) * (quantity + 1) * -1;
                var expectedGross = expectedNet + expectedTax;

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
                args.Quantity += 1;
                handler.RefundItem(args);
                args.Quantity -= 1;
                handler.VoidRefund(args);

                Assert.AreEqual(expectedTax, handler.TaxTotal, 0.0001);
                Assert.AreEqual(expectedNet, handler.NetTotal, 0.0001);
                Assert.AreEqual(expectedGross, handler.GrossTotal, 0.0001);
                Assert.AreEqual(0, handler.ItemCount);
            }
        }

        [Test]
        public void VoidRefundItemsGreaterThanTotalInOrder()
        {
            var items = ItemRepo.Items.Where(i => !i.Weighed);
            foreach (var item in items)
            {
                var handler = new TransactionHandler(TestManager.Config, ItemRepo);

                var expectedNet = Math.Round(item.SellPrice / item.SellMultiple, 2) * -11;
                var expectedTax = TransactionHandlerHelper.GetTaxAmount(item) * -11;
                var expectedGross = expectedNet + expectedTax;

                var args = new AddItemArgs()
                {
                    ItemId = item.ItemId,
                    SellPrice = item.SellPrice,
                    SellMultiple = item.SellMultiple,
                    Quantity = 5,
                    Weight = 0,
                    PriceOverride = false,
                };
                handler.RefundItem(args);
                args.Quantity = 6;
                handler.RefundItem(args);
                args.Quantity = 12;
                handler.VoidRefund(args);

                Assert.AreEqual(expectedTax, handler.TaxTotal, 0.0001);
                Assert.AreEqual(expectedNet, handler.NetTotal, 0.0001);
                Assert.AreEqual(expectedGross, handler.GrossTotal, 0.0001);
                Assert.AreEqual(0, handler.ItemCount);
            }
        }


    }
}
