namespace EdFi.Ods.Admin.Api.ActionFilters
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    internal class OperationDescriptionAttribute : Attribute
    {
        public string Summary { get; set; }

        public string? Description { get; set; }

        public OperationDescriptionAttribute(string summary, string? description)
        {
            Summary = summary;
            Description = description;
        }
    }
}

