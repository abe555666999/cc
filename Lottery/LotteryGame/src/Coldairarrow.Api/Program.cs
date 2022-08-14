using Coldairarrow.Util;
using Colder.Logging.Serilog;
using EFCore.Sharding;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;


namespace Coldairarrow.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Host.CreateDefaultBuilder(args)
                .ConfigureLoggingDefaults()
                .UseIdHelper()
                .UseCache()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddFxServices();
                    services.AddAutoMapper();
                    services.AddEFCoreSharding(config =>
                    {
                        config.SetEntityAssemblies(GlobalAssemblies.AllAssemblies);

                        var dbOptions = hostContext.Configuration.GetSection("Database:BaseDb").Get<DatabaseOptions>();

                        config.UseDatabase(dbOptions.ConnectionString, dbOptions.DatabaseType);
                    });

                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .Build()
                .Run();
        }

        //public static async Task Main(string[] args)
        //{
        //    IHost host = CreateHostBuilder(args).Build();   
        //    await host.RunAsync();
        //}

        //public static IHostBuilder CreateHostBuilder(string[] args)
        //{
        //    return Host.CreateDefaultBuilder(args).ConfigureLoggingDefaults().UseIdHelper()
        //        .UseCache()
        //        .ConfigureServices(delegate (HostBuilderContext hostContext, IServiceCollection services)
        //        {
        //            services.AddFxServices();
        //            services.AddAutoMapper();
        //            services.AddEFCoreSharding(delegate (IShardingBuilder config)
        //            {
        //                config.SetEntityAssemblies(GlobalAssemblies.AllAssemblies);
        //                DatabaseOptions databaseOptions = hostContext.Configuration.GetSection("Database:BaseDb").Get<DatabaseOptions>();
        //                config.UseDatabase(databaseOptions.ConnectionString, databaseOptions.DatabaseType);
        //            });
        //        })
        //        .ConfigureWebHostDefaults(delegate (IWebHostBuilder webBuilder)
        //        {
        //            webBuilder.UseKestrel(delegate (KestrelServerOptions options)
        //            {
        //                options.Limits.MaxRequestBodySize = 2147483647L;
        //            });
        //            //webBuilder.UseSetting(
        //            //WebHostDefaults.PreventHostingStartupKey, "true");
        //            webBuilder.UseStartup<Startup>();
        //        });
        //}

    }
}
