using System;
using System.Globalization;
using Microsoft.Win32;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Security.AccessControl;

namespace Tools.RegistrySerialization
{
    public sealed class RegistrySerializer : ISerializer
    {
        private string _keyName;

        public RegistrySerializer(string keyName)
        {
            _keyName = keyName;
        }

        public string KeyName { get { return _keyName; } }

        public void Serialize(object obj)
        {
            Type objType = obj.GetType();

            Object[] objAttributes = objType.GetCustomAttributes(typeof(RegistryDataContractAttribute), false);

            if (objAttributes.Length == 0)
            {
                throw new InvalidOperationException("Error! You cannot serialize the instance of this class!");
            }

            PropertyInfo[] objPropeties = objType.GetProperties();

            Attribute objPropertyAttribute = null;
            bool noPropertyWithAttribute = true;

            foreach (var objPropety in objPropeties)
            {
                objPropertyAttribute = objPropety.GetCustomAttribute(typeof(RegistryDataMemberAttribute), false);

                if (objPropertyAttribute == null)
                {
                    continue;
                }

                noPropertyWithAttribute = false;

                var propertyType = objPropety.PropertyType;

                using (var rootKey = Registry.LocalMachine.OpenSubKey("SOFTWARE", RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.FullControl))
                {
                    using (var mainKey = rootKey.CreateSubKey(KeyName, RegistryKeyPermissionCheck.ReadWriteSubTree))
                    {
                        var subKey = mainKey.OpenSubKey(objType.Name, RegistryKeyPermissionCheck.ReadWriteSubTree);
                        if (subKey != null)
                        {
                            try { mainKey.DeleteSubKey(objType.Name); }
                            finally { subKey.Close(); }
                        }

                        using (subKey = mainKey.CreateSubKey(objType.Name, RegistryKeyPermissionCheck.ReadWriteSubTree))
                        {
                            if (propertyType == typeof(string))
                            {
                                subKey.SetValue(objPropety.Name, (string)objPropety.GetValue(obj), RegistryValueKind.String);
                            }
                            else if (propertyType == typeof(int))
                            {
                                subKey.SetValue(objPropety.Name, (int)objPropety.GetValue(obj), RegistryValueKind.DWord);
                            }
                            else
                            {
                                throw new InvalidOperationException(string.Format("Error! You cannot serialize a property of type '{0}'", propertyType.Name));
                            }
                        }
                    }
                }
            }

            if (noPropertyWithAttribute)
            {
                throw new InvalidOperationException("Error! You cannot serialize properties of this class!");
            }
        }

        public object Deserialize(Type objType)
        {
            object obj = Activator.CreateInstance(objType);

            Object[] objAttributes = objType.GetCustomAttributes(typeof(RegistryDataContractAttribute), false);

            if (objAttributes.Length == 0)
            {
                throw new InvalidOperationException("Error! You cannot deserialize the instance of this class!");
            }

            PropertyInfo[] objPropeties = objType.GetProperties();

            Attribute objPropertyAttribute = null;
            bool noPropertyWithAttribute = true;

            foreach (var objPropety in objPropeties)
            {
                objPropertyAttribute = objPropety.GetCustomAttribute(typeof(RegistryDataMemberAttribute), false);

                if (objPropertyAttribute == null)
                {
                    continue;
                }

                noPropertyWithAttribute = false;

                var propertyType = objPropety.PropertyType;

                using (var rootKey = Registry.LocalMachine.OpenSubKey("SOFTWARE", RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.FullControl))
                {
                    using (var mainKey = rootKey.OpenSubKey(KeyName, RegistryKeyPermissionCheck.ReadWriteSubTree))
                    {
                        if (mainKey == null)
                        {
                            throw new InvalidOperationException("Error! The instance of this class wasn't serialized!");
                        }

                        using (var subKey = mainKey.OpenSubKey(objType.Name, RegistryKeyPermissionCheck.ReadWriteSubTree))
                        {
                            if (subKey == null)
                            {
                                throw new InvalidOperationException("Error! The instance of this class wasn't serialized!");
                            }

                            if (propertyType == typeof(string))
                            {
                                objPropety.SetValue(obj, (string)subKey.GetValue(objPropety.Name, ""));
                            }
                            else if (propertyType == typeof(int))
                            {
                                objPropety.SetValue(obj, (int)subKey.GetValue(objPropety.Name, -1));
                            }
                            else
                            {
                                throw new InvalidOperationException(string.Format("Error! You cannot serialize a property of type '{0}'", propertyType.Name));
                            }
                        }
                    }
                }
            }

            if (noPropertyWithAttribute)
            {
                throw new InvalidOperationException("Error! You cannot deserialize properties of this class!");
            }

            return obj;
        }
    }
}
