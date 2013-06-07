#region Using

using System;
using System.Collections.Generic;

#endregion

namespace Pyxis
{
    public sealed class ModuleFactory
    {
        ModuleInfoManager moduleInfoManager;

        Dictionary<string, object> moduleMap;

        public List<IPropertyHandler> PropertyHandlers { get; private set; }

        public object this[string moduleName]
        {
            get
            {
                if (moduleName == null) throw new ArgumentNullException("moduleName");
                if (moduleMap == null) throw new KeyNotFoundException("Invalid key: " + moduleName);
                
                return moduleMap[moduleName];
            }
        }

        public ModuleFactory(ModuleInfoManager moduleInfoManager)
        {
            if (moduleInfoManager == null) throw new ArgumentNullException("moduleInfoManager");

            this.moduleInfoManager = moduleInfoManager;

            PropertyHandlers = new List<IPropertyHandler>();
            PropertyHandlers.Add(DefaultPropertyHandler.Instance);
            PropertyHandlers.Add(new ModulePropertyHandler(this));
        }

        public bool Contains(string moduleName)
        {
            if (moduleName == null) throw new ArgumentNullException("moduleName");

            return moduleMap.ContainsKey(moduleName);
        }

        public void Clear()
        {
            moduleMap.Clear();
        }

        public void Build(ModuleBundleDefinition definition)
        {
            if (definition.Modules == null || definition.Modules.Length == 0)
                return;

            moduleMap = new Dictionary<string, object>(definition.Modules.Length);

            for (int i = 0; i < definition.Modules.Length; i++)
            {
                var moduleInfo = moduleInfoManager.GetModuleInfo(definition.Modules[i].Type);
                var moduleName = definition.Modules[i].Name;
                var module = moduleInfo.CreateInstance();
                moduleMap[moduleName] = module;
            }

            for (int i = 0; i < definition.Modules.Length; i++)
            {
                var moduleInfo = moduleInfoManager.GetModuleInfo(definition.Modules[i].Type);
                var moduleName = definition.Modules[i].Name;
                var module = moduleMap[moduleName];

                var propertyDefinitions = definition.Modules[i].Properties;
                if (propertyDefinitions == null || propertyDefinitions.Length == 0)
                    continue;

                for (int j = 0; j < propertyDefinitions.Length; j++)
                    PopulateProperty(moduleInfo, module, ref propertyDefinitions[j]);
            }

            for (int i = 0; i < definition.Modules.Length; i++)
            {
                var moduleName = definition.Modules[i].Name;
                var module = moduleMap[moduleName];

                var initializeMethodName = definition.Modules[i].InitializeMethodName;
                if (initializeMethodName != null)
                {
                    var initializeMethod = module.GetType().GetMethod(initializeMethodName);
                    initializeMethod.Invoke(module, null);
                }
            }
        }

        void PopulateProperty(ModuleInfo moduleInfo, object module, ref PropertyDefinition propertyDefinition)
        {
            var propertyName = propertyDefinition.Name;
            if (string.IsNullOrEmpty(propertyName)) return;

            var property = moduleInfo.GetProperty(propertyName);
            var propertyValue = propertyDefinition.Value;

            for (int i = 0; i < PropertyHandlers.Count; i++)
            {
                var handler = PropertyHandlers[i];
                if (handler.SetPropertyValue(module, property, propertyValue))
                    return;
            }

            throw new InvalidOperationException("Property not handled: " + propertyName);
        }
    }
}
