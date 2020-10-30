using CommandLine;
using System;
using System.Collections.Generic;
using System.Text;

namespace VeracodeDSC.Options
{
    [Verb("test", HelpText = "Will test that a configuration will produce a valid scan")]
    public class TestOptions : BaseOptions {

        [Option("lastscan", Default = false, Required = false, HelpText = "Location of JSON configuration file")]
        public bool LastScan { get; set; }
    }

}
