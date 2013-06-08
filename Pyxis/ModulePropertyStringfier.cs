#region Using

using System;
using System.Reflection;

#endregion

namespace Pyxis
{
    public sealed class ModulePropertyStringfier : IModulePropertyStringfier
    {
        ModuleBundleBuilder moduleBundleBuilder;

        public ModulePropertyStringfier(ModuleBundleBuilder moduleBundleBuilder)
        {
            if (moduleBundleBuilder == null) throw new ArgumentNullException("moduleBundleBuilder");

            this.moduleBundleBuilder = moduleBundleBuilder;
        }

        public bool CanConvertToString(object module, PropertyInfo property, object propertyValue)
        {
            if (moduleBundleBuilder.Contains(propertyValue)) return true;
            if (moduleBundleBuilder.ContainsExternalReference(propertyValue)) return true;

            return false;
        }

        public bool ConvertToString(object module, PropertyInfo property, object propertyValue, out string stringValue)
        {
            if (moduleBundleBuilder.Contains(propertyValue))
            {
                stringValue = moduleBundleBuilder.GetModuleName(propertyValue);
                return true;
            }

            if (moduleBundleBuilder.ContainsExternalReference(propertyValue))
            {
                stringValue = moduleBundleBuilder.GetExternalReferenceName(propertyValue);
                return true;
            }

            stringValue = null;
            return false;
        }
    }
}
