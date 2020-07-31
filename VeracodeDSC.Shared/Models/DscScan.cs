using System;
using System.Collections.Generic;
using System.Text;

namespace VeracodeDSC.Shared.Models
{
    public class DscScan
    {
        public string build_id { get; set; }
        public DscModule[] Modules { get; set; }
        public string[] Messages { get; set; }
    }
}
