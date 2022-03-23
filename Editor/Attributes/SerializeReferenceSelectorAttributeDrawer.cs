using System.Reflection;
using InspectorEssentials.Core;
using InspectorEssentials.Editor.Internal;
using InspectorEssentials.Editor.Internal.ContextMenus;
using UnityEditor;
using UnityEngine;

namespace InspectorEssentials.Editor.Attributes
{
    [CustomPropertyDrawer(typeof(SerializeReferenceSelectorAttribute))]
    public class SerializeReferenceSelectorAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var isValid = IsValid(property);

            if (!isValid)
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
                $"Field ${fieldInfo.Name} is invalid. SerializeReferenceSelector only supports managed references.",
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
        
        private string GetShownTypeName(string typeName)
        {
            if (string.IsNullOrEmpty(typeName))
            {
                var concreteTypeName =
                    TypeUtils.GetPrimaryConcreteTypeName(fieldInfo.FieldType);
                
                return $"Select {concreteTypeName}";
            }

            var index = typeName.LastIndexOf(' ');
            
            if (index >= 0)
                typeName = typeName.Substring(index + 1);

            var assembly = Assembly.GetAssembly(fieldInfo.FieldType);

            var type = assembly.GetType(typeName);

            var potentialTypeMenuAttribute =
                type?.GetCustomAttribute(typeof(TypeMenuNameAttribute));

            if (potentialTypeMenuAttribute is TypeMenuNameAttribute typeMenuNameAttribute)
            {
                var parts = typeMenuNameAttribute.MenuName.Split('/');

                return $"{parts[parts.Length - 1]} ({assembly.GetName().Name})";
            }

            return typeName;
        }

        private bool IsValid(SerializedProperty property)
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