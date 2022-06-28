using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESWebViewInternal.Configuration.Attributes
{
    public enum Access
    {
        PUBLIC = 1,
        INTERNAL = 2,
        URL_ITEM = 4
    }

    [AttributeUsage(System.AttributeTargets.Property)]
    public class ConfigItemAttribute : System.Attribute
    {
        public string DisplayName { get; set; }
        public Access AccessType { get; set; }

        public ConfigItemAttribute(string _DisplayName, Access _AccessType = Access.PUBLIC)
        {
            this.DisplayName = _DisplayName;
            this.AccessType = _AccessType;

        }
    }
  
}

