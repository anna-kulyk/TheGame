using System;
using Tools;
using Tools.RegistrySerialization;
using System.Globalization;
using Microsoft.Win32;
using System.Security.AccessControl;
using NUnit.Framework;

namespace Tools.Tests.RegistrySerialization
{
    [TestFixture]
    public sealed class RegistrySerializerTests
    {
        private const string registryKey = "HKEY_LOCAL_MACHINE\\SOFTWARE\\{0}\\{1}";
        private const string Success = "Success";
        private const string Failed = "Failed";
        private const string mainKeyName = "TheGameTest";
        private RegistrySerializer regser;

        [SetUp]
        public void Init()
        {
            regser = new RegistrySerializer(mainKeyName);
        }

        [TearDown]
        public void CleanUp()
        {
            using (var rootKey = Registry.LocalMachine.OpenSubKey("SOFTWARE", RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.FullControl))
            {
                using (var mainKey = rootKey.OpenSubKey(mainKeyName, RegistryKeyPermissionCheck.ReadWriteSubTree))
                {
                    if (mainKey != null)
                    {
                        rootKey.DeleteSubKeyTree(mainKeyName);
                    }
                }                
            }
        }

        [Test]
        public void Serialize_SuccessTest()
        {
            // Arrange
            var expected = "Anna";
            var keyName = "Name";
            var accset = new AccountSettings(expected);
            var path = string.Format(CultureInfo.InvariantCulture, registryKey, mainKeyName, typeof(AccountSettings).Name);            

            // Act
            regser.Serialize(accset);
            
            // Assert
            var actual = (string)Registry.GetValue(path, keyName, "");

            Assert.AreEqual(expected, actual);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Serialize_ClassWithoutAttribute_Throws()
        {
            // Arrange
            var instance = new ClassWithoutAttribute();
            
            // Act            
            regser.Serialize(instance);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Serialize_WrongTypeProperty_Throws()
        {
            // Arrange
            var instance = new WrongTypeProperty();
            
            // Act
            regser.Serialize(instance);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Serialize_PropertiesWithoutAttribute_Throws()
        {
            // Arrange
            var instance = new PropertiesWithoutAttribute();
            
            // Act
            regser.Serialize(instance);
        }

        [Test]
        public void Deserialize_SuccessTest()
        {
            // Arrange
            var expected = "Anna";
            var accset = new AccountSettings(expected);

            // Act
            regser.Serialize(accset);
            var accsetTest = (AccountSettings)regser.Deserialize(typeof(AccountSettings));

            // Assert
            var actual = accsetTest.Name;

            Assert.AreEqual(expected, actual);  
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Deserialize_ClassWithoutAttribute_Throws()
        {            
            // Act
            var instance = (ClassWithoutAttribute)regser.Deserialize(typeof(ClassWithoutAttribute));            
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Deserialize_ClassWasNotSerialized_Throws()
        {
            // Act
            var instance = (AccountSettings)regser.Deserialize(typeof(AccountSettings));            
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Deserialize_WrongTypeProperty_Throws()
        {
            // Act
            using (RegistryKey rootKey = Registry.LocalMachine.OpenSubKey("SOFTWARE", RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.FullControl),
                               mainKey = rootKey.CreateSubKey(mainKeyName, RegistryKeyPermissionCheck.ReadWriteSubTree),
                               subKey = mainKey.CreateSubKey(typeof(WrongTypeProperty).Name, RegistryKeyPermissionCheck.ReadWriteSubTree))
            {
                var instanceTest = (WrongTypeProperty)regser.Deserialize(typeof(WrongTypeProperty));
            }      
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Deserialize_PropertiesWithoutAttribute_Throws()
        {
            // Act
            var instance = (PropertiesWithoutAttribute)regser.Deserialize(typeof(PropertiesWithoutAttribute));
        }

        [Test]
        public void Deserialize_NoValuePropertiesTest()
        {
            // Arrange
            var expectedInt = -1;
            var scset = new ServerConnectionSettings();

            // Act
            regser.Serialize(scset);
            var scsetTest = (ServerConnectionSettings)regser.Deserialize(typeof(ServerConnectionSettings));

            // Assert
            var actualString = scsetTest.IpAddress;
            var actualInt = scsetTest.Port;

            Assert.IsEmpty(actualString);
            Assert.AreEqual(expectedInt, actualInt);
        }        

        private class ClassWithoutAttribute { }

        [RegistryDataContract]
        private class WrongTypeProperty
        {
            [RegistryDataMember]
            public ClassWithoutAttribute WrongType { get; set; }
        }

        [RegistryDataContract]
        private class PropertiesWithoutAttribute
        {
            public string FirstProperty { get; set; }
            public string SecondProperty { get; set; }
            public string ThirdProperty { get; set; }
        }
    }
}
