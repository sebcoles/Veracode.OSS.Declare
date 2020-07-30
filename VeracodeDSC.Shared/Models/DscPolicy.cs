using System;
using System.Collections.Generic;
using System.Text;

namespace VeracodeDSC.Shared.Models
{
    public class DscPolicy
    {
        public string sev0_grace_period { get; set; }
        public string sev1_grace_period { get; set; }
        public string sev2_grace_period { get; set; }
        public string sev3_grace_period { get; set; }
        public string sev4_grace_period { get; set; }
        public string sev5_grace_period { get; set; }
    }

    public class custom_severities
    {
        public string cwe { get; set; }
        public string severity { get; set; }
    }

    public class finding_rules
    {
        public string[] scan_type { get; set; }
        public string type { get; set; }
        public string value { get; set; }
    }

    public class scan_frequency_rules
    {
        public string frequency { get; set; }
        public string scan_type { get; set; }
    }
}
