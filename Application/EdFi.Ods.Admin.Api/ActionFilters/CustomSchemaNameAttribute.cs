
namespace EdFi.Ods.Admin.Api.ActionFilters
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class CustomSchemaNameAttribute : Attribute
    {
        public string Name { get; set; }

        public CustomSchemaNameAttribute(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                Name = "Empty schema name";
            }
            else
            {
                Name = name;
            }
        }
    }
}

