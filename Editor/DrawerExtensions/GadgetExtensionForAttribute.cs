using System;
using Gadget.Core;
using JetBrains.Annotations;

namespace Gadget.Editor.DrawerExtensions
{
    /// <summary>
    /// When used on a <see cref="GadgetDrawerExtension"/>, marks it as an
    /// extension for the given <see cref="GadgetPropertyAttribute"/> type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    [MeansImplicitUse]
    [BaseTypeRequired(typeof(GadgetDrawerExtension))]
    public class GadgetExtensionForAttribute : Attribute
    {
        public readonly Type ExtendsAttributeType;

        public GadgetExtensionForAttribute(Type extendsAttributeType)
        {
            ExtendsAttributeType = extendsAttributeType;
        }
    }
}