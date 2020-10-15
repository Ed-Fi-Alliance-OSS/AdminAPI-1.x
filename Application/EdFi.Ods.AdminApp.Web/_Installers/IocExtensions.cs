// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

#if NET48
using System;
using Castle.MicroKernel.Registration;
using Castle.Windsor;

namespace EdFi.Ods.AdminApp.Web._Installers
{
    public static class IocExtensions
    {
        public static void AddSingleton<TService>(this IWindsorContainer services, TService implementationInstance)
            where TService : class
        {
            services.Register(
                Component.For<TService>()
                    .Instance(implementationInstance)
                    .LifestyleSingleton());
        }

        public static void AddSingleton<TService, TImplementation>(this IWindsorContainer services)
            where TService : class
            where TImplementation : class, TService
        {
            services.Register(
                Component.For<TService>()
                    .ImplementedBy<TImplementation>()
                    .LifestyleSingleton());
        }

        public static void AddSingleton<TService>(this IWindsorContainer services)
            where TService : class
        {
            services.Register(
                Component.For<TService>()
                    .ImplementedBy<TService>()
                    .LifestyleSingleton());
        }

        public static void AddTransient<TService, TImplementation>(this IWindsorContainer services)
            where TService : class
            where TImplementation : class, TService
        {
            services.Register(
                Component.For<TService>()
                    .ImplementedBy<TImplementation>()
                    .LifestyleTransient());
        }

        public static void AddTransient<TService>(this IWindsorContainer services)
            where TService : class
        {
            services.Register(
                Component.For<TService>()
                    .ImplementedBy<TService>()
                    .LifestyleTransient());
        }

        public static void AddTransient(this IWindsorContainer services, Type serviceType)
        {
            services.Register(
                Component.For(serviceType)
                    .ImplementedBy(serviceType)
                    .LifestyleTransient());
        }

        public static void AddTransient(this IWindsorContainer services, Type serviceType, Type implementationType)
        {
            services.Register(
                Component.For(serviceType)
                    .ImplementedBy(implementationType)
                    .LifestyleTransient());
        }
    }
}
#endif
