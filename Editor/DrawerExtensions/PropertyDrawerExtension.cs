﻿using Gadget.Core;

namespace Gadget.Editor.DrawerExtensions
{
    /// <summary>
    /// <para>
    /// PropertyDrawerExtensions is a workaround for the "1 PropertyDrawer per property" limitation.
    /// They can be applied to any attribute that inherits from <see cref="GadgetPropertyAttribute"/>.
    /// The <see cref="Editor.Drawers.BasePropertyDrawer"/> is responsible for applying the effects
    /// of all drawer extensions correctly.
    /// </para>
    /// <para>
    /// Reflection is used to link the extension to the <see cref="GadgetPropertyAttribute"/>
    /// specified as the type parameter.
    /// </para>
    /// </summary>
    /// <typeparam name="T"><see cref="GadgetPropertyAttribute"/> the drawer extension is for</typeparam>
    /// <seealso cref="GadgetPropertyAttribute"/>
    /// <seealso cref="Editor.Drawers.BasePropertyDrawer"/>
    public abstract class PropertyDrawerExtension<T> : PropertyDrawerExtensionBase where T : GadgetPropertyAttribute
    {
        protected PropertyDrawerExtension(GadgetPropertyAttribute attribute) : base(attribute) {}
    }
}