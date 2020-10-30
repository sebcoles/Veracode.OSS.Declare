using CommandLine;
using System;
using System.Collections.Generic;
using System.Text;

namespace VeracodeDSC.Options
{
    [Verb("mitigation", HelpText = "This will generate templates")]
    public class MitigationOptions : BaseOptions
    {
        [Option("policyonly", Default = true, Required = true, HelpText = "Location of JSON configuration file")]
        public bool PolicyOnly { get; set; }
    }
}
