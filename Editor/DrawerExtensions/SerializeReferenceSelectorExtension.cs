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
        
        public override void OnPreGUI(Rect position)
        {
            if (IsPropertyValid(Property))
                DrawTypeDropdownButton(position);
        }

        private void DrawTypeDropdownButton(Rect position)
        {
            var buttonText = GetButtonText();
            var buttonContent = new GUIContent(buttonText);
                
            var buttonWidth = 14f + GUI.skin.button.CalcSize(buttonContent).x;
            var buttonHeight = EditorGUIUtility.singleLineHeight;

            var buttonX = position.x + position.width - buttonWidth;
            var buttonRect = new Rect(buttonX, position.y, buttonWidth, buttonHeight);
                
            if (EditorGUI.DropdownButton(buttonRect, buttonContent, FocusType.Passive))
                ShowTypeDropdown(buttonRect, Property, FieldInfo);
        }

        private string GetButtonText()
        {
            var fullTypeName = Property.managedReferenceFullTypename;
            
            if (string.IsNullOrEmpty(fullTypeName))
                return $"Select {TypeUtils.GetPrimaryConcreteTypeName(FieldInfo.FieldType)}";

            return TypeUtils.GetShownTypeName(fullTypeName);
        }

        public override bool IsInvalid(out string errorMessage)
        {
            errorMessage =
                $"Field {FieldInfo.Name} is invalid. SerializeReferenceSelector only supports managed references.";
            
            return !IsPropertyValid(Property);
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