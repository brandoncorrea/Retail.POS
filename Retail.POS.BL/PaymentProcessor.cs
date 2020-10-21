using Retail.POS.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Retail.POS.BL
{
    public class PaymentProcessor : IPaymentProcessor
    {
        private List<Tender> TenderStore { get; set; }

        public PaymentProcessor()
        {
            TenderStore = new List<Tender>();
        }

        public Tender ProcessPayment(string method, double amount)
        {
            var tender = new Tender()
            {
                Method = method,
                Amount = amount,
                Reference = Guid.NewGuid().ToString(),
                Succeeded = IsValidPaymentMethod(method),
            };

            TenderStore.Add(tender);
            return tender;
        }

        public Tender ReversePayment(string reference)
        {
            var tender = TenderStore.FirstOrDefault(i => i.Reference == reference);
            TenderStore.RemoveAll(i => i.Reference == reference);
            tender.Reversed = true;
            TenderStore.Add(tender);
            return tender;
        }

        private bool IsValidPaymentMethod(string method) =>
            method.ToUpper() == "CASH";
    }
}
