using Retail.POS.Common.Models.LineItems;

namespace Retail.POS.Common.Repositories
{
    public interface IItemRepository
    {
        public IItem Get(object id);
    }
}
