using CommandLine;
using Newtonsoft.Json;

namespace Veracode.OSS.Declare.Options
{
    public class BaseOptions
    {
        [Option('f', "jsonfile", Default = "veracode.json", Required = true, HelpText = "Location of JSON configuration file")]
        public string JsonFileLocation { get; set; }

        [Option('l', "language", Default = "en-GB", Required = false, HelpText = "Language for tool output")]
        public string Language { get; set; }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
