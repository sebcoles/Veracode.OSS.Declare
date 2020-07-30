using CommandLine;
using System;
using System.Collections.Generic;
using System.Text;

namespace VeracodeDSC.Options
{
    [Verb("test", HelpText = "Will test that a configuration will produce a valid scan")]
    public class TestOptions : BaseOptions { }

}
