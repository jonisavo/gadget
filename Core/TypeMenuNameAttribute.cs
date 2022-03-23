using System;

namespace InspectorEssentials.Core
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
    public class TypeMenuNameAttribute : Attribute
    {
        public readonly string MenuName;

        public TypeMenuNameAttribute(string menuName)
        {
            MenuName = menuName;
        }
    }
}