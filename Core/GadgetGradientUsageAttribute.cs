using UnityEngine;

namespace Gadget.Core
{
    /// <summary>
    /// A version of <see cref="GradientUsageAttribute"/> that works with other
    /// <see cref="GadgetPropertyAttribute"/> type Attributes.
    /// Has additional <c>ColorSpace</c> and <c>HDR</c> properties for
    /// configuring the attribute without boolean arguments.
    /// </summary>
    /// <example>
    /// <code>
    /// [GadgetGradientUsage(ColorSpace.Linear, true)]
    /// public Gradient gradient;
    /// 
    /// [GadgetGradientUsage(ColorSpace = ColorSpace.Linear, HDR = true)]
    /// public Gradient anotherGradient;
    /// </code>
    /// </example>
    public class GadgetGradientUsageAttribute : GadgetPropertyAttribute
    {
        public ColorSpace ColorSpace { get; set; }

        public bool HDR { get; set; }

        public GadgetGradientUsageAttribute() {}

        public GadgetGradientUsageAttribute(bool hdr)
        {
            HDR = hdr;
        }

        public GadgetGradientUsageAttribute(bool hdr, ColorSpace colorSpace)
        {
            HDR = hdr;
            ColorSpace = colorSpace;
        }
    }
}