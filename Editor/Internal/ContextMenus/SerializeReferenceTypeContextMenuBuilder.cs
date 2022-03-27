using System;
using System.Reflection;
using UnityEditor;

namespace Gadget.Editor.Internal.ContextMenus
{
    internal class SerializeReferenceTypeContextMenuBuilder : TypeContextMenuBuilderBase
    {
        public SerializeReferenceTypeContextMenuBuilder(
            GenericMenu menu,
            FieldInfo fieldInfo,
            SerializedProperty property,
            bool useTypeFullName) : base(menu, fieldInfo, property, useTypeFullName) {}
        
        protected override void OnChoose(Type type)
        {
            var instance = Activator.CreateInstance(type);
            Property.managedReferenceValue = instance;
            Property.serializedObject.ApplyModifiedProperties();
        }
    }
}