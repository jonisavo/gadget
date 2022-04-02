using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Gadget.Editor.Internal.Utilities;
using UnityEditor;
using UnityEngine;

namespace Gadget.Editor.Internal.ContextMenus
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

        public override Type[] GetChoices()
        {
            return TypeUtils.GetConcreteTypes(FieldInfo.FieldType)
                .Where(type => type != FieldInfo.FieldType)
                .ToArray();
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
            var niceName = ObjectNames.NicifyVariableName(concreteType.Name);
            
            var guiContent = new GUIContent(niceName);

            if (concreteType.IsAbstract || concreteType.IsGenericType)
                Menu.AddDisabledItem(guiContent);
            else
                Menu.AddItem(guiContent, false, () => OnChoose(concreteType));
        }
    }
}