using System;
using System.Collections.Generic;
using System.Text;

namespace VeracodeDSC.Shared.Models
{
    public class DscApp
    {
        public string application_name { get; set; }
        public string criticality { get; set; }
        public string business_owner { get; set; }
        public string business_owner_email { get; set; }
    }
}
