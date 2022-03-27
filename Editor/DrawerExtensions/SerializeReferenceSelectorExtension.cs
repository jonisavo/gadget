using System.Reflection;
using Gadget.Core;
using Gadget.Editor.Internal.ContextMenus;
using Gadget.Editor.Internal.Utilities;
using UnityEditor;
using UnityEngine;

namespace Gadget.Editor.DrawerExtensions
{
    public class SerializeReferenceSelectorExtension : PropertyDrawerExtension<SerializeReferenceSelectorAttribute>
    {
        public SerializeReferenceSelectorExtension(GadgetPropertyAttribute attribute) : base(attribute) {}
        
        public override void OnPreGUI(DrawerExtensionCallbackInfo info)
        {
            if (IsPropertyValid(info.Property))
                DrawTypeDropdownButton(info);
        }

        private static void DrawTypeDropdownButton(DrawerExtensionCallbackInfo info)
        {
            var buttonText = GetButtonText(info);
            var buttonContent = new GUIContent(buttonText);
                
            var buttonWidth = 14f + GUI.skin.button.CalcSize(buttonContent).x;
            var buttonHeight = EditorGUI.GetPropertyHeight(info.Property, info.Content, false);

            var buttonX = info.Position.x + info.Position.width - buttonWidth;
            var buttonRect = new Rect(buttonX, info.Position.y, buttonWidth, buttonHeight);
                
            if (EditorGUI.DropdownButton(buttonRect, buttonContent, FocusType.Passive))
                ShowTypeDropdown(buttonRect, info.Property, info.FieldInfo);
        }

        private static string GetButtonText(DrawerExtensionCallbackInfo info)
        {
            var fullTypeName = info.Property.managedReferenceFullTypename;
            
            if (string.IsNullOrEmpty(fullTypeName))
                return $"Select {TypeUtils.GetPrimaryConcreteTypeName(info.FieldInfo.FieldType)}";

            return TypeUtils.GetShownTypeName(fullTypeName);
        }

        public override bool IsInvalid(SerializedProperty property, FieldInfo fieldInfo, out string errorMessage)
        {
            errorMessage =
                $"Field {fieldInfo.Name} is invalid. SerializeReferenceSelector only supports managed references.";
            
            return !IsPropertyValid(property);
        }

        private static bool IsPropertyValid(SerializedProperty property)
        {
            return property.propertyType == SerializedPropertyType.ManagedReference;
        }

        private static void ShowTypeDropdown(Rect position, SerializedProperty property, FieldInfo fieldInfo)
        {
            var menu = new GenericMenu();
            
            var menuBuilder = new SerializeReferenceTypeContextMenuBuilder(menu, fieldInfo, property, false);

            var choices = menuBuilder.GetChoices();
            
            menuBuilder.Show(position, choices);
        }
    }
}