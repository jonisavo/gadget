using System.Collections.Generic;
using System.Reflection;
using Gadget.Core;
using Gadget.Editor.Internal.Utilities;
using UnityEditor;
using UnityEngine;

namespace Gadget.Editor.DrawerExtensions
{
    [GadgetExtensionFor(typeof(GadgetContextMenuItemAttribute))]
    public class GadgetContextMenuItemDrawerExtension : GadgetDrawerExtension
    {
        public GadgetContextMenuItemDrawerExtension(GadgetPropertyAttribute attribute) : base(attribute) {}

        private static readonly Dictionary<SerializedProperty, GenericMenu> MenuDictionary
            = new Dictionary<SerializedProperty, GenericMenu>();

        private GadgetContextMenuItemAttribute ContextMenuItemAttribute
        {
            get
            {
                if (_contextMenuItemAttribute == null)
                    _contextMenuItemAttribute = (GadgetContextMenuItemAttribute) Attribute;

                return _contextMenuItemAttribute;
            }
        }

        private GadgetContextMenuItemAttribute _contextMenuItemAttribute;

        public override void OnPreGUI(Rect position, SerializedProperty property)
        {
            GenericMenu menu;
            
            if (!MenuDictionary.ContainsKey(property))
            {
                menu = new GenericMenu();
                MenuDictionary.Add(property, menu);
            }
            else
            {
                menu = MenuDictionary[property];
            }

            if (!TryGetMethodOfProperty(property, out var methodInfo))
                return;

            if (IsInvalid(property, out _))
                return;

            menu.AddItem(new GUIContent(ContextMenuItemAttribute.MenuItemName), false,
                () => InvokeMethodOnTargetObject(property, methodInfo));
        }

        private static void InvokeMethodOnTargetObject(SerializedProperty property, MethodInfo methodInfo)
        {
            var targetObject =
                SerializedPropertyUtils.GetNearestInspectedObject(property);
            
            object[] parameters = null;

            if (methodInfo.GetParameters().Length == 1)
                parameters = new[]
                {
                    SerializedPropertyUtils.GetTargetObjectOfProperty(property)
                };

            methodInfo.Invoke(targetObject, parameters);
        }

        public override void OnPostGUI(Rect position, SerializedProperty property)
        {
            var evt = Event.current;

            if (evt.type != EventType.MouseDown || evt.button != 1)
                return;

            var contextMenuArea = position;
            contextMenuArea.width = EditorGUIUtility.labelWidth;

            if (!contextMenuArea.Contains(evt.mousePosition))
                return;
            
            if (MenuDictionary.ContainsKey(property) && MenuDictionary[property].GetItemCount() > 0)
                MenuDictionary[property].ShowAsContext();
            
            evt.Use();
        }

        public override bool IsInvalid(SerializedProperty property, out string errorMessage)
        {
            errorMessage =
                $"Field {FieldInfo.Name} has invalid method name {ContextMenuItemAttribute.MethodName}";

            if (!TryGetMethodOfProperty(property, out var methodInfo))
                return true;
            
            var methodParameters = methodInfo.GetParameters();
            
            errorMessage =
                $"Method {methodInfo.Name} has more than one parameter";

            if (methodParameters.Length > 1)
                return true;
            
            var fieldConcreteType = TypeUtils.GetPrimaryConcreteType(FieldInfo.FieldType);

            if (methodParameters.Length == 0)
                return false;
            
            var parameterType = methodParameters[0].ParameterType;
            
            errorMessage =
                $"Method {methodInfo.Name} has a {parameterType} parameter which is incompatible with {fieldConcreteType}";

            return !parameterType.IsAssignableFrom(fieldConcreteType);
        }

        private bool TryGetMethodOfProperty(SerializedProperty property, out MethodInfo methodInfo)
        {
            var targetObject =
                SerializedPropertyUtils.GetNearestInspectedObject(property);

            return TypeUtils.TryGetMethodOfObject(
                targetObject, ContextMenuItemAttribute.MethodName, out methodInfo);
        }
    }
}