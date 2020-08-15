using System.Collections.Generic;

namespace VeracodeDSC.Shared
{
    public class JsonConfig
    {
        public List<ApplicationProfile> application_profiles { get; set; }       
    }

    public class Binary
    {
        public string location { get; set; }
    }

    public class Module
    {
        public Module()
        {
            messages = new List<string>();
        }
        public string module_id { get; set; }
        public string module_name { get; set; }
        public bool can_be_entry_point { get; set; }
        public bool is_entry_point { get; set; }
        public List<string> messages { get; set; }
    }

    public class CustomSeverity
    {
        public int cwe { get; set; }
        public int severity { get; set; }
    }

    public class FindingRule
    {
        public List<string> scan_type { get; set; }
        public string type { get; set; }
        public string value { get; set; }
    }

    public class ScanFrequencyRule
    {
        public string frequency { get; set; }
        public string scan_type { get; set; }
    }

    public class Policy
    {
        public List<CustomSeverity> custom_severities { get; set; }
        public List<FindingRule> finding_rules { get; set; }
        public List<ScanFrequencyRule> scan_frequency_rules { get; set; }
        public int sev0_grace_period { get; set; }
        public int sev1_grace_period { get; set; }
        public int sev2_grace_period { get; set; }
        public int sev3_grace_period { get; set; }
        public int sev4_grace_period { get; set; }
        public int sev5_grace_period { get; set; }
    }

    public class User
    {
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string email_address { get; set; }
        public string roles { get; set; }
    }

    public class ApplicationProfile
    {
        public string id { get; set; }
        public string application_name { get; set; }
        public string criticality { get; set; }
        public string business_owner { get; set; }
        public string business_owner_email { get; set; }
        public List<Binary> binaries { get; set; }
        public List<Module> modules { get; set; }
        public Policy policy { get; set; }
        public List<User> users { get; set; }
    }
}
