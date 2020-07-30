using System;
using System.Collections.Generic;
using System.Text;

namespace VeracodeDSC.Shared.Models
{
    public class DscModule
    {
        public string module_id { get; set; }
        public string module_name { get; set; }
        public bool entry_point { get; set; }
    }
}
