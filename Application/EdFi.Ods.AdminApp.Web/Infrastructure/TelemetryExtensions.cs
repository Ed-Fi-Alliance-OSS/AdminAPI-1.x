using System.Threading.Tasks;
using EdFi.Ods.AdminApp.Management;

namespace EdFi.Ods.AdminApp.Web.Infrastructure
{
    public static class TelemetryExtensions
    {
        public static async Task View(this ITelemetry telemetry, string item)
            => await telemetry.Event($"View {item}");

        public static async Task Event(this ITelemetry telemetry, string action, string label = null)
            => await telemetry.Event("Admin App Web", action, label);
    }
}
