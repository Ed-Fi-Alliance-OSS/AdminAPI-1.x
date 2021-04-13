// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using EdFi.Ods.AdminApp.Management.Database.Models;
using Microsoft.AspNetCore.DataProtection.Repositories;

namespace EdFi.Ods.AdminApp.Management.Database
{
    public class DataProtectionRepository : IXmlRepository
    {
        private readonly AdminAppDbContext _adminAppDbContext;

        public DataProtectionRepository(AdminAppDbContext adminAppDbContext)
        {
            _adminAppDbContext = adminAppDbContext;
        }

        public IReadOnlyCollection<XElement> GetAllElements()
        {
            return _adminAppDbContext.DataProtectionKeys
                .Select(x => XElement.Parse(x.XmlData))
                .ToList();
        }

        public void StoreElement(XElement element, string friendlyName)
        {
            var key = new DataProtectionKey
            {
                XmlData = element.ToString(SaveOptions.DisableFormatting)
            };

            _adminAppDbContext.DataProtectionKeys.Add(key);
            _adminAppDbContext.SaveChanges();
        }
    }
}
