using System;
using EdFi.Ods.AdminApp.Web.Infrastructure;
using NUnit.Framework;
using Shouldly;

namespace EdFi.Ods.AdminApp.Management.Tests.Infrastructure
{
    public class ProductRegistrationModelTests
    {
        [Test]
        public void ShouldRenderConsistentJson()
        {
            // Since this model's JSON representation must be handled by a single
            // production endpoint, we must be sure that there are no breaking
            // changes in the JSON structure from one Admin App release to the
            // next.
            //
            // Any test failure here should be taken with great care. Rather than
            // blindly changing the assertion to start passing, consider whether
            // that would break messages submitted by older Admin App versions
            // still being used in production environments.

            var model = new ProductRegistrationModel
            {
                ProductRegistrationId = "0031cc17-0fe7-4a98-92d8-179065ff35ea",
                OdsApiVersion = "5.2",
                OdsApiMode = "YearSpecific",
                ProductVersion = "2.2.1",
                OsVersion = "Windows 95",
                DatabaseVersion = "Microsoft SQL Server 2019 (RTM-GDR) (KB4583458) - 15.0.2080.9 (X64)   Nov  6 2020 16:50:01   Copyright (C) 2019 Microsoft Corporation  Developer Edition (64-bit) on Windows 10 Pro 10.0 <X64> (Build 19042: ) ",
                HostName = "the-installed-admin-app-hostname.example.com",
                UserId = "347f41d2-2e4b-45c3-b757-44d9545dba4b",
                UserName = "the-logged-in-user@example.com",
                UtcTimeStamp = new DateTime(2021, 8, 23, 21, 33,59, 268, DateTimeKind.Utc)
            };

            model.Serialize().ShouldBe(@"{
  ""ProductRegistrationId"": ""0031cc17-0fe7-4a98-92d8-179065ff35ea"",
  ""OdsApiVersion"": ""5.2"",
  ""OdsApiMode"": ""YearSpecific"",
  ""ProductType"": ""Admin App"",
  ""ProductVersion"": ""2.2.1"",
  ""OSVersion"": ""Windows 95"",
  ""DatabaseVersion"": ""Microsoft SQL Server 2019 (RTM-GDR) (KB4583458) - 15.0.2080.9 (X64)   Nov  6 2020 16:50:01   Copyright (C) 2019 Microsoft Corporation  Developer Edition (64-bit) on Windows 10 Pro 10.0 <X64> (Build 19042: ) "",
  ""HostName"": ""the-installed-admin-app-hostname.example.com"",
  ""UserId"": ""347f41d2-2e4b-45c3-b757-44d9545dba4b"",
  ""UserName"": ""the-logged-in-user@example.com"",
  ""UtcTimeStamp"": ""2021-08-23T21:33:59.268Z""
}");
        }
    }
}
