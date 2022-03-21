using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using InspectorEssentials.Core;
using InspectorEssentials.Editor.Internal;
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

            public readonly GUIContent NoAssetsToCreate =
                new GUIContent("No assets to create");
        }

        private static GUIResources s_gui;
        private static GUIResources gui => s_gui ??= new GUIResources();

        //----------------------------------------------------------------------

        private static readonly Dictionary<Type, Type[]> 
            s_concreteTypes = new Dictionary<Type, Type[]>();

        private static Type[] GetConcreteTypes(Type type)
        {
            if (s_concreteTypes.TryGetValue(type, out var concreteTypes))
                return concreteTypes;

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var types = assemblies.SelectMany(a => a.GetTypes());
            concreteTypes =
                types
                .Where(t =>
                    t.IsAbstract == false &&
                    t.IsGenericTypeDefinition == false &&
                    type.IsAssignableFrom(t))
                .OrderBy(t => t.FullName.ToLower())
                .ToArray();

            s_concreteTypes.Add(type, concreteTypes);
            return concreteTypes;
        }

        //----------------------------------------------------------------------

        public new InlineAttribute attribute => (InlineAttribute)base.attribute;

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
                if (targetExists && !ObjectScope.Contains(target))
                {
                    var spacing = EditorGUIUtility.standardVerticalSpacing;
                    height += spacing;
                    height += GetInlinePropertyHeight(target);
                    height += 1;
                }
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
                var dropDownStyle = gui.InDropDownStyle;
                var rect = buttonRect;
                rect.x += 2;
                rect.y += 6;
                dropDownStyle.Draw(rect, false, false, false, false);
            }

            if (!GUI.Button(buttonRect, gui.CreateContent, buttonStyle))
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

            var objectType = fieldInfo.FieldType;
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
            var menu = new GenericMenu();

            var types = GetConcreteTypes(fieldInfo.FieldType);

            if (types.Length > 0)
            {
                var typeIndex = 0;
                var useTypeFullName = types.Length > 16;
                foreach (var type in types)
                {
                    var createAssetMenuAttribute =
                        (CreateAssetMenuAttribute)
                        type.GetCustomAttribute(
                            typeof(CreateAssetMenuAttribute));
                    var menuPath =
                        createAssetMenuAttribute != null
                        ? createAssetMenuAttribute.menuName
                        : useTypeFullName
                        ? type.FullName.Replace('.', '/')
                        : type.Name;
                    var menuTypeIndex = typeIndex++;
                    menu.AddItem(
                        new GUIContent(menuPath),
                        on: false,
                        func: () =>
                            CreateAsset(property, types[menuTypeIndex]));
                }
            }
            else
            {
                menu.AddDisabledItem(gui.NoAssetsToCreate);
            }

            menu.DropDown(position);
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
            using (ColorAlphaScope(0.5f))
            {
                style.Draw(position, false, false, false, false);
            }
            // EditorGUI.DrawRect()
        }

        //----------------------------------------------------------------------

        private readonly Dictionary<Object, SerializedObject> 
            m_serializedObjectMap = new Dictionary<Object, SerializedObject>();

        private SerializedObject GetSerializedObject(Object target)
        {
            Debug.Assert(target != null);
            
            if (m_serializedObjectMap.TryGetValue(target, out var serializedObject))
                return serializedObject;

            serializedObject = new SerializedObject(target);
            m_serializedObjectMap.Add(target, serializedObject);
            return serializedObject;
        }

        private void DiscardObsoleteSerializedObjects()
        {
            var destroyedObjects =
                m_serializedObjectMap.Keys.Where(key => key == null);

            var enumerable = destroyedObjects as Object[] ?? destroyedObjects.ToArray();
            
            if (!enumerable.Any())
                return;
            
            foreach (var @object in enumerable.ToArray())
            {
                m_serializedObjectMap.Remove(@object);
            }
        }

        private void DiscardObsoleteSerializedObjectsOnNextEditorUpdate()
        {
            EditorApplication.delayCall -= DiscardObsoleteSerializedObjects;
            EditorApplication.delayCall += DiscardObsoleteSerializedObjects;
        }

        //----------------------------------------------------------------------


        private void CreateAsset(SerializedProperty property, Type type)
        {
            if (!ScriptableObjectUtils.TryCreateNewAsset(type, out var scriptableObject))
                return;
            
            SetObjectReferenceValue(property, scriptableObject);
        }

        //----------------------------------------------------------------------

        private readonly struct ObjectScope : IDisposable
        {
            private static readonly HashSet<int> s_objectScopeSet =
                new HashSet<int>();

            private readonly int m_instanceID;

            public ObjectScope(Object obj)
            {
                m_instanceID = obj.GetInstanceID();
                s_objectScopeSet.Add(m_instanceID);
            }

            public void Dispose()
            {
                s_objectScopeSet.Remove(m_instanceID);
            }

            public static bool Contains(Object obj)
            {
                if (obj == null)
                    return false;
                var instanceID = obj.GetInstanceID();
                return s_objectScopeSet.Contains(instanceID);
            }
        }

        //======================================================================

        protected struct Deferred : IDisposable
        {
            private readonly Action _onDispose;

            public Deferred(Action onDispose)
            {
                _onDispose = onDispose;
            }

            public void Dispose()
            {
                if (_onDispose != null)
                    _onDispose();
            }
        }

        protected static Deferred ColorScope(Color newColor)
        {
            var oldColor = GUI.color;
            GUI.color = newColor;
            return new Deferred(() => GUI.color = oldColor);
        }

        protected static Deferred ColorAlphaScope(float a)
        {
            var oldColor = GUI.color;
            GUI.color = new Color(1, 1, 1, a);
            return new Deferred(() => GUI.color = oldColor);
        }

        protected static Deferred IndentLevelScope(int indent = 1)
        {
            EditorGUI.indentLevel += indent;
            return new Deferred(() => EditorGUI.indentLevel -= indent);
        }
    }

}
