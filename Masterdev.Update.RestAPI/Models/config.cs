using System;
using System.ComponentModel;

namespace Masterdev.Update.RestAPI.Models
{
    public class config
    {
        [DefaultValue("http://10.0.1.172")]
        public string updateFilePath { get; set; }
        [DefaultValue("http://10.0.1.172")]
        public string updateZipFilePath { get; set; }
        public string serverFileName { get; set; }
        public string applicationName { get; set; }
        public string directoryName { get; set; }
    }
}

