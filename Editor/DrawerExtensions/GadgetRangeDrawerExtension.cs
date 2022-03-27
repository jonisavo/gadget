using System.Reflection;
using Gadget.Core;
using UnityEditor;

namespace Gadget.Editor.DrawerExtensions
{
    public class GadgetRangeDrawerExtension : PropertyDrawerExtension<GadgetRangeAttribute>
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

        public override bool TryOverrideMainGUI(DrawerExtensionCallbackInfo info)
        {
            if (!IsPropertyValid(info.Property))
                return false;
            
            switch (info.Property.propertyType)
            {
                case SerializedPropertyType.Integer:
                    EditorGUI.IntSlider(info.Position, info.Property,
                        (int) RangeAttribute.Min, (int) RangeAttribute.Max, info.Content);
                    break;
                case SerializedPropertyType.Float:
                    EditorGUI.Slider(info.Position, info.Property,
                        RangeAttribute.Min, RangeAttribute.Max);
                    break;
                default:
                    return false;
            }

            return true;
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
    }
}