using FluentValidation;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;

namespace EdFi.Ods.AdminApi.Features.Profiles
{
    public class ProfileValidator
    {
        public void Validate<T>(string name, string definition, ValidationContext<T> context)
        {
            var schema = new XmlSchemaSet();
            var path = new Uri(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!).LocalPath;
            schema.Add("", $"{path}\\Schema\\Ed-Fi-ODS-API-Profile.xsd");
            var propertyName = "Profile";

            void EventHandler(object? sender, ValidationEventArgs e)
            {
                if (e.Severity == XmlSeverityType.Error)
                {
                    context.AddFailure(propertyName, e.Message);
                }
            }
            try
            {
                var document = new XmlDocument();
                document.LoadXml(definition);
                document.Schemas.Add(schema);
                document.Validate(EventHandler);

                var profile = document.DocumentElement;
                if (profile != null)
                {
                    var profileName = profile.GetAttribute("name");
                    if(!profileName.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                    {
                        context.AddFailure(propertyName, $"Profile name attribute value should match with {name}." );
                    }
                }          
            }
            catch (Exception ex)
            {
                context.AddFailure(propertyName, ex.Message.ToString());
            }
        }
    }
}
