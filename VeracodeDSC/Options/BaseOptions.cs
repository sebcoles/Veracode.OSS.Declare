using CommandLine;
using System;
using System.Collections.Generic;
using System.Text;

namespace VeracodeDSC.Options
{
    public class BaseOptions
    {
        [Option('f', "jsonfile", Default = "", Required = true, HelpText = "Location of JSON configuration file")]
        public string JsonFileLocation { get; set; }
    }
}
