namespace Retail.POS.Common.Models.LineItems.Tenders
{
    public enum TenderType
    {
        Credit = 0,
        Debit = 1,
        EbtWic = 2,
        EbtCash = 4,
        EbtFood = 8,
        PersonalCheck = 16,
        CashierCheck = 32,
        Cash = 64,
        MobilePay = 128,
        ApplePay = 256,
    }
}
