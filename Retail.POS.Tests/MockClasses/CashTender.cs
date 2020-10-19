using Retail.POS.Common.Interfaces;

namespace Retail.POS.Tests.MockClasses
{
    public class CashTender : ITender
    {
        public double Amount { get; set; }
        public string Description { get; set; }
        public string Method { get; set; }
        public object Details { get; set; }
        public string Reference { get; set; }
        public bool Succeeded { get; set; }
    }
}
