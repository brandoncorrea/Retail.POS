using System;

namespace Retail.POS.Common.Scale
{
    public class PosScale : IScale
    {
        private readonly Random Rand = new Random();
        public decimal GetWeight() =>
            (decimal)Rand.NextDouble() * 100;
    }
}
