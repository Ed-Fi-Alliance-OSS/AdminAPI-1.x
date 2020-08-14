// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using AutoMapper;
using EdFi.Ods.AdminApp.Management.Api;
using EdFi.Ods.AdminApp.Web.Models.ViewModels.Descriptors;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace EdFi.Ods.AdminApp.Web.Controllers
{
    public class DescriptorsController : ControllerBase
    {
        private readonly IOdsApiFacadeFactory _odsApiFacadeFactory;
        private readonly IMapper _mapper;

        public DescriptorsController(IOdsApiFacadeFactory odsApiFacadeFactory
            , IMapper mapper)
        {
            _odsApiFacadeFactory = odsApiFacadeFactory;
            _mapper = mapper;
        }

        public async Task<ActionResult> DescriptorCategoryList()
        {
            var model = new DescriptorCategoriesModel
            {
                DescriptorCategories = (await _odsApiFacadeFactory.Create()).GetAllDescriptors()
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