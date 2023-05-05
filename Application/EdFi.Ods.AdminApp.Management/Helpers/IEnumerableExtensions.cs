using EdFi.Admin.DataAccess.Models;
using System.Collections.Generic;
using System.Linq;

namespace EdFi.Ods.AdminApp.Management.Helpers
{
    public static class IEnumerableExtensions
    {
        public static string ToCommaSeparated(this IEnumerable<VendorNamespacePrefix> vendorNamespacePrefixes)
        {
            return vendorNamespacePrefixes != null && vendorNamespacePrefixes.Any()
                            ? ToDelimiterSeparated(vendorNamespacePrefixes.Select(x => x.NamespacePrefix))
                            : string.Empty;
        }

        public static string ToDelimiterSeparated(this IEnumerable<string> inputStrings, string separator = ",")
        {
            var listOfStrings = inputStrings.ToList();

            return listOfStrings.Any()
                ? string.Join(separator, listOfStrings)
                : string.Empty;
        }
    }
}
