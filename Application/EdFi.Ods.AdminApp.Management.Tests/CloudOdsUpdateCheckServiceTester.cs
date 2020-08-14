// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using NUnit.Framework;
using EdFi.Ods.AdminApp.Management;

namespace EdFi.Ods.AdminApp.Management.Tests
{
    [TestFixture]
    public class CloudOdsUpdateCheckServiceTester
    {
        [TestCase(null, "1.0")]
        [TestCase("1.0", null)]
        [TestCase(null, null)]
        [TestCase("", "1.0")]
        [TestCase("1.0", "")]
        [TestCase("", "")]
        public void VersionInformationIsValid_InvalidVersion_ShouldReturnFalse(string currentVersion, string latestVersion)
        {
            var currentInstanceVersion = GetVersion(currentVersion);
            var latestPublishedVersion = GetVersion(latestVersion);

            var cloudOdsUpdateInfo = new CloudOdsUpdateInfo
            {
                CurrentInstanceVersion = currentInstanceVersion,
                LatestPublishedVersion = latestPublishedVersion
            };

            var result = new CloudOdsUpdateCheckService().VersionInformationIsValid(cloudOdsUpdateInfo);

            Assert.False(result);
        }

        [TestCase("1.0", "1.0")]
        [TestCase("2.0", "2.0")]
        [TestCase("1.0", "2.0")]
        public void VersionInformationIsValid_ValidVersion_ShouldReturnTrue(string currentVersion, string latestVersion)
        {
            var currentInstanceVersion = GetVersion(currentVersion);
            var latestPublishedVersion = GetVersion(latestVersion);

            var cloudOdsUpdateInfo = new CloudOdsUpdateInfo
            {
                CurrentInstanceVersion = currentInstanceVersion,
                LatestPublishedVersion = latestPublishedVersion
            };

            var result = new CloudOdsUpdateCheckService().VersionInformationIsValid(cloudOdsUpdateInfo);

            Assert.True(result);
        }

        [TestCase(null, "1.0")]
        [TestCase("1.0", null)]
        [TestCase(null, null)]
        [TestCase("", "1.0")]
        [TestCase("1.0", "")]
        [TestCase("", "")]
        public void GetUpdateInfo_InvalidVersion_ShouldReturnFalse(string currentVersion, string latestVersion)
        {
            var currentInstanceVersion = GetVersion(currentVersion);
            var latestPublishedVersion = GetVersion(latestVersion);

            var cloudOdsUpdateInfo = new CloudOdsUpdateInfo
            {
                CurrentInstanceVersion = currentInstanceVersion,
                LatestPublishedVersion = latestPublishedVersion
            };

            var result = new CloudOdsUpdateCheckService().UpdateAvailable(cloudOdsUpdateInfo);

            Assert.False(result);
        }

        [TestCase("1.0", "2.0")]
        [TestCase("2.0", "3.0")]
        [TestCase("1.0.1", "1.0.2")]
        [TestCase("1.0.1", "1.1.1")]
        [TestCase("1.0.1", "2.0.1")]
        public void GetUpdateInfo_LowerCurrentVersion_ShouldReturnTrue(string currentVersion, string latestVersion)
        {
            var currentInstanceVersion = GetVersion(currentVersion);
            var latestPublishedVersion = GetVersion(latestVersion);

            var cloudOdsUpdateInfo = new CloudOdsUpdateInfo
            {
                CurrentInstanceVersion = currentInstanceVersion,
                LatestPublishedVersion = latestPublishedVersion
            };

            var result = new CloudOdsUpdateCheckService().UpdateAvailable(cloudOdsUpdateInfo);

            Assert.True(result);
        }

