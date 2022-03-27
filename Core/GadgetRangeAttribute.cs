using UnityEngine;

namespace Gadget.Core
{
    /// <summary>
    /// A version of <see cref="RangeAttribute"/> that works with other
    /// <see cref="GadgetPropertyAttribute"/> type Attributes.
    /// </summary>
    /// <example>
    /// <code>
    /// [GadgetRange(0f, 2f), Disabled]
    /// public float disabledRange;
    /// </code>
    /// </example>
    public class GadgetRangeAttribute : GadgetPropertyAttribute
    {
        public readonly float Min;
        public readonly float Max;

        public GadgetRangeAttribute(float min, float max)
        {
            Min = min;
            Max = max;
        }
    }
}