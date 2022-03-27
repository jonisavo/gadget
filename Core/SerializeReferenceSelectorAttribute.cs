using UnityEngine;

namespace Gadget.Core
{
    /// <summary>
    /// An attribute, when applied to a field with <see cref="SerializeReference"/>, allows
    /// its user to select a type which inherits from the field type.
    /// </summary>
    /// <example>
    /// <code>
    /// [Serializable]
    /// public abstract class Character
    /// {
    ///     public string name;
    ///     public abstract void Attack();
    /// }
    ///
    /// [Serializable]
    /// public class Hero : Character
    /// {
    ///     public float health;
    ///     public float strength;
    ///     public override void Attack() { /* ... */ }
    /// }
    ///
    /// [SerializeReference, SerializeReferenceSelector]
    /// public Character character;
    ///
    /// [SerializeReference, SerializeReferenceSelector]
    /// public Character[] characters;
    ///
    /// public interface IEvent
    /// {
    ///     public void Trigger();
    /// }
    ///
    /// [Serializable]
    /// public struct PlayerDamageEvent : IEvent
    /// {
    ///     public float damageAmount;
    /// }
    ///
    /// [SerializeReference, SerializeReferenceSelector]
    /// public IEvent eventField;
    ///
    /// [SerializeReference, SerializeReferenceSelector]
    /// public IEvent[] events;
    /// </code>
    /// </example>
    /// <seealso cref="SerializeReference"/>
    /// <seealso cref="TypeMenuPathAttribute"/>
    public class SerializeReferenceSelectorAttribute : GadgetPropertyAttribute {}
}