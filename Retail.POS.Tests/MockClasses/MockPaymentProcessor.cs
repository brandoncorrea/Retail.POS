using NUnit.Framework;
using Retail.POS.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Retail.POS.Tests.MockClasses
{
    public class MockPaymentProcessor : IPaymentProcessor
    {
        private List<Tender> Tenders { get; set; }
        
        public MockPaymentProcessor()
        {
            Tenders = new List<Tender>();
        }
        
        public Tender ProcessPayment(string method, double amount)
        {
            method = method.ToUpper();
            Tender tender;
            
            if (method == "CASH")
                tender = TenderCash(method, amount);
            else
                tender = FailTender(method, amount);
            
            Tenders.Add(tender);
            return tender;
        }

        public Tender ReversePayment(string reference)
        {
            var tender = Tenders.FirstOrDefault(i => i.Reference == reference);
            if (tender != null)
                tender.Reversed = true;
            return tender;
        }

        private Tender FailTender(string method, double amount) => new Tender()
        {
            Method = method,
            Amount = amount,
            Succeeded = false,
            Reversed = false,
            Description = $"{method} tender for {amount} failed",
            Details = $"{method} ${amount}",
            Reference = Guid.NewGuid().ToString(),
        };

        private Tender TenderCash(string method, double amount) => new Tender()
        {
            Method = method,
            Amount = amount,
            Reference = Guid.NewGuid().ToString(),
            Succeeded = true,
            Reversed = false,
            Details = $"A {method} tender for {amount}",
            Description = $"{method} ${amount}",
        };
    }
}
