// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using AutoMapper;
using EdFi.Ods.AdminApp.Management.Api.Models;
using System.Collections.Generic;
using System.Linq;
using EdFi.Ods.AdminApp.Management.Api.Common;
using EdFi.Ods.AdminApp.Management.Api.Descriptors;
using LocalEducationAgency = EdFi.Ods.AdminApp.Management.Api.DomainModels.EdFiLocalEducationAgency;
using School = EdFi.Ods.AdminApp.Management.Api.DomainModels.EdFiSchool;

namespace EdFi.Ods.AdminApp.Management.Api
{
    public class OdsApiFacade : IOdsApiFacade
    {
        private readonly IMapper _mapper;
        private readonly IOdsRestClient _restClient;

        public OdsApiFacade(IMapper mapper, IOdsRestClient restClient)
        {
            _mapper = mapper;
            _restClient = restClient;
        }

        public List<Models.School> GetAllSchools()
        {
            var response = _restClient.GetAll<School>(ResourcePaths.Schools);
            return _mapper.Map<List<Models.School>>(response);
        }

        public List<SelectOptionModel> GetLocalEducationAgencyCategories()
        {
            var response = _restClient.GetAll<DomainModels.EdFiDescriptor>(ResourcePaths.LocalEducationAgencyCategoryDescriptors);
            return response.Select(x => BuildDescriptorSelectOptionModel(x.Namespace, x.CodeValue, x.Description))
                .ToList();
        }

        public List<Models.LocalEducationAgency> GetAllLocalEducationAgencies()
        {
            var response = _restClient.GetAll<LocalEducationAgency>(ResourcePaths.LocalEducationAgencies);
            return _mapper.Map<List<Models.LocalEducationAgency>>(response);
        }

        public List<Models.LocalEducationAgency> GetLocalEducationAgenciesByPage(int offset, int limit)
        {
            var response = _restClient.GetAll<LocalEducationAgency>(ResourcePaths.LocalEducationAgencies, offset, limit);
            return _mapper.Map<List<Models.LocalEducationAgency>>(response);
        }

        public List<SelectOptionModel> GetAllGradeLevels()
        {
            var response = _restClient.GetAll<DomainModels.EdFiDescriptor>(ResourcePaths.GradeLevelDescriptors);
            return response.OrderBy(x => GradeLevelOrder.GetDefaultSortValue(x.CodeValue))
                .Select(x => BuildDescriptorSelectOptionModel(x.Namespace, x.CodeValue, x.Description))
                .ToList();
        }

        public OdsApiResult AddLocalEducationAgency(Models.LocalEducationAgency newLocalEducationAgency)
        {
             var request = _mapper.Map<LocalEducationAgency>(newLocalEducationAgency);
             return _restClient.PostResource(request, ResourcePaths.LocalEducationAgencies);
        }

        public Models.LocalEducationAgency GetLocalEducationAgencyById(string id)
        {
            var localEducationAgency =
                _restClient.GetById<LocalEducationAgency>(ResourcePaths.LocalEducationAgencyById, id);
            return _mapper.Map<Models.LocalEducationAgency>(localEducationAgency);
        }

        public OdsApiResult EditLocalEducationAgency(Models.LocalEducationAgency model)
        {
            var request = _mapper.Map<LocalEducationAgency>(model);
            return _restClient.PutResource(request, ResourcePaths.LocalEducationAgencies, request.Id);
        }

        public OdsApiResult DeleteLocalEducationAgency(string id)
        {
            return _restClient.DeleteResource(ResourcePaths.LocalEducationAgencyById, id);
        }

        public OdsApiResult DeleteSchool(string id)
        {
            return _restClient.DeleteResource(ResourcePaths.SchoolById, id);
        }

        public OdsApiResult AddSchool(Models.School newSchool)
        {
            var request = _mapper.Map<School>(newSchool);
            return _restClient.PostResource(request, ResourcePaths.Schools);
        }

        public IReadOnlyList<string> GetAllDescriptors()
        {
            return _restClient.GetAllDescriptors();
        }

        public List<Descriptor> GetDescriptorsByPath(string descriptorPath)
        {
            var response = _restClient.GetAll<DomainModels.EdFiDescriptor>(descriptorPath);

            var descriptors = new List<Descriptor>();
            foreach (var descriptor in response)
            {
                descriptors.Add(new Descriptor
                {
                    Id = descriptor.CodeValue,
                    Namespace = descriptor.Namespace,
                    Description = descriptor.Description,
                });
            }
            return descriptors;
        }

        public Models.School GetSchoolById(string id)
        {
            var school = _restClient.GetById<School>(ResourcePaths.SchoolById, id);
            return _mapper.Map<Models.School>(school);
        }

        public OdsApiResult EditSchool(Models.School model)
        {
            var request = _mapper.Map<School>(model);
            return _restClient.PutResource(request, ResourcePaths.Schools, request.Id);
        }

        public bool DoesApiDataExist()
        {
            var response = _restClient.GetAll<DomainModels.EdFiDescriptor>(ResourcePaths.GradeLevelDescriptors);
            return response.Count > 0;
        }

        public bool DoesLearningStandardsDataExist()
        {
            return false;
        }

        public List<SelectOptionModel> GetAllStateAbbreviations()
        {
            var response =
                _restClient.GetAll<DomainModels.EdFiDescriptor>(ResourcePaths.StateAbbreviationDescriptors);
            return response.Select(x => BuildDescriptorSelectOptionModel(x.Namespace, x.CodeValue, x.Description))
                .ToList();
        }

        private static SelectOptionModel BuildDescriptorSelectOptionModel(string nameSpace, string code, string description)
        {
            return new SelectOptionModel
            {
                Value = nameSpace + "#" + code,
                DisplayText = description
            };
        }
    }
}
