#region Using

using System;
using System.Xml.Serialization;

#endregion

namespace Pyxis
{
    public struct PropertyDefinition
    {
        [XmlAttribute]
        public string Name;

        [XmlAttribute]
        public string Value;
    }
}
