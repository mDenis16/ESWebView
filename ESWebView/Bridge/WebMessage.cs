using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESWebView.Bridge
{
    public enum MessageType {
        SAVE_SETTINGS
    }
    public class WebMessage
    {
        public MessageType Type { get; set; }
        public string PayLoad { get; set; }
    }
}
