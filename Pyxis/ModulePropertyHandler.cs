#region Using

using System;
using System.Reflection;

#endregion

namespace Pyxis
{
    public sealed class ModulePropertyHandler : IModulePropertyHandler
    {
        ModuleFactory moduleFactory;

        public ModulePropertyHandler(ModuleFactory moduleFactory)
        {
            if (moduleFactory == null) throw new ArgumentNullException("moduleFactory");

            this.moduleFactory = moduleFactory;
        }

        public bool SetPropertyValue(object module, PropertyInfo property, string propertyValue)
        {
            if (propertyValue == null) return false;

            if (moduleFactory.Contains(propertyValue))
            {
                var propertyType = property.PropertyType;
                var referencedModule = moduleFactory[propertyValue];

                if (propertyType.IsAssignableFrom(referencedModule.GetType()))
                {
                    property.SetValue(module, referencedModule, null);
                    return true;
                }
            }

            return false;
        }
    }
}
