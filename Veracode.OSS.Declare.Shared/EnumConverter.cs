using System;
using System.Collections.Generic;
using System.Text;
using Veracode.OSS.Wrapper.Enums;

namespace Veracode.OSS.Declare.Shared
{
    public static class EnumConverter
    {
        public static Roles Convert(string role)
        {
            var stripped = role.ToLower().Replace(" ", "");
            switch (stripped)
            {
                case "administrator":
                    return Roles.Administrator;
                case "creator":
                    return Roles.Creator;
                case "executive":
                    return Roles.Executive;
                case "mitigationapprover":
                    return Roles.MitigationApprover;
                case "policyadministrator":
                    return Roles.PolicyAdministrator;
                case "reviewer":
                    return Roles.Reviewer;
                case "securitylead":
                    return Roles.SecurityLead;
                case "submitter":
                    return Roles.Submitter;
                case "securityinsights":
                    return Roles.SecurityInsights;
                case "elearning":
                    return Roles.Elearning;
                case "manualscan":
                    return Roles.ManualScan;
                case "dynamicscan":
                    return Roles.DynamicScan;
                case "staticscan":
                    return Roles.StaticScan;
                case "anyscan":
                    return Roles.AnyScan;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
