using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Veracode.OSS.Declare.Shared;
using Veracode.OSS.Wrapper;
using Veracode.OSS.Wrapper.Models;
using Veracode.OSS.Wrapper.Rest;

namespace Veracode.OSS.Declare.Logic
{
    public interface IDscLogic
    {
        void MakeItSoSandboxes(ApplicationProfile app);
        bool MakeItSoApp(ApplicationProfile app);
        void MakeItSoPolicy(ApplicationProfile app, Policy policy);
        void MakeItSoUser(User user, ApplicationProfile app);
        void MakeItSoTeam(ApplicationProfile app);
        bool ConformConfiguration(ApplicationProfile app, File[] files, Module[] configModules, bool isTest);
        void MakeItSoScan(ApplicationProfile app, File[] files, Module[] configModules);
        void MakeItSoMitigations(ApplicationProfile app);
        void GetLatestStatus(ApplicationProfile app);
        void MakeMitigationTemplates(ApplicationProfile app, bool policy_only);
        bool ConformToPreviousScan(ApplicationProfile app, Module[] configModules);
        bool IsScanDueFromSchedule(ApplicationProfile app);
        void TearDownProfile(ApplicationProfile app);
    }
    public class DscLogic : IDscLogic
    {
        private IVeracodeService _veracodeService;
        private IVeracodeRepository _veracodeRepository;
        private ILogger _logger;

        public DscLogic(ILogger<DscLogic> logger, IVeracodeService veracodeService, IVeracodeRepository veracodeRepository)
        {
            _logger = logger;
            _veracodeService = veracodeService;
            _veracodeRepository = veracodeRepository;
        }

        public void MakeItSoMitigations(ApplicationProfile app)
        {
            app.id = $"{_veracodeRepository.GetAllApps().SingleOrDefault(x => x.app_name == app.application_name).app_id}";
            var latest_build = _veracodeRepository.GetLatestScan(app.id);
            if (latest_build == null)
            {
                _logger.LogInformation($"{app.application_name} has no completed scans, cannot apply mitigations.");
                return;
            }

            var latest_build_id = $"{latest_build.build.build_id}";

            if (app.mitigations.Any())
            {
                _logger.LogInformation($"Checking if the mitigations for Application Profile {app.application_name} have already been applied.");
                var flaw_ids = app.mitigations.Select(x => x.flaw_id).ToArray();

                foreach (var flaw_id in flaw_ids)
                {
                    if (!int.TryParse(flaw_id, out int result))
                    {
                        _logger.LogWarning($"The flaw_id of {flaw_id} is invalid, skipping.");
                        continue;
                    }

                    var mitigations = _veracodeRepository.GetMitigationForFlaw(latest_build_id, flaw_id);
                    var config_mitigation = app.mitigations.SingleOrDefault(x => x.flaw_id == flaw_id);
                    if (mitigations.Any()
                        && mitigations[0].mitigation_action.Any()
                        && mitigations[0].mitigation_action.Any(x => x.comment == config_mitigation.action))
                    {
                        _logger.LogWarning($"Mitigation for Flaw ID {flaw_id} has already been applied for Application Profile {app.application_name}.");
                    }
                    else
                    {
                        _logger.LogInformation($"Applying mitigation for Flaw ID {flaw_id} for Application Profile {app.application_name}.");
                        _veracodeRepository.UpdateMitigations(latest_build_id, config_mitigation.action, config_mitigation.tsrv, flaw_id);
                        _veracodeRepository.UpdateMitigations(latest_build_id, "accepted", "Mitigation applied via DSC", flaw_id);
                    }
                }
            }
        }
        public bool MakeItSoApp(ApplicationProfile app)
        {
            _logger.LogInformation($"Checking to see if Application Profile {app.application_name} already exists.");
            if (!_veracodeService.DoesAppExist(app))
            {
                _logger.LogInformation($"Application Profile {app.application_name} does not exist, adding configuration.");
                try
                {
                    _veracodeService.CreateApp(app);
                    _logger.LogInformation($"Application Profile {app.application_name} created succesfully.");
                }
                catch (Exception e)
                {
                    _logger.LogError($"Application Profile {app.application_name} could not be created!");
                    _logger.LogCritical($"{e.Message}.");
                    return false;
                }
                return true;
            }

            _logger.LogInformation($"Application Profile {app.application_name} exists.");
            if (_veracodeService.HasAppChanged(app))
            {
                _logger.LogInformation($"Application Profile {app.application_name} has changes, updating configuration.");
                try
                {
                    _veracodeService.UpdateApp(app);
                    _logger.LogInformation($"Application Profile {app.application_name} updated succesfully.");
                }
                catch (Exception e)
                {
                    _logger.LogError($"Application Profile {app.application_name} could not be updated!");
                    _logger.LogError($"{e.Message}.");
                    return false;
                }
                return true;
            }

            _logger.LogInformation($"Application Profile {app.application_name} has no changes.");
            return true;
        }

