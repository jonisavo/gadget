using System.Reflection;
using Gadget.Core;
using UnityEditor;
using UnityEngine;

namespace Gadget.Editor.DrawerExtensions
{
    public class GadgetGradientUsageDrawerExtension : PropertyDrawerExtension<GadgetGradientUsageAttribute>
    {
        public GadgetGradientUsageDrawerExtension(GadgetPropertyAttribute attribute) : base(attribute) {}

        public override bool TryOverrideMainGUI(Rect position, SerializedProperty property)
        {
            if (IsInvalid(property, out _))
                return false;

            // gradientValue is internal for some strange reason.
            var gradientPropertyInfo = property.GetType().GetProperty("gradientValue",
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            if (gradientPropertyInfo == null)
                return false;
            
            var gradientUsageAttribute = (GadgetGradientUsageAttribute) Attribute;
            
            EditorGUI.BeginChangeCheck();

            var currentGradient = gradientPropertyInfo.GetValue(property, null) as Gradient;

            var gradient = EditorGUI.GradientField(position, Label, currentGradient,
                gradientUsageAttribute.HDR, gradientUsageAttribute.ColorSpace);

            if (EditorGUI.EndChangeCheck())
                gradientPropertyInfo.SetValue(property, gradient);

            return true;
        }

        public override bool IsInvalid(SerializedProperty property, out string errorMessage)
        {
            errorMessage = $"Field {FieldInfo.Name} is not a Gradient.";
            return property.propertyType != SerializedPropertyType.Gradient;
        }
    }
}