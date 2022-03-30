using UnityEngine;

namespace Gadget.Core
{
    /// <summary>
    /// A version of <see cref="TooltipAttribute"/> that works with other
    /// <see cref="GadgetPropertyAttribute"/> type Attributes.
    /// </summary>
    public class GadgetTooltipAttribute : GadgetPropertyAttribute
    {
        public readonly string Text;

        public GadgetTooltipAttribute(string tooltip)
        {
            Text = tooltip;
        }
    }
}