using Gadget.Core;
using UnityEditor;
using UnityEngine;

namespace Gadget.Editor.DrawerExtensions
{
    [GadgetExtensionFor(typeof(GadgetTooltipAttribute))]
    public class GadgetTooltipDrawerExtension : GadgetDrawerExtension
    {
        public GadgetTooltipDrawerExtension(GadgetPropertyAttribute attribute) : base(attribute) {}

        private string GetTooltipText()
        {
            return ((GadgetTooltipAttribute) Attribute).Text;
        }
        
        public override void OnPreGUI(Rect position, SerializedProperty property)
        {
            Label.tooltip = GetTooltipText();
        }

        public override bool IsInvalid(SerializedProperty property, out string errorMessage)
        {
            errorMessage = "Tooltip is empty or null";

            return string.IsNullOrEmpty(GetTooltipText());
        }
    }
}