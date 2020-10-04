using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Text;

[SetUpFixture]
public class TestManager
{
    public static IConfiguration Config { get; private set; }

    [OneTimeSetUp]
    public void SetUp()
    {
        Config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("Setup\\appsettings.json", optional: false, reloadOnChange: false)
            .Build();
    }

    [OneTimeTearDown]
    public void TearDown()
    {

    }
}
