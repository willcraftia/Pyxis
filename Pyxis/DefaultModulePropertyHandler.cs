﻿#region Using

using System;
using System.Reflection;

#endregion

namespace Pyxis
{
    public sealed class DefaultModulePropertyHandler : IModulePropertyHandler
    {
        public static readonly DefaultModulePropertyHandler Instance = new DefaultModulePropertyHandler();

        DefaultModulePropertyHandler() { }

        public bool SetPropertyValue(object module, PropertyInfo property, string propertyValue)
        {
            var propertyType = property.PropertyType;

            if (propertyType == typeof(string))
            {
                property.SetValue(module, propertyValue, null);
                return true;
            }

            if (propertyType.IsEnum)
            {
                var convertedValue = Enum.Parse(propertyType, propertyValue, true);
                property.SetValue(module, convertedValue, null);
                return true;
            }

            if (typeof(IConvertible).IsAssignableFrom(propertyType))
            {
                var convertedValue = Convert.ChangeType(propertyValue, propertyType, null);
                property.SetValue(module, convertedValue, null);
                return true;
            }

            return false;
        }
    }
}
