// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using EdFi.Ods.AdminApp.Management.Instances;

namespace EdFi.Ods.AdminApp.Management.Database.Ods.Reports
{ 
    public interface IReportViewsSetUp
    {
        void CreateReportViews(string odsInstanceName, ApiMode apiMode);
    }

    public class ReportViewsSetUp : IReportViewsSetUp
    {
        private readonly IReportsConfigProvider _reportsConfigProvider;
        private readonly IUpgradeEngineFactory _upgradeEngineFactory;

        public ReportViewsSetUp(IReportsConfigProvider reportsConfigProvider, IUpgradeEngineFactory upgradeEngineFactory)
        {
            _reportsConfigProvider = reportsConfigProvider;
            _upgradeEngineFactory = upgradeEngineFactory;
        }

        public void CreateReportViews(string odsInstanceName, ApiMode apiMode)
        {
            var reportConfig = _reportsConfigProvider.Create(odsInstanceName, apiMode);
            var upgradeEngine = _upgradeEngineFactory.Create(reportConfig);
            var result = upgradeEngine.PerformUpgrade();
            if (result.Successful) return;
            throw new Exception("Error while creating report views.", result.Error);
        }
    }
}
