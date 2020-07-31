using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VeracodeDSC.Shared.Enums;
using VeracodeDSC.Shared.Models;

namespace VeracodeDSC.Logic
{
    public interface IDscLogic
    {
        void MakeItSoApp(DscApp app);
        void MakeItSoPolicy(DscApp app, DscPolicy policy);
        void MakeItSoUser(DscUser user, DscApp app);
        void MakeItSoTeam(DscApp app, DscTeam user);
        void MakeItSoScan(DscApp app, DscScan scan, DscBinary[] binaries);
    }
    public class DscLogic : IDscLogic
    {
        private IVeracodeService _veracodeService;

        public DscLogic(IVeracodeService veracodeService)
        {
            _veracodeService = veracodeService;
        }
        public void MakeItSoApp(DscApp app)
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

        public void MakeItSoPolicy(DscApp app, DscPolicy policy)
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

        public void MakeItSoUser(DscUser user, DscApp app)
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

        public void MakeItSoTeam(DscApp app, DscTeam team)
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
            foreach (var user in team.Users)
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

        public void MakeItSoScan(DscApp app, DscScan scan, DscBinary[] binaries)
        {          
            try
            {
                IsScanInProgress(app);
                var newScan = _veracodeService.CreateScan(app);
                Console.WriteLine($"New scan created with Build Id {newScan.build_id}. Uploading binaries");
                UploadFiles(newScan, binaries);
                RunPreScan(newScan);
                ConformModules(scan, newScan);
                RunScan(newScan);
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.Message}.");
                return;
            }
            Console.WriteLine($"Deployment complete.");
        }

        public void IsScanInProgress(DscApp app)
        {
            Console.WriteLine($"Checking if a policy scan for {app.application_name} is already in progress.");
            if (_veracodeService.IsPolicyScanInProgress(app))
            {
                var currentScan = _veracodeService.GetScan(app);
                foreach (var message in currentScan.Messages)
                    Console.WriteLine(message);

                throw new Exception($"Policy scan for {app.application_name} in progress.");
            }

            Console.WriteLine($"No scan in progress. Creating policy scan for {app.application_name}.");
        }

        public void UploadFiles(DscScan newScan, DscBinary[] binaries)
        {
            var tasks = new Task[binaries.Length];
            for (var i = 0; i < tasks.Length; i++)
            {
                var fileStream = new FileStream(binaries[i].location, FileMode.Open, FileAccess.Read);
                tasks[i] = new Task(() =>
                {
                    Console.WriteLine($"Uploading {binaries[i]} to scan {newScan.build_id}.");
                    _veracodeService.AddBinaryToScan(newScan, fileStream);
                    Console.WriteLine($"Upload of {binaries[i]} complete.");
                });
            }
            Task.WaitAll(tasks);
        }
        public void RunScan(DscScan newScan)
        {
            _veracodeService.StartScan(newScan);

            var scanStatus = ScanStatus.Running;
            while (scanStatus == ScanStatus.Running)
            {
                Console.WriteLine($"Scan {newScan.build_id} is still running.");
                Thread.Sleep(60000);
                scanStatus = _veracodeService.GetScanStatus(newScan);
            }

            if (scanStatus == ScanStatus.Error)            
                throw new Exception("Scan status returned an error status.");
            
            Console.WriteLine($"Scan complete for {newScan.build_id}.");
        }

        public void RunPreScan(DscScan newScan)
        {
            _veracodeService.StartPrescan(newScan);

            var scanStatus = PreScanStatus.Running;
            while (scanStatus == PreScanStatus.Running)
            {
                Console.WriteLine($"Pre scan {newScan.build_id} is still running.");
                Thread.Sleep(60000);
                scanStatus = _veracodeService.GetPrescanStatus(newScan);
            }

            if (scanStatus == PreScanStatus.Error)
                throw new Exception("Pre scan status returned an error status.");

            Console.WriteLine($"Pre scan complete for {newScan.build_id}.");
        }

        public void ConformModules(DscScan scan, DscScan newScan)
        {
            var modules = _veracodeService.GetModules(newScan);
            var missingFromConfig = modules.Except(scan.Modules);
            var missingFromPrescan = scan.Modules.Except(modules);

            if (missingFromConfig.Count() > 0 || missingFromPrescan.Count() > 0)
            {
                if(missingFromConfig.Count() > 0)
                    foreach(var mod in missingFromConfig)
                        Console.WriteLine($"Module module_id={mod.module_id}, " +
                            $"module_name={mod.module_name}, " +
                            $"entry_point={mod.entry_point} was missing from the configuration.");

                if (missingFromPrescan.Count() > 0)
                    foreach (var mod in missingFromPrescan)
                        Console.WriteLine($"Module module_id={mod.module_id}, " +
                            $"module_name={mod.module_name}, " +
                            $"entry_point={mod.entry_point} was missing from the pre scan results.");

                _veracodeService.DeleteScan(newScan);
                throw new Exception($"Module selection configuration was incorrect for {newScan.build_id}.");
            }

            Console.WriteLine($"Module selection conforms for {newScan.build_id} and the scan can commence.");
        }
    }
}
