using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using System.Text;

namespace VeracodeDSC.Options
{
    [Verb("configure", HelpText = "This will create the app profile, policy and users")]
    public class ConfigureOptions : BaseOptions { }
}
