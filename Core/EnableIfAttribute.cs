namespace InspectorEssentials.Core
{
    /// <summary>
    /// When used on a field, it is marked as enabled if the given boolean
    /// field is true.
    /// If the field name is prefixed with a !, the boolean field must be false.
    /// </summary>
    public class EnableIfAttribute : ConditionalPropertyAttribute
    {
        public EnableIfAttribute(string fieldName) : base(fieldName) {}
    }
}