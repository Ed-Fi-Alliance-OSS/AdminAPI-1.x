using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace EdFi.Ods.AdminApi.Infrastructure.Documentation;

[AttributeUsage(AttributeTargets.Method)]
public class XmlRequestPayloadAttribute : Attribute
{   
    public XmlRequestPayloadAttribute()
    {
        ParameterName = "payload";
        Required = true;
        MediaType = "application/xml";
        Format = "xml";
    }
    public string Format { get; set; } 
    public string MediaType { get; set; } 
    public bool Required { get; set; }  
    public string ParameterName { get; set; }
}

public class SwaggerXmlRequestPayloadFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var attribute = context.MethodInfo.GetCustomAttributes(typeof(XmlRequestPayloadAttribute), false).FirstOrDefault();
        if (attribute == null)
        {
            return;
        }

        operation.RequestBody = new OpenApiRequestBody() { Required = true };
        var xml = @"<Profile name=""Test-Profile"">
  <Resource name=""Resource1"">
    <ReadContentType memberSelection=""IncludeOnly"">
      <Collection name=""Collection1"" memberSelection=""IncludeOnly"">
        <Property name=""Property1"" />
        <Property name=""Property2"" />
      </Collection>
    </ReadContentType>
    <WriteContentType memberSelection=""IncludeOnly"">
      <Collection name=""Collection2"" memberSelection=""IncludeOnly"">
        <Property name=""Property1"" />
        <Property name=""Property2"" />
      </Collection>
    </WriteContentType>
  </Resource>
</Profile>"; 
        operation.RequestBody.Content.Add("application/text", new OpenApiMediaType()
        {
            Schema = new OpenApiSchema()
            {
                Example = new OpenApiString(xml, true)
            }
        });
    }
}
