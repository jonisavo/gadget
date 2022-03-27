using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using InspectorEssentials.Core;
using InspectorEssentials.Editor.Internal.ContextMenus;
using InspectorEssentials.Editor.Internal.Scope;
using InspectorEssentials.Editor.Internal.Utilities;
using UnityEditor;
using UnityEngine;

using Object = UnityEngine.Object;

namespace InspectorEssentials.Editor.DrawerExtensions
{
    public class InlineDrawerExtension : PropertyDrawerExtension<InlineAttribute>
    {
        public InlineDrawerExtension(BasePropertyAttribute attribute) : base(attribute) {}
        
        private sealed class GUIResources
        {
            public readonly GUIContent CreateContent =
                new GUIContent("Create");

            public readonly float CreateButtonWidth = 54f;
        }

        private static GUIResources _guiResources;
        private static GUIResources Resources => _guiResources ??= new GUIResources();

        public override bool CanCacheInspectorGUI(SerializedProperty property)
        {
            return false;
        }

        public override bool TryOverrideHeight(
            float currentHeight,
            DrawerExtensionCallbackInfo info,
            out float newHeight)
        {
            newHeight = currentHeight;

            var property = info.Property;

            if (!property.isExpanded)
                return false;

            if (!IsPropertyValid(property))
                return false;
            
            var serializedObject = property.serializedObject;
            var asset = serializedObject.targetObject;
            using (new ObjectScope(asset))
            {
                var target = property.objectReferenceValue;
                var targetExists = target != null;
                
                if (!targetExists || ObjectScope.Contains(target))
                    return true;
                
                var spacing = EditorGUIUtility.standardVerticalSpacing;
                newHeight += spacing;
                newHeight += GetInlinePropertyHeight(target);
                newHeight += 1;
            }
            return true;
        }

        public override bool TryOverrideMainGUI(DrawerExtensionCallbackInfo info)
        {
            if (!IsPropertyValid(info.Property))
                return false;
            
            var propertyRect = info.Position;
            propertyRect.height = EditorGUIUtility.singleLineHeight;

            if (ShouldShowInlineCreation(info.FieldInfo)) 
                DoInlineCreationGUI(propertyRect, info);
            
            DoObjectFieldGUI(propertyRect, info);

            if (info.Property.objectReferenceValue)
                DoFoldoutGUI(propertyRect, info.Property);

            if (info.Property.isExpanded)
                DrawExpandedDrawer(info.Position, propertyRect, info.Property);

            DiscardObsoleteSerializedObjectsOnNextEditorUpdate();

            return true;
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

        public override bool IsInvalid(SerializedProperty property, FieldInfo fieldInfo, out string errorMessage)
        {
            errorMessage = $"Field {fieldInfo.Name} is invalid as InlineAttribute works only on Object references.";
            return !IsPropertyValid(property);
        }

        private static bool IsPropertyValid(SerializedProperty property)
        {
            return property.propertyType == SerializedPropertyType.ObjectReference;
        }
        
        private bool ShouldShowInlineCreation(FieldInfo fieldInfo)
        {
            var inlineAttribute = (InlineAttribute) Attribute;
            return !inlineAttribute.DisallowInlineCreation &&
                   InlineUtils.DoesTypeSupportInlineCreation(fieldInfo.FieldType);
        }

        private static void DoInlineCreationGUI(Rect position, DrawerExtensionCallbackInfo info)
        {
            var buttonRect = position;
            buttonRect.xMin = buttonRect.xMax - Resources.CreateButtonWidth;
            var buttonStyle = EditorStyles.miniButton;

            if (GUI.Button(buttonRect, Resources.CreateContent, buttonStyle)) 
                ShowTypeMenu(buttonRect, info);
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

        private static bool AllowSceneObjects(SerializedProperty property)
        {
            var asset = property.serializedObject.targetObject;
            return asset != null && !EditorUtility.IsPersistent(asset);
        }

        private void DoObjectFieldGUI(Rect position, DrawerExtensionCallbackInfo info)
        {
            var label = EditorGUI.BeginProperty(position, info.Content, info.Property);

            if (ShouldShowInlineCreation(info.FieldInfo))
                position.xMax -= Resources.CreateButtonWidth;

            var objectType = TypeUtils.GetPrimaryConcreteType(info.FieldInfo.FieldType);

            var oldTarget = info.Property.objectReferenceValue;
            
            var newTarget =
                EditorGUI.ObjectField(
                    position,
                    label,
                    oldTarget,
                    objectType,
                    AllowSceneObjects(info.Property));

            EditorGUI.EndProperty();
            
            if (!ReferenceEquals(newTarget, oldTarget))
                SetObjectReferenceValue(info.Property, newTarget);
        }

        private static void DoFoldoutGUI(
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

        private static void ShowTypeMenu(Rect position, DrawerExtensionCallbackInfo info)
        {
            var types = TypeUtils.GetConcreteTypes(info.FieldInfo.FieldType);

            var menu = new GenericMenu();

            var menuBuilder = new InlineTypeContextMenuBuilder(menu, info.FieldInfo, info.Property, types.Length > 16);

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
            var inlineAttribute = (InlineAttribute) Attribute;
            EditorGUI.BeginDisabledGroup(inlineAttribute.DisallowEditing);
            
            EditorGUI.BeginChangeCheck();
            
            foreach (var property in properties)
            {
                position.y += spacing;
                position.height = EditorGUI.GetPropertyHeight(property, true);
                EditorGUI.PropertyField(position, property, true);
                position.y += position.height;
            }
            
            EditorGUI.EndDisabledGroup();
            
            if (EditorGUI.EndChangeCheck() && !inlineAttribute.DisallowEditing)
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
