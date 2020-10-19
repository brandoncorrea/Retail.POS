using Retail.POS.Common.Interfaces;
using System;

namespace Retail.POS.BL.Hardware
{
    public class Scale : IScale
    {
        private readonly Random Random = new Random();
        public double Weight => Random.NextDouble();
    }
}
