using NUnit.Framework;
using Retail.POS.BL;
using Retail.POS.Common.Models;

namespace Retail.POS.Tests.PaymentProcessorTests
{
    public class PaymentProcessorShould
    {
        private IPaymentProcessor Processor { get; set; }

        [SetUp]
        public void SetUp()
        {
            Processor = new PaymentProcessor();
        }

        [Test]
        [TestCase("Cash", 1)]
        [TestCase("cASH", 0.01)]
        [TestCase("CASH", 100.00)]
        public void PerformCashTenders(string method, double amount)
        {
            var tender = Processor.ProcessPayment(method, amount);
            Assert.AreEqual(amount, tender.Amount);
            Assert.IsTrue(tender.Succeeded);
            Assert.AreEqual(method.ToUpper(), tender.Method.ToUpper());
        }

        [Test]
        [TestCase("Credit", 1.00)]
        [TestCase("Debit", 1.00)]
        [TestCase("Ebt", 1.00)]
        [TestCase("Check", 1.00)]
        [TestCase("Wic", 1.00)]
        [TestCase("eWIC", 1.00)]
        public void CannotPerformNoncashTenders(string method, double amount)
        {
            var tender = Processor.ProcessPayment(method, amount);
            Assert.AreEqual(amount, tender.Amount);
            Assert.AreEqual(method.ToUpper(), tender.Method.ToUpper());
            Assert.IsFalse(tender.Succeeded);
        }

        [Test]
        [TestCase(1)]
        [TestCase(0.01)]
        [TestCase(100.00)]
        [TestCase(-1)]
        [TestCase(-0.01)]
        [TestCase(0)]
        public void ReverseCashTenders(double amount)
        {
            var tender = Processor.ProcessPayment("Cash", amount);
            var reversal = Processor.ReversePayment(tender.Reference);
            Assert.AreEqual(tender, reversal);
        }
    }
}
