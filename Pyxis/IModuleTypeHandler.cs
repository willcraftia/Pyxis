#region Using

using System;
using System.Reflection;

#endregion

namespace Pyxis
{
    public interface IModuleTypeHandler
    {
        PropertyInfo[] GetProperties(Type type);

        object CreateInstance(Type type);
    }
}
