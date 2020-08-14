// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApp.Management.Api.Common;

namespace EdFi.Ods.AdminApp.Management.Api.DomainModels
{
    public class EdFiEducationOrganizationAddress
    {
        public EdFiEducationOrganizationAddress(
            string addressTypeDescriptor,
            string stateAbbreviationDescriptor,
            string city,
            string postalCode,
            string streetNumberName,
            string apartmentRoomSuiteNumber = null
        )
        {
            var resourceName = GetType().Name;
            AddressTypeDescriptor = addressTypeDescriptor.IsRequired(nameof(addressTypeDescriptor), resourceName);
            StateAbbreviationDescriptor = stateAbbreviationDescriptor.IsRequired(nameof(stateAbbreviationDescriptor), resourceName);
            City = city.IsRequired(nameof(city), resourceName);
            PostalCode = postalCode.IsRequired(nameof(postalCode), resourceName);
            StreetNumberName = streetNumberName.IsRequired(nameof(streetNumberName), resourceName);
            ApartmentRoomSuiteNumber = apartmentRoomSuiteNumber;
        }

        public string AddressTypeDescriptor { get; }
        public string StateAbbreviationDescriptor { get; }
        public string ApartmentRoomSuiteNumber { get;  }
        public string City { get; }
        public string PostalCode { get; }
        public string StreetNumberName { get; }
    }
}