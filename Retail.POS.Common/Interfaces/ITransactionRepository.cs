namespace Retail.POS.Common.Interfaces
{
    public interface ITransactionRepository
    {
        public ITransaction Get(object id);
        public void StoreCompleted(ITransaction transaction);
        public void StoreIncompleted(ITransaction transaction);
        public void StoreVoided(ITransaction transaction);
    }
}
