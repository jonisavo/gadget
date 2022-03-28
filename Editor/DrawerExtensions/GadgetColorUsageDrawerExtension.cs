using System.Reflection;
using Gadget.Core;
using UnityEditor;
using UnityEngine;

namespace Gadget.Editor.DrawerExtensions
{
    public class GadgetColorUsageDrawerExtension : PropertyDrawerExtension<GadgetColorUsageAttribute>
    {
        public GadgetColorUsageDrawerExtension(GadgetPropertyAttribute attribute) : base(attribute) {}

        public override bool TryOverrideMainGUI(Rect position)
        {
            if (IsInvalid(out _))
                return false;

            var colorUsageAttribute = (GadgetColorUsageAttribute) Attribute;
            
            EditorGUI.BeginChangeCheck();

            var color = EditorGUI.ColorField(position, Content, Property.colorValue,
                colorUsageAttribute.ShowEyedropper, colorUsageAttribute.ShowAlpha,
                colorUsageAttribute.HDR);

            if (EditorGUI.EndChangeCheck())
                Property.colorValue = color;

            return true;
        }

        public override bool IsInvalid(out string errorMessage)
        {
            errorMessage = $"Field {FieldInfo.Name} is not a Color.";
            return Property.propertyType != SerializedPropertyType.Color;
        }
    }
}