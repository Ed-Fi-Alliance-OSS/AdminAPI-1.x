using System;
using EdFi.Ods.AdminApp.Management.Helpers;
using EdFi.Ods.AdminApp.Management.Instances;
using EdFi.Ods.AdminApp.Management.Services;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace EdFi.Ods.AdminApp.Management.Tests.Services
{
    public class ConnectionStringServiceTests
    {
        const string Template = "Data Source=.\\;Initial Catalog=EdFi_{0};Integrated Security=True";
        const string FixedTemplate = "Data Source=.\\;Initial Catalog=EdFi_Ods;Integrated Security=True";

        [Test]
        public void SharedInstance()
        {
            const string DefaultOdsInstanceName = "EdFi ODS";
            const string TypicalDefaultValue = "Data Source=.\\;Initial Catalog=EdFi_Ods;Integrated Security=True";
            const string ArbitraryFixedValue = "Data Source=.\\;Initial Catalog=EdFi_Ods_ArbitraryFixedName;Integrated Security=True";

            GetConnectionString(TypicalDefaultValue, DefaultOdsInstanceName, ApiMode.SharedInstance)
                .ShouldBe(TypicalDefaultValue);

            GetConnectionString(ArbitraryFixedValue, DefaultOdsInstanceName, ApiMode.SharedInstance)
                .ShouldBe(ArbitraryFixedValue);

            GetConnectionString(Template, DefaultOdsInstanceName, ApiMode.SharedInstance)
                .ShouldBe(TypicalDefaultValue);
        }

        [Test]
        public void YearSpecific()
        {
            GetConnectionString(Template, "EdFi_Ods_2009", ApiMode.YearSpecific)
                .ShouldBe("Data Source=.\\;Initial Catalog=EdFi_Ods_2009;Integrated Security=True");

           GetConnectionString(Template, "EdFi_Ods_2010", ApiMode.YearSpecific)
                .ShouldBe("Data Source=.\\;Initial Catalog=EdFi_Ods_2010;Integrated Security=True");
        }

        [Test]
        public void YearSpecificFixedValue()
        {
            Action ambiguousRequest = () => GetConnectionString(FixedTemplate, "EdFi_Ods_2009", ApiMode.YearSpecific);

            ambiguousRequest.ShouldThrow<InvalidOperationException>(
                "The connection string must contain a placeholder {0} for the multi-instance modes to work!");
        }

        [Test]
        public void DistrictSpecific()
        {
            GetConnectionString(Template, "EdFi_Ods_2995001", ApiMode.DistrictSpecific)
                .ShouldBe("Data Source=.\\;Initial Catalog=EdFi_Ods_2995001;Integrated Security=True");

            GetConnectionString(Template, "EdFi_Ods_2995002", ApiMode.DistrictSpecific)
                .ShouldBe("Data Source=.\\;Initial Catalog=EdFi_Ods_2995002;Integrated Security=True");
        }

        [Test]
        public void DistrictSpecificFixedValue()
        {
            Action ambiguousRequest = () => GetConnectionString(FixedTemplate, "EdFi_Ods_2995001", ApiMode.DistrictSpecific);

            ambiguousRequest.ShouldThrow<InvalidOperationException>(
                "The connection string must contain a placeholder {0} for the multi-instance modes to work!");
        }

        private string GetConnectionString(string odsConnectionStringTemplate, string odsInstanceName, ApiMode apiMode)
            => new ConnectionStringService(GetConnectionStringsAccessor(odsConnectionStringTemplate))
                .GetConnectionString(odsInstanceName, apiMode);

        private IOptions<ConnectionStrings> GetConnectionStringsAccessor(string productionOdsTemplate)
        {
            var connectionStrings = new Mock<IOptions<ConnectionStrings>>();

            connectionStrings.Setup(x => x.Value).Returns(new ConnectionStrings
            {
                ProductionOds = productionOdsTemplate
            });

            return connectionStrings.Object;
        }
    }
}
