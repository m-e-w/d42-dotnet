using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D42_POC_Demo.Classes.D42_Objects
{
    class VMwareJob : VServerJob
    {
        public string url_prefix { get; set; }
        public string port { get; set; }

        public VMwareJob()
        {
            port = "443";
            url_prefix = "https";
            discover_vms = "no";
        }
    }
}
