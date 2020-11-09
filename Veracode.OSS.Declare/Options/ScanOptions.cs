using CommandLine;

namespace Veracode.OSS.Declare.Options
{
    [Verb("scan", HelpText = "This will run a policy scan if the config if valid")]
    public class ScanOptions : BaseOptions
    {
        [Option('i',"ignore_schedule", Default = false, Required = false, HelpText = "Use this option to override schedule in configuration")]
        public bool IgnoreSchedule { get; set; }
    }

}
