using System;
using System.Collections.Generic;
using Tools.RegistrySerialization;
using System.Text;
using System.Threading.Tasks;

namespace Tools
{
    [RegistryDataContract]
    public sealed class AccountSettings
    {
        public AccountSettings() { Name = ""; }

        public AccountSettings(string name)
        {
            Name = name;
        }

        [RegistryDataMember]
        public string Name { get; set;}
    }
}
