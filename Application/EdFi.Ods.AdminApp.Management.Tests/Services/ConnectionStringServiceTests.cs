using System;
using EdFi.Common.Database;
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
        const string TemplateWithServerNamePlaceholder = "Data Source={0};Initial Catalog=EdFi_{0};Integrated Security=True";       

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
        public void Sandbox()
        {
            // When the ODS/API is configured to use Sandbox mode, the intention is to use the Sandbox Admin
            // tool *instead* of Admin App. Still, if an Admin App developer ever bothers to explicitly point at
            // and ODS Database that is in fact a sandbox, the hardcoded connection string should be passed
            // through and used similar to non-template SharedInstance connection strings.

            const string DefaultOdsInstanceName = "EdFi ODS";
            var apiKey = Guid.NewGuid().ToString().Replace("-", "");

            var sandboxConnectionString = "Data Source=.\\;Initial Catalog=EdFi_Ods_"+apiKey+";Integrated Security=True";

            GetConnectionString(sandboxConnectionString, DefaultOdsInstanceName, ApiMode.Sandbox)
                .ShouldBe(sandboxConnectionString);
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

            ambiguousRequest.ShouldThrow<InvalidOperationException>().Message.
                ShouldBe("The database name on the connection string must contain a placeholder {0} for the multi-instance modes to work.");
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

            ambiguousRequest.ShouldThrow<InvalidOperationException>().Message.
                ShouldBe("The database name on the connection string must contain a placeholder {0} for the multi-instance modes to work."); ;
        }

        [Test]
        public void YearSpecificWithDynamicServerName()
        {
            GetConnectionString(TemplateWithServerNamePlaceholder, "EdFi_Ods_2009", ApiMode.YearSpecific)
                .ShouldBe("Data Source=Ods_2009;Initial Catalog=EdFi_Ods_2009;Integrated Security=True");

            GetConnectionString(TemplateWithServerNamePlaceholder, "EdFi_Ods_2010", ApiMode.YearSpecific)
                 .ShouldBe("Data Source=Ods_2010;Initial Catalog=EdFi_Ods_2010;Integrated Security=True");
        }      

        [Test]
        public void DistrictSpecificWithDynamicServerName()
        {
            GetConnectionString(TemplateWithServerNamePlaceholder, "EdFi_Ods_2995001", ApiMode.DistrictSpecific)
                .ShouldBe("Data Source=Ods_2995001;Initial Catalog=EdFi_Ods_2995001;Integrated Security=True");

            GetConnectionString(TemplateWithServerNamePlaceholder, "EdFi_Ods_2995002", ApiMode.DistrictSpecific)
                .ShouldBe("Data Source=Ods_2995002;Initial Catalog=EdFi_Ods_2995002;Integrated Security=True");
        }      

        private string GetConnectionString(string odsConnectionStringTemplate, string odsInstanceName, ApiMode apiMode)
        {
            var mockConnectionStringBuilderAdapterFactory = new Mock<IConnectionStringBuilderAdapterFactory>();
            mockConnectionStringBuilderAdapterFactory.Setup(x => x.Get()).Returns(new SqlConnectionStringBuilderAdapter());
            var service = new ConnectionStringService(GetConnectionStringsAccessor(odsConnectionStringTemplate), mockConnectionStringBuilderAdapterFactory.Object);
            return service.GetConnectionString(odsInstanceName, apiMode);
        }
            

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
