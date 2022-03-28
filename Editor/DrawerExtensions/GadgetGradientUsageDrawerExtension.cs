using System.Reflection;
using Gadget.Core;
using UnityEditor;
using UnityEngine;

namespace Gadget.Editor.DrawerExtensions
{
    public class GadgetGradientUsageDrawerExtension : PropertyDrawerExtension<GadgetGradientUsageAttribute>
    {
        public GadgetGradientUsageDrawerExtension(GadgetPropertyAttribute attribute) : base(attribute) {}

        public override bool TryOverrideMainGUI(DrawerExtensionCallbackInfo info)
        {
            if (IsInvalid(info.Property, info.FieldInfo, out _))
                return false;

            // gradientValue is internal for some strange reason.
            var gradientPropertyInfo = info.Property.GetType().GetProperty("gradientValue",
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            if (gradientPropertyInfo == null)
                return false;
            
            var gradientUsageAttribute = (GadgetGradientUsageAttribute) Attribute;
            
            EditorGUI.BeginChangeCheck();

            var currentGradient = gradientPropertyInfo.GetValue(info.Property, null) as Gradient;

            var gradient = EditorGUI.GradientField(info.Position, info.Content, currentGradient,
                gradientUsageAttribute.HDR, gradientUsageAttribute.ColorSpace);

            if (EditorGUI.EndChangeCheck())
                gradientPropertyInfo.SetValue(info.Property, gradient);

            return true;
        }

        public override bool IsInvalid(SerializedProperty property, FieldInfo fieldInfo, out string errorMessage)
        {
            errorMessage = $"Field {fieldInfo.Name} is not a Gradient.";
            return property.propertyType != SerializedPropertyType.Gradient;
        }
    }
}