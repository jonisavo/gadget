using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Gadget.Core;
using UnityEngine;

namespace Gadget.Editor.Internal.Utilities
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
            concreteTypes = types
                .Where(t => IsTypeConcrete(t) && type.IsAssignableFrom(t))
                .OrderBy(t => t.FullName.ToLower())
                .ToArray();

            ConcreteTypes.Add(type, concreteTypes);
            return concreteTypes;
        }

        public static bool IsTypeConcrete(Type type)
        {
            var typeIsSealedClass = type.IsClass && type.IsSealed;

            return !type.IsAbstract &&
                   !type.IsGenericTypeDefinition &&
                   !type.IsArray &&
                   !typeIsSealedClass &&
                   type.FullName != null &&
                   type.GetCustomAttribute(typeof(ObsoleteAttribute)) == null;
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
                type.GetCustomAttribute(typeof(TypeMenuPathAttribute));

            if (attribute is TypeMenuPathAttribute typeMenuNameAttribute)
                return typeMenuNameAttribute.MenuPath;

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

            return $"{parts[parts.Length - 1]}";
        }

        private const BindingFlags MemberBindingFlags =
            BindingFlags.Instance | BindingFlags.Static |
            BindingFlags.Public | BindingFlags.NonPublic;

        public static bool TryGetValueFromMemberOfObject<T>(object obj, string memberName, out T result)
        {
            result = default;

            var targetObjectType = obj.GetType();

            var memberInfo = targetObjectType.GetMember(
                memberName,
                MemberTypes.Method | MemberTypes.Property | MemberTypes.Field,
                MemberBindingFlags
            );

            foreach (var member in memberInfo)
            {
                return member.MemberType switch
                {
                    MemberTypes.Field => TryGetFieldOfObject(obj, memberName, out result),
                    MemberTypes.Method => TryInvokeMethodOfObject(obj, memberName, out result),
                    MemberTypes.Property => TryInvokePropertyOfObject(obj, memberName, out result),
                    _ => throw new ArgumentOutOfRangeException()
                };
            }

            return false;
        }

        private static bool TryGetFieldOfObject<T>(object obj, string name, out T result)
        {
            result = default;

            var objectType = obj.GetType();

            var field = objectType.GetField(name, MemberBindingFlags);

            if (field == null || field.FieldType != typeof(T))
                return false;

            result = (T) field.GetValue(obj);
            
            return true;
        }

        public static bool TryGetMethodOfObject(object obj, string name, out MethodInfo methodInfo)
        {
            methodInfo = null;

            if (obj == null)
                return false;

            var objectType = obj.GetType();

            methodInfo = objectType.GetMethod(name, MemberBindingFlags);

            return methodInfo != null;
        }

        private static bool TryInvokeMethodOfObject<T>(object obj, string name, out T result)
        {
            result = default;

            var objectType = obj.GetType();

            var method = objectType.GetMethod(name, MemberBindingFlags);

            if (method == null || method.ReturnType != typeof(T))
                return false;

            result = (T) method.Invoke(obj, null);

            return true;
        }

        private static bool TryInvokePropertyOfObject<T>(object obj, string name, out T result)
        {
            result = default;
            
            var objectType = obj.GetType();
            
            var objectProperty = objectType.GetProperty(name, MemberBindingFlags);

            if (objectProperty == null ||
                !objectProperty.CanRead ||
                objectProperty.PropertyType != typeof(bool))
            {
                return false;
            }

            var accessors = objectProperty.GetAccessors(true)
                .Where(accessor => accessor.ReturnType == typeof(T))
                .ToArray();

            if (accessors.Length == 0)
                return false;

            result = (T) accessors[0].Invoke(obj, null);

            return true;
        }
    }
}