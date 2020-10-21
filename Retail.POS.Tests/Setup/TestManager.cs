using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using Retail.POS.BL;
using Retail.POS.Common.Interfaces;
using Retail.POS.Tests.MockClasses;
using System;
using System.IO;

[SetUpFixture]
public class TestManager
{
    public static IConfiguration MockConfig { get; private set; }
    public static MockPaymentProcessor MockPaymentProcessor { get; private set; }
    public static MockTransactionRepository MockTransactionRepository { get; private set; }

    [OneTimeSetUp]
    public void SetUp()
    {
        MockConfig = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("Setup\\mockConfig.json", optional: false, reloadOnChange: false)
            .Build();
        MockPaymentProcessor = new MockPaymentProcessor();
        MockTransactionRepository = new MockTransactionRepository();
    }

    [OneTimeTearDown]
    public void TearDown()
    {

    }

    public static ITransaction CreateTransaction() => new Transaction
    (
        MockConfig, 
        MockPaymentProcessor
    );

    public static ITransactionHandler CreateTransactionHandler() => new TransactionHandler
    (
        MockConfig,
        MockPaymentProcessor,
        MockTransactionRepository
    );
}
