using CommandLine;

namespace Veracode.OSS.Declare.Options
{
    [Verb("mitigation", HelpText = "This will generate templates")]
    public class MitigationOptions : BaseOptions
    {
        [Option("policyonly", Default = true, Required = false, HelpText = "Only includes policy breaking mitigtions in output if set")]
        public bool PolicyOnly { get; set; }
    }
}
