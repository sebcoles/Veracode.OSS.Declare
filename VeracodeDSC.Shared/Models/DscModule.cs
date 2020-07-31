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

        public override bool Equals(Object obj)
        {
            DscModule moduleObj = obj as DscModule;
            if (moduleObj == null)
                return false;
            else
                return module_name.Equals(moduleObj.module_name)
                    && entry_point.Equals(moduleObj.entry_point);
        }

        public override int GetHashCode()
        {
            return module_name.GetHashCode();
        }
    }


}
