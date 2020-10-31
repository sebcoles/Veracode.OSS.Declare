using CommandLine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using Veracode.OSS.Declare.DataAccess.Json;
using Veracode.OSS.Declare.Logic;
using Veracode.OSS.Declare.Options;
using Veracode.OSS.Wrapper;

namespace Veracode.OSS.Declare
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
                ConfigureOptions,
                EvaluateOptions,
                MitigationOptions>(args)
                .MapResult(
                    (TestOptions options) => Test(options),
                    (ScanOptions options) => Scan(options),
                    (ConfigureOptions options) => Configure(options),
                    (EvaluateOptions options) => Evaluate(options),
                    (MitigationOptions options) => Template(options),
                    errs => HandleParseError(errs));
        }

        static int Test(TestOptions options)
        {
            var jsonRepository = new JsonRepository(options.JsonFileLocation);
            var dscLogic = _serviceProvider.GetService<IDscLogic>();
            var results = new List<KeyValuePair<string, bool>>();
            foreach (var app in jsonRepository.Apps())
            {
                bool doesScanConfirm;
                if (options.LastScan)
                    doesScanConfirm = dscLogic.ConformToPreviousScan(app, app.modules.ToArray());
                else
                    doesScanConfirm = dscLogic.ConformConfiguration(app,
                        app.files.ToArray(),
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
                dscLogic.MakeItSoScan(app, app.files.ToArray(), app.modules.ToArray());

            return 1;
        }

        static int Evaluate(EvaluateOptions options)
        {
            var jsonRepository = new JsonRepository(options.JsonFileLocation);
            var dscLogic = _serviceProvider.GetService<IDscLogic>();

            foreach (var app in jsonRepository.Apps())
                dscLogic.GetLatestStatus(app);

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
                dscLogic.MakeItSoMitigations(app);
            }
            return 1;
        }

        static int Template(MitigationOptions options)
        {
            var jsonRepository = new JsonRepository(options.JsonFileLocation);
            var dscLogic = _serviceProvider.GetService<IDscLogic>();

            foreach (var app in jsonRepository.Apps())
                dscLogic.MakeMitigationTemplates(app, options.PolicyOnly);

            return 1;
        }

        static int HandleParseError(IEnumerable<Error> errs)
        {
            return 1;
        }
    }
}
