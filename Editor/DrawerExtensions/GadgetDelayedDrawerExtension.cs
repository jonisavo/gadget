using Gadget.Core;
using UnityEditor;
using UnityEngine;

namespace Gadget.Editor.DrawerExtensions
{
    [GadgetExtensionFor(typeof(GadgetDelayedAttribute))]
    public class GadgetDelayedDrawerExtension : GadgetDrawerExtension
    {
        public GadgetDelayedDrawerExtension(GadgetPropertyAttribute attribute) : base(attribute) {}

        public override bool TryOverrideMainGUI(Rect position, SerializedProperty property)
        {
            if (!IsPropertyValid(property))
                return false;

            switch (property.propertyType)
            {
                case SerializedPropertyType.Float:
                    EditorGUI.DelayedFloatField(position, property, Label);
                    break;
                case SerializedPropertyType.Integer:
                    EditorGUI.DelayedIntField(position, property, Label);
                    break;
                case SerializedPropertyType.String:
                    EditorGUI.DelayedTextField(position, property, Label);
                    break;
                default:
                    return false;
            }

            return true;
        }

        public override bool IsInvalid(SerializedProperty property, out string errorMessage)
        {
            errorMessage = $"Field {FieldInfo.Name} is not a float, an integer or a string";
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