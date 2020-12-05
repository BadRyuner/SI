﻿using System;

namespace SIQuester.Model
{
    public sealed class PackageTypeNameAttribute: Attribute
    {
        public string Name { get; set; }

        internal PackageTypeNameAttribute(string name)
        {
            Name = name;
        }
    }
}
