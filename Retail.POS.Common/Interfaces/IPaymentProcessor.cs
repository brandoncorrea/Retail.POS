namespace Retail.POS.Common.Models
{
    public interface IPaymentProcessor
    {
        public Tender ProcessPayment(string method, double amount);
        public Tender ReversePayment(string reference);
    }
}