        public void MakeItSoPolicy(ApplicationProfile app, Policy policy)
        {
            _logger.LogInformation($"Checking to see if policy for {app.application_name} already exists.");
            if (!_veracodeService.DoesPolicyExist(app))
            {
                _logger.LogInformation($"Policy for {app.application_name} does not exist, adding configuration.");
                try
                {
                    _veracodeService.CreatePolicy(app, policy);
                    _logger.LogInformation($"Policy for {app.application_name} created succesfully.");
                }
                catch (Exception e)
                {
                    _logger.LogError($"Policy for {app.application_name} could not be created!");
                    _logger.LogError($"{e.Message}.");
                }
                return;
            }

            _logger.LogInformation($"Policy for {app.application_name} exists.");
            if (_veracodeService.HasPolicyChanged(app, policy))
            {
                _logger.LogInformation($"Policy for {app.application_name} has changed, updating configuration.");
                try
                {
                    _veracodeService.UpdatePolicy(app, policy);
                    _logger.LogInformation($"Policy for {app.application_name} updated succesfully.");
                }
                catch (Exception e)
                {
                    _logger.LogError($"Policy for {app.application_name} could not be updated!");
                    _logger.LogError($"{e.Message}.");
                }
                return;
            }

            _logger.LogInformation($"Policy for {app.application_name} has no changes.");
        }

        public void MakeItSoUser(User user, ApplicationProfile app)
        {
            _logger.LogInformation($"Checking to see if user {user.email_address} already exists.");
            if (!_veracodeService.DoesUserExist(user))
            {
                _logger.LogInformation($"User {user.email_address} does not exist, adding configuration.");
                try
                {
                    _veracodeService.CreateUser(user, app);
                    _logger.LogInformation($"User {user.email_address} created succesfully.");
                }
                catch (Exception e)
                {
                    _logger.LogError($"User {user.email_address} could not be created!");
                    _logger.LogError($"{e.Message}.");
                }
                return;
            }

            _logger.LogInformation($"User {user.email_address} exists.");
            if (_veracodeService.HasUserChanged(user))
            {
                _logger.LogInformation($"User {user.email_address} has changed, updating configuration.");
                try
                {
                    _veracodeService.UpdateUser(user);
                    _logger.LogInformation($"User {user.email_address} updated succesfully.");
                }
                catch (Exception e)
                {
                    _logger.LogError($"User {user.email_address} could not be updated!");
                    _logger.LogError($"{e.Message}.");
                }
                return;
            }

            _logger.LogInformation($"User {user.email_address} has no changes.");
        }

