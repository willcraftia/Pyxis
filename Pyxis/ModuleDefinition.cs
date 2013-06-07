#region Using

using System;
using System.ComponentModel;
using System.Xml.Serialization;

#endregion

namespace Pyxis
{
    public struct ModuleDefinition
    {
        [XmlAttribute]
        [DefaultValue(null)]
        public string Name;

        [XmlAttribute]
        [DefaultValue(null)]
        public string Type;

        [XmlArrayItem("Property")]
        [DefaultValue(null)]
        public PropertyDefinition[] Properties;

        [XmlAttribute]
        [DefaultValue(null)]
        public string InitializeMethodName;
    }
}
