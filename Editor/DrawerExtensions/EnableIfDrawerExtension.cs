using Gadget.Core;
using UnityEditor;

namespace Gadget.Editor.DrawerExtensions
{
    public class EnableIfDrawerExtension : ConditionalPropertyDrawerExtensionBase<EnableIfAttribute>
    {
        public EnableIfDrawerExtension(BasePropertyAttribute attribute) : base(attribute) {}

        public override bool IsEnabled(SerializedProperty property)
        {
            if (!TryGetBooleanField(property, out var shouldEnable))
                return true;

            return shouldEnable;
        }
    } 
}

