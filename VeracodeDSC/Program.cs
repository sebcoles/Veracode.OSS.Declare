using CommandLine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using VeracodeDSC.DataAccess.Json;
using VeracodeDSC.Logic;
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
            serviceCollection.AddScoped<IVeracodeService, VeracodeService>();
            serviceCollection.AddScoped<IDscLogic, DscLogic>();
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
            var dscLogic = _serviceProvider.GetService<IDscLogic>();
            var results = new List<KeyValuePair<string, bool>>();
            foreach (var app in jsonRepository.Apps())
            {
                var doesScanConfirm = dscLogic.ConformConfiguration(app,
                        app.binaries.ToArray(),
                        app.modules.ToArray(), true);

                results.Add(new KeyValuePair<string, bool>(
                    app.application_name,
                    doesScanConfirm
                ));
            }

            foreach (var summary in results)
            {
                var message = summary.Value ? "DOES" : "DOES NOT";
                Console.Write($"Application {summary.Key} scan config {message} conforms.");
            }
            return 1;
        }

        static int Scan(ScanOptions options)
        {
            var jsonRepository = new JsonRepository(options.JsonFileLocation);
            var dscLogic = _serviceProvider.GetService<IDscLogic>();

            foreach (var app in jsonRepository.Apps())
                dscLogic.MakeItSoScan(app, app.binaries.ToArray(), app.modules.ToArray());
            
            return 1;
        }

        static int Configure(ConfigureOptions options)
        {
            var jsonRepository = new JsonRepository(options.JsonFileLocation);
            var dscLogic = _serviceProvider.GetService<IDscLogic>();
            foreach (var app in jsonRepository.Apps())
            {
                dscLogic.MakeItSoApp(app);
                dscLogic.MakeItSoPolicy(app, app.policy);
                dscLogic.MakeItSoTeam(app);
                foreach (var user in app.users)
                {
                    user.teams = app.application_name;
                    dscLogic.MakeItSoUser(user, app);
                }
            }
            return 1;
        }

        static int HandleParseError(IEnumerable<Error> errs)
        {
            return 1;
        }
    }
}
