using System;
using UnityEngine;

namespace Gadget.Core
{
    /// <summary>
    /// A version of <see cref="ContextMenuItemAttribute"/> that works with other
    /// <see cref="GadgetPropertyAttribute"/> type Attributes.
    /// Has an additional constructor which can be used to set the menu item
    /// and method name at once.
    /// </summary>
    /// <example>
    /// <code>
    /// [Disabled]
    /// [GadgetContextMenuItem("Touch")]
    /// [GadgetContextMenuItem("Peek")]
    /// [GadgetContextMenuItem("Leave Alone", "LeaveAlone")]
    /// string dontTouch = "Don't touch!";
    /// 
    /// private void Touch()
    /// {
    ///     dontTouch = "You touched! :(";
    /// }
    /// 
    /// public void Peek()
    /// {
    ///     dontTouch = "You peeked! :o";
    /// }
    /// 
    /// public void LeaveAlone()
    /// {
    ///     dontTouch = "You left me alone :)";
    /// }
    /// </code>
    /// </example>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class GadgetContextMenuItemAttribute : GadgetPropertyAttribute
    {
        public readonly string MenuItemName;

        public readonly string MethodName;

        public GadgetContextMenuItemAttribute(string name)
        {
            MenuItemName = name;
            MethodName = name;
        }

        public GadgetContextMenuItemAttribute(string menuItemName, string methodName)
        {
            MenuItemName = menuItemName;
            MethodName = methodName;
        }
    }
}