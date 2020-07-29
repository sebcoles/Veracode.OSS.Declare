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
        List<DscApp> Apps();
        List<DscBinary> Binaries();
        List<DscModule> Modules();
        DscPolicy Policy();
        List<DscUser> Users();
    }

    public class JsonRepository : IJsonRepository
    {
        private List<DscApp> _apps;
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
            _apps = config.Apps;
            _binaries = config.Binaries;
            _modules = config.Modules;
            _policy = config.Policy;
            _users = config.Users;
        }
        public List<DscApp> Apps()
        {
            return _apps;
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
