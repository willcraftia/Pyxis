#region Using

using System;
using System.Collections.Generic;
using System.ComponentModel;

#endregion

namespace Pyxis
{
    public sealed class ModuleBundleBuilder
    {
        ModuleInfoManager moduleInfoManager;

        List<object> modules = new List<object>();

        Dictionary<string, object> moduleMap = new Dictionary<string, object>();

        Dictionary<object, string> externalReferenceMap;

        Dictionary<string, string> initializeMethodNameMap = new Dictionary<string, string>();

        public List<IModulePropertyStringfier> PropertyStringfiers { get; private set; }

        public ModuleBundleBuilder(ModuleInfoManager moduleInfoManager)
        {
            if (moduleInfoManager == null) throw new ArgumentNullException("moduleInfoManager");

            this.moduleInfoManager = moduleInfoManager;

            PropertyStringfiers = new List<IModulePropertyStringfier>();
            PropertyStringfiers.Add(DefaultModulePropertyStringfier.Instance);
            PropertyStringfiers.Add(new ModulePropertyStringfier(this));
        }

        public void Add(string moduleName, object module)
        {
            if (moduleName == null) throw new ArgumentNullException("moduleName");
            if (module == null) throw new ArgumentNullException("module");
            if (moduleMap.ContainsKey(moduleName)) throw new ArgumentException("Name duplicated: " + moduleName);
            if (modules.Contains(module)) throw new ArgumentException("Module duplicated.");

            modules.Add(module);
            moduleMap[moduleName] = module;
        }

        public void AddExternalReference(object module, string uri)
        {
            if (module == null) throw new ArgumentNullException("module");
            if (uri == null) throw new ArgumentNullException("uri");

            if (externalReferenceMap == null) externalReferenceMap = new Dictionary<object, string>();
            externalReferenceMap[module] = uri;
        }

        public void SetInitializeMethodName(string moduleName, string methodName)
        {
            if (moduleName == null) throw new ArgumentNullException("moduleName");

            if (string.IsNullOrEmpty(methodName))
            {
                initializeMethodNameMap.Remove(moduleName);
            }
            else
            {
                initializeMethodNameMap[moduleName] = methodName;
            }
        }

        public string GetInitializeMethodName(string moduleName)
        {
            if (moduleName == null) throw new ArgumentNullException("moduleName");

            string result;
            initializeMethodNameMap.TryGetValue(moduleName, out result);
            return result;
        }

        public bool Contains(string moduleName)
        {
            return moduleMap.ContainsKey(moduleName);
        }

        public bool Contains(object module)
        {
            return moduleMap.ContainsValue(module);
        }

        public bool ContainsExternalReference(string uri)
        {
            return externalReferenceMap != null && externalReferenceMap.ContainsValue(uri);
        }

        public bool ContainsExternalReference(object module)
        {
            return externalReferenceMap != null && externalReferenceMap.ContainsKey(module);
        }

        public void Remove(string moduleName)
        {
            object module;
            if (moduleMap.TryGetValue(moduleName, out module))
            {
                modules.Remove(module);
                moduleMap.Remove(moduleName);
            }
        }

        public void Clear()
        {
            modules.Clear();
            moduleMap.Clear();
            if (externalReferenceMap != null) externalReferenceMap.Clear();
        }

        public string GetModuleName(object module)
        {
            foreach (var entry in moduleMap)
            {
                if (entry.Value == module)
                    return entry.Key;
            }

            throw new ArgumentException("Module not found.");
        }

        public string GetExternalReferenceName(object module)
        {
            if (externalReferenceMap == null) throw new KeyNotFoundException("External reference not found.");
            return externalReferenceMap[module];
        }

        public ModuleBundleDefinition Build()
        {
            AddNamelessModules();

            var definition = new ModuleBundleDefinition();

            if (modules.Count == 0) return definition;

            definition.Modules = new ModuleDefinition[modules.Count];
            for (int i = 0; i < modules.Count; i++)
            {
                var module = modules[i];
                var moduleInfo = moduleInfoManager.GetModuleInfo(module.GetType());

                var moduleName = GetModuleName(module);

                definition.Modules[i] = new ModuleDefinition
                {
                    Name = moduleName,
                    Type = moduleInfoManager.GetTypeDefinitionName(moduleInfo),
                    InitializeMethodName = GetInitializeMethodName(moduleName)
                };

                if (moduleInfo.PropertyCount == 0) continue;

                // TODO
                // 作業用リストはフィールドで定義する。

                var workingList = new List<ModulePropertyDefinition>();

                for (int j = 0; j < moduleInfo.PropertyCount; j++)
                {
                    var property = moduleInfo.GetProperty(j);
                    var propertyType = property.PropertyType;

                    var propertyName = property.Name;
                    var propertyValue = property.GetValue(module, null);

                    var defaultValue = moduleInfo.GetDefaultValue(j);

                    if (propertyValue == defaultValue)
                        continue;

                    if (defaultValue != null && defaultValue.Equals(propertyValue))
                        continue;

                    string stringValue = null;

                    for (int k = 0; k < PropertyStringfiers.Count; k++)
                    {
                        var stringfier = PropertyStringfiers[k];

                        if (stringfier.ConvertToString(module, property, propertyValue, out stringValue))
                            break;
                    }

                    var propertyDefinition = new ModulePropertyDefinition
                    {
                        Name = propertyName,
                        Value = stringValue
                    };
                    workingList.Add(propertyDefinition);
                }

                definition.Modules[i].Properties = workingList.ToArray();
            }

            return definition;
        }

        void AddNamelessModules()
        {
            for (int i = 0; i < modules.Count; i++)
                AddNamelessModule(modules[i]);
        }

        void AddNamelessModule(object module)
        {
            var moduleInfo = moduleInfoManager.GetModuleInfo(module.GetType());

            if (moduleInfo.PropertyCount == 0) return;

            for (int j = 0; j < moduleInfo.PropertyCount; j++)
            {
                var property = moduleInfo.GetProperty(j);
                var propertyType = property.PropertyType;

                var propertyName = property.Name;
                var propertyValue = property.GetValue(module, null);

                bool handled = false;
                for (int k = 0; k < PropertyStringfiers.Count; k++)
                {
                    var stringfier = PropertyStringfiers[k];

                    if (stringfier.CanConvertToString(module, property, propertyValue))
                    {
                        handled = true;
                        break;
                    }
                }
                if (handled) continue;

                var ownerModuleName = GetModuleName(module);
                var moduleName = ownerModuleName + "_" + propertyName;
                Add(moduleName, propertyValue);

                // Recursively
                AddNamelessModule(propertyValue);
            }
        }
    }
}
