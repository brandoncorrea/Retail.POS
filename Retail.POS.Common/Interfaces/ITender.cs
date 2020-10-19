namespace Retail.POS.Common.Interfaces
{
    public interface ITender
    {
        public double Amount { get; }
        public string Description { get; }
        public string Method { get; }
        public object Details { get; }
        public string Reference { get; }
        public bool Succeeded { get; }
    }
}
