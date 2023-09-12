// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


using EdFi.Admin.DataAccess.Models;
using EdFi.Ods.AdminApi.Infrastructure.Database.Commands;
using EdFi.Ods.AdminApi.Infrastructure.ErrorHandling;
using Microsoft.AspNetCore.Builder;
using Moq;
using NUnit.Framework;
using Shouldly;
using System;
using System.Data.Entity.Validation;
using System.Linq;

namespace EdFi.Ods.AdminApi.DBTests.Database.CommandTests;

[TestFixture]
public class AddOdsInstanceDerivativeTests : PlatformUsersContextTestBase
{
    [Test]
    public void ShouldAddOdsInstanceDerivative()
    {
        var odsInstance = new OdsInstance
        {
            Name = "ODS Instance Name",
            InstanceType = "Ods",
            ConnectionString = "Data Source=(local);Initial Catalog=EdFi_Ods;Integrated Security=True;Encrypt=False"
        };

        Save(odsInstance);

        var derivativeType = "ReadReplica";
        var connectionString = "Data Source=(local);Initial Catalog=EdFi_Ods;Integrated Security=True;Encrypt=False";

        var newOdsInstanceDerivative = new Mock<IAddOdsInstanceDerivativeModel>();
        newOdsInstanceDerivative.Setup(x => x.OdsInstanceId).Returns(odsInstance.OdsInstanceId);
        newOdsInstanceDerivative.Setup(x => x.DerivativeType).Returns(derivativeType);
        newOdsInstanceDerivative.Setup(x => x.ConnectionString).Returns(connectionString);

        var id = 0;
        Transaction(usersContext =>
        {
            var command = new AddOdsInstanceDerivativeCommand(usersContext);
            id = command.Execute(newOdsInstanceDerivative.Object).OdsInstanceDerivativeId;
            id.ShouldBeGreaterThan(0);
        });

        Transaction(usersContext =>
        {
            var odsInstanceDerivative = usersContext.OdsInstanceDerivatives.Single(v => v.OdsInstanceDerivativeId == id);
            odsInstanceDerivative.OdsInstanceId.ShouldBe(odsInstance.OdsInstanceId);
            odsInstanceDerivative.DerivativeType.ShouldBe(derivativeType);
            odsInstanceDerivative.ConnectionString.ShouldBe(connectionString);
        });
    }

    [Test]
    public void ShouldFailToAddWhenConnectionStringIsEmpty()
    {
        var odsInstance = new OdsInstance
        {
            Name = "ODS Instance Name",
            InstanceType = "Ods",
            ConnectionString = "Data Source=(local);Initial Catalog=EdFi_Ods;Integrated Security=True;Encrypt=False"
        };

        Save(odsInstance);

        var derivativeType = "ReadReplica";
        var connectionString = string.Empty;

        var newOdsInstanceDerivative = new Mock<IAddOdsInstanceDerivativeModel>();
        newOdsInstanceDerivative.Setup(x => x.OdsInstanceId).Returns(odsInstance.OdsInstanceId);
        newOdsInstanceDerivative.Setup(x => x.DerivativeType).Returns(derivativeType);
        newOdsInstanceDerivative.Setup(x => x.ConnectionString).Returns(connectionString);

        var id = 0;
        Assert.Throws<DbEntityValidationException>(() =>
        {
            Transaction(usersContext =>
            {
                var command = new AddOdsInstanceDerivativeCommand(usersContext);
                id = command.Execute(newOdsInstanceDerivative.Object).OdsInstanceDerivativeId;
            });
        });
        Assert.That(id, Is.EqualTo(0));
    }

    [Test]
    public void ShouldFailToAddWhenDerivativeTypeIsEmpty()
    {
        var odsInstance = new OdsInstance
        {
            Name = "ODS Instance Name",
            InstanceType = "Ods",
            ConnectionString = "Data Source=(local);Initial Catalog=EdFi_Ods;Integrated Security=True;Encrypt=False"
        };

        Save(odsInstance);

        var derivativeType = string.Empty;
        var connectionString = "Data Source=(local);Initial Catalog=EdFi_Ods;Integrated Security=True;Encrypt=False";

        var newOdsInstanceDerivative = new Mock<IAddOdsInstanceDerivativeModel>();
        newOdsInstanceDerivative.Setup(x => x.OdsInstanceId).Returns(odsInstance.OdsInstanceId);
        newOdsInstanceDerivative.Setup(x => x.DerivativeType).Returns(derivativeType);
        newOdsInstanceDerivative.Setup(x => x.ConnectionString).Returns(connectionString);

        var id = 0;
        Assert.Throws<DbEntityValidationException>(() =>
        {
            Transaction(usersContext =>
            {
                var command = new AddOdsInstanceDerivativeCommand(usersContext);
                id = command.Execute(newOdsInstanceDerivative.Object).OdsInstanceDerivativeId;
            });
        });
        Assert.That(id, Is.EqualTo(0));
    }

}


