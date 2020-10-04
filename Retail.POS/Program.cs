using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Retail.POS.Common.Logging;
using Retail.POS.Common.Repositories;
using Retail.POS.Common.Scale;
using Retail.POS.Common.TransactionHandler;
using System;
using System.IO;
using System.Windows.Forms;

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

            var services = new ServiceCollection();
            ConfigureServices(services);

            using ServiceProvider provider = services.BuildServiceProvider();
            var posui = provider.GetRequiredService<POSUI>();
            Application.Run(posui);
        }

        private static void ConfigureServices(ServiceCollection services)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .Build();

            services
                .AddSingleton<POSUI>()
                .AddSingleton<IConfiguration>(configuration)
                .AddScoped<ILogger, PosLogger>()
                .AddScoped<IItemRepository, ItemRepository>()
                .AddScoped<IScale, PosScale>()
                .AddScoped<ITransactionHandler, TransactionHandler>();
        }
    }
}
