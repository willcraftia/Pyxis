#region Using

using System;
using System.Reflection;

#endregion

namespace Pyxis
{
    public interface ITypeHandler
    {
        PropertyInfo[] GetProperties(Type type);

        object CreateInstance(Type type);
    }
}
