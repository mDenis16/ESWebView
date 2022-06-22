using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESWebViewInternal.Configuration.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Property)
   ]
    public class ConfigItemAttribute : System.Attribute
    {
        public string DisplayName { get; set; }

        public ConfigItemAttribute(string _DisplayName)
        {
            this.DisplayName = _DisplayName;

        }
    }
}