        public void MakeItSoTeam(ApplicationProfile app)
        {
            _logger.LogInformation($"Checking to see if team {app.application_name} already exists.");
            if (!_veracodeService.DoesTeamExistForApp(app))
            {
                _logger.LogInformation($"Team {app.application_name} does not exist, adding configuration.");
                try
                {
                    _veracodeService.CreateTeamForApp(app);
                    _logger.LogInformation($"Team {app.application_name} created succesfully.");
                }
                catch (Exception e)
                {
                    _logger.LogError($"Team {app.application_name} could not be created!");
                    _logger.LogError($"{e.Message}.");
                }
                return;
            }

            _logger.LogInformation($"Team {app.application_name} exists.");
            var usersInTeam = _veracodeService.GetUserEmailsOnTeam(app);
            foreach (var user in usersInTeam)
            {
                _logger.LogInformation($"Checking if {user.email_address} is assigned to team {app.application_name}.");
                if (!_veracodeService.IsUserAssignedToTeam(user, app))
                {
                    _logger.LogInformation($"User {user.email_address} is not assigned to team {app.application_name}, updating configuration.");
                    try
                    {
                        if (string.IsNullOrEmpty(user.teams))
                            user.teams = $"{app.application_name}";
                        else
                            user.teams = $",{app.application_name}";

                        _veracodeService.UpdateUser(user);
                        _logger.LogInformation($"User {user.email_address} assigned to team {app.application_name} succesfully.");
                    }
                    catch (Exception e)
                    {
                        _logger.LogError($"User {user.email_address} could not be added to team {app.application_name}!");
                        _logger.LogError($"{e.Message}.");
                    }
                }
                else
                {
                    _logger.LogInformation($"User {user.email_address} is already assigned to team {app.application_name}.");
                }
            }
        }

        public bool ConformConfiguration(ApplicationProfile app, File[] files, Module[] configModules, bool isTest)
        {
            try
            {
                if (!_veracodeService.DoesAppExist(app))
                    throw new Exception($"Application Profile {app.application_name} does not exist, uou need to run -configure first.");

                var app_id = _veracodeRepository.GetAllApps().SingleOrDefault(x => x.app_name == app.application_name).app_id;
                app.id = $"{app_id}";

                if (!_veracodeService.IsPolicyScanInProgress(app))
                {
                    var scan_id = _veracodeService.CreateScan(app);
                    _logger.LogInformation($"New scan created with Build Id {scan_id}. Uploading files");
                    UploadFiles(app, scan_id, files);
                    RunScan(app, scan_id, "", _veracodeService.StartPreScan, 
                        BuildStatusType.PreScanSubmitted, BuildStatusType.PreScanFailed);

                    var prescanModules = _veracodeService.GetModules(app.id, scan_id);
                    var doesScanConform = DoesModuleConfigConform(scan_id, configModules, prescanModules);

                    if (isTest)
                        _logger.LogInformation($"Test Finished. Deleting Build Id {scan_id}.");

                    if (doesScanConform)
                        _logger.LogInformation($"Configuration conforms.");
                    else
                        _logger.LogInformation($"Scan does not conform. Deleting Build Id {scan_id}.");


                    if (isTest || !doesScanConform)
                        _veracodeService.DeleteScan(app.id);

                    return doesScanConform;
                }
                else
                {
                    _logger.LogWarning($"Policy scan for {app.application_name} already in progress.");
                    _logger.LogWarning($"This must be cancelled or completed before this job can be continued.");
                    return false;
                }
            }
            catch (Exception e)
            {
                _logger.LogInformation($"{e.Message}.");
                if(!e.Message.Contains("Profile"))
                    _veracodeService.DeleteScan(app.id);

                return false;
            }
        }

        public void MakeItSoScan(ApplicationProfile app, File[] files, Module[] configModules)
        {
            try
            {
                var app_id = _veracodeRepository.GetAllApps().SingleOrDefault(x => x.app_name == app.application_name).app_id;

                if (!ConformConfiguration(app, files, configModules, false))
                {
                    _logger.LogWarning("Config does not conform, cancelling scan.");
                    return;
                }

                var scan_id = _veracodeRepository.GetLatestScan($"{app_id}").build_id;
                var entry_points = configModules
                    .Where(x => x.entry_point)
                    .Select(y => y.module_name)
                    .ToArray();

                var modulesToScan = _veracodeService.GetModules(app.id, $"{scan_id}")
                    .Where(x => entry_points.Contains(x.module_name))
                    .Select(y => y.module_id)
                    .ToArray();

                var moduleList = string.Join(",", modulesToScan);

                _logger.LogInformation("Starting scan.");
                RunScan(app, $"{scan_id}", moduleList, _veracodeService.StartScan, 
                    BuildStatusType.ScanInProcess, BuildStatusType.ScanErrors);

                _logger.LogInformation($"Deployment complete.");
            }
            catch (Exception e)
            {
                _logger.LogError($"{e.Message}.");
            }
        }

