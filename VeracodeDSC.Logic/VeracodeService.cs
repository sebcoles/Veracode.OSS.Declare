using System.IO;
using System.Threading.Tasks;
using VeracodeDSC.Shared;
using VeracodeDSC.Shared.Enums;
using VeracodeService;

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
        ScanStatus GetScanStatus(string scan);
        bool CancelScan(string scan);
        bool IsPolicyScanInProgress(ApplicationProfile app);
        string GetScan(ApplicationProfile app);
        void AddBinaryToScan(string newScan, FileStream fileStream);
        void StartPrescan(string newScan);
        PreScanStatus GetPrescanStatus(string newScan);
        Module[] GetModules(string newScan);
        void StartScan(string newScan);
        void DeleteScan(string newScan);
        User[] GetUserEmailsOnTeam(ApplicationProfile app);
        string[] GetScanMessages(ApplicationProfile app);
    }
    public class VeracodeService : IVeracodeService
    {
        private IVeracodeRepository _veracodeRepository;
        public VeracodeService(IVeracodeRepository veracodeRepository)
        {
            _veracodeRepository = veracodeRepository;
        }
        public void AddBinaryToScan(string newScan, FileStream fileStream)
        {
            throw new System.NotImplementedException();
        }

        public bool CancelScan(string scan)
        {
            throw new System.NotImplementedException();
        }

        public bool CreateApp(ApplicationProfile app)
        {
            throw new System.NotImplementedException();
        }

        public bool CreatePolicy(ApplicationProfile app, Policy policy)
        {
            throw new System.NotImplementedException();
        }

        public string CreateScan(ApplicationProfile app)
        {
            throw new System.NotImplementedException();
        }

        public bool CreateTeamForApp(ApplicationProfile app)
        {
            throw new System.NotImplementedException();
        }

        public bool CreateUser(User user)
        {
            throw new System.NotImplementedException();
        }

        public void DeleteScan(string newScan)
        {
            throw new System.NotImplementedException();
        }

        public bool DoesAppExist(ApplicationProfile app)
        {
            throw new System.NotImplementedException();
        }

        public bool DoesPolicyExist(ApplicationProfile app)
        {
            throw new System.NotImplementedException();
        }

        public bool DoesTeamExistForApp(ApplicationProfile app)
        {
            throw new System.NotImplementedException();
        }

        public bool DoesUserExist(User user)
        {
            throw new System.NotImplementedException();
        }

        public Module[] GetModules(string newScan)
        {
            throw new System.NotImplementedException();
        }

        public PreScanStatus GetPrescanStatus(string newScan)
        {
            throw new System.NotImplementedException();
        }

        public string GetScan(ApplicationProfile app)
        {
            throw new System.NotImplementedException();
        }

        public string[] GetScanMessages(ApplicationProfile app)
        {
            throw new System.NotImplementedException();
        }

        public ScanStatus GetScanStatus(string scan)
        {
            throw new System.NotImplementedException();
        }

        public User[] GetUserEmailsOnTeam(ApplicationProfile app)
        {
            throw new System.NotImplementedException();
        }

        public bool HasAppChanged(ApplicationProfile app)
        {
            throw new System.NotImplementedException();
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
            throw new System.NotImplementedException();
        }

        public bool IsUserAssignedToTeam(User user, ApplicationProfile app)
        {
            throw new System.NotImplementedException();
        }

        public void StartPrescan(string newScan)
        {
            throw new System.NotImplementedException();
        }

        public void StartScan(string newScan)
        {
            throw new System.NotImplementedException();
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