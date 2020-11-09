using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Veracode.OSS.Declare.Shared;
using Veracode.OSS.Wrapper;
using Veracode.OSS.Wrapper.Models;

namespace Veracode.OSS.Declare.Logic
{
    public interface IDscLogic
    {
        void MakeItSoSandboxes(ApplicationProfile app);
        void MakeItSoApp(ApplicationProfile app);
        void MakeItSoPolicy(ApplicationProfile app, Policy policy);
        void MakeItSoUser(User user, ApplicationProfile app);
        void MakeItSoTeam(ApplicationProfile app);
        bool ConformConfiguration(ApplicationProfile app, File[] files, Module[] configModules, bool isTest);
        void MakeItSoScan(ApplicationProfile app, File[] files, Module[] configModules);
        void MakeItSoMitigations(ApplicationProfile app);
        void GetLatestStatus(ApplicationProfile app);
        void MakeMitigationTemplates(ApplicationProfile app, bool policy_only);
        bool ConformToPreviousScan(ApplicationProfile app, Module[] configModules);
    }
    public class DscLogic : IDscLogic
    {
        private IVeracodeService _veracodeService;
        private IVeracodeRepository _veracodeRepository;

        public DscLogic(IVeracodeService veracodeService, IVeracodeRepository veracodeRepository)
        {
            _veracodeService = veracodeService;
            _veracodeRepository = veracodeRepository;
        }

        public void MakeItSoMitigations(ApplicationProfile app)
        {
            app.id = $"{_veracodeRepository.GetAllApps().SingleOrDefault(x => x.app_name == app.application_name).app_id}";
            var latest_build = _veracodeRepository.GetLatestScan(app.id);
            if (latest_build == null)
            {
                Console.WriteLine($"{app.application_name} has no completed scans, cannot apply mitigations.");
                return;
            }

            var latest_build_id = $"{latest_build.build.build_id}";

            if (app.mitigations.Any())
            {
                Console.WriteLine($"Checking if the mitigations for Application Profile {app.application_name} have already been applied.");
                var flaw_ids = app.mitigations.Select(x => x.flaw_id).ToArray();

                foreach (var flaw_id in flaw_ids)
                {
                    if (!int.TryParse(flaw_id, out int result))
                    {
                        Console.WriteLine($"The flaw_id of {flaw_id} is invalid, skipping.");
                        continue;
                    }

                    var mitigations = _veracodeRepository.GetMitigationForFlaw(latest_build_id, flaw_id);
                    var config_mitigation = app.mitigations.SingleOrDefault(x => x.flaw_id == flaw_id);
                    if (mitigations.Any()
                        && mitigations[0].mitigation_action.Any()
                        && mitigations[0].mitigation_action.Any(x => x.comment == config_mitigation.action))
                    {
                        Console.WriteLine($"Mitigation for Flaw ID {flaw_id} has already been applied for Application Profile {app.application_name}.");
                    }
                    else
                    {
                        Console.WriteLine($"Applying mitigation for Flaw ID {flaw_id} for Application Profile {app.application_name}.");
                        _veracodeRepository.UpdateMitigations(latest_build_id, config_mitigation.action, config_mitigation.tsrv, flaw_id);
                        _veracodeRepository.UpdateMitigations(latest_build_id, "accepted", "Mitigation applied via DSC", flaw_id);
                    }
                }
            }
        }
        public void MakeItSoApp(ApplicationProfile app)
        {
            Console.WriteLine($"Checking to see if Application Profile {app.application_name} already exists.");
            if (!_veracodeService.DoesAppExist(app))
            {
                Console.WriteLine($"Application Profile {app.application_name} does not exist, adding configuration.");
                try
                {
                    _veracodeService.CreateApp(app);
                    Console.WriteLine($"Application Profile {app.application_name} created succesfully.");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Application Profile {app.application_name} could not be created!");
                    Console.WriteLine($"{e.Message}.");
                }
                return;
            }

            Console.WriteLine($"Application Profile {app.application_name} exists.");
            if (_veracodeService.HasAppChanged(app))
            {
                Console.WriteLine($"Application Profile {app.application_name} has changes, updating configuration.");
                try
                {
                    _veracodeService.UpdateApp(app);
                    Console.WriteLine($"Application Profile {app.application_name} updated succesfully.");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Application Profile {app.application_name} could not be updated!");
                    Console.WriteLine($"{e.Message}.");
                }
                return;
            }

            Console.WriteLine($"Application Profile {app.application_name} has no changes.");
        }

