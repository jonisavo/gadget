using UnityEngine;

namespace InspectorEssentials.Core
{
    /// <summary>
    /// Used internally as a base class for conditional PropertyAttributes.
    /// </summary>
    public abstract class ConditionalPropertyAttribute : PropertyAttribute
    {
        public string FieldName { get; }
        
        protected ConditionalPropertyAttribute(string fieldName)
        {
            FieldName = fieldName;
        }
    }
}