        public void UploadFiles(ApplicationProfile app, string scan_id, File[] files)
        {
            var tasks = new Task[files.Length];
            for (var i = 0; i < tasks.Length; i++)
            {
                var binary = files[i];
                tasks[i] = new Task(() => UploadTask(binary, app.id, scan_id));
            }

            foreach (var task in tasks)
                task.Start();

            Task.WaitAll(tasks);
        }

        private void UploadTask(File binary, string app_id, string scan_id)
        {
            _logger.LogInformation($"Uploading {binary.location} to scan {scan_id}.");
            _veracodeService.AddFileToScan(app_id, binary.location);
            _logger.LogInformation($"Upload of {binary.location} complete.");
        }
        public void RunScan(ApplicationProfile app, string scan_id, 
            string modules, Action<string, string> ScanMethod, BuildStatusType running, BuildStatusType failure)
        {
            var stopWatch = new Stopwatch();
            TimeSpan ts;
            string elapsedTime;
            stopWatch.Start();

            ScanMethod(app.id, modules);
            var scanStatus = running;
            while (scanStatus == running)
            {
                ts = stopWatch.Elapsed;
                elapsedTime = string.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                    ts.Hours, ts.Minutes, ts.Seconds,
                    ts.Milliseconds / 10);
                _logger.LogInformation($"Scan {scan_id} is still running and has been running for {elapsedTime}.");
                Thread.Sleep(20000);
                scanStatus = _veracodeService.GetScanStatus(app.id, $"{scan_id}");
            }

            ts = stopWatch.Elapsed;
            elapsedTime = string.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            stopWatch.Stop();

            if (scanStatus == failure)
                throw new Exception("Scan status returned an error status.");

            _logger.LogInformation($"Scan complete for {scan_id} and took {elapsedTime}.");
        }

        public bool DoesModuleConfigConform(string newScan, Module[] configModules, Module[] prescanModules)
        {
            var missingFromConfig = prescanModules
                .Where(x => configModules
                .All(y => y.module_name != x.module_name)).ToArray();

            var missingFromPrescan = configModules
                .Where(x => prescanModules
                .All(y => y.module_name != x.module_name)).ToArray();

            if (missingFromConfig.Count() > 0)
            {
                _logger.LogWarning($"There are {missingFromConfig.Count()} modules from prescan that do not match the config.");

                _logger.LogWarning($"Please include and complete the below configuration and add to your .json file");
                var messages = new List<string>();
                foreach (var mod in missingFromConfig)
                {
                    var module_selection = mod.can_be_entry_point ? ",\"entry_point\":\"true\"" : "";
                    messages.Add($"{{ " +
                        $"\"module_name\":\"{mod.module_name}\" " +
                        $"{module_selection}" +
                        $"}}");
                }
                _logger.LogInformation("\"modules\":[\n" + string.Join(",\n", messages) + "\n]");
            }

            if (missingFromPrescan.Count() > 0)
            {
                _logger.LogWarning($"There are {missingFromPrescan.Count()} modules that are configured but are not in the prescan results.");
                _logger.LogWarning($"Thes modules need removed or resolved before a scan can continue.");
                foreach (var mod in missingFromPrescan)
                    _logger.LogInformation($"{mod.module_name}");
            }

            foreach (var mod in prescanModules)
                foreach (var message in mod.messages)
                    _logger.LogWarning($"PRE SCAN ERRORS: {mod.module_name}:{message}");

            if (missingFromConfig.Count() > 0 || missingFromPrescan.Count() > 0)
            {
                _logger.LogWarning($"Module selection configuration was incorrect for {newScan}.");
                return false;
            }

            _logger.LogInformation($"Module selection conforms for {newScan} and the scan can commence.");
            return true;
        }

