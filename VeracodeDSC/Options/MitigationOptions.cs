using CommandLine;

namespace Veracode.OSS.Declare.Options
{
    [Verb("mitigation", HelpText = "This will generate templates")]
    public class MitigationOptions : BaseOptions
    {
        [Option("policyonly", Default = true, Required = true, HelpText = "Location of JSON configuration file")]
        public bool PolicyOnly { get; set; }
    }
}
