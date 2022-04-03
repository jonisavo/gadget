using Gadget.Core;
using UnityEditor;
using UnityEngine;

namespace Gadget.Editor.DrawerExtensions
{
    [GadgetExtensionFor(typeof(GadgetMultilineAttribute))]
    public class GadgetMultilineDrawerExtension : GadgetDrawerExtension
    {
        public GadgetMultilineDrawerExtension(GadgetPropertyAttribute attribute) : base(attribute) {}

        public override bool TryOverrideMainGUI(Rect position, SerializedProperty property)
        {
            if (!IsPropertyValid(property))
                return false;

            var textAreaPosition = EditorGUI.PrefixLabel(position, Label);
            textAreaPosition.height = GetMultilineHeight();

            EditorGUI.BeginChangeCheck();

            // TextArea is given an additional indent for some strange reason,
            // this undoes it.
            var previousIndentLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            var newString = EditorGUI.TextArea(
                textAreaPosition,
                property.stringValue,
                EditorStyles.textArea
            );
            
            EditorGUI.indentLevel = previousIndentLevel;

            if (EditorGUI.EndChangeCheck())
                property.stringValue = newString;

            return true;
        }

        public override bool TryOverrideHeight(SerializedProperty property, GUIContent label, out float newHeight)
        {
            newHeight = 0f;
            
            if (!IsPropertyValid(property))
                return false;

            newHeight = GetMultilineHeight();

            return true;
        }

        private float GetMultilineHeight()
        {
            var multilineAttribute = (GadgetMultilineAttribute) Attribute;

            return EditorGUIUtility.singleLineHeight * multilineAttribute.Lines;
        }

        public override bool IsInvalid(SerializedProperty property, out string errorMessage)
        {
            errorMessage = $"Field {FieldInfo.Name} is not a string";
            return !IsPropertyValid(property);
        }

        private static bool IsPropertyValid(SerializedProperty property)
        {
            return property.propertyType == SerializedPropertyType.String;
        }
    }
}