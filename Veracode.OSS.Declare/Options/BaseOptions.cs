﻿using CommandLine;
using System;
using System.Collections.Generic;
using System.Text;

namespace Veracode.OSS.Declare.Options
{
    public class BaseOptions
    {
        [Option('f', "jsonfile", Default = "veracode.json", Required = true, HelpText = "Location of JSON configuration file")]
        public string JsonFileLocation { get; set; }
    }
}