namespace Gadget.Core
{
    /// <summary>
    /// Used internally as a base class for conditional PropertyAttributes.
    /// </summary>
    public abstract class ConditionalPropertyAttribute : GadgetPropertyAttribute
    {
        public string MemberName { get; }
        
        protected ConditionalPropertyAttribute(string memberName)
        {
            MemberName = memberName;
        }
    }
}