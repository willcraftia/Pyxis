#region Using

using System;
using System.ComponentModel;
using System.Xml.Serialization;

#endregion

namespace Pyxis
{
    public struct ModulePropertyDefinition
    {
        [XmlAttribute]
        [DefaultValue(null)]
        public string Name;

        [XmlAttribute]
        [DefaultValue(null)]
        public string Value;
    }
}
