using System;

namespace Gadget.Core
{
    /// <summary>
    /// An attribute used on classes and structs, which overrides their menu path
    /// in type dropdowns, seen in <see cref="InlineAttribute"/> and
    /// <see cref="ReferenceTypeSelectorAttribute"/>.
    /// </summary>
    /// <seealso cref="InlineAttribute"/>
    /// <seealso cref="ReferenceTypeSelectorAttribute"/>
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