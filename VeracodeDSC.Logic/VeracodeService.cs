using System;
using System.Linq;
using VeracodeDSC.Shared;
using VeracodeService;
using VeracodeService.Models;
using VeracodeService.Rest;

namespace VeracodeDSC
{
    public interface IVeracodeService
    {
        bool DoesAppExist(ApplicationProfile app);
        bool HasAppChanged(ApplicationProfile app);
        bool CreateApp(ApplicationProfile app);
        bool UpdateApp(ApplicationProfile app);
        bool DoesTeamExistForApp(ApplicationProfile app);
        bool CreateTeamForApp(ApplicationProfile app);
        bool IsUserAssignedToTeam(User user, ApplicationProfile app);
        bool DoesUserExist(User user);
        bool HasUserChanged(User user);
        bool CreateUser(User user);
        bool UpdateUser(User user);
        bool DoesPolicyExist(ApplicationProfile app);
        bool CreatePolicy(ApplicationProfile app, Policy policy);
        bool UpdatePolicy(ApplicationProfile app, Policy policy);
        bool HasPolicyChanged(ApplicationProfile app, Policy policy);
        string CreateScan(ApplicationProfile app);
        BuildStatusType GetScanStatus(string app_id, string scan);
        bool CancelScan(string scan_id, string app_id);
        bool IsPolicyScanInProgress(ApplicationProfile app);
        void AddBinaryToScan(string app_id, string filepath);
        void StartPrescan(string app_id);
        Module[] GetModules(string app_id, string scan_id);
        void StartScan(string app_id, string modules);
        void DeleteScan(string newScan);
        User[] GetUserEmailsOnTeam(ApplicationProfile app);
    }
    public class VeracodeService : IVeracodeService
    {
        private IVeracodeRepository _veracodeRepository;
        public VeracodeService(IVeracodeRepository veracodeRepository)
        {
            _veracodeRepository = veracodeRepository;
        }
        public void AddBinaryToScan(string app_id, string filepath)
        {
            _veracodeRepository.UploadFileForPrescan(app_id, filepath);
        }

        public bool CancelScan(string scan_id, string app_id)
        {
            _veracodeRepository.DeleteBuild(app_id);
            var buildList = _veracodeRepository.GetAllBuildsForApp(app_id);
            return buildList.Any(x => $"{x.build_id}" == scan_id);
        }

        public bool CreateApp(ApplicationProfile app)
        {
            _veracodeRepository.CreateApp(new ApplicationType
            {
                app_name = app.application_name,
                business_owner = app.business_owner,
                business_owner_email = app.business_owner,
                business_criticality = VeracodeEnumConverter.Convert(app.criticality)
            });
            return _veracodeRepository
                .GetAllApps()
                .Any(x => x.app_name == app.application_name);
        }

        public bool CreatePolicy(ApplicationProfile app, Policy policy)
        {
            _veracodeRepository.CreatePolicy(new PolicyVersion
            {
               name = app.application_name
               // MORE NEEDED
            });
            return _veracodeRepository
                .GetPolicies()
                .Any(x => x.name == app.application_name);
        }

        public string CreateScan(ApplicationProfile app)
        {
            var build = new BuildInfoBuildType
            {
                version = $"VeracodeDSC - {app.application_name} - {DateTime.Now.ToShortTimeString()}"
            };
            return _veracodeRepository
                .CreateBuild(app.id, build)
                .build_id.ToString();
        }

        public bool CreateTeamForApp(ApplicationProfile app)
        {
            _veracodeRepository.CreateTeam(new teaminfo
            {
                team_name = app.application_name
            });
            return _veracodeRepository
                .GetTeams()
                .Any(x => x.team_name == app.application_name);
        }

        public bool CreateUser(User user)
        {
            var roles = user.roles
                .Split(",")
                .Select(EnumConverter.Convert)
                .ToArray();

            _veracodeRepository.CreateUser(new LoginAccount
            {
                email_address = user.email_address,
                first_name = user.first_name,
                last_name = user.last_name
            }, roles);

            return _veracodeRepository
                .GetUsers()
                .Any(x => x == user.email_address);
        }

