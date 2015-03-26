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
            
            if (!objType.IsDefined(typeof(RegistryDataContractAttribute)))
            {
                throw new RegistrySerializationException(string.Format(RegistrySerializationLocalization.NoRegistryDataContractAttribute, objType.Name, typeof(RegistryDataContractAttribute).Name));
            }

            PropertyInfo[] objPropeties = objType.GetProperties();

            bool noPropertyWithAttribute = true;

            foreach (var objPropety in objPropeties)
            {     
                if (!objPropety.IsDefined(typeof(RegistryDataMemberAttribute)))
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
                                throw new RegistrySerializationException(string.Format(RegistrySerializationLocalization.WrongPropertyType, propertyType.Name));
                            }
                        }
                    }
                }
            }

            if (noPropertyWithAttribute)
            {
                throw new RegistrySerializationException(string.Format(RegistrySerializationLocalization.NoRegistryDataMemberAttribute, objType.Name, typeof(RegistryDataMemberAttribute).Name));
            }
        }

        public object Deserialize(Type objType)
        {
            object obj = Activator.CreateInstance(objType);

            if (!objType.IsDefined(typeof(RegistryDataContractAttribute)))
            {
                throw new RegistrySerializationException(string.Format(RegistrySerializationLocalization.NoRegistryDataContractAttribute, objType.Name, typeof(RegistryDataContractAttribute).Name));
            }

            PropertyInfo[] objPropeties = objType.GetProperties();

            bool noPropertyWithAttribute = true;

            foreach (var objPropety in objPropeties)
            {
                if (!objPropety.IsDefined(typeof(RegistryDataMemberAttribute)))
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
                            throw new RegistrySerializationException(string.Format(RegistrySerializationLocalization.ClassWasNotSerailized, objType.Name));
                        }

                        using (var subKey = mainKey.OpenSubKey(objType.Name, RegistryKeyPermissionCheck.ReadWriteSubTree))
                        {
                            if (subKey == null)
                            {
                                throw new RegistrySerializationException(string.Format(RegistrySerializationLocalization.ClassWasNotSerailized, objType.Name));
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
                                throw new RegistrySerializationException(string.Format(RegistrySerializationLocalization.WrongPropertyType, propertyType.Name));
                            }
                        }
                    }
                }
            }

            if (noPropertyWithAttribute)
            {
                throw new RegistrySerializationException(string.Format(RegistrySerializationLocalization.NoRegistryDataMemberAttribute, objType.Name, typeof(RegistryDataMemberAttribute).Name));
            }

            return obj;
        }
    }
}
