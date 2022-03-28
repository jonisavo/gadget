using Gadget.Core;
using UnityEditor;
using UnityEngine;

namespace Gadget.Editor.DrawerExtensions
{
    public class GadgetMinDrawerExtension : PropertyDrawerExtension<GadgetMinAttribute>
    {
        public GadgetMinDrawerExtension(GadgetPropertyAttribute attribute) : base(attribute) {}

        private float _previousValue;

        private GadgetMinAttribute MinAttribute
        {
            get
            {
                if (_minAttribute == null)
                    _minAttribute = (GadgetMinAttribute) Attribute;

                return _minAttribute;
            }
        }

        private GadgetMinAttribute _minAttribute;
        
        public override void OnPreGUI(Rect position)
        {
            if (!IsPropertyValid(CurrentProperty))
                return;
            
            _previousValue = GetPropertyValue(CurrentProperty);
            
            EditorGUI.BeginChangeCheck();
        }

        public override void OnPostGUI(Rect position)
        {
            if (!EditorGUI.EndChangeCheck() || !IsPropertyValid(CurrentProperty))
                return;

            var currentValue = GetPropertyValue(CurrentProperty);

            if (currentValue < _previousValue)
                SetPropertyValue(CurrentProperty, MinAttribute.MinValue);
        }

        public override bool IsInvalid(out string errorMessage)
        {
            errorMessage = $"Field {FieldInfo.Name} is not an integer or a float";
            return !IsPropertyValid(CurrentProperty);
        }

        private static bool IsPropertyValid(SerializedProperty property)
        {
            return property.propertyType == SerializedPropertyType.Integer ||
                   property.propertyType == SerializedPropertyType.Float;
        }

        private static float GetPropertyValue(SerializedProperty property)
        {
            if (property.propertyType == SerializedPropertyType.Float)
                return property.floatValue;
            
            return property.intValue;
        }

        private static void SetPropertyValue(SerializedProperty property, float value)
        {
            if (property.propertyType == SerializedPropertyType.Float)
                property.floatValue = value;
            else
                property.intValue = (int) value;
        }
    }
}