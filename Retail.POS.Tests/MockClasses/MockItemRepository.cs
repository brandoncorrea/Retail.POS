using Newtonsoft.Json;
using Retail.POS.Common.Interfaces;
using System;
using System.IO;
using System.Linq;

namespace Retail.POS.Tests.MockClasses
{
    public class MockItemRepository : IItemRepository
    {
        public IItem[] Items { get; private set; }

        public MockItemRepository()
        {
            var filePath = TestManager.Config.GetSection("MockSettings:ItemStoreLocation").Value;
            var jsonStr = File.ReadAllText(filePath);
            Items = JsonConvert.DeserializeObject<IItem[]>(jsonStr);
        }

        public IItem Get(object id)
        {
            long gtin = long.Parse(id.ToString());
            return Items.FirstOrDefault(i => long.Parse(i.ItemId) == gtin);
        }
    }
}
