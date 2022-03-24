using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace InspectorEssentials.Editor.Internal.Utilities
{
    internal static class TypeUtils
    {
        private static readonly Dictionary<Type, Type[]> 
            ConcreteTypes = new Dictionary<Type, Type[]>();
        
        public static Type[] GetConcreteTypes(Type type)
        {
            if (type.IsArray)
                type = type.GetElementType();

            if (type == null)
                return Array.Empty<Type>();
            
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
                        t.FullName != null &&
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

        public static string GetPrimaryConcreteTypeName(Type type)
        {
            var concreteType = GetPrimaryConcreteType(type);

            return concreteType.Name;
        }
    }
}