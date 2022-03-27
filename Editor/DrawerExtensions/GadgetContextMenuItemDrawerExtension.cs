using System.Collections.Generic;
using System.Reflection;
using Gadget.Core;
using Gadget.Editor.Internal.Utilities;
using UnityEditor;
using UnityEngine;

namespace Gadget.Editor.DrawerExtensions
{
    public class GadgetContextMenuItemDrawerExtension : PropertyDrawerExtension<GadgetContextMenuItemAttribute>
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

        public override void OnPreGUI(DrawerExtensionCallbackInfo info)
        {
            var property = info.Property;
            
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
            
            var targetObject = info.Property.serializedObject.targetObject;
            
            menu.AddItem(new GUIContent(ContextMenuItemAttribute.MenuItemName), false,
                () => methodInfo.Invoke(targetObject, null));
        }

        public override void OnPostGUI(DrawerExtensionCallbackInfo info)
        {
            var evt = Event.current;

            if (evt.type != EventType.MouseDown || evt.button != 1)
                return;

            if (!info.Position.Contains(evt.mousePosition))
                return;
            
            if (MenuDictionary.ContainsKey(info.Property))
                MenuDictionary[info.Property].ShowAsContext();
            
            evt.Use();
        }

        public override bool IsInvalid(SerializedProperty property, FieldInfo fieldInfo, out string errorMessage)
        {
            errorMessage =
                $"Field {fieldInfo.Name} has invalid method name {ContextMenuItemAttribute.MethodName}";
            return !TryGetMethodOfProperty(property, out _);
        }

        private bool TryGetMethodOfProperty(SerializedProperty property, out MethodInfo methodInfo)
        {
            var targetObject = property.serializedObject.targetObject;

            return TypeUtils.TryGetMethodOfObject(
                targetObject, ContextMenuItemAttribute.MethodName, out methodInfo);
        }
    }
}