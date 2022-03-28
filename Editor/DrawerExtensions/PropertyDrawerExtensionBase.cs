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
    /// The base class for PropertyDrawerExtensions. Defines all methods.
    /// </summary>
    public abstract class PropertyDrawerExtensionBase
    {
        protected readonly GadgetPropertyAttribute Attribute;
        
        protected SerializedProperty Property;
        protected GUIContent Content;
        protected FieldInfo FieldInfo;

        protected PropertyDrawerExtensionBase(GadgetPropertyAttribute attribute)
        {
            Attribute = attribute;
        }

        public void Initialize(SerializedProperty property, GUIContent label, FieldInfo fieldInfo)
        {
            Property = property;
            Content = label;
            FieldInfo = fieldInfo;
        }

        public bool IsInitialized()
        {
            return Property != null && Content != null && FieldInfo != null;
        }

        /// <summary>
        /// By default, <see cref="GadgetPropertyDrawer"/> draws a single
        /// property field. A PropertyDrawerExtension is able to override it
        /// in this function and should return <c>true</c> if it does so.
        /// </summary>
        /// <param name="info">Properties from the calling PropertyDrawer</param>
        /// <returns>Whether main GUI was overridden</returns>
        public virtual bool TryOverrideMainGUI(Rect position)
        {
            return false;
        }

        /// <summary>
        /// A callback invoked before drawing the main GUI.
        /// </summary>
        /// <param name="info">Properties from the calling PropertyDrawer</param>
        public virtual void OnPreGUI(Rect position)
        {
        }

        /// <summary>
        /// A callback invoked after drawing the main GUI.
        /// </summary>
        /// <param name="info">Properties from the calling PropertyDrawer</param>
        public virtual void OnPostGUI(Rect position)
        {
        }

        /// <summary>
        /// If <c>false</c>, the property is hidden.
        /// </summary>
        /// <param name="property">The SerializedProperty from the calling PropertyDrawer</param>
        /// <returns>Whether the property should be visible</returns>
        public virtual bool IsVisible()
        {
            return true;
        }

        /// <summary>
        /// If <c>false</c>, the property is disabled.
        /// </summary>
        /// <param name="property">The SerializedProperty from the calling PropertyDrawer</param>
        /// <returns>Whether the property should be enabled</returns>
        public virtual bool IsEnabled()
        {
            return true;
        }

        /// <summary>
        /// Returns whether the extension is invalid.
        /// If <c>true</c>, the created error message is shown to the user.
        /// </summary>
        /// <param name="property">The SerializedProperty from the calling PropertyDrawer</param>
        /// <param name="fieldInfo">The FieldInfo from the calling PropertyDrawer</param>
        /// <param name="errorMessage">An error message to show to the user</param>
        /// <returns></returns>
        public virtual bool IsInvalid(out string errorMessage)
        {
            errorMessage = null;
            return false;
        }

        /// <summary>
        /// Returns whether the extension should override the property height.
        /// It outputs the new height.
        /// </summary>
        /// <param name="currentHeight">Current property height</param>
        /// <param name="info">Information passed from the calling PropertyDrawer</param>
        /// <param name="newHeight">New height</param>
        /// <returns></returns>
        public virtual bool TryOverrideHeight(float currentHeight, out float newHeight)
        {
            newHeight = currentHeight;
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

        internal static PropertyDrawerExtensionBase GetDrawerExtensionForAttribute(PropertyAttribute attribute)
        {
            if (attribute == null)
                return null;
            
            var type = attribute.GetType();
            var typeofDrawerExtension = typeof(PropertyDrawerExtensionBase);

            var assembly = Assembly.GetAssembly(typeofDrawerExtension);

            if (assembly == null)
                return null;

            var drawerExtensionAttributesWithType = assembly.GetTypes()
                .Where(assemblyType => assemblyType.IsSubclassOf(typeofDrawerExtension))
                .Where(drawerExtensionType => drawerExtensionType.BaseType is {IsGenericType: true})
                .Where(drawerExtensionType => drawerExtensionType.BaseType.GenericTypeArguments[0] == type)
                .Select(drawerExtensionType => Activator.CreateInstance(drawerExtensionType, attribute))
                .ToArray();

            if (drawerExtensionAttributesWithType.Length == 0)
                return null;
            if (drawerExtensionAttributesWithType.Length > 1)
                Debug.LogWarning($"More than one property drawer extension found for type {type}");

            if (drawerExtensionAttributesWithType[0] is PropertyDrawerExtensionBase extension)
                return extension;

            return null;
        }
    }
}