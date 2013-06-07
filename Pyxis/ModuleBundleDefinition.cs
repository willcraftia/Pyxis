#region Using

using System;
using System.Xml.Serialization;

#endregion

namespace Pyxis
{
    [XmlRoot("Bundle")]
    public struct ModuleBundleDefinition
    {
        [XmlArrayItem("Module")]
        public ModuleDefinition[] Modules;
    }
}
