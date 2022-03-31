using Gadget.Core;
using UnityEditor;

namespace Gadget.Editor.DrawerExtensions
{
    [GadgetExtensionFor(typeof(DisabledAttribute))]
    public class DisabledDrawerExtension : GadgetDrawerExtension
    {
        public DisabledDrawerExtension(GadgetPropertyAttribute attribute) : base(attribute) {}

        public override bool IsEnabled(SerializedProperty property)
        {
            return false;
        }
    }
}