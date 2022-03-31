using Gadget.Core;
using UnityEditor;

namespace Gadget.Editor.DrawerExtensions
{
    [GadgetExtensionFor(typeof(EnableIfAttribute))]
    public class EnableIfDrawerExtension : ConditionalGadgetDrawerExtensionBase<EnableIfAttribute>
    {
        public EnableIfDrawerExtension(EnableIfAttribute attribute) : base(attribute) {}

        public override bool IsEnabled(SerializedProperty property)
        {
            if (!TryGetBooleanField(property, out var shouldEnable))
                return true;

            return shouldEnable;
        }
    } 
}

