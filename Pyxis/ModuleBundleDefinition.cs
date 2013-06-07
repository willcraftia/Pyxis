#region Using

using System;
using System.ComponentModel;
using System.Xml.Serialization;

#endregion

namespace Pyxis
{
    [XmlRoot("Bundle")]
    public struct ModuleBundleDefinition
    {
        [XmlArrayItem("Module")]
        [DefaultValue(null)]
        public ModuleDefinition[] Modules;
    }
}
