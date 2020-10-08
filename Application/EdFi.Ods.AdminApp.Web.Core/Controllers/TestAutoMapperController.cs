// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using EdFi.Ods.AdminApp.Web.Models;

namespace EdFi.Ods.AdminApp.Web.Controllers
{
    public class TestAutoMapperController : Controller
    {
        private readonly IMapper _mapper;

        public TestAutoMapperController(IMapper mapper) => _mapper = mapper;

        public IActionResult Index()
        {
            var sourceModel = new SourceModel
            {
                SourceId = 1,
                SourceString = "SourceString"
            };

            var destinationModel = _mapper.Map<DestinationModel>(sourceModel);

            return View(destinationModel);
        }
    }
}
