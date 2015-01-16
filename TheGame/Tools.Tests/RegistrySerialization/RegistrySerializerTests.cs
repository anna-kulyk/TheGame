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

        [Test]
        public void Serialize_SuccessTest()
        {
            // Arrange
            var expected = "Anna";
            var mainKeyName = "TheGameTest";

            var accset = new AccountSettings(expected);
            var regser = new RegistrySerializer(mainKeyName);
            var path = string.Format(CultureInfo.InvariantCulture, registryKey, mainKeyName, typeof(AccountSettings).Name);
            var keyName = "Name";

            // Act
            regser.Serialize(accset);
            
            // Assert
            var actual = (string)Registry.GetValue(path, keyName, "");

            Assert.AreEqual(expected, actual);

            //Console.Write("Serialize_SuccessTest(): ");
            //var initialColor = Console.ForegroundColor;
            //if (actual == expected)
            //{                
            //    Console.ForegroundColor = ConsoleColor.Green;
            //    Console.WriteLine(Success);
            //}
            //else
            //{
            //    Console.ForegroundColor = ConsoleColor.Red;
            //    Console.WriteLine(Failed);
            //}

            //Console.ForegroundColor = initialColor;

            CleanUp(mainKeyName);   
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Serialize_ClassWithoutAttribute_Throws()
        {
            // Arrange
            var mainKeyName = "TheGameTest";
            var instance = new ClassWithoutAttribute();
            var regser = new RegistrySerializer(mainKeyName);
            //var exceptionCatched = false;
            //var initialColor = Console.ForegroundColor;

            // Act
            //try
            //{
            //    regser.Serialize(instance);
            //}
            //catch (InvalidOperationException)
            //{
            //    exceptionCatched = true;
            //}

            regser.Serialize(instance);

            //Assert
            //Console.Write("Serialize_ClassWithoutAttribute_Throws(): ");
            //if (exceptionCatched)
            //{
            //    Console.ForegroundColor = ConsoleColor.Green;
            //    Console.WriteLine(Success);
            //}
            //else
            //{
            //    Console.ForegroundColor = ConsoleColor.Red;
            //    Console.WriteLine(Failed);
            //}
            //Console.ForegroundColor = initialColor;
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Serialize_WrongTypeProperty_Throws()
        {
            // Arrange
            var mainKeyName = "TheGameTest";
            var instance = new WrongTypeProperty();
            var regser = new RegistrySerializer(mainKeyName);
            //var exceptionCatched = false;
            //var initialColor = Console.ForegroundColor;

            // Act
            //try
            //{
            regser.Serialize(instance);
            //}
            //catch (InvalidOperationException)
            //{
            //    exceptionCatched = true;
            //}

            //Assert
            //Console.Write("Serialize_WrongTypeProperty_Throws(): ");
            //if (exceptionCatched)
            //{
            //    Console.ForegroundColor = ConsoleColor.Green;
            //    Console.WriteLine(Success);
            //}
            //else
            //{
            //    Console.ForegroundColor = ConsoleColor.Red;
            //    Console.WriteLine(Failed);
            //}
            //Console.ForegroundColor = initialColor;

            CleanUp(mainKeyName);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Serialize_PropertiesWithoutAttribute_Throws()
        {
            // Arrange
            var mainKeyName = "TheGameTest";
            var instance = new PropertiesWithoutAttribute();
            var regser = new RegistrySerializer(mainKeyName);
            //var exceptionCatched = false;
            //var initialColor = Console.ForegroundColor;

            // Act
            //try
            //{
            regser.Serialize(instance);
            //}
            //catch (InvalidOperationException)
            //{
            //    exceptionCatched = true;
            //}

            //Assert
            //Console.Write("Serialize_PropertiesWithoutAttribute_Throws(): ");
            //if (exceptionCatched)
            //{
            //    Console.ForegroundColor = ConsoleColor.Green;
            //    Console.WriteLine(Success);
            //}
            //else
            //{
            //    Console.ForegroundColor = ConsoleColor.Red;
            //    Console.WriteLine(Failed);
            //}
            //Console.ForegroundColor = initialColor;
        }

        [Test]
        public void Deserialize_SuccessTest()
        {
            // Arrange
            var expected = "Anna";
            var mainKeyName = "TheGameTest";
            var accset = new AccountSettings(expected);
            var regser = new RegistrySerializer(mainKeyName);
            //var initialColor = Console.ForegroundColor;

            // Act
            regser.Serialize(accset);
            var accsetTest = (AccountSettings)regser.Deserialize(typeof(AccountSettings));

            // Assert
            var actual = accsetTest.Name;

            Assert.AreEqual(expected, actual);
            //Console.Write("Deserialize_SuccessTest(): ");            
            //if (actual == expected)
            //{
            //    Console.ForegroundColor = ConsoleColor.Green;
            //    Console.WriteLine(Success);
            //}
            //else
            //{
            //    Console.ForegroundColor = ConsoleColor.Red;
            //    Console.WriteLine(Failed);
            //}

            //Console.ForegroundColor = initialColor;

            CleanUp(mainKeyName);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Deserialize_ClassWithoutAttribute_Throws()
        {
            // Arrange
            var mainKeyName = "TheGameTest";
            var regser = new RegistrySerializer(mainKeyName);
            //var exceptionCatched = false;
            //var initialColor = Console.ForegroundColor;

            // Act
            //try
            //{
            var instance = (ClassWithoutAttribute)regser.Deserialize(typeof(ClassWithoutAttribute));
            //}
            //catch (InvalidOperationException)
            //{
            //    exceptionCatched = true;
            //}

            //Assert
            //Console.Write("Deserialize_ClassWithoutAttribute_Throws(): ");
            //if (exceptionCatched)
            //{
            //    Console.ForegroundColor = ConsoleColor.Green;
            //    Console.WriteLine(Success);
            //}
            //else
            //{
            //    Console.ForegroundColor = ConsoleColor.Red;
            //    Console.WriteLine(Failed);
            //}
            //Console.ForegroundColor = initialColor;
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Deserialize_ClassWasNotSerialized_Throws()
        {
            // Arrange
            var mainKeyName = "TheGameTest";
            var regser = new RegistrySerializer(mainKeyName);
            //var exeptionCatched = false;
            //var initialColor = Console.ForegroundColor;

            // Act
            //try
            //{
            var instance = (AccountSettings)regser.Deserialize(typeof(AccountSettings));
            //}
            //catch (InvalidOperationException)
            //{
            //    exeptionCatched = true;
            //}

            //Assert
            //Console.Write("Deserialize_ClassWasNotSerialized_Throws(): ");
            //if (exeptionCatched)
            //{
            //    Console.ForegroundColor = ConsoleColor.Green;
            //    Console.WriteLine(Success);
            //}
            //else
            //{
            //    Console.ForegroundColor = ConsoleColor.Red;
            //    Console.WriteLine(Failed);
            //}
            //Console.ForegroundColor = initialColor;
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Deserialize_WrongTypeProperty_Throws()
        {
            // Arrange
            var mainKeyName = "TheGameTest";
            var regser = new RegistrySerializer(mainKeyName);
            //var exeptionCatched = false;
            //var initialColor = Console.ForegroundColor;

            // Act
            //try
            //{
            using (RegistryKey rootKey = Registry.LocalMachine.OpenSubKey("SOFTWARE", RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.FullControl),
                               mainKey = rootKey.CreateSubKey(mainKeyName, RegistryKeyPermissionCheck.ReadWriteSubTree),
                               subKey = mainKey.CreateSubKey(typeof(WrongTypeProperty).Name, RegistryKeyPermissionCheck.ReadWriteSubTree))
            {
                var instanceTest = (WrongTypeProperty)regser.Deserialize(typeof(WrongTypeProperty));
            }                
            //}
            //catch (InvalidOperationException)
            //{
            //    exeptionCatched = true;
            //}

            //Assert
            //Console.Write("Deserialize_WrongTypeProperty_Throws(): ");
            //if (exeptionCatched)
            //{
            //    Console.ForegroundColor = ConsoleColor.Green;
            //    Console.WriteLine(Success);
            //}
            //else
            //{
            //    Console.ForegroundColor = ConsoleColor.Red;
            //    Console.WriteLine(Failed);
            //}
            //Console.ForegroundColor = initialColor;

            CleanUp(mainKeyName);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Deserialize_PropertiesWithoutAttribute_Throws()
        {
            // Arrange
            var mainKeyName = "TheGameTest";
            var regser = new RegistrySerializer(mainKeyName);
            //var exeptionCatched = false;
            //var initialColor = Console.ForegroundColor;

            // Act
            //try
            //{
            var instance = (PropertiesWithoutAttribute)regser.Deserialize(typeof(PropertiesWithoutAttribute));
            //}
            //catch (InvalidOperationException)
            //{
            //    exeptionCatched = true;
            //}

            //Assert
            //Console.Write("Deserialize_PropertiesWithoutAttribute_Throws(): ");
            //if (exeptionCatched)
            //{
            //    Console.ForegroundColor = ConsoleColor.Green;
            //    Console.WriteLine(Success);
            //}
            //else
            //{
            //    Console.ForegroundColor = ConsoleColor.Red;
            //    Console.WriteLine(Failed);
            //}
            //Console.ForegroundColor = initialColor;
        }

        [Test]
        public void Deserialize_NoValuePropertiesTest()
        {
            // Arrange
            //var expectedString = "";
            var expectedInt = -1;
            var mainKeyName = "TheGameTest";
            var scset = new ServerConnectionSettings();
            var regser = new RegistrySerializer(mainKeyName);
            //var initialColor = Console.ForegroundColor;

            // Act
            regser.Serialize(scset);
            var scsetTest = (ServerConnectionSettings)regser.Deserialize(typeof(ServerConnectionSettings));

            // Assert
            var actualString = scsetTest.IpAddress;
            var actualInt = scsetTest.Port;

            Assert.IsEmpty(actualString);
            Assert.AreEqual(expectedInt, actualInt);
            //Console.Write("Deserialize_NoValuePropertiesTest(): ");
            //if (actualString == expectedString && actualInt == expectedInt)
            //{
            //    Console.ForegroundColor = ConsoleColor.Green;
            //    Console.WriteLine(Success);
            //}
            //else
            //{
            //    Console.ForegroundColor = ConsoleColor.Red;
            //    Console.WriteLine(Failed);
            //}

            //Console.ForegroundColor = initialColor;

            CleanUp(mainKeyName);
        }

        private void CleanUp(string mainKeyName)
        {
            using (var rootKey = Registry.LocalMachine.OpenSubKey("SOFTWARE", RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.FullControl))
            {
                rootKey.DeleteSubKeyTree(mainKeyName);
            } 
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
