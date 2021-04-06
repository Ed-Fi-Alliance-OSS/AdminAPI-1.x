// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using AutoMapper;
using EdFi.Ods.AdminApp.Management.Api;
using EdFi.Ods.AdminApp.Web.Models.ViewModels.Descriptors;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EdFi.Ods.AdminApp.Management;
using EdFi.Ods.AdminApp.Web.ActionFilters;
using EdFi.Ods.AdminApp.Web.Display.TabEnumeration;
using EdFi.Ods.AdminApp.Web.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace EdFi.Ods.AdminApp.Web.Controllers
{
    public class DescriptorsController : ControllerBase
    {
        private readonly IOdsApiFacadeFactory _odsApiFacadeFactory;
        private readonly IMapper _mapper;
        private readonly ITabDisplayService _tabDisplayService;
        private readonly InstanceContext _instanceContext;

        public DescriptorsController(IOdsApiFacadeFactory odsApiFacadeFactory
            , IMapper mapper
            , ITabDisplayService tabDisplayService
            , InstanceContext instanceContext)
        {
            _odsApiFacadeFactory = odsApiFacadeFactory;
            _mapper = mapper;
            _tabDisplayService = tabDisplayService;
            _instanceContext = instanceContext;
        }

        [AddTelemetry("Descriptors Index", TelemetryType.View)]
        public ActionResult Index()
        {
            var model = new DescriptorsIndexModel
            {
                OdsInstanceSettingsTabEnumerations =
                    _tabDisplayService.GetOdsInstanceSettingsTabDisplay(OdsInstanceSettingsTabEnumeration.Descriptors),
                OdsInstance = _instanceContext
            };

            return View(model);
        }

        public async Task<ActionResult> DescriptorCategoryList()
        {
            var descriptorCategoryPaths = (await _odsApiFacadeFactory.Create()).GetAllDescriptors();

            var model = new DescriptorCategoriesModel
            {
                DescriptorCategories = descriptorCategoryPaths.Select(path => new DescriptorCategoriesModel.Category
                {
                    Path = path,
                    Name = path.GetDescriptorCategoryName()
                }).OrderBy(x => x.Name).ToList()
            };

           return PartialView("_DescriptorCategories", model);
        }

        [AddTelemetry("Individual Descriptor", TelemetryType.View)]
        public async Task<ActionResult> GetDescriptorsFromCategory(string categoryPath)
        {
            var descriptors = (await _odsApiFacadeFactory.Create()).GetDescriptorsByPath(categoryPath);
            return PartialView("_Descriptor", _mapper.Map<List<DescriptorModel>>(descriptors));
        }
    }
}
