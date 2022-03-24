using InspectorEssentials.Core;
using InspectorEssentials.Editor.Internal.ContextMenus;
using InspectorEssentials.Editor.Internal.Utilities;
using UnityEditor;
using UnityEngine;

namespace InspectorEssentials.Editor.Attributes
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
            var typeName = GetShownTypeName(property.managedReferenceFullTypename);
            var typeNameGUIContent = new GUIContent(typeName);
                
            var buttonWidth = 14f + GUI.skin.button.CalcSize(typeNameGUIContent).x;
            var buttonHeight = EditorGUI.GetPropertyHeight(property, label, false);

            var buttonX = position.x + position.width - buttonWidth;
            var buttonRect = new Rect(buttonX, position.y, buttonWidth, buttonHeight);
                
            if (EditorGUI.DropdownButton(buttonRect, typeNameGUIContent, FocusType.Passive))
                ShowTypeDropdown(buttonRect, property);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
        
        private string GetShownTypeName(string fullTypeName)
        {
            if (string.IsNullOrEmpty(fullTypeName))
                return GetShownInvalidTypeName();

            return TypeUtils.GetShownTypeName(fullTypeName);
        }

        private string GetShownInvalidTypeName()
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
            var menuBuilder = new SerializeReferenceTypeContextMenuBuilder(false);
            
            menuBuilder.Show(position, fieldInfo, property);
        }
    }
}