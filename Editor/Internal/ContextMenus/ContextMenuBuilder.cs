using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace InspectorEssentials.Editor.Internal.ContextMenus
{
    public abstract class ContextMenuBuilder<T>
    {
        protected abstract string BuildMenuPath(T item);

        protected abstract void OnChoose(T item, SerializedProperty property);

        protected abstract void BuildEmptyMenu(GenericMenu menu);

        protected abstract T[] GetChoices(FieldInfo fieldInfo, SerializedProperty property);

        protected virtual void BuildMenu(
            GenericMenu menu,
            IEnumerable<T> items,
            FieldInfo fieldInfo,
            SerializedProperty property)
        {
            foreach (var item in items)
            {
                var content = new GUIContent(BuildMenuPath(item));
                menu.AddItem(content, false, () => OnChoose(item, property));
            }
        }

        public void Show(Rect position, FieldInfo fieldInfo, SerializedProperty property)
        {
            var menu = new GenericMenu();
            
            var choices = GetChoices(fieldInfo, property);

            if (choices.Length == 0)
                BuildEmptyMenu(menu);
            else
                BuildMenu(menu, choices, fieldInfo, property);

            menu.DropDown(position);
        }
    }
}