using Gadget.Core;
using UnityEditor;

namespace Gadget.Editor.DrawerExtensions
{
    public class ShowIfDrawerExtension : ConditionalPropertyDrawerExtensionBase<ShowIfAttribute>
    {
        public ShowIfDrawerExtension(GadgetPropertyAttribute attribute) : base(attribute) {}

        public override bool IsVisible(SerializedProperty property)
        {
            if (PropertyIsPartOfArray(property))
                return true;
            
            if (!TryGetBooleanField(property, out var shouldShow))
                return true;

            return shouldShow;
        }

        public override bool IsInvalid(SerializedProperty property, out string errorMessage)
        {
            errorMessage = $"Field {FieldInfo.Name} is an array.";
            return PropertyIsPartOfArray(property);
        }

        private static bool PropertyIsPartOfArray(SerializedProperty property)
        {
            return property.propertyPath.Contains("Array.data[");
        }
    } 
}

