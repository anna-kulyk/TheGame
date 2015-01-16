using System;
using System.Collections.Generic;
using Tools.RegistrySerialization;
using System.Text;
using System.Threading.Tasks;

namespace Tools
{
    [RegistryDataContract]
    public sealed class ServerConnectionSettings
    {
        public ServerConnectionSettings() 
        {
            IpAddress = "";
            Port = -1; 
        }

        public ServerConnectionSettings(string ipAddress, int port)
        {
            IpAddress = ipAddress;
            Port = port;
        }

        [RegistryDataMember]
        public string IpAddress { get; set; }
        [RegistryDataMember]
        public int Port { get; set; }
    }
}
