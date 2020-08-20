using System;
using System.Collections.Generic;
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
        void UpdateApp(ApplicationProfile app);
        bool DoesTeamExistForApp(ApplicationProfile app);
        bool CreateTeamForApp(ApplicationProfile app);
        bool IsUserAssignedToTeam(User user, ApplicationProfile app);
        bool DoesUserExist(User user);
        bool HasUserChanged(User user);
        bool CreateUser(User user, ApplicationProfile app);
        void UpdateUser(User user);
        bool DoesPolicyExist(ApplicationProfile app);
        bool CreatePolicy(ApplicationProfile app, Policy policy);
        void UpdatePolicy(ApplicationProfile app, Policy policy);
        bool HasPolicyChanged(ApplicationProfile app, Policy policy);
        string CreateScan(ApplicationProfile app);
        BuildStatusType GetScanStatus(string app_id, string scan);
        bool CancelScan(string scan_id, string app_id);
        bool IsPolicyScanInProgress(ApplicationProfile app);
        void AddBinaryToScan(string app_id, string filepath);
        void StartPrescan(string app_id);
        Module[] GetModules(string app_id, string scan_id);
        void StartScan(string app_id, string modules);
        void DeleteScan(string app_id);
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
            var newApp = _veracodeRepository.CreateApp(new ApplicationType
            {
                app_name = app.application_name,
                business_owner = app.business_owner,
                business_owner_email = app.business_owner_email,
                business_criticality = VeracodeEnumConverter.Convert(app.criticality),
                policy = app.policy.name
            });
            return _veracodeRepository
                .GetAllApps()
                .Any(x => x.app_name == app.application_name);
        }

        public bool CreatePolicy(ApplicationProfile app, Policy policy)
        {
            _veracodeRepository.CreatePolicy(new PolicyVersion
            {
               name = $"{app.application_name}",
               description = $"Custom policy for application {app.application_name}",
               sca_blacklist_grace_period = policy.sca_blacklist_grace_period,
               score_grace_period = policy.score_grace_period,
               sev0_grace_period = policy.sev0_grace_period,
               sev1_grace_period = policy.sev1_grace_period,
               sev2_grace_period = policy.sev2_grace_period,
               sev3_grace_period = policy.sev3_grace_period,
               sev4_grace_period = policy.sev4_grace_period,
               sev5_grace_period = policy.sev5_grace_period,
               type = "CUSTOMER",
               vendor_policy = false,
               scan_frequency_rules = policy.scan_frequency_rules,
               finding_rules = policy.finding_rules,
               created = DateTime.Now
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
                team_name = $"{app.application_name}"
            });
            return _veracodeRepository
                .GetTeams()
                .Any(x => x.team_name == app.application_name);
        }

        public bool CreateUser(User user, ApplicationProfile app)
        {
            var roles = user.roles
                .Split(",")
                .Select(EnumConverter.Convert)
                .ToArray();

            _veracodeRepository.CreateUser(new LoginAccount
            {
                email_address = user.email_address,
                first_name = user.first_name,
                last_name = user.last_name,
                teams = app.application_name
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
                       messages = x.issue != null ?
                       x.issue.Select(x => x.details).ToList()
                       : new List<string>()
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
            var teamlite = _veracodeRepository.GetTeams().Single(x => x.team_name == $"{app.application_name}");
            return _veracodeRepository.GetTeamInfo(teamlite.team_id, true, false)
                .user.Select(x => new User
            {
                    email_address = x.email_address
            }).ToArray();
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
                Console.WriteLine($"The criticality for {app.application_name} is no longer {appDetail.application[0].business_criticality} it is {app.criticality}.");
                return true;
            }

            if (appDetail.application[0].business_owner_email != app.business_owner_email)
            {
                Console.WriteLine($"The business_owner_email for {app.application_name} is no longer {appDetail.application[0].business_owner_email} it is {app.business_owner_email}.");
                return true;
            }

            if (appDetail.application[0].business_owner != app.business_owner)
            {
                Console.WriteLine($"The business_owner for {app.application_name} is no longer {appDetail.application[0].business_owner} it is {app.business_owner}.");
                return true;
            }
            return false;
        }

        public bool HasPolicyChanged(ApplicationProfile app, Policy policy)
        {
            var retrievedPolicy = _veracodeRepository.GetPolicies()
                .SingleOrDefault(x => x.name == app.application_name);
            if(retrievedPolicy.description != policy.description) return true;
            if (retrievedPolicy.sca_blacklist_grace_period != policy.sca_blacklist_grace_period) return true;
            if (retrievedPolicy.score_grace_period != policy.score_grace_period) return true;
            if (retrievedPolicy.sev0_grace_period != policy.sev0_grace_period) return true;
            if (retrievedPolicy.sev1_grace_period != policy.sev1_grace_period) return true;
            if (retrievedPolicy.sev2_grace_period != policy.sev2_grace_period) return true;
            if (retrievedPolicy.sev3_grace_period != policy.sev3_grace_period) return true;
            if (retrievedPolicy.sev4_grace_period != policy.sev4_grace_period) return true;
            if (retrievedPolicy.sev5_grace_period != policy.sev5_grace_period) return true;

            foreach (var custom_severity in retrievedPolicy.custom_severities)
                if(!policy.custom_severities.Any(x => x.cwe == custom_severity.cwe && x.severity == custom_severity.severity))
                    return true;

            foreach (var finding_rule in retrievedPolicy.finding_rules)
                if (!policy.finding_rules.Any(x => x.type == finding_rule.type && x.value == finding_rule.value))
                    return true;

            foreach (var scan_frequency_rule in retrievedPolicy.scan_frequency_rules)
                if (!policy.scan_frequency_rules.Any(x => x.frequency == scan_frequency_rule.frequency && x.scan_type == scan_frequency_rule.scan_type))
                    return true;

            return false;
        }

        public bool HasUserChanged(User user)
        {
            var returnedUser = _veracodeRepository.GetUser(user.email_address);
            if(returnedUser.first_name != user.first_name) return true;
            if (returnedUser.last_name != user.last_name) return true;
            var myRoles = user.roles.Split(",").ToArray();
            var savedRoles = returnedUser.roles.Split(",").ToArray();

            foreach (var role in myRoles)
                if (!savedRoles.Any(x => x == role))
                    return true;

            return false;
        }

        public bool IsPolicyScanInProgress(ApplicationProfile app)
        {
            try
            {
                var latestBuild = _veracodeRepository.GetLatestcan(app.id);
                if (latestBuild == null)
                    return false;

                return latestBuild.build.analysis_unit[0].status != BuildStatusType.PreScanSubmitted || latestBuild.build.analysis_unit[0].status != BuildStatusType.ScanInProcess;

            } catch (Exception e)
            {
                if (e.Message.ToLower().Contains("could not find"))
                    return false;

                throw e;
            }
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

        public void UpdateApp(ApplicationProfile app)
        {
            var app_id = _veracodeRepository
                .GetAllApps()
                .SingleOrDefault(x => x.app_name == app.application_name)
                .app_id;

            var returnedApp = _veracodeRepository.GetAppDetail($"{app_id}").application[0];
            returnedApp.business_criticality = VeracodeEnumConverter.Convert(app.criticality);
            returnedApp.business_owner = app.business_owner;
            returnedApp.business_owner_email = app.business_owner_email;
            _veracodeRepository.UpdateApp(returnedApp);
        }

        public void UpdatePolicy(ApplicationProfile app, Policy policy)
        {
            var retrievedPolicy = _veracodeRepository.GetPolicies()
                .SingleOrDefault(x => x.name == app.application_name);
            retrievedPolicy.sca_blacklist_grace_period = policy.sca_blacklist_grace_period;
            retrievedPolicy.score_grace_period = policy.score_grace_period;
            retrievedPolicy.sev0_grace_period = policy.sev0_grace_period;
            retrievedPolicy.sev1_grace_period = policy.sev1_grace_period;
            retrievedPolicy.sev2_grace_period = policy.sev2_grace_period;
            retrievedPolicy.sev3_grace_period = policy.sev3_grace_period;
            retrievedPolicy.sev4_grace_period = policy.sev4_grace_period;
            retrievedPolicy.sev5_grace_period = policy.sev5_grace_period;
            retrievedPolicy.custom_severities = policy.custom_severities;
            retrievedPolicy.finding_rules = policy.finding_rules;
            retrievedPolicy.scan_frequency_rules = policy.scan_frequency_rules;
            retrievedPolicy.created = DateTime.Now;
            _veracodeRepository.UpdatePolicy(retrievedPolicy, retrievedPolicy.guid);
        }

        public void UpdateUser(User user)
        {
            var returnedUser = _veracodeRepository.GetUser(user.email_address);
            returnedUser.first_name = user.first_name;
            returnedUser.last_name = user.last_name;
            var roles = user.roles
             .Split(",")
             .Select(EnumConverter.Convert)
             .ToArray();

            _veracodeRepository.UpdateUser(returnedUser, roles);
        }
    }
}