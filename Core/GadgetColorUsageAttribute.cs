using UnityEngine;

namespace Gadget.Core
{
    /// <summary>
    /// A version of <see cref="ColorUsageAttribute"/> that works with other
    /// <see cref="GadgetPropertyAttribute"/> type Attributes.
    /// Has an additional property <c>ShowEyedropper</c> which controls
    /// whether the eye dropper button should be visible.
    /// Has additional <c>ShowAlpha</c> and <c>HDR</c> properties for
    /// configuring the attribute without boolean arguments.
    /// </summary>
    /// <example>
    /// <code>
    /// [GadgetColorUsage(true, true)]
    /// public Color color;
    /// 
    /// [GadgetColorUsage(ShowAlpha = false, HDR = true, ShowEyedropper = false)]
    /// public Color anotherColor;
    /// </code>
    /// </example>
    public class GadgetColorUsageAttribute : GadgetPropertyAttribute
    {
        public bool ShowAlpha { get; set; }

        public bool ShowEyedropper { get; set; } = true;
        
        public bool HDR { get; set; }

        public GadgetColorUsageAttribute() {}

        public GadgetColorUsageAttribute(bool showAlpha)
        {
            ShowAlpha = showAlpha;
        }

        public GadgetColorUsageAttribute(bool showAlpha, bool hdr)
        {
            ShowAlpha = showAlpha;
            HDR = hdr;
        }
    }
}