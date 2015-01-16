using System;
using System.Collections.Generic;
using Tools.RegistrySerialization;
using Tools;
using Tools.Tests.RegistrySerialization;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var test = new RegistrySerializerTests();

            test.Serialize_SuccessTest();
            test.Serialize_ClassWithoutAttribute_Throws();
            test.Serialize_WrongTypeProperty_Throws();
            test.Serialize_PropertiesWithoutAttribute_Throws();

            test.Deserialize_SuccessTest();
            test.Deserialize_ClassWithoutAttribute_Throws();
            test.Deserialize_ClassWasNotSerialized_Throws();
            test.Deserialize_WrongTypeProperty_Throws();
            test.Deserialize_PropertiesWithoutAttribute_Throws();
            test.Deserialize_NoValuePropertiesTest();

            //var accset = new AccountSettings("Anna");
            //var serconset = new ServerConnectionSettings("My IP Address", 2356);
            //var regser = new RegistrySerializer("TheGame");

            //regser.Serialize(accset);
            //regser.Serialize(serconset);

            //var accsetNEW = (AccountSettings)regser.Deserialize(typeof(AccountSettings));
            //var serconsetNEW = (ServerConnectionSettings)regser.Deserialize(typeof(ServerConnectionSettings));
            //Console.WriteLine(accsetNEW.Name);
            //Console.WriteLine(serconsetNEW.IpAddress);
            //Console.WriteLine(serconsetNEW.Port);
        }
    }
}
