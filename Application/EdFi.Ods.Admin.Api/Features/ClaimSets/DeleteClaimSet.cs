// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.Admin.Api.Infrastructure;
using EdFi.Ods.AdminApp.Management.ClaimSetEditor;
using EdFi.Ods.AdminApp.Management.ErrorHandling;
using EdFi.Security.DataAccess.Contexts;
using FluentValidation;
using FluentValidation.Results;

namespace EdFi.Ods.Admin.Api.Features.ClaimSets
{
    public class DeleteClaimSet : IFeature
    {
        public void MapEndpoints(IEndpointRouteBuilder endpoints)
        {
            AdminApiEndpointBuilder.MapDelete(endpoints, "/claimsets/{id}", Handle)
                .WithDefaultDescription()
                .WithRouteOptions(b => b.WithResponseCode(200, FeatureConstants.DeletedSuccessResponseDescription))
                .BuildForVersions(AdminApiVersions.V1);
        }

        public Task<IResult> Handle(DeleteClaimSetCommand deleteClaimSetCommand, ISecurityContext context, IGetApplicationsByClaimSetIdQuery getApplications, int id)
        {
            CheckClaimSetExists(id, context);
            CheckAgainstDeletingClaimSetsWithApplications(id, getApplications);

            try
            {
                deleteClaimSetCommand.Execute(new DeleteClaimSetModel { Id = id });
            }
            catch (AdminAppException exception)
            {
                throw new ValidationException(new[] { new ValidationFailure(nameof(id), exception.Message)});
            }

            return Task.FromResult(AdminApiResponse.Deleted("ClaimSet"));
        }

        private void CheckClaimSetExists(int id, ISecurityContext context)
        {
            var claimSetToDelete = context.ClaimSets.SingleOrDefault(x => x.ClaimSetId == id);

            if (claimSetToDelete == null)
            {
                throw new NotFoundException<int>("claimset", id);
            }
        }

        private void CheckAgainstDeletingClaimSetsWithApplications(int id, IGetApplicationsByClaimSetIdQuery getApplications)
        {
            var associatedApplicationsCount = getApplications.Execute(id).Count();
            if (associatedApplicationsCount > 0)
                throw new ValidationException(new[] { new ValidationFailure(nameof(id),
                    $"Cannot delete this claim set. This claim set has {associatedApplicationsCount} associated application(s).") });
        }
    }
}