        public void GetLatestStatus(ApplicationProfile app)
        {
            app.id = $"{_veracodeRepository.GetAllApps().SingleOrDefault(x => x.app_name == app.application_name).app_id}";
            var sandboxes = _veracodeRepository.GetSandboxesForApp(app.id);
            var latest_policy_build = _veracodeRepository.GetLatestScan(app.id).build;

            var scanStatus = _veracodeService.GetScanStatus(app.id, $"{latest_policy_build.build_id}");
            _logger.LogInformation($"[{app.application_name}][Policy][Scan Status] {VeracodeEnumConverter.Convert(scanStatus)}");

            var compliance = VeracodeEnumConverter.Convert(latest_policy_build.policy_compliance_status);
            _logger.LogInformation($"[{app.application_name}][Policy][Compliance Status] {compliance}");

            foreach (var sandbox in sandboxes)
            {
                var latest_sandbox_build = _veracodeRepository.GetLatestScanSandbox(app.id, $"{sandbox.sandbox_id}");
                if (latest_sandbox_build == null)
                {
                    _logger.LogInformation($"[{app.application_name}][Sandbox {sandbox.sandbox_name}][Scan Status] There are no scans!");
                }
                else
                {
                    var latest_sandbox_build_id = $"{latest_sandbox_build.build.build_id}";
                    var scanSandboxStatus = _veracodeService.GetScanStatus(app.id, latest_sandbox_build_id);
                    _logger.LogInformation($"[{app.application_name}][Sandbox {sandbox.sandbox_name}][Scan Status] {VeracodeEnumConverter.Convert(scanSandboxStatus)}");

                    var sandboxCompliance = VeracodeEnumConverter.Convert(latest_sandbox_build.build.policy_compliance_status);
                    _logger.LogInformation($"[{app.application_name}][Sandbox {sandbox.sandbox_name}][Compliance Status] {VeracodeEnumConverter.Convert(latest_sandbox_build.build.policy_compliance_status)}");
                }
            }
        }

        public void MakeMitigationTemplates(ApplicationProfile app, bool policy_only)
        {
            app.id = $"{_veracodeRepository.GetAllApps().SingleOrDefault(x => x.app_name == app.application_name).app_id}";
            var latest_build = _veracodeRepository.GetLatestScan(app.id);
            if (latest_build == null)
            {
                _logger.LogInformation($"{app.application_name} has no completed scans, no mitigations to create templates for.");
                return;
            }

            var latest_build_id = $"{latest_build.build.build_id}";
            var flaws = _veracodeRepository.GetFlaws(latest_build_id);
            var messages = new List<string>();

            if (policy_only)
                flaws = flaws.Where(x => x.affects_policy_compliance).ToArray();

            foreach (var flaw in flaws)
                messages.Add($"{{ " +
                             $"\"flaw_id\":\"{flaw.issueid}\"," +
                             $"\"cwe_id\":\"{flaw.cweid}\"," +
                             $"\"file_name\":\"{flaw.sourcefile}\"," +
                             $"\"line_number\":\"{flaw.line}\"," +
                             $"\"link\":\"__ADD_A_REPOSITORY_LINK__\"," +
                             $"\"action\":\"fp || appdesign || osenv || netenv\"," +
                             $"\"technique\":\"__ENTER_TECHNIQUES__\"," +
                             $"\"specifics\":\"__ENTER_SPECIFICS__\"," +
                             $"\"remaining_risk\":\"__ENTER_REMAINING_RISK__\"," +
                             $"\"verification\":\"__ENTER_VERIFICATION__\" " +
                             $"}}");

            _logger.LogInformation("\"mitigations\":[\n" + string.Join(",\n", messages) + "\n]");
        }


        public bool ConformToPreviousScan(ApplicationProfile app, Module[] configModules)
        {
            app.id = $"{_veracodeRepository.GetAllApps().SingleOrDefault(x => x.app_name == app.application_name).app_id}";
            var latest_build = _veracodeRepository.GetLatestScan(app.id);
            var prescanModules = _veracodeService.GetModules(app.id, $"{latest_build.build.build_id}");
            return DoesModuleConfigConform($"{latest_build.build.build_id}", configModules, prescanModules);
        }

