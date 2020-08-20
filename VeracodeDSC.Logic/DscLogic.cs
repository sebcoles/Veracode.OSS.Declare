using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VeracodeDSC.Shared;
using VeracodeService;
using VeracodeService.Models;

namespace VeracodeDSC.Logic
{
    public interface IDscLogic
    {
        void MakeItSoApp(ApplicationProfile app);
        void MakeItSoPolicy(ApplicationProfile app, Policy policy);
        void MakeItSoUser(User user, ApplicationProfile app);
        void MakeItSoTeam(ApplicationProfile app);
        bool ConformConfiguration(ApplicationProfile app, Binary[] binaries, Module[] configModules, bool isTest);
        void MakeItSoScan(ApplicationProfile app, Binary[] binaries, Module[] configModules);
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
                        if(string.IsNullOrEmpty(user.teams))
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

        public bool ConformConfiguration(ApplicationProfile app, Binary[] binaries, Module[] configModules, bool isTest)
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
                    Console.WriteLine($"New scan created with Build Id {scan_id}. Uploading binaries");
                    UploadFiles(app, scan_id, binaries);
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
                return false;
            }
        }

        public void MakeItSoScan(ApplicationProfile app, Binary[] binaries, Module[] configModules)
        {          
            try
            {
                var app_id = _veracodeRepository.GetAllApps().SingleOrDefault(x => x.app_name == app.application_name).app_id;
                
                if(!ConformConfiguration(app, binaries, configModules, false))
                {
                    Console.WriteLine("Config does not conform, cancelling scan.");
                    return;
                }               

                var scan_id = _veracodeRepository.GetLatestcan($"{app_id}").build_id;
                var entry_points = configModules
                    .Where(x => x.entry_point)
                    .Select(y => y.module_name)
                    .ToArray();

                var modulesToScan = _veracodeService.GetModules(app.id, $"{scan_id}")
                    .Where(x => entry_points.Contains(x.module_name))
                    .Select(y => y.module_id)
                    .ToArray();

                var moduleList = string.Join(",", modulesToScan);

                RunScan(app, $"{scan_id}", moduleList);
                Console.WriteLine($"Deployment complete.");               
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.Message}.");
            }
        }

        public void UploadFiles(ApplicationProfile app, string scan_id, Binary[] binaries)
        {
            var tasks = new Task[binaries.Length];
            for (var i = 0; i < tasks.Length; i++)
            {
                var binary = binaries[i];
                tasks[i] = new Task(() => UploadTask(binary, app.id, scan_id));
            }

            foreach (var task in tasks)
                task.Start();

            Task.WaitAll(tasks);
        }

        private void UploadTask(Binary binary, string app_id, string scan_id)
        {
            Console.WriteLine($"Uploading {binary.location} to scan {scan_id}.");
            _veracodeService.AddBinaryToScan(app_id, binary.location);
            Console.WriteLine($"Upload of {binary.location} complete.");
        }
        public void RunScan(ApplicationProfile app, string scan_id, string modules)
        {
            _veracodeService.StartScan(app.id, modules);
            var scanStatus = BuildStatusType.ScanInProcess;
            while (scanStatus == BuildStatusType.ScanInProcess)
            {
                Console.WriteLine($"Scan {scan_id} is still running.");
                Thread.Sleep(60000);
                scanStatus = _veracodeService.GetScanStatus(app.id, $"{scan_id}");
            }

            if (scanStatus == BuildStatusType.ScanErrors)            
                throw new Exception("Scan status returned an error status.");
            
            Console.WriteLine($"Scan complete for {scan_id}.");
        }

        public void RunPreScan(ApplicationProfile app, string newScan)
        {
            _veracodeService.StartPrescan(app.id);

            var scanStatus = BuildStatusType.PreScanSubmitted;
            while (scanStatus == BuildStatusType.PreScanSubmitted)
            {
                Console.WriteLine($"Pre scan {newScan} is still running.");
                Thread.Sleep(10000);
                scanStatus = _veracodeService.GetScanStatus(app.id, newScan);
            }

            if (scanStatus == BuildStatusType.PreScanFailed)
                throw new Exception("Pre scan status returned an error status.");

            Console.WriteLine($"Pre scan complete for {newScan}.");
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
                        $"\"module_id\":\"{mod.module_id}\", " +
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
    }
}
