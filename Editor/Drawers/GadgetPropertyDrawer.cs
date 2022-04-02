using System.Linq;
using Gadget.Core;
using Gadget.Editor.DrawerExtensions;
using UnityEditor;
using UnityEngine;

namespace Gadget.Editor.Drawers
{
    /// <summary>
    /// Used as the drawer for each <see cref="GadgetPropertyAttribute"/>.
    /// GUI drawing methods are implemented in each attribute's <see cref="GadgetDrawerExtension"/>.
    /// </summary>
    /// <seealso cref="GadgetDrawerExtension"/>
    /// <seealso cref="GadgetPropertyAttribute"/>
    [CustomPropertyDrawer(typeof(GadgetPropertyAttribute), true)]
    public class GadgetPropertyDrawer : PropertyDrawer
    {
        private const float WarningInfoBoxBottomPadding = 2f;

        private GadgetDrawerExtension[] _propertyDrawerExtensions;

        private GadgetDrawerExtension[] GetExtensions(GUIContent label)
        {
            if (_propertyDrawerExtensions != null &&
                _propertyDrawerExtensions.All(extension => extension.IsInitialized()))
                return _propertyDrawerExtensions;

            _propertyDrawerExtensions = fieldInfo.GetCustomAttributes(false)
                .Select(attr =>
                    GadgetDrawerExtension.GetDrawerExtensionForAttribute(attr as GadgetPropertyAttribute))
                .Where(attr => attr != null)
                .ToArray();

            foreach (var extension in _propertyDrawerExtensions)
                extension.Initialize(label, fieldInfo);

            return _propertyDrawerExtensions;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var extensions = GetExtensions(label);
            
            foreach (var extension in extensions)
            {
                if (!extension.IsInvalid(property, out var errorMessage))
                    continue;

                var shownErrorMessage = $"{extension.GetType().Name}\n{errorMessage}";

                var errorBoxPosition = position;
                errorBoxPosition.height = GetInfoBoxHeight(shownErrorMessage);

                EditorGUI.HelpBox(errorBoxPosition, shownErrorMessage, MessageType.Error);

                var totalHeight = errorBoxPosition.height + WarningInfoBoxBottomPadding;

                position.y += totalHeight;
                position.height -= totalHeight;
            }
            
            if (extensions.Any(extension => !extension.IsVisible(property)))
                return;
            
            foreach (var extension in extensions)
                extension.OnPreGUI(position, property);

            EditorGUI.BeginDisabledGroup(extensions.Any(extension => !extension.IsEnabled(property)));

            var overridden = false;

            foreach (var extension in extensions)
            {
                overridden = extension.TryOverrideMainGUI(position, property);
                if (overridden)
                    break;
            }
            
            if (!overridden)
                EditorGUI.PropertyField(position, property, label, true);

            EditorGUI.EndDisabledGroup();
            
            foreach (var extension in extensions.Reverse())
                extension.OnPostGUI(position, property);
        }

        public override bool CanCacheInspectorGUI(SerializedProperty property)
        {
            if (_propertyDrawerExtensions == null)
                return base.CanCacheInspectorGUI(property);
            
            return _propertyDrawerExtensions.All(extension => extension.CanCacheInspectorGUI(property));
        }

        private static float GetInfoBoxHeight(string text)
        {
            return GUI.skin.box.CalcHeight(new GUIContent(text),
                EditorGUIUtility.fieldWidth + EditorGUIUtility.labelWidth * 2);
        }

        private static float SumErrorBoxHeights(
            SerializedProperty property, GadgetDrawerExtension[] extensions)
        {
            var height = 0f;

            foreach (var extension in extensions)
            {
                if (!extension.IsInvalid(property, out var errorMessage)) 
                    continue;
                
                var shownErrorMessage = $"{extension.GetType().Name}\n{errorMessage}";
                height += GetInfoBoxHeight(shownErrorMessage);
                height += WarningInfoBoxBottomPadding;
            }

            return height;
        }
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var extensions = GetExtensions(label);
            
            var height = SumErrorBoxHeights(property, extensions);

            if (extensions.Any(extension => !extension.IsVisible(property)))
                return height;

            height += EditorGUI.GetPropertyHeight(property, label, true);
            
            foreach (var extension in extensions)
            {
                var didOverrideHeight =
                    extension.TryOverrideHeight(height, property, out var newHeight);
                
                if (!didOverrideHeight)
                    continue;
                
                height = newHeight;
                break;
            }
            return height;
        }
    }
}