namespace InspectorEssentials.Core
{
    /// <summary>
    /// When used on a field, it is shown only if the given boolean field is true.
    /// If the field name is prefixed with a !, the boolean field must be false.
    /// Works on properties and methods as well.
    /// </summary>
    /// <example>
    /// <code>
    /// public bool enableDebugMode;
    /// [ShowIf("enableDebugMode")]
    /// public bool invincibility;
    /// [ShowIf("!enableDebugMode")
    /// public bool enforceSecurity;
    /// public bool IsActive => gameObject.activeInHierarchy;
    /// [ShowIf("!IsActive")]
    /// public string IAmHidden = "I am hidden most of the time :)";
    /// private bool _angry;
    /// [SerializeField, ShowIf("_angry")]
    /// private int _angerLevel;
    /// </code>
    /// </example>
    public class ShowIfAttribute : ConditionalPropertyAttribute
    {
        public ShowIfAttribute(string memberName) : base(memberName) {}
    }
}