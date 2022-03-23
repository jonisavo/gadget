using System;
using System.Collections.Generic;
using System.Reflection;
using InspectorEssentials.Core;
using UnityEditor;
using UnityEngine;

namespace InspectorEssentials.Editor.Internal.ContextMenus
{
    public class TypeContextMenuBuilderBase : ContextMenuBuilder<Type>
    {
        private readonly bool _useTypeFullName;

        protected TypeContextMenuBuilderBase(bool useTypeFullName)
        {
            _useTypeFullName = useTypeFullName;
        }
        
        protected override string BuildMenuPath(Type type)
        {
            var attribute =
                type.GetCustomAttribute(typeof(TypeMenuNameAttribute));

            if (attribute is TypeMenuNameAttribute typeMenuNameAttribute)
                return typeMenuNameAttribute.MenuName;
            
            attribute =
                type.GetCustomAttribute(typeof(CreateAssetMenuAttribute));

            if (attribute is CreateAssetMenuAttribute createAssetMenuAttribute)
                return createAssetMenuAttribute.menuName;

            if (_useTypeFullName && !string.IsNullOrEmpty(type.FullName))
                return type.FullName.Replace('.', '/');

            return type.Name;
        }

        protected override void OnChoose(Type type, SerializedProperty property)
        {
            throw new NotImplementedException();
        }

        protected override void BuildEmptyMenu(GenericMenu menu)
        {
            menu.AddDisabledItem(new GUIContent("No available types"));
        }

        protected override Type[] GetChoices(FieldInfo fieldInfo, SerializedProperty property)
        {
            return TypeUtils.GetConcreteTypes(fieldInfo.FieldType);
        }

        protected override void BuildMenu(
            GenericMenu menu,
            IEnumerable<Type> items,
            FieldInfo fieldInfo,
            SerializedProperty property)
        {
            var typeName = TypeUtils.GetPrimaryConcreteTypeName(fieldInfo.FieldType);
            menu.AddDisabledItem(new GUIContent(typeName));
            
            menu.AddSeparator("");
            
            base.BuildMenu(menu, items, fieldInfo, property);
        }
    }
}