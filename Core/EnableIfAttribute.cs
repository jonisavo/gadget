namespace InspectorEssentials.Core
{
    /// <summary>
    /// When used on a field, it is marked as enabled if the given boolean
    /// field is true.
    /// If the field name is prefixed with a !, the boolean field must be false.
    /// Works on properties and methods as well.
    /// </summary>
    /// <example>
    /// <code>
    /// public bool followPlayer;
    /// [EnableIf("followPlayer")]
    /// public Transform playerPosition;
    /// [EnableIf("!followPlayer")
    /// public bool goShopping;
    /// public bool IsActive => gameObject.activeInHierarchy;
    /// private static bool IsHappyHour()
    /// {
    ///     var hour = DateTime.Now.Hour;
    ///     return hour == 21 || hour == 22;   
    /// }
    /// [EnableIf("IsHappyHour")]
    /// public bool happyHourBooleanField;
    /// </code>
    /// </example>
    public class EnableIfAttribute : ConditionalPropertyAttribute
    {
        public EnableIfAttribute(string memberName) : base(memberName) {}
    }
}