using System.Reflection;
using Gadget.Core;
using UnityEditor;

namespace Gadget.Editor.DrawerExtensions
{
    public class GadgetColorUsageDrawerExtension : PropertyDrawerExtension<GadgetColorUsageAttribute>
    {
        public GadgetColorUsageDrawerExtension(GadgetPropertyAttribute attribute) : base(attribute) {}

        public override bool TryOverrideMainGUI(DrawerExtensionCallbackInfo info)
        {
            if (IsInvalid(info.Property, info.FieldInfo, out _))
                return false;

            var colorUsageAttribute = (GadgetColorUsageAttribute) Attribute;
            
            EditorGUI.BeginChangeCheck();

            var color = EditorGUI.ColorField(info.Position, info.Content, info.Property.colorValue,
                colorUsageAttribute.ShowEyedropper, colorUsageAttribute.ShowAlpha,
                colorUsageAttribute.HDR);

            if (EditorGUI.EndChangeCheck())
                info.Property.colorValue = color;

            return true;
        }

        public override bool IsInvalid(SerializedProperty property, FieldInfo fieldInfo, out string errorMessage)
        {
            errorMessage = $"Field {fieldInfo.Name} is not a Color.";
            return property.propertyType != SerializedPropertyType.Color;
        }
    }
}