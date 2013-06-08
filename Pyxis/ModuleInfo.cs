#region Using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;

#endregion

namespace Pyxis
{
    public sealed class ModuleInfo
    {
        static readonly string[] emptyPropertyNames = new string[0];

        static readonly object[] emptyDefaultValues = new object[0];

        IModuleTypeHandler typeHandler;

        PropertyInfo[] properties;

        object[] defaultValues;

        string[] propertyNames;

        public Type ModuleType { get; private set; }

        public int PropertyCount
        {
            get { return properties.Length; }
        }

        public ModuleInfo(Type moduleType, IModuleTypeHandler typeHandler)
        {
            if (moduleType == null) throw new ArgumentNullException("moduleType");
            if (typeHandler == null) throw new ArgumentNullException("typeHandler");

            ModuleType = moduleType;
            this.typeHandler = typeHandler;

            properties = typeHandler.GetProperties(moduleType);
            if (properties.Length == 0)
            {
                propertyNames = emptyPropertyNames;
                defaultValues = emptyDefaultValues;
            }
            else
            {
                propertyNames = new string[properties.Length];
                defaultValues = new object[properties.Length];
                for (int i = 0; i < properties.Length; i++)
                {
                    propertyNames[i] = properties[i].Name;

                    var defaultValueAttribute = Attribute.GetCustomAttribute(properties[i], typeof(DefaultValueAttribute)) as DefaultValueAttribute;
                    if (defaultValueAttribute != null)
                    {
                        defaultValues[i] = defaultValueAttribute.Value;
                    }
                }
            }
        }

        public object CreateInstance()
        {
            return typeHandler.CreateInstance(ModuleType);
        }

        public bool PropertyExists(string propertyName)
        {
            if (propertyName == null) throw new ArgumentNullException("propertyName");

            return -1 < GetPropertyIndex(propertyName);
        }

        public int GetPropertyIndex(string propertyName)
        {
            if (propertyName == null) throw new ArgumentNullException("propertyName");

            return Array.IndexOf(propertyNames, propertyName);
        }

        public PropertyInfo GetProperty(string propertyName)
        {
            var index = GetPropertyIndex(propertyName);
            if (index < 0)
                throw new InvalidOperationException("Property not found: " + propertyName);

            return properties[index];
        }

        public PropertyInfo GetProperty(int index)
        {
            if (index < 0 || PropertyCount <= index) throw new ArgumentOutOfRangeException("index");

            return properties[index];
        }

        public object GetDefaultValue(int index)
        {
            if (index < 0 || PropertyCount <= index) throw new ArgumentOutOfRangeException("index");

            return defaultValues[index];
        }
    }
}
