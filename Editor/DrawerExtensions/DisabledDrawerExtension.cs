﻿using Gadget.Core;
using UnityEditor;

namespace Gadget.Editor.DrawerExtensions
{
    public class DisabledDrawerExtension : PropertyDrawerExtension<DisabledAttribute>
    {
        public DisabledDrawerExtension(BasePropertyAttribute attribute) : base(attribute) {}

        public override bool IsEnabled(SerializedProperty property)
        {
            return false;
        }
    }
}