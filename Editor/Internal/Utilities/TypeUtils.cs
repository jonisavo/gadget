using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using InspectorEssentials.Core;
using UnityEngine;

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

        public enum GetMenuPathMode
        {
            IgnoreTypeFullName,
            UseTypeFullName
        }

        public static string GetMenuPathForType(
            Type type,
            GetMenuPathMode menuPathMode = GetMenuPathMode.IgnoreTypeFullName)
        {
            var attribute =
                type.GetCustomAttribute(typeof(TypeMenuNameAttribute));

            if (attribute is TypeMenuNameAttribute typeMenuNameAttribute)
                return typeMenuNameAttribute.MenuName;
            
            attribute =
                type.GetCustomAttribute(typeof(CreateAssetMenuAttribute));

            if (attribute is CreateAssetMenuAttribute createAssetMenuAttribute)
                return createAssetMenuAttribute.menuName;

            if (menuPathMode == GetMenuPathMode.UseTypeFullName &&
                !string.IsNullOrEmpty(type.FullName))
            {
                return type.FullName.Replace('.', '/');
            }

            return type.Name;
        }

        public static string GetShownTypeName(string fullTypeName)
        {
            var typeName = fullTypeName;

            var index = fullTypeName.LastIndexOf(' ');

            if (index >= 0)
                typeName = typeName.Substring(index + 1);

            var assemblyName = fullTypeName.Substring(0, index);

            var assembly = Assembly.Load(assemblyName);

            var type = assembly.GetType(typeName);

            var menuPath = GetMenuPathForType(type, GetMenuPathMode.UseTypeFullName);
            
            var parts = menuPath.Split('/');
            
            return $"{parts[parts.Length - 1]} ({assemblyName})";
        }
    }
}