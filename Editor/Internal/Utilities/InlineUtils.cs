using System;
using System.Linq;
using UnityEngine;

namespace Gadget.Editor.Internal.Utilities
{
    internal static class InlineUtils
    {
        private static readonly Type[] TypesSupportingInlineCreation =
        {
            typeof(ScriptableObject),
            typeof(Material)
        };

        public static bool DoesTypeSupportInlineCreation(Type type)
        {
            var concreteType = TypeUtils.GetPrimaryConcreteType(type);
            
            return TypesSupportingInlineCreation
                .Any(supportedType => supportedType.IsAssignableFrom(concreteType));
        }
    }
}