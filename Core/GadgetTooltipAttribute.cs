namespace Gadget.Core
{
    public class GadgetTooltipAttribute : GadgetPropertyAttribute
    {
        public readonly string Text;

        public GadgetTooltipAttribute(string tooltip)
        {
            Text = tooltip;
        }
    }
}