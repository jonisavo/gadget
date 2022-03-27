namespace Gadget.Core
{
    /// <summary>
    /// When used on a field, it is marked as disabled in the inspector.
    /// </summary>
    /// <example>
    /// <code>
    /// [SerializeField] [Disabled]
    /// private Vector3 _currentPosition;
    /// </code>
    /// </example>
    public class DisabledAttribute : GadgetPropertyAttribute {}
}