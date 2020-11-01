using CommandLine;

namespace Veracode.OSS.Declare.Options
{
    [Verb("scan", HelpText = "This will run a policy scan if the config if valid")]
    public class ScanOptions : BaseOptions
    {
        [Option("scan_name", Default = "", Required = false, HelpText = "Location of JSON configuration file")]
        public string ScanName { get; set; }
    }

}
