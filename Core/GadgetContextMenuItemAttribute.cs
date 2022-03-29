using System;
using UnityEngine;

namespace Gadget.Core
{
    /// <summary>
    /// A version of <see cref="ContextMenuItemAttribute"/> that works with other
    /// <see cref="GadgetPropertyAttribute"/> type Attributes.
    /// Has an additional constructor which can be used to set the menu item
    /// and method name at once.
    /// The method can take the field object as a parameter.
    /// </summary>
    /// <example>
    /// <code>
    /// [Disabled]
    /// [GadgetContextMenuItem("Touch")]
    /// [GadgetContextMenuItem("Peek")]
    /// [GadgetContextMenuItem("Leave Alone", "LeaveAlone")]
    /// string dontTouch = "Don't touch!";
    /// 
    /// private void Touch(string str)
    /// {
    ///     dontTouch = "You touched! :(";
    ///     Debug.Log("You touched string " + str);
    /// }
    /// 
    /// public void Peek()
    /// {
    ///     dontTouch = "You peeked! :o";
    /// }
    /// 
    /// public void LeaveAlone(object obj)
    /// {
    ///     dontTouch = "You left me alone :)";
    ///     Debug.Log("You left alone object of type " + obj.GetType());
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