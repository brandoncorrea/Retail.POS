namespace Retail.POS.Common.Models
{
    public class Tender
    {
        public double Amount { get; set; }
        public string Description { get; set; }
        public string Method { get; set; }
        public object Details { get; set; }
        public string Reference { get; set; }
        public bool Succeeded { get; set; }
        public bool Reversed { get; set; }
    }
}
