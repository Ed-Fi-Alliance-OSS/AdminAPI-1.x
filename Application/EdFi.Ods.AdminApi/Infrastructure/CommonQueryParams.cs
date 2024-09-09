using Microsoft.AspNetCore.Mvc;

namespace EdFi.Ods.AdminApi.Infrastructure
{
    public struct CommonQueryParams
    {
        [FromQuery(Name = "offset")]
        public int? Offset { get; set; }
        [FromQuery(Name = "limit")]
        public int? Limit { get; set; }
        public CommonQueryParams() { }
        public CommonQueryParams(int? offset, int? limit)
        {
            Offset = offset;
            Limit = limit;
        }
    }
}
