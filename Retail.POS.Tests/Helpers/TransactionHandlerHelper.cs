using Retail.POS.Common.Models.LineItems;
using System;

namespace Retail.POS.Tests.Helpers
{
    public static class TransactionHandlerHelper
    {
        public static double GetTaxAmount(IItem item, double weight)
        {
            var tax1Rate = double.Parse(TestManager.Config.GetSection("Taxes:Rate1").Value);
            var tax2Rate = double.Parse(TestManager.Config.GetSection("Taxes:Rate2").Value);
            var tax3Rate = double.Parse(TestManager.Config.GetSection("Taxes:Rate3").Value);
            var tax4Rate = double.Parse(TestManager.Config.GetSection("Taxes:Rate4").Value);

            double taxAmount = 0;
            double unitPrice = Math.Round(item.SellPrice / item.SellMultiple, 2);

            if (item.Tax1)
                taxAmount += Math.Round(unitPrice * tax1Rate * weight, 2);
            if (item.Tax2)
                taxAmount += Math.Round(unitPrice * tax2Rate * weight, 2);
            if (item.Tax3)
                taxAmount += Math.Round(unitPrice * tax3Rate * weight, 2);
            if (item.Tax4)
                taxAmount += Math.Round(unitPrice * tax4Rate * weight, 2);

            return taxAmount / 100;
        }

        public static double GetTaxAmount(IItem item, int quantity = 1)
        {
            var tax1Rate = double.Parse(TestManager.Config.GetSection("Taxes:Rate1").Value);
            var tax2Rate = double.Parse(TestManager.Config.GetSection("Taxes:Rate2").Value);
            var tax3Rate = double.Parse(TestManager.Config.GetSection("Taxes:Rate3").Value);
            var tax4Rate = double.Parse(TestManager.Config.GetSection("Taxes:Rate4").Value);

            double taxAmount = 0;
            double unitPrice = Math.Round(item.SellPrice / item.SellMultiple, 2);

            if (item.Tax1)
                taxAmount += Math.Round(unitPrice * tax1Rate, 2);
            if (item.Tax2)
                taxAmount += Math.Round(unitPrice * tax2Rate, 2);
            if (item.Tax3)
                taxAmount += Math.Round(unitPrice * tax3Rate, 2);
            if (item.Tax4)
                taxAmount += Math.Round(unitPrice * tax4Rate, 2);

            return (taxAmount / 100) * quantity;
        }
    }
}
