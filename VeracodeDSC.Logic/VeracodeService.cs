using VeracodeDSC.Shared.Models;

namespace VeracodeDSC
{
    public interface IVeracodeService
    {
        bool DoesAppExist(DscApp app);
        bool CreateApp(DscApp app);
        bool UpdateApp(DscApp app);
        bool DoesTeamExistForApp(DscApp app);
        bool CreateTeamForApp(DscApp app);
        bool DoesUserExist(DscUser app);
        bool CreateUser(DscUser app);
        bool UpdateUser(DscUser app);
        bool DoesPolicyExist(DscPolicy app);
        bool CreatePolicy(DscPolicy app);
        bool UpdatePolicy(DscPolicy app);
        DscScan CreateScan(DscUser app, DscBinary[] binaries);
        bool CancelScan(DscScan scan);
        DscScan GetScan(DscScan scan);
    }
    public class VeracodeService : IVeracodeService
    {
        public bool CancelScan(DscScan scan)
        {
            throw new System.NotImplementedException();
        }

        public bool CreateApp(DscApp app)
        {
            throw new System.NotImplementedException();
        }

        public bool CreatePolicy(DscPolicy app)
        {
            throw new System.NotImplementedException();
        }

        public DscScan CreateScan(DscUser app, DscBinary[] binaries)
        {
            throw new System.NotImplementedException();
        }

        public bool CreateTeamForApp(DscApp app)
        {
            throw new System.NotImplementedException();
        }

        public bool CreateUser(DscUser app)
        {
            throw new System.NotImplementedException();
        }

        public bool DoesAppExist(DscApp app)
        {
            throw new System.NotImplementedException();
        }

        public bool DoesPolicyExist(DscPolicy app)
        {
            throw new System.NotImplementedException();
        }

        public bool DoesTeamExistForApp(DscApp app)
        {
            throw new System.NotImplementedException();
        }

        public bool DoesUserExist(DscUser app)
        {
            throw new System.NotImplementedException();
        }

        public DscScan GetScan(DscScan scan)
        {
            throw new System.NotImplementedException();
        }

        public bool UpdateApp(DscApp app)
        {
            throw new System.NotImplementedException();
        }

        public bool UpdatePolicy(DscPolicy app)
        {
            throw new System.NotImplementedException();
        }

        public bool UpdateUser(DscUser app)
        {
            throw new System.NotImplementedException();
        }
    }
}