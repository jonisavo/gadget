using Gadget.Core;
using UnityEditor;
using UnityEngine;

namespace Gadget.Editor.DrawerExtensions
{
    [GadgetExtensionFor(typeof(GadgetTooltipAttribute))]
    public class GadgetTooltipDrawerExtension : GadgetDrawerExtension
    {
        public GadgetTooltipDrawerExtension(GadgetPropertyAttribute attribute) : base(attribute) {}
        
        public override void OnPreGUI(Rect position, SerializedProperty property)
        {
            Label.tooltip = ((GadgetTooltipAttribute) Attribute).Text;
        }
    }
}