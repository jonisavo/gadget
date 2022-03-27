namespace Gadget.Core
{
    /// <summary>
    /// Used internally as a base class for conditional PropertyAttributes.
    /// </summary>
    public abstract class ConditionalPropertyAttribute : BasePropertyAttribute
    {
        public string MemberName { get; }
        
        protected ConditionalPropertyAttribute(string memberName)
        {
            MemberName = memberName;
        }
    }
}