using Gadget.Core;
using UnityEditor;

namespace Gadget.Editor.DrawerExtensions
{
    public class DisabledDrawerExtension : PropertyDrawerExtension<DisabledAttribute>
    {
        public DisabledDrawerExtension(GadgetPropertyAttribute attribute) : base(attribute) {}

        public override bool IsEnabled()
        {
            return false;
        }
    }
}