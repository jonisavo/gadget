using Gadget.Core;
using UnityEditor;

namespace Gadget.Editor.DrawerExtensions
{
    public class ShowIfDrawerExtension : ConditionalPropertyDrawerExtensionBase<ShowIfAttribute>
    {
        public ShowIfDrawerExtension(GadgetPropertyAttribute attribute) : base(attribute) {}

        public override bool IsVisible()
        {
            if (!TryGetBooleanField(CurrentProperty, out var shouldShow))
                return true;

            return shouldShow;
        }
    } 
}

