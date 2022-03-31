using System;
using JetBrains.Annotations;

namespace Gadget.Editor.DrawerExtensions
{
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