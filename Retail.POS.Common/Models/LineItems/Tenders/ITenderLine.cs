namespace Retail.POS.Common.Models.LineItems.Tenders
{
    public interface ITenderLine : ILineItem
    {
        TenderType TenderType { get; set; }
    }
}
