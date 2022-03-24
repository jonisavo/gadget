using System;
using InspectorEssentials.Editor.Internal.Utilities;
using UnityEditor;

namespace InspectorEssentials.Editor.Internal.ContextMenus
{
    public class InlineTypeContextMenuBuilder : TypeContextMenuBuilderBase
    {
        public InlineTypeContextMenuBuilder(bool useTypeFullName) : base(useTypeFullName) {}

        protected override void OnChoose(Type type, SerializedProperty property)
        {
            if (!ScriptableObjectUtils.TryCreateNewAsset(type, out var scriptableObject))
                return;
            
            var serializedObject = property.serializedObject;
            property.objectReferenceValue = scriptableObject;
            property.isExpanded = true;
            serializedObject.ApplyModifiedProperties();
        }
    }
}