﻿using NUnit.Framework;
using Retail.POS.Common.Models;
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
                Handler.VoidItem(args);
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
                Handler.VoidItem(args);
                Handler.VoidItem(args);
                Assert.AreEqual(0, Handler.TaxTotal);
                Assert.AreEqual(0, Handler.NetTotal);
                Assert.AreEqual(0, Handler.GrossTotal);
                Assert.AreEqual(0, Handler.ItemCount);

                Handler.AddItem(args);
                Handler.VoidItem(args);
                Handler.AddItem(args);
                Handler.VoidItem(args);
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
                var args = new AddItemArgs()
                {
                    ItemId = item.ItemId,
                    SellPrice = item.SellPrice,
                    SellMultiple = item.SellMultiple,
                    Quantity = 1,
                    Weight = weight,
                    PriceOverride = false,
                };
                Handler.AddItem(args);
                Handler.VoidItem(args);
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
                var args = new AddItemArgs()
                {
                    ItemId = item.ItemId,
                    SellPrice = item.SellPrice,
                    SellMultiple = item.SellMultiple,
                    Quantity = 1,
                    Weight = 0,
                    PriceOverride = false,
                };
                Handler.VoidItem(args);
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

                var args = new AddItemArgs()
                {
                    ItemId = item.ItemId,
                    SellPrice = item.SellPrice,
                    SellMultiple = item.SellMultiple,
                    Quantity = 1,
                    Weight = weight,
                    PriceOverride = false,
                };
                handler.AddItem(args);
                handler.AddItem(args);
                handler.VoidItem(args);

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
                handler.AddItem(args);
                handler.VoidItem(args);

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
                handler.AddItem(args);
                handler.VoidItem(args);

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
                args.Quantity += 1;
                handler.AddItem(args);
                args.Quantity -= 1;
                handler.VoidItem(args);

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

                var args = new AddItemArgs()
                {
                    ItemId = item.ItemId,
                    SellPrice = item.SellPrice,
                    SellMultiple = item.SellMultiple,
                    Quantity = 5,
                    Weight = 0,
                    PriceOverride = false,
                };
                handler.AddItem(args);
                args.Quantity = 6;
                handler.AddItem(args);
                args.Quantity = 12;
                handler.VoidItem(args);

                Assert.AreEqual(expectedTax, handler.TaxTotal, 0.0001);
                Assert.AreEqual(expectedNet, handler.NetTotal, 0.0001);
                Assert.AreEqual(expectedGross, handler.GrossTotal, 0.0001);
                Assert.AreEqual(11, handler.ItemCount);
            }
        }

    }
}
