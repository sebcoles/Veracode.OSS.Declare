using System;
using System.Collections.Generic;
using System.Text;
using VeracodeDSC.Shared.Models;

namespace VeracodeDSC.Shared
{
    public class JsonConfig
    {
        public DscApp app { get; set; }
        public List<DscBinary> binaries { get; set; }
        public List<DscModule> modules { get; set; }
        public DscPolicy policy { get; set; }
        public List<DscUser> users { get; set; }
    }
}
