using System;
using System.Reflection;
using InspectorEssentials.Editor.Internal.Utilities;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace InspectorEssentials.Editor.Internal.ContextMenus
{
    internal class InlineTypeContextMenuBuilder : TypeContextMenuBuilderBase
    {
        public InlineTypeContextMenuBuilder(
            GenericMenu menu,
            FieldInfo fieldInfo,
            SerializedProperty property,
            bool useTypeFullName) : base(menu, fieldInfo, property, useTypeFullName) {}

        protected override void OnChoose(Type type)
        {
            if (!TryGetObjectFromType(type, out var objectReference))
                return;
            
            var serializedObject = Property.serializedObject;
            Property.objectReferenceValue = objectReference;
            Property.isExpanded = true;
            serializedObject.ApplyModifiedProperties();
        }

        private static bool TryGetObjectFromType(Type type, out Object objectReferenceValue)
        {
            if (typeof(ScriptableObject).IsAssignableFrom(type))
            {
                var success = AssetUtils.TryCreateScriptableObject(type, out var scriptableObject);
                objectReferenceValue = scriptableObject;
                return success;
            }

            if (typeof(Material).IsAssignableFrom(type))
            {
                var success = AssetUtils.TryCreateMaterial(out var material);
                objectReferenceValue = material;
                return success;
            }

            objectReferenceValue = null;

            return false;
        }
    }
}