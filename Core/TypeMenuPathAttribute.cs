using System;

namespace InspectorEssentials.Core
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
    public class TypeMenuPathAttribute : Attribute
    {
        public readonly string MenuPath;

        public TypeMenuPathAttribute(string menuPath)
        {
            MenuPath = menuPath;
        }
    }
}