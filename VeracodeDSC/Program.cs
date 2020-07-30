using CommandLine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using VeracodeDSC.DataAccess.Json;
using VeracodeDSC.Options;
using VeracodeDSC.Shared;
using VeracodeService;

namespace VeracodeDSC
{
    class Program
    {
        private static IServiceProvider _serviceProvider;

        static void Main(string[] args)
        {
                        IConfiguration Configuration = new ConfigurationBuilder()
   .SetBasePath(Directory.GetCurrentDirectory())
#if DEBUG
                .AddJsonFile($"appsettings.Development.json", false)
#else
                .AddJsonFile("appsettings.json", false)
#endif
                .Build();
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddTransient(options => Microsoft.Extensions.Options.Options.Create(
                VeracodeFileHelper.GetConfiguration(
                    Configuration.GetValue<string>("VeracodeFileLocation"))));
            serviceCollection.AddScoped<IVeracodeRepository, VeracodeRepository>();
            _serviceProvider = serviceCollection.BuildServiceProvider();

            Parser.Default.ParseArguments<
                TestOptions, 
                ScanOptions, 
                ConfigureOptions>(args)
                .MapResult(
                    (TestOptions options) => Test(options),
                    (ScanOptions options) => Scan(options),
                    (ConfigureOptions options) => Configure(options),
                    errs => HandleParseError(errs));
        }

        static int Test(TestOptions options)
        {
            var jsonRepository = new JsonRepository(options.JsonFileLocation);
            return 1;
        }

        static int Scan(ScanOptions options)
        {
            var jsonRepository = new JsonRepository(options.JsonFileLocation);
            return 1;
        }

        static int Configure(ConfigureOptions options)
        {
            var jsonRepository = new JsonRepository(options.JsonFileLocation);
            return 1;
        }

        static int HandleParseError(IEnumerable<Error> errs)
        {
            return 1;
        }
    }
}
