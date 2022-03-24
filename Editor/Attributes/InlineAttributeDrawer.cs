using System.Collections.Generic;
using System.Linq;
using InspectorEssentials.Core;
using InspectorEssentials.Editor.Internal.ContextMenus;
using InspectorEssentials.Editor.Internal.Scope;
using InspectorEssentials.Editor.Internal.Utilities;
using UnityEditor;
using UnityEngine;

using Object = UnityEngine.Object;

namespace InspectorEssentials.Editor.Attributes
{
    [CustomPropertyDrawer(typeof(InlineAttribute))]
    public class InlineAttributeDrawer : PropertyDrawer
    {
        private static readonly int s_controlIdHash =
            nameof(InlineAttributeDrawer).GetHashCode();

        private sealed class GUIResources
        {
            public readonly GUIStyle InDropDownStyle =
                new GUIStyle("IN DropDown");

            public readonly GUIContent CreateContent =
                new GUIContent("Create");
        }

        private static GUIResources _guiResources;
        private static GUIResources Resources => _guiResources ??= new GUIResources();

        public new InlineAttribute attribute => base.attribute as InlineAttribute;

        //----------------------------------------------------------------------

        public override bool CanCacheInspectorGUI(SerializedProperty property)
        {
            return false;
        }

        public override float GetPropertyHeight(
            SerializedProperty property,
            GUIContent label)
        {
            var height = EditorGUIUtility.singleLineHeight;
            
            if (!property.isExpanded)
                return height;
            
            var serializedObject = property.serializedObject;
            var asset = serializedObject.targetObject;
            using (new ObjectScope(asset))
            {
                var target = property.objectReferenceValue;
                var targetExists = target != null;
                
                if (!targetExists || ObjectScope.Contains(target))
                    return height;
                
                var spacing = EditorGUIUtility.standardVerticalSpacing;
                height += spacing;
                height += GetInlinePropertyHeight(target);
                height += 1;
            }
            return height;
        }

        public override void OnGUI(
            Rect position,
            SerializedProperty property,
            GUIContent label)
        {
            var propertyRect = position;
            propertyRect.height = EditorGUIUtility.singleLineHeight;

            DoContextMenuGUI(propertyRect, property);
            DoObjectFieldGUI(propertyRect, property, label);

            if (property.objectReferenceValue)
                DoFoldoutGUI(propertyRect, property);

            if (property.isExpanded)
                DrawExpandedDrawer(position, propertyRect, property);

            DiscardObsoleteSerializedObjectsOnNextEditorUpdate();
        }

        private void DrawExpandedDrawer(Rect position, Rect propertyRect, SerializedProperty property)
        {
            var serializedObject = property.serializedObject;
            var asset = serializedObject.targetObject;
            using (new ObjectScope(asset))
            {
                var target = property.objectReferenceValue;
                var targetExists = target != null;
                
                if (!targetExists || ObjectScope.Contains(target))
                    return;
                
                var inlineRect = position;
                inlineRect.yMin = propertyRect.yMax;
                var spacing = EditorGUIUtility.standardVerticalSpacing;
                inlineRect.xMin += 2;
                inlineRect.xMax -= 18;
                inlineRect.yMin += spacing;
                inlineRect.yMax -= 1;
                DoInlinePropertyGUI(inlineRect, target);
            }
        }

        //----------------------------------------------------------------------

        private static int GetControlID(Rect position)
        {
            return GUIUtility.GetControlID(s_controlIdHash, FocusType.Keyboard, position);
        }

        //----------------------------------------------------------------------

        private void DoContextMenuGUI(
            Rect position,
            SerializedProperty property)
        {
            if (!attribute.AllowInlineCreation)
                return;

            var controlID = GetControlID(position);
            ObjectSelector.DoGUI(controlID, property, SetObjectReferenceValue);

            var buttonRect = position;
            buttonRect.xMin = buttonRect.xMax - 54;
            var buttonStyle = EditorStyles.miniButton;

            var isRepaint = Event.current.type == EventType.Repaint;
            if (isRepaint)
            {
                var dropDownStyle = Resources.InDropDownStyle;
                var rect = buttonRect;
                rect.x += 2;
                rect.y += 6;
                dropDownStyle.Draw(rect, false, false, false, false);
            }

            if (!GUI.Button(buttonRect, Resources.CreateContent, buttonStyle))
                return;

            ShowContextMenu(buttonRect, property);
        }

        private static void SetObjectReferenceValue(
            SerializedProperty property,
            Object newTarget)
        {
            var serializedObject = property.serializedObject;
            property.objectReferenceValue = newTarget;
            property.isExpanded = true;
            serializedObject.ApplyModifiedProperties();
        }

        //----------------------------------------------------------------------

        private bool AllowSceneObjects(SerializedProperty property)
        {
            var asset = property.serializedObject.targetObject;
            return asset != null && !EditorUtility.IsPersistent(asset);
        }

