using InspectorEssentials.Core;
using InspectorEssentials.Editor.Internal.ContextMenus;
using InspectorEssentials.Editor.Internal.Utilities;
using UnityEditor;
using UnityEngine;

namespace InspectorEssentials.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(SerializeReferenceSelectorAttribute))]
    public class SerializeReferenceSelectorAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!IsValid(property))
            {
                DrawErrorBox(position);
                return;
            }
            
            EditorGUI.BeginChangeCheck();
            
            DrawTypeDropdownButton(position, property, label);
            
            EditorGUI.PropertyField(position, property, label, true);

            EditorGUI.EndChangeCheck();
        }

        private void DrawErrorBox(Rect position)
        {
            EditorGUI.HelpBox(
                position,
                $"Field {fieldInfo.Name} is invalid. SerializeReferenceSelector only supports managed references.",
                MessageType.Error);
        }

        private void DrawTypeDropdownButton(Rect position, SerializedProperty property, GUIContent label)
        {
            var buttonText = GetButtonText(property.managedReferenceFullTypename);
            var buttonContent = new GUIContent(buttonText);
                
            var buttonWidth = 14f + GUI.skin.button.CalcSize(buttonContent).x;
            var buttonHeight = EditorGUI.GetPropertyHeight(property, label, false);

            var buttonX = position.x + position.width - buttonWidth;
            var buttonRect = new Rect(buttonX, position.y, buttonWidth, buttonHeight);
                
            if (EditorGUI.DropdownButton(buttonRect, buttonContent, FocusType.Passive))
                ShowTypeDropdown(buttonRect, property);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        private string GetButtonText(string fullTypeName)
        {
            if (string.IsNullOrEmpty(fullTypeName))
                return GetSelectionPromptText();

            return TypeUtils.GetShownTypeName(fullTypeName);
        }

        private string GetSelectionPromptText()
        {
            var concreteTypeName =
                TypeUtils.GetPrimaryConcreteTypeName(fieldInfo.FieldType);
                
            return $"Select {concreteTypeName}";
        }

        private static bool IsValid(SerializedProperty property)
        {
            return property.propertyType == SerializedPropertyType.ManagedReference;
        }
        
        private void ShowTypeDropdown(Rect position, SerializedProperty property)
        {
            var menu = new GenericMenu();
            
            var menuBuilder = new SerializeReferenceTypeContextMenuBuilder(menu, fieldInfo, property , false);
            
            menuBuilder.Show(position);
        }
    }
}