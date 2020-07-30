using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using VeracodeDSC.Shared;
using VeracodeDSC.Shared.Models;

namespace VeracodeDSC.DataAccess.Json
{
    public interface IJsonRepository
    {
        DscApp App();
        List<DscBinary> Binaries();
        List<DscModule> Modules();
        DscPolicy Policy();
        List<DscUser> Users();
    }

    public class JsonRepository : IJsonRepository
    {
        private DscApp _app;
        private List<DscBinary> _binaries;
        private List<DscModule> _modules;
        private DscPolicy _policy;
        private List<DscUser> _users;

        public JsonRepository(string filePath)
        {
            JsonConfig config;
            using (StreamReader r = new StreamReader(filePath))
            {
                string json = r.ReadToEnd();
                config = JsonConvert.DeserializeObject<JsonConfig>(json);
            }
            _app = config.app;
            _binaries = config.binaries;
            _modules = config.modules;
            _policy = config.policy;
            _users = config.users;
        }
        public DscApp App()
        {
            return _app;
        }

        public List<DscBinary> Binaries()
        {
            return _binaries;
        }

        public List<DscModule> Modules()
        {
            return _modules;
        }

        public DscPolicy Policy()
        {
            return _policy;
        }

        public List<DscUser> Users()
        {
            return _users;
        }
    }
}
