﻿#region Using

using System;

#endregion

namespace Pyxis
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class PropertyIgnoredAttribute : Attribute
    {
    }
}
