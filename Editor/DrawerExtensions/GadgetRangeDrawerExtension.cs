using Gadget.Core;
using UnityEditor;
using UnityEngine;

namespace Gadget.Editor.DrawerExtensions
{
    [GadgetExtensionFor(typeof(GadgetRangeAttribute))]
    public class GadgetRangeDrawerExtension : GadgetDrawerExtension
    {
        public GadgetRangeDrawerExtension(GadgetPropertyAttribute attribute) : base(attribute) {}

        private GadgetRangeAttribute RangeAttribute
        {
            get
            {
                if (_rangeAttribute == null)
                    _rangeAttribute = (GadgetRangeAttribute) Attribute;

                return _rangeAttribute;
            }
        }

        private GadgetRangeAttribute _rangeAttribute;

        public override bool TryOverrideMainGUI(Rect position, SerializedProperty property)
        {
            if (!IsPropertyValid(property))
                return false;
            
            switch (property.propertyType)
            {
                case SerializedPropertyType.Integer:
                    EditorGUI.IntSlider(position, property,
                        (int) RangeAttribute.Min, (int) RangeAttribute.Max, Label);
                    break;
                case SerializedPropertyType.Float:
                    EditorGUI.Slider(position, property,
                        RangeAttribute.Min, RangeAttribute.Max);
                    break;
                default:
                    return false;
            }

            return true;
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
    }
}