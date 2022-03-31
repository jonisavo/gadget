using Gadget.Core;
using UnityEditor;
using UnityEngine;

namespace Gadget.Editor.DrawerExtensions
{
    [GadgetExtensionFor(typeof(GadgetMinAttribute))]
    public class GadgetMinDrawerExtension : GadgetDrawerExtension
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
        
        public override void OnPreGUI(Rect position, SerializedProperty property)
        {
            if (!IsPropertyValid(property))
                return;
            
            _previousValue = GetPropertyValue(property);
            
            EditorGUI.BeginChangeCheck();
        }

        public override void OnPostGUI(Rect position, SerializedProperty property)
        {
            if (!EditorGUI.EndChangeCheck() || !IsPropertyValid(property))
                return;

            var currentValue = GetPropertyValue(property);

            if (currentValue < _previousValue)
                SetPropertyValue(property, MinAttribute.MinValue);
        }

        public override bool IsInvalid(SerializedProperty property, out string errorMessage)
        {
            errorMessage = $"Field {FieldInfo.Name} is not an integer or a float";
            return !IsPropertyValid(property);
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