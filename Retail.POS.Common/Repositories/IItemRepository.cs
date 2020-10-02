namespace Retail.POS.Common.Repositories
{
    public interface IItemRepository<T>
    {
        public T Get(object id);
    }
}
