using InspectorEssentials.Core;
using InspectorEssentials.Editor.Internal.Utilities;
using UnityEditor;
using UnityEngine;

namespace InspectorEssentials.Editor.Drawers
{
    /// <summary>
    /// Used internally as a base drawer class for conditional properties.
    /// </summary>
    /// <typeparam name="T">ConditionalPropertyAttribute type</typeparam>
    public abstract class ConditionalBoolPropertyDrawerBase<T> : PropertyDrawer
        where T : ConditionalPropertyAttribute
    {
        protected const float WarningInfoBoxHeight = 32f;
        
        private string MemberName
        {
            get
            {
                var propertyAttribute = (T) attribute;

                var fieldName = propertyAttribute.MemberName;

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

                return propertyAttribute.MemberName[0] == '!';
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

            if (string.IsNullOrEmpty(MemberName))
                return false;

            var boolProperty = property.serializedObject.FindProperty(MemberName);

            if (boolProperty == null)
                return TryGetBooleanValueFromTargetMember(property, out value);

            if (boolProperty.propertyType != SerializedPropertyType.Boolean)
                return false;

            value = boolProperty.boolValue;

            if (Inverted)
                value = !value;

            return true;
        }

        private bool TryGetBooleanValueFromTargetMember(SerializedProperty property, out bool value)
        {
            value = true;

            var targetObject = property.serializedObject.targetObject;

            if (targetObject == null)
                return false;

            if (!TypeUtils.TryGetValueFromMethodOrPropertyOfObject(targetObject, MemberName, out value))
                return false;

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
                
            EditorGUI.HelpBox(helpBoxPosition,
                $"Field {MemberName} not found or is not a boolean",
                MessageType.Error);
            return WarningInfoBoxHeight;
        }
    }
}