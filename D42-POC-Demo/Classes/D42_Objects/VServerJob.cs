using D42_POC_Demo.Classes.D42;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D42_POC_Demo.Classes
{
    public class VServerJob : DiscoveryJob
    {
        protected string platform { get; set; }
        public string discover_vms { get; set; }
    }
}
