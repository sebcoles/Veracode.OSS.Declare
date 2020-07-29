using System;
using System.Collections.Generic;
using System.Text;
using VeracodeDSC.Shared.Models;

namespace VeracodeDSC.Shared
{
    public class JsonConfig
    {
        public List<DscApp> Apps { get; set; }
        public List<DscBinary> Binaries { get; set; }
        public List<DscModule> Modules { get; set; }
        public DscPolicy Policy { get; set; }
        public List<DscUser> Users { get; set; }
    }
}
