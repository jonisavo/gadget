using UnityEngine;

namespace Gadget.Core
{
    /// <summary>
    /// A version of <see cref="MinAttribute"/> that works with other
    /// <see cref="GadgetPropertyAttribute"/> type Attributes.
    /// <example>
    /// <code>
    /// [GadgetMin(0f)]
    /// public float value;
    /// </code>
    /// </example>
    /// </summary>
    public class GadgetMinAttribute : GadgetPropertyAttribute
    {
        public readonly float MinValue;

        public GadgetMinAttribute(float min)
        {
            MinValue = min;
        }
    }
}