        public void MakeItSoPolicy(ApplicationProfile app, Policy policy)
        {
            Console.WriteLine($"Checking to see if policy for {app.application_name} already exists.");
            if (!_veracodeService.DoesPolicyExist(app))
            {
                Console.WriteLine($"Policy for {app.application_name} does not exist, adding configuration.");
                try
                {
                    _veracodeService.CreatePolicy(app, policy);
                    Console.WriteLine($"Policy for {app.application_name} created succesfully.");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Policy for {app.application_name} could not be created!");
                    Console.WriteLine($"{e.Message}.");
                }
                return;
            }

            Console.WriteLine($"Policy for {app.application_name} exists.");
            if (_veracodeService.HasPolicyChanged(app, policy))
            {
                Console.WriteLine($"Policy for {app.application_name} has changed, updating configuration.");
                try
                {
                    _veracodeService.UpdatePolicy(app, policy);
                    Console.WriteLine($"Policy for {app.application_name} updated succesfully.");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Policy for {app.application_name} could not be updated!");
                    Console.WriteLine($"{e.Message}.");
                }
                return;
            }

            Console.WriteLine($"Policy for {app.application_name} has no changes.");
        }

        public void MakeItSoUser(User user, ApplicationProfile app)
        {
            Console.WriteLine($"Checking to see if user {user.email_address} already exists.");
            if (!_veracodeService.DoesUserExist(user))
            {
                Console.WriteLine($"User {user.email_address} does not exist, adding configuration.");
                try
                {
                    _veracodeService.CreateUser(user, app);
                    Console.WriteLine($"User {user.email_address} created succesfully.");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"User {user.email_address} could not be created!");
                    Console.WriteLine($"{e.Message}.");
                }
                return;
            }

            Console.WriteLine($"User {user.email_address} exists.");
            if (_veracodeService.HasUserChanged(user))
            {
                Console.WriteLine($"User {user.email_address} has changed, updating configuration.");
                try
                {
                    _veracodeService.UpdateUser(user);
                    Console.WriteLine($"User {user.email_address} updated succesfully.");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"User {user.email_address} could not be updated!");
                    Console.WriteLine($"{e.Message}.");
                }
                return;
            }

