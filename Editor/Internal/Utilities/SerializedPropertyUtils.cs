using System.Collections.Generic;
using UnityEditor;

namespace InspectorEssentials.Editor.Internal.Utilities
{
    internal static class SerializedPropertyUtils
    {
        public static IEnumerable<SerializedProperty> 
            EnumerateChildProperties(this SerializedObject serializedObject)
        {
            var iterator = serializedObject.GetIterator();
            
            if (!iterator.NextVisible(enterChildren: true))
                yield break;
            
            // yield return property; // skip "m_Script"
            while (iterator.NextVisible(enterChildren: false))
            {
                yield return iterator;
            }
        }

        public static IEnumerable<SerializedProperty> 
            EnumerateChildProperties(this SerializedProperty parentProperty)
        {
            var iterator = parentProperty.Copy();
            var end = iterator.GetEndProperty();
            
            if (!iterator.NextVisible(enterChildren: true))
                yield break;
            
            do
            {
                if (SerializedProperty.EqualContents(iterator, end))
                    yield break;

                yield return iterator;
            }
            while (iterator.NextVisible(enterChildren: false));
        }
    }
}