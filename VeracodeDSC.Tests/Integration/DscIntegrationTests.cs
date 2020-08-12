using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using VeracodeDSC.Shared;
using VeracodeService;
using VeracodeService.Configuration;

namespace VeracodeDSC.Tests.Integration
{
    [TestFixture]
    [Category("Integration")]
    public class DscIntegrationTests
    {
        private VeracodeConfiguration veracodeConfig = new VeracodeConfiguration();
        private IVeracodeRepository _repo;
        private JsonConfig application_profiles;
        private Random _rand = new Random();

        [OneTimeSetUp]
        public void Setup()
        {
            IConfiguration Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"appsettings.Development.json", false)
                .Build();

            Configuration.Bind("Veracode", veracodeConfig);

            application_profiles = JsonConvert.DeserializeObject<JsonConfig>(File.ReadAllText("veracode.verademo.json"));            var options = Options.Create(veracodeConfig);
            _repo = new VeracodeRepository(options);
        }

        [Test]
        public void Jam()
        {
            var jam = 1;
            jam = 2;
            Assert.AreEqual(2, jam);
        }
    }
}
