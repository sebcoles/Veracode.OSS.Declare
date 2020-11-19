using CommandLine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Veracode.OSS.Declare.Configuration;
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
        private static IDscLogic _dscLogic;

        static void Main(string[] args)
        {
            IConfiguration Configuration = new ConfigurationBuilder()
.SetBasePath(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location))
#if DEBUG
                .AddJsonFile($"appsettings.Development.json", false)
#else
                .AddJsonFile("appsettings.json", false)
#endif
                .Build();
            var serviceCollection = new ServiceCollection();
            var profileName = Configuration.GetValue<string>("VeracodeProfileName");
            var veracodeCredFilePath = Configuration.GetValue<string>("VeracodeFileLocation");
            var useEnvironmentVariables = Configuration.GetValue<bool>("UseEnvironmentVariables");
            if(useEnvironmentVariables || String.IsNullOrWhiteSpace(veracodeCredFilePath))
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
            _dscLogic = _serviceProvider.GetService<IDscLogic>();

            Parser.Default.ParseArguments<
                TestOptions,
                ScanOptions,
                ConfigureOptions,
                EvaluateOptions,
                MitigationOptions,
                DeleteOptions>(args)
                .MapResult(
                    (TestOptions options) => Test(options),
                    (ScanOptions options) => Scan(options),
                    (ConfigureOptions options) => Configure(options),
                    (EvaluateOptions options) => Evaluate(options),
                    (MitigationOptions options) => MitigationTemplates(options),
                    (DeleteOptions options) => Delete(options),
                    errs => HandleParseError(errs));
        }

        static int Test(TestOptions options)
        {
            _logger.LogDebug($"Entering {LoggingHelper.GetMyMethodName()} with scan options {options}");
            var _localizationRepository = LoggingHelper.GetLocalizationRepository(options.Language);
            _logger.LogInformation(_localizationRepository.GetText("INFO00001", options.ToString()));
            var declareConfigRepository = new DeclareConfigurationRepository(options.JsonFileLocation);
            foreach (var app in declareConfigRepository.Apps())
            {
                _logger.LogInformation(_localizationRepository.GetText("INFO00002", app.application_name));
                bool doesScanConfirm;
                if (options.LastScan)
                {
                    _logger.LogInformation(_localizationRepository.GetText("INFO00003"));
                    doesScanConfirm = _dscLogic.ConformToPreviousScan(app, app.modules.ToArray());
                }
                else
                {
                    _logger.LogInformation(_localizationRepository.GetText("INFO00004"));
                    doesScanConfirm = _dscLogic.ConformConfiguration(app,
                            app.upload.ToArray(),
                            app.modules.ToArray(), true);
                }

                if(doesScanConfirm)
                    _logger.LogInformation(_localizationRepository.GetText("INFO00005", app.application_name));
                else
                    _logger.LogWarning(_localizationRepository.GetText("WARN00001", app.application_name));
            }

            _logger.LogInformation(_localizationRepository.GetText("INFO00006"));
            _logger.LogDebug($"Exiting {LoggingHelper.GetMyMethodName()} with value {1}");
            return 1;
        }

        static int Scan(ScanOptions options)
        {
            _logger.LogDebug($"Entering {LoggingHelper.GetMyMethodName()} with scan options {options}");
            var _localizationRepository = LoggingHelper.GetLocalizationRepository(options.Language);
            _logger.LogInformation(_localizationRepository.GetText("INFO00001", options.ToString()));

            var declareConfigRepository = new DeclareConfigurationRepository(options.JsonFileLocation);

            foreach (var app in declareConfigRepository.Apps())
            {
                bool scheduled = false;

                if (String.IsNullOrWhiteSpace(app.policy_schedule))
                    _logger.LogWarning(_localizationRepository.GetText("WARN00002", app.application_name));
                else
                    scheduled = _dscLogic.IsScanDueFromSchedule(app);

                if(scheduled || options.IgnoreSchedule)
                {
                    _logger.LogWarning(_localizationRepository.GetText("INFO00007", app.application_name));
                    _logger.LogWarning(_localizationRepository.GetText("INFO00008", app.application_name));
                    foreach (var file in app.upload.Select(x => x.location))
                        _logger.LogInformation($"{file}");

                    _logger.LogWarning(_localizationRepository.GetText("INFO00009", app.application_name));
                    foreach (var module in app.modules.Select(x => $"module_name={x.module_name},entry_point={x.entry_point}"))
                        _logger.LogInformation($"{module}");

                    _dscLogic.MakeItSoScan(app, app.upload.ToArray(), app.modules.ToArray());
                }
            }

            _logger.LogDebug($"Exiting {LoggingHelper.GetMyMethodName()} with value {1}");
            return 1;
        }

        static int Evaluate(EvaluateOptions options)
        {
            _logger.LogDebug("Evaluating the applications in the configuration file.");
            var _localizationRepository = LoggingHelper.GetLocalizationRepository(options.Language);
            _logger.LogInformation(_localizationRepository.GetText("INFO00001", options.ToString()));

            var declareConfigRepository = new DeclareConfigurationRepository(options.JsonFileLocation);

            foreach (var app in declareConfigRepository.Apps())
            {
                _logger.LogInformation($"Starting evaluation for {app.application_name}");
                _dscLogic.GetLatestStatus(app);

                if (String.IsNullOrWhiteSpace(app.policy_schedule))                
                    _logger.LogWarning($"There is no policy schedule configured for {app.application_name}");
                else                
                    _dscLogic.IsScanDueFromSchedule(app);

                _logger.LogInformation($"Evaluation complete for {app.application_name}");
            }

            _logger.LogDebug($"Exiting {LoggingHelper.GetMyMethodName()} with value {1}");
            return 1;
        }

        static int Configure(ConfigureOptions options)
        {
            _logger.LogDebug($"Entering {LoggingHelper.GetMyMethodName()} with scan options {options}");
            var _localizationRepository = LoggingHelper.GetLocalizationRepository(options.Language);
            _logger.LogInformation(_localizationRepository.GetText("INFO00001", options.ToString()));

            var declareConfigRepository = new DeclareConfigurationRepository(options.JsonFileLocation);
            foreach (var app in declareConfigRepository.Apps())
            {
                _logger.LogInformation($"Starting build for {app.application_name}");
                if (!_dscLogic.MakeItSoApp(app))
                    return 0;

                _dscLogic.MakeItSoPolicy(app, app.policy);
                _dscLogic.MakeItSoTeam(app);
                foreach (var user in app.users)
                {
                    user.teams = app.application_name;
                    _dscLogic.MakeItSoUser(user, app);
                }
                _dscLogic.MakeItSoMitigations(app);
                _dscLogic.MakeItSoSandboxes(app);
                _logger.LogInformation($"build complete for {app.application_name}");
            }

            _logger.LogDebug($"Exiting {LoggingHelper.GetMyMethodName()} with value {1}");
            return 1;
        }

        static int MitigationTemplates(MitigationOptions options)
        {
            _logger.LogDebug($"Entering {LoggingHelper.GetMyMethodName()} with scan options {options}");
            var _localizationRepository = LoggingHelper.GetLocalizationRepository(options.Language);
            _logger.LogInformation(_localizationRepository.GetText("INFO00001", options.ToString()));

            var declareConfigRepository = new DeclareConfigurationRepository(options.JsonFileLocation);

            foreach (var app in declareConfigRepository.Apps())
            {
                _logger.LogInformation($"Generating mitigations templates for {app.application_name}");
                _dscLogic.MakeMitigationTemplates(app, options.PolicyOnly);
                _logger.LogInformation($"Generated mitigations templates for {app.application_name}");
            }

            _logger.LogDebug($"Exiting {LoggingHelper.GetMyMethodName()} with value {1}");
            return 1;
        }

        static int Delete(DeleteOptions options)
        {
            _logger.LogDebug($"Entering {LoggingHelper.GetMyMethodName()} with scan options {options}");
            var _localizationRepository = LoggingHelper.GetLocalizationRepository(options.Language);
            _logger.LogInformation(_localizationRepository.GetText("INFO00001", options.ToString()));

            var declareConfigRepository = new DeclareConfigurationRepository(options.JsonFileLocation);
            foreach (var app in declareConfigRepository.Apps())
            {
                _logger.LogInformation($"Tearing down {app.application_name}");
                _dscLogic.TearDownProfile(app);
                _logger.LogInformation($"Tear down complete for {app.application_name}");
            }

            _logger.LogDebug($"Exiting {LoggingHelper.GetMyMethodName()} with value {1}");
            return 1;
        }

        static int HandleParseError(IEnumerable<Error> errs)
        {
            _logger.LogDebug($"Entering {LoggingHelper.GetMyMethodName()}");

            foreach(var error in errs)
                _logger.LogCritical($"{error}");

            _logger.LogDebug($"Exiting {LoggingHelper.GetMyMethodName()} with value {1}");
            return 1;
        }
    }
}
