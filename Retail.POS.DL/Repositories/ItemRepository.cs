using Newtonsoft.Json;
using Retail.POS.Common.Interfaces;
using System.IO;
using System.Linq;

namespace Retail.POS.DL.Repositories
{
    public class ItemRepository : IItemRepository
    {
        private IItem[] Items { get; set; }

        public ItemRepository()
        {
            var jsonStr = File.ReadAllText("Repositories\\itemStore.json");
            Items = JsonConvert.DeserializeObject<IItem[]>(jsonStr);
        }

        public IItem Get(object id)
        {
            long gtin = long.Parse(id.ToString());
            return Items.FirstOrDefault(i => long.Parse(i.ItemId) == gtin);
        }
    }
}
