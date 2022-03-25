﻿using InspectorEssentials.Core;
using UnityEditor;
using UnityEngine;

namespace InspectorEssentials.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(ShowIfAttribute))]
    public class ShowIfAttributeDrawer : ConditionalBoolPropertyDrawerBase<ShowIfAttribute>
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!TryGetBooleanField(property, out var shouldShow))
                position.y += DrawInvalidFieldHelpBox(position);

            if (shouldShow)
                EditorGUI.PropertyField(position, property, label, true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!TryGetBooleanField(property, out var shouldShow))
                return WarningInfoBoxHeight;

            if (!shouldShow)
                return 0f;

            return base.GetPropertyHeight(property, label);
        }
    } 
}

