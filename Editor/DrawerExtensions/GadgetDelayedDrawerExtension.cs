using Gadget.Core;
using UnityEditor;
using UnityEngine;

namespace Gadget.Editor.DrawerExtensions
{
    public class GadgetDelayedDrawerExtension : PropertyDrawerExtension<GadgetDelayedAttribute>
    {
        public GadgetDelayedDrawerExtension(GadgetPropertyAttribute attribute) : base(attribute) {}

        public override bool TryOverrideMainGUI(Rect position)
        {
            if (!IsPropertyValid(CurrentProperty))
                return false;

            switch (CurrentProperty.propertyType)
            {
                case SerializedPropertyType.Float:
                    EditorGUI.DelayedFloatField(position, CurrentProperty, Content);
                    break;
                case SerializedPropertyType.Integer:
                    EditorGUI.DelayedIntField(position, CurrentProperty, Content);
                    break;
                case SerializedPropertyType.String:
                    EditorGUI.DelayedTextField(position, CurrentProperty, Content);
                    break;
                default:
                    return false;
            }

            return true;
        }

        public override bool IsInvalid(out string errorMessage)
        {
            errorMessage = $"Field {FieldInfo.Name} is not a float, an integer or a string";
            return !IsPropertyValid(CurrentProperty);
        }

        private static bool IsPropertyValid(SerializedProperty property)
        {
            return property.propertyType == SerializedPropertyType.Float ||
                   property.propertyType == SerializedPropertyType.Integer ||
                   property.propertyType == SerializedPropertyType.String;
        }
    }
}