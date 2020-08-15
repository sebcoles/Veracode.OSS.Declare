using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VeracodeDSC.Shared;
using VeracodeService.Models;

namespace VeracodeDSC.Logic
{
    public interface IDscLogic
    {
        void MakeItSoApp(ApplicationProfile app);
        void MakeItSoPolicy(ApplicationProfile app, Policy policy);
        void MakeItSoUser(User user, ApplicationProfile app);
        void MakeItSoTeam(ApplicationProfile app);
        void MakeItSoScan(ApplicationProfile app, Binary[] binaries, Module[] configModules);
    }
    public class DscLogic : IDscLogic
    {
        private IVeracodeService _veracodeService;

        public DscLogic(IVeracodeService veracodeService)
        {
            _veracodeService = veracodeService;
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
                    _veracodeService.CreateUser(user);
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

        public void MakeItSoScan(ApplicationProfile app, Binary[] binaries, Module[] configModules)
        {          
            try
            {
                IsScanInProgress(app);
                var scan_id = _veracodeService.CreateScan(app);
                Console.WriteLine($"New scan created with Build Id {scan_id}. Uploading binaries");
                UploadFiles(app, scan_id, binaries);
                RunPreScan(app, scan_id);
                ConformModules(app, scan_id, configModules);
                RunScan(app, 
                    string.Join(",", configModules
                    .Select(y => y.module_id).ToArray()));
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.Message}.");
                return;
            }
            Console.WriteLine($"Deployment complete.");
        }

        public void IsScanInProgress(ApplicationProfile app)
        {
            Console.WriteLine($"Checking if a policy scan for {app.application_name} is already in progress.");
            if (_veracodeService.IsPolicyScanInProgress(app))
                throw new Exception($"Policy scan for {app.application_name} in progress.");
            
            Console.WriteLine($"No scan in progress. Creating policy scan for {app.application_name}.");
        }

        public void UploadFiles(ApplicationProfile app, string scan_id, Binary[] binaries)
        {
            var tasks = new Task[binaries.Length];
            for (var i = 0; i < tasks.Length; i++)
            {
                tasks[i] = new Task(() =>
                {
                    Console.WriteLine($"Uploading {binaries[i]} to scan {scan_id}.");
                    _veracodeService.AddBinaryToScan(app.id, binaries[i].location);
                    Console.WriteLine($"Upload of {binaries[i]} complete.");
                });
            }
            Task.WaitAll(tasks);
        }
        public void RunScan(ApplicationProfile app, string newScan)
        {
            _veracodeService.StartScan(app.id, newScan);

            var scanStatus = BuildStatusType.ScanInProcess;
            while (scanStatus == BuildStatusType.ScanInProcess)
            {
                Console.WriteLine($"Scan {newScan} is still running.");
                Thread.Sleep(60000);
                scanStatus = _veracodeService.GetScanStatus(app.id, newScan);
            }

            if (scanStatus == BuildStatusType.ScanErrors)            
                throw new Exception("Scan status returned an error status.");
            
            Console.WriteLine($"Scan complete for {newScan}.");
        }

        public void RunPreScan(ApplicationProfile app, string newScan)
        {
            _veracodeService.StartPrescan(app.id);

            var scanStatus = BuildStatusType.PreScanSubmitted;
            while (scanStatus == BuildStatusType.PreScanSubmitted)
            {
                Console.WriteLine($"Pre scan {newScan} is still running.");
                Thread.Sleep(60000);
                scanStatus = _veracodeService.GetScanStatus(app.id, newScan);
            }

            if (scanStatus == BuildStatusType.PreScanFailed)
                throw new Exception("Pre scan status returned an error status.");

            Console.WriteLine($"Pre scan complete for {newScan}.");
        }

        public void ConformModules(ApplicationProfile app, string newScan, Module[] configModules)
        {
            var modules = _veracodeService.GetModules(app.id, newScan);
            var missingFromConfig = modules.Except(configModules);
            var missingFromPrescan = configModules.Except(modules);

            var invalidEntryPoints = configModules.Where(x => x.is_entry_point 
                && modules.Any(y => y.module_id == x.module_id && !y.can_be_entry_point));

            if(invalidEntryPoints.Count() > 0)
            {
                foreach (var mod in invalidEntryPoints)
                {
                    Console.WriteLine($"Module module_id={mod.module_id}, " +
                                      $"module_name={mod.module_name} " +
                                      $"cannot be selected as an entry point. " +
                                      $"Please set entry_poiny to false or resolve issue.");
                    foreach (var message in mod.messages)
                        Console.WriteLine(message);
                }
                _veracodeService.DeleteScan(newScan);
                throw new Exception($"Module selection configuration was incorrect for {newScan}.");
            }

            if (missingFromConfig.Count() > 0 || missingFromPrescan.Count() > 0)
            {
                if(missingFromConfig.Count() > 0)
                    foreach(var mod in missingFromConfig)
                        Console.WriteLine($"Module module_id={mod.module_id}, " +
                            $"module_name={mod.module_name}, " +
                            $"can_be_entry_point={mod.can_be_entry_point} was missing from the configuration.");

                if (missingFromPrescan.Count() > 0)
                    foreach (var mod in missingFromPrescan)
                        Console.WriteLine($"Module module_id={mod.module_id}, " +
                            $"module_name={mod.module_name}, " +
                            $"can_be_entry_point={mod.can_be_entry_point} was missing from the pre scan results.");

                _veracodeService.DeleteScan(newScan);
                throw new Exception($"Module selection configuration was incorrect for {newScan}.");
            }

            Console.WriteLine($"Module selection conforms for {newScan} and the scan can commence.");
        }
    }
}