        //----------------------------------------------------------------------

        private void DoObjectFieldGUI(
            Rect position,
            SerializedProperty property,
            GUIContent label)
        {
            label = EditorGUI.BeginProperty(position, label, property);

            if (attribute.AllowInlineCreation)
                position.xMax -= 54;

            var objectType = TypeUtils.GetPrimaryConcreteType(fieldInfo.FieldType);

            var oldTarget = property.objectReferenceValue;
            
            var newTarget =
                EditorGUI.ObjectField(
                    position,
                    label,
                    oldTarget,
                    objectType,
                    AllowSceneObjects(property));

            EditorGUI.EndProperty();
            if (!ReferenceEquals(newTarget, oldTarget))
            {
                SetObjectReferenceValue(property, newTarget);
            }
        }

        //----------------------------------------------------------------------

        private void DoFoldoutGUI(
            Rect position,
            SerializedProperty property)
        {
            var foldoutRect = position;
            foldoutRect.width = EditorGUIUtility.labelWidth;

            var target = property.objectReferenceValue;
            var targetExists = target != null;
            var isExpanded = targetExists && property.isExpanded;

            var noLabel = GUIContent.none;
            isExpanded = EditorGUI.Foldout(foldoutRect, isExpanded, noLabel);

            if (targetExists)
                property.isExpanded = isExpanded;
        }

        //----------------------------------------------------------------------

        private void ShowContextMenu(Rect position, SerializedProperty property)
        {
            var types = TypeUtils.GetConcreteTypes(fieldInfo.FieldType);

            var menuBuilder = new InlineTypeContextMenuBuilder(types.Length > 16);

            menuBuilder.Show(position, fieldInfo, property);
        }

        private float GetInlinePropertyHeight(Object target)
        {
            var serializedObject = GetSerializedObject(target);
            serializedObject.Update();
            var height = 2f;
            var spacing = EditorGUIUtility.standardVerticalSpacing;
            var properties = serializedObject.EnumerateChildProperties();
            foreach (var property in properties)
            {
                height += spacing;
                height += EditorGUI
                    .GetPropertyHeight(property, includeChildren: true);
            }
            if (height > 0)
                height += spacing;
            return height;
        }

        private void DoInlinePropertyGUI(Rect position, Object target)
        {
            DrawInlineBackground(position);
            var serializedObject = GetSerializedObject(target);
            serializedObject.Update();
            var spacing = EditorGUIUtility.standardVerticalSpacing;
            var properties = serializedObject.EnumerateChildProperties();
            position.xMin += 14;
            position.xMax -= 5;
            position.yMin += 1;
            position.yMax -= 1;
            EditorGUI.BeginDisabledGroup(attribute.DisallowEditing);
            
            foreach (var property in properties)
            {
                position.y += spacing;
                position.height =
                    EditorGUI.GetPropertyHeight(property, includeChildren: true);
                EditorGUI.PropertyField(position, property, includeChildren: true);
                position.y += position.height;
            }
            
            EditorGUI.EndDisabledGroup();
            
            if (!attribute.DisallowEditing)
                serializedObject.ApplyModifiedProperties();
        }

        private static void DrawInlineBackground(Rect position)
        {
            var isRepaint = Event.current.type == EventType.Repaint;
            
            if (!isRepaint)
                return;
            
            // var style = new GUIStyle("ProgressBarBack");
            // var style = new GUIStyle("Badge");
            // var style = new GUIStyle("HelpBox");
            // var style = new GUIStyle("ObjectFieldThumb");
            var style = new GUIStyle("ShurikenEffectBg");
            using (ScopeUtils.ColorAlphaScope(0.5f))
            {
                style.Draw(position, false, false, false, false);
            }
        }

        //----------------------------------------------------------------------

        private readonly Dictionary<Object, SerializedObject> 
            _serializedObjectMap = new Dictionary<Object, SerializedObject>();

        private SerializedObject GetSerializedObject(Object target)
        {
            Debug.Assert(target != null);
            
            if (_serializedObjectMap.TryGetValue(target, out var serializedObject))
                return serializedObject;

            serializedObject = new SerializedObject(target);
            _serializedObjectMap.Add(target, serializedObject);
            return serializedObject;
        }

        private void DiscardObsoleteSerializedObjects()
        {
            var destroyedObjects =
                _serializedObjectMap.Keys.Where(key => key == null);

            var enumerable = destroyedObjects as Object[] ?? destroyedObjects.ToArray();
            
            if (!enumerable.Any())
                return;
            
            foreach (var @object in enumerable.ToArray())
                _serializedObjectMap.Remove(@object);
        }

        private void DiscardObsoleteSerializedObjectsOnNextEditorUpdate()
        {
            EditorApplication.delayCall -= DiscardObsoleteSerializedObjects;
            EditorApplication.delayCall += DiscardObsoleteSerializedObjects;
        }
    }
}
