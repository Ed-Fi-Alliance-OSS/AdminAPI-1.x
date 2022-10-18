// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using EdFi.Ods.AdminApp.Management.Api.DomainModels;
using School = EdFi.Ods.AdminApp.Management.Api.Models.School;

namespace EdFi.Ods.AdminApp.Management.Api.Automapper
{
    public class SchoolGradeLevelResolver : IValueResolver<School, EdFiSchool, List<EdFiSchoolGradeLevel>>
    {
        public List<EdFiSchoolGradeLevel> Resolve(School source, EdFiSchool destination, List<EdFiSchoolGradeLevel> destMember, ResolutionContext context)
        {
            destMember?.Clear(); // by default, automapper combines the source list with the destination list. we want to replace the destination list instead
            return source.GradeLevels?.Select(x => new EdFiSchoolGradeLevel(x)).ToList();
        }

        public static List<EdFiSchoolGradeLevel> Resolve(School source, ResolutionContext context)
        {
            return source.GradeLevels?.Select(x => new EdFiSchoolGradeLevel(x)).ToList();
        }
    }
}
