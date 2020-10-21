namespace Retail.POS.Common.Interfaces
{
    public interface ITransactionHandler
    {
        public ITransaction Create();
        public ITransaction Recall(object id);
        public void Save(ITransaction transaction);
        public void Complete(ITransaction transaction);
    }
}
