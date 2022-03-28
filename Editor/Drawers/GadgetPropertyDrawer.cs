using System.Linq;
using Gadget.Core;
using Gadget.Editor.DrawerExtensions;
using UnityEditor;
using UnityEngine;

namespace Gadget.Editor.Drawers
{
    /// <summary>
    /// Used as the drawer for each <see cref="GadgetPropertyAttribute"/>.
    /// GUI drawing methods are implemented in each attribute's <see cref="PropertyDrawerExtension{T}"/>.
    /// </summary>
    /// <seealso cref="PropertyDrawerExtension{T}"/>
    /// <seealso cref="GadgetPropertyAttribute"/>
    [CustomPropertyDrawer(typeof(GadgetPropertyAttribute), true)]
    public class GadgetPropertyDrawer : PropertyDrawer
    {
        private const float WarningInfoBoxBottomPadding = 2f;

        private PropertyDrawerExtensionBase[] _propertyDrawerExtensions;

        private PropertyDrawerExtensionBase[] GetExtensions(SerializedProperty property, GUIContent label)
        {
            if (_propertyDrawerExtensions != null &&
                _propertyDrawerExtensions.All(extension => extension.IsInitialized()))
                return _propertyDrawerExtensions;

            _propertyDrawerExtensions = fieldInfo.GetCustomAttributes(false)
                .Select(attr =>
                    PropertyDrawerExtensionBase.GetDrawerExtensionForAttribute(attr as PropertyAttribute))
                .Where(attr => attr != null)
                .ToArray();;

            foreach (var extension in _propertyDrawerExtensions)
                extension.Initialize(property, label, fieldInfo);

            return _propertyDrawerExtensions;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var extensions = GetExtensions(property, label);
            
            foreach (var extension in extensions)
            {
                if (!extension.IsInvalid(out var errorMessage))
                    continue;

                var errorBoxPosition = new Rect(position)
                {
                    height = GetInfoBoxHeight(errorMessage)
                };

                EditorGUI.HelpBox(errorBoxPosition, $"{extension.GetType().Name}\n{errorMessage}", MessageType.Error);

                position.y += errorBoxPosition.height + WarningInfoBoxBottomPadding;
            }
            
            if (extensions.Any(extension => !extension.IsVisible()))
                return;
            
            foreach (var extension in extensions)
                extension.OnPreGUI(position);

            EditorGUI.BeginDisabledGroup(extensions.Any(extension => !extension.IsEnabled()));

            var overridden = false;

            foreach (var extension in extensions)
            {
                overridden = extension.TryOverrideMainGUI(position);
                if (overridden)
                    break;
            }
            
            if (!overridden)
                EditorGUI.PropertyField(position, property, label, true);

            EditorGUI.EndDisabledGroup();
            
            foreach (var extension in extensions.Reverse())
                extension.OnPostGUI(position);
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
                EditorGUIUtility.fieldWidth + EditorGUIUtility.labelWidth);
        }
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var extensions = GetExtensions(property, label);
            
            var height = 0f;

            foreach (var extension in extensions)
            {
                if (extension.IsInvalid(out var errorMessage))
                    height += GetInfoBoxHeight(errorMessage) + WarningInfoBoxBottomPadding;
            }

            if (extensions.Any(extension => !extension.IsVisible()))
                return height;

            height += EditorGUI.GetPropertyHeight(property, label, true);
            
            foreach (var extension in extensions)
            {
                var didOverrideHeight =
                    extension.TryOverrideHeight(height, out var newHeight);
                
                if (!didOverrideHeight)
                    continue;
                
                height = newHeight;
                break;
            }
            return height;
        }
    }
}