using System;
using System.Collections.Generic;
using System.Reflection;
using InspectorEssentials.Editor.Internal.Utilities;
using UnityEditor;
using UnityEngine;

namespace InspectorEssentials.Editor.Internal.ContextMenus
{
    internal class TypeContextMenuBuilderBase : ContextMenuBuilder<Type>
    {
        private readonly bool _useTypeFullName;

        protected TypeContextMenuBuilderBase(
            GenericMenu menu,
            FieldInfo fieldInfo,
            SerializedProperty property,
            bool useTypeFullName) : base(menu, fieldInfo, property)
        {
            _useTypeFullName = useTypeFullName;
        }
        
        protected override string BuildMenuPath(Type type)
        {
            var getMenuPathMode = _useTypeFullName
                ? TypeUtils.GetMenuPathMode.UseTypeFullName
                : TypeUtils.GetMenuPathMode.IgnoreTypeFullName;

            return TypeUtils.GetMenuPathForType(type, getMenuPathMode);
        }

        protected override void OnChoose(Type type)
        {
            throw new NotImplementedException();
        }

        protected override void BuildEmptyMenu()
        {
            AddTypeNameHeaderItem();
            
            Menu.AddSeparator("");
            
            Menu.AddDisabledItem(new GUIContent("No available types"));
        }

        protected override Type[] GetChoices()
        {
            return TypeUtils.GetConcreteTypes(FieldInfo.FieldType);
        }

        protected override void BuildMenu(IEnumerable<Type> items)
        {
            AddTypeNameHeaderItem();
            
            Menu.AddSeparator("");
            
            base.BuildMenu(items);
        }

        private void AddTypeNameHeaderItem()
        {
            var concreteType = TypeUtils.GetPrimaryConcreteType(FieldInfo.FieldType);

            if (concreteType.IsAbstract || concreteType.IsGenericType)
            {
                Menu.AddDisabledItem(new GUIContent(concreteType.Name));
            }
            else
            {
                Menu.AddItem(
                    new GUIContent(concreteType.Name), false,
                    () => OnChoose(FieldInfo.FieldType));
            }
        }
    }
}