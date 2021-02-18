// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using AutoMapper;
using EdFi.Ods.AdminApp.Management.Api;
using EdFi.Ods.AdminApp.Web.Models.ViewModels.Descriptors;
using System.Collections.Generic;
using System.Threading.Tasks;
using EdFi.Ods.AdminApp.Management;
using EdFi.Ods.AdminApp.Web.Display.TabEnumeration;
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
            var model = new DescriptorCategoriesModel
            {
                DescriptorCategoryPaths = (await _odsApiFacadeFactory.Create()).GetAllDescriptors()
            };

           return PartialView("_DescriptorCategories", model);
        }

        public async Task<ActionResult> GetDescriptorsFromCategoryName(string category)
        {
            var descriptors = (await _odsApiFacadeFactory.Create()).GetDescriptorsByName(category);
            return PartialView("_Descriptor", _mapper.Map<List<DescriptorModel>>(descriptors));
        }
    }
}
