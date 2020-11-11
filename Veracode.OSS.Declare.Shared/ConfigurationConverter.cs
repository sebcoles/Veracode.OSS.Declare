using System;
using System.Collections.Generic;
using System.Text;
using Veracode.OSS.Declare.Configuration.Models;
using Veracode.OSS.Wrapper;

namespace Veracode.OSS.Declare.Shared
{
    public static class ConfigurationConverter
    {
        public static Wrapper.Rest.FindingRule Convert(FindingRule rule)
        {
            return new Wrapper.Rest.FindingRule
            {
                scan_type = rule.scan_type,
                type = rule.type,
                value = rule.value
            };
        }
        public static Wrapper.Rest.CustomSeverity Convert(CustomSeverity rule)
        {
            return new Wrapper.Rest.CustomSeverity
            {
                cwe = rule.cwe,
                severity = rule.severity
            };
        }

        public static Wrapper.Rest.ScanFrequencyRule Convert(ScanFrequencyRule rule)
        {
            return new Wrapper.Rest.ScanFrequencyRule
            {
                frequency = rule.frequency,
                scan_type = rule.scan_type
            };
        }
    }
}
