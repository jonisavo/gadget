using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Gadget.Editor.Internal.ContextMenus
{
    internal abstract class ContextMenuBuilder<T>
    {
        protected readonly GenericMenu Menu;

        protected readonly FieldInfo FieldInfo;

        protected readonly SerializedProperty Property;
        
        protected ContextMenuBuilder(GenericMenu menu, FieldInfo fieldInfo, SerializedProperty property)
        {
            Menu = menu;
            FieldInfo = fieldInfo;
            Property = property;
        }
        
        protected abstract string BuildMenuPath(T item);

        protected abstract void OnChoose(T item);

        protected abstract void BuildEmptyMenu();

        public abstract T[] GetChoices();

        public void Choose(T item)
        {
            OnChoose(item);
        }

        protected virtual void BuildMenu(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                var content = new GUIContent(BuildMenuPath(item));
                Menu.AddItem(content, false, () => OnChoose(item));
            }
        }

        public void Show(Rect position, T[] choices)
        {
            if (choices.Length == 0)
                BuildEmptyMenu();
            else
                BuildMenu(choices);

            Menu.DropDown(position);
        }
    }
}