            Console.WriteLine($"User {user.email_address} has no changes.");
        }

        public void MakeItSoTeam(ApplicationProfile app)
        {
            Console.WriteLine($"Checking to see if team {app.application_name} already exists.");
            if (!_veracodeService.DoesTeamExistForApp(app))
            {
                Console.WriteLine($"Team {app.application_name} does not exist, adding configuration.");
                try
                {
                    _veracodeService.CreateTeamForApp(app);
                    Console.WriteLine($"Team {app.application_name} created succesfully.");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Team {app.application_name} could not be created!");
                    Console.WriteLine($"{e.Message}.");
                }
                return;
            }

            Console.WriteLine($"Team {app.application_name} exists.");
            var usersInTeam = _veracodeService.GetUserEmailsOnTeam(app);
            foreach (var user in usersInTeam)
            {
                Console.WriteLine($"Checking if {user.email_address} is assigned to team {app.application_name}.");
                if (!_veracodeService.IsUserAssignedToTeam(user, app))
                {
                    Console.WriteLine($"User {user.email_address} is not assigned to team {app.application_name}, updating configuration.");
                    try
                    {
                        if (string.IsNullOrEmpty(user.teams))
                            user.teams = $"{app.application_name}";
                        else
                            user.teams = $",{app.application_name}";

                        _veracodeService.UpdateUser(user);
                        Console.WriteLine($"User {user.email_address} assigned to team {app.application_name} succesfully.");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"User {user.email_address} could not be added to team {app.application_name}!");
                        Console.WriteLine($"{e.Message}.");
                    }
                }
                else
                {
                    Console.WriteLine($"User {user.email_address} is already assigned to team {app.application_name}.");
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
                    Console.WriteLine($"New scan created with Build Id {scan_id}. Uploading files");
                    UploadFiles(app, scan_id, files);
                    RunPreScan(app, scan_id);
                    var prescanModules = _veracodeService.GetModules(app.id, scan_id);
                    var doesScanConform = DoesModuleConfigConform(scan_id, configModules, prescanModules);

                    if (isTest)
                        Console.WriteLine($"Test Finished. Deleting Build Id {scan_id}.");

                    if (doesScanConform)
                        Console.WriteLine($"Configuration conforms.");
                    else
                        Console.WriteLine($"Scan does not conform. Deleting Build Id {scan_id}.");


                    if (isTest || !doesScanConform)
                        _veracodeService.DeleteScan(app.id);

                    return doesScanConform;
                }
                else
                {
                    Console.WriteLine($"Policy scan for {app.application_name} already in progress.");
                    Console.WriteLine($"This must be cancelled or completed before this job can be continued.");
                    return false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.Message}.");
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
                    Console.WriteLine("Config does not conform, cancelling scan.");
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

                Console.WriteLine("Starting scan.");
                RunScan(app, $"{scan_id}", moduleList);
                Console.WriteLine($"Deployment complete.");
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.Message}.");
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
            Console.WriteLine($"Uploading {binary.location} to scan {scan_id}.");
            _veracodeService.AddFileToScan(app_id, binary.location);
            Console.WriteLine($"Upload of {binary.location} complete.");
        }
        public void RunScan(ApplicationProfile app, string scan_id, string modules)
        {
            var stopWatch = new Stopwatch();
            TimeSpan ts;
            string elapsedTime;
            stopWatch.Start();

            _veracodeService.StartScan(app.id, modules);
            var scanStatus = BuildStatusType.ScanInProcess;
            while (scanStatus == BuildStatusType.ScanInProcess)
            {
                ts = stopWatch.Elapsed;
                elapsedTime = string.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                    ts.Hours, ts.Minutes, ts.Seconds,
                    ts.Milliseconds / 10);
                Console.WriteLine($"{DateTime.Now.ToLongTimeString()} : Scan {scan_id} is still running and has been running for {elapsedTime}.");
                Thread.Sleep(20000);
                scanStatus = _veracodeService.GetScanStatus(app.id, $"{scan_id}");
            }

            ts = stopWatch.Elapsed;
            elapsedTime = string.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            stopWatch.Stop();

            if (scanStatus == BuildStatusType.ScanErrors)
                throw new Exception("Scan status returned an error status.");

            Console.WriteLine($"Scan complete for {scan_id} and took {elapsedTime}.");
        }

        public void RunPreScan(ApplicationProfile app, string scan_id)
        {
            var stopWatch = new Stopwatch();
            TimeSpan ts;
            string elapsedTime;
            stopWatch.Start();

            _veracodeService.StartPrescan(app.id);

            var scanStatus = BuildStatusType.PreScanSubmitted;
            while (scanStatus == BuildStatusType.PreScanSubmitted)
            {
                ts = stopWatch.Elapsed;
                elapsedTime = string.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                    ts.Hours, ts.Minutes, ts.Seconds,
                    ts.Milliseconds / 10);

                Console.WriteLine($"{DateTime.Now.ToLongTimeString()} : Pre scan {scan_id} is still running and has been running for {elapsedTime}.");
                Thread.Sleep(20000);
                scanStatus = _veracodeService.GetScanStatus(app.id, scan_id);
            }

            ts = stopWatch.Elapsed;
            elapsedTime = string.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            stopWatch.Stop();
            stopWatch.Stop();

            if (scanStatus == BuildStatusType.PreScanFailed)
                throw new Exception("Pre scan status returned an error status.");

            Console.WriteLine($"Pre scan complete for {scan_id} and took {elapsedTime}.");
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
                Console.WriteLine($"There are {missingFromConfig.Count()} modules from prescan that do not match the config.");
                foreach (var mod in missingFromConfig)
                    foreach (var message in mod.messages)
                        Console.WriteLine($"{mod.module_name}:{message}");

                Console.WriteLine($"Please include and complete the below configuration and add to your .json file");
                var messages = new List<string>();
                foreach (var mod in missingFromConfig)
                {
                    var module_selection = mod.can_be_entry_point ? ",\"entry_point\":\"true\"" : "";
                    messages.Add($"{{ " +
                        $"\"module_name\":\"{mod.module_name}\" " +
                        $"{module_selection}" +
                        $"}}");
                }
                Console.WriteLine("\"modules\":[\n" + string.Join(",\n", messages) + "\n]");
            }

            if (missingFromPrescan.Count() > 0)
            {
                Console.WriteLine($"There are {missingFromPrescan.Count()} modules that are configured but are not in the prescan results.");
                Console.WriteLine($"Thes modules need removed or resolved before a scan can continue.");
                foreach (var mod in missingFromPrescan)
                    Console.WriteLine($"{mod.module_name}");
            }

            if (missingFromConfig.Count() > 0 || missingFromPrescan.Count() > 0)
            {
                Console.WriteLine($"Module selection configuration was incorrect for {newScan}.");
                return false;
            }

            Console.WriteLine($"Module selection conforms for {newScan} and the scan can commence.");
            return true;
        }

        public void GetLatestStatus(ApplicationProfile app)
        {
            app.id = $"{_veracodeRepository.GetAllApps().SingleOrDefault(x => x.app_name == app.application_name).app_id}";
            var sandboxes = _veracodeRepository.GetSandboxesForApp(app.id);
            var latest_policy_build = _veracodeRepository.GetLatestScan(app.id).build;

            var scanStatus = _veracodeService.GetScanStatus(app.id, $"{latest_policy_build.build_id}");
            Console.WriteLine($"[{app.application_name}][Policy][Scan Status] {VeracodeEnumConverter.Convert(scanStatus)}");

            var compliance = VeracodeEnumConverter.Convert(latest_policy_build.policy_compliance_status);
            Console.WriteLine($"[{app.application_name}][Policy][Compliance Status] {compliance}");

            foreach (var sandbox in sandboxes)
            {
                var latest_sandbox_build = _veracodeRepository.GetLatestScanSandbox(app.id, $"{sandbox.sandbox_id}");
                if (latest_sandbox_build == null)
                {
                    Console.WriteLine($"[{app.application_name}][Sandbox {sandbox.sandbox_name}][Scan Status] There are no scans!");
                }
                else
                {
                    var latest_sandbox_build_id = $"{latest_sandbox_build.build.build_id}";
                    var scanSandboxStatus = _veracodeService.GetScanStatus(app.id, latest_sandbox_build_id);
                    Console.WriteLine($"[{app.application_name}][Sandbox {sandbox.sandbox_name}][Scan Status] {VeracodeEnumConverter.Convert(scanSandboxStatus)}");

                    var sandboxCompliance = VeracodeEnumConverter.Convert(latest_sandbox_build.build.policy_compliance_status);
                    Console.WriteLine($"[{app.application_name}][Sandbox {sandbox.sandbox_name}][Compliance Status] {VeracodeEnumConverter.Convert(latest_sandbox_build.build.policy_compliance_status)}");
                }
            }
        }

        public void MakeMitigationTemplates(ApplicationProfile app, bool policy_only)
        {
            app.id = $"{_veracodeRepository.GetAllApps().SingleOrDefault(x => x.app_name == app.application_name).app_id}";
            var latest_build = _veracodeRepository.GetLatestScan(app.id);
            if (latest_build == null)
            {
                Console.WriteLine($"{app.application_name} has no completed scans, no mitigations to create templates for.");
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

            Console.WriteLine("\"mitigations\":[\n" + string.Join(",\n", messages) + "\n]");
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
            Console.WriteLine($"[{app.application_name}] Checking Sandboxes...");
            var current_sandboxes = _veracodeRepository.GetSandboxesForApp(app.id);
            var config_sandboxes = app.sandboxes;

            if (!config_sandboxes.Any())
            {
                Console.WriteLine($"[{app.application_name}] No sandboxes in configuration. Skipping.");
                return;
            }

            foreach (var config_sandbox in config_sandboxes)
            {
                if (!current_sandboxes.Any(x => x.sandbox_name == config_sandbox.sandbox_name))
                {
                    Console.WriteLine($"[{app.application_name}] Does not have sandbox with name {config_sandbox.sandbox_name}. Creating...");
                    _veracodeRepository.CreateSandbox(app.id, config_sandbox.sandbox_name);
                    Console.WriteLine($"[{app.application_name}] {config_sandbox.sandbox_name} creation complete!");
                } else
                {
                    Console.WriteLine($"[{app.application_name}] {config_sandbox.sandbox_name} already exists! Nothing to do.");
                }
            }

            Console.WriteLine($"[{app.application_name}] Finished Sandboxes!");
        }
    }
}
