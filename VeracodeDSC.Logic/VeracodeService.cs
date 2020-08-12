using System.IO;
using System.Threading.Tasks;
using VeracodeDSC.Shared.Enums;
using VeracodeDSC.Shared.Models;
using VeracodeService;

namespace VeracodeDSC
{
    public interface IVeracodeService
    {
        bool DoesAppExist(DscApp app);
        bool HasAppChanged(DscApp app);
        bool CreateApp(DscApp app);
        bool UpdateApp(DscApp app);
        bool DoesTeamExistForApp(DscApp app);
        bool CreateTeamForApp(DscApp app);
        bool IsUserAssignedToTeam(DscUser user, DscApp app);
        bool DoesUserExist(DscUser user);
        bool HasUserChanged(DscUser user);
        bool CreateUser(DscUser user);
        bool UpdateUser(DscUser user);
        bool DoesPolicyExist(DscApp app);
        bool CreatePolicy(DscApp app, DscPolicy policy);
        bool UpdatePolicy(DscApp app, DscPolicy policy);
        bool HasPolicyChanged(DscApp app, DscPolicy policy);
        DscScan CreateScan(DscApp app);
        ScanStatus GetScanStatus(DscScan scan);
        bool CancelScan(DscScan scan);
        bool IsPolicyScanInProgress(DscApp app);
        DscScan GetScan(DscApp app);
        void AddBinaryToScan(DscScan newScan, FileStream fileStream);
        void StartPrescan(DscScan newScan);
        PreScanStatus GetPrescanStatus(DscScan newScan);
        DscModule[] GetModules(DscScan newScan);
        void StartScan(DscScan newScan);
        void DeleteScan(DscScan newScan);
    }
    public class VeracodeService : IVeracodeService
    {
        private IVeracodeRepository _veracodeRepository;
        public VeracodeService(IVeracodeRepository veracodeRepository)
        {
            _veracodeRepository = veracodeRepository;
        }
        public void AddBinaryToScan(DscScan newScan, FileStream fileStream)
        {
            throw new System.NotImplementedException();
        }

        public bool CancelScan(DscScan scan)
        {
            throw new System.NotImplementedException();
        }

        public bool CreateApp(DscApp app)
        {
            throw new System.NotImplementedException();
        }

        public bool CreatePolicy(DscApp app, DscPolicy policy)
        {
            throw new System.NotImplementedException();
        }

        public DscScan CreateScan(DscApp app)
        {
            throw new System.NotImplementedException();
        }

        public bool CreateTeamForApp(DscApp app)
        {
            throw new System.NotImplementedException();
        }

        public bool CreateUser(DscUser user)
        {
            throw new System.NotImplementedException();
        }

        public void DeleteScan(DscScan newScan)
        {
            throw new System.NotImplementedException();
        }

        public bool DoesAppExist(DscApp app)
        {
            throw new System.NotImplementedException();
        }

        public bool DoesPolicyExist(DscApp app)
        {
            throw new System.NotImplementedException();
        }

        public bool DoesTeamExistForApp(DscApp app)
        {
            throw new System.NotImplementedException();
        }

        public bool DoesUserExist(DscUser user)
        {
            throw new System.NotImplementedException();
        }

        public DscModule[] GetModules(DscScan newScan)
        {
            throw new System.NotImplementedException();
        }

        public PreScanStatus GetPrescanStatus(DscScan newScan)
        {
            throw new System.NotImplementedException();
        }

        public DscScan GetScan(DscApp app)
        {
            throw new System.NotImplementedException();
        }

        public ScanStatus GetScanStatus(DscScan scan)
        {
            throw new System.NotImplementedException();
        }

        public bool HasAppChanged(DscApp app)
        {
            throw new System.NotImplementedException();
        }

        public bool HasPolicyChanged(DscApp app, DscPolicy policy)
        {
            throw new System.NotImplementedException();
        }

        public bool HasUserChanged(DscUser user)
        {
            throw new System.NotImplementedException();
        }

        public bool IsPolicyScanInProgress(DscApp app)
        {
            throw new System.NotImplementedException();
        }

        public bool IsUserAssignedToTeam(DscUser user, DscApp app)
        {
            throw new System.NotImplementedException();
        }

        public void StartPrescan(DscScan newScan)
        {
            throw new System.NotImplementedException();
        }

        public void StartScan(DscScan newScan)
        {
            throw new System.NotImplementedException();
        }

        public bool UpdateApp(DscApp app)
        {
            throw new System.NotImplementedException();
        }

        public bool UpdatePolicy(DscApp app, DscPolicy policy)
        {
            throw new System.NotImplementedException();
        }

        public bool UpdateUser(DscUser user)
        {
            throw new System.NotImplementedException();
        }
    }
}