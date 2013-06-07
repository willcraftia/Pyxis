#region Using

using System;
using System.Reflection;

#endregion

namespace Pyxis
{
    public interface IPropertyStringfier
    {
        bool CanConvertToString(object module, PropertyInfo property, object propertyValue);

        bool ConvertToString(object module, PropertyInfo property, object propertyValue, out string stringValue);
    }
}
