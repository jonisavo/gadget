using EditorAttributesLite.Core;
using UnityEditor;
using UnityEngine;

namespace EditorAttributesLite.Editor.Internal
{
    public abstract class ConditionalBoolPropertyDrawerBase<T> : PropertyDrawer
        where T : ConditionalPropertyAttribute
    {
        private const float WarningInfoBoxHeight = 32f;
        
        private string FieldName
        {
            get
            {
                var propertyAttribute = (T) attribute;

                var fieldName = propertyAttribute.FieldName;

                if (fieldName[0] == '!')
                    fieldName = fieldName.Remove(0, 1);
                
                return fieldName;
            }
        }
        
        private bool Inverted
        {
            get
            {
                var propertyAttribute = (T) attribute;

                return propertyAttribute.FieldName[0] == '!';
            }
        }
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var height = EditorGUI.GetPropertyHeight(property, label);

            if (!TryGetBooleanField(property, out _))
                height += WarningInfoBoxHeight;

            return height;
        }
        
        protected bool TryGetBooleanField(SerializedProperty property, out bool value)
        {
            value = true;

            if (string.IsNullOrEmpty(FieldName))
                return false;

            var boolProperty = property.serializedObject.FindProperty(FieldName);

            if (boolProperty == null || boolProperty.propertyType != SerializedPropertyType.Boolean)
                return false;

            value = boolProperty.boolValue;

            if (Inverted)
                value = !value;

            return true;
        }

        protected float DrawInvalidFieldHelpBox(Rect position)
        {
            var helpBoxPosition = new Rect(position)
            {
                height = WarningInfoBoxHeight
            };
                
            EditorGUI.HelpBox(helpBoxPosition, $"Field {FieldName} not found or is not a boolean", MessageType.Error);
            return WarningInfoBoxHeight;
        }
    }
}