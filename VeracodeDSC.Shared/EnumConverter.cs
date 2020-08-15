using System;
using System.Collections.Generic;
using System.Text;
using VeracodeService.Enums;

namespace VeracodeDSC.Shared
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
                case "Creator":
                    return Roles.Creator;
                case "Executive":
                    return Roles.Executive;
                case "MitigationApprover":
                    return Roles.MitigationApprover;
                case "PolicyAdministrator":
                    return Roles.PolicyAdministrator;
                case "Reviewer":
                    return Roles.Reviewer;
                case "SecurityLead":
                    return Roles.SecurityLead;
                case "Submitter":
                    return Roles.Submitter;
                case "SecurityInsights":
                    return Roles.SecurityInsights;
                case "Elearning":
                    return Roles.Elearning;
                case "ManualScan":
                    return Roles.ManualScan;
                case "DynamicScan":
                    return Roles.DynamicScan;
                case "StaticScan":
                    return Roles.StaticScan;
                case "AnyScan":
                    return Roles.AnyScan;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
