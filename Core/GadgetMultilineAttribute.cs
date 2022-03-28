using UnityEngine;

namespace Gadget.Core
{
    /// <summary>
    /// A version of <see cref="MultilineAttribute"/> that works with other
    /// <see cref="GadgetPropertyAttribute"/> type Attributes.
    /// Has a Lines property.
    /// <example>
    /// <code>
    /// [GadgetMultiline]
    /// public string message;
    ///
    /// [GadgetMultiline(5)]
    /// public string messageWithFiveLines;
    ///
    /// [GadgetMultiline(Lines = 5)]
    /// public string otherMessageWithFiveLines;
    /// </code>
    /// </example>
    /// </summary>
    public class GadgetMultilineAttribute : GadgetPropertyAttribute
    {
        public int Lines { get; set; } = 3;

        public GadgetMultilineAttribute() {}

        public GadgetMultilineAttribute(int lines)
        {
            Lines = lines;
        }
    }
}