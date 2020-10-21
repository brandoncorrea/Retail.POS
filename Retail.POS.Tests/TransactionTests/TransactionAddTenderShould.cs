using NUnit.Framework;
using Retail.POS.Common.Interfaces;
using Retail.POS.Tests.MockClasses;
using System.Linq;

namespace Retail.POS.Tests.TransactionTests
{
    public class TransactionAddTenderShould
    {
        private ITransaction Order { get; set; }

        [SetUp]
        public void SetUp()
        {
            Order = TestManager.CreateTransaction();
        }

        [Test]
        public void ReduceBalance()
        {
            var item = new MockItem()
            {
                ItemId = "123",
                SellPrice = 1.25,
                SellMultiple = 1
            };

            Order.Add(item, 1);
            Order.AddTender("CASH", 1.00);
            Assert.AreEqual(0.25, Order.Balance);
        }

        [Test]
        public void OnlyReduceBalanceBySuccessfulTenders()
        {
            var item = new MockItem()
            {
                ItemId = "123",
                SellPrice = 5.00,
                SellMultiple = 1
            };

            Order.Add(item, 1);
            Order.AddTender("CASH", 1.00);
            Order.AddTender("FAIL", 3.00);
            Assert.AreEqual(4.00, Order.Balance);
        }

        [Test]
        public void DisplayNegativeBalanceForOvertender()
        {
            var item = new MockItem()
            {
                ItemId = "123",
                SellPrice = 5.00,
                SellMultiple = 1
            };

            Order.Add(item, 1);
            Order.AddTender("CASH", 6.00);
            Assert.AreEqual(-1.00, Order.Balance);
        }
        
        [Test]
        public void AppearInTenderList()
        {
            Order.AddTender("CASH", 1.00);
            var tender = Order.Tenders.FirstOrDefault();
            Assert.AreEqual("CASH", tender.Method);
            Assert.AreEqual(1.00, tender.Amount);
        }

        [Test]
        public void ReturnsCorrectCount()
        {
            Order.AddTender("CASH", 1.00);
            Order.AddTender("CASH", 1.00);
            Assert.AreEqual(2, Order.Tenders.Count());
        }
    }
}
