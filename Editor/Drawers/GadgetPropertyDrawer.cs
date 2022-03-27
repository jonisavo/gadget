using System.Collections.Generic;
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
        
        private IEnumerable<PropertyDrawerExtensionBase> _propertyDrawerExtensions;

        private IEnumerable<PropertyDrawerExtensionBase> Extensions
        {
            get
            {
                if (_propertyDrawerExtensions == null)
                    _propertyDrawerExtensions = fieldInfo.GetCustomAttributes(false)
                        .Select(attr =>
                            PropertyDrawerExtensionBase.GetDrawerExtensionForAttribute(attr as PropertyAttribute))
                        .Where(attr => attr != null);

                return _propertyDrawerExtensions;
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            foreach (var extension in Extensions)
            {
                if (!extension.IsInvalid(property, fieldInfo, out var errorMessage))
                    continue;

                var errorBoxPosition = new Rect(position)
                {
                    height = GetInfoBoxHeight(errorMessage)
                };

                EditorGUI.HelpBox(errorBoxPosition, $"{extension.GetType().Name}\n{errorMessage}", MessageType.Error);

                position.y += errorBoxPosition.height + WarningInfoBoxBottomPadding;
            }
            
            if (Extensions.Any(extension => !extension.IsVisible(property)))
                return;

            EditorGUI.BeginDisabledGroup(Extensions.Any(extension => !extension.IsEnabled(property)));

            foreach (var extension in Extensions)
                extension.OnPreGUI(new PropertyDrawerExtensionBase.DrawerExtensionCallbackInfo
                {
                    Property = property,
                    Content = label,
                    FieldInfo = fieldInfo,
                    Position = position
                });

            var overridden = false;

            foreach (var extension in Extensions)
            {
                overridden = extension.TryOverrideMainGUI(new PropertyDrawerExtensionBase.DrawerExtensionCallbackInfo
                {
                    Property = property,
                    Content = label,
                    FieldInfo = fieldInfo,
                    Position = position
                });
                if (overridden)
                    break;
            }
            
            if (!overridden)
                EditorGUI.PropertyField(position, property, label, true);

            foreach (var extension in Extensions.Reverse())
                extension.OnPostGUI(new PropertyDrawerExtensionBase.DrawerExtensionCallbackInfo
                {
                    Property = property,
                    Content = label,
                    FieldInfo = fieldInfo,
                    Position = position
                });

            EditorGUI.EndDisabledGroup();
        }

        public override bool CanCacheInspectorGUI(SerializedProperty property)
        {
            return Extensions.All(extension => extension.CanCacheInspectorGUI(property));
        }

        private static float GetInfoBoxHeight(string text)
        {
            return GUI.skin.box.CalcHeight(new GUIContent(text),
                EditorGUIUtility.fieldWidth + EditorGUIUtility.labelWidth);
        }
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var height = 0f;
            
            foreach (var extension in Extensions)
            {
                if (extension.IsInvalid(property, fieldInfo, out var errorMessage))
                    height += GetInfoBoxHeight(errorMessage) + WarningInfoBoxBottomPadding;
            }

            if (Extensions.Any(extension => !extension.IsVisible(property)))
                return height;

            height += EditorGUI.GetPropertyHeight(property, label, true);
            
            foreach (var extension in Extensions)
            {
                var info = new PropertyDrawerExtensionBase.DrawerExtensionCallbackInfo
                {
                    Property = property,
                    Content = label,
                    FieldInfo = fieldInfo,
                    Position = new Rect()
                };
                var didOverrideHeight =
                    extension.TryOverrideHeight(height, info, out var newHeight);
                
                if (!didOverrideHeight)
                    continue;
                
                height = newHeight;
                break;
            }
            return height;
        }
    }
}