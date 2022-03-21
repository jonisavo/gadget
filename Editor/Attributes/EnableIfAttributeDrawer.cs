using InspectorEssentials.Core;
using InspectorEssentials.Editor.Internal;
using UnityEditor;
using UnityEngine;

namespace InspectorEssentials.Editor.Attributes
{
    [CustomPropertyDrawer(typeof(EnableIfAttribute))]
    public class EnableIfAttributeDrawer : ConditionalBoolPropertyDrawerBase<EnableIfAttribute>
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!TryGetBooleanField(property, out var shouldEnable))
                position.y += DrawInvalidFieldHelpBox(position);

            var previouslyEnabled = GUI.enabled;

            GUI.enabled = shouldEnable;
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = previouslyEnabled;
        }
    } 
}

