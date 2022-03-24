using System;
using System.Linq;
using UnityEngine;

namespace InspectorEssentials.Editor.Internal.Utilities
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
            return TypesSupportingInlineCreation
                .Any(supportedType => supportedType.IsAssignableFrom(type));
        }
    }
}