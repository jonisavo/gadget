using System.Collections.Generic;
using Gadget.Core;
using UnityEditor;

namespace Gadget.Editor.DrawerExtensions
{
    [GadgetExtensionFor(typeof(ShowIfAttribute))]
    public class ShowIfDrawerExtension : ConditionalGadgetDrawerExtensionBase<ShowIfAttribute>
    {
        public ShowIfDrawerExtension(ShowIfAttribute attribute) : base(attribute) {}

        public override bool IsVisible(SerializedProperty property)
        {
            if (FieldIsArrayOrList())
                return true;
            
            if (!TryGetBooleanField(property, out var shouldShow))
                return true;

            return shouldShow;
        }

        public override bool IsInvalid(SerializedProperty property, out string errorMessage)
        {
            errorMessage = $"Field {FieldInfo.Name} is an array or List<>.";
            return FieldIsArrayOrList();
        }

        private bool FieldIsArrayOrList()
        {
            return FieldInfo.FieldType.IsArray || FieldInfo.FieldType == typeof(List<>);
        }
    } 
}

