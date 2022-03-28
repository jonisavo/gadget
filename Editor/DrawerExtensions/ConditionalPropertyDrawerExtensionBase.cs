using System.Reflection;
using Gadget.Core;
using Gadget.Editor.Internal.Utilities;
using UnityEditor;

namespace Gadget.Editor.DrawerExtensions
{
    /// <summary>
    /// Used internally as a base drawer class for conditional properties.
    /// </summary>
    /// <typeparam name="T">ConditionalPropertyAttribute type</typeparam>
    public abstract class ConditionalPropertyDrawerExtensionBase<T> : PropertyDrawerExtension<T>
        where T : ConditionalPropertyAttribute
    {
        protected ConditionalPropertyDrawerExtensionBase(GadgetPropertyAttribute attribute) : base(attribute) {}

        private new T Attribute => base.Attribute as T;
        
        private string MemberName
        {
            get
            {
                if (Attribute == null)
                    return null;
                
                var memberName = Attribute.MemberName;

                if (memberName[0] == '!')
                    memberName = memberName.Remove(0, 1);
                
                return memberName;
            }
        }
        
        private bool Inverted => Attribute.MemberName[0] == '!';

        public override bool IsInvalid(out string errorMessage)
        {
            errorMessage = null;

            if (Attribute == null)
            {
                errorMessage = "Attribute not found. This is likely a bug.";
                return true;
            }

            if (TryGetBooleanField(Property, out _))
                return false;

            errorMessage = $"Member {Attribute.MemberName} not found or is not a boolean";
            return true;
        }
        
        protected bool TryGetBooleanField(SerializedProperty property, out bool value)
        {
            value = true;

            if (string.IsNullOrEmpty(MemberName))
                return false;

            var targetObject = property.serializedObject.targetObject;

            if (targetObject == null)
                return false;

            if (!TypeUtils.TryGetValueFromMemberOfObject(targetObject, MemberName, out value))
                return false;

            if (Inverted)
                value = !value;

            return true;
        }
    }
}