        [TestCase("2.0", "1.0")]
        [TestCase("3.0", "2.0")]
        [TestCase("1.0.2", "1.0.1")]
        [TestCase("1.1.1", "1.0.1")]
        [TestCase("2.0.1", "1.0.1")]
        [TestCase("1.0.1", "1.0.1")]
        [TestCase("2.0", "2.0")]
        public void GetUpdateInfo_HigherCurrentVersion_ShouldReturnFalse(string currentVersion, string latestVersion)
        {
            var currentInstanceVersion = GetVersion(currentVersion);
            var latestPublishedVersion = GetVersion(latestVersion);

            var cloudOdsUpdateInfo = new CloudOdsUpdateInfo
            {
                CurrentInstanceVersion = currentInstanceVersion,
                LatestPublishedVersion = latestPublishedVersion
            };

            var result = new CloudOdsUpdateCheckService().UpdateAvailable(cloudOdsUpdateInfo);

            Assert.False(result);
        }

        [TestCase(null, "1.0")]
        [TestCase("1.0", null)]
        [TestCase(null, null)]
        [TestCase("", "1.0")]
        [TestCase("1.0", "")]
        [TestCase("", "")]
        public void UpdateIsCompatible_InvalidVersion_ShouldReturnFalse(string currentVersion, string latestVersion)
        {
            var currentInstanceVersion = GetVersion(currentVersion);
            var latestPublishedVersion = GetVersion(latestVersion);

            var cloudOdsUpdateInfo = new CloudOdsUpdateInfo
            {
                CurrentInstanceVersion = currentInstanceVersion,
                LatestPublishedVersion = latestPublishedVersion
            };

            var result = new CloudOdsUpdateCheckService().UpdateIsCompatible(cloudOdsUpdateInfo);

            Assert.False(result);
        }

        [TestCase("2.0", "1.0")]
        [TestCase("3.0", "2.0")]
        [TestCase("1.0.2", "1.0.1")]
        [TestCase("1.1.1", "1.0.1")]
        [TestCase("2.0.1", "1.0.1")]
        [TestCase("1.0.1", "1.0.1")]
        [TestCase("2.0", "2.0")]
        public void UpdateIsCompatible_UpdateNotAvilable_ShouldReturnFalse(string currentVersion, string latestVersion)
        {
            var currentInstanceVersion = GetVersion(currentVersion);
            var latestPublishedVersion = GetVersion(latestVersion);

            var cloudOdsUpdateInfo = new CloudOdsUpdateInfo
            {
                CurrentInstanceVersion = currentInstanceVersion,
                LatestPublishedVersion = latestPublishedVersion
            };

            var result = new CloudOdsUpdateCheckService().UpdateIsCompatible(cloudOdsUpdateInfo);

            Assert.False(result);
        }

        [TestCase("1.0", "2.0")]
        [TestCase("2.0", "3.0")]
        [TestCase("1.0.2", "2.0.1")]
        [TestCase("1.1.1", "6.0.1")]
        public void UpdateIsCompatible_DifferentMajorVersion_ShouldReturnFalse(string currentVersion, string latestVersion)
        {
            var currentInstanceVersion = GetVersion(currentVersion);
            var latestPublishedVersion = GetVersion(latestVersion);

            var cloudOdsUpdateInfo = new CloudOdsUpdateInfo
            {
                CurrentInstanceVersion = currentInstanceVersion,
                LatestPublishedVersion = latestPublishedVersion
            };

            var result = new CloudOdsUpdateCheckService().UpdateIsCompatible(cloudOdsUpdateInfo);

            Assert.False(result);
        }

        [TestCase("1.0.1", "1.0.2")]
        [TestCase("1.0.1", "1.1.1")]
        [TestCase("1.0.1", "1.2.1")]
        public void UpdateIsCompatible_SameMajorVersion_ShouldReturnTrue(string currentVersion, string latestVersion)
        {
            var currentInstanceVersion = GetVersion(currentVersion);
            var latestPublishedVersion = GetVersion(latestVersion);

            var cloudOdsUpdateInfo = new CloudOdsUpdateInfo
            {
                CurrentInstanceVersion = currentInstanceVersion,
                LatestPublishedVersion = latestPublishedVersion
            };

            var result = new CloudOdsUpdateCheckService().UpdateIsCompatible(cloudOdsUpdateInfo);

            Assert.True(result);
        }

        private Version GetVersion(string version)
        {
            if (string.IsNullOrWhiteSpace(version))
            {
                return null;
            }
            return new Version(version);
        }
    }
}