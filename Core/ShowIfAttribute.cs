﻿namespace InspectorEssentials.Core
{
    /// <summary>
    /// When used on a field, it is shown only if the given boolean field is true.
    /// If the field name is prefixed with a !, the boolean field must be false.
    /// </summary>
    /// <example>
    /// <code>
    /// public bool enableDebugMode;
    /// [ShowIf("enableDebugMode")]
    /// public bool invincibility;
    /// [ShowIf("!enableDebugMode")
    /// public bool enforceSecurity;
    /// </code>
    /// </example>
    public class ShowIfAttribute : ConditionalPropertyAttribute
    {
        public ShowIfAttribute(string fieldName) : base(fieldName) {}
    }
}