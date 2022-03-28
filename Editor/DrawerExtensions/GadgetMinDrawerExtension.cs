using System.Reflection;
using Gadget.Core;
using UnityEditor;

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
        
        public override void OnPreGUI(DrawerExtensionCallbackInfo info)
        {
            if (!IsPropertyValid(info.Property))
                return;
            
            _previousValue = GetPropertyValue(info.Property);
            
            EditorGUI.BeginChangeCheck();
        }

        public override void OnPostGUI(DrawerExtensionCallbackInfo info)
        {
            if (!EditorGUI.EndChangeCheck() || !IsPropertyValid(info.Property))
                return;

            var currentValue = GetPropertyValue(info.Property);

            if (currentValue < _previousValue)
                SetPropertyValue(info.Property, MinAttribute.MinValue);
        }

        public override bool IsInvalid(SerializedProperty property, FieldInfo fieldInfo, out string errorMessage)
        {
            errorMessage = $"Field {fieldInfo.Name} is not an integer or a float";
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