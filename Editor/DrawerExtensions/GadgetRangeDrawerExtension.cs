﻿using System.Reflection;
using Gadget.Core;
using UnityEditor;
using UnityEngine;

namespace Gadget.Editor.DrawerExtensions
{
    public class GadgetRangeDrawerExtension : PropertyDrawerExtension<GadgetRangeAttribute>
    {
        public GadgetRangeDrawerExtension(GadgetPropertyAttribute attribute) : base(attribute) {}

        private GadgetRangeAttribute RangeAttribute
        {
            get
            {
                if (_rangeAttribute == null)
                    _rangeAttribute = (GadgetRangeAttribute) Attribute;

                return _rangeAttribute;
            }
        }

        private GadgetRangeAttribute _rangeAttribute;

        public override bool TryOverrideMainGUI(Rect position)
        {
            if (!IsPropertyValid(Property))
                return false;
            
            switch (Property.propertyType)
            {
                case SerializedPropertyType.Integer:
                    EditorGUI.IntSlider(position, Property,
                        (int) RangeAttribute.Min, (int) RangeAttribute.Max, Content);
                    break;
                case SerializedPropertyType.Float:
                    EditorGUI.Slider(position, Property,
                        RangeAttribute.Min, RangeAttribute.Max);
                    break;
                default:
                    return false;
            }

            return true;
        }

        public override bool IsInvalid(out string errorMessage)
        {
            errorMessage = $"Field {FieldInfo.Name} is not an integer or a float";
            return !IsPropertyValid(Property);
        }

        private static bool IsPropertyValid(SerializedProperty property)
        {
            return property.propertyType == SerializedPropertyType.Integer ||
                   property.propertyType == SerializedPropertyType.Float;
        }
    }
}