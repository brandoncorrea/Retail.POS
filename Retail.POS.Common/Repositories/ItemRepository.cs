using Newtonsoft.Json;
using Retail.POS.Common.Models;
using Retail.POS.Common.Models.LineItems;
using System.IO;
using System.Linq;

namespace Retail.POS.Common.Repositories
{
    public class ItemRepository : IItemRepository
    {
        public IItem Get(object id)
        {
            long gtin = long.Parse(id.ToString());
            var jsonStr = File.ReadAllText("itemStore.json");
            var items = JsonConvert.DeserializeObject<PosItem[]>(jsonStr);
            return items.FirstOrDefault(i => long.Parse(i.ItemId) == gtin);
        }
    }
}
