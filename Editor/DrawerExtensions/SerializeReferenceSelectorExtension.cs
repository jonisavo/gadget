using System.Reflection;
using Gadget.Core;
using Gadget.Editor.Internal.ContextMenus;
using Gadget.Editor.Internal.Utilities;
using UnityEditor;
using UnityEngine;

namespace Gadget.Editor.DrawerExtensions
{
    [GadgetExtensionFor(typeof(SerializeReferenceSelectorAttribute))]
    public class SerializeReferenceSelectorExtension : GadgetDrawerExtension
    {
        public SerializeReferenceSelectorExtension(GadgetPropertyAttribute attribute) : base(attribute) {}
        
        public override void OnPreGUI(Rect position, SerializedProperty property)
        {
            if (IsPropertyValid(property))
                DrawTypeDropdownButton(position, property);
        }

        private void DrawTypeDropdownButton(Rect position, SerializedProperty property)
        {
            var buttonText = GetButtonText(property);
            var buttonContent = new GUIContent(buttonText);
                
            var buttonWidth = 14f + GUI.skin.button.CalcSize(buttonContent).x;
            var buttonHeight = EditorGUIUtility.singleLineHeight;

            var buttonX = position.x + position.width - buttonWidth;
            var buttonRect = new Rect(buttonX, position.y, buttonWidth, buttonHeight);
                
            if (EditorGUI.DropdownButton(buttonRect, buttonContent, FocusType.Passive))
                ShowTypeDropdown(buttonRect, property, FieldInfo);
        }

        private string GetButtonText(SerializedProperty property)
        {
            var fullTypeName = property.managedReferenceFullTypename;
            
            if (string.IsNullOrEmpty(fullTypeName))
                return $"Select {TypeUtils.GetPrimaryConcreteTypeName(FieldInfo.FieldType)}";

            return TypeUtils.GetShownTypeName(fullTypeName);
        }

        public override bool IsInvalid(SerializedProperty property, out string errorMessage)
        {
            errorMessage =
                $"Field {FieldInfo.Name} is invalid. SerializeReferenceSelector only supports managed references.";
            
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