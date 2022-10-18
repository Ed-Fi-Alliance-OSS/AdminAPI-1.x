// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace EdFi.Ods.AdminApp.Management.Api.Enumerations
{
    public class AddressType : Enumeration<AddressType>
    {
        public static AddressType Billing = new AddressType(1, "Billing");
        public static AddressType FatherAddress = new AddressType(2, "Father Address");
        public static AddressType Home = new AddressType(3, "Home");
        public static AddressType GuardianAddress = new AddressType(4, "Guardian Address");
        public static AddressType Mailing = new AddressType(5, "Mailing");
        public static AddressType MotherAddress = new AddressType(6, "Mother Address");
        public static AddressType Other = new AddressType(7, "Other");
        public static AddressType Physical = new AddressType(8, "Physical");
        public static AddressType Shipping = new AddressType(9, "Shipping");
        public static AddressType Temporary = new AddressType(10, "Temporary");
        public static AddressType Work = new AddressType(11, "Work");
        public static AddressType Shelter = new AddressType(12, "Shelters, Transitional housing, Awaiting Foster Care");
        public static AddressType DoubledUp = new AddressType(13, "Doubled - up (i.e., living with another family)");
        public static AddressType Unsheltered = new AddressType(14, "Unsheltered (e.g. cars, parks, campgrounds, temporary trailers including FEMA trailers, or abandoned buildings)");
        public static AddressType HotelMotel = new AddressType(15, "Hotels/Motels");

        public AddressType(int value, string displayName) : base(value, displayName)
        {
        }
    }
}