using Newtonsoft.Json;
using Retail.POS.Common.Models.LineItems;
using System.IO;
using System.Linq;

namespace Retail.POS.Common.Repositories
{
    public class ItemRepository : IItemRepository
    {
        private IItem[] Items { get; set; }

        public ItemRepository()
        {
            var jsonStr = File.ReadAllText("Repositories\\itemStore.json");
            Items = JsonConvert.DeserializeObject<PosItem[]>(jsonStr);
        }

        public IItem Get(object id)
        {
            long gtin = long.Parse(id.ToString());
            return Items.FirstOrDefault(i => long.Parse(i.ItemId) == gtin);
        }
    }
}
