using System.Reflection;
using Gadget.Core;
using UnityEditor;
using UnityEngine;

namespace Gadget.Editor.DrawerExtensions
{
    public class GadgetGradientUsageDrawerExtension : PropertyDrawerExtension<GadgetGradientUsageAttribute>
    {
        public GadgetGradientUsageDrawerExtension(GadgetPropertyAttribute attribute) : base(attribute) {}

        public override bool TryOverrideMainGUI(Rect position)
        {
            if (IsInvalid(out _))
                return false;

            // gradientValue is internal for some strange reason.
            var gradientPropertyInfo = CurrentProperty.GetType().GetProperty("gradientValue",
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            if (gradientPropertyInfo == null)
                return false;
            
            var gradientUsageAttribute = (GadgetGradientUsageAttribute) Attribute;
            
            EditorGUI.BeginChangeCheck();

            var currentGradient = gradientPropertyInfo.GetValue(CurrentProperty, null) as Gradient;

            var gradient = EditorGUI.GradientField(position, Content, currentGradient,
                gradientUsageAttribute.HDR, gradientUsageAttribute.ColorSpace);

            if (EditorGUI.EndChangeCheck())
                gradientPropertyInfo.SetValue(CurrentProperty, gradient);

            return true;
        }

        public override bool IsInvalid(out string errorMessage)
        {
            errorMessage = $"Field {FieldInfo.Name} is not a Gradient.";
            return CurrentProperty.propertyType != SerializedPropertyType.Gradient;
        }
    }
}