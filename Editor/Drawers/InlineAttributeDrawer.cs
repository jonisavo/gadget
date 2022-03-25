using System.Collections.Generic;
using System.Linq;
using InspectorEssentials.Core;
using InspectorEssentials.Editor.Internal.ContextMenus;
using InspectorEssentials.Editor.Internal.Scope;
using InspectorEssentials.Editor.Internal.Utilities;
using UnityEditor;
using UnityEngine;

using Object = UnityEngine.Object;

namespace InspectorEssentials.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(InlineAttribute))]
    public class InlineAttributeDrawer : PropertyDrawer
    {
        private sealed class GUIResources
        {
            public readonly GUIContent CreateContent =
                new GUIContent("Create");
            
            public readonly float ErrorBoxHeight =
                EditorGUIUtility.singleLineHeight + 16f;

            public readonly float CreateButtonWidth = 54f;
        }

        private static GUIResources _guiResources;
        private static GUIResources Resources => _guiResources ??= new GUIResources();

        private InlineAttribute Attribute => attribute as InlineAttribute;

        public override bool CanCacheInspectorGUI(SerializedProperty property)
        {
            return false;
        }

        public override float GetPropertyHeight(
            SerializedProperty property,
            GUIContent label)
        {
            var height = EditorGUIUtility.singleLineHeight;

            if (!IsValid(property))
                return Resources.ErrorBoxHeight;

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

            if (!IsValid(property))
            {
                DoErrorBox(position);
                return;
            }

            if (ShouldShowInlineCreation()) 
                DoInlineCreationGUI(propertyRect, property);
            
            DoObjectFieldGUI(propertyRect, property, label);

            if (property.objectReferenceValue)
                DoFoldoutGUI(propertyRect, property);

            if (property.isExpanded)
                DrawExpandedDrawer(position, propertyRect, property);

            DiscardObsoleteSerializedObjectsOnNextEditorUpdate();
        }

        private void DoErrorBox(Rect position)
        {
            EditorGUI.HelpBox(position,
                $"Field {fieldInfo.Name} is invalid as InlineAttribute works only on Object references.",
                MessageType.Error);
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
                
                var spacing = EditorGUIUtility.standardVerticalSpacing;
                var inlineRect = new Rect(position)
                {
                    xMin = position.xMin + 2,
                    xMax = position.xMax - 18,
                    yMin = propertyRect.yMax + spacing,
                    yMax = position.yMax - 1
                };
                DoInlinePropertyGUI(inlineRect, target);
            }
        }

        private static bool IsValid(SerializedProperty property)
        {
            return property.propertyType == SerializedPropertyType.ObjectReference;
        }

        private bool ShouldShowInlineCreation()
        {
            return !Attribute.DisallowInlineCreation &&
                   InlineUtils.DoesTypeSupportInlineCreation(fieldInfo.FieldType);
        }

        private void DoInlineCreationGUI(
            Rect position,
            SerializedProperty property)
        {
            var buttonRect = position;
            buttonRect.xMin = buttonRect.xMax - Resources.CreateButtonWidth;
            var buttonStyle = EditorStyles.miniButton;

            if (GUI.Button(buttonRect, Resources.CreateContent, buttonStyle)) 
                ShowTypeMenu(buttonRect, property);
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

        private bool AllowSceneObjects(SerializedProperty property)
        {
            var asset = property.serializedObject.targetObject;
            return asset != null && !EditorUtility.IsPersistent(asset);
        }

        private void DoObjectFieldGUI(
            Rect position,
            SerializedProperty property,
            GUIContent label)
        {
            label = EditorGUI.BeginProperty(position, label, property);

            if (ShouldShowInlineCreation())
                position.xMax -= Resources.CreateButtonWidth;

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
                SetObjectReferenceValue(property, newTarget);
        }

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

        private void ShowTypeMenu(Rect position, SerializedProperty property)
        {
            var types = TypeUtils.GetConcreteTypes(fieldInfo.FieldType);

            var menu = new GenericMenu();

            var menuBuilder = new InlineTypeContextMenuBuilder(menu, fieldInfo, property, types.Length > 16);

            if (types.Length == 1)
                menuBuilder.Choose(types[0]);
            else
                menuBuilder.Show(position, types);
            
            GUIUtility.ExitGUI();
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
                height += EditorGUI.GetPropertyHeight(property, true);
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
            EditorGUI.BeginDisabledGroup(Attribute.DisallowEditing);
            
            foreach (var property in properties)
            {
                position.y += spacing;
                position.height = EditorGUI.GetPropertyHeight(property, true);
                EditorGUI.PropertyField(position, property, true);
                position.y += position.height;
            }
            
            EditorGUI.EndDisabledGroup();
            
            if (!Attribute.DisallowEditing)
                serializedObject.ApplyModifiedProperties();
        }

        private static void DrawInlineBackground(Rect position)
        {
            if (Event.current.type != EventType.Repaint)
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
