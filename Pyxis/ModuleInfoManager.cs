#region Using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

#endregion

namespace Pyxis
{
    public sealed class ModuleInfoManager
    {
        #region ModuleInfoCollection

        class ModuleInfoCollection : KeyedCollection<Type, ModuleInfo>
        {
            protected override Type GetKeyForItem(ModuleInfo item)
            {
                return item.ModuleType;
            }
        }

        #endregion

        static readonly TypeHandler defaultTypeHandler = new TypeHandler();

        ModuleTypeRegistory typeRegistory;

        Dictionary<Type, ITypeHandler> typeHandlerMap;

        ModuleInfoCollection moduleInfoCache = new ModuleInfoCollection();

        public ModuleInfoManager(ModuleTypeRegistory typeRegistory)
        {
            if (typeRegistory == null) throw new ArgumentNullException("typeRegistory");

            this.typeRegistory = typeRegistory;
        }

        public void AddTypeHandler(Type type, ITypeHandler typeHandler)
        {
            if (type == null) throw new ArgumentNullException("type");
            if (typeHandler == null) throw new ArgumentNullException("typeHandler");

            if (typeHandlerMap == null) typeHandlerMap = new Dictionary<Type, ITypeHandler>();
            typeHandlerMap[type] = typeHandler;
        }

        public ModuleInfo GetModuleInfo(string typeName)
        {
            if (typeName == null) throw new ArgumentNullException("typeName");

            var type = typeRegistory.GetType(typeName);
            return GetModuleInfo(type);
        }

        public ModuleInfo GetModuleInfo(Type type)
        {
            if (type == null) throw new ArgumentNullException("type");

            if (moduleInfoCache.Contains(type)) return moduleInfoCache[type];

            var typeHandler = GetTypeHandler(type);
            var moduleInfo = new ModuleInfo(type, typeHandler);
            moduleInfoCache.Add(moduleInfo);
            return moduleInfo;
        }

        public string GetTypeDefinitionName(ModuleInfo moduleInfo)
        {
            if (moduleInfo == null) throw new ArgumentNullException("moduleInfo");

            return typeRegistory.GetTypeDefinitionName(moduleInfo.ModuleType);
        }

        ITypeHandler GetTypeHandler(Type type)
        {
            ITypeHandler typeHandler;
            if (typeHandlerMap == null || !typeHandlerMap.TryGetValue(type, out typeHandler))
                typeHandler = defaultTypeHandler;

            return typeHandler;
        }
    }
}
