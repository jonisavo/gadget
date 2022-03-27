using UnityEngine;

namespace Gadget.Core
{
    /// <summary>
    /// Displays <see cref="UnityEngine.Object"/> fields with a foldout, which
    /// shows its properties. Additionally shows a Create button on assets which
    /// support it.
    /// </summary>
    /// <example>
    /// <code>
    /// public class Hero : ScriptableObject
    /// {
    ///     public string name;
    ///     public float health;
    /// }
    ///
    /// // Will display a Create button
    /// [Inline]
    /// public Hero hero;
    ///
    /// [Inline]
    /// public Hero[] hero;
    ///
    /// // Will display a Create button
    /// [Inline]
    /// public Material material;
    ///
    /// // Will not display a Create button or allow editing the object
    /// [Inline(DisallowEditing = true, DisallowInlineCreation = true)]
    /// public Hero secretHero;
    ///
    /// // Will not display a Create button, as Animator does not support it
    /// [Inline]
    /// public Animator animator;
    /// </code>
    /// </example>
    /// <seealso cref="TypeMenuPathAttribute"/>
    public class InlineAttribute : GadgetPropertyAttribute
    {
        public bool DisallowEditing { get; set; }
        
        public bool DisallowInlineCreation { get; set; }
    }
}
