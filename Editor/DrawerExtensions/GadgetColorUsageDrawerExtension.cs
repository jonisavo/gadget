using Gadget.Core;
using UnityEditor;
using UnityEngine;

namespace Gadget.Editor.DrawerExtensions
{
    [GadgetExtensionFor(typeof(GadgetColorUsageAttribute))]
    public class GadgetColorUsageDrawerExtension : GadgetDrawerExtension
    {
        public GadgetColorUsageDrawerExtension(GadgetPropertyAttribute attribute) : base(attribute) {}

        public override bool TryOverrideMainGUI(Rect position, SerializedProperty property)
        {
            if (IsInvalid(property, out _))
                return false;

            var colorUsageAttribute = (GadgetColorUsageAttribute) Attribute;
            
            EditorGUI.BeginChangeCheck();

            var color = EditorGUI.ColorField(position, Label, property.colorValue,
                colorUsageAttribute.ShowEyedropper, colorUsageAttribute.ShowAlpha,
                colorUsageAttribute.HDR);

            if (EditorGUI.EndChangeCheck())
                property.colorValue = color;

            return true;
        }

        public override bool IsInvalid(SerializedProperty property, out string errorMessage)
        {
            errorMessage = $"Field {FieldInfo.Name} is not a Color.";
            return property.propertyType != SerializedPropertyType.Color;
        }
    }
}