        public void MakeItSoSandboxes(ApplicationProfile app)
        {
            app.id = $"{_veracodeRepository.GetAllApps().SingleOrDefault(x => x.app_name == app.application_name).app_id}";
            _logger.LogInformation($"[{app.application_name}] Checking Sandboxes...");
            var current_sandboxes = _veracodeRepository.GetSandboxesForApp(app.id);
            var config_sandboxes = app.sandboxes;

            if (!config_sandboxes.Any())
            {
                _logger.LogInformation($"[{app.application_name}] No sandboxes in configuration. Skipping.");
                return;
            }

            foreach (var config_sandbox in config_sandboxes)
            {
                if (!current_sandboxes.Any(x => x.sandbox_name == config_sandbox.sandbox_name))
                {
                    _logger.LogInformation($"[{app.application_name}] Does not have sandbox with name {config_sandbox.sandbox_name}. Creating...");
                    _veracodeRepository.CreateSandbox(app.id, config_sandbox.sandbox_name);
                    _logger.LogInformation($"[{app.application_name}] {config_sandbox.sandbox_name} creation complete!");
                } else
                {
                    _logger.LogInformation($"[{app.application_name}] {config_sandbox.sandbox_name} already exists! Nothing to do.");
                }
            }

            _logger.LogInformation($"[{app.application_name}] Finished Sandboxes!");
        }

        public bool IsScanDueFromSchedule(ApplicationProfile app)
        { 
            app.id = $"{_veracodeRepository.GetAllApps().SingleOrDefault(x => x.app_name == app.application_name).app_id}";
            var lastScan = _veracodeRepository.GetLatestScan(app.id);

            if(lastScan == null)
            {
                _logger.LogWarning($"[{app.application_name}] Has not had a scan yet, the first scan is due.");
                return true;
            }

            if (lastScan.build.analysis_unit[0].status != BuildStatusType.ResultsReady)            
                _logger.LogWarning($"[{app.application_name}] Currently has a scan in progress in the status of [{lastScan.build.analysis_unit[0].status}]");
            

            if (CronHelper.IsANewScanDue(app.policy_schedule, lastScan.build.analysis_unit[0].published_date))
            {
                var dueDays = CronHelper.HowManyDaysAgo(app.policy_schedule, lastScan.build.analysis_unit[0].published_date);
                _logger.LogWarning($"[{app.application_name}] last scan was {lastScan.build.analysis_unit[0].published_date.ToLongDateString()} and was due {dueDays} days ago.");
                return true;
            } else
            {
                _logger.LogInformation($"A scan is not due according to the schedule.");
                _logger.LogInformation($"Last scan completed at {lastScan.build.analysis_unit[0].published_date.ToLongDateString()}");
            }

            _logger.LogInformation($"A new scan does not need to be started.");
            return false;
        }

        public void TearDownProfile(ApplicationProfile app)
        {
            _logger.LogInformation($"Checking if profile exists.");
            if (_veracodeService.DoesAppExist(app))
            {
                app.id = $"{_veracodeRepository.GetAllApps().SingleOrDefault(x => x.app_name == app.application_name).app_id}";
                _logger.LogInformation($"Deleting profile.");
                _veracodeService.DeleteApp(app);
                _logger.LogInformation($"Profile deleted.");
            } else
            {
                _logger.LogInformation($"Profile doesn't exists, nothing to delete.");
            }

            _logger.LogInformation($"Checking if policy exists.");
            if (_veracodeService.DoesPolicyExist(app))
            {
                _logger.LogInformation($"Deleting policy.");
                _veracodeService.DeletePolicy(app);
                _logger.LogInformation($"Policy deleted.");
            }
            else
            {
                _logger.LogInformation($"Policy doesn't exists, nothing to delete.");
            }

            _logger.LogInformation($"Checking if team exists.");
            if (_veracodeService.DoesTeamExistForApp(app))
            {
                _logger.LogInformation($"Deleting team.");
                _veracodeService.DeleteTeam(app);
                _logger.LogInformation($"Team deleted.");
            }
            else
            {
                _logger.LogInformation($"Team doesn't exists, nothing to delete.");
            }

            foreach (var user in app.users)
            {
                _logger.LogInformation($"Checking if user exists.");
                if (_veracodeService.DoesUserExist(user))
                {
                    _logger.LogInformation($"Deleting user.");
                    try
                    {
                        _veracodeService.DeleteUser(user);
                        _logger.LogInformation($"User deleted.");
                    }
                    catch (XmlParseError e)
                    {
                        if (e.Message.Contains("access denied"))
                            _logger.LogError($"These API credentials do not have permission to delete user {user.email_address}");
                    }
                }
                else
                {
                    _logger.LogInformation($"User doesn't exists, nothing to delete.");
                }
            }
        }
    }
}
