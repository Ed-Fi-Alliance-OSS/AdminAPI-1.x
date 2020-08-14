// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq;
using System.Reflection;

namespace EdFi.Ods.AdminApp.Web.Helpers
{
    public static class ReflectionExtensions
    {
        public static bool HasAttribute<T>(this ICustomAttributeProvider provider) where T : Attribute
        {
            if (provider == null)
                return false;
            return provider.GetCustomAttributes(typeof(T), true).Any();
        }

        public static T GetAttribute<T>(this ICustomAttributeProvider provider) where T : Attribute
        {
            return (T)provider.GetCustomAttributes(typeof(T), true).FirstOrDefault();
        }

        public static bool IsAssignableTo<T>(this Type type)
        {
            return typeof(T).IsAssignableFrom(type);
        }

        public static bool Closes(this Type type, Type genericTypeDefinition)
        {
            if (type.IsGenericTypeDefinition)
                return false;

            Predicate<Type> closes = x => x.IsGenericType && x.GetGenericTypeDefinition().Equals(genericTypeDefinition);

            if (type.GetInterfaces().Any(x => closes(x)))
                return true;

            var baseType = type.BaseType;

            while (baseType != null)
            {
                if (closes(baseType))
                    return true;

                baseType = baseType.BaseType;
            }

            return false;
        }
    }
}