using Retail.POS.Common.Models.LineItems.Tenders;

namespace Retail.POS.Common.Models
{
    public interface ITender
    {
        public TenderType TenderType { get; }
        public double TenderAmount { get; }
        public string AccountNumber { get; }
        public string ReferenceNumber { get; }
        public string TraceNumber { get; }
        public string AuthorizationNumber { get; }
    }
}
