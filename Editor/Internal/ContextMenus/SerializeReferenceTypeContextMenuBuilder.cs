using System;
using UnityEditor;

namespace InspectorEssentials.Editor.Internal.ContextMenus
{
    internal class SerializeReferenceTypeContextMenuBuilder : TypeContextMenuBuilderBase
    {
        public SerializeReferenceTypeContextMenuBuilder(bool useTypeFullName) : base(useTypeFullName) {}
        
        protected override void OnChoose(Type type, SerializedProperty property)
        {
            var instance = Activator.CreateInstance(type);
            property.managedReferenceValue = instance;
            property.serializedObject.ApplyModifiedProperties();
        }
    }
}