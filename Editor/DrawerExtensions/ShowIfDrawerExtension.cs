using Gadget.Core;
using UnityEditor;

namespace Gadget.Editor.DrawerExtensions
{
    public class ShowIfDrawerExtension : ConditionalPropertyDrawerExtensionBase<ShowIfAttribute>
    {
        public ShowIfDrawerExtension(BasePropertyAttribute attribute) : base(attribute) {}

        public override bool IsVisible(SerializedProperty property)
        {
            if (!TryGetBooleanField(property, out var shouldShow))
                return true;

            return shouldShow;
        }
    } 
}

