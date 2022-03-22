using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace InspectorEssentials.Editor.Internal
{
    internal static class TypeUtils
    {
        private static readonly Dictionary<Type, Type[]> 
            ConcreteTypes = new Dictionary<Type, Type[]>();
        
        public static Type[] GetConcreteTypes(Type type)
        {
            if (type.IsArray)
                type = type.GetElementType();
            
            if (ConcreteTypes.TryGetValue(type, out var concreteTypes))
                return concreteTypes;

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var types = assemblies.SelectMany(a =>
            {
                try
                {
                    return a.GetTypes();
                }
                catch (ReflectionTypeLoadException e)
                {
                    return e.Types.Where(t => t != null);
                }
            });
            concreteTypes =
                types
                    .Where(t =>
                        t.IsAbstract == false &&
                        t.IsGenericTypeDefinition == false &&
                        type.IsAssignableFrom(t))
                    .OrderBy(t => t.FullName.ToLower())
                    .ToArray();

            ConcreteTypes.Add(type, concreteTypes);
            return concreteTypes;
        }

        public static Type GetPrimaryConcreteType(Type type)
        {
            if (type.IsArray)
                return type.GetElementType();
            if (type.IsGenericType)
                return type.GenericTypeArguments[0];

            return type;
        }
    }
}