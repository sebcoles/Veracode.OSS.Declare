using CommandLine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NLog.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Veracode.OSS.Declare.DataAccess.Json;
using Veracode.OSS.Declare.Logic;
using Veracode.OSS.Declare.Options;
using Veracode.OSS.Declare.Shared;
using Veracode.OSS.Wrapper;

namespace Veracode.OSS.Declare
{
    class Program
    {
        private static IServiceProvider _serviceProvider;
        private static ILogger _logger;

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
            var profileName = Configuration.GetValue<string>("VeracodeProfileName");
            var veracodeCredFilePath = Configuration.GetValue<string>("VeracodeFileLocation");
            if(String.IsNullOrWhiteSpace(veracodeCredFilePath))
                serviceCollection.AddTransient(options => Microsoft.Extensions.Options.Options.Create(
                    VeracodeEnvHelper.GetConfiguration()));
            else
                serviceCollection.AddTransient(options => Microsoft.Extensions.Options.Options.Create(
                    VeracodeFileHelper.GetConfiguration(veracodeCredFilePath, profileName)));

            serviceCollection.AddScoped<IVeracodeRepository, VeracodeRepository>();
            serviceCollection.AddScoped<IVeracodeService, VeracodeService>();
            serviceCollection.AddScoped<IDscLogic, DscLogic>();
            serviceCollection.AddLogging(loggingBuilder => {
                loggingBuilder.AddNLog("nlog.config");
            });

            _serviceProvider = serviceCollection.BuildServiceProvider();
            _logger = _serviceProvider.GetService<ILogger<Program>>();

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
                    (MitigationOptions options) => MitigationTemplates(options),
                    errs => HandleParseError(errs));
        }

        static int Test(TestOptions options)
        {
            _logger.LogInformation($"Entering {LoggingHelper.GetMyMethodName()} with scan options {options}");
            var jsonRepository = new JsonRepository(options.JsonFileLocation);
            var dscLogic = _serviceProvider.GetService<IDscLogic>();
            foreach (var app in jsonRepository.Apps())
            {
                _logger.LogInformation($"Testing configuration for {app.application_name}");
                bool doesScanConfirm;
                if (options.LastScan)
                {
                    _logger.LogInformation($"Testing against the lastest scan");
                    doesScanConfirm = dscLogic.ConformToPreviousScan(app, app.modules.ToArray());
                }
                else
                {
                    _logger.LogInformation($"Testing against a new scan");
                    doesScanConfirm = dscLogic.ConformConfiguration(app,
                            app.files.ToArray(),
                            app.modules.ToArray(), true);
                }

                var message = doesScanConfirm ? "DOES" : "DOES NOT";
                _logger.LogInformation($"Application {app.application_name} scan config {message} conforms.");
            }

            _logger.LogInformation($"Scan Configuration testing complete.");
            _logger.LogInformation($"Exiting {LoggingHelper.GetMyMethodName()} with value {1}");
            return 1;
        }

        static int Scan(ScanOptions options)
        {
            _logger.LogInformation($"Entering {LoggingHelper.GetMyMethodName()} with scan options {options}");

            var jsonRepository = new JsonRepository(options.JsonFileLocation);
            var dscLogic = _serviceProvider.GetService<IDscLogic>();

            foreach (var app in jsonRepository.Apps())
            {
                bool scheduled = false;

                if (String.IsNullOrWhiteSpace(app.policy_schedule))
                    _logger.LogWarning($"There is no policy schedule configured for {app.application_name}");
                else
                    scheduled = dscLogic.IsScanDueFromSchedule(app);

                if(scheduled || options.IgnoreSchedule)
                {
                    _logger.LogInformation($"Starting scan for {app.application_name}");
                    _logger.LogInformation($"Files being scanned are:");
                    foreach (var file in app.files.Select(x => x.location))
                        _logger.LogInformation($"{file}");

                    _logger.LogInformation($"Modules being scanned are:");
                    foreach (var module in app.modules.Select(x => $"module_name={x.module_name},entry_point={x.entry_point}"))
                        _logger.LogInformation($"{module}");

                    dscLogic.MakeItSoScan(app, app.files.ToArray(), app.modules.ToArray());
                }
            }

            _logger.LogInformation($"Exiting {LoggingHelper.GetMyMethodName()} with value {1}");
            return 1;
        }

        static int Evaluate(EvaluateOptions options)
        {
            _logger.LogInformation("Evaluating the applications in the configuration file.");
           
            var jsonRepository = new JsonRepository(options.JsonFileLocation);
            var dscLogic = _serviceProvider.GetService<IDscLogic>();

            foreach (var app in jsonRepository.Apps())
            {
                _logger.LogInformation($"Starting evaluation for {app.application_name}");
                dscLogic.GetLatestStatus(app);

                if (String.IsNullOrWhiteSpace(app.policy_schedule))                
                    _logger.LogWarning($"There is no policy schedule configured for {app.application_name}");
                else                
                    dscLogic.IsScanDueFromSchedule(app);

                _logger.LogInformation($"Evaluation complete for {app.application_name}");
            }

            _logger.LogInformation($"Exiting {LoggingHelper.GetMyMethodName()} with value {1}");
            return 1;
        }

        static int Configure(ConfigureOptions options)
        {
            _logger.LogInformation($"Entering {LoggingHelper.GetMyMethodName()} with scan options {options}");
            var jsonRepository = new JsonRepository(options.JsonFileLocation);
            var dscLogic = _serviceProvider.GetService<IDscLogic>();
            foreach (var app in jsonRepository.Apps())
            {
                _logger.LogInformation($"Starting build for {app.application_name}");
                if (!dscLogic.MakeItSoApp(app))
                    return 0;

                dscLogic.MakeItSoPolicy(app, app.policy);
                dscLogic.MakeItSoTeam(app);
                foreach (var user in app.users)
                {
                    user.teams = app.application_name;
                    dscLogic.MakeItSoUser(user, app);
                }
                dscLogic.MakeItSoMitigations(app);
                dscLogic.MakeItSoSandboxes(app);
                _logger.LogInformation($"build complete for {app.application_name}");
            }

            _logger.LogInformation($"Exiting {LoggingHelper.GetMyMethodName()} with value {1}");
            return 1;
        }

        static int MitigationTemplates(MitigationOptions options)
        {
            _logger.LogInformation($"Entering {LoggingHelper.GetMyMethodName()} with scan options {options}");
            var jsonRepository = new JsonRepository(options.JsonFileLocation);
            var dscLogic = _serviceProvider.GetService<IDscLogic>();

            foreach (var app in jsonRepository.Apps())
            {
                _logger.LogInformation($"Generating mitigations templates for {app.application_name}");
                dscLogic.MakeMitigationTemplates(app, options.PolicyOnly);
                _logger.LogInformation($"Generated mitigations templates for {app.application_name}");
            }

            _logger.LogInformation($"Exiting {LoggingHelper.GetMyMethodName()} with value {1}");
            return 1;
        }

        static int HandleParseError(IEnumerable<Error> errs)
        {
            _logger.LogInformation($"Entering {LoggingHelper.GetMyMethodName()}");

            foreach(var error in errs)
                _logger.LogError($"{error}");

            _logger.LogInformation($"Exiting {LoggingHelper.GetMyMethodName()} with value {1}");
            return 1;
        }
    }
}
