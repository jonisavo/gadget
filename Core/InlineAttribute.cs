﻿using UnityEngine;

namespace InspectorEssentials.Core
{
    public class InlineAttribute : PropertyAttribute
    {
        public bool DisallowEditing { get; set; }
        
        public bool AllowInlineCreation { get; set; }
    }
}
