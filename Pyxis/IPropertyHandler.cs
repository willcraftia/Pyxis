#region Using

using System;
using System.Reflection;

#endregion

namespace Pyxis
{
    public interface IPropertyHandler
    {
        bool SetPropertyValue(object module, PropertyInfo property, string propertyValue);
    }
}
