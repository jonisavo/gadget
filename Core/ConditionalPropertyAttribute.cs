using UnityEngine;

namespace EditorAttributesLite.Core
{
    public abstract class ConditionalPropertyAttribute : PropertyAttribute
    {
        public string FieldName { get; }
        
        public ConditionalPropertyAttribute(string fieldName)
        {
            FieldName = fieldName;
        }
    }
}