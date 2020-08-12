using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using VeracodeDSC.Shared;
using VeracodeDSC.Shared.Models;

namespace VeracodeDSC.DataAccess.Json
{
    public interface IJsonRepository
    {
        List<DscApp> Apps();       
    }

    public class JsonRepository : IJsonRepository
    {
        private DscApp[] _apps;

        public JsonRepository(string filePath)
        {
            JsonConfig config;
            using (StreamReader r = new StreamReader(filePath))
            {
                string json = r.ReadToEnd();
                config = JsonConvert.DeserializeObject<JsonConfig>(json);
            }
            _apps = config.application_profiles;
        }
        public List<DscApp> Apps()   
        {
            return _apps.ToList();
        }
    }
}
