// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using EdFi.Ods.AdminApi.AdminConsole.Helpers;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Models;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Repositories;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using EdFi.Ods.AdminApi.Common.Infrastructure.ErrorHandling;

namespace EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services.Instances.Commands
{
    public interface IEditInstanceCommand
    {
        Task<Instance> Execute(int odsinstanceid, IEditInstanceModel instance);
    }

    public class EditInstanceCommand : IEditInstanceCommand
    {
        private readonly ICommandRepository<Instance> _instanceCommand;
        private readonly IQueriesRepository<Instance> _instanceQuery;
        private readonly IEncryptionService _encryptionService;
        private readonly string _encryptionKey;

        public EditInstanceCommand(ICommandRepository<Instance> instanceCommand, IQueriesRepository<Instance> instanceQuery, IEncryptionKeyResolver encryptionKeyResolver, IEncryptionService encryptionService)
        {
            _instanceCommand = instanceCommand;
            _instanceQuery = instanceQuery;
            _encryptionKey = encryptionKeyResolver.GetEncryptionKey();
            _encryptionService = encryptionService;
        }

        public async Task<Instance> Execute(int odsInstanceId, IEditInstanceModel instance)
        {
            var cleanedDocument = ExpandoObjectHelper.NormalizeExpandoObject(instance.Document);

            var document = JsonConvert.SerializeObject(cleanedDocument, new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver(),
                Converters = new List<JsonConverter> { new ExpandoObjectConverter() },
                Formatting = Formatting.Indented
            });

            JsonNode? jnDocument = JsonNode.Parse(document);

            var clientId = jnDocument!["clientId"]?.AsValue().ToString();
            var clientSecret = jnDocument!["clientSecret"]?.AsValue().ToString();

            var encryptedClientId = string.Empty;
            var encryptedClientSecret = string.Empty;

            if (!string.IsNullOrEmpty(clientId) && !string.IsNullOrEmpty(clientSecret))
            {
                _encryptionService.TryEncrypt(clientId, _encryptionKey, out encryptedClientId);
                _encryptionService.TryEncrypt(clientSecret, _encryptionKey, out encryptedClientSecret);

                jnDocument!["clientId"] = encryptedClientId;
                jnDocument!["clientSecret"] = encryptedClientSecret;
            }

            try
            {
                var existingInstance = await _instanceQuery.Query().SingleOrDefaultAsync(w => w.OdsInstanceId == odsInstanceId) ?? throw new NotFoundException<int>("Instance", odsInstanceId);

                existingInstance.Document = document;
                await _instanceCommand.UpdateAsync(existingInstance);
                return existingInstance;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }

    public interface IEditInstanceModel
    {
        ExpandoObject Document { get; }
    }
}
