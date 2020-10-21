using NUnit.Framework;
using Retail.POS.Common.Interfaces;
using Retail.POS.Tests.MockClasses;
using System;
using System.Linq;

namespace Retail.POS.Tests.TransactionTests
{
    public class TransactionNetTotalShould
    {
        private ITransaction Order { get; set; }
        private double TaxRate1 { get; set; }
        private double TaxRate2 { get; set; }
        private double TaxRate3 { get; set; }
        private double TaxRate4 { get; set; }

        [SetUp]
        public void SetUp()
        {
            Order = TestManager.CreateTransaction();
            TaxRate1 = double.Parse(TestManager.MockConfig.GetSection("Taxes:Rate1").Value);
            TaxRate2 = double.Parse(TestManager.MockConfig.GetSection("Taxes:Rate2").Value);
            TaxRate3 = double.Parse(TestManager.MockConfig.GetSection("Taxes:Rate3").Value);
            TaxRate4 = double.Parse(TestManager.MockConfig.GetSection("Taxes:Rate4").Value);
        }

        [Test]
        public void ReturnCorrectAfterAddingItems()
        {
            // Price, Multiple
            var unitScenarios = new (double, int, int)[]
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
                Order.Add(scenario.Item1, scenario.Item2);

            foreach (var scenario in weightScenarios)
                Order.Add(scenario.Item1, scenario.Item2);

            var expectedTotal =
                unitScenarios.Sum(i => CalculateNet(i.Item1, i.Item2)) +
                weightScenarios.Sum(i => CalculateNet(i.Item1, i.Item2));

            Assert.AreEqual(expectedTotal, Order.NetTotal, 0.0001);
            Assert.AreEqual(expectedTotal, Order.GrossTotal, 0.0001);
            Assert.AreEqual(0, Order.TaxTotal, 0.0001);
        }

        [Test]
        public void ReturnCorrectAfterAddingAndVoidingUnweighedItems()
        {
            var item1 = new MockItem()
            {
                ItemId = "123",
                SellPrice = 1.25,
                SellMultiple = 1
            };

            var item2 = new MockItem()
            {
                ItemId = "123",
                SellPrice = 2.00,
                SellMultiple = 1
            };
            
            var item3 = new MockItem()
            {
                ItemId = "123",
                SellPrice = 3.00,
                SellMultiple = 2
            };

            Order.Add(item1, 1);
            Order.Add(item2, 1);
            Order.Add(item3, 1);
            Order.Void(item2, 1);

            var expectedTotal = 
                CalculateNet(item1, 1) + 
                CalculateNet(item3, 1);

            Assert.AreEqual(expectedTotal, Order.NetTotal, 0.0001);
            Assert.AreEqual(expectedTotal, Order.GrossTotal, 0.0001);
            Assert.AreEqual(0, Order.TaxTotal, 0.0001);
        }

        [Test]
        public void AddingTaxedItemsReturnsCorrectTotals()
        {
            var item = new MockItem()
            {
                ItemId = "123",
                Tax1 = true,
                SellPrice = 1.25,
                SellMultiple = 1,
            };

            Order.Add(item, 1);
            Assert.AreEqual(CalculateNet(item, 1), Order.NetTotal);
            Assert.AreEqual(CalculateGross(item, 1), Order.GrossTotal);
            Assert.AreEqual(CalculateTaxes(item, 1), Order.TaxTotal);
        }

        private double CalculateNet(MockItem item, double quantity)
            => (item.SellPrice / item.SellMultiple) * quantity;

        private double CalculateTaxes(MockItem item, double quantity)
        {
            double rateTotal = 0;
            if (item.Tax1)
                rateTotal += TaxRate1;
            if (item.Tax2)
                rateTotal += TaxRate2;
            if (item.Tax3)
                rateTotal += TaxRate3;
            if (item.Tax4)
                rateTotal += TaxRate4;

            return CalculateNet(item, quantity) * rateTotal;
        }

        private double CalculateGross(MockItem item, double quantity)
            => CalculateNet(item, quantity) + 
            CalculateTaxes(item, quantity);
    }
}
