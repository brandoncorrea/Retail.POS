using NUnit.Framework;
using Retail.POS.BL;
using Retail.POS.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Retail.POS.Tests.TransactionRepositoryTests
{
    public class TransactionRepositoryShould
    {
        private ITransactionRepository Repository { get; set; }

        [SetUp]
        public void SetUp()
        {
            Repository = new TransactionRepository();
        }

        [Test]
        public void SaveTransactions()
        {
        }
    }
}
