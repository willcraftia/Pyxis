#region Using

using System;
using System.Xml.Serialization;

#endregion

namespace Pyxis
{
    public struct ModuleDefinition
    {
        [XmlAttribute]
        public string Name;

        [XmlAttribute]
        public string Type;

        [XmlArrayItem("Property")]
        public PropertyDefinition[] Properties;

        [XmlAttribute]
        public string InitializeMethodName;
    }
}
