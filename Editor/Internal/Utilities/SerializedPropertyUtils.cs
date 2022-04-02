using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;
using UnityEditor;

namespace Gadget.Editor.Internal.Utilities
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
        
        private static void GetIndexAndNameFromArrayPathElement(string element, out string name, out int index)
        {
            var indexOfBracket = element.IndexOf("[", StringComparison.Ordinal);
            name = element.Substring(0, indexOfBracket);
            index = Convert.ToInt32(element.Substring(indexOfBracket)
                .Replace("[", "")
                .Replace("]", ""));
        }

        private static string[] GetPropertyPathElements(SerializedProperty property)
        {
            var path = property.propertyPath.Replace(".Array.data[", "[");
            return path.Split('.');
        }

        public static object GetNearestInspectedObject(SerializedProperty property)
        {
            var propertyPath = property.propertyPath;
            var indexOfLastDotInPropertyPath = propertyPath.LastIndexOf('.');

            if (indexOfLastDotInPropertyPath <= 0)
                return property.serializedObject.targetObject;

            propertyPath = propertyPath.Substring(0, indexOfLastDotInPropertyPath);
            
            return GetNearestInspectedObject(property, propertyPath);
        }

        private static object GetNearestInspectedObject(SerializedProperty property, string propertyPath)
        {
            var serializedObject = property.serializedObject;

            // we are in the root object
            if (string.IsNullOrEmpty(propertyPath))
                return serializedObject.targetObject;
            
            var baseProperty = serializedObject.FindProperty(propertyPath);
            object targetObject = null;
            
            if (baseProperty != null) 
                targetObject = GetTargetObjectOfProperty(baseProperty);

            var indexOfLastDotInPropertyPath = propertyPath.LastIndexOf('.');
            
            // if no valid object is retrieved from the current property path,
            // recurse towards the root object
            
            var propertyPathOneLayerAbove = indexOfLastDotInPropertyPath > 0
                ? propertyPath.Substring(0, indexOfLastDotInPropertyPath)
                : "";

            if (targetObject == null)
                return GetNearestInspectedObject(property, propertyPathOneLayerAbove);

            var targetObjectType = targetObject.GetType();

            if (!targetObjectType.IsSerializable)
                return GetNearestInspectedObject(property, propertyPathOneLayerAbove);
                    
            // valid object has been retrieved
            return targetObject;
        }

        /// <summary>
        /// Gets the object the property represents, i.e. its value.
        /// </summary>
        /// <param name="prop">SerializedProperty object</param>
        /// <returns>The property value</returns>
        public static object GetTargetObjectOfProperty([NotNull] SerializedProperty prop)
        {
            object targetObj = prop.serializedObject.targetObject;
            var elements = GetPropertyPathElements(prop);
            foreach (var element in elements)
            {
                if (element.Contains("["))
                {
                    GetIndexAndNameFromArrayPathElement(element, out var elementName, out var index);
                    targetObj = GetValue_Imp(targetObj, elementName, index);
                }
                else
                {
                    targetObj = GetValue_Imp(targetObj, element);
                }
            }
            return targetObj;
        }

        private const BindingFlags ValueFieldBindingFlags =
            BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;

        private const BindingFlags ValuePropertyBindingFlags =
            BindingFlags.NonPublic | BindingFlags.Public |
            BindingFlags.Instance | BindingFlags.IgnoreCase;

        private static object GetValue_Imp(object source, string name)
        {
            if (source == null)
                return null;
            var type = source.GetType();

            while (type != null)
            {
                var field = type.GetField(name, ValueFieldBindingFlags);
                if (field != null)
                    return field.GetValue(source);

                var property = type.GetProperty(name, ValuePropertyBindingFlags);
                if (property != null)
                    return property.GetValue(source, null);

                type = type.BaseType;
            }
            return null;
        }

        private static object GetValue_Imp(object source, string name, int index)
        {
            if (!(GetValue_Imp(source, name) is IEnumerable enumerable))
                return null;
            
            var enm = enumerable.GetEnumerator();

            for (var i = 0; i <= index; i++)
                if (!enm.MoveNext())
                    return null;
            
            return enm.Current;
        }
    }
}