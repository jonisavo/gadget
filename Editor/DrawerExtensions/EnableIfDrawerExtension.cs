using InspectorEssentials.Core;
using UnityEditor;

namespace InspectorEssentials.Editor.DrawerExtensions
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

