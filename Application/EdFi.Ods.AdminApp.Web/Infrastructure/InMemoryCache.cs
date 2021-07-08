// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Runtime.Caching;
using System.Threading.Tasks;

namespace EdFi.Ods.AdminApp.Web.Infrastructure
{
    public class InMemoryCache
    {
        private static readonly Lazy<InMemoryCache> _instance = new Lazy<InMemoryCache>(() => new InMemoryCache());
        public static InMemoryCache Instance => _instance.Value; 
        

        private InMemoryCache()
        {
        } 

        public T GetOrSet<T>(string cacheKey, Func<T> getItemCallback, int expirationInMinutes = 1440)
        {
            var obj = MemoryCache.Default.Get(cacheKey);
            if (obj is T typedItem)
                return typedItem;

            var item = getItemCallback();
            MemoryCache.Default.Add(cacheKey, item, DateTime.Now.AddMinutes(expirationInMinutes));
            return item;
        }

        public async Task<T> GetOrSet<T>(string cacheKey, Func<Task<T>> getItemCallback, int expirationInMinutes = 1440) where T : class
        {
            var item = MemoryCache.Default.Get(cacheKey) as T;
            if (item != null) return item;

            item = await getItemCallback();
            MemoryCache.Default.Add(cacheKey, item, DateTime.Now.AddMinutes(expirationInMinutes));
            return item;
        }
    }
}
