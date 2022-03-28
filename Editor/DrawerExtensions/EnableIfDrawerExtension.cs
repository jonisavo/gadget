using Gadget.Core;
using UnityEditor;

namespace Gadget.Editor.DrawerExtensions
{
    public class EnableIfDrawerExtension : ConditionalPropertyDrawerExtensionBase<EnableIfAttribute>
    {
        public EnableIfDrawerExtension(GadgetPropertyAttribute attribute) : base(attribute) {}

        public override bool IsEnabled()
        {
            if (!TryGetBooleanField(Property, out var shouldEnable))
                return true;

            return shouldEnable;
        }
    } 
}

