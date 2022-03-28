﻿using Gadget.Core;
using UnityEditor;
using UnityEngine;

namespace Gadget.Editor.DrawerExtensions
{
    public class GadgetMultilineDrawerExtension : PropertyDrawerExtension<GadgetMultilineAttribute>
    {
        public GadgetMultilineDrawerExtension(GadgetPropertyAttribute attribute) : base(attribute) {}

        public override bool TryOverrideMainGUI(Rect position)
        {
            if (!IsPropertyValid(Property))
                return false;

            EditorGUI.BeginChangeCheck();

            var labelPosition = position;
            labelPosition.height = GetMultilineHeight();
            var labelFieldStyle = GUI.skin.label;
            labelFieldStyle.alignment = TextAnchor.UpperLeft;
            EditorGUI.LabelField(labelPosition, Content, labelFieldStyle);

            var textAreaPosition = position;
            textAreaPosition.x += EditorGUIUtility.labelWidth;
            textAreaPosition.height = GetMultilineHeight();
            
            var newString = EditorGUI.TextArea(textAreaPosition, Property.stringValue);

            if (EditorGUI.EndChangeCheck())
                Property.stringValue = newString;

            return true;
        }

        public override bool TryOverrideHeight(float currentHeight, out float newHeight)
        {
            newHeight = currentHeight;
            
            if (!IsPropertyValid(Property))
                return false;

            newHeight = GetMultilineHeight();

            return true;
        }

        private float GetMultilineHeight()
        {
            var multilineAttribute = (GadgetMultilineAttribute) Attribute;

            return EditorGUIUtility.singleLineHeight * multilineAttribute.Lines;
        }

        public override bool IsInvalid(out string errorMessage)
        {
            errorMessage = $"Field {FieldInfo.Name} is not a string";
            return !IsPropertyValid(Property);
        }

        private static bool IsPropertyValid(SerializedProperty property)
        {
            return property.propertyType == SerializedPropertyType.String;
        }
    }
}