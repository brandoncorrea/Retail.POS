using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Retail.POS.Common.Logging;
using Retail.POS.Common.Interfaces;
using System;
using System.IO;
using System.Windows.Forms;
using Retail.POS.DL.Repositories;
using Retail.POS.BL;
using Retail.POS.Common.Models;

namespace Retail.POS
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(
                ConfigureServices().
                BuildServiceProvider().
                GetRequiredService<POSUI>());
        }

        private static IServiceCollection ConfigureServices()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .Build();

            return new ServiceCollection()
                .AddSingleton<POSUI>()
                .AddSingleton<IConfiguration>(configuration)
                .AddScoped<ILogger, PosLogger>()
                .AddScoped<IItemRepository, ItemRepository>()
                .AddScoped<IScale, Scale>()
                .AddScoped<ITransaction, Transaction>()
                .AddScoped<ITransactionRepository, TransactionRepository>()
                .AddScoped<IPaymentProcessor, PaymentProcessor>()
                ;
        }
    }
}