        public void DeleteScan(string app_id)
        {
            _veracodeRepository.DeleteBuild(app_id);
        }

        public bool DoesAppExist(ApplicationProfile app)
        {
            return _veracodeRepository
                .GetAllApps()
                .Any(x => x.app_name == app.application_name);
        }

        public bool DoesPolicyExist(ApplicationProfile app)
        {
            return _veracodeRepository
                .GetPolicies()
                .Any(x => x.name == app.application_name);
        }

        public bool DoesTeamExistForApp(ApplicationProfile app)
        {
            return _veracodeRepository
                .GetTeams()
                .Any(x => x.team_name == app.application_name);
        }

        public bool DoesUserExist(User user)
        {
            return _veracodeRepository
                   .GetUsers()
                   .Any(x => x == user.email_address);
        }

        public Module[] GetModules(string app_id, string scan_id)
        {
            return _veracodeRepository
                   .GetModules(app_id, scan_id)
                   .Select(x => new Module
                   {
                       module_name = x.name,
                       can_be_entry_point = !x.has_fatal_errors,
                       module_id = $"{x.id}",
                       messages = x.issue
                       .Select(x => x.details)
                       .ToList()
                   }).ToArray();
        }

        public BuildStatusType GetScanStatus(string app_id, string scan_id)
        {
            return _veracodeRepository
                .GetBuildDetail(app_id, scan_id)
                .build.analysis_unit[0].status;
        }

        public User[] GetUserEmailsOnTeam(ApplicationProfile app)
        {
            //var teaminfo = _veracodeRepository.GetTeams().Single(x => x.team_name == app.application_name);
            //var users = _veracodeRepository.GetUsers().
            throw new NotImplementedException();
        }

        public bool HasAppChanged(ApplicationProfile app)
        {
            var retrievedApp = _veracodeRepository.GetAllApps()
                .SingleOrDefault(x => x.app_name == app.application_name);

            if (retrievedApp == null)
            {
                Console.WriteLine($"There is no application profile with the name {app.application_name}.");
                return true;
            }

            var appDetail = _veracodeRepository.GetAppDetail($"{retrievedApp.app_id}");

            if(appDetail.application[0].business_criticality != VeracodeEnumConverter.Convert(app.criticality))
            {
                Console.WriteLine($"The criticality for {app.application_name} has changed.");
                return true;
            }

            if (appDetail.application[0].business_owner_email != app.business_owner_email)
            {
                Console.WriteLine($"The business_owner_email for {app.application_name} is no longer {appDetail.application[0].business_owner_email}.");
                return true;
            }

            if (appDetail.application[0].business_owner != app.business_owner)
            {
                Console.WriteLine($"The business_owner for {app.application_name} is no longer {appDetail.application[0].business_owner}.");
                return true;
            }
            return false;
        }

        public bool HasPolicyChanged(ApplicationProfile app, Policy policy)
        {
            throw new System.NotImplementedException();
        }

        public bool HasUserChanged(User user)
        {
            throw new System.NotImplementedException();
        }

        public bool IsPolicyScanInProgress(ApplicationProfile app)
        {

            throw new NotImplementedException();
           // return GetScanStatus() == BuildStatusType.ScanInProcess;
        }

        public bool IsUserAssignedToTeam(User user, ApplicationProfile app)
        {
            throw new System.NotImplementedException();
        }

        public void StartPrescan(string app_id)
        {
            _veracodeRepository.StartPrescan(app_id);
        }

        public void StartScan(string app_id, string modules)
        {
            _veracodeRepository.StartScan(app_id, modules);
        }

        public bool UpdateApp(ApplicationProfile app)
        {
            throw new System.NotImplementedException();
        }

        public bool UpdatePolicy(ApplicationProfile app, Policy policy)
        {
            throw new System.NotImplementedException();
        }

        public bool UpdateUser(User user)
        {
            throw new System.NotImplementedException();
        }
    }
}