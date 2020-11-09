using Newtonsoft.Json;
using System.Collections.Generic;
using Veracode.OSS.Wrapper.Rest;

namespace Veracode.OSS.Declare.Shared
{
    public class JsonConfig
    {
        public List<ApplicationProfile> application_profiles { get; set; }
    }

    public class File
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
        public bool entry_point { get; set; }
        public List<string> messages { get; set; }
    }

    public class Policy
    {
        public string name { get; set; }
        public int sca_blacklist_grace_period { get; set; }

        public List<CustomSeverity> custom_severities { get; set; }
        public List<FindingRule> finding_rules { get; set; }
        public List<ScanFrequencyRule> scan_frequency_rules { get; set; }
        public int sev0_grace_period { get; set; }
        public int sev1_grace_period { get; set; }
        public int sev2_grace_period { get; set; }
        public int sev3_grace_period { get; set; }
        public int sev4_grace_period { get; set; }
        public int sev5_grace_period { get; set; }
        public string description { get; set; }
        public int score_grace_period { get; set; }
    }

    public class User
    {
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string email_address { get; set; }
        public string roles { get; set; }
        public string teams { get; set; }
    }

    public class ApplicationProfile
    {
        public string id { get; set; }
        public string policy_schedule { get; set; }
        public string application_name { get; set; }
        public string criticality { get; set; }
        public string business_owner { get; set; }
        public string business_owner_email { get; set; }
        public List<File> files { get; set; }
        public List<Module> modules { get; set; }
        public Policy policy { get; set; }
        public List<User> users { get; set; }
        public List<Mitigation> mitigations { get; set; }
        public List<Sandbox> sandboxes { get; set; }
    }
    public class Sandbox
    {
        public string sandbox_id { get; set; }
        public string sandbox_name { get; set; }
    }
    public class Mitigation
    {
        public string flaw_id { get; set; }
        public string cve_id { get; set; }
        public string file_name { get; set; }
        public string line_number { get; set; }
        public string link { get; set; }
        public string action { get; set; }
        public string technique { get; set; }
        public string specifics { get; set; }
        public string remaining_risk { get; set; }
        public string verification { get; set; }
        public string tsrv => $"\rTechnique : {technique}\r\nSpecifics : {specifics}\r\nRemaining Risk : {remaining_risk}\r\nVerification : {verification}";
    }
}
