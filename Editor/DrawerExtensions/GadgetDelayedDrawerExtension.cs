using System.Reflection;
using Gadget.Core;
using UnityEditor;

namespace Gadget.Editor.DrawerExtensions
{
    public class GadgetDelayedDrawerExtension : PropertyDrawerExtension<GadgetDelayedAttribute>
    {
        public GadgetDelayedDrawerExtension(GadgetPropertyAttribute attribute) : base(attribute) {}

        public override bool TryOverrideMainGUI(DrawerExtensionCallbackInfo info)
        {
            if (!IsPropertyValid(info.Property))
                return false;

            switch (info.Property.propertyType)
            {
                case SerializedPropertyType.Float:
                    EditorGUI.DelayedFloatField(info.Position, info.Property, info.Content);
                    break;
                case SerializedPropertyType.Integer:
                    EditorGUI.DelayedIntField(info.Position, info.Property, info.Content);
                    break;
                case SerializedPropertyType.String:
                    EditorGUI.DelayedTextField(info.Position, info.Property, info.Content);
                    break;
                default:
                    return false;
            }

            return true;
        }

        public override bool IsInvalid(SerializedProperty property, FieldInfo fieldInfo, out string errorMessage)
        {
            errorMessage = $"Field {fieldInfo.Name} is not a float, an integer or a string";
            return !IsPropertyValid(property);
        }

        private static bool IsPropertyValid(SerializedProperty property)
        {
            return property.propertyType == SerializedPropertyType.Float ||
                   property.propertyType == SerializedPropertyType.Integer ||
                   property.propertyType == SerializedPropertyType.String;
        }
    }
}