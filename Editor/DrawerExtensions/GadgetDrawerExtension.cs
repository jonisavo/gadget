using System;
using System.Linq;
using System.Reflection;
using Gadget.Core;
using Gadget.Editor.Drawers;
using UnityEditor;
using UnityEngine;

namespace Gadget.Editor.DrawerExtensions
{
    /// <summary>
    /// <para>
    /// <see cref="GadgetDrawerExtension"/> is a workaround for the "1 PropertyDrawer per property" limitation.
    /// They can be applied to any attribute that inherits from <see cref="GadgetPropertyAttribute"/>.
    /// The <see cref="GadgetPropertyDrawer"/> is responsible for applying the effects
    /// of all drawer extensions correctly.
    /// </para>
    /// <para>
    /// <see cref="GadgetExtensionForAttribute"/> is used to link the extension to
    /// a <see cref="GadgetPropertyAttribute"/>.
    /// </para>
    /// </summary>
    /// <seealso cref="GadgetExtensionForAttribute"/>
    /// <seealso cref="GadgetPropertyAttribute"/>
    /// <seealso cref="GadgetPropertyDrawer"/>
    public class GadgetDrawerExtension
    {
        protected readonly GadgetPropertyAttribute Attribute;
        
        protected GUIContent Label;
        protected FieldInfo FieldInfo;

        protected GadgetDrawerExtension(GadgetPropertyAttribute attribute)
        {
            Attribute = attribute;
        }

        public void Initialize(GUIContent label, FieldInfo fieldInfo)
        {
            Label = label;
            FieldInfo = fieldInfo;
        }

        public bool IsInitialized()
        {
            return Label != null && FieldInfo != null;
        }

        /// <summary>
        /// By default, <see cref="GadgetPropertyDrawer"/> draws a single
        /// property field. A PropertyDrawerExtension is able to override it
        /// in this function and should return <c>true</c> if it does so.
        /// </summary>
        /// <param name="position">Position of the property passed from the calling PropertyDrawer</param>
        /// <param name="property">The SerializedProperty from the calling PropertyDrawer</param>
        /// <returns>Whether main GUI was overridden</returns>
        public virtual bool TryOverrideMainGUI(Rect position, SerializedProperty property)
        {
            return false;
        }

        /// <summary>
        /// A callback invoked before drawing the main GUI.
        /// </summary>
        /// <param name="position">Position of the property passed from the calling PropertyDrawer</param>
        /// <param name="property">The SerializedProperty from the calling PropertyDrawer</param>
        public virtual void OnPreGUI(Rect position, SerializedProperty property)
        {
        }

        /// <summary>
        /// A callback invoked after drawing the main GUI.
        /// </summary>
        /// <param name="position">Position of the property passed from the calling PropertyDrawer</param>
        /// <param name="property">The SerializedProperty from the calling PropertyDrawer</param>
        public virtual void OnPostGUI(Rect position, SerializedProperty property)
        {
        }

        /// <summary>
        /// If <c>false</c>, the property is hidden.
        /// </summary>
        /// <param name="property">The SerializedProperty from the calling PropertyDrawer</param>
        /// <returns>Whether the property should be visible</returns>
        public virtual bool IsVisible(SerializedProperty property)
        {
            return true;
        }

        /// <summary>
        /// If <c>false</c>, the property is disabled.
        /// </summary>
        /// <param name="property">The SerializedProperty from the calling PropertyDrawer</param>
        /// <returns>Whether the property should be enabled</returns>
        public virtual bool IsEnabled(SerializedProperty property)
        {
            return true;
        }

        /// <summary>
        /// Returns whether the extension is invalid.
        /// If <c>true</c>, the created error message is shown to the user.
        /// </summary>
        /// <param name="property">The SerializedProperty from the calling PropertyDrawer</param>
        /// <param name="errorMessage">An error message to show to the user</param>
        /// <returns></returns>
        public virtual bool IsInvalid(SerializedProperty property, out string errorMessage)
        {
            errorMessage = null;
            return false;
        }

        /// <summary>
        /// Returns whether the extension should override the property height.
        /// It outputs the new height.
        /// </summary>
        /// <param name="property">The SerializedProperty from the calling PropertyDrawer</param>
        /// <param name="label">The GUIContent label</param>
        /// <param name="newHeight">New height</param>
        /// <returns></returns>
        public virtual bool TryOverrideHeight(SerializedProperty property, GUIContent label, out float newHeight)
        {
            newHeight = 0f;
            return false;
        }

        /// <summary>
        /// If overridden, determines whether the property's GUI can be cached.
        /// </summary>
        /// <param name="property">SerializedProperty passed from the calling PropertyDrawer</param>
        /// <returns>Whether the property's GUI can be cached</returns>
        public virtual bool CanCacheInspectorGUI(SerializedProperty property)
        {
            return true;
        }

        internal static GadgetDrawerExtension GetDrawerExtensionForAttribute(GadgetPropertyAttribute attribute)
        {
            if (attribute == null)
                return null;
            
            var type = attribute.GetType();
            var typeofDrawerExtension = typeof(GadgetDrawerExtension);

            var assembly = Assembly.GetAssembly(typeofDrawerExtension);

            if (assembly == null)
                return null;

            var drawerExtensionAttributesWithType = assembly.GetTypes()
                .Where(assemblyType => assemblyType.IsSubclassOf(typeofDrawerExtension))
                .Where(drawerExtensionType => drawerExtensionType.BaseType != null)
                .Where(drawerExtensionType =>
                {
                    var extensionForAttribute =
                        drawerExtensionType.GetCustomAttribute(typeof(GadgetExtensionForAttribute));

                    if (extensionForAttribute is GadgetExtensionForAttribute extensionAttribute)
                        return extensionAttribute.ExtendsAttributeType == type;

                    return false;
                })
                .Select(drawerExtensionType => Activator.CreateInstance(drawerExtensionType, attribute))
                .ToArray();

            if (drawerExtensionAttributesWithType.Length == 0)
                return null;
            if (drawerExtensionAttributesWithType.Length > 1)
                Debug.LogWarning($"More than one property drawer extension found for type {type}");

            if (drawerExtensionAttributesWithType[0] is GadgetDrawerExtension extension)
                return extension;

            return null;
        }
    }
}