using Gadget.Core;
using UnityEditor;
using UnityEngine;

namespace Gadget.Editor.DrawerExtensions
{
    public class GadgetTooltipDrawerExtension : PropertyDrawerExtension<GadgetTooltipAttribute>
    {
        public GadgetTooltipDrawerExtension(GadgetPropertyAttribute attribute) : base(attribute) {}
        
        public override void OnPreGUI(Rect position, SerializedProperty property)
        {
            Label.tooltip = ((GadgetTooltipAttribute) Attribute).Text;
        }
    }
}