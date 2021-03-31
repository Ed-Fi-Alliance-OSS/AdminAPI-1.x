using System.Collections.Generic;
using System.Threading.Tasks;
using EdFi.Ods.AdminApp.Management.Api;
using EdFi.Ods.AdminApp.Management.Helpers;
using GoogleAnalyticsTracker.Simple;
using Microsoft.Extensions.Options;
using static System.Environment;

namespace EdFi.Ods.AdminApp.Management
{
    public interface ITelemetry
    {
        Task Event(string category, string action, string label);
    }

    public class Telemetry : ITelemetry
    {
        private readonly AppSettings _appSettings;
        private const string NotSet = "(not set)";

        private readonly string _measurementId;
        private readonly string _internalVersion;
        private readonly SimpleTrackerEnvironment _environment;

        public Telemetry(IOptions<AppSettings> appSettingsAccessor)
        {
            _appSettings = appSettingsAccessor.Value;
            _measurementId = _appSettings.GoogleAnalyticsMeasurementId;

            _internalVersion = Version.InternalVersion;

            _environment = new SimpleTrackerEnvironment(
                OSVersion.Platform.ToString(),
                OSVersion.Version.ToString(),
                OSVersion.VersionString
            );
        }

        public async Task Event(string category, string action, string label)
        {
            //NOTE: Custom Dimension numbers are meaningful, but defined within Google Analytics.
            var apiVersion = new InferOdsApiVersion().Version(_appSettings.ProductionApiUrl);

            var customDimensions = new Dictionary<int, string>
            {
                [1] = ExplicitWhenNotSet(_internalVersion),
                [2] = ExplicitWhenNotSet(apiVersion)
            };

            using (var tracker = new SimpleTracker(_measurementId, _environment))
                await tracker.TrackEventAsync(
                    ExplicitWhenNotSet(category),
                    ExplicitWhenNotSet(action),
                    ExplicitWhenNotSet(label),
                    customDimensions);
        }

        private static string ExplicitWhenNotSet(string customDimensionValue)
            => string.IsNullOrEmpty(customDimensionValue) ? NotSet : customDimensionValue;
    }
}
