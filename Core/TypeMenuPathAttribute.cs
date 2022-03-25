using System;

namespace InspectorEssentials.Core
{
    /// <summary>
    /// An attribute used on classes and structs, which overrides their menu path
    /// in type dropdowns, seen in <see cref="InlineAttribute"/> and
    /// <see cref="SerializeReferenceSelectorAttribute"/>.
    /// </summary>
    /// <seealso cref="InlineAttribute"/>
    /// <seealso cref="SerializeReferenceSelectorAttribute"/>
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